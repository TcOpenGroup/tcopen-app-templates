using HmiProjectx_template_x.Wpf;
using x_template_xPlc;
using x_template_xPlcConnector;
using Serilog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TcOpen.Inxton.Data;
using TcOpen.Inxton.Local.Security;
using TcOpen.Inxton.Local.Security.Wpf;
using TcOpen.Inxton.RavenDb;
using TcOpen.Inxton.TcoCore.Wpf;
using Vortex.Presentation.Wpf;
using Raven.Embedded;
using System.IO;

namespace x_template_xHmi.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Start embedded ravendb server

            EmbeddedServer.Instance.StartServer(new ServerOptions
            {
                DataDirectory = Path.Combine(Environment.CurrentDirectory, "data"),
                AcceptEula = true,
                ServerUrl = "http://127.0.0.1:8080"
            });

            x_template_xPlc.Connector.BuildAndStart().ReadWriteCycleDelay = 100;

            var authenticationService = SecurityManager
                .Create(new RavenDbRepository<UserData>(new RavenDbRepositorySettings<UserData>(new string[] { Constants.CONNECTION_STRING_DB }, "Users", "", "")));
            
            // App setup
            TcOpen.Inxton.TcoAppDomain.Current.Builder
                .SetUpLogger(new TcOpen.Inxton.Logging.SerilogAdapter(new LoggerConfiguration()
                                        .WriteTo.Console()     
                                        .WriteTo.File(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter(), "logs\\logs.log")
                                        .MinimumLevel.Verbose()
                                        .Enrich.WithEnvironmentName()
                                        .Enrich.WithEnvironmentUserName()
                                        .Enrich.WithEnrichedProperties()))
                .SetDispatcher(TcoCore.Wpf.Threading.Dispatcher.Get) // This is necessary for UI operation.  
                .SetSecurity(authenticationService)
                .SetEditValueChangeLogging(Entry.Plc.Connector)              
                .SetLogin(() => { var login = new LoginWindow(); login.ShowDialog(); })
                .SetPlcDialogs(DialogProxyServiceWpf.Create(new[] { x_template_xPlc.MAIN }));

            if (SecurityManager.Manager.UserRepository.Count == 0)
            {
                SecurityManager.Manager.UserRepository.Create("default", new UserData("default", "", new string[] { "Administrator" }));
            }

            
            LazyRenderer.Get.CreateSecureContainer = (permissions) => new PermissionBox { Permissions = permissions, SecurityMode = SecurityModeEnum.Invisible };

            SetUpRepositories();

            new Roles();

            x_template_xPlc.MAIN._technology._logger.StartLoggingMessages(TcoCore.eMessageCategory.Trace);

            SecurityManager.Manager.Service.AuthenticateUser("default", "");
            
        }
             
        private void SetUpRepositories()
        {
            var ProcessDataRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Constants.CONNECTION_STRING_DB }, "ProcessSettings", "", "");
            IntializeProcessDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._processSettings, new RavenDbRepository<PlainProcessData>(ProcessDataRepoSettings));

            var TechnologicalDataRepoSettings = new RavenDbRepositorySettings<PlainTechnologyData>(new string[] { Constants.CONNECTION_STRING_DB }, "TechnologySettings", "", "");
            IntializeTechnologyDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._technologySettings, new RavenDbRepository<PlainTechnologyData>(TechnologicalDataRepoSettings));

            var Traceability = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Constants.CONNECTION_STRING_DB }, "Traceability", "", "");
            IntializeProcessDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._processTraceability, new RavenDbRepository<PlainProcessData>(Traceability));            
            IntializeProcessDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._cu00x._processData, new RavenDbRepository<PlainProcessData>(Traceability));
        }
        
        private static void IntializeProcessDataRepositoryWithDataExchange(ProcessDataManager processData, IRepository<PlainProcessData> repository)
        {
            repository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            repository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };
            processData.InitializeRepository(repository);
            processData.InitializeRemoteDataExchange(repository);
        }

        private static void IntializeTechnologyDataRepositoryWithDataExchange(TechnologicalDataManager manager, IRepository<PlainTechnologyData> repository)
        {
            repository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; };
            repository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };
            manager.InitializeRepository(repository);
            manager.InitializeRemoteDataExchange(repository);
        }

        public static x_template_xPlcTwinController x_template_xPlc 
        {  
            get
            {
                return designTime ? Entry.PlcDesign : Entry.Plc;                
            }
        }

        private static bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
    }
}
