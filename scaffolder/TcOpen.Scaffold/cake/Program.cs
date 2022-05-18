using Cake.Common.Tools.DotNet;
using Cake.Core;
using Cake.Frosting;
using Octokit;
using System;
using System.IO;

namespace scaffolder_build
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

    public class BuildContext : FrostingContext
    {
        public bool Delay { get; set; }

        public BuildContext(ICakeContext context)
            : base(context)
        {
            Delay = context.Arguments.HasArgument("delay");

        }

        public void ZipFolder(string sourceFolder, string destinationFile)
        {
            var outputDirectory = new FileInfo(destinationFile).Directory;

            if (!outputDirectory.Exists)
            {
                outputDirectory.Create();
            }

            System.IO.Compression.ZipFile.CreateFromDirectory(sourceFolder, destinationFile);
        }
    }

    [TaskName("Clean")]
    public sealed class CleanTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            // delete artifacts folder
            var artifactsFolder = context.FileSystem.GetDirectory("..\\artifacts");
            if (artifactsFolder.Exists)
            {
                artifactsFolder.Delete(true);
            }

            context.DotNetClean("..\\TcOpen.Scaffold.sln");
        }
    }

    [TaskName("Build")]
    [IsDependentOn(typeof(CleanTask))]
    public sealed class BuildTask : FrostingTask<BuildContext>
    {
        // Tasks can be asynchronous
        public override void Run(BuildContext context)
        {
            context.DotNetBuild("..\\TcOpen.Scaffold.sln", new Cake.Common.Tools.DotNet.Build.DotNetBuildSettings()
            {
                Configuration = "Release"
            });
        }
    }

    [TaskName("Create artifacts")]
    [IsDependentOn(typeof(BuildTask))]
    public sealed class ArtifactTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.DotNetPublish("..\\src\\TcOpen.Scaffold.UI\\TcOpen.Scaffold.UI.csproj",
                new Cake.Common.Tools.DotNet.Publish.DotNetPublishSettings()
                {
                    Configuration = "Release",
                    Framework = "net5.0-windows",
                    PublishSingleFile = true,
                    SelfContained = true,
                    PublishReadyToRun = true,
                    Runtime = "win10-x64",                    
                    OutputDirectory = "..\\src\\TcOpen.Scaffold.UI\\Publish"
                });

            context.ZipFolder("..\\src\\TcOpen.Scaffold.UI\\bin\\Release\\net5.0-windows", "..\\artifacts\\TcOpen.Scaffold.UI.zip");
        }
    }


    [TaskName("Publish release")]
    [IsDependentOn(typeof(ArtifactTask))]
    public sealed class PublishReleaseTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            //var githubActionsProvider = new Cake.Common.Build.GitHubActions.GitHubActionsProvider(context.Environment, context.FileSystem);

            if (GitVersionInformation.BranchName == "dev")
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

                var asset = new ReleaseAssetUpload("..\\artifacts\\TcOpen.Scaffold.UI.zip", "application/zip", new StreamReader("..\\artifacts\\TcOpen.Scaffold.UI.zip").BaseStream, TimeSpan.FromSeconds(3600));

                githubClient.Repository.Release.UploadAsset(release, asset).Wait();
            }
        }
    }


    [TaskName("Default")]
    [IsDependentOn(typeof(PublishReleaseTask))]
    public class DefaultTask : FrostingTask
    {
    }
}