namespace Develix.Helper.Model.Dependencies;

public class TransitivePackage : IPackage
{
    public string Id { get; set; } = null!;
    public string ResolvedVersion { get; set; } = null!;
}
