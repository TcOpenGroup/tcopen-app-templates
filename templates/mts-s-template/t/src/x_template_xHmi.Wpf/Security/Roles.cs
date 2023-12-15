using TcOpen.Inxton.Local.Security;
using TcOpen.Inxton.Security;

namespace x_template_xHmi.Wpf
{
    public class Roles
    {
        private Roles()
        {
            SecurityManager.Manager.GetOrCreateRole(new Role(process_settings_access, "Administrator"));
            SecurityManager.Manager.GetOrCreateRole(new Role(rework_settings_access, "Administrator"));
            SecurityManager.Manager.GetOrCreateRole(new Role(process_traceability_access, "Administrator"));
            SecurityManager.Manager.GetOrCreateRole(new Role(instructor_access, "Administrator"));

            SecurityManager.Manager.GetOrCreateRole(new Role(technology_settings_access, "Maintenance"));
            SecurityManager.Manager.GetOrCreateRole(new Role(ground_position_start, "Operator"));
            SecurityManager.Manager.GetOrCreateRole(new Role(automat_start, "Operator"));            
            SecurityManager.Manager.GetOrCreateRole(new Role(manual_start, "Maintenance"));
            SecurityManager.Manager.GetOrCreateRole(new Role(station_details, "Maintenance"));
            SecurityManager.Manager.GetOrCreateRole(new Role(sequencer_step, "Maintenance"));            
            SecurityManager.Manager.GetOrCreateRole(new Role(technology_ground_all, "Maintenance"));
            SecurityManager.Manager.GetOrCreateRole(new Role(technology_automat_all, "Maintenance"));
            SecurityManager.Manager.GetOrCreateRole(new Role(can_user_change_localization, "Operator"));
            SecurityManager.Manager.GetOrCreateRole(new Role(can_user_close_application, "Operator"));
        }
        public const string instructor_access = nameof(instructor_access);
        public const string process_settings_access = nameof(process_settings_access);
        public const string rework_settings_access = nameof(rework_settings_access);
        public const string process_traceability_access = nameof(process_traceability_access);
        public const string can_user_open_technological_settings = nameof(can_user_open_technological_settings);
        public const string ground_position_start = nameof(ground_position_start);
        public const string automat_start = nameof(automat_start);
        public const string station_details = nameof(station_details);
        public const string technology_settings_access = nameof(technology_settings_access);
        public const string manual_start = nameof(manual_start);
        public const string sequencer_step = nameof(sequencer_step);       
        public const string technology_automat_all = nameof(technology_automat_all);
        public const string technology_ground_all = nameof(technology_ground_all);
        public const string can_user_close_application = nameof(can_user_close_application);
        public const string can_user_change_localization = nameof(can_user_change_localization);
        private static Roles _roles;

        public static void Create()
        {
            _roles = new Roles();
        }
        
    }
}
