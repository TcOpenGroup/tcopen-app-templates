using TcOpen.Inxton.Local.Security;
using TcOpen.Inxton.Security;

namespace x_template_xPlcConnector
{
    public class DefaultGroups
    {
        private DefaultGroups()
        {
            SecurityManager.RoleGroupManager.CreateGroup(Administrator);
            SecurityManager.RoleGroupManager.CreateGroup(Maintenance);
            SecurityManager.RoleGroupManager.CreateGroup(Operator);

        }
        public const string Administrator = nameof(Administrator);
        public const string Maintenance = nameof(Maintenance);
        public const string Operator = nameof(Operator);

        private static DefaultGroups _groups;

        public static void Create()
        {
            _groups = new DefaultGroups();
        }
        
    }
}
