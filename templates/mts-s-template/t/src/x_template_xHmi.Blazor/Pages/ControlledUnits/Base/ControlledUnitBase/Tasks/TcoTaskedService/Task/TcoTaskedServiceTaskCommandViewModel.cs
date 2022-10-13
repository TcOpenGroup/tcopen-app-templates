using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vortex.Presentation;

namespace x_template_xPlc
{
    public class TcoTaskedServiceTaskCommandViewModel : RenderableViewModelBase
    {
        public TcoTaskedServiceTaskCommandViewModel()
        {

        }

        TcoTaskedService component;
        public TcoTaskedService Component
        {
            get => component;
            private set
            {
                if (component == value)
                {
                    return;
                }

                SetProperty(ref component, value);
            }
        }

        public override object Model { get => Component; set => Component = (TcoTaskedService)value; }
    }
}