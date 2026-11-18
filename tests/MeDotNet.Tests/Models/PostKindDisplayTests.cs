using FluentAssertions;
using MeDotNet.Models;

namespace MeDotNet.Tests.Models;

public class PostKindDisplayTests
{
    [Theory]
    [InlineData(PostKind.Note, "NOTE")]
    [InlineData(PostKind.Spec, "SPEC")]
    [InlineData(PostKind.Plan, "PLAN")]
    [InlineData(PostKind.PostMortem, "POST-MORTEM")]
    public void Label_ReturnsChipText(PostKind kind, string expected)
    {
        kind.Label().Should().Be(expected);
    }

    [Theory]
    [InlineData(PostKind.Spec, "fm-kind-spec")]
    [InlineData(PostKind.Plan, "fm-kind-plan")]
    [InlineData(PostKind.PostMortem, "fm-kind-postmortem")]
    [InlineData(PostKind.Note, "fm-kind-note")]
    public void CssClass_IsDistinctPerKind(PostKind kind, string expected)
    {
        // Colour and outline style both come from this class (wwwroot/app.css), so the
        // kind is never carried by colour alone.
        kind.CssClass().Should().Be(expected);
    }

    [Theory]
    [InlineData(PostKind.Note, "note")]
    [InlineData(PostKind.PostMortem, "postmortem")]
    public void FilterName_IsLowercaseToken(PostKind kind, string expected)
    {
        PostKindDisplay.FilterName(kind).Should().Be(expected);
    }

    [Theory]
    [InlineData("postmortem", PostKind.PostMortem)]
    [InlineData("POSTMORTEM", PostKind.PostMortem)]
    [InlineData("spec", PostKind.Spec)]
    public void TryParseFilter_AcceptsKnownTokensCaseInsensitively(string input, PostKind expected)
    {
        PostKindDisplay.TryParseFilter(input, out var kind).Should().BeTrue();
        kind.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("nonsense")]
    public void TryParseFilter_RejectsUnknownTokens(string? input)
    {
        PostKindDisplay.TryParseFilter(input, out _).Should().BeFalse();
    }
}
