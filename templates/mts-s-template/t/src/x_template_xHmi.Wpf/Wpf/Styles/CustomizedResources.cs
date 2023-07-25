using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace x_template_xHmi.Wpf.Wpf.Styles
{
    public class CustomizedResources : ResourceDictionary
    {
        public CustomizedResources()
        {
            UseDefaultGroupBox();
        }

        private void UseDefaultGroupBox()
        {
            var groupbox = new GroupBox();
            var defualtStyle = new Style(typeof(GroupBox), groupbox.Style);
            Add(typeof(GroupBox), defualtStyle);
        }

    }
}
