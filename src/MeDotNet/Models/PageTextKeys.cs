namespace MeDotNet.Models;

public enum PageTextFormat { Plain, Inline, Block }

public record PageTextDefinition(string Key, string Label, string WhereShown, PageTextFormat Format);

/// <summary>The fixed set of page text blocks. The admin edits values; it cannot add keys.</summary>
public static class PageTextKeys
{
    public const string WorkIntro = "work.intro";
    public const string WorkTogetherContract = "work.together.contract";
    public const string WorkTogetherFullTime = "work.together.fulltime";
    public const string WorkInquiryIntro = "work.inquiry.intro";
    public const string ProjectsIntro = "projects.intro";
    public const string ProjectsEmployerNote = "projects.employerNote";

    public static readonly IReadOnlyList<PageTextDefinition> All =
    [
        new(WorkIntro, "Work: intro", "/work, the paragraph under the page heading", PageTextFormat.Block),
        new(WorkTogetherContract, "Work: contract and architecture", "/work, \"How we can work together\", after the bold \"Contract & Architecture:\" label", PageTextFormat.Inline),
        new(WorkTogetherFullTime, "Work: full-time leadership", "/work, \"How we can work together\", after the bold \"Full-Time Leadership:\" label", PageTextFormat.Inline),
        new(WorkInquiryIntro, "Work: inquiry instructions", "/work, under \"Start a conversation\"", PageTextFormat.Block),
        new(ProjectsIntro, "Projects: intro", "/projects, the paragraph under the page heading", PageTextFormat.Block),
        new(ProjectsEmployerNote, "Projects: employer note", "/projects, the caption under each previous-employer project", PageTextFormat.Plain),
    ];
}
