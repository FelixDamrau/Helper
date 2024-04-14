namespace Develix.Helper.Model.Dependencies;

public class Project
{
    public string Path { get; set; } = null!;
    public List<FrameworkReference> Frameworks { get; set; } = null!;
}
