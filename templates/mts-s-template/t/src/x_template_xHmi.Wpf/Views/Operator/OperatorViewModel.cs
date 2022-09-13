using x_template_xPlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using x_template_xProductionPlaner.Planer.View;

namespace x_template_xHmi.Wpf.Views.Operator
{
    public class OperatorViewModel
    {
        public OperatorViewModel()
        {
            x_template_xPlc.MAIN._technology._automatAllTask.ExecuteDialog = () => 
            {
                return MessageBox.Show(x_template_xHmi.Wpf.Properties.strings.AutomatAllWarning, "Automat", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;                    
            };

            x_template_xPlc.MAIN._technology._groundAllTask.ExecuteDialog = () =>
            {
                return MessageBox.Show(x_template_xHmi.Wpf.Properties.strings.GroundAllWarning, "Automat", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
            };

            x_template_xPlc.MAIN._technology._automatAllTask.Roles = Roles.technology_automat_all;
            x_template_xPlc.MAIN._technology._groundAllTask.Roles = Roles.technology_ground_all;

            ProductionPlanViewModel = new ProductionPlanViewModel(App.ProductonPlaner);
        }

        public x_template_xPlcTwinController x_template_xPlc { get { return App.x_template_xPlc; } }

        public ProductionPlanViewModel ProductionPlanViewModel { get; private set; }
    }
}
