using Cake.Common.Tools.DotNet;
using Cake.Core;
using Cake.Frosting;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Build.Scaffolder
{
    public class Const
    {
        public const string BuildGroupName = " : Scaffolder";
    }

    [TaskName("Startup" + Const.BuildGroupName)]
    [IsDependentOn(typeof(mts_s_template.LastTask))]
    public sealed class StartUpTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            
        }
    }

    [TaskName("Setup" + Const.BuildGroupName)]
    [IsDependentOn(typeof(StartUpTask))]
    public sealed class SetupTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.ProjectRootDirectory = Path.GetFullPath(Path.Combine(context.Environment.WorkingDirectory.FullPath, "..//scaffolder//TcOpen.Scaffold"));            
        }
    }

    [TaskName("Clean" + Const.BuildGroupName)]
    [IsDependentOn(typeof(SetupTask))]
    public sealed class CleanTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.ProjectRootDirectory = Path.GetFullPath(Path.Combine(context.Environment.WorkingDirectory.FullPath, "..//scaffolder//TcOpen.Scaffold"));
            // delete artifacts folder
            var artifactsFolder = context.FileSystem.GetDirectory("..\\artifacts");
            if (artifactsFolder.Exists)
            {
                artifactsFolder.Delete(true);
            }

            context.DotNetClean($"{context.ProjectRootDirectory}\\TcOpen.Scaffold.sln");
        }
    }

    [TaskName("Build" + Const.BuildGroupName)]
    [IsDependentOn(typeof(CleanTask))]
    public sealed class BuildTask : FrostingTask<BuildContext>
    {    
        public override void Run(BuildContext context)
        {
            context.DotNetBuild($"{context.ProjectRootDirectory}\\TcOpen.Scaffold.sln", new Cake.Common.Tools.DotNet.Build.DotNetBuildSettings()
            {
                Configuration = "Release"
            });
        }
    }

    [TaskName("Create artifacts" + Const.BuildGroupName)]
    [IsDependentOn(typeof(BuildTask))]
    public sealed class ArtifactTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.DotNetPublish($"{context.ProjectRootDirectory}\\src\\TcOpen.Scaffold.UI\\TcOpen.Scaffold.UI.csproj",
                new Cake.Common.Tools.DotNet.Publish.DotNetPublishSettings()
                {
                    Configuration = "Release",
                    Framework = "net5.0-windows",
                    PublishSingleFile = true,
                    SelfContained = false,
                    PublishReadyToRun = true,
                    Runtime = "win10-x64",
                    OutputDirectory = "${context.ProjectRootDirectory}\\src\\TcOpen.Scaffold.UI\\Publish"
                });

            context.ZipFolder($"{context.ProjectRootDirectory}\\src\\TcOpen.Scaffold.UI\\bin\\Release\\net5.0-windows", "..\\artifacts\\TcOpen.Scaffold.UI.zip");
        }
    }

    [TaskName("Publish release" + Const.BuildGroupName)]
    [IsDependentOn(typeof(ArtifactTask))]
    public sealed class PublishReleaseTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            //var githubActionsProvider = new Cake.Common.Build.GitHubActions.GitHubActionsProvider(context.Environment, context.FileSystem);

            //if (GitVersionInformation.BranchName == "dev")
            {
                var githubToken = context.Environment.GetEnvironmentVariable("gh-public-repos");
                var githubClient = new GitHubClient(new ProductHeaderValue("TcOpen.Scaffold.UI"));
                githubClient.Credentials = new Credentials(githubToken);

                var release = githubClient.Repository.Release.Create(
                    "TcOpenGroup",
                    "tcopen-app-templates",
                    new NewRelease($"{GitVersionInformation.SemVer}")
                    {
                        Name = $"{GitVersionInformation.SemVer}",
                        TargetCommitish = GitVersionInformation.Sha,
                        Body = $"Release v{GitVersionInformation.SemVer}",
                        Draft = true,
                        Prerelease = true
                    }
                ).Result;

                var asset = new ReleaseAssetUpload($"{context.ProjectRootDirectory}\\artifacts\\TcOpen.Scaffold.UI.zip", "application/zip", new StreamReader($"{context.ProjectRootDirectory}\\artifacts\\TcOpen.Scaffold.UI.zip").BaseStream, TimeSpan.FromSeconds(3600));

                githubClient.Repository.Release.UploadAsset(release, asset).Wait();
            }
        }
    }

    [TaskName("Default" + Const.BuildGroupName)]
    [IsDependentOn(typeof(PublishReleaseTask))]
    public class LastTask : FrostingTask
    {
    }
}
