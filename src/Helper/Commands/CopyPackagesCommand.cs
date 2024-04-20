using Develix.Helper.Model;
using Develix.Helper.Settings;
using Humanizer;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Develix.Helper.Commands;

public class CopyPackagesCommand : Command<CopyPackagesSettings>
{
    private readonly string localPackageCache;

    public CopyPackagesCommand(AppSettings appSettings) => localPackageCache = appSettings.LocalPackageCache;

    public override int Execute(CommandContext context, CopyPackagesSettings settings)
    {
        var result = Run();
        return CommandResultRenderer.Display(result);
    }

    private CommandResult Run()
    {
        try
        {
            if (!Directory.Exists(localPackageCache))
                return new CommandResult(false, $"The directory of the local package cache '{localPackageCache}' does not exist!");

            var directory = Directory.GetCurrentDirectory();
            var packageFiles = Directory.GetFiles(directory, "*.nupkg", SearchOption.AllDirectories);
            var count = packageFiles.Length;

            Console.WriteLine($"Found {"package".ToQuantity(count)}.");

            foreach (var filePath in packageFiles)
            {
                var fileName = Path.GetFileName(filePath);
                Console.WriteLine($"Copy nuget file: {fileName}");
                var destinationFile = Path.Combine(localPackageCache, fileName);
                File.Copy(filePath, destinationFile, true);
            }
            return new CommandResult(true, "package".ToQuantity(count) + " copied successfully.");
        }
        catch (Exception exception)
        {
            return new CommandResult(false, $"Copying packages failed. [{exception.Message}]");
        }
    }
}
