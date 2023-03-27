using x_template_xHmi.Wpf.Data;
using x_template_xHmi.Wpf.Properties;
using x_template_xHmi.Wpf.Views.Operator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcOpen.Inxton.Local.Security.Wpf;
using x_template_xPlc;
using x_template_xHmi.Wpf.Views.Diagnostics;

namespace x_template_xHmi.Wpf.Views.MainView
{
    public class MainViewModel : MenuControlViewModel
    {
        public MainViewModel()
        {
            this.Title = "TECHNOLOGY";
            this.OpenCommand(this.AddCommand(typeof(OperatorView), strings.Operator));
            this.AddCommand(typeof(DataView), strings.Data); 
            this.AddCommand(typeof(UserManagementGroupManagementView), strings.UserManagement);
            this.AddCommand(typeof(DiagnosticsView), strings.Diagnostics);
            this.OpenLoginWindowCommand = new TcOpen.Inxton.Input.RelayCommand(a => OpenLoginWindow());
            this.LogOutWindowCommand = new TcOpen.Inxton.Input.RelayCommand(a => TcOpen.Inxton.TcoAppDomain.Current.AuthenticationService.DeAuthenticateCurrentUser() );
        }

        public TcOpen.Inxton.Input.RelayCommand OpenLoginWindowCommand { get; private set; }
        public TcOpen.Inxton.Input.RelayCommand LogOutWindowCommand { get; private set; }

        public void OpenLoginWindow()
        {
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }
        public x_template_xPlcTwinController x_template_xPlc { get { return App.x_template_xPlc; } }
    }
}
