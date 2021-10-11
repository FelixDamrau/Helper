using Helper.Core.Model;
using Humanizer;
using System;
using System.IO;

namespace Helper.Core.Modules
{
    public class CopyPackages : IModule
    {
        private readonly string localPackageCache;

        public CopyPackages(AppSettings appSettings)
        {
            localPackageCache = appSettings.LocalPackageCache;
        }

        public ModuleResult Run()
        {
            try
            {
                if (!Directory.Exists(localPackageCache))
                    return new ModuleResult(false, $"The directory of the local package cache '{localPackageCache}' does not exist!");

                var directory = Directory.GetCurrentDirectory();
                var packageFiles = Directory.GetFiles(directory, "*.nupkg", SearchOption.AllDirectories);
                var count = packageFiles.Length;

                Console.WriteLine($"Found {"package".ToQuantity(count)}.");

                foreach (var filePath in packageFiles)
                {
                    var fileName = Path.GetFileName(filePath);
                    Console.WriteLine($"Copy nuget file: {fileName}");
                    var destinationFile = Path.Combine(localPackageCache, fileName);
                    File.Copy(filePath, destinationFile, true);
                }
                return new ModuleResult(true, "package".ToQuantity(count) + " copied successfully.");
            }
            catch (Exception exception)
            {
                return new ModuleResult(false, $"Copying packages failed. [{exception.Message}]");
            }
        }
    }
}
