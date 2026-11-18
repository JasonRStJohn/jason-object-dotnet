using System.Text.RegularExpressions;
using FluentAssertions;

namespace MeDotNet.Tests.Components;

/// <summary>
/// Dev scripts must pass secrets to docker by name (-e NAME, inherited from the
/// environment), never as -e NAME="$VALUE", which puts the value in the process list.
/// </summary>
public class ScriptSecretsGuardTests
{
    private static readonly Regex ExpandedEnvArg = new(@"-e\s+[A-Z_]+=""?\$", RegexOptions.Compiled);

    [Fact]
    public void Scripts_NeverExpandSecretsIntoDockerEnvArgs()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "MeDotNet.sln"))) root = root.Parent;
        var scripts = Directory.EnumerateFiles(Path.Combine(root!.FullName, "scripts"), "*.sh", SearchOption.AllDirectories);

        var offenders = scripts
            .SelectMany(f => File.ReadLines(f).Select((line, i) => (f, line, i)))
            .Where(x => ExpandedEnvArg.IsMatch(x.line))
            .Select(x => $"{Path.GetFileName(x.f)}:{x.i + 1}: {x.line.Trim()}")
            .ToList();

        offenders.Should().BeEmpty();
    }
}
