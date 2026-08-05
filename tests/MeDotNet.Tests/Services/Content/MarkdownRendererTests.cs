using FluentAssertions;
using MeDotNet.Services.Content;

namespace MeDotNet.Tests.Services.Content;

public class MarkdownRendererTests
{
    [Fact]
    public void ToHtml_Heading_RendersAsHeadingTag_NotLiteralHash()
    {
        var html = MarkdownRenderer.ToHtml("# Title");

        html.Should().MatchRegex("<h1[^>]*>Title</h1>");
        html.Should().NotContain("#");
    }

    [Fact]
    public void ToHtml_Link_RendersAsAnchorTag()
    {
        var html = MarkdownRenderer.ToHtml("[JasonObject](https://example.com)");

        html.Should().Contain("<a href=\"https://example.com\"");
    }

    [Fact]
    public void ToHtml_Image_RendersAsImgTag()
    {
        var html = MarkdownRenderer.ToHtml("![alt](/img/x.jpg)");

        html.Should().Contain("<img");
        html.Should().Contain("src=\"/img/x.jpg\"");
    }

    [Fact]
    public void ToHtml_RawHtml_IsNeutralised()
    {
        var html = MarkdownRenderer.ToHtml("<script>alert('x')</script>");

        html.Should().NotContain("<script");
    }

    [Fact]
    public void ToHtml_FencedCodeBlock_RendersAsPreCode()
    {
        var html = MarkdownRenderer.ToHtml("```\nvar x = 1;\n```");

        html.Should().Contain("<pre><code");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ToHtml_NullOrEmptyOrWhitespace_ReturnsEmptyString(string? input)
    {
        MarkdownRenderer.ToHtml(input).Should().BeEmpty();
    }
}
