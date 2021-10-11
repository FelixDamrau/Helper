using System.IO.Compression;
using Develix.Essentials.Core;
using Helper.Core.Model;

namespace Helper.Core.Modules;
public class PublishSetup : IModule
{
    private readonly string setupDirectoryIdentifier;
    private readonly string publishSetupRoot;
    private readonly string setupName;

    public PublishSetup(AppSettings appSettings, string setupName)
    {
        setupDirectoryIdentifier = appSettings.SetupDirectoryIdentifier;
        publishSetupRoot = appSettings.PublishSetupRoot;
        this.setupName = setupName;
    }

    public ModuleResult Run()
    {
        var setupDirectoryResult = GetSetupDirectory();
        if (!setupDirectoryResult.Valid)
            return new(false, setupDirectoryResult.Message);

        var publishDirectory = GetPublishDirectory();
        if (!publishDirectory.Valid)
            return new(false, publishDirectory.Message);

        var publishResult = PublishZippedSetup(setupDirectoryResult.Value, publishDirectory.Value);
        return publishResult.Valid
            ? new(true, $"Published Setup '{setupName}' to '{publishSetupRoot}'")
            : new(false, publishResult.Message);
    }

    private Result<string> GetSetupDirectory()
    {
        var root = Directory.GetCurrentDirectory();
        var subDirectories = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);
        var setupDirectory = subDirectories.FirstOrDefault(d => d.EndsWith(setupDirectoryIdentifier, StringComparison.OrdinalIgnoreCase));
        return setupDirectory is null 
            ? Result.Fail<string>($"Setup directory '{setupDirectoryIdentifier}' not found! (Directories searched: [{string.Join(", ", subDirectories)}]") 
            : Result.Ok(setupDirectory);
    }

    private Result<string> GetPublishDirectory()
    {
        if (!Directory.Exists(publishSetupRoot))
            return Result.Fail<string>($"The publish setup root directory '{publishSetupRoot}' does not exist.");
        var fullPublishSetupPath = Path.Combine(publishSetupRoot, setupName);
        if (!Directory.Exists(fullPublishSetupPath))
            Directory.CreateDirectory(fullPublishSetupPath);
        return Result.Ok(fullPublishSetupPath);
    }

    private Result PublishZippedSetup(string setupDirectory, string publishDirectory)
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
