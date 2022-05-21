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
            var assemblyFile = Path.Combine(Environment.CurrentDirectory, "l\\default.artifacts.TcOpen.Scaffold.UI\\TcOpen.Scaffold.UI.exe");
            string version = "0.0.0";
            if (File.Exists(assemblyFile))
            {
                System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyFile);
                version = fvi.FileVersion;
            }

            Updater.UpdateToNewestRelease(Updater.GetNewestVersion(version));

            Process.Start(assemblyFile);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, true);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }      
    }

    public class Updater
    {
        public static Release GetNewestVersion(string currentVersion)
        {
            var githubToken = Environment.GetEnvironmentVariable("gh-public-repos");
            var githubClient = new GitHubClient(new ProductHeaderValue(Guid.NewGuid().ToString()));
            if (githubToken != null)
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

            var latestVersion = versions.Where(p => p.Version > currentSemVersion).OrderBy(p => p.Version).LastOrDefault();

            return latestVersion.Release;
        }

        public static void UpdateToNewestRelease(Release release)
        {
            //Where(p => p.Name == "default.artifacts.TcOpen.Scaffold.UI.zip")
            var assets = release.Assets.Select(p => new { Name = p.Name, BrowserDownloadUrl = p.BrowserDownloadUrl });
            var entryAssembly = Assembly.GetEntryAssembly().Location;
            var currentDirectory = new FileInfo(entryAssembly).Directory.FullName;
            var outputDirectory = Path.Combine(currentDirectory, "l");

            if (!Directory.Exists(outputDirectory))
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
                    var outputFileName = new FileInfo(Path.Combine(outputDirectory, item.Name));
                    var fileNameWithoutExtension = outputFileName.FullName.Remove(outputFileName.FullName.Length - outputFileName.Extension.Length, outputFileName.Extension.Length);
                    client.DownloadFile(item.BrowserDownloadUrl, Path.Combine(outputDirectory, outputFileName.FullName));
                    using(var zip = ZipFile.OpenRead(outputFileName.FullName))
                    {
                        zip.ExtractToDirectory(Path.Combine(outputDirectory, fileNameWithoutExtension));
                    }
                }
            }
        }

    }
}
