namespace KeepassXcProxy;

public class KeepassXcGetDatabaseGroups : KeepassXcAction, IActionNamed
{
    static string IActionNamed.ActionName => ActionName;
    public const string ActionName = "get-database-groups";
    public KeepassXcGetDatabaseGroups() : base(ActionName)
    {
    }
}