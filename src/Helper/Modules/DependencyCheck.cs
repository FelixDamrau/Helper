using System.Diagnostics;
using System.Text.Json;
using Develix.Helper.Model;
using Develix.Helper.Model.Dependencies;
using Humanizer;

namespace Develix.Helper.Modules;

public class DependencyCheck(AppSettings appSettings) : IModule
{
    private readonly AppSettings appSettings = appSettings;
    private readonly JsonSerializerOptions serializerOptions = new() { PropertyNameCaseInsensitive = true, };

    public ModuleResult Run()
    {
        var process = new Process();
        var startInfo = new ProcessStartInfo()
        {
            FileName = "dotnet.exe",
            Arguments = "list package --include-transitive --format json",
            WorkingDirectory = appSettings.WorkingDirectory,
            RedirectStandardOutput = true,
        };
        process.StartInfo = startInfo;
        process.Start();
        var listPackageJson = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
            return new(false, $"Exit code is {process.ExitCode}");

        var dependencyCheckModel = JsonSerializer.Deserialize<DependencyCheckModel>(listPackageJson, serializerOptions)
            ?? throw new InvalidOperationException($"Could not deserialize data model!");
        var res = new DependencyCheckResolver();
        foreach (var project in dependencyCheckModel.Projects ?? [])
        {
            foreach (var package in project.Frameworks.First().TopLevelPackages ?? [])
                res.Add(project, package);

            foreach (var package in project.Frameworks.First().TransitivePackages ?? [])
                res.Add(project, package);
        }

        var conflictCount = res.ShowConflicts();

        var message = $"""
            {"project".ToQuantity(dependencyCheckModel.Projects?.Count ?? 0)} analyzed.
            {"conflict".ToQuantity(conflictCount)} found.
            """;
        return new(true, message);
    }
}
