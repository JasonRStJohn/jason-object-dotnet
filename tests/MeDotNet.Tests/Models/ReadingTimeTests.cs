using FluentAssertions;
using MeDotNet.Models;

namespace MeDotNet.Tests.Models;

public class ReadingTimeTests
{
    [Theory]
    [InlineData("", 1)]
    [InlineData("one two three", 1)]
    [InlineData(null, 1)]
    public void Minutes_IsAtLeastOne(string? body, int expected) =>
        ReadingTime.Minutes(body).Should().Be(expected);

    [Fact]
    public void Minutes_RoundsUpAt220WordsPerMinute()
    {
        var body = string.Join(' ', Enumerable.Repeat("word", 441));
        ReadingTime.Minutes(body).Should().Be(3);
    }
}
