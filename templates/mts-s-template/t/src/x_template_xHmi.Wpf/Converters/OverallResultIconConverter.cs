using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using TcoInspectors;

namespace x_template_xPlc
{
    public class OverallResultIconConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var result = (eOverallResult)Enum.ToObject(typeof(eOverallResult), value);

                switch (result)
                {
                    case eOverallResult.NoAction:
                        return MaterialDesignThemes.Wpf.PackIconKind.Minus;
                    case eOverallResult.InProgress:
                        return MaterialDesignThemes.Wpf.PackIconKind.Run;
                    case eOverallResult.Passed:
                        return MaterialDesignThemes.Wpf.PackIconKind.Check;
                    case eOverallResult.Failed:
                        return MaterialDesignThemes.Wpf.PackIconKind.AlphabetXCircle;
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
