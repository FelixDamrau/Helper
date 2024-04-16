using System.Diagnostics;
using System.Text.Json;
using Develix.Helper.Model;
using Develix.Helper.Model.Dependencies;
using Develix.Helper.Settings;
using Humanizer;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Develix.Helper.Commands;

public class DependencyCheckCommand : Command<DependencyCheckSettings>
{
    private readonly JsonSerializerOptions serializerOptions = new() { PropertyNameCaseInsensitive = true, };

    public override int Execute(CommandContext context, DependencyCheckSettings settings)
    {
        var result = Run(settings.WorkingDirectory ?? ".", settings.ExcludeProjects, settings.Framework);
        AnsiConsole.WriteLine(result.Message.EscapeMarkup());
        return result.Valid ? 0 : -1;
    }

    private ModuleResult Run(string workingDirectory, string? excludeProjects, string? framework)
    {
        var process = InitProcess(workingDirectory, framework);
        process.Start();
        var listPackageJson = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
            return new(false, $"Exit code is {process.ExitCode}");

        var dependencyCheckModel = JsonSerializer.Deserialize<DependencyCheckModel>(listPackageJson, serializerOptions)
            ?? throw new InvalidOperationException($"Could not deserialize data model!");
        
        var res = InitResolver(excludeProjects, dependencyCheckModel);
        var conflictCount = res.ShowConflicts();

        var message = $"""
            {"project".ToQuantity(dependencyCheckModel.Projects?.Count ?? 0)} analyzed.
            {"conflict".ToQuantity(conflictCount)} found.
            """;
        return new(true, message);
    }

    private static Process InitProcess(string workingDirectory, string? framework)
    {
        var process = new Process();
        var frameworkFlag = framework is null
            ? null
            : $" --framework {framework}";
        var startInfo = new ProcessStartInfo()
        {
            FileName = "dotnet.exe",
            Arguments = $"list package{frameworkFlag} --include-transitive --format json",
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
        };
        process.StartInfo = startInfo;
        return process;
    }

    private static DependencyCheckResolver InitResolver(string? excludeProjects, DependencyCheckModel dependencyCheckModel)
    {
        var resolver = new DependencyCheckResolver();
        var projects = dependencyCheckModel.Projects ?? [];
        var relevantProjects = excludeProjects is null
            ? projects
            : projects.Where(p => !p.Path.Contains(excludeProjects));
        foreach (var project in relevantProjects)
        {
            foreach (var package in project.Frameworks?.First().TopLevelPackages ?? [])
                resolver.Add(project, package);

            foreach (var package in project.Frameworks?.First().TransitivePackages ?? [])
                resolver.Add(project, package);
        }

        return resolver;
    }
}
