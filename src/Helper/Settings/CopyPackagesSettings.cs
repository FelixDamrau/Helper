using System.ComponentModel;
using Spectre.Console.Cli;

namespace Develix.Helper.Settings;

public class CopyPackagesSettings : CommandSettings
{
    [CommandOption("-c|--local-package-cache <path>")]
    [Description("The path to the local temp package cache")]
    public string? LocalPackageCache { get; set; }
}
