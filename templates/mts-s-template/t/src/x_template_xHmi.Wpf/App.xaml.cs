using HmiProjectx_template_x.Wpf;
using Raven.Embedded;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using TcOpen.Inxton.Data;
using TcOpen.Inxton.Local.Security;
using TcOpen.Inxton.Local.Security.Wpf;
using TcOpen.Inxton.RavenDb;
using TcOpen.Inxton.Security;
using TcOpen.Inxton.TcoCore.Wpf;
using Vortex.Presentation.Wpf;
using x_template_xPlc;
using x_template_xPlcConnector;

namespace x_template_xHmi.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            StopIfRunning();

            // This starts the twin connector operations
            x_template_xPlc.Connector.BuildAndStart().ReadWriteCycleDelay = 100;
            
            StartRavenDBEmbeddedServer();
          
            // TcOpen app setup
            TcOpen.Inxton.TcoAppDomain.Current.Builder
                .SetUpLogger(new TcOpen.Inxton.Logging.SerilogAdapter(new LoggerConfiguration()
                                        .WriteTo.Console()
                                        .WriteTo.File(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter(), "logs\\logs.log")
                                        .MinimumLevel.Verbose()
                                        .Enrich.WithEnvironmentName()
                                        .Enrich.WithEnvironmentUserName()
                                        .Enrich.WithEnrichedProperties()))
                .SetDispatcher(TcoCore.Wpf.Threading.Dispatcher.Get) // This is necessary for UI operation.  
                .SetSecurity(CreateSecurityManageUsingRavenDb())
                .SetEditValueChangeLogging(Entry.Plc.Connector)
                .SetLogin(() => { var login = new LoginWindow(); login.ShowDialog(); })
                .SetPlcDialogs(DialogProxyServiceWpf.Create(new[] { x_template_xPlc.MAIN }));

            // Creates new 'default' user if the user repository is empty.

            if (SecurityManager.Manager.UserRepository.Count == 0)
            {
                TcOpen.Inxton.TcoAppDomain.Current.Logger.Warning("No users are defined in the repository. " +
                                  "\nCreating first user with default setting. " +
                                  "\nMake sure you remove this user once you have set up user rights for your users.", new { });

                SecurityManager.Manager.UserRepository.Create("default", new UserData("default", "", new string[] { "Administrator" }));
            }

            // Otherwise undocumented feature in official IVF, for details refer to internal documentation.
            LazyRenderer.Get.CreateSecureContainer = (permissions) => new PermissionBox { Permissions = permissions, SecurityMode = SecurityModeEnum.Invisible };

            SetUpRepositoriesUsingRavenDb();

            // Create user roles for this application.
            Roles.Create();

            // Starts the retrieval loop from of the messages from the PLC
            // If you have more TcOpen.Inxton application make sure you retrieve the messages only one of them.
            x_template_xPlc.MAIN._technology._logger.StartLoggingMessages(TcoCore.eMessageCategory.Info);
            
            SetUpExternalAuthenticationDevice();


            // Authenticates default user, change this line if you need to authenticate different user.
            SecurityManager.Manager.Service.AuthenticateUser("default", "");
            
        }

        private static void SetUpExternalAuthenticationDevice()
        {            
            try
            {
                SecurityManager.Manager.Service.ExternalAuthorization = TcOpen.Inxton.Local.Security.Readers.ExternalTokenAuthorization.CreateComReader("COM3");
            }
            catch (Exception ex)
            {
                TcOpen.Inxton.TcoAppDomain.Current.Logger.Warning($"Authentication device was not properly initialized:'{ex.Message}'", ex);
            }
        }

        /// <summary>
        /// Starts embedded instance of RavenDB server.
        /// IMPORTANT! 
        /// CHECK EULA BEFORE USING @ https://ravendb.org/terms
        /// GET APPROPRIATE LICENCE https://ravendb.org/buy FREE COMUNITY EDITION IS ASLO AVAILABLE, BUT YOU NEED TO REGISTER.
        /// STORAGE IS DIRECTED TO THE BIN FOLDER OF REDIRECT 
        /// `DataDirectory` property in this method to persist tha data elsewhere.
        /// </summary>
        private static void StartRavenDBEmbeddedServer()
        {
            // Start embedded RavenDB server

            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Starting embedded RavenDB server instance. " +
                "\nYou should not use this instance in production. " +
                "\nUsing embedded RavenDB server you agree to the respective EULA." +
                "\nYou will need to register the licence." +
                "\nThe data are strored in temporary 'bin' folder of your application, " +
                "\nif you want to persist your data safely redirect the DataDirectory into different location.");
            Console.WriteLine("---------------------------------------------------");

            EmbeddedServer.Instance.StartServer(new ServerOptions
            {
                DataDirectory = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "tmp", "data"),
                AcceptEula = true,
                ServerUrl = "http://127.0.0.1:8080",
            });
            
           // EmbeddedServer.Instance.OpenStudioInBrowser();
        }

        private IAuthenticationService CreateSecurityManageUsingRavenDb()
        {
            return SecurityManager.Create(new RavenDbRepository<UserData>(
                                          new RavenDbRepositorySettings<UserData>
                                          (new string[] { Constants.CONNECTION_STRING_DB }, "Users", "", "")));
        }

        private void SetUpRepositoriesUsingRavenDb()
        {
            var ProcessDataRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Constants.CONNECTION_STRING_DB }, "ProcessSettings", "", "");
            IntializeProcessDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._processSettings, new RavenDbRepository<PlainProcessData>(ProcessDataRepoSettings));

            var TechnologicalDataRepoSettings = new RavenDbRepositorySettings<PlainTechnologyData>(new string[] { Constants.CONNECTION_STRING_DB }, "TechnologySettings", "", "");
            IntializeTechnologyDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._technologySettings, new RavenDbRepository<PlainTechnologyData>(TechnologicalDataRepoSettings));

            var Traceability = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Constants.CONNECTION_STRING_DB }, "Traceability", "", "");
            IntializeProcessDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._processTraceability, new RavenDbRepository<PlainProcessData>(Traceability));            
            IntializeProcessDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._cu00x._processData, new RavenDbRepository<PlainProcessData>(Traceability));
        }

        /// <summary>
        /// Initializes <see cref="ProcessDataManager"/>s repository for data exchange between PLC and storage (database).
        /// </summary>
        /// <param name="manager">Data manager</param>
        /// <param name="repository">Repository</param>
        private static void IntializeProcessDataRepositoryWithDataExchange(ProcessDataManager processData, IRepository<PlainProcessData> repository)
        {
            repository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            repository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };
            processData.InitializeRepository(repository);
            processData.InitializeRemoteDataExchange(repository);
        }

        /// <summary>
        /// Initializes <see cref="TechnologicalDataManager"/>s repository for data exchange between PLC and storage (database).
        /// </summary>
        /// <param name="manager">Data manager</param>
        /// <param name="repository">Repository</param>
        private static void IntializeTechnologyDataRepositoryWithDataExchange(TechnologicalDataManager manager, IRepository<PlainTechnologyData> repository)
        {
            repository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; };
            repository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };
            manager.InitializeRepository(repository);
            manager.InitializeRemoteDataExchange(repository);
        }

        /// <summary>
        /// Gets the twin connector for this application.
        /// </summary>
        public static x_template_xPlcTwinController x_template_xPlc 
        {  
            get
            {
                return designTime ? Entry.PlcDesign : Entry.Plc;                
            }
        }

        /// <summary>
        /// Determines whether the application at design time. (true when at design, false at runtime)
        /// </summary>
        private static bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());   
        
        /// <summary>
        /// Checks that no other instance of this program is running on this system.
        /// </summary>
        private void StopIfRunning()
        {
            var processes = System.Diagnostics.Process.GetProcessesByName(Assembly.GetEntryAssembly().GetName().Name);

            if (processes.Count() > 1)
            {
                MessageBox.Show("This program is already running on this system. We cannot run another instance of this program.",
                                "Checking for running processes", MessageBoxButton.OK, MessageBoxImage.Error);

                Application.Current.Shutdown(-1);
            }
        }
    }
}
