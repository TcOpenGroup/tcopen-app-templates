using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace x_template_xReworkInstructor.Instructor.View
{
    /// <summary>
    /// Interaction logic for InstructionConfiguratorView.xaml
    /// </summary>
    public partial class InstructionConfiguratorView : UserControl
    {
        public InstructionConfiguratorView()
        {
            InitializeComponent();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (ReworkInstructionItem)((Button)e.Source).DataContext;
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    var ofd = new Microsoft.Win32.OpenFileDialog();
                    ofd.ShowDialog();
                    selectedItem.ContentSource = ofd.FileName;

                });
        }
    }
    //class StatusToBackgroundConverter : BaseConverter
    //{
    //    public override object ToConvert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        try
    //        {
    //            var result = (InstructionItemStatus)Enum.ToObject(typeof(InstructionItemStatus), value);

    //            switch (result)
    //            {
    //                case InstructionItemStatus.Deleted:
    //                    return Application.Current.Resources["Alert"];
    //                case InstructionItemStatus.Active:
    //                    return Application.Current.Resources["Accent"];
    //                default:
    //                    return Application.Current.Resources["Surface"];

    //            }

    //        }
    //        catch (Exception)
    //        {

    //            // swallow
    //        }

    //        return Brushes.LightGray;

    //    }
    //}
    //class StatusToForegroundConverter : BaseConverter
    //{
    //    public override object ToConvert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        try
    //        {
    //            var result = (InstructionItemStatus)Enum.ToObject(typeof(InstructionItemStatus), value);

    //            switch (result)
    //            {
    //                case InstructionItemStatus.Deleted:
    //                    return Application.Current.Resources["OnAlert"];
    //                case InstructionItemStatus.Active:
    //                    return Application.Current.Resources["OnAccent"];
    //                default:
    //                    return Application.Current.Resources["OnSurface"];

    //            }

    //        }
    //        catch (Exception)
    //        {

    //            // swallow
    //        }

    //        return Brushes.LightGray;

    //    }
    //}
   
}