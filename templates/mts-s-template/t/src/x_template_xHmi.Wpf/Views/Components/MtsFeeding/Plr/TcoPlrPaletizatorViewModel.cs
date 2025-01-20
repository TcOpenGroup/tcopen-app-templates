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
    public class TcoPlrPaletizatorViewModel : RenderableViewModel
    {
        public TcoPlrPaletizatorViewModel()
        { }

        public TcoPlrPaletizator Component { get; private set; } = new TcoPlrPaletizator();

        public override object Model { get => Component; set { Component = (TcoPlrPaletizator)value; } }


    }

    public class TcoPlrPaletizatorServiceViewModel : TcoPlrPaletizatorViewModel
    {
    }

}
