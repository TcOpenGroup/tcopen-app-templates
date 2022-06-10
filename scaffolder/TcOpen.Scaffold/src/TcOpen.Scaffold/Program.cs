using CommandLine;
using System;
using System.IO;
using System.Net;

namespace TcOpen.Scaffold
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Running version: {GitVersionInformation.SemVer}");
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(o =>
                   {
                       o.BranchOrTag = string.IsNullOrEmpty(o.BranchOrTag) ? GitVersionInformation.SemVer : o.BranchOrTag;
                       o.Release = string.IsNullOrEmpty(o.Release) ? GitVersionInformation.SemVer : o.Release;
                       var context = new Context(o);
                       context.Execute(o);
                   });                   
        }
    }
}
