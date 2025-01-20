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
    public class TcoPlbPaletizatorViewModel : RenderableViewModel
    {
        public TcoPlbPaletizatorViewModel()
        { }

        public TcoPlbPaletizator Component { get; private set; } = new TcoPlbPaletizator();

        public override object Model { get => Component; set { Component = (TcoPlbPaletizator)value; } }


    }

    public class TcoPlbPaletizatorServiceViewModel : TcoPlbPaletizatorViewModel
    {
    }

}
