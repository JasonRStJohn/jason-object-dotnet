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

    public static string RailColor(this PostKind kind) => kind switch
    {
        PostKind.Spec => "#7a5cff",
        PostKind.Plan => "#7a5cff",
        PostKind.PostMortem => "#d4763a",
        _ => "#2fb47c"
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
