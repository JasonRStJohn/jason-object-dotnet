using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MeDotNet.Components;

/// <summary>
/// Renders an &lt;h2&gt;..&lt;h6&gt; chosen at runtime, so a reusable block can sit at
/// the right level in each page's heading outline.
/// </summary>
public class DynamicHeading : ComponentBase
{
    private static readonly HashSet<string> Allowed = ["h2", "h3", "h4", "h5", "h6"];

    [Parameter] public string Tag { get; set; } = "h3";
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Allowed.Contains(Tag) ? Tag : "h3");
        builder.AddAttribute(1, "class", Class);
        builder.AddContent(2, ChildContent);
        builder.CloseElement();
    }
}
