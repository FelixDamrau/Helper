using System.Diagnostics;
using System.Text.Json;
using Develix.Helper.Model;
using Develix.Helper.Model.Dependencies;
using Develix.Helper.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Develix.Helper.Commands;

public class DependencyCheckCommand : Command<DependencyCheckSettings>
{
    private readonly JsonSerializerOptions serializerOptions = new() { PropertyNameCaseInsensitive = true, };

    public override int Execute(CommandContext context, DependencyCheckSettings settings)
    {
        AnsiConsole.MarkupLine("[b]Info:[/] Ensure that the dependencies and tools of the project are restored.");
        var result = Run(settings.WorkingDirectory ?? ".", settings.ExcludeProjects, settings.Framework);
        return CommandResultRenderer.Display(result);
    }

    private CommandResult Run(string workingDirectory, string? excludeProjects, string? framework)
    {
        var process = InitProcess(workingDirectory, framework);
        process.Start();
        var listPackageJson = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
            return new(false, $"dotnet process failed. Exit code was {process.ExitCode}");

        var dependencyCheckModel = JsonSerializer.Deserialize<DependencyCheckModel>(listPackageJson, serializerOptions);
        if(dependencyCheckModel is null)
            return new(false, $"Could not deserialize {nameof(DependencyCheckModel)}.");

        var resolver = InitResolver(excludeProjects, dependencyCheckModel);
        var (conflicts, problems) = resolver.Analyze();
        DependencyCheckVisualizer.Show(conflicts, problems);
        
        return new(true, "Dependency check done");
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
        var projects = dependencyCheckModel.Projects ?? [];
        var relevantProjects = excludeProjects is null
            ? projects
            : projects.Where(p => !p.Path.Contains(excludeProjects));

        return new DependencyCheckResolver(relevantProjects);
    }
}
