using System;
using System.Diagnostics;
using DotNetAssemblyInformer.Core;
using DotNetAssemblyInformer.Core.Report;
using Mono.Options;

namespace DotNetAssemblyInformer
{
    class Program
    {
        static int Main(string[] args)
        {
            var runnerSettings = new RunnerSettings();
            var showHelp = false;
            var displayDetails = false;

            var options = new OptionSet
            {
                { "r|recursive", "Recursive", v => runnerSettings.Recursive=v!=null },
                { "h|help", "Show help", v => showHelp=true },
                { "d|details", "Display Details", v => displayDetails=true }
            };

            try
            {
                runnerSettings.StartPaths = options.Parse(args).ToArray();
            }
            catch (OptionException e)
            {
                Console.Write("invalid options: {0}", e.Message);
                Console.WriteLine("Try `DotNetAssemblyInformer --help' for more information.");
                return -1;
            }

            if (showHelp)
            {
                ShowHelp(options);
                return 0;
            }

            var runner = new Runner(runnerSettings);
            var runnerResult = runner.Run();

            var reporter = !displayDetails ? new BasicConsoleReporter() : new JsonConsoleReporter();

            reporter.Generate(runnerResult);

            if (Debugger.IsAttached)
                Console.ReadKey();

            return runnerResult.OverralSuccess ? 0 : 1;
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: DotNetAssemblyInformer [OPTIONS] path");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
