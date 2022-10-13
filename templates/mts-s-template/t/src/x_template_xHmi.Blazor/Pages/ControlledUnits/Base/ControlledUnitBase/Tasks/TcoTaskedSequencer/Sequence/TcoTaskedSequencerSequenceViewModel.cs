using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TcoCore;
using Vortex.Connector;
using Vortex.Presentation;

namespace x_template_xPlc
{
    public class TcoTaskedSequencerSequenceViewModel : RenderableViewModelBase
    {
        public TcoTaskedSequencerSequenceViewModel()
        {

        }

        TcoTaskedSequencer component = new TcoTaskedSequencer();
        public TcoTaskedSequencer Component
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

        public override object Model { get => Component; set => Component = (TcoTaskedSequencer)value; }

        public IEnumerable<object> _parallelTasks = new List<object>();
        public IEnumerable<object> ParallelTasks
        {
            get
            {
                if (Component != null && Component.GetChildren() != null)
                {
                    _parallelTasks = Component.GetChildren<TcoSequencerBase>();
                }

                return _parallelTasks;
            }
        }


        public IVortexElement StepController { get { return this.Component._modeController; } }
    }
}

