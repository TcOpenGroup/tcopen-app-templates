using x_template_xHmi.Wpf.Properties;
using x_template_xHmi.Wpf.Views.Data.OfflineReworkData;
using x_template_xHmi.Wpf.Views.Data.ProcessTraceability;

namespace x_template_xHmi.Wpf.DataTraceability
{
    public class DataTraceabilityViewModel : MenuControlViewModel
    {
        public DataTraceabilityViewModel()
        {
            this.Title = strings.ProductionData;
            this.AddCommand(typeof(ProcessTraceabilityView), strings.ProductionData);
            this.AddCommand(typeof(OfflineReworkDataView), strings.ReworkOfflineData);
        }
    }
}
