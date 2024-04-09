namespace KeepassXcProxy;

public class KeepassXcGetDatabaseHash : KeepassXcAction, IActionNamed
{
    static string IActionNamed.ActionName => ActionName;
    public const string ActionName = "get-databasehash";
    public KeepassXcGetDatabaseHash()
        : base(ActionName)
    {
    }
}