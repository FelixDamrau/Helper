using Develix.Helper.Commands;
using Develix.Helper.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Develix.Helper;

class Program
{
    static int Main(string[] args)
    {
        var registrar = InitRegistrar();
        var app = new CommandApp(registrar);
        app.Configure(ConfigureApp);
        return app.Run(args);
    }

    private static TypeRegistrar InitRegistrar()
    {
        var appSettings = AppSettings.Create();
        var registrations = new ServiceCollection();
        registrations.AddSingleton(appSettings);
        var registrar = new TypeRegistrar(registrations);
        return registrar;
    }

    private static void ConfigureApp(IConfigurator config)
    {
        config.SetApplicationName("Helper");
        config
            .AddCommand<CopyPackagesCommand>("package")
            .WithDescription("Copy all nuget packages to the local package cache.");
        config
            .AddCommand<PublishSetupCommand>("setup")
            .WithDescription("Publish setup to the publish directory");
        config
            .AddCommand<DependencyCheckCommand>("deps")
            .WithDescription("Check the package references of a solution");
    }
}
