using MeDotNet.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MeDotNet.Data.Seed;

/// <summary>
/// The /work and /projects copy as it stood when it moved into the database
/// (feature/design-identity, 2026-11-18), used once by the EditablePageCopy migration.
/// Generated from Work.razor and Projects.razor so the wording is exact.
/// Frozen once merged: later copy changes happen in the admin, never here.
/// </summary>
public static class PageCopySeed
{
    public static readonly IReadOnlyList<ProjectCategory> Categories =
    [
        new()
        {
            Key = "ai-systems",
            SortOrder = 1,
            WorkTitle = "1. Curated AI Harnesses & Workflow Architecture",
            WorkTag = "Architecture / Sprint",
            WorkProblem = "Organizations adopting AI usually get stuck between two bad options: pasting sensitive context into raw chat windows with no guardrails, or buying black-box \"agent\" platforms that hallucinate the moment they touch messy enterprise data.",
            WorkApproach = "Directing models reliably is an instruction and systems-architecture problem. I build constrained, multi-stage harnesses that sit between your existing systems of record (ERPs, databases, internal APIs) and the people making decisions. Separating context assembly, generation, and cold verification keeps outputs auditable and repeatable.",
            WorkLinkText = "See the AI harness & workflow systems",
            InquiryLabel = "AI Harness & Workflow Architecture",
            ProjectsShortLabel = "AI Harnesses",
            ProjectsTitle = "AI Harnesses & Agent-Directed Systems",
            ProjectsSubtitle = "Constrained, multi-stage workflows that put structure and verification around model output.",
        },
        new()
        {
            Key = "platforms",
            SortOrder = 2,
            WorkTitle = "2. Custom Line-of-Business & Civic Software",
            WorkTag = "Project / Fractional Lead",
            WorkProblem = "Operations teams, startups, and municipalities regularly outgrow spreadsheets and off-the-shelf SaaS, or get trapped paying steep licensing fees for rigid legacy software that doesn't match how their people actually work.",
            WorkApproach = "I architect and ship custom web platforms and automation engines in .NET, C#, Vue/Nuxt, and SQL, from multi-workflow production systems and airspace tooling to civic utilities and operational dashboards. Clean data models, permissions that match the floor, and codebases another engineer can actually maintain.",
            WorkLinkText = "See the platform & line-of-business builds",
            InquiryLabel = "Custom Platform / .NET / Vue Build",
            ProjectsShortLabel = "Platforms & .NET",
            ProjectsTitle = "Line-of-Business, Airspace & Real-Time Systems",
            ProjectsSubtitle = "Custom operational software, workflow engines, and real-time applications.",
        },
        new()
        {
            Key = "claudepress",
            SortOrder = 3,
            WorkTitle = "3. Legacy Web & Fleet Migrations",
            WorkTag = "Fixed-Scope / Retainer",
            WorkProblem = "Small businesses, public library systems, and local governments often run on aging WordPress installs buried under twenty plugins. They are slow, fragile on update day, and confusing for staff who just need to post an agenda, a catalog update, or an event.",
            WorkApproach = "I built **ClaudePress** (which currently underlies five live production sites, plus vertical layers like **PawPress**). It is a fast, model-agnostic Nuxt 3 + PocketBase stack with an admin interface tailored to the organization's actual content, paired with an automated migration pipeline that moves sites off WordPress without billing for a hundred hours of hand-copying.",
            WorkLinkText = "See the web platforms & migrations",
            InquiryLabel = "Legacy Web / ClaudePress Migration",
            ProjectsShortLabel = "Web & Migrations",
            ProjectsTitle = "Web Platforms & Legacy Migrations",
            ProjectsSubtitle = "Replacing fragile, plugin-heavy WordPress installs with fast, self-hosted Nuxt + PocketBase applications.",
        },
    ];

