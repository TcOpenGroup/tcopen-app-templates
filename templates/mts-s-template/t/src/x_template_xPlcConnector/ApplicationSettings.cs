using System;

namespace x_template_xPlcConnector
{
    public class ApplicationSettings
    {

        public DeployMode DepoyMode{ get; set; } = DeployMode.Local;
        public DatabaseEngine DatabaseEngine { get; set; } = DatabaseEngine.RavenDbEmbded;
        public string PlcAmsId = Environment.GetEnvironmentVariable("Tc3Target");
        public bool ShowConsoleOutput { get; set; } = false;

        public string DbName { get; set; } = "tcomtsx_template_x";
        public string MongoPath {get;set;} = @"C:\Program Files\MongoDB\Server\3.6\bin\mongod.exe";
        public string MongoArgs { get; set; } = "--dbpath D:\\DATA\\DB\\ --bind_ip_all";
        public bool MongoDbRun { get; set; } = false;
        private string MongoDbLocal { get; set; } = @"mongodb://localhost:27017";
        private string MongoDbProduction { get; set; } = @"mongodb://localhost:27017";
        private string RavenDbLocal { get; set; } = @"http://localhost:8080";
        private string RavenDbProduction { get; set; } = @"http://localhost:8080";

        // USER
        public  string AutologinUserName { get; set; } = "default";
        public  string AutologinUserPassword { get; set; }= "";

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
