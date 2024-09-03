using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace x_template_xPlc
{
    public class AlertToBrushConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((bool)value)
            {
                return Application.Current.Resources["Primary"];
            }
            return Application.Current.Resources["Alert"];
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class IconConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch (value)
                {
                    case "controlVoltage": return "Power";
                    case "airPressure": return "OxygenTank";
                    case "emergencyStop": return "Alert";
                    case "safetyDoor": return "ShieldCheck";
                    case "doorClosed": return "Door";
                    case "doorLocked": return "DoorClosedLock";
                    case "processDataLoaded": return "CogOutline";
                    case "technologyDataLoaded": return "ClipboardCheckOutline";
                    case "opticBarrier": return "Hand";
                    case "robot": return "RobotIndustrial";
                    case "plcConnection": return "HeartPulse";
                    case "automatAllowed": return "RunFast";
                    case "none": return "BorderNoneVariant";
                        //default: return value.ToString();
                }
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

    }

    public class TitleToVisibilityCollapsedConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
            {
                return Visibility.Collapsed;

            }
            return Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }


}
