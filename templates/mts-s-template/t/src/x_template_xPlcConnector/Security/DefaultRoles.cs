using System.Collections.Generic;
using TcOpen.Inxton.Local.Security;
using TcOpen.Inxton.Security;

namespace x_template_xPlcConnector
{
    public class DefaultRoles
    {
        private DefaultRoles()
        { 
          

            SecurityManager.RoleGroupManager.AddRolesToGroup(DefaultGroups.Administrator, 
                new string[] { "Administrator",
                    process_settings_access, process_settings_change,rework_settings_access,process_traceability_access,technology_settings_access,
                    instructor_access,planer_access,statistic_access,
                    technology_automat_all,technology_ground_all,
                    ground_position_start,automat_start,manual_start,
                    station_details,sequencer_step,
                    can_override_inspection,can_change_hw_settings,
                    can_user_close_application,can_user_change_localization});

            SecurityManager.RoleGroupManager.AddRolesToGroup(DefaultGroups.Maintenance,
               new string[] {
                    //process_settings_access, process_settings_change,rework_settings_access,process_traceability_access,technology_settings_access,
                    //instructor_access,planer_access,statistic_access,
                    technology_automat_all,technology_ground_all,
                    ground_position_start,automat_start,manual_start,
                    station_details,sequencer_step,
                    can_override_inspection,can_change_hw_settings,
                    can_user_close_application,can_user_change_localization});
            SecurityManager.RoleGroupManager.AddRolesToGroup(DefaultGroups.Operator,
              new string[] {
                    //process_settings_access, process_settings_change,rework_settings_access,process_traceability_access,technology_settings_access,
                    //instructor_access,planer_access,statistic_access,
                    //technology_automat_all,technology_ground_all,
                    //ground_position_start,automat_start,manual_start,
                    //station_details,sequencer_step,
                    //can_override_inspection,can_change_hw_settings,
                    can_user_close_application,can_user_change_localization});
        }
        public const string process_settings_access = nameof(process_settings_access);
        public const string process_settings_change = nameof(process_settings_change);
        public const string rework_settings_access = nameof(rework_settings_access);
        public const string process_traceability_access = nameof(process_traceability_access);
        public const string technology_settings_access = nameof(technology_settings_access);

        public const string instructor_access = nameof(instructor_access);
        public const string planer_access = nameof(planer_access);
        public const string statistic_access = nameof(planer_access);


        public const string technology_automat_all = nameof(technology_automat_all);
        public const string technology_ground_all = nameof(technology_ground_all);

        public const string ground_position_start = nameof(ground_position_start);
        public const string automat_start = nameof(automat_start);
        public const string manual_start = nameof(manual_start);

        public const string station_details = nameof(station_details);
        public const string sequencer_step = nameof(sequencer_step);

        public const string can_override_inspection = nameof(can_override_inspection);
        public const string can_change_hw_settings = nameof(can_change_hw_settings);

        public const string can_user_close_application = nameof(can_user_close_application);
        public const string can_user_change_localization = nameof(can_user_change_localization);

        private static DefaultRoles _roles;
        private string roles;
        private List<string> _rolesCollection;

        public static void Create()
        {
            _roles = new DefaultRoles();
        }
        
    }
}
