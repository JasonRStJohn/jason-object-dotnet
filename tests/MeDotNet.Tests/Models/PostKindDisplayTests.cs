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
    [InlineData(PostKind.Spec, "#7a5cff")]
    [InlineData(PostKind.Plan, "#7a5cff")]
    [InlineData(PostKind.PostMortem, "#d4763a")]
    [InlineData(PostKind.Note, "#2fb47c")]
    public void RailColor_ReturnsSpecifiedHex(PostKind kind, string expected)
    {
        kind.RailColor().Should().Be(expected);
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
