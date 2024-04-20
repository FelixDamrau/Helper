using Develix.Helper.Model;
using Spectre.Console;

namespace Develix.Helper.Commands;

public static class CommandResultRenderer
{
    public static int Display(CommandResult result)
    {
        var color = result.Valid ? Color.Grey.ToMarkup() : Color.Red.ToMarkup();
        AnsiConsole.Markup($"[{color}]{result.Message.EscapeMarkup()}[/]");
        return result.Valid ? 0 : -1;
    }
}
