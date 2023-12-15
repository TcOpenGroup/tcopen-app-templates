
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;

using x_template_xHmi.Wpf.Properties;

namespace x_template_xHmi.Wpf
{

    public partial class ShutdownView : Window
    {
        public ShutdownView()
        {
          
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}