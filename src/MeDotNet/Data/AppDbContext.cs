using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MeDotNet.Models;

namespace MeDotNet.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Post> Posts { get; set; }
    public DbSet<ProjectCategory> ProjectCategories { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<PageText> PageTexts { get; set; }

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

        builder.Entity<ProjectCategory>(category =>
        {
            category.HasKey(c => c.Key);
            category.Property(c => c.Key).HasMaxLength(64);
        });

        builder.Entity<Project>(project =>
        {
            project.Property(p => p.CategoryKey).HasMaxLength(64);
            project.HasIndex(p => new { p.CategoryKey, p.SortOrder });
            // Restrict: a category is never deleted, and if one ever were, its
            // projects must not vanish with it.
            project.HasOne<ProjectCategory>()
                .WithMany()
                .HasForeignKey(p => p.CategoryKey)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PageText>(text =>
        {
            text.HasKey(t => t.Key);
            text.Property(t => t.Key).HasMaxLength(64);
        });
    }
}
