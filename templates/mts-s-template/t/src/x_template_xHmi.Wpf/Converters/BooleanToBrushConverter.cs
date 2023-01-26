
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;


namespace x_template_xPlc
{
    public class BooleanToBrushConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           

            if ((bool)value)
            {
                return  Application.Current.Resources["Accent"];//Vortex.Presentation.Styling.Wpf.VortexResources.SignalOn;
            }
            return   Application.Current.Resources["Alert"];//Vortex.Presentation.Styling.Wpf.VortexResources.Alert;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
