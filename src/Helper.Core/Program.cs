using Helper.Core.Model;
using Helper.Core.Modules;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics.Contracts;
using System.Reflection.Metadata;

namespace Helper.Core
{
    class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand("FD Helper App")
            {
                new Option<bool>(
                    alias: "--package",
                    getDefaultValue: () => false,
                    description: "Copy all nuget packages to the local package cache")
            };

            rootCommand.Handler = CommandHandler.Create<bool>((package) =>
            {
                var result = (package) switch
                {
                    (true) => CopyPackages(),
                    _ => NotFound(),
                };

                Console.Write(result.Valid ? "SUCCESS" : "FAIL");
                Console.WriteLine($" - {result.Message}");
            });

            return rootCommand.Invoke(args);
        }

        private static ModuleResult CopyPackages()
        {
            var copy = new CopyPackages();
            return copy.Run();
        }

        private static ModuleResult NotFound() => new ModuleResult(false, "Could not find option to execute. :'(");
    }
}
