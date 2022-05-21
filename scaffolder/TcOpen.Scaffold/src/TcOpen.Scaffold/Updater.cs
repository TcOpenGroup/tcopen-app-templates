using Octokit;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;

namespace TcOpen.Scaffold
{
    public class Updater
    {
        public static Release GetNewestVersion(string currentVersion)
        {
            var githubToken = Environment.GetEnvironmentVariable("gh-public-repos");
            var githubClient = new GitHubClient(new ProductHeaderValue(Guid.NewGuid().ToString()));
            if(githubToken != null)
            { 
                githubClient.Credentials = new Credentials(githubToken);
            }

            var releases = githubClient.Repository.Release.GetAll("TcOpenGroup", "tcopen-app-templates").Result.ToList();

            var versions = releases.Select(p =>
            {
                Semver.SemVersion semVersion = null;

                if (!Semver.SemVersion.TryParse(p.Name, Semver.SemVersionStyles.Any, out semVersion))
                {
                    semVersion = Semver.SemVersion.ParsedFrom(0, 0, 0);
                }

                return new { Version = semVersion, Release = p };
            });

            Semver.SemVersion currentSemVersion;
            Semver.SemVersion.TryParse(currentVersion, Semver.SemVersionStyles.Any, out currentSemVersion);

            var latestVersion = versions.Where(p => p.Version >= currentSemVersion).OrderBy(p => p.Version).LastOrDefault();

            return latestVersion.Release;
        }

        public static void UpdateToNewestRelease(Release release)
        {
            //Where(p => p.Name == "default.artifacts.TcOpen.Scaffold.UI.zip")
            var assets = release.Assets.Select(p => new { Name = p.Name, BrowserDownloadUrl = p.BrowserDownloadUrl });
            var entryAssembly = Assembly.GetEntryAssembly().Location;
            var currentDirectory = new FileInfo(entryAssembly).Directory.FullName;
            var outputDirectory = Path.Combine(currentDirectory, "update");           
            
            if(!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            else
            {
                Directory.Delete(outputDirectory, true);
                Directory.CreateDirectory(outputDirectory);
            }

            foreach (var item in assets)
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(item.BrowserDownloadUrl, Path.Combine(outputDirectory, item.Name));
                }
            }                     
        }

    }
}
