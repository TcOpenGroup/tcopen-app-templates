using x_template_xHmi.Wpf;
using x_template_xHmi.Wpf.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TcoCore;
using TcOpen.Inxton.Local.Security;
using x_template_xPlcConnector;

namespace x_template_xPlc
{
    public class CUBaseViewModel : MenuRenderableControlViewModel
    {
        public CUBaseViewModel()
        {
            this.AddCommand(typeof(CUBaseOverviewView), strings.Overview, this);
            this.OpenCommand(this.AddCommand(typeof(CUBaseTasksView), strings.Control, this));
            this.AddCommand(typeof(CUBaseDataView), strings.OnlineData, this);
            this.AddCommand(typeof(CUBaseComponentsView), strings.Components, this);
            this.AddCommand(typeof(CUBaseDiagView), strings.Diagnostics, this);
        
            this.OpenDetailsCommand = new TcOpen.Inxton.Input.RelayCommand((a) => OpenDetails());
            OpenTasksDetailsCommand = new TcOpen.Inxton.Input.RelayCommand((a) => OpenTasksDetails());
        }

        protected async void OpenTasksDetails()
        {
            x_template_xHmi.Wpf.NavigableViewModelBase.Current.ShowInWindow(new CUBaseInfoDetailsTasksView() { DataContext = this });

        }
        private void OpenDetails()
        {
            if (AuthorizationChecker.HasAuthorization(DefaultRoles.station_details))
            {
                var detailsView = Vortex.Presentation.Wpf.LazyRenderer.Get.CreatePresentation("Control", Component, new Grid(), false);
                NavigableViewModelBase.Current.OpenView(detailsView as FrameworkElement);
            }
        }

        public IEnumerable<object> _taskControls = new List<object>();
        public IEnumerable<object> TaskControls
        {
            get
            {
                var suffixesToMatch = new List<string> { "_automatTask", "_groundTask", "_manualTask" };
                if (Component != null && Component.GetKids() != null)
                {
                    _taskControls = Component.GetKids().Where(p => suffixesToMatch.Any(suffix => p.Symbol.EndsWith(suffix)));
                    //_taskControls = Component.GetChildren<ITcoTasked>();                    
                }

                return _taskControls;
            }
        }

        public IEnumerable<object> _safetyTaskControls = new List<object>();
        public IEnumerable<object> SafetyTaskControls
        {
            get
            {
                var suffixesToMatch = new List<string> { "_suspendTask", "_recoverTask" };
                if (Component != null && Component.GetKids() != null)
                {
                    _safetyTaskControls = Component.GetKids().Where(p => suffixesToMatch.Any(suffix => p.Symbol.EndsWith(suffix)));
                                  
                }

                return _safetyTaskControls;
            }
        }

        public CUBase Component { get; private set; } = new CUBase();

        public ProcessData OnlineData { get { return Component.GetChildren<TcoData.TcoDataExchange>().FirstOrDefault()?.GetChildren<TcoData.TcoEntity>().FirstOrDefault() as ProcessData; } }

    
        public EntityHeader EntityHeader { get { return OnlineData.EntityHeader; } }

        public object Components { get { return Component.GetChildren<CUComponentsBase>().FirstOrDefault(); } }

        void Update()
        {
            var symbolOrName = new NameOrSymbolConverter();
            this.Title = (string)symbolOrName.Convert(Component, typeof(string), null, System.Globalization.CultureInfo.InvariantCulture);

            
                var automatTask = Component.GetType().GetProperty("_automatTask")?.GetValue(Component) as TcoTaskedSequencer;
                if (automatTask != null)
                {
                    automatTask._task.ExecuteDialog = () =>
                    {
                        return MessageBox.Show(x_template_xHmi.Wpf.Properties.strings.AutomatWarning, "Automat", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
                    };
                }

                var groundTask = Component.GetType().GetProperty("_groundTask")?.GetValue(Component) as TcoTaskedSequencer;
                if (groundTask != null)
                {
                    groundTask._task.ExecuteDialog = () =>
                    {
                        return MessageBox.Show(x_template_xHmi.Wpf.Properties.strings.GroundWarning, "Ground", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
                    };
                }

                var manualTask = Component.GetType().GetProperty("_manualTask")?.GetValue(Component) as TcoTaskedService;
                if (manualTask != null)
                {
                    manualTask.ExecuteDialog = () =>
                    {
                        return MessageBox.Show(x_template_xHmi.Wpf.Properties.strings.ManualWarning, "Manual", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
                    };
                }
           
        }

        public override object Model { get => Component; set { Component = (CUBase)value; this.Update(); } }

        public TcOpen.Inxton.Input.RelayCommand OpenTasksDetailsCommand { get; private set; }

        public TcOpen.Inxton.Input.RelayCommand OpenDetailsCommand { get; }
    }
}
