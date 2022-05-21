using Build;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.MSBuild;
using Cake.Core;
using Cake.Common;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.Cli;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Build.mts_s_template
{
    public class Const
    {
        public const string BuildGroupName = " : mts-s-template";
    }

    [TaskName("Setup" + Const.BuildGroupName)]
    public sealed class SetupTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.ProjectRootDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(context.Environment.WorkingDirectory.FullPath, "..//templates//mts-s-template"));
        }
    }

    [TaskName("Clean" + Const.BuildGroupName)]
    [IsDependentOn(typeof(SetupTask))]    
    public sealed class CleanTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {            
            if(context.Arguments.HasArgument("clean") && context.Argument<bool>("clean"))
            { 
                context.Clean();
            }
            else
            {
                
            }
        }
    }

    [TaskName("Restore" + Const.BuildGroupName)]
    [IsDependentOn(typeof(CleanTask))]
    public sealed class RestoreTask : FrostingTask<BuildContext>
    {
        // Tasks can be asynchronous
        public override void Run(BuildContext context)
        {
            context.ProjectRootDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(context.Environment.WorkingDirectory.FullPath, "../templates/mts-s-template/"));
            foreach (var projectFile in context.TemplateProjects)
            {
                context.RestorePackages(projectFile);
            }

        }
    }

    [TaskName("IvcBuild" + Const.BuildGroupName)]
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

    [TaskName("Build" + Const.BuildGroupName)]
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

    [TaskName("Test" + Const.BuildGroupName)]
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

    [TaskName("Closing" + Const.BuildGroupName)]
    [IsDependentOn(typeof(TestTask))]
    [ContinueOnError]
    public sealed class LastTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            
        }
    }

    [TaskName("TearDownTask" + Const.BuildGroupName)]
    public sealed class TearDownTask : FrostingTaskTeardown<BuildContext>
    {
        public override void Teardown(BuildContext context, ITaskTeardownContext info)
        {
            context.CloseVsSolution(context.TemplateSolutions.FirstOrDefault());
        }
    }    
}
