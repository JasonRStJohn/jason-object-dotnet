using System.Text.RegularExpressions;
using Ganss.Xss;
using Markdig;
using Markdig.Extensions.GenericAttributes;
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
///
/// Markdig's generic-attributes syntax ({onclick=...}) is removed from the pipeline,
/// because it let Markdown attach event handlers and styles to any element. Finally,
/// the rendered HTML passes an allowlist sanitizer, so whatever a future pipeline
/// option produces, no script, event attribute, style or unsafe URL reaches the page.
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

    private static readonly MarkdownPipeline Pipeline = BuildPipeline();

    private static MarkdownPipeline BuildPipeline()
    {
        var builder = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .DisableHtml();
        builder.Extensions.RemoveAll(e => e is GenericAttributesExtension);
        return builder.Build();
    }

    // Configured once; HtmlSanitizer is safe to share across threads after setup.
    private static readonly HtmlSanitizer Sanitizer = BuildSanitizer();

    private static HtmlSanitizer BuildSanitizer()
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedAttributes.Remove("style");
        sanitizer.AllowedCssProperties.Clear();
        sanitizer.AllowedSchemes.Clear();
        foreach (var scheme in new[] { "http", "https", "mailto" })
            sanitizer.AllowedSchemes.Add(scheme);
        sanitizer.AllowedAttributes.Add("id"); // heading anchors from auto-identifiers
        return sanitizer;
    }

    public static string ToHtml(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown)) return string.Empty;

        var document = Markdig.Markdown.Parse(markdown, Pipeline);

        foreach (var link in document.Descendants<LinkInline>())
        {
            if (!IsSafeUrl(link.Url)) link.Url = "#";
        }

        return Sanitizer.Sanitize(document.ToHtml(Pipeline));
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
