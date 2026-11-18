namespace MeDotNet.Models;

/// <summary>
/// One of the three fixed problem categories. Holds that category's copy for both
/// /work (the track panel) and /projects (the group), so the two cannot drift.
/// Rows are seeded, never created or deleted from the admin; the accent colour is
/// mapped from <see cref="Key"/> in code (CategoryAccent), not stored.
/// </summary>
public class ProjectCategory
{
    public string Key { get; set; } = "";
    public int SortOrder { get; set; }

    public string WorkTitle { get; set; } = "";
    public string WorkTag { get; set; } = "";
    /// <summary>Inline Markdown, one paragraph; rendered after a bold "The problem:" label.</summary>
    public string WorkProblem { get; set; } = "";
    /// <summary>Block Markdown, shown in the "How I approach it" disclosure.</summary>
    public string WorkApproach { get; set; } = "";
    public string WorkLinkText { get; set; } = "";
    public string InquiryLabel { get; set; } = "";

    public string ProjectsShortLabel { get; set; } = "";
    public string ProjectsTitle { get; set; } = "";
    public string ProjectsSubtitle { get; set; } = "";
}
