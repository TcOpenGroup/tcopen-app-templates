using System;
using System.Windows.Controls;


namespace x_template_xHmi.Wpf
{
    /// <summary>
    /// Interaction logic for SystemMonitor.xaml
    /// </summary>
    public partial class SystemMonitor : UserControl
    {
        public SystemMonitor()
        {
            InitializeComponent();
            Console.SetOut(SystemDiagnosticsSingleton.Instance.ConsoleWriter);
        }
    }
}
