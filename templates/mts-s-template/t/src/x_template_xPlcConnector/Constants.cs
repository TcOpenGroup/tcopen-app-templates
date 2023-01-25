using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace x_template_xPlcConnector
{
    public class Constants
    {
        // PLC
        public const DeployMode DEPLOY_MODE = DeployMode.Local;
        public static DatabaseEngine DATABASE_ENGINE = DatabaseEngine.RavenDbEmbded;
        public string PLC_AMS_ID = Environment.GetEnvironmentVariable("Tc3Target");
       

        // DB
        public static string GetConnectionString()
        {
            var connectionString = DATABASE_ENGINE == DatabaseEngine.MongoDb ? MONOGODB_PRODUCTION : RAVENDB_PRODUCTION;
            if (DEPLOY_MODE == DeployMode.Local)
            {
                connectionString = DATABASE_ENGINE == DatabaseEngine.MongoDb ? MONOGODB_LOCAL : RAVENDB_LOCAL;
            }
            return connectionString;
        }


        public const string DB_NAME = "tcomtsx_template_x";
        public const string MONGODB_PATH = @"C:\Program Files\MongoDB\Server\3.6\bin\mongod.exe";
        public const string MONGODB_ARGS = "--dbpath D:\\DATA\\DB\\ --bind_ip_all";
        public const bool MONGODB_RUN = true;
        private const string MONOGODB_LOCAL = @"mongodb://localhost:27017";
        private const string MONOGODB_PRODUCTION = @"mongodb://localhost:27017";
        private const string RAVENDB_LOCAL = @"http://localhost:8080";
        private const string RAVENDB_PRODUCTION = @"http://localhost:8080";

        // USER
        public const string AUTOLOGIN_USERNAME = "default";
        public const string AUTOLOGIN_USERPASS = "";

        public const string DOCU_PATH = @"c:\MTS\Develop\vts\vortex.mts.documentation\";
        public const bool DOCU_AUTO_GENERATE_BLANK = true;
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
