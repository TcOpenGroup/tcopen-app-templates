using Cake.Core;
using Cake.Frosting;
using Cake.Common.Tools.DotNet;
using System;
using System.IO;
using Octokit;

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
        context.ZipFolder("..\\src\\TcOpen.Scaffold.UI\\bin\\Release\\net5.0-windows", "..\\artifacts\\TcOpen.Scaffold.UI.zip");
    }
}


[TaskName("Publish release")]
[IsDependentOn(typeof(ArtifactTask))]
public sealed class PublishReleaseTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {               
        var githubToken = context.Environment.GetEnvironmentVariable("gh-public-repos");
        var githubClient = new GitHubClient(new ProductHeaderValue("TcOpen.Scaffold.UI"));
        githubClient.Credentials = new Credentials(githubToken);
        
        var release = githubClient.Repository.Release.Create(
            "TcOpenGroup",
            "tcopen-app-templates",
            new NewRelease($"v{GitVersionInformation.SemVer}")
            {
                Name = $"v{GitVersionInformation.SemVer}",
                Body = $"Release v{GitVersionInformation.SemVer}",
                Draft = true,
                Prerelease = true
            }
        ).Result;

        var asset = new ReleaseAssetUpload("..\\artifacts\\TcOpen.Scaffold.UI.zip", "application/zip", new StreamReader("..\\artifacts\\TcOpen.Scaffold.UI.zip").BaseStream, TimeSpan.FromSeconds(3600));

        githubClient.Repository.Release.UploadAsset(release, asset).Wait();        
    }
}


[TaskName("Default")]
[IsDependentOn(typeof(PublishReleaseTask))]
public class DefaultTask : FrostingTask
{
}