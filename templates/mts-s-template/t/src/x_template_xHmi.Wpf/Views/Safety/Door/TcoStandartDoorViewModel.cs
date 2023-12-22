

using Vortex.Presentation.Wpf;

namespace x_template_xPlc
{
    public class TcoStandartDoorViewModel : RenderableViewModel
    {
        public TcoStandartDoorViewModel() : base()
        {
            Component = new TcoStandartDoor();
        }

        public TcoStandartDoor Component { get; internal set; }

        public override object Model { get => Component; set => Component = value as TcoStandartDoor; }
    }
}
