using FluentAssertions;
using MeDotNet.Models;
using MeDotNet.Services.PageCopy;

namespace MeDotNet.Tests.Services.PageCopy;

public class PageCopyServiceWriteTests
{
    private static Project NewProject(string category = "ai-systems") => new()
    {
        CategoryKey = category, Title = "New thing", Badge = "Contract", Meta = "2026", Body = "Body.", Tags = "C#",
    };

    [Fact]
    public async Task SaveProjectAsync_New_AppendsToEndOfCategory()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());

        (await svc.SaveProjectAsync(NewProject())).Should().BeEmpty();

        var ai = (await svc.GetAsync()).Categories[0].Projects;
        ai.Should().HaveCount(5);
        ai[^1].Title.Should().Be("New thing");
        ai[^1].SortOrder.Should().Be(5);
    }

    [Fact]
    public async Task SaveProjectAsync_Existing_UpdatesFields()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());
        var p = (await svc.GetProjectAsync(1))!;
        p.Title = "Renamed";

        (await svc.SaveProjectAsync(p)).Should().BeEmpty();

        (await svc.GetProjectAsync(1))!.Title.Should().Be("Renamed");
    }

    [Fact]
    public async Task SaveProjectAsync_MissingId_ReturnsError()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());
        var p = NewProject();
        p.Id = 999;

        (await svc.SaveProjectAsync(p)).Should().Equal("This project no longer exists.");
    }

    [Fact]
    public async Task SaveProjectAsync_Invalid_DoesNotPersist()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());
        var p = (await svc.GetProjectAsync(1))!;
        p.Title = " ";

        (await svc.SaveProjectAsync(p)).Should().NotBeEmpty();

        (await svc.GetProjectAsync(1))!.Title.Should().NotBe(" ");
    }

    [Fact]
    public async Task SaveProjectAsync_MovedToAnotherCategory_GoesToItsEnd()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());
        var p = (await svc.GetProjectAsync(1))!;
        p.CategoryKey = "claudepress";

        (await svc.SaveProjectAsync(p)).Should().BeEmpty();

        var copy = await svc.GetAsync();
        copy.Categories[2].Projects[^1].Id.Should().Be(1);
        copy.Categories[2].Projects[^1].SortOrder.Should().Be(5);
    }

    [Fact]
    public async Task MoveProjectAsync_SwapsWithNeighbour()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());

        await svc.MoveProjectAsync(2, +1);

        (await svc.GetAsync()).Categories[0].Projects.Select(p => p.Id).Should().Equal(1, 3, 2, 4);
    }

    [Fact]
    public async Task MoveProjectAsync_AtEdges_IsNoOp()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());

        await svc.MoveProjectAsync(1, -1);
        await svc.MoveProjectAsync(4, +1);

        var ai = (await svc.GetAsync()).Categories[0].Projects;
        ai.Select(p => p.Id).Should().Equal(1, 2, 3, 4);
        ai.Select(p => p.SortOrder).Should().Equal(1, 2, 3, 4);
    }

    [Fact]
    public async Task MoveProjectAsync_CountsHiddenNeighbours_LikeTheAdminList()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());
        await svc.SetProjectVisibleAsync(2, false);

        await svc.MoveProjectAsync(1, +1);

        (await svc.GetAllProjectsAsync()).Where(p => p.CategoryKey == "ai-systems")
            .Select(p => p.Id).Should().Equal(2, 1, 3, 4);
    }

    [Fact]
    public async Task SetProjectVisibleAsync_HidesFromGetAsync()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());

        await svc.SetProjectVisibleAsync(1, false);

        (await svc.GetAsync()).Categories[0].Projects.Select(p => p.Id).Should().NotContain(1);
        (await svc.GetAllProjectsAsync()).Should().Contain(p => p.Id == 1 && !p.IsVisible);
    }

    [Fact]
    public async Task DeleteProjectAsync_Removes()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());

        await svc.DeleteProjectAsync(1);

        (await svc.GetProjectAsync(1)).Should().BeNull();
    }

    [Fact]
    public async Task GetAllProjectsAsync_OrdersByCategoryThenProject()
    {
        var all = await new PageCopyService(PageCopyTestDb.Create()).GetAllProjectsAsync();

        all.Select(p => p.CategoryKey).Distinct().Should().Equal("ai-systems", "platforms", "claudepress");
        all.Should().HaveCount(16);
    }

    [Fact]
    public async Task SaveCategoryAsync_UpdatesFields()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());
        var c = (await svc.GetCategoryAsync("platforms"))!;
        c.WorkTag = "Project / Lead";

        (await svc.SaveCategoryAsync(c)).Should().BeEmpty();

        (await svc.GetCategoryAsync("platforms"))!.WorkTag.Should().Be("Project / Lead");
    }

    [Fact]
    public async Task SaveCategoryAsync_UnknownKey_ReturnsError()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());
        var c = (await svc.GetCategoryAsync("platforms"))!;
        c.Key = "new-category";

        (await svc.SaveCategoryAsync(c)).Should().NotBeEmpty();
        (await svc.GetCategoryAsync("new-category")).Should().BeNull();
    }

    [Fact]
    public async Task SaveTextsAsync_UpdatesKnownAndIgnoresUnknownKeys()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create());

        var errors = await svc.SaveTextsAsync(new Dictionary<string, string>
        {
            [PageTextKeys.WorkIntro] = "New intro.",
            ["made.up"] = "x",
        });

        errors.Should().BeEmpty();
        var copy = await svc.GetAsync();
        copy.Text(PageTextKeys.WorkIntro).Should().Be("New intro.");
        copy.Texts.Should().NotContainKey("made.up");
    }

    [Fact]
    public async Task SaveTextsAsync_RecreatesMissingKnownKey()
    {
        var svc = new PageCopyService(PageCopyTestDb.Create(seed: false));

        await svc.SaveTextsAsync(new Dictionary<string, string> { [PageTextKeys.ProjectsIntro] = "Hello." });

        (await svc.GetAsync()).Text(PageTextKeys.ProjectsIntro).Should().Be("Hello.");
    }
}
