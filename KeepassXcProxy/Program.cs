using System.Runtime.InteropServices;
using System.Text.Json;

namespace KeepassXcProxy
{
    class Program
    {
        private static NativeMessagingConfig GetConfig()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                
            }
            else
            {
                string[] possibleBrowserNativeMessageClients = [
                    ".mozilla/native-messaging-hosts",
                    ".config/google-chrome/NativeMessagingHosts",
                    ".config/chromium/NativeMessagingHosts",
                    ".config/vivaldi/NativeMessagingHosts",
                    ".config/BraveSoftware/Brave-Browser/NativeMessagingHosts",
                    ".tor-browser/app/Browser/TorBrowser/Data/Browser/.mozilla/native-messaging-hosts",
                    ".config/microsoft-edge/NativeMessagingHosts"
                ];
                NativeMessagingConfig config;
                foreach (var possibleBrowserNMC in possibleBrowserNativeMessageClients)
                {
                    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        possibleBrowserNMC, "org.keepassxc.keepassxc_browser.json");
                    if (File.Exists(path))
                    {
                        try
                        {
                            using var str = File.OpenRead(path);
                            return JsonSerializer.Deserialize<NativeMessagingConfig>(str);
                        }
                        catch (Exception ex)
                            when (ex is JsonException or UnauthorizedAccessException or IOException)
                        {
                        
                        }
                    }
                }

                throw new FileNotFoundException();
            }

            throw new NotSupportedException();
        }
        public static void Main(string[] args)
        {
            var p = new NativeMessageProxy();
            
            p.Connect();

            p.Associate();

            var test = p.TestAssociate();
            
            
            var res = p.Send(new KeepassXcGetLogins() { Keys = [ p._association! ], Url = "https://hans.peter"}, true);
            ;
        }
    }
}
