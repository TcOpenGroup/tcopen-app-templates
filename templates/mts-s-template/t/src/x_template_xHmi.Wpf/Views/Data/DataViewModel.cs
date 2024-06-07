using x_template_xHmi.Wpf.Properties;
using x_template_xHmi.Wpf.Views.Data.ProcessSettings;
using x_template_xHmi.Wpf.Views.Data.ReworkSettings;
using x_template_xHmi.Wpf.Views.Data.TechnologicalSettings;

using x_template_xHmi.Wpf.Views.Data.OfflineReworkData;

namespace x_template_xHmi.Wpf.Data
{
    public class DataViewModel : MenuControlViewModel
    {
        public DataViewModel()
        {
            this.Title = strings.Settings;
            this.AddCommand(typeof(ProcessSettingsView), strings.ProcessData);
            this.AddCommand(typeof(TechnologicalSettingsView), strings.TechnologicalSettings);
            this.AddCommand(typeof(ReworkSettingsView), strings.ReworkData);
        }
    }
}
