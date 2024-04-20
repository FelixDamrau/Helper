using System.IO.Compression;
using Develix.Essentials.Core;
using Develix.Helper.Model;
using Develix.Helper.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Develix.Helper.Commands;

public class PublishSetupCommand(AppSettings appSettings) : Command<PublishSetupSettings>
{
    private readonly AppSettings appSettings = appSettings;

    public override int Execute(CommandContext context, PublishSetupSettings settings)
    {
        var result = Run(
            settings.SetupName,
            settings.PublishSetupRoot ?? appSettings.PublishSetupRoot,
            settings.SetupDirectoryIdentifier ?? appSettings.SetupDirectoryIdentifier);

        return ModuleResultRenderer.Display(result);
    }

    private static ModuleResult Run(string setupName, string publishSetupRoot, string setupDirectoryIdentifier)
    {
        var setupDirectoryResult = GetSetupDirectory(setupDirectoryIdentifier);
        if (!setupDirectoryResult.Valid)
            return new(false, setupDirectoryResult.Message);

        var publishDirectory = GetPublishDirectory(setupName, publishSetupRoot);
        if (!publishDirectory.Valid)
            return new(false, publishDirectory.Message);

        var publishResult = PublishZippedSetup(setupName, setupDirectoryResult.Value, publishDirectory.Value);
        return publishResult.Valid
            ? new(true, $"Published Setup '{setupName}' to '{publishSetupRoot}'")
            : new(false, publishResult.Message);
    }

    private static Result<string> GetSetupDirectory(string setupDirectoryIdentifier)
    {
        var root = Directory.GetCurrentDirectory();
        var subDirectories = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);
        var setupDirectory = subDirectories.FirstOrDefault(d => d.EndsWith(setupDirectoryIdentifier, StringComparison.OrdinalIgnoreCase));
        return setupDirectory is null
            ? Result.Fail<string>($"Setup directory '{setupDirectoryIdentifier}' not found! (Directories searched: [{string.Join(", ", subDirectories)}]")
            : Result.Ok(setupDirectory);
    }

    private static Result<string> GetPublishDirectory(string setupName, string publishSetupRoot)
    {
        if (!Directory.Exists(publishSetupRoot))
            return Result.Fail<string>($"The publish setup root directory '{publishSetupRoot}' does not exist.");
        var fullPublishSetupPath = Path.Combine(publishSetupRoot, setupName);
        if (!Directory.Exists(fullPublishSetupPath))
            Directory.CreateDirectory(fullPublishSetupPath);
        return Result.Ok(fullPublishSetupPath);
    }

    private static Result PublishZippedSetup(string setupName, string setupDirectory, string publishDirectory)
    {
        try
        {
            var build = (DateTime.Now - new DateTime(1970, 1, 1)).TotalDays;
            ZipFile.CreateFromDirectory(setupDirectory, publishDirectory + $"\\Setup {setupName} - {GetSetupId()}.zip");
        }
        catch (Exception ex)
        {
            return Result.Fail("Publish setup failed!" + Environment.NewLine + ex.Message);
        }
        return Result.Ok();
    }

    private static string GetSetupId() => (DateTime.Now - new DateTime(1970, 1, 1)).TotalDays.ToString("0.00000");
}
