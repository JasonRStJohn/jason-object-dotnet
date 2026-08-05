using System.Text.RegularExpressions;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace MeDotNet.Services.Content;

/// <summary>
/// Renders record entry bodies from markdown to HTML.
/// Raw HTML is disabled: bodies are admin-authored, but the renderer should not be
/// the thing standing between a compromised admin account and stored XSS.
///
/// Disabling raw HTML doesn't stop a link or image URL from using an executable
/// scheme (javascript:, data:, etc), so link/image URLs are additionally filtered
/// against an allowlist on the parsed AST before rendering.
/// </summary>
public static class MarkdownRenderer
{
    private static readonly string[] AllowedSchemes = { "http:", "https:", "mailto:" };

    // Matches a URI scheme per RFC 3986 (ALPHA *( ALPHA / DIGIT / "+" / "-" / "." ) ":"),
    // anchored at the start. A leading "/", "#", "." etc. (relative/anchor URLs) never
    // matches, so those correctly fall through as scheme-less. This also avoids false
    // positives on a colon appearing later in a relative URL's path/query/fragment
    // (e.g. "/search?q=10:30").
    private static readonly Regex SchemePattern = new(@"^[a-zA-Z][a-zA-Z0-9+.\-]*:", RegexOptions.Compiled);

    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .DisableHtml()
        .Build();

    public static string ToHtml(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown)) return string.Empty;

        var document = Markdig.Markdown.Parse(markdown, Pipeline);

        foreach (var link in document.Descendants<LinkInline>())
        {
            if (!IsSafeUrl(link.Url)) link.Url = "#";
        }

        return document.ToHtml(Pipeline);
    }

    /// <summary>
    /// Allowlist check: a URL is safe if, after stripping all control characters
    /// (a deliberate superset of what the WHATWG URL spec requires; the spec requires
    /// stripping only ASCII tab, CR, LF, and leading/trailing C0-control-or-space, but
    /// being over-inclusive here fails safe) and surrounding whitespace, it either has
    /// no scheme at all (relative/anchor URLs like "/img/x.jpg", "../thing", "#section")
    /// or its scheme is one of http, https, mailto (compared case-insensitively).
    /// Everything else - javascript:, data:, vbscript:, file:, etc - is rejected.
    /// </summary>
    private static bool IsSafeUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true; // no href to attack

        var cleaned = new string(url.Where(c => !char.IsControl(c)).ToArray()).Trim();
        if (cleaned.Length == 0) return true;

        var match = SchemePattern.Match(cleaned);
        if (!match.Success) return true; // no scheme -> relative/anchor URL, allowed

        return AllowedSchemes.Contains(match.Value, StringComparer.OrdinalIgnoreCase);
    }
}
