namespace Develix.Helper.Model.Dependencies;

public interface IPackage
{
    string Id { get; set; }
    string ResolvedVersion { get; set; }
}
