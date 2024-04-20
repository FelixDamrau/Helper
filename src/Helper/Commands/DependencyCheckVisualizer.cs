using Develix.Helper.Model.Dependencies;
using Humanizer;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Develix.Helper.Commands;

public static class DependencyCheckVisualizer
{
    public static void Show(IReadOnlyCollection<ProjectConflicts> conflicts, IReadOnlyCollection<Problem> problems)
    {
        IList<IRenderable> renderables = [];
        if (problems.Count > 0)
            renderables.Add(RenderProblems(problems));
        if (conflicts.Count > 0)
            renderables.Add(RenderConflicts(conflicts));
        AnsiConsole.Write(new Rows(renderables));
    }

    private static IRenderable RenderConflicts(IReadOnlyCollection<ProjectConflicts> projectConflicts)
    {
        if (projectConflicts.Count == 0)
            return GetSummary(projectConflicts);

        var table = new Table
        {
            Caption = new("(T) - Transitive package"),
            Border = TableBorder.Rounded,
        };
        table.AddColumn("[bold]Conflict[/]");
        table.AddColumn("[bold]Project[/]");
        table.AddColumn("[bold]Version[/]");
        table.AddColumn("[bold]Info[/]");

        foreach (var conflicts in projectConflicts)
        {
            table.AddRow(conflicts.Project.EscapeMarkup(), string.Empty, string.Empty, string.Empty);
            foreach (var conflict in conflicts.Conflicts)
            {
                var projectFile = new FileInfo(conflict.Project);
                table.AddRow(string.Empty, projectFile.Name.EscapeMarkup(), conflict.Version.EscapeMarkup(), conflict.AdditionalInfo.EscapeMarkup());
            }
        }

        return new Rows(
            table,
            GetSummary(projectConflicts));

        static Text GetSummary(IEnumerable<ProjectConflicts> projectConflicts)
        {
            var projectCount = projectConflicts.Select(p => p.Project).Distinct().Count();
            var conflictCount = projectConflicts.SelectMany(p => p.Conflicts).Distinct().Count();
            return new Text($"""
                {"project".ToQuantity(projectCount)} analyzed.
                {"conflict".ToQuantity(conflictCount)} found.
                """);
        }
    }

    private static Rows RenderProblems(IReadOnlyCollection<Problem> problems)
    {
        var header = new Markup($"Found {"problem".ToQuantity(problems.Count)}", new Style(foreground: Color.Red, decoration: Decoration.Bold));
        var data = problems.Select(p => new Markup($"[bold]{p.Project.Path.EscapeMarkup()}[/]: {p.Description}"));
        return new Rows([header, .. data]);

    }
}
