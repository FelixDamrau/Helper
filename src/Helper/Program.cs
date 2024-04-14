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
                    description: "Zip setup and copy to publish directory")
            };

        rootCommand.Handler = CommandHandler.Create<bool, string>((package, setup) =>
        {
            IModule module = (package, setup) switch
            {
                (true, _) => CopyPackages(appSettings),
                (_, not "") => PublishSetup(appSettings, setup),
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
    private static InvalidOption NotFound() => new();
}