    public static readonly IReadOnlyList<Project> Projects =
    [
        new()
        {
            Id = 1,
            CategoryKey = "ai-systems",
            SortOrder = 1,
            Title = "Enterprise Workforce & ERP AI Harness",
            Badge = "Contract Architecture",
            Meta = "2026 · Enterprise ERP Integration, Curated AI Harness",
            Body = "Scoping and architecting a workforce management platform that bridges enterprise ERP data (Oracle Fusion) with a curated AI evaluation harness. Built to give operational leaders real-time, grounded feedback on personnel moves across multiple projects and regions without exposing raw data to unconstrained prompts.",
            Tags = "Enterprise ERP\nAI Harness Architecture\nWorkforce Systems",
            LinkText = null,
            LinkHref = null,
            IsEmployer = false,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 2,
            CategoryKey = "ai-systems",
            SortOrder = 2,
            Title = "Site-Creator 2.0 (Agentic Site Factory)",
            Badge = "Internal Pipeline",
            Meta = "2026 · Multi-Agent Workflow, Nuxt 3, PocketBase, Vitest",
            Body = "A five-department, model-agnostic agentic pipeline (Intake, Architecture, Design, Implementation, and QA) that ingests client briefs, design mockups, or legacy WordPress exports and produces tested, production-ready web applications. Each stage operates from isolated Markdown context files with automated verification gates between handoffs.",
            Tags = "Multi-Agent Pipeline\nNuxt 3\nPocketBase\nVitest",
            LinkText = null,
            LinkHref = null,
            IsEmployer = false,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 3,
            CategoryKey = "ai-systems",
            SortOrder = 3,
            Title = "Agentic Job Search",
            Badge = "Open Source",
            Meta = "2026 · Multi-Agent Harness, Python, Markdown",
            Body = "A portable multi-department AI workflow that separates generation (resume tailoring and drafting) from cold verification (zero-context screener simulation and source-of-truth auditing). Built in plain Markdown context files with no framework lock-in.",
            Tags = "AI Evaluation\nPython\nPortable Contexts",
            LinkText = "Source on GitHub",
            LinkHref = "https://github.com/JasonRStJohn/agentic-job-search",
            IsEmployer = false,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 4,
            CategoryKey = "ai-systems",
            SortOrder = 4,
            Title = "jasonobject.work (this site)",
            Badge = "Live in Production",
            Meta = "2026 · Blazor Web App, .NET 9, EF Core, ASP.NET Identity, MudBlazor, Docker",
            Body = "The site you're reading, built as a working demonstration of AI-assisted engineering. Every phase ran from brainstorm to spec to implementation plan to agent-executed tasks, with code review gating each merge. Blazor Web App with a hand-rolled CMS, EF Core, Identity auth, and MudBlazor, shipping via Docker.",
            Tags = ".NET 9\nBlazor Server\nEF Core\nSQL Server\nMudBlazor\nDocker",
            LinkText = "Source on GitHub",
            LinkHref = "https://github.com/JasonRStJohn/jason-object-dotnet",
            IsEmployer = false,
            RecordProject = "JasonObject",
            IsVisible = true,
        },
        new()
        {
            Id = 5,
            CategoryKey = "platforms",
            SortOrder = 1,
            Title = "Civilian VTOL Airspace & ATC Tooling",
            Badge = "Contract",
            Meta = "2026 · Aviation & Airspace Operations Software",
            Body = "Contract engineering for an emerging aviation technology company building mid-range air traffic control and operational software to support the rollout of civilian VTOL aircraft.",
            Tags = "Airspace Operations\nWeb Engineering\nReal-Time Workflows",
            LinkText = null,
            LinkHref = null,
            IsEmployer = false,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 6,
            CategoryKey = "platforms",
            SortOrder = 2,
            Title = "Job Management Platform 2.0",
            Badge = "Previous Employer",
            Meta = "Lead Architect · .NET Core, Vue, Nuxt",
            Body = "Lead developer and architect of the company's flagship rebuild: job creation, dynamic workflow management, and progress tracking on a modern stack, with inventory and production reporting on the roadmap.",
            Tags = ".NET Core\nC#\nVue\nNuxt\nSQL Server",
            LinkText = null,
            LinkHref = null,
            IsEmployer = true,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 7,
            CategoryKey = "platforms",
            SortOrder = 3,
            Title = "Automation scripting engine",
            Badge = "Previous Employer",
            Meta = "Lead Developer · C#, JSON instruction sets",
            Body = "A modular automation language that consumes JSON instruction sets to process mailing lists and artwork into personalized, USPS-sorted mail files for the production floor. My favorite part: a workflow that generates its own instructions from defined criteria.",
            Tags = "C#\nJSON DSL\nWorkflow Automation",
            LinkText = null,
            LinkHref = null,
            IsEmployer = true,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 8,
            CategoryKey = "platforms",
            SortOrder = 4,
            Title = "Ballot production & tracking",
            Badge = "Previous Employer",
            Meta = "Primary Developer · Vue, .NET Framework",
            Body = "Primary developer on a high-velocity system carrying election ballot orders from intake through production, then tracking mail pieces to voters and back to boards of elections. It leaned hard on the automation engine above, and the pace taught me a lot about technical debt by way of the flexibility it cost that engine.",
            Tags = "Vue\n.NET Framework\nHigh-Volume Tracking",
            LinkText = null,
            LinkHref = null,
            IsEmployer = true,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 9,
            CategoryKey = "platforms",
            SortOrder = 5,
            Title = "Client Portal 2.0",
            Badge = "Previous Employer",
            Meta = "Senior Developer · Vue",
            Body = "One of three developers who salvaged an outsourced client portal (inherited stack choices and all), reorganizing the codebase into something maintainable enough to hand off for continued development.",
            Tags = "Vue\nLegacy Rescue\nRefactoring",
            LinkText = null,
            LinkHref = null,
            IsEmployer = true,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 10,
            CategoryKey = "platforms",
            SortOrder = 6,
            Title = "Multi-workflow job system",
            Badge = "Previous Employer",
            Meta = "Developer · C#",
            Body = "My first major C# project: re-architected a single-workflow job system so one job could run multiple template-generated workflows, with steps and ordering editable by management on the fly.",
            Tags = "C#\n.NET\nState Machines",
            LinkText = null,
            LinkHref = null,
            IsEmployer = true,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 11,
            CategoryKey = "platforms",
            SortOrder = 7,
            Title = "Reactive invoicing system",
            Badge = "Previous Employer",
            Meta = "Developer · Vue, .NET Framework",
            Body = "Asked to make invoicing \"look like PayPal\" in a jQuery shop, I introduced Vue instead (CDN-loaded, no build step, but reactive). It seeded the modern front-end stack the company now uses everywhere.",
            Tags = "Vue\n.NET Framework",
            LinkText = null,
            LinkHref = null,
            IsEmployer = true,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 12,
            CategoryKey = "platforms",
            SortOrder = 8,
            Title = "Chasma & CYOA",
            Badge = "Open Source",
            Meta = "2026 · Python, WebSockets, Rust, Docker",
            Body = "Two explorations in real-time state and systems programming: Chasma, an online multiplayer 2D space arena game running over WebSockets (whose ships occasionally fly across the background of this site in dark mode), and CYOA, an interactive branching narrative engine built in Rust while learning the language alongside an agent.",
            Tags = "Python\nWebSockets\nRust\nDocker",
            LinkText = "Chasma on GitHub",
            LinkHref = "https://github.com/JasonRStJohn/Chasma",
            IsEmployer = false,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 13,
            CategoryKey = "claudepress",
            SortOrder = 1,
            Title = "ClaudePress & PawPress",
            Badge = "Open Source Core",
            Meta = "2026 · Nuxt 3, Vue, TypeScript, PocketBase, Tailwind, Docker",
            Body = "A modular, model-agnostic Nuxt 3 + PocketBase content and application layer designed to replace bloated WordPress stacks, currently underlying five live production sites. Extended into specialized vertical layers including PawPress (for specialty breeders and kennels) and a current prototype for public libraries (ShelfPress).",
            Tags = "Nuxt 3\nVue 3\nTypeScript\nPocketBase\nTailwind CSS",
            LinkText = "ClaudePress on GitHub",
            LinkHref = "https://github.com/JasonRStJohn/ClaudePress",
            IsEmployer = false,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 14,
            CategoryKey = "claudepress",
            SortOrder = 2,
            Title = "Municipal & Small-Business Platform Migrations",
            Badge = "Live in Production",
            Meta = "2026 · Nuxt 3, PocketBase, Automated Content Migration, VPS Infrastructure",
            Body = "Migrating municipal village government and commercial sites off fragile legacy WordPress installs onto self-hosted ClaudePress stacks. Preserves years of public records, meeting minutes, and search indexing while giving non-technical staff a clean, foolproof publishing workflow.",
            Tags = "Legacy Migration\nNuxt 3\nPocketBase\nLinux VPS",
            LinkText = null,
            LinkHref = null,
            IsEmployer = false,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 15,
            CategoryKey = "claudepress",
            SortOrder = 3,
            Title = "CateredTo",
            Badge = "Historical",
            Meta = "Infinity Graphics · WordPress, PHP",
            Body = "A menu-editing plugin for a catering business that ballooned into a full quoting system with invoice generation. In the long run it shouldn't have been a WordPress plugin, and it should have taught me more about scope creep than it did.",
            Tags = "PHP\nWordPress Plugin",
            LinkText = "Source on GitHub",
            LinkHref = "https://github.com/JasonRStJohn/catered-to",
            IsEmployer = false,
            RecordProject = null,
            IsVisible = true,
        },
        new()
        {
            Id = 16,
            CategoryKey = "claudepress",
            SortOrder = 4,
            Title = "AGFTC website",
            Badge = "Historical",
            Meta = "Infinity Graphics · WordPress (underscores base)",
            Body = "My first theme written mostly from scratch, with hand-built support for multiple post types, for the Adirondack/Glens Falls Transportation Council. The organization has since been reorganized, so the live site's days may be numbered.",
            Tags = "WordPress Theme\nPHP",
            LinkText = "agftc.org",
            LinkHref = "https://agftc.org",
            IsEmployer = false,
            RecordProject = null,
            IsVisible = true,
        },
    ];

