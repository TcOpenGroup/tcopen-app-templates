using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace x_template_xPlc
{
    public partial class NotificationPanelView : UserControl
    {
        public NotificationPanelView()
        {
            InitializeComponent();
        }




        public NotificationPanel ModelObject
        {
            get { return (NotificationPanel)GetValue(ModelObjectProperty); }
            set { SetValue(ModelObjectProperty, value); }
        }

        public static readonly DependencyProperty ModelObjectProperty =
            DependencyProperty.Register("ModelObject", typeof(NotificationPanel), typeof(NotificationPanelView),
                new PropertyMetadata(default(NotificationPanel), new PropertyChangedCallback(ModelChanged)));

        private static void ModelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var uc = ((NotificationPanelView)obj);

            if (uc.ModelObject != null)
            {
                uc.DataContext = new NotificationPanelViewModel() { Model = uc.ModelObject };
            }
        }

          }




}
