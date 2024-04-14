using System.Diagnostics;
using System.Reflection;
using Develix.Helper.Model;

namespace Develix.Helper.Modules;

internal class DependencyCheck(AppSettings appSettings) : IModule
{
    private readonly AppSettings appSettings = appSettings;

    public ModuleResult Run()
    {
        var process = new Process();
        var startInfo = new ProcessStartInfo()
        {
            FileName = "dotnet.exe",
            Arguments = "list package --format json",
            WorkingDirectory = appSettings.WorkingDirectory,
            RedirectStandardOutput = true,
        };
        process.StartInfo = startInfo;
        process.Start();
        var message = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
            return new(false, $"Exit code is {process.ExitCode}");

        return new(true, message);
    }
}
