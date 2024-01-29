
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcOpen.Inxton.Local.Security.Wpf;
using Vortex.Presentation.Wpf;
using x_template_xPlc;

namespace x_template_xHmi.Wpf.Views
{
    public class TechnologyViewModel 
    {
        public TechnologyViewModel()
        {
            
        }

        public x_template_xPlcTwinController x_template_xPlc { get { return App.x_template_xPlc; } }

    }
}
