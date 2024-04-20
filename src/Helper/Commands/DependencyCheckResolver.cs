using System.Diagnostics;
using System.Reflection;
using Develix.Essentials.Core;
using Develix.Helper.Model.Dependencies;
using Spectre.Console;

namespace Develix.Helper.Commands;

public partial class DependencyCheckResolver
{
    private readonly HashSet<PackageData> packageData = [];
    private readonly IList<Problem> problems = [];

    public DependencyCheckResolver(IEnumerable<Project> projects)
    {
        Init(projects);
    }

    private void Init(IEnumerable<Project> projects)
    {
        foreach (var project in projects)
        {
            var frameworkResult = project.Frameworks switch
            {
                null or { Count: < 1 } => Result.Fail<FrameworkReference>($"The project '{project.Path}' has no frameworks."),
                { Count: 1 } => Result.Ok(project.Frameworks[0]),
                { Count: > 1 }
                    => Result.Fail<FrameworkReference>(
                    $"The project '{project.Path}' has multiple frameworks ({GetFrameworksDisplayString(project)}). " +
                    $"Please specify a specific framework with the '{GetFrameworkOptionDisplayString()}' option"),
            };

            if (frameworkResult.Valid)
                Add(project, frameworkResult.Value);
            else
                Add(project, frameworkResult.Message);
        }
    }

    public (IReadOnlyCollection<ProjectConflicts> Conflicts, IReadOnlyCollection<Problem> Problems) Analyze()
    {
        var conflicts = packageData
            .ToLookup(p => p.Id)
            .Where(x => x.Count() > 1)
            .Select(x => new ProjectConflicts(x.Key, [.. x]));
        return ([.. conflicts], [.. problems]);
    }

    private static string GetFrameworksDisplayString(Project project) => string.Join(", ", project.Frameworks.Select(f => f.Framework.EscapeMarkup()));

    private static string GetFrameworkOptionDisplayString()
    {
        var frameworkOptionAttribute = typeof(Settings.DependencyCheckSettings)
            .GetProperty(nameof(Settings.DependencyCheckSettings.Framework))
            !.GetCustomAttribute<Spectre.Console.Cli.CommandOptionAttribute>()
                ?? throw new UnreachableException($"Could not find the command option definition for framework");
        var longCommandName = frameworkOptionAttribute.LongNames[0];
        var valueName = frameworkOptionAttribute.ValueName;
        return $"--{longCommandName} <{valueName}>".EscapeMarkup();
    }

    private void Add(Project project, FrameworkReference framework)
    {
        foreach (var package in framework.TopLevelPackages ?? [])
            Add(project, package);

        foreach (var package in framework.TransitivePackages ?? [])
            Add(project, package);
    }

    private void Add(Project project, IPackage package)
    {
        var additional = package switch
        {
            TransitivePackage => "(T)",
            TopLevelPackage tlp when tlp.RequestedVersion != package.ResolvedVersion => $"Requested: {tlp.RequestedVersion}",
            TopLevelPackage => string.Empty,
            _ => throw new NotSupportedException($"Package type {package.GetType().Name} is not supported!"),
        };
        packageData.Add(new(project.Path, package.Id, package.ResolvedVersion, additional));
    }

    private void Add(Project project, string problemDescription) => problems.Add(new(project, problemDescription));
}
