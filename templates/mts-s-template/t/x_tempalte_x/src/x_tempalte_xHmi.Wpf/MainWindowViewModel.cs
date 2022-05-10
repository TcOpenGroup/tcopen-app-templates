using x_template_xPlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcoCore;

namespace x_template_xHmi.Wpf
{
    public class MainWindowViewModel
    {
        public x_template_xPlcTwinController x_template_xPlc { get { return App.x_template_xPlc; } }        
    }
}
