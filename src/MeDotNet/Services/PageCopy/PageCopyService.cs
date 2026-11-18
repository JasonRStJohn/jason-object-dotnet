using MeDotNet.Data;
using MeDotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace MeDotNet.Services.PageCopy;

/// <summary>Reads and writes the /work and /projects copy.</summary>
public class PageCopyService(IDbContextFactory<AppDbContext> dbFactory)
{
    /// <summary>All three categories in order, each with its visible projects in order, plus every page text.</summary>
    public async Task<PageCopy> GetAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var categories = await db.ProjectCategories.AsNoTracking().OrderBy(c => c.SortOrder).ToListAsync();
        var projects = await db.Projects.AsNoTracking()
            .Where(p => p.IsVisible)
            .OrderBy(p => p.SortOrder)
            .ToListAsync();
        var texts = await db.PageTexts.AsNoTracking().ToDictionaryAsync(t => t.Key, t => t.Value);

        return new PageCopy(
            categories.Select(c => new CategoryCopy(c, projects.Where(p => p.CategoryKey == c.Key).ToList())).ToList(),
            texts);
    }

    // ---- Admin ----------------------------------------------------------------

    /// <summary>Every project, hidden ones included, by category order then project order.</summary>
    public async Task<List<Project>> GetAllProjectsAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var order = await db.ProjectCategories.AsNoTracking().ToDictionaryAsync(c => c.Key, c => c.SortOrder);
        var projects = await db.Projects.AsNoTracking().ToListAsync();
        return projects
            .OrderBy(p => order.GetValueOrDefault(p.CategoryKey, int.MaxValue))
            .ThenBy(p => p.SortOrder)
            .ToList();
    }

    public async Task<Project?> GetProjectAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Creates (Id 0) or updates a project. New projects, and projects moved to another
    /// category, go to the end of their category. Returns errors; empty on success.
    /// </summary>
    public async Task<IReadOnlyList<string>> SaveProjectAsync(Project project)
    {
        var errors = PageCopyValidator.Validate(project);
        if (errors.Count > 0) return errors;

        await using var db = await dbFactory.CreateDbContextAsync();
        Project target;
        if (project.Id == 0)
        {
            target = new Project();
            db.Projects.Add(target);
        }
        else
        {
            var existing = await db.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
            if (existing is null) return ["This project no longer exists."];
            target = existing;
        }

        if (project.Id == 0 || target.CategoryKey != project.CategoryKey)
            target.SortOrder = await NextSortOrderAsync(db, project.CategoryKey);

        target.CategoryKey = project.CategoryKey;
        target.Title = project.Title.Trim();
        target.Badge = project.Badge.Trim();
        target.Meta = project.Meta.Trim();
        target.Body = project.Body.Trim();
        target.Tags = string.Join("\n", project.TagList);
        target.LinkText = NullIfBlank(project.LinkText);
        target.LinkHref = NullIfBlank(project.LinkHref);
        target.IsEmployer = project.IsEmployer;
        target.RecordProject = NullIfBlank(project.RecordProject);
        target.IsVisible = project.IsVisible;

        await db.SaveChangesAsync();
        return [];
    }

    public async Task DeleteProjectAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var project = await db.Projects.FindAsync(id);
        if (project is null) return;
        db.Projects.Remove(project);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Moves a project one place up (-1) or down (+1) within its category, counting hidden
    /// projects the way the admin list shows them. A no-op at either end. Renumbers the
    /// category 1..n so the order stays contiguous after deletes.
    /// </summary>
    public async Task MoveProjectAsync(int id, int direction)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var project = await db.Projects.FindAsync(id);
        if (project is null) return;

        var siblings = await db.Projects.Where(p => p.CategoryKey == project.CategoryKey)
            .OrderBy(p => p.SortOrder).ThenBy(p => p.Id).ToListAsync();
        var index = siblings.FindIndex(p => p.Id == id);
        var swapWith = index + Math.Sign(direction);
        if (swapWith < 0 || swapWith >= siblings.Count) return;

        (siblings[index], siblings[swapWith]) = (siblings[swapWith], siblings[index]);
        for (var i = 0; i < siblings.Count; i++) siblings[i].SortOrder = i + 1;
        await db.SaveChangesAsync();
    }

    public async Task SetProjectVisibleAsync(int id, bool visible)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var project = await db.Projects.FindAsync(id);
        if (project is null) return;
        project.IsVisible = visible;
        await db.SaveChangesAsync();
    }

    public async Task<ProjectCategory?> GetCategoryAsync(string key)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.ProjectCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Key == key);
    }

    /// <summary>Updates an existing category's copy. Never creates one; SortOrder is not editable.</summary>
    public async Task<IReadOnlyList<string>> SaveCategoryAsync(ProjectCategory category)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var existing = await db.ProjectCategories.FirstOrDefaultAsync(c => c.Key == category.Key);
        if (existing is null) return ["That category doesn't exist."];

        var errors = PageCopyValidator.Validate(category);
        if (errors.Count > 0) return errors;

        existing.WorkTitle = category.WorkTitle.Trim();
        existing.WorkTag = category.WorkTag.Trim();
        existing.WorkProblem = category.WorkProblem.Trim();
        existing.WorkApproach = category.WorkApproach.Trim();
        existing.WorkLinkText = category.WorkLinkText.Trim();
        existing.InquiryLabel = category.InquiryLabel.Trim();
        existing.ProjectsShortLabel = category.ProjectsShortLabel.Trim();
        existing.ProjectsTitle = category.ProjectsTitle.Trim();
        existing.ProjectsSubtitle = category.ProjectsSubtitle.Trim();
        await db.SaveChangesAsync();
        return [];
    }

    /// <summary>Saves the known page text blocks (creating any that are missing); unknown keys are ignored.</summary>
    public async Task<IReadOnlyList<string>> SaveTextsAsync(IReadOnlyDictionary<string, string> values)
    {
        var errors = PageCopyValidator.ValidateTexts(values);
        if (errors.Count > 0) return errors;

        await using var db = await dbFactory.CreateDbContextAsync();
        var existing = await db.PageTexts.ToDictionaryAsync(t => t.Key);
        foreach (var definition in PageTextKeys.All)
        {
            if (!values.TryGetValue(definition.Key, out var value)) continue;
            value = (value ?? "").Trim();
            if (existing.TryGetValue(definition.Key, out var row)) row.Value = value;
            else db.PageTexts.Add(new PageText { Key = definition.Key, Value = value });
        }
        await db.SaveChangesAsync();
        return [];
    }

    private static async Task<int> NextSortOrderAsync(AppDbContext db, string categoryKey) =>
        (await db.Projects.Where(p => p.CategoryKey == categoryKey).MaxAsync(p => (int?)p.SortOrder) ?? 0) + 1;

    private static string? NullIfBlank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