    public static readonly IReadOnlyList<PageText> Texts =
    [
        new() { Key = PageTextKeys.WorkIntro, Value = "I take on contract architecture and development work, and I'm open to the right lead or principal engineering role. Below are the three problems people usually bring me in to solve." },
        new() { Key = PageTextKeys.WorkTogetherContract, Value = "Available for scoped system builds, ERP and AI harness architecture, fractional technical leadership, and single-site or fleet web migrations." },
        new() { Key = PageTextKeys.WorkTogetherFullTime, Value = "Open to Lead, Staff, or Principal engineering roles (remote or around Upstate New York) with teams that value clear architecture and pragmatic AI adoption over hype." },
        new() { Key = PageTextKeys.WorkInquiryIntro, Value = "Pick a track and timeline below to format a quick note, then open it in your email client or copy it to your clipboard. Or skip the form and email [jasonrstjohn@gmail.com](mailto:jasonrstjohn@gmail.com) directly." },
        new() { Key = PageTextKeys.ProjectsIntro, Value = "Grouped by the kind of problem they solve. Public repositories link straight to GitHub. Client and employer systems are described by their architecture so proprietary details stay private." },
        new() { Key = PageTextKeys.ProjectsEmployerNote, Value = "Built during an 8-year tenure as Lead Developer. Proprietary system; ask me about the architecture." },
    ];

