using System.Diagnostics;
using Develix.Helper.Model;

namespace Develix.Helper.Modules;

internal class DependencyCheck : IModule
{
    public ModuleResult Run()
    {
        var process = new Process();
        var startInfo = new ProcessStartInfo()
        {
            FileName = "dotnet.exe",
            Arguments = "list package --format json",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        process.StartInfo = startInfo;
        process.Start();
        var message = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        var valid = process.ExitCode == 0;
        return new(valid, message);
    }
}
