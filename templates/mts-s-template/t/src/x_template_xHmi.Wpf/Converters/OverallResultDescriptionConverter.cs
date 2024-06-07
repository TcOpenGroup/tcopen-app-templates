using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using TcoInspectors;

namespace x_template_xPlc
{
    public class OverallResultDescriptionConverter : MarkupExtension, IValueConverter
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
                        return "NoAction";
                    case eOverallResult.InProgress:
                        return "InProgress";
                    case eOverallResult.Passed:
                        return "Passed";
                    case eOverallResult.Failed:
                        return "Failed";
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
