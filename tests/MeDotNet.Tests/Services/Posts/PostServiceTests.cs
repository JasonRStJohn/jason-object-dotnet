using FluentAssertions;
using MeDotNet.Data;
using MeDotNet.Models;
using MeDotNet.Services.Posts;
using Microsoft.EntityFrameworkCore;

namespace MeDotNet.Tests.Services.Posts;

public class PostServiceTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static Post MakePost(string title = "Test", bool published = false) => new()
    {
        Title = title,
        Slug = title.ToLowerInvariant().Replace(' ', '-'),
        Body = "Body content.",
        AuthorId = "user-1",
        PublishedAt = published ? DateTime.UtcNow : null
    };

    [Fact]
    public async Task GetAllAsync_ReturnsAllPostsNewestFirst()
    {
        using var db = CreateDb();
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
        using var db = CreateDb();
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
        using var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakePost("draft-post", published: false));

        var result = await svc.GetBySlugAsync("draft-post");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsPublishedPost()
    {
        using var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakePost("live-post", published: true));

        var result = await svc.GetBySlugAsync("live-post");

        result.Should().NotBeNull();
        result!.Title.Should().Be("live-post");
    }

    [Fact]
    public async Task CreateAsync_PersistsPost()
    {
        using var db = CreateDb();
        var svc = new PostService(db);

        await svc.CreateAsync(MakePost("Hello"));

        var all = await svc.GetAllAsync();
        all.Should().HaveCount(1);
        all[0].Title.Should().Be("Hello");
    }

    [Fact]
    public async Task UpdateAsync_MutatesPost()
    {
        using var db = CreateDb();
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
        using var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakePost("To Delete"));
        var post = (await svc.GetAllAsync())[0];

        await svc.DeleteAsync(post.Id);

        var all = await svc.GetAllAsync();
        all.Should().BeEmpty();
    }
}
