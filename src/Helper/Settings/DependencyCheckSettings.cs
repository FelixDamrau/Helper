using System.ComponentModel;
using Spectre.Console.Cli;

namespace Develix.Helper.Settings;

public class DependencyCheckSettings : CommandSettings
{
    [CommandOption("-d|--working-directory <path>")]
    [Description("The path to the project that should be checked")]
    public string? WorkingDirectory { get; set; }
}
