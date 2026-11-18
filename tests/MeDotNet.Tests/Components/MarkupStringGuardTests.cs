using FluentAssertions;

namespace MeDotNet.Tests.Components;

/// <summary>
/// The one route from stored text to raw HTML on a page is MarkdownRenderer, which
/// sanitizes its output. Any other MarkupString cast would bypass that, so it fails here.
/// </summary>
public class MarkupStringGuardTests
{
    [Fact]
    public void Components_OnlyCastRendererOutputToMarkupString()
    {
        var root = FindRepoRoot();
        var components = Path.Combine(root, "src", "MeDotNet", "Components");

        var offenders = Directory.EnumerateFiles(components, "*.*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".razor") || f.EndsWith(".cs"))
            .SelectMany(f => File.ReadLines(f).Select((line, i) => (f, line, i)))
            .Where(x => x.line.Contains("MarkupString") && !x.line.Contains("MarkdownRenderer."))
            .Select(x => $"{Path.GetRelativePath(root, x.f)}:{x.i + 1}: {x.line.Trim()}")
            .ToList();

        offenders.Should().BeEmpty("raw HTML must come from MarkdownRenderer, which sanitizes it");
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "MeDotNet.sln")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("MeDotNet.sln not found above the test output");
    }
}
