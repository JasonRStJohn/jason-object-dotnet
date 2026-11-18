using MeDotNet.Data;
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
}
