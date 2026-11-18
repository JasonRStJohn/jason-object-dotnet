using MeDotNet.Models;

namespace MeDotNet.Services.PageCopy;

/// <summary>A category with its visible projects, in display order.</summary>
public record CategoryCopy(ProjectCategory Category, List<Project> Projects)
{
    public bool HasVisibleProjects => Projects.Count > 0;
}

/// <summary>
/// Everything /work and /projects render from the database. Plain records so
/// PersistentLoader can carry it from prerender to the interactive pass as JSON.
/// </summary>
public record PageCopy(List<CategoryCopy> Categories, Dictionary<string, string> Texts)
{
    /// <summary>The block's text, or "" when the key is missing (the block then renders nothing).</summary>
    public string Text(string key) => Texts.TryGetValue(key, out var value) ? value : "";
}
