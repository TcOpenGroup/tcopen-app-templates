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
using TcOpen.Inxton.Instructor;

namespace x_template_xInstructor.Configurator
{
    /// <summary>
    /// Interaction logic for InstructionConfiguratorView.xaml
    /// </summary>
    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {   
           
            var selectedItem = (InstructionItem)((Button)e.Source).DataContext;

            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    var ofd = new Microsoft.Win32.OpenFileDialog();
                    ofd.ShowDialog();
                    selectedItem.ContentSource = ofd.FileName;

                });
        }
    }
}