

using Vortex.Presentation.Wpf;

namespace x_template_xPlc
{
    public class TcoDoorViewModel : RenderableViewModel
    {
        public TcoDoorViewModel() : base()
        {
            Component = new TcoDoor();
        }

        public TcoDoor Component { get; internal set; }

        public override object Model { get => Component; set => Component = value as TcoDoor; }
    }
}
