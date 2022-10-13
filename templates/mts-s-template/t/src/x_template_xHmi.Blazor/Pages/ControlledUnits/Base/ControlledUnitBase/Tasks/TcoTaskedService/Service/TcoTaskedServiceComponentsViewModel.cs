using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vortex.Connector;
using Vortex.Presentation;

namespace x_template_xPlc
{
    public class TcoTaskedServiceComponentsViewModel : RenderableViewModelBase
    {
        public TcoTaskedServiceComponentsViewModel()
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

        public IVortexElement Serviceable { get { return Component.GetParent().GetChildren().Where(p => p is CUComponentsBase).FirstOrDefault(); } }

        public override object Model { get => Component; set => Component = (TcoTaskedService)value; }
    }
}
