namespace Develix.Helper.Model.Dependencies;

public record ProjectConflicts(string Project, IReadOnlyCollection<PackageData> Conflicts);
