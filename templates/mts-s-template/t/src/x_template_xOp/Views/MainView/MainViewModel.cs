using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TcOpen.Inxton;
using TcOpen.Inxton.Local.Security.Wpf;
using x_template_xHmi.Wpf;
using x_template_xHmi.Wpf.Views.Diagnostics;
using x_template_xOp.Data;
using x_template_xOp.Properties;
using x_template_xOp.Views.Operator;
using x_template_xPlc;

namespace x_template_xOp.Views.MainView
{
    public class MainViewModel : MenuControlViewModel
    {
        public MainViewModel()
        {
            Title = "TECHNOLOGY";
            OpenCommand(this.AddCommand(typeof(OperatorView), strings.Operator));
            AddCommand(typeof(DataView), strings.Data);
            AddCommand(typeof(UserManagementGroupManagementView), strings.UserManagement);
            AddCommand(typeof(DiagnosticsView), strings.Diagnostics);
            OpenLoginWindowCommand = new TcOpen.Inxton.Input.RelayCommand(a => OpenLoginWindow());
            LogOutWindowCommand = new TcOpen.Inxton.Input.RelayCommand(a => TcOpen.Inxton.TcoAppDomain.Current.AuthenticationService.DeAuthenticateCurrentUser());
            OpenLanguageCommand = new TcOpen.Inxton.Input.RelayCommand(a => OpenLanguageWindow());
            CloseApplicationCommand = new TcOpen.Inxton.Input.RelayCommand(a => ClloseApplication());
        }

        private void ClloseApplication()
        {
            Application.Current.Shutdown();
        }

        public TcOpen.Inxton.Input.RelayCommand CloseApplicationCommand { get; private set; }
        public TcOpen.Inxton.Input.RelayCommand OpenLoginWindowCommand { get; private set; }
        public TcOpen.Inxton.Input.RelayCommand LogOutWindowCommand { get; private set; }
        public TcOpen.Inxton.Input.RelayCommand OpenLanguageCommand { get; private set; }



        private void OpenLanguageWindow()
        {
            TcoAppDomain.Current.Dispatcher.Invoke(
           (Action)(() =>
           {

               /* Unmerged change from project 'x_template_xOp (net5.0-windows)'
               Before:
                              var win = new CultureWindowView();
               After:
                              var win = new Wpf.CultureWindowView();
               */
               var win = new LanguageSelectionView();
               var viewInstance = Activator.CreateInstance((Type)win.GetType());

               win = viewInstance as LanguageSelectionView;
               if (win != null)
               {
                   win.DataContext = App.LanguageSelectionModel;
                   win.ShowDialog();


               }
           })
       );
        }
        public void OpenLoginWindow()
        {
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }
        public x_template_xPlcTwinController x_template_xPlc { get { return App.x_template_xPlc; } }
    }

}
