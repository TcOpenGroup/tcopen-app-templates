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
    public class TcoOscilatingConveyorFeederViewModel : RenderableViewModel
    {
        public TcoOscilatingConveyorFeederViewModel()
        { }

        public TcoOscilatingConveyorFeeder Component { get; private set; } = new TcoOscilatingConveyorFeeder();

        public override object Model { get => Component; set { Component = (TcoOscilatingConveyorFeeder)value; } }


    }

    public class TcoOscilatingConveyorFeederServiceViewModel : TcoOscilatingConveyorFeederViewModel
    {
    }

}
