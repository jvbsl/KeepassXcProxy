using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Sodium;

namespace KeepassXcProxy;

public class NativeMessageProxy : IDisposable
{
    public static string GetSocketPath()
    {
        string socketName = "org.keepassxc.KeePassXC.BrowserServer";
        string[] socketSearchPaths;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            socketSearchPaths = [socketName + "_" + Environment.UserName];
        }
        else
        {
            socketSearchPaths = new[]
                                {
                                    Path.Combine("/tmp", socketName),
                                    Environment.GetEnvironmentVariable("TMPDIR") is {} tempDir
                                        ? Path.Combine(tempDir, socketName)
                                        : null,
                                    Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR") is {} xdgRuntimeDir
                                        ? Path.Combine(xdgRuntimeDir,
                                            "app/org.keepassxc.KeePassXC", socketName)
                                        : null,
                                    Environment.GetEnvironmentVariable("HOME") is {} homeDir
                                        ? Path.Combine(homeDir,
                                            "snap/keepassxc/common", socketName)
                                        : null
                                }.Where(path => path != null).OfType<string>().ToArray();
        }

        foreach (var socketPath in socketSearchPaths)
        {
            if (File.Exists(socketPath))
            {
                return socketPath;
            }
        }

        throw new Exception(
            $"Error: Could not find any keepassxc socket in the following locations: {string.Join(", ", socketSearchPaths)}");
    }

    public static Stream GetSocketStream()
    {
        var path = GetSocketPath();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WinNamedPipe(path);
        }

        return new UnixSocketStream(path);
    }

    private readonly Stream _stream;
    private readonly JsonSerializerOptions _deserializerOptions;
    private readonly string _clientId;
    private readonly byte[] _nonce;

    private readonly KeyPair _privateKeyPair;
    private byte[]? _publicKey;
    
    public NativeMessageProxy()
    {
        _stream = GetSocketStream();
        _deserializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
                               {
                                    Converters = { new JsonResponseConverter(), new JsonStringConverter<bool>(), new JsonStringConverter<int>() }
                               };

        _clientId = Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
        _privateKeyPair = PublicKeyBox.GenerateKeyPair();
        _nonce = RandomNumberGenerator.GetBytes(24);
    }

    public void Connect()
    {
        var response = Send(new KeepassXcChangePublicKeys(_privateKeyPair.PublicKey, _nonce, _clientId));
        if (response is KeepassXcErrorResponse errorResponse)
        {
            throw new Exception();
        }
        else if (response is KeepassXcChangePublicKeysResponse publicKeysResponse)
        {
            if (!publicKeysResponse.Success)
                throw new Exception();
            _publicKey = publicKeysResponse.PublicKey;
            IncrementNonce(publicKeysResponse.Nonce, _nonce);
        }
        else
        {
            throw new Exception("Unexpected response");
        }
    }
    private static void AddNonce(Span<byte> nonce, int value, Span<byte> nonceOutput)
    {
        if (nonceOutput.Length < nonce.Length)
            throw new ArgumentOutOfRangeException(nameof(nonceOutput));
        if ((new BigInteger(nonce, true, false) + 1).TryWriteBytes(nonceOutput, out var written, true))
        {
            if (written < nonce.Length)
                throw new ArgumentOutOfRangeException(nameof(nonceOutput));
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(nonceOutput));
        }
        // long carry = 0;
        // for (int i = 0; i < nonce.Length; i++)
        // {
        //     var newValue = (long)nonce[i] + value + carry;
        //     if (newValue > 0xFF)
        //     {
        //         carry = Math.DivRem(newValue, 0xFF, out newValue);
        //         newValue = 0;
        //     }
        //     else if (newValue < 0)
        //     {
        //         carry = Math.DivRem(newValue, 0xFF, out var tempNewValue);
        //         newValue = 0xFF + tempNewValue;
        //     }
        //     else
        //     {
        //         carry = 0;
        //     }
        //     nonceOutput[i] = (byte)newValue;
        // }
        // if (carry > 0)
        //    throw new OverflowException();
    }

    private static void IncrementNonce(Span<byte> nonce, Span<byte> nonceOutput)
    {
        AddNonce(nonce, 1, nonceOutput);
    }

    private static void IncrementNonce(Span<byte> nonce)
    {
        IncrementNonce(nonce, nonce);
    }

    public void Dispose()
    {
    }

    public KeepassXcAssociateKey? _association;
    public void Associate()
    {
        var associateKey = RandomNumberGenerator.GetBytes(32);
        var res = Send(new KeepassXcAssociate()
                       { IdKey = associateKey, Key = _privateKeyPair.PublicKey});
        if (res is KeepassXcAssociateResponse associateResponse)
        {
            if (!associateResponse.Success)
                throw new Exception();
            _association = new KeepassXcAssociateKey() { Id = associateResponse.Id, Key = associateKey };
        }
        else if (res is KeepassXcErrorResponse errorResponse)
        {
            throw new Exception();
        }
    }

    public bool TestAssociate()
    {
        if (_association is null)
        {
            return false;
        }
        var res = Send(new KeepassXcTestAssociate()
                       { Id = _association.Id, Key = _association.Key });
        if (res is KeepassXcTestAssociateResponse associateResponse)
        {
            if (!associateResponse.Success)
                throw new Exception();
            return associateResponse.Id.SequenceEqual(_association.Id);
        }
        else if (res is KeepassXcErrorResponse errorResponse)
        {
            throw new Exception();
        }

        return false;
    }

    public KeepassXcBaseResponse SendUnencrypted<T>(T action)
        where T : KeepassXcAction
    {
        JsonSerializer.Serialize(_stream, action);
        return JsonSerializer.Deserialize<KeepassXcBaseResponse>(_stream, _deserializerOptions)
               ?? throw new JsonException();
    }

    public KeepassXcBaseResponse Send(KeepassXcChangePublicKeys action)
    {
        return SendUnencrypted(action);
    }

    public KeepassXcBaseResponse Send<T>(T action, bool? triggerUnlock = null)
        where T : KeepassXcAction, IActionNamed
    {
        var req = KeepassXcEncryptedAction<T>.Create(_clientId, _nonce, _privateKeyPair.PrivateKey, _publicKey, action);
        req.TriggerUnlock = triggerUnlock ?? action.TriggerUnlock;
        var res = SendUnencrypted(req);
        if (res is KeepassXcEncryptedResponse { } encryptedResponse)
        {
            res = encryptedResponse.GetMessage(encryptedResponse.Nonce, _privateKeyPair.PrivateKey, _publicKey);
            IncrementNonce(encryptedResponse.Nonce, _nonce);
        }

        return res;
    }
}

