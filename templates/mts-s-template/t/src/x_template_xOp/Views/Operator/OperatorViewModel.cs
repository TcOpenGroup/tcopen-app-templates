using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using x_template_xHmi.Wpf;
using x_template_xInstructor;
using x_template_xPlc;
using x_template_xPlcConnector;
using x_template_xProductionPlaner.Planer.View;
using x_template_xStatistic.Statistics.View;
using x_template_xTagsDictionary.View;

namespace x_template_xOp.Views.Operator
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
                return MessageBox.Show(x_template_xHmi.Wpf.Properties.strings.GroundAllWarning, "Ground", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
            };

            x_template_xPlc.MAIN._technology._automatAllTask.Roles = DefaultRoles.technology_automat_all;
            x_template_xPlc.MAIN._technology._groundAllTask.Roles = DefaultRoles.technology_ground_all;

            ProductionPlanViewModel = new ProductionPlanViewModel(App.ProductionPlaner);
            InstructorViewModel = new InstructorViewModel(App.CuxInstructor);
            InstructorParalellViewModel = new InstructorViewModel(App.CuxParalellInstructor);
            StatisticViewModel = new StatisticsDataViewModel(App.CuxStatistic);
            TagsPairingViewModel = new TagsPairingViewModel(App.CuxTagsPairing);



        }

        public x_template_xPlcTwinController x_template_xPlc { get { return App.x_template_xPlc; } }

        public ProductionPlanViewModel ProductionPlanViewModel { get; private set; }
        public InstructorViewModel InstructorViewModel { get; private set; }
        public InstructorViewModel InstructorParalellViewModel { get; private set; }
        public StatisticsDataViewModel StatisticViewModel { get; private set; }
        public TagsPairingViewModel TagsPairingViewModel { get; private set; }
    }
}
