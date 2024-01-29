

using Vortex.Presentation.Wpf;

namespace x_template_xPlc
{
    public class TcoStandardDoorViewModel
        : RenderableViewModel
    {
        public TcoStandardDoorViewModel() : base()
        {
            Component = new TcoStandardDoor();
        }

        public TcoStandardDoor Component { get; internal set; }

        public override object Model { get => Component; set => Component = value as TcoStandardDoor; }
    }
}
