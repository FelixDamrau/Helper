using Develix.Helper.Model;
using Develix.Helper.Modules;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Develix.Helper;

class Program
{
    static int Main(string[] args)
    {
        var appSettings = AppSettings.Create();
        var rootCommand = new RootCommand("FD Helper App")
            {
                new Option<bool>(
                    alias: "--package",
                    getDefaultValue: () => false,
                    description: "Copy all nuget packages to the local package cache"),
                new Option<string>(
                    alias: "--setup",
                    description: "Zip setup and copy to publish directory"),
                new Option<bool>(
                    alias: "--deps",
                    getDefaultValue: () => false,
                    description: "Lists any packages that resolve to different versions across all projects.")
            };

        rootCommand.Handler = CommandHandler.Create<bool, string, bool>((package, setup, deps) =>
        {
            IModule module = (package, setup, deps) switch
            {
                (true, _, _) => CopyPackages(appSettings),
                (_, not "", _) => PublishSetup(appSettings, setup),
                (_, _, true) => CheckDependencies(appSettings),
                _ => NotFound(),
            };

            var result = module.Run();
            var message = result.Valid
                ? $"SUCCESS -- {result.Message}"
                : $"FAIL -- {result.Message}";
            Console.WriteLine(message);
        });

        return rootCommand.Invoke(args);
    }

    private static CopyPackages CopyPackages(AppSettings appSettings) => new(appSettings);
    private static PublishSetup PublishSetup(AppSettings appSettings, string setupName) => new(appSettings, setupName);
    private static DependencyCheck CheckDependencies(AppSettings appSettings) => new(appSettings);
    private static InvalidOption NotFound() => new();
}
