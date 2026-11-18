using FluentAssertions;
using MeDotNet.Services.Content;

namespace MeDotNet.Tests.Services.Content;

/// <summary>
/// Render-time sanitizing: whatever Markdown an admin types, the HTML that reaches the
/// page carries no script. Links and images must still come through intact.
/// </summary>
public class MarkdownSanitizingTests
{
    [Theory]
    [InlineData("hello {onclick=\"alert(1)\"}")]
    [InlineData("# Title {onmouseover=alert(1)}")]
    [InlineData("[x](https://example.com){onclick=alert(1)}")]
    [InlineData("![i](/img/a.png){onerror=alert(1)}")]
    [InlineData("text {style=\"position:fixed;inset:0\"}")]
    public void ToHtml_AttributeSyntax_CannotAddEventHandlersOrStyles(string markdown)
    {
        var html = MarkdownRenderer.ToHtml(markdown);

        // Checked inside tags only: the syntax itself may survive as harmless literal text.
        html.Should().NotMatchRegex(@"<[^>]*\son\w+\s*=");
        html.Should().NotMatchRegex(@"<[^>]*\sstyle\s*=");
    }

    [Theory]
    [InlineData("[site](https://example.com)", "href=\"https://example.com\"")]
    [InlineData("[plain](http://example.com)", "href=\"http://example.com\"")]
    [InlineData("[mail](mailto:someone@example.com)", "href=\"mailto:someone@example.com\"")]
    [InlineData("[projects](/projects)", "href=\"/projects\"")]
    [InlineData("[filtered](/notes?project=JasonObject)", "href=\"/notes?project=JasonObject\"")]
    [InlineData("[jump](#the-arc)", "href=\"#the-arc\"")]
    [InlineData("see https://example.com/page today", "href=\"https://example.com/page\"")]
    public void ToHtml_Links_SurviveSanitizing(string markdown, string expected)
    {
        MarkdownRenderer.ToHtml(markdown).Should().Contain(expected);
    }

    [Fact]
    public void ToHtml_Image_SurvivesSanitizingWithAlt()
    {
        var html = MarkdownRenderer.ToHtml("![A foggy coast](/img/foggy-coast.jpg)");

        html.Should().Contain("src=\"/img/foggy-coast.jpg\"");
        html.Should().Contain("alt=\"A foggy coast\"");
    }

    [Theory]
    [InlineData("[x](javascript:alert(1))")]
    [InlineData("[x](JaVaScRiPt:alert(1))")]
    [InlineData("[x](data:text/html,<script>alert(1)</script>)")]
    [InlineData("![x](javascript:alert(1))")]
    public void ToHtml_ExecutableSchemes_NeverReachThePage(string markdown)
    {
        var html = MarkdownRenderer.ToHtml(markdown).ToLowerInvariant();

        html.Should().NotContain("javascript:");
        html.Should().NotContain("data:");
    }

    [Fact]
    public void ToHtml_RecordFormatting_StillRenders()
    {
        var md = "## Section\n\n> a quote\n\n| a | b |\n|---|---|\n| 1 | 2 |\n\n```\ncode\n```\n\n- item";
        var html = MarkdownRenderer.ToHtml(md);

        html.Should().MatchRegex("<h2[^>]*id=\"section\"[^>]*>Section</h2>");
        html.Should().Contain("<blockquote>");
        html.Should().Contain("<table>");
        html.Should().Contain("<pre><code>");
        html.Should().Contain("<li>item</li>");
    }
}
