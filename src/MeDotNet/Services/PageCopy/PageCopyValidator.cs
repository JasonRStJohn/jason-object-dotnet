using System.Text.RegularExpressions;
using MeDotNet.Models;

namespace MeDotNet.Services.PageCopy;

/// <summary>Rules for admin edits to page copy. Each method returns error messages; empty means valid.</summary>
public static class PageCopyValidator
{
    public static readonly IReadOnlyList<string> CategoryKeys = ["ai-systems", "platforms", "claudepress"];

    // A blank line (possibly holding spaces) starts a second paragraph in Markdown.
    private static readonly Regex BlankLine = new(@"\n[ \t]*\r?\n", RegexOptions.Compiled);

    public static IReadOnlyList<string> Validate(Project p)
    {
        var errors = new List<string>();
        if (!CategoryKeys.Contains(p.CategoryKey)) errors.Add("Pick one of the three categories.");
        if (string.IsNullOrWhiteSpace(p.Title)) errors.Add("Title is required.");
        if (string.IsNullOrWhiteSpace(p.Body)) errors.Add("Body is required.");
        if (string.IsNullOrWhiteSpace(p.Badge)) errors.Add("Badge is required.");
        if (string.IsNullOrWhiteSpace(p.Meta)) errors.Add("Meta line is required.");

        var hasText = !string.IsNullOrWhiteSpace(p.LinkText);
        var hasHref = !string.IsNullOrWhiteSpace(p.LinkHref);
        if (hasText != hasHref)
            errors.Add("A link needs both its text and its URL, or neither.");
        else if (hasHref && !IsHttpUrl(p.LinkHref!))
            errors.Add("The link URL must start with http:// or https://.");
        return errors;
    }

    public static IReadOnlyList<string> Validate(ProjectCategory c)
    {
        var errors = new List<string>();
        var required = new (string Value, string Name)[]
        {
            (c.WorkTitle, "Work title"), (c.WorkTag, "Work tag"), (c.WorkProblem, "The problem"),
            (c.WorkApproach, "How I approach it"), (c.WorkLinkText, "Projects link text"),
            (c.InquiryLabel, "Inquiry label"), (c.ProjectsShortLabel, "Filter label"),
            (c.ProjectsTitle, "Projects title"), (c.ProjectsSubtitle, "Projects subtitle"),
        };
        errors.AddRange(required.Where(f => string.IsNullOrWhiteSpace(f.Value)).Select(f => $"{f.Name} is required."));
        if (c.WorkProblem is not null && BlankLine.IsMatch(c.WorkProblem))
            errors.Add("The problem must be one paragraph.");
        return errors;
    }

    public static IReadOnlyList<string> ValidateTexts(IReadOnlyDictionary<string, string> values) =>
        PageTextKeys.All
            .Where(d => d.Format == PageTextFormat.Inline
                        && values.TryGetValue(d.Key, out var v) && v is not null && BlankLine.IsMatch(v))
            .Select(d => $"{d.Label} must be one paragraph.")
            .ToList();

    private static bool IsHttpUrl(string href) =>
        Uri.TryCreate(href.Trim(), UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
