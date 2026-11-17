namespace MeDotNet.Models;

public static class PostKindDisplay
{
    public static string Label(this PostKind kind) => kind switch
    {
        PostKind.Spec => "SPEC",
        PostKind.Plan => "PLAN",
        PostKind.PostMortem => "POST-MORTEM",
        _ => "NOTE"
    };

    /// <summary>
    /// The CSS class that styles this kind (wwwroot/app.css): an accent colour plus a
    /// distinct outline style, so the kind never depends on colour alone.
    /// </summary>
    public static string CssClass(this PostKind kind) => kind switch
    {
        PostKind.Spec => "fm-kind-spec",
        PostKind.Plan => "fm-kind-plan",
        PostKind.PostMortem => "fm-kind-postmortem",
        _ => "fm-kind-note"
    };

    public static string FilterName(PostKind kind) => kind switch
    {
        PostKind.Spec => "spec",
        PostKind.Plan => "plan",
        PostKind.PostMortem => "postmortem",
        _ => "note"
    };

    public static bool TryParseFilter(string? value, out PostKind kind)
    {
        switch (value?.ToLowerInvariant())
        {
            case "spec": kind = PostKind.Spec; return true;
            case "plan": kind = PostKind.Plan; return true;
            case "postmortem": kind = PostKind.PostMortem; return true;
            case "note": kind = PostKind.Note; return true;
            default: kind = PostKind.Note; return false;
        }
    }
}