    /// <summary>Inserts every seed row. Called at the end of the migration's Up.</summary>
    public static void Apply(MigrationBuilder migrationBuilder)
    {
        foreach (var c in Categories)
            migrationBuilder.InsertData("ProjectCategories",
                ["Key", "SortOrder", "WorkTitle", "WorkTag", "WorkProblem", "WorkApproach", "WorkLinkText",
                 "InquiryLabel", "ProjectsShortLabel", "ProjectsTitle", "ProjectsSubtitle"],
                [c.Key, c.SortOrder, c.WorkTitle, c.WorkTag, c.WorkProblem, c.WorkApproach, c.WorkLinkText,
                 c.InquiryLabel, c.ProjectsShortLabel, c.ProjectsTitle, c.ProjectsSubtitle]);

        foreach (var p in Projects)
            migrationBuilder.InsertData("Projects",
                ["Id", "CategoryKey", "SortOrder", "Title", "Badge", "Meta", "Body", "Tags",
                 "LinkText", "LinkHref", "IsEmployer", "RecordProject", "IsVisible"],
                [p.Id, p.CategoryKey, p.SortOrder, p.Title, p.Badge, p.Meta, p.Body, p.Tags,
                 p.LinkText, p.LinkHref, p.IsEmployer, p.RecordProject, p.IsVisible]);

        foreach (var t in Texts)
            migrationBuilder.InsertData("PageTexts", ["Key", "Value"], [t.Key, t.Value]);
    }
}
