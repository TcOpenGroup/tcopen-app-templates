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
using Cake.Common.Tools.DotNet.Pack;

namespace Build.Scaffolder
{
    public class Const
    {
        public const string BuildGroupName = " : Scaffolder";
        public static readonly IEnumerable<string> Publishable = new List<string>() {"dev", "main", "master","release"};
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
            var artifactsFolder = context.FileSystem.GetDirectory(context.ArtifactsFolder);
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
    public sealed class CreateArtifactTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.PublishCsProjAsArtifact(context, "net5.0-windows", "TcOpen.Scaffold.UI.zip",  $"{context.ProjectRootDirectory}\\src\\TcOpen.Scaffold.UI\\TcOpen.Scaffold.UI.csproj",  $"{context.ProjectRootDirectory}\\src\\TcOpen.Scaffold.UI\\Publish");
            context.PublishCsProjAsArtifact(context, "net5.0", "tco.cli.zip", $"{context.ProjectRootDirectory}\\src\\TcOpen.Scaffold\\tco.csproj", $"{context.ProjectRootDirectory}\\src\\TcOpen.Scaffold\\Publish");
            context.DotNetPack($"{context.ProjectRootDirectory}\\src\\TcOpen.Scaffold\\tco.csproj", new DotNetPackSettings() { OutputDirectory = Path.Combine(context.ArtifactsFolder, "nugets")});
            context.CleanBins(System.IO.Path.GetFullPath(System.IO.Path.Combine(context.Environment.WorkingDirectory.FullPath, "..//templates//mts-s-template//t")));
            context.ZipFolder(System.IO.Path.GetFullPath(System.IO.Path.Combine(context.Environment.WorkingDirectory.FullPath, "..//templates//mts-s-template//t")), System.IO.Path.Combine(context.ArtifactsFolder, "mts-s-template.zip"));
        }

       
    }

    [TaskName("Publish nugets" + Const.BuildGroupName)]
    [IsDependentOn(typeof(CreateArtifactTask))]
    public sealed class PublishNugets : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            if (Const.Publishable.Any(predicate => predicate == GitVersionInformation.BranchName))
            {
                foreach (var nugetFile in Directory
                             .EnumerateFiles(Path.Combine(context.ArtifactsFolder, "nugets"), "*.nupkg")
                             .Select(p => new FileInfo(p)))
                {
                    context.DotNetNuGetPush(nugetFile.FullName,
                        new Cake.Common.Tools.DotNet.NuGet.Push.DotNetNuGetPushSettings()
                        {
                            Source = "https://nuget.pkg.github.com/TcOpenGroup/index.json",
                            ApiKey = System.Environment.GetEnvironmentVariable("gh-public-repos"),
                            SkipDuplicate = true
                        });
                }


                if (GitVersionInformation.BranchName == "release" || GitVersionInformation.BranchName == "main" ||
                    GitVersionInformation.BranchName == "master")
                {
                    foreach (var nugetFile in Directory
                                 .EnumerateFiles(Path.Combine(context.ArtifactsFolder, "nugets"), "*.nupkg")
                                 .Select(p => new FileInfo(p)))
                    {
                        context.DotNetNuGetPush(nugetFile.FullName,
                            new Cake.Common.Tools.DotNet.NuGet.Push.DotNetNuGetPushSettings()
                            {
                                Source = "https://api.nuget.org/v3/index.json",
                                ApiKey = System.Environment.GetEnvironmentVariable("TCOOPENNUGETDOTORGPAT"),
                                SkipDuplicate = true
                            });
                    }
                }
            }
        }
    }

    [TaskName("Publish release" + Const.BuildGroupName)]
    [IsDependentOn(typeof(PublishNugets))]
    public sealed class PublishReleaseTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            if (Const.Publishable.Any(predicate => predicate == GitVersionInformation.BranchName))
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


                foreach (var artifact in Directory.EnumerateFiles(context.ArtifactsFolder, "*.zip").Select(p => new FileInfo(p)))
                {
                    var asset = new ReleaseAssetUpload(artifact.Name, "application/zip", new StreamReader(artifact.FullName).BaseStream, TimeSpan.FromSeconds(3600));
                    githubClient.Repository.Release.UploadAsset(release, asset).Wait();
                }                
            }
        }
    }

    [TaskName("Default" + Const.BuildGroupName)]
    [IsDependentOn(typeof(PublishReleaseTask))]
    public class LastTask : FrostingTask
    {
    }
}
