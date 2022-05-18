using Cake.Common.Tools.MSBuild;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Common.Tools.DotNet;

namespace mts_s_template
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

    [TaskName("Clean")]
    public sealed class CleanTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Clean();
        }
    }

    [TaskName("Restore")]
    [IsDependentOn(typeof(CleanTask))]
    public sealed class RestoreTask : FrostingTask<BuildContext>
    {
        // Tasks can be asynchronous
        public override void Run(BuildContext context)
        {
            foreach (var projectFile in context.TemplateProjects)
            {
                context.RestorePackages(projectFile);
            }

        }
    }

    [TaskName("IvcBuild")]
    [IsDependentOn(typeof(RestoreTask))]
    public sealed class IvcBuildTask : FrostingTask<BuildContext>
    {
        // Tasks can be asynchronous
        public override void Run(BuildContext context)
        {
            var cupdater = context.TemplateProjects.Where(p => new FileInfo(p).Name == "cupdater.csproj").FirstOrDefault();
            var settings = new Cake.Common.Tools.DotNet.MSBuild.DotNetMSBuildSettings();                        
            settings.Properties.Add("SolutionDir", new List<string>() { new FileInfo(context.TemplateSolutions.FirstOrDefault()).DirectoryName + "\\" });

            context.DotNetBuild(cupdater, new Cake.Common.Tools.DotNet.Build.DotNetBuildSettings() { MSBuildSettings = settings });

            foreach (var solutionFile in context.TemplateSolutions)
            {                
                context.RunIvc(solutionFile);
            }

        }
    }

    [TaskName("Build")]
    [IsDependentOn(typeof(IvcBuildTask))]
    public sealed class BuildTask : FrostingTask<BuildContext>
    {
        // Tasks can be asynchronous
        public override void Run(BuildContext context)
        {
            foreach (var solutionFile in context.TemplateSolutions)
            {
                var runner = new MSBuildRunner(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);
                runner.Run(solutionFile, new MSBuildSettings());
            }
        }
    }

    [TaskName("Test")]
    [IsDependentOn(typeof(BuildTask))]
    [ContinueOnError]
    public sealed class TestTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.LoadXaeToPlc(context.TemplateSolutions.FirstOrDefault());
            var testRunner = new Cake.Common.Tools.VSTest.VSTestRunner(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);
            testRunner.Run(context.TemplateTestFiles.Select(p => new FilePath(p)), new Cake.Common.Tools.VSTest.VSTestSettings());
        }
    }

    [TaskName("TearDown")]
    [IsDependentOn(typeof(TestTask))]
    public sealed class TearDownTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.CloseVsSolution(context.TemplateSolutions.FirstOrDefault());
        }
    }

    [TaskName("Default")]
    [IsDependentOn(typeof(TearDownTask))]
    public class DefaultTask : FrostingTask
    {

    }
}