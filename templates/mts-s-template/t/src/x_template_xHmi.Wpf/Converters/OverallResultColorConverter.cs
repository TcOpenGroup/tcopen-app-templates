using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using TcoCore.Wpf;
using TcoInspectors;

namespace x_template_xPlc
{
    public class OverallResultColorConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var result = (eOverallResult)Enum.ToObject(typeof(eOverallResult), value);
                //(VortexCore.enumCheckResult)((int)value);

                switch (result)
                {
                    case eOverallResult.NoAction:
                        return Brushes.Gray;
                    case eOverallResult.InProgress:
                        return TcoColors.Accent;
                    case eOverallResult.Passed:
                        return Brushes.Green;
                    case eOverallResult.Failed:
                        return Brushes.Red;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                // swallow
            }

            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
