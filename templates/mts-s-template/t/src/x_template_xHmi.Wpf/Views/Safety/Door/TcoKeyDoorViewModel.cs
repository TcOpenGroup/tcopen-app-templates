using Vortex.Presentation.Wpf;

namespace x_template_xPlc
{
    public class TcoKeyDoorViewModel : RenderableViewModel
    {
        public TcoKeyDoorViewModel() : base()
        {
            Component = new TcoKeyDoor();
        }

        public TcoKeyDoor Component { get; internal set; }

        public override object Model { get => Component; set => Component = value as TcoKeyDoor; }
    }
}
