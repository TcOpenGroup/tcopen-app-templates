using Cake.Common.Tools.MSBuild;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Common.Tools.DotNet;


namespace Build
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return new CakeHost()
                .UseContext<BuildContext>()                
                .Run(args);
            
        }
    } 
}