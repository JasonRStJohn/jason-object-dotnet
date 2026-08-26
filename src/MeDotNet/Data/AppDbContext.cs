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

            // Restrict, not the convention default (Cascade). Deleting a user who
            // still has posts must FAIL loudly rather than silently deleting their
            // content — a reseed/rotation that deletes the admin once wiped the
            // record this way. Rotate passwords in-app instead (ChangePassword).
            post.HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
