using Develix.Helper.Commands;
using Develix.Helper.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Develix.Helper;

class Program
{
    static int Main(string[] args)
    {
        var appSettings = AppSettings.Create();
        var registrations = new ServiceCollection();
        registrations.AddSingleton(appSettings);
        var registrar = new TypeRegistrar(registrations);
        var app = new CommandApp(registrar);
        app.Configure(config =>
            {
                config
                    .AddCommand<CopyPackagesCommand>("package")
                    .WithDescription("Copy all nuget packages to the local package cache.");
                config
                    .AddCommand<PublishSetupCommand>("setup")
                    .WithDescription("Publish setup to the publish directory");
                config
                    .AddCommand<DependencyCheckCommand>("deps")
                    .WithDescription("Check the package references of a solution");
            });
        return app.Run(args);
    }
}
