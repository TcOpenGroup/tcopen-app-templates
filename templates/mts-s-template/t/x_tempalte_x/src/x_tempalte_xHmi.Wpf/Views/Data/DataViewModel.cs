using x_template_xHmi.Wpf.Properties;
using x_template_xHmi.Wpf.Views.Data.ProcessSettings;
using x_template_xHmi.Wpf.Views.Data.ProcessTraceability;
using x_template_xHmi.Wpf.Views.Data.TechnologicalSettings;
using x_template_xHmi.Wpf.Views.Operator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcOpen.Inxton.Local.Security.Wpf;
using Vortex.Presentation.Wpf;

namespace x_template_xHmi.Wpf.Data
{
    public class DataViewModel : MenuControlViewModel
    {
        public DataViewModel()
        {
            this.Title = strings.Data;
            this.AddCommand(typeof(ProcessSettingsView), strings.ProcessData);
            this.AddCommand(typeof(TechnologicalSettingsView), strings.TechData);
            this.AddCommand(typeof(ProcessTraceabilityView), strings.ProductionData);
        }
    }
}
