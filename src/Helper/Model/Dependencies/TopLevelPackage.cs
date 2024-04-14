namespace Develix.Helper.Model.Dependencies;

public class TopLevelPackage : IPackage
{
    public string Id { get; set; } = null!;
    public string RequestedVersion { get; set; } = null!;
    public string ResolvedVersion { get; set; } = null!;
}
