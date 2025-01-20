using System;
using System.Collections.Generic;
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
using TcOpen.Inxton.TcoCore.Wpf;
using TcoCore;

namespace x_template_xPlc
{
    /// <summary>
    /// Interaction logic for CUBaseControlView.xaml
    /// </summary>
    public partial class CUBaseDataView : UserControl
    {
        public CUBaseDataView()
        {
            InitializeComponent();

        }

        private System.Timers.Timer messageUpdateTimer;
        private void SetTimer()
        {
            if (messageUpdateTimer == null)
            {
                messageUpdateTimer = new System.Timers.Timer(2000);
                messageUpdateTimer.Elapsed += UpdateDataTimer_Elapsed;
                messageUpdateTimer.AutoReset = true;
                messageUpdateTimer.Enabled = true;
            }
        }

        private void DisposeTimer()
        {
            if (messageUpdateTimer != null)
            {
                messageUpdateTimer.Stop();
                messageUpdateTimer.Elapsed -= UpdateDataTimer_Elapsed;
                messageUpdateTimer.Dispose();
                messageUpdateTimer = null;
            }
        }

        private ProcessData ProcessData { get; set; }
        private dynamic CuProcessData { get; set; }
        private dynamic CuTechnologyData { get; set; }
        private int c = 0;
        private void UpdateDataTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var isInSight = false;
            CUBaseViewModel context = null;
            TcOpen.Inxton.TcoAppDomain.Current.Dispatcher.Invoke(() =>
            {
                context = this.DataContext as CUBaseViewModel;
                isInSight = UIElementAccessibilityHelper.IsInSight<Grid>(this.Element, this);
            });

            if (isInSight)
            {
                if (CuProcessData == null) { CuProcessData = context?.Component.GetChildren<CUProcessDataBase>().FirstOrDefault(); }
                CuProcessData.FlushOnlineToShadow();

            }
        }

        private void LoadTechnologyDataButton_Click(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as CUBaseViewModel;
            if (CuTechnologyData == null)
            {
                CuTechnologyData = context?.Component.GetChildren<CUTechnologicalDataBase>().FirstOrDefault();
            }

            CuTechnologyData.FlushOnlineToShadow();
        }

        private void LoadFullProcessDataButton_Click(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as CUBaseViewModel;
            if (ProcessData == null) { ProcessData = context?.Component.GetDescendants<ProcessData>().FirstOrDefault(); }
            ProcessData.FlushOnlineToShadow();
        }



        private void CyclicalyLoadingOnlineData_Checked(object sender, RoutedEventArgs e)
        {
            SetTimer();
        }

        private void CyclicalyLoadingOnlineData_Unchecked(object sender, RoutedEventArgs e)
        {
            DisposeTimer();
        }
    }
}
