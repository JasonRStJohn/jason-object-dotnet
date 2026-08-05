using FluentAssertions;
using MeDotNet.Data;
using MeDotNet.Models;
using MeDotNet.Services.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace MeDotNet.Tests.Services.Posts;

public class PostServiceTests
{
    private static IDbContextFactory<AppDbContext> CreateDb()
    {
        var root = new InMemoryDatabaseRoot();
        var databaseName = Guid.NewGuid().ToString();
        var services = new ServiceCollection();
        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseInMemoryDatabase(databaseName, root));
        return services.BuildServiceProvider().GetRequiredService<IDbContextFactory<AppDbContext>>();
    }

    private static Post MakePost(string title = "Test", bool published = false) => new()
    {
        Title = title,
        Slug = title.ToLowerInvariant().Replace(' ', '-'),
        Body = "Body content.",
        AuthorId = "user-1",
        PublishedAt = published ? DateTime.UtcNow : null
    };

    private static Post MakeEntry(string title, PostKind kind, string? project, bool published = true)
    {
        var post = MakePost(title, published);
        post.Kind = kind;
        post.Project = project;
        return post;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPostsNewestFirst()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        var older = MakePost("Older");
        older.CreatedAt = DateTime.UtcNow.AddDays(-1);
        var newer = MakePost("Newer");
        newer.CreatedAt = DateTime.UtcNow;
        await svc.CreateAsync(older);
        await svc.CreateAsync(newer);

        var result = await svc.GetAllAsync();

        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Newer");
        result[1].Title.Should().Be("Older");
    }

    [Fact]
    public async Task GetPublishedAsync_ExcludesDrafts()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakePost("Draft", published: false));
        await svc.CreateAsync(MakePost("Published", published: true));

        var result = await svc.GetPublishedAsync();

        result.Should().HaveCount(1);
        result[0].Title.Should().Be("Published");
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsNullForUnpublishedPost()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakePost("draft-post", published: false));

        var result = await svc.GetBySlugAsync("draft-post");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsPublishedPost()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakePost("live-post", published: true));

        var result = await svc.GetBySlugAsync("live-post");

        result.Should().NotBeNull();
        result!.Title.Should().Be("live-post");
    }

    [Fact]
    public async Task CreateAsync_PersistsPost()
    {
        var db = CreateDb();
        var svc = new PostService(db);

        await svc.CreateAsync(MakePost("Hello"));

        var all = await svc.GetAllAsync();
        all.Should().HaveCount(1);
        all[0].Title.Should().Be("Hello");
    }

    [Fact]
    public async Task UpdateAsync_MutatesPost()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakePost("Original"));
        var post = (await svc.GetAllAsync())[0];

        post.Title = "Updated";
        await svc.UpdateAsync(post);

        var result = await svc.GetByIdAsync(post.Id);
        result!.Title.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_RemovesPost()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakePost("To Delete"));
        var post = (await svc.GetAllAsync())[0];

        await svc.DeleteAsync(post.Id);

        var all = await svc.GetAllAsync();
        all.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_DefaultsKindToNote()
    {
        var db = CreateDb();
        var svc = new PostService(db);

        await svc.CreateAsync(MakePost("No Kind Given"));

        var all = await svc.GetAllAsync();
        all[0].Kind.Should().Be(PostKind.Note);
        all[0].Project.Should().BeNull();
        all[0].Summary.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_PersistsKindProjectAndSummary()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        var post = MakePost("Phase 4 Spec", published: true);
        post.Kind = PostKind.Spec;
        post.Project = "JasonObject";
        post.Summary = "The spec that decided what this site says.";

        await svc.CreateAsync(post);

        var all = await svc.GetAllAsync();
        all[0].Kind.Should().Be(PostKind.Spec);
        all[0].Project.Should().Be("JasonObject");
        all[0].Summary.Should().Be("The spec that decided what this site says.");
    }

    [Fact]
    public async Task GetPublishedAsync_FiltersByKind()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("A Spec", PostKind.Spec, "JasonObject"));
        await svc.CreateAsync(MakeEntry("A Note", PostKind.Note, null));

        var result = await svc.GetPublishedAsync(PostKind.Spec, null);

        result.Should().ContainSingle();
        result[0].Title.Should().Be("A Spec");
    }

    [Fact]
    public async Task GetPublishedAsync_FiltersByProject()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("Site Spec", PostKind.Spec, "JasonObject"));
        await svc.CreateAsync(MakeEntry("Press Spec", PostKind.Spec, "ClaudePress"));

        var result = await svc.GetPublishedAsync(null, "ClaudePress");

        result.Should().ContainSingle();
        result[0].Title.Should().Be("Press Spec");
    }

    [Fact]
    public async Task GetPublishedAsync_FiltersByKindAndProjectTogether()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("Right One", PostKind.PostMortem, "JasonObject"));
        await svc.CreateAsync(MakeEntry("Wrong Kind", PostKind.Spec, "JasonObject"));
        await svc.CreateAsync(MakeEntry("Wrong Project", PostKind.PostMortem, "ClaudePress"));

        var result = await svc.GetPublishedAsync(PostKind.PostMortem, "JasonObject");

        result.Should().ContainSingle();
        result[0].Title.Should().Be("Right One");
    }

    [Fact]
    public async Task GetPublishedAsync_WithNoFilters_ReturnsAllPublished()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("Published Spec", PostKind.Spec, "JasonObject"));
        await svc.CreateAsync(MakeEntry("Draft Note", PostKind.Note, null, published: false));

        var result = await svc.GetPublishedAsync(null, null);

        result.Should().ContainSingle();
        result[0].Title.Should().Be("Published Spec");
    }

    [Fact]
    public async Task GetPublishedAsync_ExcludesDraftsWhenFiltering()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("Draft Spec", PostKind.Spec, "JasonObject", published: false));

        var result = await svc.GetPublishedAsync(PostKind.Spec, null);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProjectsAsync_ReturnsDistinctSortedNames()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("One", PostKind.Spec, "Whindancer"));
        await svc.CreateAsync(MakeEntry("Two", PostKind.Plan, "ClaudePress"));
        await svc.CreateAsync(MakeEntry("Three", PostKind.Note, "ClaudePress"));
        await svc.CreateAsync(MakeEntry("Four", PostKind.Note, null));

        var result = await svc.GetProjectsAsync();

        result.Should().Equal("ClaudePress", "Whindancer");
    }

    [Fact]
    public async Task GetProjectsAsync_ExcludesDrafts()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("Hidden", PostKind.Spec, "SecretProject", published: false));

        var result = await svc.GetProjectsAsync();

        result.Should().BeEmpty();
    }
}
