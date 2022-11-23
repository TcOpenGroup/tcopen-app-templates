using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace x_template_xTests
{
    internal class x_template_xApp
    {
        private Process appProcess;
        private void StartApp()
        {
            var slnFolder = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, "..\\..\\..\\..\\..\\..\\..\\..\\..\\")); //Environment.GetEnvironmentVariable("slnFolder") ?? @"..\";

#if DEBUG
            var x_template_xFolder = @"templates\mts-s-template\t\src\x_template_xHmi.Wpf\bin\Debug\net48";
#else
            var x_template_xFolder = @"templates\mts-s-template\t\src\x_template_xHmi.Wpf\bin\Release\net48";
#endif
            var x_template_xExe = "x_template_xHmi.Wpf.exe";
            var applicationPath = Path.GetFullPath(Path.Combine(slnFolder, x_template_xFolder, x_template_xExe));
            var app = Path.GetFullPath(applicationPath);
            if (File.Exists(applicationPath))
            {
                appProcess = Process.Start(applicationPath, "RavenDbEmbded");
                System.Threading.Thread.Sleep(10000);
            }
            else
                throw new EntryPointNotFoundException(applicationPath + "Not found. Current PWD " + Environment.CurrentDirectory);

        }

        public void KillApp()
        {
            appProcess?.Kill();
            _instance = null;
        }

        private x_template_xApp()
        {
            this.StartApp();
        }


        static x_template_xApp _instance;

        public static x_template_xApp Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new x_template_xApp();
                }

                return _instance;
            }

        }



    }
}
