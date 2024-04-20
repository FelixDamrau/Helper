namespace Develix.Helper.Model.Dependencies;

public class PackageData(string project, string id, string version, string additionalInfo) : IEquatable<PackageData>
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
