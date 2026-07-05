using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MeDotNet.Models;

namespace MeDotNet.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Post>(post =>
        {
            post.HasIndex(p => p.Slug).IsUnique();
            post.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
