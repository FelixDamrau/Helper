using System.ComponentModel;
using Spectre.Console.Cli;

namespace Develix.Helper.Settings;

public class DependencyCheckSettings : CommandSettings
{
    [CommandOption("-d|--working-directory <path>")]
    [Description("The path to the project that should be checked")]
    public string? WorkingDirectory { get; set; }

    [CommandOption("-e|--exclude <projectFilter>")]
    [Description("All projects that contain this string will be excluded from the check")]
    public string? ExcludeProjects { get; set; }

    [CommandOption("-f|--framework <framework>")]
    [Description("Considers only the packages applicable for the specified target framework")]
    public string? Framework { get; set; }
    
}
