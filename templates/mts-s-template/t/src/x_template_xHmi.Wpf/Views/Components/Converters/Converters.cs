using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace x_template_xPlc.Converters
{
    public class TcoSmartFunctionKitVisibilityDefaultConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            eTcoSmartFunctionKitCommand command = (eTcoSmartFunctionKitCommand)Enum.ToObject(typeof(eCUMode), value);

            switch (command)
            {
                case eTcoSmartFunctionKitCommand.SetSystemVariable:
                case eTcoSmartFunctionKitCommand.LockParticipant:
                case eTcoSmartFunctionKitCommand.ReadSystemVariable:
                    return Visibility.Visible;
                case eTcoSmartFunctionKitCommand.StartProgram:
                case eTcoSmartFunctionKitCommand.Positioning:
                case eTcoSmartFunctionKitCommand.Jog:
                case eTcoSmartFunctionKitCommand.SetProgramActive:
                case eTcoSmartFunctionKitCommand.Tare:
                case eTcoSmartFunctionKitCommand.ClearError:
                case eTcoSmartFunctionKitCommand.StopMovement:
                case eTcoSmartFunctionKitCommand.RestartDrive:
                case eTcoSmartFunctionKitCommand.StartHoming:
                case eTcoSmartFunctionKitCommand.SetReference:
                    return Visibility.Collapsed;
                default:
                    return Visibility.Collapsed;
                    break;
            }
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
    public class TcoSmartFunctionKitVisibilityStartProgramConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            eTcoSmartFunctionKitCommand command = (eTcoSmartFunctionKitCommand)Enum.ToObject(typeof(eCUMode), value);

            switch (command)
            {
                case eTcoSmartFunctionKitCommand.StartProgram:
                    return Visibility.Visible;
                case eTcoSmartFunctionKitCommand.Positioning:
                case eTcoSmartFunctionKitCommand.Jog:
                case eTcoSmartFunctionKitCommand.SetProgramActive:
                case eTcoSmartFunctionKitCommand.Tare:
                case eTcoSmartFunctionKitCommand.ClearError:
                case eTcoSmartFunctionKitCommand.StopMovement:
                case eTcoSmartFunctionKitCommand.RestartDrive:
                case eTcoSmartFunctionKitCommand.StartHoming:
                case eTcoSmartFunctionKitCommand.SetReference:
                case eTcoSmartFunctionKitCommand.SetSystemVariable:
                case eTcoSmartFunctionKitCommand.LockParticipant:
                case eTcoSmartFunctionKitCommand.ReadSystemVariable:
                    return Visibility.Collapsed;
                default:
                    return Visibility.Collapsed;
                    break;
            }
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

    public class TcoSmartFunctionKitVisibilityJogTaraPositioningConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            eTcoSmartFunctionKitCommand command = (eTcoSmartFunctionKitCommand)Enum.ToObject(typeof(eCUMode), value);

            switch (command)
            {
                case eTcoSmartFunctionKitCommand.StartProgram:
                case eTcoSmartFunctionKitCommand.ClearError:
                case eTcoSmartFunctionKitCommand.StopMovement:
                case eTcoSmartFunctionKitCommand.RestartDrive:
                case eTcoSmartFunctionKitCommand.StartHoming:
                case eTcoSmartFunctionKitCommand.SetSystemVariable:
                case eTcoSmartFunctionKitCommand.LockParticipant:
                case eTcoSmartFunctionKitCommand.ReadSystemVariable:
                case eTcoSmartFunctionKitCommand.SetProgramActive:
                case eTcoSmartFunctionKitCommand.SetReference:
                    return Visibility.Collapsed;
                case eTcoSmartFunctionKitCommand.Tare:
                case eTcoSmartFunctionKitCommand.Positioning:
                case eTcoSmartFunctionKitCommand.Jog:
                    return Visibility.Visible;

                default:
                    return Visibility.Collapsed;
   
            }
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

    public class TcoSmartFunctionKitVisibilitySetProgramConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            eTcoSmartFunctionKitCommand command = (eTcoSmartFunctionKitCommand)Enum.ToObject(typeof(eCUMode), value);

            switch (command)
            {
                case eTcoSmartFunctionKitCommand.StartProgram:
                case eTcoSmartFunctionKitCommand.ClearError:
                case eTcoSmartFunctionKitCommand.StopMovement:
                case eTcoSmartFunctionKitCommand.RestartDrive:
                case eTcoSmartFunctionKitCommand.StartHoming:
                case eTcoSmartFunctionKitCommand.SetSystemVariable:
                case eTcoSmartFunctionKitCommand.LockParticipant:
                case eTcoSmartFunctionKitCommand.ReadSystemVariable:
                case eTcoSmartFunctionKitCommand.Tare:
                case eTcoSmartFunctionKitCommand.SetReference:
                case eTcoSmartFunctionKitCommand.Positioning:
                case eTcoSmartFunctionKitCommand.Jog:
                    return Visibility.Collapsed;
                case eTcoSmartFunctionKitCommand.SetProgramActive:
                    return Visibility.Visible;

                default:
                    return Visibility.Collapsed;
            
            }
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
