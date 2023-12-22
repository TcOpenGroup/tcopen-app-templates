using Vortex.Presentation.Wpf;

namespace x_template_xPlc
{
    public class TcoControlledZoneViewModel : RenderableViewModel
    {
        public TcoControlledZoneViewModel() : base()
        {
            Component = new TcoControlledZone();
        }

        public TcoControlledZone Component { get; internal set; }

        public override object Model { get => Component; set => Component = value as TcoControlledZone; }
    }

    public class TcoControlledZoneSpotViewModel : TcoControlledZoneViewModel
    { 
    }
}
