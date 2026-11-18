namespace MeDotNet.Models;

/// <summary>A named block of page copy. The set of keys is fixed in <see cref="PageTextKeys"/>.</summary>
public class PageText
{
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}
