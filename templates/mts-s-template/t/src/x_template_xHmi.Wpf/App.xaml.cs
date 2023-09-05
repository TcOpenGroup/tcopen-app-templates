using Raven.Embedded;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using TcOpen.Inxton.Data;
using TcOpen.Inxton.Instructor;
using TcOpen.Inxton.Local.Security;
using TcOpen.Inxton.Local.Security.Wpf;
using TcOpen.Inxton.Security;
using TcOpen.Inxton.TcoCore.Wpf;
using TcOpen.Inxton.RepositoryDataSet;
using Vortex.Presentation.Wpf;
using x_template_xDataMerge.Rework;
using x_template_xInstructor.TcoSequencer;
using x_template_xPlc;
using x_template_xPlcConnector;
using x_template_xProductionPlaner.Planer;
using x_template_xStatistic.Statistics;
using TcOpen.Inxton.Data.MongoDb;
using TcOpen.Inxton.RavenDb;
using System.Diagnostics;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using x_template_xHmi.Wpf.Properties;
using System.Globalization;
using System.Threading;
using MongoDB.Driver;
using x_template_xTagsDictionary;
using Vortex.Connector;

namespace x_template_xHmi.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            Color primaryColor = SwatchHelper.Lookup[MaterialDesignColor.Indigo];
            Color accentColor = SwatchHelper.Lookup[MaterialDesignColor.Lime];
            ITheme theme = Theme.Create(new MaterialDesignLightTheme(), primaryColor, accentColor);
            Resources.SetTheme(theme);


            base.OnStartup(e);
        }

        public App()
        {
            SetCulture();
            Entry.LoadAppSettings("default");

           Console.SetOut(SystemDiagnosticsSingleton.Instance.ConsoleWriter);

            GeAssembliesVersion("Tc");
            GeAssembliesVersion("Vortex");
            StopIfRunning();


           

            // This starts the twin connector operations
            x_template_xPlc.Connector.BuildAndStart().ReadWriteCycleDelay = Entry.Settings.ReadWriteCycleDelay;




            switch (Entry.Settings.DatabaseEngine)
            {
                case DatabaseEngine.RavenDbEmbded:
                    StartRavenDBEmbeddedServer();
                    CreateSecurityManageUsingRavenDb();
                    SetUpRepositoriesUsingRavenDb();
                    CuxTagsPairing = new TagsPairingController(RepositoryDataSetHandler<TagItem>.CreateSet(new RavenDbRepository<EntitySet<TagItem>>(new RavenDbRepositorySettings<EntitySet<TagItem>>(new string[] { Entry.Settings.GetConnectionString() }, "TagsDictionary", "", ""))), "TagsCfg"); ;


                  
                    break;
                case DatabaseEngine.MongoDb:
                    StartMongoDbServer(Entry.Settings.MongoPath, Entry.Settings.MongoArgs, Entry.Settings.MongoDbRun);
                    CreateSecurityManageUsingMongoDb();
                    SetUpRepositoriesUsingMongoDb();
                    CuxTagsPairing = new TagsPairingController(RepositoryDataSetHandler<TagItem>.CreateSet(new MongoDbRepository<EntitySet<TagItem>>(new MongoDbRepositorySettings<EntitySet<TagItem>>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "TagsDictionary"))), "TagsCfg");

                    break;
                default:
                    break;
            }


            // TcOpen app setup
            TcOpen.Inxton.TcoAppDomain.Current.Builder
                .SetUpLogger(new TcOpen.Inxton.Logging.SerilogAdapter(new LoggerConfiguration()
                                        .WriteTo.Console()
                                        //     .WriteTo.MongoDBBson($@"{Entry.Settings.GetConnectionString()}/{Entry.Settings.DbName}", "log",
                                        //                                                    Entry.Settings.LogRestrictedToMiniummLevel, 50, TimeSpan.FromSeconds(1), Entry.Settings.CappedMaxSizeMb, Entry.Settings.CappedMaxDocuments)
                                        //                                        .WriteTo.File(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter(), "logs\\logs.log")
                                        .MinimumLevel.Verbose()
                                        .Enrich.WithEnvironmentName()
                                        .Enrich.WithEnvironmentUserName()
                                        .Enrich.WithEnrichedProperties()))
                .SetDispatcher(TcoCore.Wpf.Threading.Dispatcher.Get) // This is necessary for UI operation.  
                .SetSecurity(SecurityManager.Manager.Service)
                .SetEditValueChangeLogging(Entry.Plc.Connector)
                .SetLogin(() => { var login = new LoginWindow(); login.ShowDialog(); })
                .SetPlcDialogs(DialogProxyServiceWpf.Create(new[] { x_template_xPlc.MAIN._technology._cu00x._processData }));



            // Otherwise undocumented feature in official IVF, for details refer to internal documentation.
            LazyRenderer.Get.CreateSecureContainer = (permissions) => new PermissionBox { Permissions = permissions, SecurityMode = SecurityModeEnum.Invisible };






            // Create user roles for this application.
            Roles.Create();

            // Starts the retrieval loop from of the messages from the PLC
            // If you have more TcOpen.Inxton application make sure you retrieve the messages only one of them.
            x_template_xPlc.MAIN._technology._logger.StartLoggingMessages(TcoCore.eMessageCategory.Info);

            SetUpExternalAuthenticationDevice();


            // Authenticates default user, change this line if you need to authenticate different user.
            SecurityManager.Manager.Service.AuthenticateUser(Entry.Settings.AutologinUserName, Entry.Settings.AutologinUserPassword);



            // initialize custom remote tasks here
            Action assignTagValeAction = () => TagsPairingOperation(x_template_xPlc.MAIN._technology._cu00x._components.PairTagTask);
            x_template_xPlc.MAIN._technology._cu00x._components.PairTagTask.InitializeExclusively(assignTagValeAction);



        }
        /// <summary>
        /// this is remontely invoked from plc , 
        /// </summary>
        /// <param name="pairTagTask"></param>
        private void TagsPairingOperation(PairTagTask pairTagTask)
        {
            pairTagTask.Read();

            TagItem currentItem = new TagItem();
            EnumResultsStatus result;
            switch ((eTagPairingMode) pairTagTask._mode.Cyclic)
            {
                case eTagPairingMode.GetTag:
                    CuxTagsPairing.GetTag(pairTagTask._key.Cyclic, out currentItem, out result);
                    pairTagTask._answer.AssignedValue.Cyclic = currentItem.AssignedValue;
                    pairTagTask._answer.Status.Cyclic =(short)currentItem.Status;
                    pairTagTask._answer.Answer.Cyclic = (short)result;
                    pairTagTask._answerInstruction.Cyclic = "";
                    break;
                case eTagPairingMode.RemoveTag:
                    //not used , for removing use UI
                    break;
                case eTagPairingMode.AddTag:
                    CuxTagsPairing.AddTag(new TagItem() {Key= pairTagTask._key.Cyclic, AssignedValue= pairTagTask._assignedValue.Cyclic}, out result);
                    pairTagTask._answer.Answer.Cyclic = (short)result;
                    pairTagTask._answerInstruction.Cyclic = "";
                    break;
                default:
                    break;
            }


            pairTagTask.Write();


        }

        private static void SetCulture()
        {
            Culture = Settings.Default.Culture;
            CultureInfo ci = new CultureInfo(Culture);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            LanguageSelectionModel = new LanguageSelectionViewModel();
            LanguageSelectionModel.AddCulture("sk-SK", Path.Combine(Assembly.GetExecutingAssembly().Location, @"\..\..\..\Assets\CulturalFlags\sk.png"));
            LanguageSelectionModel.AddCulture("cs-CZ", Path.Combine(Assembly.GetExecutingAssembly().Location, @"\..\..\..\Assets\CulturalFlags\cz.png"));
            LanguageSelectionModel.AddCulture("en-US", Path.Combine(Assembly.GetExecutingAssembly().Location, @"\..\..\..\Assets\CulturalFlags\us.png"));
        }

        private static void GeAssembliesVersion(string contains)
        {
    

            Assembly
            .GetExecutingAssembly()
            .GetReferencedAssemblies()
            .Where(assembly => assembly.FullName.Contains(contains)).Where(a => a.Version.Major != 0 || a.Version.Minor != 0)
            .ToList()
            .ForEach(assembly =>
            {
                var info = Assembly.Load(assembly)?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
                Console.WriteLine($"{assembly.Name}\t{info.Split('+').First()}\t{assembly}");
            });
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
        /// Starts mongo on local machine
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="args"></param>
        /// <param name="run"></param>
        public static void StartMongoDbServer(string filePath, string args, bool run)
        {
            bool runLocalMongo = run;
            var fileName = Path.GetFileName(filePath);
            bool isMongoRunning = System.Diagnostics.Process.GetProcesses().Where(p => p.ProcessName.Contains("mongod")).Any();
            if (!isMongoRunning && runLocalMongo)
            {
                var proc = new Process();
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.FileName = filePath;
                proc.StartInfo.Arguments = args;
                proc.Start();

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

            var users = new RavenDbRepository<UserData>(new RavenDbRepositorySettings<UserData>(new string[] { Entry.Settings.GetConnectionString() }, "Users", "", ""));
            var groups = new RavenDbRepository<GroupData>(new RavenDbRepositorySettings<GroupData>(new string[] { Entry.Settings.GetConnectionString() }, "Groups", "", ""));
            var roleGroupManager = new RoleGroupManager(groups);
            return SecurityManager.Create(users, roleGroupManager);
        }
      

        private void SetUpRepositoriesUsingRavenDb()
        {
            var ProcessDataRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Entry.Settings.GetConnectionString() }, "ProcessSettings", "", "");
            InitializeProcessDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._processSettings, new RavenDbRepository<PlainProcessData>(ProcessDataRepoSettings));

            var TechnologicalDataRepoSettings = new RavenDbRepositorySettings<PlainTechnologyData>(new string[] { Entry.Settings.GetConnectionString() }, "TechnologySettings", "", "");
            IntializeTechnologyDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._technologySettings, new RavenDbRepository<PlainTechnologyData>(TechnologicalDataRepoSettings));

            var ReworklDataRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Entry.Settings.GetConnectionString() }, "ReworkSettings", "", "");
            InitializeProcessDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._reworkSettings, new RavenDbRepository<PlainProcessData>(ReworklDataRepoSettings));

            //Statistics
            var _statisticsDataHandler = RepositoryDataSetHandler<StatisticsDataItem>.CreateSet(new RavenDbRepository<EntitySet<StatisticsDataItem>>(new RavenDbRepositorySettings<EntitySet<StatisticsDataItem>>(new string[] { Entry.Settings.GetConnectionString() }, "Statistics", "", "")));
            var _statisticsConfigHandler = RepositoryDataSetHandler<StatisticsConfig>.CreateSet(new RavenDbRepository<EntitySet<StatisticsConfig>>(new RavenDbRepositorySettings<EntitySet<StatisticsConfig>>(new string[] { Entry.Settings.GetConnectionString() }, "StatisticsConfig", "", "")));


            CuxStatistic = new StatisticsDataController(x_template_xPlc.MAIN._technology._cu00x.AttributeShortName, _statisticsDataHandler, _statisticsConfigHandler);



            var Traceability = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Entry.Settings.GetConnectionString() }, "Traceability", "", "");
            InitializeProcessDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._processTraceability, new RavenDbRepository<PlainProcessData>(Traceability));
            InitializeProcessDataRepositoryWithDataExchangeWithStatistic(x_template_xPlc.MAIN._technology._cu00x._processData, new RavenDbRepository<PlainProcessData>(Traceability), CuxStatistic);

            Rework = new ReworkModel(new RavenDbRepository<PlainProcessData>(ReworklDataRepoSettings), new RavenDbRepository<PlainProcessData>(Traceability));

            //Production planer         
            var _productionPlanHandler = RepositoryDataSetHandler<ProductionItem>.CreateSet(new RavenDbRepository<EntitySet<ProductionItem>>(new RavenDbRepositorySettings<EntitySet<ProductionItem>>(new string[] { Entry.Settings.GetConnectionString() }, "ProductionPlan", "", "")));

            ProductionPlaner = new ProductionPlanController(_productionPlanHandler, "ProductionPlanerTest", new RavenDbRepository<PlainProcessData>(ProcessDataRepoSettings));

            Action prodPlan = () => GetProductionPlan(x_template_xPlc.MAIN._technology._cu00x._productionPlaner);
            x_template_xPlc.MAIN._technology._cu00x._productionPlaner.InitializeExclusively(prodPlan);

            //Instructors
            var _instructionPlanHandler = RepositoryDataSetHandler<InstructionItem>.CreateSet(new RavenDbRepository<EntitySet<InstructionItem>>(new RavenDbRepositorySettings<EntitySet<InstructionItem>>(new string[] { Entry.Settings.GetConnectionString() }, "Instructions", "", "")));

            CuxInstructor = new InstructorController(_instructionPlanHandler, new InstructableSequencer(x_template_xPlc.MAIN._technology._cu00x._automatTask));
            CuxParalellInstructor = new InstructorController(_instructionPlanHandler, new InstructableSequencer(x_template_xPlc.MAIN._technology._cu00x._automatTask._paralellTask));



        }
        private IAuthenticationService CreateSecurityManageUsingMongoDb()
        {

            var users = new MongoDbRepository<UserData>(new MongoDbRepositorySettings<UserData>( Entry.Settings.GetConnectionString() , Entry.Settings.DbName, "Users"));
            var groups = new MongoDbRepository<GroupData>(new MongoDbRepositorySettings<GroupData>( Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "Groups"));
            var roleGroupManager = new RoleGroupManager(groups);
            return SecurityManager.Create(users, roleGroupManager);

        }


        private void SetUpRepositoriesUsingMongoDb()
        {
            var ProcessDataRepoSettings = new MongoDbRepositorySettings<PlainProcessData>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "ProcessSettings");
            InitializeProcessDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._processSettings, new MongoDbRepository<PlainProcessData>(ProcessDataRepoSettings));
            InitializeIndexProcessDataRepositoryMongoDb(ProcessDataRepoSettings);

            var TechnologicalDataRepoSettings = new MongoDbRepositorySettings<PlainTechnologyData>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "TechnologySettings");
            IntializeTechnologyDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._technologySettings, new MongoDbRepository<PlainTechnologyData>(TechnologicalDataRepoSettings));

         

            //Statistics
            var _statisticsDataHandler = RepositoryDataSetHandler<StatisticsDataItem>.CreateSet(new MongoDbRepository<EntitySet<StatisticsDataItem>>(new MongoDbRepositorySettings<EntitySet<StatisticsDataItem>>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "Statistics")));
            var _statisticsConfigHandler = RepositoryDataSetHandler<StatisticsConfig>.CreateSet(new MongoDbRepository<EntitySet<StatisticsConfig>>(new MongoDbRepositorySettings<EntitySet<StatisticsConfig>>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "StatisticsConfig")));


            CuxStatistic = new StatisticsDataController(x_template_xPlc.MAIN._technology._cu00x.AttributeShortName,_statisticsDataHandler,_statisticsConfigHandler);


            var ReworklDataRepoSettings = new MongoDbRepositorySettings<PlainProcessData>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "ReworkSettings");
            InitializeProcessDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._reworkSettings, new MongoDbRepository<PlainProcessData>(ReworklDataRepoSettings));

            var Traceability = new MongoDbRepositorySettings<PlainProcessData>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "Traceability");
            InitializeProcessDataRepositoryWithDataExchange(x_template_xPlc.MAIN._technology._processTraceability, new MongoDbRepository<PlainProcessData>(Traceability));       
            
            InitializeProcessDataRepositoryWithDataExchangeWithStatistic(x_template_xPlc.MAIN._technology._cu00x._processData, new MongoDbRepository<PlainProcessData>(Traceability),CuxStatistic);
            InitializeIndexProcessDataRepositoryMongoDb(Traceability);

            Rework = new ReworkModel(new MongoDbRepository<PlainProcessData>(ReworklDataRepoSettings), new MongoDbRepository<PlainProcessData>(Traceability));

            //Production planer         
            var _productionPlanHandler = RepositoryDataSetHandler<ProductionItem>.CreateSet(new MongoDbRepository<EntitySet<ProductionItem>>(new MongoDbRepositorySettings<EntitySet<ProductionItem>>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "ProductionPlan")));

            ProductionPlaner = new ProductionPlanController(_productionPlanHandler, "ProductionPlanerTest", new MongoDbRepository<PlainProcessData>(ProcessDataRepoSettings));

            Action prodPlan = () => GetProductionPlan(x_template_xPlc.MAIN._technology._cu00x._productionPlaner);
            x_template_xPlc.MAIN._technology._cu00x._productionPlaner.InitializeExclusively(prodPlan);
            
            //Instructors
            var _instructionPlanHandler= RepositoryDataSetHandler<InstructionItem>.CreateSet(new MongoDbRepository<EntitySet<InstructionItem>>(new MongoDbRepositorySettings<EntitySet<InstructionItem>>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "Instructions")));
         
            CuxInstructor = new InstructorController(_instructionPlanHandler, new InstructableSequencer(x_template_xPlc.MAIN._technology._cu00x._automatTask));
            CuxParalellInstructor = new InstructorController(_instructionPlanHandler, new InstructableSequencer(x_template_xPlc.MAIN._technology._cu00x._automatTask._paralellTask));


          
        }

        private void GetProductionPlan(ProductionPlaner productionPlaner)
        {
            ProductionItem item;

            ProductionPlaner.RefreshItems(out item);
            productionPlaner._requiredProcessSettings.Synchron = item.Key;
            productionPlaner._productonPlanCompleted.Synchron = ProductionPlaner.ProductionPlanCompleted;
            productionPlaner._productionPlanIsEmpty.Synchron = ProductionPlaner.ProductionPlanEmpty;



        }


        public void InitializeIndexProcessDataRepositoryMongoDb(MongoDbRepositorySettings<PlainProcessData> mongoDbRepositorySettings)
        {

            var indexes = mongoDbRepositorySettings.Collection.Indexes.List().ToList();
            var name = "_EntityId";
            if (!indexes.Exists(i => i.GetElement("name").ToString().Contains(name)))
            {
                var indexOptions = new CreateIndexOptions();
                indexOptions.Name = name;
                var indexKey = Builders<PlainProcessData>.IndexKeys.Descending(p => p._EntityId);
                mongoDbRepositorySettings.Collection.Indexes.CreateOne(new CreateIndexModel<PlainProcessData>(indexKey, indexOptions));

            }

            name = "_Created";
            if (!indexes.Exists(i => i.GetElement("name").ToString().Contains(name)))
            {
                var indexOptions = new CreateIndexOptions();
                indexOptions.Name = name;
                var indexKey = Builders<PlainProcessData>.IndexKeys.Descending(p => p._Created);
                mongoDbRepositorySettings.Collection.Indexes.CreateOne(new CreateIndexModel<PlainProcessData>(indexKey, indexOptions));

            }
            name = "_Modified";
            if (!indexes.Exists(i => i.GetElement("name").ToString().Contains(name)))
            {
                var indexOptions = new CreateIndexOptions();
                indexOptions.Name = name;
                var indexKey = Builders<PlainProcessData>.IndexKeys.Descending(p => p._Modified);
                mongoDbRepositorySettings.Collection.Indexes.CreateOne(new CreateIndexModel<PlainProcessData>(indexKey, indexOptions));

            }

        }
     
        /// <summary>
        /// Initializes <see cref="ProcessDataManager"/>s repository for data exchange between PLC and storage (database).
        /// </summary>
        /// <param name="manager">Data manager</param>
        /// <param name="repository">Repository</param>
        private static void InitializeProcessDataRepositoryWithDataExchange(ProcessDataManager processData, IRepository<PlainProcessData> repository)
        {
            repository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            repository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };
            processData.InitializeRepository(repository);
            processData.InitializeRemoteDataExchange(repository);

        }

        /// Initializes <see cref="ProcessDataManager"/>s repository for data exchange between PLC and storage (database).
        /// </summary>
        /// <param name="manager">Data manager</param>
        /// <param name="repository">Repository</param>
        private static void InitializeProcessDataRepositoryWithDataExchangeWithStatistic(ProcessDataManager processData, IRepository<PlainProcessData> repository, StatisticsDataController cuxStatistic)
        {
            repository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            repository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; cuxStatistic.Count(data); };
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

        static string Culture = "";
        public static ReworkModel Rework { get; private set; }
        public static ProductionPlanController ProductionPlaner { get; private set; }
        public static InstructorController CuxInstructor { get; private set; }
        public static InstructorController CuxParalellInstructor { get; private set; }
        public static StatisticsDataController CuxStatistic { get; private set; }
        public static TagsPairingController CuxTagsPairing { get; private set; }
        public static LanguageSelectionViewModel LanguageSelectionModel { get; private set; }

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
                KillOtherInstances();
                Application.Current.Shutdown(-1);
            }
        }


        private void KillOtherInstances() => Process
                    .GetProcessesByName(Process.GetCurrentProcess().ProcessName)
                    .Where(process => process.Id != Process.GetCurrentProcess().Id)
                    .ToList()
                    .ForEach(p => p.Kill());


    }
}
