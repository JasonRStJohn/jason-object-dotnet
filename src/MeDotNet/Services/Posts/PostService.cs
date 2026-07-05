using MeDotNet.Data;
using MeDotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace MeDotNet.Services.Posts;

public class PostService(AppDbContext db)
{
    public Task<List<Post>> GetAllAsync() =>
        db.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();

    public Task<List<Post>> GetPublishedAsync() =>
        db.Posts.Where(p => p.PublishedAt != null)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

    public Task<Post?> GetByIdAsync(int id) =>
        db.Posts.FirstOrDefaultAsync(p => p.Id == id);

    public Task<Post?> GetBySlugAsync(string slug) =>
        db.Posts.FirstOrDefaultAsync(p => p.Slug == slug && p.PublishedAt != null);

    public async Task CreateAsync(Post post)
    {
        db.Posts.Add(post);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Post post)
    {
        db.Posts.Update(post);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is not null)
        {
            db.Posts.Remove(post);
            await db.SaveChangesAsync();
        }
    }
}
