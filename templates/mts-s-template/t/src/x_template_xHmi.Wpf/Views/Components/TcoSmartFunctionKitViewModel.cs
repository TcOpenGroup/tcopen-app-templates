using Vortex.Presentation.Wpf;


namespace x_template_xPlc

{
    public class TcoSmartFunctionKitViewModel   : RenderableViewModel
    {

        public TcoSmartFunctionKitViewModel() 
        {

         
        }
        public TcoSmartFunctionKit Component { get; private set; } 
        public override object Model { get => Component; set { Component = value as TcoSmartFunctionKit; } }

    }
    public class TcoSmartFunctionKitServiceViewModel : TcoSmartFunctionKitViewModel
    {


    }

}