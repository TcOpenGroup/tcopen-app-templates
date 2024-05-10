using System;

namespace x_template_xPlcConnector
{
    public class ApplicationSettings
    {

        public DeployMode DepoyMode{ get; set; } = DeployMode.Local;
        public DatabaseEngine DatabaseEngine { get; set; } = DatabaseEngine.MongoDb;
        public string PlcAmsId =Environment.GetEnvironmentVariable("Tc3Target");
        public bool ShowConsoleOutput { get; set; } = true;

        public int ReadWriteCycleDelay { get; set; } = 100;

        public string DbName { get; set; } = "tcomtsx_template_x";
        public string MongoPath {get;set;} = @"C:\Program Files\MongoDB\Server\7.0\bin\mongod.exe";
        public string MongoArgs { get; set; } = "--dbpath D:\\DATA\\DB\\ --bind_ip_all";
        public bool MongoDbRun { get; set; } = true;
        private string MongoDbLocal { get; set; } = @"mongodb://localhost:27017";
        private string MongoDbProduction { get; set; } = @"mongodb://localhost:27017";
        private string RavenDbLocal { get; set; } = @"http://localhost:8080";
        private string RavenDbProduction { get; set; } = @"http://localhost:8080";

        //Verbose - tracing information and debugging minutiae; generally only switched on in unusual situations
        //Debug - internal control flow and diagnostic state dumps to facilitate pinpointing of recognised problems
        //Information - events of interest or that have relevance to outside observers; the default enabled minimum logging level
        //Warning - indicators of possible issues or service/functionality degradation
        //Error - indicating a failure within the application or connected system
        //Fatal - critical errors causing complete failure of the application
        public Serilog.Events.LogEventLevel LogRestrictedToMiniummLevel { get; set; } = Serilog.Events.LogEventLevel.Information;
        /// <summary>
        /// Capped size  max size of logs collection in MB
        /// </summary>
        public long CappedMaxSizeMb { get; set; } = 1000;
        /// <summary>
        /// Max documents in logs collection 
        /// </summary>
        public long CappedMaxDocuments { get; set; } = 1000000;

        // USER
        public string AutologinUserName { get; set; } = "admin";
        public  string AutologinUserPassword { get; set; }= "admin";

        public  string GetConnectionString()
        {
            var connectionString = DatabaseEngine == DatabaseEngine.MongoDb ? MongoDbProduction : RavenDbProduction;
            if (DepoyMode == DeployMode.Local)
            {
                connectionString = DatabaseEngine == DatabaseEngine.MongoDb ? MongoDbLocal : RavenDbLocal;
            }
            return connectionString;
        }

    }
    public enum DeployMode
    {
        Local = 1,
        Dummy = 2,
        Plc = 3
    }
    public enum DatabaseEngine
    {
        RavenDbEmbded = 1,
        MongoDb = 2,

    }
}
