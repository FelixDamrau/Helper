using Helper.Core.Model;

namespace Helper.Core.Modules;
public class InvalidOption : IModule
{
    public ModuleResult Run() => new(false, "Could not find option to execute. :'(");
}
