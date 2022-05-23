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
                       o.Source = string.IsNullOrEmpty(o.Source) ? GitVersionInformation.SemVer : o.Source;
                       var context = new Context(o);
                       context.Execute();
                   });                   
        }
    }
}
