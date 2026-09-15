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

    [Fact]
    public void ToHtml_JavaScriptSchemeLink_IsNeutralised()
    {
        var html = MarkdownRenderer.ToHtml("[click me](javascript:alert(1))");

        html.Should().NotContain("href=\"javascript:");
    }

    [Fact]
    public void ToHtml_CaseVariantJavaScriptSchemeLink_IsNeutralised()
    {
        var html = MarkdownRenderer.ToHtml("[click me](JavaScript:alert(1))");

        html.Should().NotContainEquivalentOf("href=\"javascript:");
    }

    [Fact]
    public void ToHtml_WhitespacePrefixedJavaScriptSchemeLink_IsNeutralised()
    {
        var html = MarkdownRenderer.ToHtml("[click me](  javascript:alert(1))");

        html.Should().NotContainEquivalentOf("href=\"javascript:");
    }

    [Fact]
    public void ToHtml_ControlCharacterObfuscatedJavaScriptSchemeLink_IsNeutralised()
    {
        // "java&#9;script:" is a markdown entity for a tab character embedded in the URL,
        // which Markdig decodes into an actual tab in LinkInline.Url. A naive
        // StartsWith("javascript:") check on the raw string would miss this.
        var html = MarkdownRenderer.ToHtml("[click me](java&#9;script:alert(1))");

        html.Should().NotContainEquivalentOf("javascript:");
        html.Should().Contain("href=\"#\"");
    }

    [Fact]
    public void ToHtml_DataSchemeLink_IsNeutralised()
    {
        var html = MarkdownRenderer.ToHtml("[click me](data:text/html,<script>alert(1)</script>)");

        html.Should().NotContain("href=\"data:");
    }

    [Fact]
    public void ToHtml_HttpAndHttpsLinks_SurviveWithHrefIntact()
    {
        var httpHtml = MarkdownRenderer.ToHtml("[link](http://example.com)");
        var httpsHtml = MarkdownRenderer.ToHtml("[link](https://example.com)");

        httpHtml.Should().Contain("href=\"http://example.com\"");
        httpsHtml.Should().Contain("href=\"https://example.com\"");
    }

    [Fact]
    public void ToHtml_MailtoLink_Survives()
    {
        var html = MarkdownRenderer.ToHtml("[email me](mailto:someone@example.com)");

        html.Should().Contain("href=\"mailto:someone@example.com\"");
    }

    [Fact]
    public void ToHtml_RelativeLink_SurvivesUnchanged()
    {
        var html = MarkdownRenderer.ToHtml("[notes](/notes/foo)");

        html.Should().Contain("href=\"/notes/foo\"");
    }

    [Fact]
    public void ToHtml_RelativeImage_StillProducesImgWithSrc()
    {
        var html = MarkdownRenderer.ToHtml("![alt](/img/x.jpg)");

        html.Should().Contain("<img");
        html.Should().Contain("src=\"/img/x.jpg\"");
    }

    [Fact]
    public void ToHtml_JavaScriptSchemeImage_IsNeutralised()
    {
        var html = MarkdownRenderer.ToHtml("![x](javascript:alert(1))");

        html.Should().NotContain("src=\"javascript:");
    }

    [Fact]
    public void ToHtml_DataSchemeImage_IsNeutralised()
    {
        var html = MarkdownRenderer.ToHtml("![x](data:image/svg+xml;base64,PHN2ZyBvbmxvYWQ9YWxlcnQoMSkgLz4=)");

        html.Should().Contain("src=\"#\"");
        html.Should().NotContain("src=\"data:");
    }

    [Fact]
    public void ToHtml_EmptyLinkDestination_DoesNotThrowAndRendersWithEmptyHref()
    {
        var html = MarkdownRenderer.ToHtml("[text]()");

        html.Should().NotBeNull();
        html.Should().Contain("<a href=\"\">text</a>");
    }
}
