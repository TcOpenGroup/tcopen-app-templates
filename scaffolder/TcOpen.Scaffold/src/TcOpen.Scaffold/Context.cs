using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TcOpen.Scaffold
{
    public class Context
    {
        public Context(Options options)
        {
            Options = options;
            if (options.OutputDirectory != null)
            {
                CurrentDirectory = options.OutputDirectory;
                if (Directory.Exists(options.OutputDirectory))                
                {
                    try
                    {
                        Directory.CreateDirectory(options.OutputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unable to create output directory [{ex.Message}]");                       
                    }
                }
            }
            else
            {
                CurrentDirectory = Environment.CurrentDirectory;
            }
        }
             
        public void Execute(Options options)
        {            
            if(options.Source == "release")
            { 
                ReplaceTemplateTags(DownloadReleaseTemplate());
            }
            else if(options.Source == "repository")
            {
                CopyTemplateFolder(DownloadRepositorySource());
                ReplaceTemplateTags(Path.Combine(CurrentDirectory, $"{Options.ProjectName}"));
            }
            else
            {
                throw new Exception("Unknown source '{options.Source}' set 'release' or 'repository'");
            }

            Process.Start("explorer.exe", Path.Combine(CurrentDirectory, this.Options.ProjectName));
        }
       
        private readonly string CurrentDirectory;
        
        private readonly Options Options;
     
        public string DownloadRepositorySource()
        {            
            var zippedBranchFile = Path.Combine(CurrentDirectory, $"{Options.BranchOrTag}.zip");
            var templateProjectFolder = Path.Combine(CurrentDirectory, $"tcopen-app-templates-{Options.BranchOrTag}", "templates", Options.TemplateName, "t");

            Console.WriteLine($"Downloading from repository '{Options.BranchOrTag}'...");

            try
            {                
                using (var client = new WebClient())
                {
                    client.DownloadFile($"https://github.com/TcOpenGroup/tcopen-app-templates/archive/refs/tags/{Options.BranchOrTag}.zip", $"{zippedBranchFile}");
                }

                Console.WriteLine($"Unpacking '{zippedBranchFile}' to '{CurrentDirectory}'");

                System.IO.Compression.ZipFile.ExtractToDirectory(zippedBranchFile, CurrentDirectory);
                File.Delete(zippedBranchFile);
                return templateProjectFolder;
            }
            catch (Exception)
            {
                Console.WriteLine($"Tag link for '{Options.BranchOrTag}' does not exists. Trying branch link. ");                
            }

            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile($"https://github.com/TcOpenGroup/tcopen-app-templates/archive/refs/heads/{Options.BranchOrTag}.zip", $"{zippedBranchFile}");                    
                }

                Console.WriteLine($"Unpacking '{zippedBranchFile}' to '{CurrentDirectory}'");

                System.IO.Compression.ZipFile.ExtractToDirectory(zippedBranchFile, CurrentDirectory);
                File.Delete(zippedBranchFile);
                return templateProjectFolder;
            }
            catch (Exception ex)
            {
                throw new Exception($"Neither branch or tag link '{Options.BranchOrTag}' found in the repository.");
            }

            
        }

        public string DownloadReleaseTemplate()
        {
            Console.WriteLine($"Downloading from release '{Options.Release}'...");
            var zippedReleaseFile = Path.Combine(CurrentDirectory, $"{Options.Release}.zip");
            var unpackFolder = Path.Combine(CurrentDirectory, Options.ProjectName);
          
            using (var client = new WebClient())
            {
                client.DownloadFile($"https://github.com/TcOpenGroup/tcopen-app-templates/releases/download/{Options.Release}/{Options.TemplateName}.zip", $"{zippedReleaseFile}");
            }

            Console.WriteLine($"Unpacking '{zippedReleaseFile}' to '{unpackFolder}'");
            System.IO.Compression.ZipFile.ExtractToDirectory(zippedReleaseFile, unpackFolder);
             File.Delete(zippedReleaseFile);

            return unpackFolder;
        }

        public void CopyTemplateFolder(string sourceFolder)
        {                        
            var destinationFolder = Path.Combine(CurrentDirectory, $"{Options.ProjectName}");
            DirectoryCopy(sourceFolder, destinationFolder, true);
            Directory.Delete(Path.Combine(CurrentDirectory, $"tcopen-app-templates-{Options.BranchOrTag}"), true);
        }

        public void ReplaceFileNames(string projectFolder)
        {            
            var files = Directory.EnumerateFiles(projectFolder, "*.*", SearchOption.AllDirectories).Select(p => new FileInfo(p));

            foreach (var file in files)
            {
                var newFileName = file.FullName.Replace("x_template_x", Options.ProjectName);
                if (newFileName != file.FullName)
                {
                    File.Move(file.FullName, newFileName);
                }
               
            }
        }

        public void ReplaceTemplateTags(string projectFolder)
        {            
            RenameDirectories(projectFolder);
            ReplaceFileNames(projectFolder);

            var files = Directory.EnumerateFiles(projectFolder, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string text = File.ReadAllText(file);
                text = text.Replace("x_template_x", Options.ProjectName);
                File.WriteAllText(file, text);
            }
        }

        private void RenameDirectories(string directory)
        {            
            var projectFolder = Path.Combine(CurrentDirectory, $"{Options.ProjectName}");
            var directories = Directory.EnumerateDirectories(directory, "*.*", SearchOption.TopDirectoryOnly).Select(p => new DirectoryInfo(p));

            foreach (var dir in directories)
            {
                var newFolderName = dir.FullName.Replace("x_template_x", Options.ProjectName);

                if (newFolderName != dir.FullName)
                {
                    Directory.Move(dir.FullName, newFolderName);
                    RenameDirectories(newFolderName);
                }
                else
                {
                    RenameDirectories(dir.FullName);
                }

                
            }
        }

        private static void DirectoryCopy(
        string sourceDirName, string destDirName, bool copySubDirs)
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
                file.CopyTo(temppath, false);
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
}
