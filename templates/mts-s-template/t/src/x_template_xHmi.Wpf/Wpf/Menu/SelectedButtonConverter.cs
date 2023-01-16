using System;
using System.Globalization;
using System.Windows;
using Vortex.Presentation.Wpf;
using Vortex.Presentation.Wpf.Converters;

namespace x_template_xHmi.Wpf
{
    public class SelectedButtonConverter : BaseMultiConverter
    {
        public override object ToConvert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var bindedCommand = values[0];
            var displayedView = values[1];
            if (displayedView != null && bindedCommand is NavCommand)
            {
                var command = (NavCommand)bindedCommand;
                var currentView = (FrameworkElement)parameter;
                if (command.ViewType == displayedView.GetType())
                    return SelectedTemplate;
                return NotSelectedTemplate;
            }
            return NotSelectedTemplate;
        }

        private object SelectedTemplate { get => Application.Current.FindResource("MenuButtonSelected"); }
        private object NotSelectedTemplate { get => Application.Current.FindResource("MenuButtonNotSelected"); }

    }
}
