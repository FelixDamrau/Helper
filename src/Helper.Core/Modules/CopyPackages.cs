using Helper.Core.Model;
using System.IO;

namespace Helper.Core.Modules
{
    public class CopyPackages : IModule
    {
        public ModuleResult Run()
        {
            var directory = Directory.GetCurrentDirectory();
            var packageFiles = Directory.GetFiles(directory, "*.nupkg", SearchOption.AllDirectories);
            
            foreach (var file in packageFiles)
            {
                var destinationFile = Path.Combine("C:\\temp\\mypackages\\", Path.GetFileName(file));
                File.Copy(file, destinationFile, true);
            }
            return new ModuleResult(true, "yo");
        }
    }
}
