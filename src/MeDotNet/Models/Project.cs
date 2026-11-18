using System.ComponentModel.DataAnnotations.Schema;

namespace MeDotNet.Models;

/// <summary>A project card on /projects, edited in the admin.</summary>
public class Project
{
    public int Id { get; set; }
    public string CategoryKey { get; set; } = "";
    public int SortOrder { get; set; }

    public string Title { get; set; } = "";
    public string Badge { get; set; } = "";
    public string Meta { get; set; } = "";
    /// <summary>Block Markdown.</summary>
    public string Body { get; set; } = "";
    /// <summary>Stack tags, one per line.</summary>
    public string Tags { get; set; } = "";

    public string? LinkText { get; set; }
    public string? LinkHref { get; set; }
    public bool IsEmployer { get; set; }
    /// <summary>Record project name; links to /notes?project=... when set.</summary>
    public string? RecordProject { get; set; }
    public bool IsVisible { get; set; } = true;

    [NotMapped]
    public IReadOnlyList<string> TagList =>
        Tags.Split('\n').Select(t => t.Trim()).Where(t => t.Length > 0).ToList();
}