public class UnixSocketStream : Stream
{
    const int NativeMessageMaxLength = 1024 * 1024;
    private readonly NetworkStream _streamImplementation;

    private bool _isFinal = false;
    private readonly Socket socket;

    public UnixSocketStream(string path)
    {
        socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
        socket.ReceiveBufferSize = socket.SendBufferSize = NativeMessageMaxLength;
        //socket.DontFragment = true;
        var endpoint = new UnixDomainSocketEndPoint(path);
        socket.Connect(endpoint);

        _streamImplementation = new NetworkStream(socket, true);
    }
    public override void Flush()
    {
        _streamImplementation.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (!_streamImplementation.DataAvailable && _isFinal)
        {
            _isFinal = false;
            return 0;
        }
        _isFinal = true;
        
        //socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, NativeMessageMaxLength);
        return _streamImplementation.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _streamImplementation.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _streamImplementation.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _streamImplementation.Write(buffer, offset, count);
        _streamImplementation.Flush();
    }

    public override bool CanRead => _streamImplementation.CanRead;

    public override bool CanSeek => _streamImplementation.CanSeek;

    public override bool CanWrite => _streamImplementation.CanWrite;

    public override long Length => _streamImplementation.Length;

    public override long Position
    {
        get => _streamImplementation.Position;
        set => _streamImplementation.Position = value;
    }


    public override void Close()
    {
        base.Close();
        _streamImplementation.Close();
    }
}

class WinNamedPipe : Stream
{
    private readonly NamedPipeClientStream _pipeClient;

    public WinNamedPipe(string address)
    {
        _pipeClient = new NamedPipeClientStream(".", address, PipeDirection.InOut, PipeOptions.None);
        _pipeClient.Connect();
    }

    public override void Close()
    {
        if (_pipeClient.IsConnected)
        {
            _pipeClient.Close();
            _pipeClient.Dispose();
        }
    }

    public override void Flush()
    {
        _pipeClient.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _pipeClient.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _pipeClient.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _pipeClient.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _pipeClient.Write(buffer, offset, count);
    }

    public override bool CanRead => _pipeClient.CanRead;

    public override bool CanSeek => _pipeClient.CanSeek;

    public override bool CanWrite => _pipeClient.CanWrite;

    public override long Length => _pipeClient.Length;

    public override long Position
    {
        get => _pipeClient.Position;
        set => _pipeClient.Position = value;
    }
}