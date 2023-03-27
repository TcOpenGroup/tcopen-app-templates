

using System;
using System.Windows;
using System.Windows.Controls;
using TcoCore;
using TcoIo;
using TcOpen.Inxton;
using TcOpen.Inxton.Input;
using x_template_xPlc;

namespace x_template_xHmi.Wpf.Views.Diagnostics
{
    public class DiagnosticsViewModel : MenuControlViewModel
    {
        public DiagnosticsViewModel() : base()
        {

            ShowTopologyCommand = new RelayCommand(a => ShowTopology());
        }

        private void ShowTopology()
        {
            TcoAppDomain.Current.Dispatcher.Invoke(
           () =>
           {
               var win = new TopologyView();
               var viewInstance = Activator.CreateInstance(win.GetType());
        
                win = viewInstance as TopologyView;
               if (win != null)
               {
                   win.DataContext = x_template_xPlc.GVL_iXlinker;
                   win.Show();

                  
               }
           }
           );
        }
        public TopologyRenderer TopologyView { get; } = new TopologyRenderer();
        public x_template_xPlcTwinController x_template_xPlc { get { return App.x_template_xPlc; } }

        public RelayCommand ShowTopologyCommand { get; }
    }
}
