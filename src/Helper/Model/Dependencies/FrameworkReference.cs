namespace Develix.Helper.Model.Dependencies;

public class FrameworkReference
{
    public string Framework { get; set; } = null!;
    public List<TopLevelPackage>? TopLevelPackages { get; set; }
    public List<TransitivePackage>? TransitivePackages { get; set; }
}
