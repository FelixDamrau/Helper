using Develix.Helper.Model;
using Spectre.Console;

namespace Develix.Helper.Commands;

public static class ModuleResultRenderer
{
    public static int Display(ModuleResult result)
    {
        var color = result.Valid ? Color.Grey.ToMarkup() : Color.Red.ToMarkup();
        AnsiConsole.Markup($"[{color}]{result.Message.EscapeMarkup()}[/]");
        return result.Valid ? 0 : -1;
    }
}
