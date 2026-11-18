namespace MeDotNet.Components;

/// <summary>
/// One colour per problem category, shared by the /work panel and the /projects group.
/// A design decision kept in code (DESIGN.md, "Colour Is A Role"), never edited in the admin.
/// </summary>
public static class CategoryAccent
{
    public static string For(string key) => key switch
    {
        "ai-systems" => "fm-accent-violet",
        "platforms" => "fm-accent-cyan",
        _ => "fm-accent-green",
    };
}
