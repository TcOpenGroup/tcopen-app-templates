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
             
        public void Execute()
        {                        
            DownloadBranchAndExtractBranch();            
            CopyTemplateFolder();
            ReplaceTemplateTags();
            Process.Start("explorer.exe", Path.Combine(CurrentDirectory, this.Options.ProjectName));
            ExecuteFirstBuildScript();
        }

        private void ExecuteFirstBuildScript()
        {
            var firstBuildScriptPath = Path.Combine(CurrentDirectory, this.Options.ProjectName, "FirstBuild.ps1");
            if (File.Exists(firstBuildScriptPath))
            {
                File.GetAttributes(firstBuildScriptPath);
                string strCmdText = Path.Combine(firstBuildScriptPath);
                var process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = @"C:\windows\system32\windowspowershell\v1.0\powershell.exe";
                process.StartInfo.Arguments = "\"&'" + strCmdText + "'\"";

                process.Start();
                string s = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                using (StreamWriter outfile = new StreamWriter("first_build.txt", true))
                {
                    outfile.Write(s);
                }

            }
        }

        private readonly string CurrentDirectory;
        
        private readonly Options Options;
     
        public void DownloadBranchAndExtractBranch()
        {            
            var zippedBranchFile = Path.Combine(CurrentDirectory, $"{Options.Source}.zip");

            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile($"https://github.com/TcOpenGroup/tcopen-app-templates/archive/refs/tags/{Options.Source}.zip", $"{zippedBranchFile}");
                }

                System.IO.Compression.ZipFile.ExtractToDirectory(zippedBranchFile, CurrentDirectory);
                File.Delete(zippedBranchFile);
                return;
            }
            catch (Exception)
            {
                Console.WriteLine($"Tag link for '{Options.Source}' does not exists. Trying branch link. ");                
            }

            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile($"https://github.com/TcOpenGroup/tcopen-app-templates/archive/refs/heads/{Options.Source}.zip", $"{zippedBranchFile}");                    
                }

                System.IO.Compression.ZipFile.ExtractToDirectory(zippedBranchFile, CurrentDirectory);
                File.Delete(zippedBranchFile);
                return;
            }
            catch (Exception ex)
            {
                throw new Exception($"Neither branch or tag link '{Options.Source}' found in the repository.");
            }            
        }

        public void CopyTemplateFolder()
        {            
            var sourceFolder = Path.Combine(CurrentDirectory, $"tcopen-app-templates-{Options.Source}", "templates", Options.TemplateName, "t");
            var destinationFolder = Path.Combine(CurrentDirectory, $"{Options.ProjectName}");
            DirectoryCopy(sourceFolder, destinationFolder, true);
            Directory.Delete(Path.Combine(CurrentDirectory, $"tcopen-app-templates-{Options.Source}"), true);
        }

        public void ReplaceFileNames()
        {
            var projectFolder = Path.Combine(CurrentDirectory, $"{Options.ProjectName}");

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

        public void ReplaceTemplateTags()
        {
            var projectFolder = Path.Combine(CurrentDirectory, $"{Options.ProjectName}");

            RenameDirectories(projectFolder);
            ReplaceFileNames();

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
