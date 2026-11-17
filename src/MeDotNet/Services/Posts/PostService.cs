using MeDotNet.Data;
using MeDotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace MeDotNet.Services.Posts;

public class PostService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<Post>> GetAllAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Posts.AsNoTracking().OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public Task<List<Post>> GetPublishedAsync() => GetPublishedAsync(null, null);

    public async Task<List<Post>> GetPublishedAsync(PostKind? kind, string? project)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var query = db.Posts.AsNoTracking().Where(p => p.PublishedAt != null);

        if (kind is not null)
            query = query.Where(p => p.Kind == kind);

        if (!string.IsNullOrWhiteSpace(project))
            query = query.Where(p => p.Project == project);

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Post?> GetBySlugAsync(string slug)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == slug && p.PublishedAt != null);
    }

    /// <summary>
    /// The published entries immediately newer and older than <paramref name="post"/>,
    /// ordered the way the record lists them (CreatedAt). Either side may be null.
    /// </summary>
    public async Task<PostNeighbours> GetNeighboursAsync(Post post)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var published = db.Posts.AsNoTracking().Where(p => p.PublishedAt != null && p.Id != post.Id);

        var newer = await published.Where(p => p.CreatedAt > post.CreatedAt)
            .OrderBy(p => p.CreatedAt)
            .Select(p => new PostLink(p.Title, p.Slug))
            .FirstOrDefaultAsync();
        var older = await published.Where(p => p.CreatedAt < post.CreatedAt)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostLink(p.Title, p.Slug))
            .FirstOrDefaultAsync();

        return new PostNeighbours(newer, older);
    }

    public async Task CreateAsync(Post post)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.Posts.Add(post);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Post post)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.Posts.Update(post);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var post = await db.Posts.FindAsync(id);
        if (post is not null)
        {
            db.Posts.Remove(post);
            await db.SaveChangesAsync();
        }
    }

    public async Task<List<string>> GetProjectsAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Posts.AsNoTracking()
            .Where(p => p.PublishedAt != null && p.Project != null && p.Project.Trim() != "")
            .Select(p => p.Project!)
            .Distinct()
            .OrderBy(p => p)
            .ToListAsync();
    }
}
