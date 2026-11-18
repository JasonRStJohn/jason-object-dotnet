using FluentAssertions;
using MeDotNet.Data.Seed;
using MeDotNet.Models;

namespace MeDotNet.Tests.Data;

public class PageCopySeedTests
{
    [Fact]
    public void Seed_HasThreeCategoriesInOrder()
    {
        PageCopySeed.Categories.Select(c => c.Key).Should().Equal("ai-systems", "platforms", "claudepress");
        PageCopySeed.Categories.Select(c => c.SortOrder).Should().Equal(1, 2, 3);
    }

    [Fact]
    public void Seed_ProjectCountsPerCategory()
    {
        var byCategory = PageCopySeed.Projects.GroupBy(p => p.CategoryKey).ToDictionary(g => g.Key, g => g.ToList());
        byCategory["ai-systems"].Should().HaveCount(4);
        byCategory["platforms"].Should().HaveCount(8);
        byCategory["claudepress"].Should().HaveCount(4);
        PageCopySeed.Projects.Select(p => p.Id).Should().OnlyHaveUniqueItems();
        foreach (var group in byCategory.Values)
            group.Select(p => p.SortOrder).Should().Equal(Enumerable.Range(1, group.Count));
    }

    [Fact]
    public void Seed_HasEveryPageTextKey() =>
        PageCopySeed.Texts.Select(t => t.Key).Should().BeEquivalentTo(PageTextKeys.All.Select(d => d.Key));

    [Fact]
    public void Seed_ContainsNoEmDash()
    {
        var strings = PageCopySeed.Categories.SelectMany(StringsOf)
            .Concat(PageCopySeed.Projects.SelectMany(StringsOf))
            .Concat(PageCopySeed.Texts.SelectMany(StringsOf));
        strings.Should().NotContain(s => s.Contains('—'));
    }

    [Fact]
    public void Seed_SpotChecks()
    {
        PageCopySeed.Categories.Single(c => c.Key == "ai-systems").WorkTag.Should().Be("Architecture / Sprint");
        PageCopySeed.Categories.Single(c => c.Key == "platforms").InquiryLabel.Should().Be("Custom Platform / .NET / Vue Build");

        var jobSearch = PageCopySeed.Projects.Single(p => p.Title == "Agentic Job Search");
        jobSearch.LinkHref.Should().Be("https://github.com/JasonRStJohn/agentic-job-search");
        jobSearch.IsEmployer.Should().BeFalse();

        PageCopySeed.Projects.Single(p => p.Title == "jasonobject.work (this site)").RecordProject.Should().Be("JasonObject");
        PageCopySeed.Projects.Single(p => p.Title == "Job Management Platform 2.0").IsEmployer.Should().BeTrue();
    }

    [Fact]
    public void Seed_ProblemFieldsHaveNoLabelAndNoBlankLines()
    {
        foreach (var c in PageCopySeed.Categories)
        {
            c.WorkProblem.Should().NotStartWith("The problem");
            c.WorkProblem.Should().NotContain("\n\n");
        }
    }

    [Fact]
    public void Seed_LinkPairsAreBothOrNeither() =>
        PageCopySeed.Projects.Should().OnlyContain(p => (p.LinkText == null) == (p.LinkHref == null));

    [Fact]
    public void TagList_TrimsAndDropsEmpty()
    {
        var project = new Project { CategoryKey = "platforms", Title = "t", Badge = "b", Meta = "m", Body = "x", Tags = " a \n\n b\n" };
        project.TagList.Should().Equal("a", "b");
    }

    private static IEnumerable<string> StringsOf(object o) =>
        o.GetType().GetProperties().Where(p => p.PropertyType == typeof(string))
            .Select(p => (string?)p.GetValue(o)).Where(s => s is not null)!;
}
