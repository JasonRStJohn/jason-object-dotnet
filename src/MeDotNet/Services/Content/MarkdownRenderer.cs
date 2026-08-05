using Markdig;

namespace MeDotNet.Services.Content;

/// <summary>
/// Renders record entry bodies from markdown to HTML.
/// Raw HTML is disabled: bodies are admin-authored, but the renderer should not be
/// the thing standing between a compromised admin account and stored XSS.
/// </summary>
public static class MarkdownRenderer
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .DisableHtml()
        .Build();

    public static string ToHtml(string? markdown) =>
        string.IsNullOrWhiteSpace(markdown) ? string.Empty : Markdown.ToHtml(markdown, Pipeline);
}
