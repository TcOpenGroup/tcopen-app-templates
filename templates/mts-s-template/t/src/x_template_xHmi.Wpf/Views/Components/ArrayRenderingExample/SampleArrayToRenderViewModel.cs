using x_template_xHmi.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TcoCore;
using TcOpen.Inxton.Local.Security;
using Vortex.Presentation.Wpf;

namespace x_template_xPlc
{
    public class SampleArrayToRenderViewModel : RenderableViewModel
    {
        public SampleArrayToRenderViewModel()
        { }

        public SampleArrayToRender Component { get; private set; } = new SampleArrayToRender();

        public override object Model { get => Component; set { Component = (SampleArrayToRender)value; } }        


    }

    public class SampleArrayToRenderServiceViewModel : SampleArrayToRenderViewModel
    { 
    }

    public class SampleArrayToRenderShadowDisplayViewModel : SampleArrayToRenderViewModel
    {
    }
}
