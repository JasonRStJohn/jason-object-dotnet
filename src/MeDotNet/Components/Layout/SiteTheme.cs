using MudBlazor;

namespace MeDotNet.Components.Layout;

/// <summary>
/// The "Field Manual" theme: the site set in the language of the Chasma
/// architectural infographic (deep navy, outlined panels, colour that means
/// something). Dark is the default; light is the same manual printed on white.
///
/// Colour roles are semantic and shared by both palettes, so a page picks a
/// role (Primary, Tertiary, Success...) and each mode supplies a value that
/// passes WCAG AA on its own ground:
///   Primary   cyan    links, focus, "platforms" work, Spec entries
///   Secondary amber   the one call to action, the JSON card, Note entries
///   Tertiary  violet  AI harness work, Plan entries
///   Success   green   web platforms and migrations
///   Error     coral   post-mortems
///   Info      sky     incidental badges
///   Warning   orange  incidental badges
/// Panel outlines and type details live in wwwroot/app.css (the fm- classes).
/// </summary>
public static class SiteTheme
{
    private static readonly string[] Sans = ["Archivo", "Helvetica Neue", "Arial", "sans-serif"];
    private static readonly string[] Mono = ["Martian Mono", "ui-monospace", "SFMono-Regular", "Consolas", "monospace"];

    public static MudTheme Theme { get; } = new()
    {
        PaletteDark = new PaletteDark
        {
            Background = "#0B1220",
            BackgroundGray = "#0D172A",
            Surface = "#0F1A2E",
            AppbarBackground = "rgba(11,18,32,0.92)",
            AppbarText = "#E6EDF7",
            DrawerBackground = "#0F1A2E",
            DrawerText = "#E6EDF7",
            DrawerIcon = "#9AABC4",
            TextPrimary = "#E6EDF7",
            TextSecondary = "#9AABC4",
            TextDisabled = "#5F708A",
            ActionDefault = "#9AABC4",
            ActionDisabled = "#5F708A",
            ActionDisabledBackground = "#1A2640",
            LinesDefault = "#22304A",
            LinesInputs = "#3A4A66",
            Divider = "#1E2A40",
            DividerLight = "#162135",
            TableLines = "#1E2A40",
            TableHover = "#13203A",
            TableStriped = "#0D172A",
            Skeleton = "#16233B",
            Primary = "#22D3EE",
            PrimaryContrastText = "#06202A",
            Secondary = "#FBBF24",
            SecondaryContrastText = "#1A1203",
            Tertiary = "#A78BFA",
            TertiaryContrastText = "#160B33",
            Info = "#60A5FA",
            InfoContrastText = "#071A33",
            Success = "#34D399",
            SuccessContrastText = "#04261A",
            Warning = "#FB923C",
            WarningContrastText = "#2A1203",
            Error = "#FB7185",
            ErrorContrastText = "#2E0812",
            Dark = "#06101F",
            HoverOpacity = 0.08,
        },
        PaletteLight = new PaletteLight
        {
            Background = "#F4F7FB",
            BackgroundGray = "#EAF0F7",
            Surface = "#FFFFFF",
            AppbarBackground = "rgba(255,255,255,0.94)",
            AppbarText = "#0B1220",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#0B1220",
            DrawerIcon = "#44536B",
            TextPrimary = "#0B1220",
            TextSecondary = "#44536B",
            TextDisabled = "#8A97AB",
            ActionDefault = "#44536B",
            LinesDefault = "#CDD6E3",
            LinesInputs = "#A9B5C7",
            Divider = "#DCE3EC",
            TableLines = "#DCE3EC",
            Primary = "#0E7490",
            PrimaryContrastText = "#FFFFFF",
            Secondary = "#B45309",
            SecondaryContrastText = "#FFFFFF",
            Tertiary = "#6D28D9",
            TertiaryContrastText = "#FFFFFF",
            Info = "#1D4ED8",
            InfoContrastText = "#FFFFFF",
            Success = "#047857",
            SuccessContrastText = "#FFFFFF",
            Warning = "#C2410C",
            WarningContrastText = "#FFFFFF",
            Error = "#BE123C",
            ErrorContrastText = "#FFFFFF",
            HoverOpacity = 0.06,
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
            AppbarHeight = "68px",
        },
        Shadows = new Shadow
        {
            // Outlines carry structure here, not elevation. Only floating layers
            // (menus, popovers, dialogs, snackbars: elevation 8+) keep a real shadow.
            Elevation = Enumerable.Range(0, 26)
                .Select(i => i < 8 ? "none" : "0 18px 40px -12px rgba(2,6,16,0.55), 0 4px 12px -4px rgba(2,6,16,0.35)")
                .ToArray(),
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = Sans, FontSize = "1rem", FontWeight = "400", LineHeight = "1.6", LetterSpacing = "0" },
            H1 = new H1Typography { FontFamily = Sans, FontSize = "3.5rem", FontWeight = "850", LineHeight = "1.02", LetterSpacing = "-0.015em", TextTransform = "uppercase" },
            H2 = new H2Typography { FontFamily = Sans, FontSize = "3rem", FontWeight = "850", LineHeight = "1.05", LetterSpacing = "-0.01em", TextTransform = "uppercase" },
            H3 = new H3Typography { FontFamily = Sans, FontSize = "2.75rem", FontWeight = "850", LineHeight = "1.04", LetterSpacing = "-0.01em", TextTransform = "uppercase" },
            H4 = new H4Typography { FontFamily = Sans, FontSize = "2rem", FontWeight = "800", LineHeight = "1.1", LetterSpacing = "-0.005em", TextTransform = "uppercase" },
            H5 = new H5Typography { FontFamily = Sans, FontSize = "1.125rem", FontWeight = "800", LineHeight = "1.3", LetterSpacing = "0.01em", TextTransform = "uppercase" },
            H6 = new H6Typography { FontFamily = Sans, FontSize = "1.1875rem", FontWeight = "700", LineHeight = "1.35", LetterSpacing = "0", TextTransform = "none" },
            Subtitle1 = new Subtitle1Typography { FontFamily = Sans, FontSize = "1.0625rem", FontWeight = "600", LineHeight = "1.5", LetterSpacing = "0" },
            Subtitle2 = new Subtitle2Typography { FontFamily = Sans, FontSize = "0.9375rem", FontWeight = "700", LineHeight = "1.4", LetterSpacing = "0" },
            Body1 = new Body1Typography { FontFamily = Sans, FontSize = "1.0625rem", FontWeight = "400", LineHeight = "1.65", LetterSpacing = "0" },
            Body2 = new Body2Typography { FontFamily = Sans, FontSize = "0.9375rem", FontWeight = "400", LineHeight = "1.6", LetterSpacing = "0" },
            Button = new ButtonTypography { FontFamily = Sans, FontSize = "0.875rem", FontWeight = "750", LineHeight = "1.75", LetterSpacing = "0.04em", TextTransform = "uppercase" },
            Caption = new CaptionTypography { FontFamily = Sans, FontSize = "0.8125rem", FontWeight = "400", LineHeight = "1.5", LetterSpacing = "0.01em" },
            Overline = new OverlineTypography { FontFamily = Mono, FontSize = "0.6875rem", FontWeight = "500", LineHeight = "1.6", LetterSpacing = "0.06em", TextTransform = "uppercase" },
        },
    };
}
