using MeDotNet.Data;
using MeDotNet.Data.Seed;
using MeDotNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MeDotNet.Tests.Services.PageCopy;

/// <summary>An in-memory database loaded with fresh copies of the seed rows.</summary>
internal static class PageCopyTestDb
{
    // One root for every test: the root is part of the options identity, so a new one per
    // test would defeat EF's provider cache. The unique database name keeps tests isolated.
    private static readonly InMemoryDatabaseRoot Root = new();

    public static IDbContextFactory<AppDbContext> Create(bool seed = true)
    {
        // Plain options rather than a fresh DI container per test: EF caches its internal
        // service provider by options, and a new container per test trips its
        // "more than twenty service providers" guard across the whole run.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString(), Root)
            .Options;
        var factory = new Factory(options);

        if (seed)
        {
            using var db = factory.CreateDbContext();
            db.ProjectCategories.AddRange(PageCopySeed.Categories.Select(Clone));
            db.Projects.AddRange(PageCopySeed.Projects.Select(Clone));
            db.PageTexts.AddRange(PageCopySeed.Texts.Select(t => new PageText { Key = t.Key, Value = t.Value }));
            db.SaveChanges();
        }
        return factory;
    }

    private sealed class Factory(DbContextOptions<AppDbContext> options) : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);
    }

    // The seed lists are static and shared; tests must never mutate them.
    private static T Clone<T>(T source) where T : new()
    {
        var copy = new T();
        foreach (var p in typeof(T).GetProperties().Where(p => p.CanWrite))
            p.SetValue(copy, p.GetValue(source));
        return copy;
    }
}
