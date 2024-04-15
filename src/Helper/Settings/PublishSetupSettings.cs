using System.ComponentModel;
using Spectre.Console.Cli;

namespace Develix.Helper.Settings;

public class PublishSetupSettings : CommandSettings
{
    [CommandArgument(0, "<setupName>")]
    [Description("The name of the setup")]
    public string SetupName { get; set; } = null!;

    [CommandOption("-d|--setup-directory <path>")]
    [Description("The absolute path to where the setup should be copied")]
    public string? PublishSetupRoot { get; set; }

    [CommandOption("-i|--setup-identifier <identifier>")]
    [Description("The sub path that identifies the setup build directory.")]
    public string? SetupDirectoryIdentifier { get; set; }
}
