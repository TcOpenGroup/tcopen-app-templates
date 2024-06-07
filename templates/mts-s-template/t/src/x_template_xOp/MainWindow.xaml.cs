using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using x_template_xPlcConnector;

namespace x_template_xOp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool IsRestarting { get; set; } = false;
        public MainWindow()
        {    

            InitializeComponent();
            if (App.IsDebug /*|| Entry.Settings.DepoyMode == DeployMode.Local*/ || Entry.Settings.DeployMode == DeployMode.Dummy)
            { this.WindowStyle = WindowStyle.SingleBorderWindow; }
            else
                this.WindowStyle = WindowStyle.None;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (!IsRestarting)
            {
                KillEveryInstance();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (NumberOfRunningInstances() > 1)
            {
                KillOtherInstances();
            }
        }
        private void KillEveryInstance() => Process
            .GetProcessesByName(Process.GetCurrentProcess().ProcessName)
            .ToList()
            .ForEach(p => p.Kill());

        private void KillOtherInstances() => Process
                    .GetProcessesByName(Process.GetCurrentProcess().ProcessName)
                    .Where(process => process.Id != Process.GetCurrentProcess().Id)
                    .ToList()
                    .ForEach(p => p.Kill());

        private int NumberOfRunningInstances() => Process
                .GetProcesses()
                .Count(p => p.ProcessName == Process.GetCurrentProcess().ProcessName);
    }
}
