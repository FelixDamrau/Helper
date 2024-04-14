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
        var rootCommand = new RootCommand("Develix Helper App")
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
                    description: "Lists any packages that resolve to different versions across all projects."),
                new Option<string>(
                    alias: "--workDir",
                    getDefaultValue: () => ".",
                    description: "Sets the working directory."),
            };

        rootCommand.Handler = CommandHandler.Create<bool, string, bool, string>((package, setup, deps, workDir) =>
        {
            IModule module = (package, setup, deps) switch
            {
                (true, _, _) => CopyPackages(appSettings),
                (_, not "", _) => PublishSetup(appSettings, setup),
                (_, _, true) => CheckDependencies(workDir),
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
    private static DependencyCheck CheckDependencies(string workingDirectory) => new(workingDirectory);
    private static InvalidOption NotFound() => new();
}
