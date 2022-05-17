using Build;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TCatSysManagerLib;

public class BuildContext : FrostingContext
{
    public string RootRepository => Path.GetFullPath(Path.Combine(Environment.WorkingDirectory.FullPath, ".."));
    public string TemplateDirectory => Path.GetFullPath(Path.Combine(Environment.WorkingDirectory.FullPath, "../t"));

    public string TemplateTestsDirectory => Path.GetFullPath(Path.Combine(TemplateDirectory, "tests"));

    public IEnumerable<string> TemplateTestFiles => Directory.EnumerateFiles(TemplateTestsDirectory, "*test*.dll", SearchOption.AllDirectories)
                                                    .Concat(Directory.EnumerateFiles(TemplateTestsDirectory, "*Test*.dll", SearchOption.AllDirectories));

    public IEnumerable<string> TemplateSolutions => Directory.EnumerateFiles(TemplateDirectory, "x_template_x.sln", SearchOption.AllDirectories);

    public IEnumerable<string> TemplateProjects => Directory.EnumerateFiles(TemplateDirectory, "*.csproj", SearchOption.AllDirectories);

    public IEnumerable<string> TemplateXaeProjects => Directory.EnumerateFiles(TemplateDirectory, "*.tsproj", SearchOption.AllDirectories);

    public void RunIvc(string solutionFile)
    {
        EnvDTE.DTE dte = GetVsInstance(solutionFile);

        var solutionFileInfo = new FileInfo(solutionFile).Directory;
        var pathToIvc = Path.Combine(solutionFileInfo.FullName, "_Vortex\\builder\\vortex.compiler.console.exe");
       // var vsProcess = ProcessRunner.Start($"{Environment.GetEnvironmentVariable("TcoDevenv")}", new Cake.Core.IO.ProcessSettings() { Arguments = $"{solutionFile}" });
        var ivcProcess = ProcessRunner.Start(pathToIvc, new Cake.Core.IO.ProcessSettings() { Arguments = $"-s {solutionFile}" });

        ivcProcess.WaitForExit();
        
    }

    public void RestorePackages(string cprojFile)
    {
        var dotnetRestore = new Cake.Common.Tools.DotNetCore.Restore.DotNetCoreRestorer(FileSystem, Environment, ProcessRunner, Tools, Log);
        dotnetRestore.Restore(new FileInfo(cprojFile).DirectoryName, new Cake.Common.Tools.DotNet.Restore.DotNetRestoreSettings());
    }
       
    private void GetProjectWithName(object proj, string name, ref dynamic prj)
    {       
        switch(proj)
        {
            case Projects p:
                for (int i = 1; i <= p.Count; i++)
                {                    
                    if (p.Item(i).Name == name)
                    {
                        prj = p.Item(i);
                        break;
                    }
                    else
                    {
                        GetProjectWithName(p.Item(i).ProjectItems, name, ref prj);
                    }
                }                
                break;
            case ProjectItems p:
                for (int i = 1; i <= p.Count; i++)
                {                    
                    if (p.Item(i).Name == name)
                    {
                        prj = p.Item(i);
                        break;
                    }
                    else
                    {
                        GetProjectWithName(p.Item(i).SubProject, name, ref prj);
                    }
                }

                break;
        }            
    }

    public void LoadXaeToPlc(string solutionFile)
    {        
        EnvDTE.DTE dte = GetVsInstance(solutionFile);
        
        dynamic project = new object();
        GetProjectWithName(dte.Solution.Projects, "x_template_x-xae", ref project);
        TcSysManager sysManager = (TcSysManager)project.Object.Object;       
        TcConfigManager sysConfigManager = (TcConfigManager)project.Object.Object.ConfigurationManager;
        ITcEnvironment environment = (ITcEnvironment)project.Object.Object.Environment;
        ITcSysManager2 sm2 = (ITcSysManager2)sysManager;
        sysConfigManager.ActiveTargetPlatform = "TwinCAT RT (x64)";
        sm2.SetTargetNetId(Environment.GetEnvironmentVariable("Tc3Target"));
        ITcSmTreeItem plcProjectRootItem = sysManager.LookupTreeItem("TIPC^x_template_xPlc");
        ITcPlcProject iecProjectRoot = (ITcPlcProject)plcProjectRootItem;
        iecProjectRoot.BootProjectAutostart = true;
        sysManager.ActivateConfiguration();
        sysManager.StartRestartTwinCAT();
    }
   
    public void CloseVsSolution(string solutionFile)
    {
        EnvDTE.DTE dte = GetVsInstance(solutionFile);
        dte.Quit();
    }

    private EnvDTE80.DTE2 GetVsInstance(string solutionPath)
    {

        EnvDTE80.DTE2 DTE = null;
        if (!MessageFilter.IsRegistered)
            MessageFilter.Register();

        var rot = ROTAccess.GetRunningDTETable();

        if (rot.ToList().Count(p => p.Key.SolutionPath.ToUpperInvariant() == solutionPath.ToUpperInvariant()) > 1)
        {
            throw new Exception($"The solution {solutionPath} is opened by multiple instances of visual studio.");
        }

        DTE = rot.FirstOrDefault(p => p.Key.SolutionPath.ToUpperInvariant() == solutionPath.ToUpperInvariant()).Value;
       
        if (DTE == null)
        {
            // 2019
            Type t = System.Type.GetTypeFromProgID("VisualStudio.DTE.16.0");

            // 2017
            if (t == null)
                t = System.Type.GetTypeFromProgID("VisualStudio.DTE.15.0");

            DTE = System.Activator.CreateInstance(t) as EnvDTE80.DTE2;
            DTE.Solution.Open(solutionPath);
            return DTE;
        }



        return DTE;
    }
    public void Clean()
    {
        // Clean directories
        Directory.EnumerateDirectories(TemplateDirectory, "bin", SearchOption.AllDirectories)
        .Concat(Directory.EnumerateDirectories(TemplateDirectory, "obj", SearchOption.AllDirectories))
        .Concat(Directory.EnumerateDirectories(TemplateDirectory, "_Boot", SearchOption.AllDirectories))
        .Concat(Directory.EnumerateDirectories(TemplateDirectory, "_CompileInfo", SearchOption.AllDirectories))
        .Concat(Directory.EnumerateDirectories(TemplateDirectory, "_generated", SearchOption.AllDirectories))
        .Concat(Directory.EnumerateDirectories(TemplateDirectory, "_meta", SearchOption.AllDirectories))
        .ToList().ForEach(dir => { Directory.Delete(dir, true); });
    }

    public BuildContext(ICakeContext context)
        : base(context)
    {

    }
}
