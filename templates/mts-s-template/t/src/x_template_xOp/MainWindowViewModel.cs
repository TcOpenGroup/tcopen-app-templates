using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcoCore;
using x_template_xPlc;


namespace x_template_xOp
{
    public class MainWindowViewModel
    {
        public x_template_xPlcTwinController x_template_xPlc { get { return App.x_template_xPlc; } }
    }
}
