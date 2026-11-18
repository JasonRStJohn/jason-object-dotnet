using System.Text.Json;
using FluentAssertions;
using MeDotNet.Models;
using MeDotNet.Services.PageCopy;

namespace MeDotNet.Tests.Services.PageCopy;

public class PageCopyServiceReadTests
{
    [Fact]
    public async Task GetAsync_ReturnsCategoriesInSortOrder()
    {
        var copy = await new PageCopyService(PageCopyTestDb.Create()).GetAsync();

        copy.Categories.Select(c => c.Category.Key).Should().Equal("ai-systems", "platforms", "claudepress");
    }

    [Fact]
    public async Task GetAsync_ExcludesHiddenProjects_AndOrdersBySortOrder()
    {
        var db = PageCopyTestDb.Create();
        using (var ctx = db.CreateDbContext())
        {
            var ai = ctx.Projects.Where(p => p.CategoryKey == "ai-systems").OrderBy(p => p.SortOrder).ToList();
            ai[0].IsVisible = false;
            (ai[1].SortOrder, ai[2].SortOrder) = (ai[2].SortOrder, ai[1].SortOrder);
            ctx.SaveChanges();
        }

        var copy = await new PageCopyService(db).GetAsync();
        var titles = copy.Categories[0].Projects.Select(p => p.Title).ToList();

        titles.Should().HaveCount(3);
        titles.Should().NotContain("Enterprise Workforce & ERP AI Harness");
        titles[0].Should().Be("Agentic Job Search");
        titles[1].Should().Be("Site-Creator 2.0 (Agentic Site Factory)");
    }

    [Fact]
    public async Task GetAsync_CategoryWithAllProjectsHidden_HasVisibleProjectsFalse()
    {
        var db = PageCopyTestDb.Create();
        using (var ctx = db.CreateDbContext())
        {
            foreach (var p in ctx.Projects.Where(p => p.CategoryKey == "claudepress")) p.IsVisible = false;
            ctx.SaveChanges();
        }

        var copy = await new PageCopyService(db).GetAsync();
        var claudepress = copy.Categories.Single(c => c.Category.Key == "claudepress");

        claudepress.HasVisibleProjects.Should().BeFalse();
        copy.Categories.Should().HaveCount(3);
    }

    [Fact]
    public async Task Text_MissingKey_ReturnsEmpty()
    {
        var copy = await new PageCopyService(PageCopyTestDb.Create()).GetAsync();

        copy.Text("no.such.key").Should().BeEmpty();
        copy.Text(PageTextKeys.ProjectsEmployerNote).Should().StartWith("Built during an 8-year tenure");
    }

    [Fact]
    public async Task GetAsync_RoundTripsThroughJson()
    {
        var copy = await new PageCopyService(PageCopyTestDb.Create()).GetAsync();

        var back = JsonSerializer.Deserialize<MeDotNet.Services.PageCopy.PageCopy>(JsonSerializer.Serialize(copy))!;

        back.Categories.Select(c => c.Projects.Count).Should().Equal(4, 8, 4);
        back.Text(PageTextKeys.WorkIntro).Should().Be(copy.Text(PageTextKeys.WorkIntro));
        back.Categories[1].Category.InquiryLabel.Should().Be("Custom Platform / .NET / Vue Build");
    }
}
