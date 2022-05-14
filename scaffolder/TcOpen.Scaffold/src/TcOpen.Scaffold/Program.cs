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
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(o =>
                   {
                       var context = new Context(o);
                       context.Execute();
                   });                   
        }

       
    }
}
