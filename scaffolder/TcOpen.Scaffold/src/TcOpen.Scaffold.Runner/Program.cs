using Octokit;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;

namespace TcOpen.Scaffold
{
    class Program
    {
        static void Main(string[] args)
        {
            var assemblyFile = Path.Combine(Environment.CurrentDirectory, "l\\TcOpen.Scaffold.UI\\TcOpen.Scaffold.UI.exe");
            string version = "0.0.0";
            if (File.Exists(assemblyFile))
            {
                System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyFile);
                version = fvi.ProductVersion;                
            }

            Updater.UpdateToNewestRelease(Updater.GetNewestVersion(version));

            Process.Start(assemblyFile);
        }        
    }
}
