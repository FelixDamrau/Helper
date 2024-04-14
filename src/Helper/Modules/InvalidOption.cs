using Develix.Helper.Model;

namespace Develix.Helper.Modules;

public class InvalidOption : IModule
{
    public ModuleResult Run() => new(false, "Could not find option to execute. :'(");
}
