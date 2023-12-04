using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcOpen.Inxton.Local.Security.Wpf;
using x_template_xHmi.Wpf;
using x_template_xHmi.Wpf.Views.Data.OfflineReworkData;
using x_template_xHmi.Wpf.Views.Data.ProcessSettings;
using x_template_xHmi.Wpf.Views.Data.ProcessTraceability;
using x_template_xHmi.Wpf.Views.Data.ReworkSettings;
using x_template_xHmi.Wpf.Views.Data.TechnologicalSettings;
using x_template_xOp.Properties;


namespace x_template_xOp.Data
{
    public class DataViewModel : MenuControlViewModel
    {
        public DataViewModel()
        {
            this.Title = strings.Data;
            this.AddCommand(typeof(ProcessSettingsView), strings.ProcessData);
            this.AddCommand(typeof(TechnologicalSettingsView), strings.TechData);
            this.AddCommand(typeof(ReworkSettingsView), strings.ReworkData);
            this.AddCommand(typeof(OfflineReworkDataView), strings.ReworkOfflineData);
            this.AddCommand(typeof(ProcessTraceabilityView), strings.ProductionData);
        }
    }
}
