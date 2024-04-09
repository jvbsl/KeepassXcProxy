namespace KeepassXcProxy;

public class KeepassXcGetDatabaseEntries : KeepassXcAction, IActionNamed
{
    static string IActionNamed.ActionName => ActionName;
    public const string ActionName = "get-database-entries";
    public KeepassXcGetDatabaseEntries() : base(ActionName)
    {
    }
}