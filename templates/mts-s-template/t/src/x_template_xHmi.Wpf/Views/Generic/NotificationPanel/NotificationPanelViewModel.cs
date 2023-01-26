using System.Windows;
using Vortex.Presentation.Wpf;
using x_template_xPlc;

namespace x_template_xPlc
{
    public class NotificationPanelViewModel : RenderableViewModel
    {
        private Visibility itemVisibility;

        public NotificationPanelViewModel() : base()
        {
           
        }

        public NotificationPanel Component { get; private set; } = new NotificationPanel();

      

        public override object Model { get => Component; set { Component = (NotificationPanel)value;} }

     
    }


    public class NotificationPanelManualViewModel : NotificationPanelViewModel
    {
        public NotificationPanelManualViewModel() : base()
        {

        }
    }
}