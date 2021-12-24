using Helper.Core.Model;
using Helper.Core.Modules;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Helper.Core;
class Program
{
    static int Main(string[] args)
    {
        var appSettings = AppSettings.Create();
        var rootCommand = new RootCommand("FD Helper App")
            {
                new Option<bool>(
                    aliases: new[] { "--package", "-p" },
                    getDefaultValue: () => false,
                    description: "Copy all nuget packages to the local package cache"),
                new Option<string>(
                    aliases: new[] { "--setup", "-s" },
                    description: "Zip setup and copy to publish directory")
            };
        rootCommand.SetHandler(GetCommandHandler(appSettings));

        return rootCommand.Invoke(args);
    }

    private static IModule CopyPackages(AppSettings appSettings) => new CopyPackages(appSettings);
    private static IModule PublishSetup(AppSettings appSettings, string setupName) => new PublishSetup(appSettings, setupName);
    private static IModule NotFound() => new InvalidOption();

    private static Action<bool, string> GetCommandHandler(AppSettings appSettings)
    {
        return (bool package, string setup) =>
        {
            var module = (package, setup) switch
            {
                (true, _) => CopyPackages(appSettings),
                (_, _) when !string.IsNullOrWhiteSpace(setup) => PublishSetup(appSettings, setup),
                _ => NotFound(),
            };

            var result = module.Run();

            Console.Write(result.Valid ? "SUCCESS" : "FAIL");
            Console.WriteLine($" - {result.Message}");
        };
    }
}
