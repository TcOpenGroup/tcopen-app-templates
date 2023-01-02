using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TcOpen.Inxton.Input;

namespace x_template_xHmi.Wpf
{
    public interface INavigable
    {
        NavCommand AddCommand(Type type, object content = null, object dataConext = null);
        FrameworkElement CurrentView { get; set; }
        Visibility MenuVisibility { get; set; }
        string Title { get; set; }
        RelayCommand ToggleMenuCommand { get; }
        ContentOpeningMode ContentPresentationMode { get; }
        void OpenDefault();
        void OpenCommand(NavCommand navCommand);
    }

    public enum ContentOpeningMode
    {
        Spa,
        ShowWindow,
        ShowDialogWindow
    }
}
