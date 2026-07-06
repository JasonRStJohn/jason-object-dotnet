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

    public async Task<List<Post>> GetPublishedAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Posts.AsNoTracking()
                .Where(p => p.PublishedAt != null)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
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
}
