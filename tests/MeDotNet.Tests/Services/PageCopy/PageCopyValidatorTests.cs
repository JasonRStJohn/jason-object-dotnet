using FluentAssertions;
using MeDotNet.Data.Seed;
using MeDotNet.Models;
using MeDotNet.Services.PageCopy;

namespace MeDotNet.Tests.Services.PageCopy;

public class PageCopyValidatorTests
{
    private static Project Valid() => new()
    {
        CategoryKey = "platforms", Title = "A project", Badge = "Contract", Meta = "2026", Body = "Body text.", Tags = "C#",
    };

    [Fact]
    public void Validate_ValidProject_HasNoErrors() => PageCopyValidator.Validate(Valid()).Should().BeEmpty();

    [Theory]
    [InlineData("title")]
    [InlineData("body")]
    public void Validate_WhitespaceOnlyRequired_Fails(string field)
    {
        var p = Valid();
        if (field == "title") p.Title = "   "; else p.Body = " \n ";
        PageCopyValidator.Validate(p).Should().NotBeEmpty();
    }

    [Fact]
    public void Validate_LinkTextWithoutHref_Fails()
    {
        var p = Valid();
        p.LinkText = "Source";
        PageCopyValidator.Validate(p).Should().ContainMatch("*link*");
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("ftp://example.com")]
    [InlineData("example.com")]
    public void Validate_NonHttpHref_Fails(string href)
    {
        var p = Valid();
        p.LinkText = "Source";
        p.LinkHref = href;
        PageCopyValidator.Validate(p).Should().NotBeEmpty();
    }

    [Fact]
    public void Validate_UnknownCategory_Fails()
    {
        var p = Valid();
        p.CategoryKey = "nope";
        PageCopyValidator.Validate(p).Should().NotBeEmpty();
    }

    [Fact]
    public void ValidateCategory_SeedCategory_HasNoErrors() =>
        PageCopyValidator.Validate(PageCopySeed.Categories[0]).Should().BeEmpty();

    [Fact]
    public void ValidateCategory_BlankLineInProblem_Fails()
    {
        var c = PageCopySeed.Categories[0];
        var edited = new ProjectCategory
        {
            Key = c.Key, SortOrder = c.SortOrder, WorkTitle = c.WorkTitle, WorkTag = c.WorkTag,
            WorkProblem = "One.\n\nTwo.", WorkApproach = c.WorkApproach, WorkLinkText = c.WorkLinkText,
            InquiryLabel = c.InquiryLabel, ProjectsShortLabel = c.ProjectsShortLabel,
            ProjectsTitle = c.ProjectsTitle, ProjectsSubtitle = c.ProjectsSubtitle,
        };
        PageCopyValidator.Validate(edited).Should().Contain("The problem must be one paragraph.");
    }

    [Fact]
    public void ValidateTexts_BlankLineInInlineKey_Fails() =>
        PageCopyValidator.ValidateTexts(new Dictionary<string, string> { [PageTextKeys.WorkTogetherContract] = "a\n\nb" })
            .Should().NotBeEmpty();

    [Fact]
    public void ValidateTexts_BlankLineInBlockKey_IsFine() =>
        PageCopyValidator.ValidateTexts(new Dictionary<string, string> { [PageTextKeys.WorkIntro] = "a\n\nb" })
            .Should().BeEmpty();
}
