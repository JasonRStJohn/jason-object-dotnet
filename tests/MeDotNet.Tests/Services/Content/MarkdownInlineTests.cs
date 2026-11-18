using FluentAssertions;
using MeDotNet.Services.Content;

namespace MeDotNet.Tests.Services.Content;

/// <summary>Inline mode: copy that continues a bold label inside an existing paragraph.</summary>
public class MarkdownInlineTests
{
    [Fact]
    public void ToInlineHtml_Bold_NoParagraphWrapper() =>
        MarkdownRenderer.ToInlineHtml("a **b**").Should().Be("a <strong>b</strong>");

    [Fact]
    public void ToInlineHtml_Link_Kept() =>
        MarkdownRenderer.ToInlineHtml("[x](https://example.com)").Should().Contain("href=\"https://example.com\"");

    [Fact]
    public void ToInlineHtml_MultipleParagraphs_JoinedWithSpace() =>
        MarkdownRenderer.ToInlineHtml("a\n\nb").Should().Be("a b");

    [Theory]
    [InlineData(null)]
    [InlineData("  ")]
    public void ToInlineHtml_Empty_ReturnsEmpty(string? markdown) =>
        MarkdownRenderer.ToInlineHtml(markdown).Should().BeEmpty();

    [Theory]
    [InlineData("[x](javascript:alert(1))")]
    [InlineData("x {onclick=alert(1)}")]
    [InlineData("<img src=x onerror=alert(1)>")]
    public void ToInlineHtml_UnsafeInput_Neutralised(string markdown)
    {
        var html = MarkdownRenderer.ToInlineHtml(markdown);

        html.ToLowerInvariant().Should().NotContain("javascript:");
        html.Should().NotMatchRegex(@"<[^>]*\son\w+\s*=");
        html.Should().NotContain("<img");
    }

    [Fact]
    public void ToInlineHtml_Heading_KeepsItsText() =>
        MarkdownRenderer.ToInlineHtml("# Big idea\nthen detail").Should().Be("Big idea then detail");
}
