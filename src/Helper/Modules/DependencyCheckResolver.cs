using Develix.Helper.Model.Dependencies;
using Spectre.Console;

namespace Develix.Helper.Modules;

public partial class DependencyCheckResolver
{
    private readonly HashSet<PackageData> packageData = [];

    public void Add(Project project, IPackage package)
    {
        var additional = package switch
        {
            TransitivePackage => "(T)",
            TopLevelPackage tlp when tlp.RequestedVersion != package.ResolvedVersion => $"Reqested: {tlp.RequestedVersion}",
            TopLevelPackage => string.Empty,
            _ => throw new NotSupportedException($"Packe type {package.GetType().Name} is not supported!"),
        };
        packageData.Add(new(project.Path, package.Id, package.ResolvedVersion, additional));
    }

    public int ShowConflicts()
    {
        var conflictedPackages = packageData
            .ToLookup(p => p.Id)
            .Where(x => x.Count() > 1);

        if (!conflictedPackages.Any())
            return 0;
        var table = VisualizeData(conflictedPackages);
        AnsiConsole.Write(table);
        return conflictedPackages.Count();
    }

    private static Table VisualizeData(IEnumerable<IGrouping<string, PackageData>> conflictedPackages)
    {
        var table = new Table
        {
            Caption = new("(T) - Transitive package"),
        };
        table.AddColumn("Conflict");
        table.AddColumn("Project");
        table.AddColumn("Version");
        table.AddColumn("Info");

        foreach (var conflict in conflictedPackages)
        {
            table.AddRow(conflict.Key.EscapeMarkup(), string.Empty, string.Empty, string.Empty);
            foreach (var data in conflict)
            {
                var projectFile = new FileInfo(data.Project);
                table.AddRow(string.Empty, projectFile.Name.EscapeMarkup(), data.Version.EscapeMarkup(), data.AdditionalInfo.EscapeMarkup());
            }
        }

        return table;
    }

    private sealed class PackageData(string project, string id, string version, string additionalInfo) : IEquatable<PackageData> 
    {
        public string Project { get; } = project;
        public string Id { get; } = id;
        public string Version { get; } = version;
        public string AdditionalInfo { get; } = additionalInfo;

        public bool Equals(PackageData? other)
        {
            return other is not null
                && Id == other.Id 
                && Version == other.Version;
        }

        public override bool Equals(object? obj) => obj is PackageData packageData && Equals(packageData);

        public override int GetHashCode() => HashCode.Combine(Id, Version);
    }
}
