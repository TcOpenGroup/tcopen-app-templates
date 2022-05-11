using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;

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

    //get template directory    
    public string TemplateDirectory => Path.GetFullPath(Path.Combine(this.Environment.WorkingDirectory.FullPath, "../templates"));

    public void Clean()
    {        
        var dirs = Directory.EnumerateDirectories(TemplateDirectory, "bin", System.IO.SearchOption.AllDirectories);
        dirs.Concat(Directory.EnumerateDirectories(TemplateDirectory, "obj", System.IO.SearchOption.AllDirectories));
        dirs.Concat(Directory.EnumerateDirectories(TemplateDirectory, "_Boot", System.IO.SearchOption.AllDirectories));
        dirs.Concat(Directory.EnumerateDirectories(TemplateDirectory, "_CompileInfo", System.IO.SearchOption.AllDirectories));

        foreach (var item in dirs)
        {
            Log.Information("Deleting {0}", item);
            Directory.Delete(item, true);
        }

    }

    public BuildContext(ICakeContext context)
        : base(context)
    {        
        Delay = context.Arguments.HasArgument("delay");           
    }    
}



[TaskName("Clean")]
public sealed class HelloTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Clean();        
    }
}

[TaskName("World")]
[IsDependentOn(typeof(HelloTask))]
public sealed class WorldTask : AsyncFrostingTask<BuildContext>
{
    // Tasks can be asynchronous
    public override async Task RunAsync(BuildContext context)
    {
        if (context.Delay)
        {
            context.Log.Information("Waiting...");
            await Task.Delay(1500);
        }

        context.Log.Information("World");
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(WorldTask))]
public class DefaultTask : FrostingTask
{
    
}