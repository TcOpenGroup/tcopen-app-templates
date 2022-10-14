using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vortex.Presentation;
using TcoCore;
using Vortex.Connector;
using TcOpen.Inxton.Local.Security;
using x_template_xHmi.Blazor.Security;

namespace x_template_xPlc
{
    public class CUBaseViewModel : RenderableViewModelBase
    {
        public CUBaseViewModel()
        {
          
        }

       

        public IEnumerable<object> _taskControls = new List<object>();
        public IEnumerable<object> TaskControls
        {
            get
            {
                if (Component != null && Component.GetChildren() != null)
                {
                    _taskControls = Component.GetChildren<ITcoTasked>().ToList();
                }

                return _taskControls;
            }
        }

        public CUBase Component { get; private set; } = new CUBase();

        public ProcessData OnlineData { get { return Component.GetChildren<TcoData.TcoDataExchange>().FirstOrDefault()?.GetChildren<TcoData.TcoEntity>().FirstOrDefault() as ProcessData; } }

        public EntityHeader EntityHeader { get { return OnlineData.EntityHeader; } }

        public IVortexElement Components { get { return Component.GetChildren<CUComponentsBase>().FirstOrDefault(); } }

        void Update()
        {

            var automatTask = Component.GetType().GetProperty("_automatTask")?.GetValue(Component) as TcoTaskedSequencer;
            if (automatTask != null)
            {
                automatTask._task.ExecuteDialog = () =>
                {
                    return true;
                    //return MessageBox.Show(x_template_xHmi.Wpf.Properties.strings.AutomatWarning, "Automat", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
                };
            }

            var groundTask = Component.GetType().GetProperty("_groundTask")?.GetValue(Component) as TcoTaskedSequencer;
            if (groundTask != null)
            {
                groundTask._task.ExecuteDialog = () =>
                {
                    return true;
                    //return MessageBox.Show(x_template_xHmi.Wpf.Properties.strings.GroundWarning, "Ground", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
                };
            }

            var manualTask = Component.GetType().GetProperty("_manualTask")?.GetValue(Component) as TcoTaskedService;
            if (manualTask != null)
            {
                manualTask.ExecuteDialog = () =>
                {
                    return true;
                    //return MessageBox.Show(x_template_xHmi.Wpf.Properties.strings.ManualWarning, "Manual", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
                };
            }

        }

        public override object Model { get => Component; set { Component = (CUBase)value; } }

        public TcOpen.Inxton.Input.RelayCommand OpenDetailsCommand { get; }
    }
}
