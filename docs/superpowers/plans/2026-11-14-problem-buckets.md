# Phase 6 — Problem-Buckets Positioning & Portfolio Update Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Update `/work`, `/projects`, and `/` on `jasonobject.work` (`medotnet`) to frame Jason's work around three problem-diagnosis buckets linked directly to anchored 2026 portfolio sections.

**Architecture:** Pure Razor component updates across `Work.razor`, `Projects.razor`, and `Home.razor` using existing MudBlazor primitives (`MudPaper`, `MudCard`, `MudText`, `MudButton`, `MudLink`). No database schema or service changes required.

**Tech Stack:** .NET 9, Blazor Web App (Server), MudBlazor 9.6, xUnit.

**Spec:** `docs/superpowers/specs/2026-11-14-problem-buckets-design.md`

## Global Constraints

- Never name freelance or contract clients in any public-facing copy; describe the engineering problem and architecture only.
- Describe `ClaudePress` as a model-agnostic Nuxt 3 + PocketBase layer so future renaming to `AnchorPress` is a single string replacement.
- Work strictly on branch `feature/problem-buckets` (with `main` preserved at `3d88d3f` for instant rollback).

---

### Task 1: Rework `/work` Around Three Problem-Diagnosis Cards (`Work.razor`)

**Files:**
- Modify: `src/MeDotNet/Components/Pages/Work.razor`

**Interfaces:**
- Consumes: MudBlazor components (`MudPaper`, `MudText`, `MudButton`, `MudList`, `MudListItem`, `MudLink`)
- Produces: Route `/work` with links to `/projects#ai-systems`, `/projects#platforms`, and `/projects#claudepress`

- [ ] **Step 1: Update `src/MeDotNet/Components/Pages/Work.razor`**

```razor
@page "/work"

<PageTitle>Work with me — JasonObject</PageTitle>

<h1 class="mud-typography mud-typography-h3 mud-typography-gutterbottom">Work with me</h1>

<MudText Typo="Typo.body1" Class="mb-6">
    I take on contract architecture and development work—and I'm open to the right lead or principal
    engineering role. Below are the three problems people usually bring me in to solve.
</MudText>

<MudPaper Class="pa-6 mb-4" Elevation="1">
    <h2 class="mud-typography mud-typography-h5 mud-typography-gutterbottom">
        1. Curated AI Harnesses &amp; Workflow Architecture
    </h2>
    <MudText Typo="Typo.body1" Class="mb-3">
        <strong>The problem:</strong> Organizations adopting AI usually get stuck between two bad options:
        pasting sensitive context into raw chat windows with no guardrails, or buying black-box "agent"
        platforms that hallucinate the moment they touch messy enterprise data.
    </MudText>
    <MudText Typo="Typo.body1" Class="mb-3">
        <strong>How I approach it:</strong> Directing models reliably is an instruction and systems-architecture
        problem. I build constrained, multi-stage harnesses that sit between your existing systems of record
        (ERPs, databases, internal APIs) and the people making decisions—separating context assembly,
        generation, and cold verification so outputs are auditable and repeatable.
    </MudText>
    <MudButton Href="/projects#ai-systems" Variant="Variant.Text" Color="Color.Primary"
               EndIcon="@Icons.Material.Filled.ArrowForward" Class="px-0">
        See the AI harness &amp; workflow systems
    </MudButton>
</MudPaper>

<MudPaper Class="pa-6 mb-4" Elevation="1">
    <h2 class="mud-typography mud-typography-h5 mud-typography-gutterbottom">
        2. Custom Line-of-Business &amp; Civic Software
    </h2>
    <MudText Typo="Typo.body1" Class="mb-3">
        <strong>The problem:</strong> Operations teams, startups, and municipalities regularly outgrow
        spreadsheets and off-the-shelf SaaS, or get trapped paying steep licensing fees for rigid legacy
        software that doesn't match how their people actually work.
    </MudText>
    <MudText Typo="Typo.body1" Class="mb-3">
        <strong>How I approach it:</strong> I architect and ship custom web platforms and automation engines
        in .NET, C#, Vue/Nuxt, and SQL—from multi-workflow production systems and airspace tooling to civic
        utilities and operational dashboards. Clean data models, permissions that match the floor, and
        codebases another engineer can actually maintain.
    </MudText>
    <MudButton Href="/projects#platforms" Variant="Variant.Text" Color="Color.Primary"
               EndIcon="@Icons.Material.Filled.ArrowForward" Class="px-0">
        See the platform &amp; line-of-business builds
    </MudButton>
</MudPaper>

<MudPaper Class="pa-6 mb-6" Elevation="1">
    <h2 class="mud-typography mud-typography-h5 mud-typography-gutterbottom">
        3. Legacy Web &amp; Fleet Migrations
    </h2>
    <MudText Typo="Typo.body1" Class="mb-3">
        <strong>The problem:</strong> Small businesses, public library systems, and local governments often
        run on aging WordPress installs buried under twenty plugins—slow, fragile on update day, and
        confusing for staff who just need to post an agenda, a catalog update, or an event.
    </MudText>
    <MudText Typo="Typo.body1" Class="mb-3">
        <strong>How I approach it:</strong> I built <strong>ClaudePress</strong> (and vertical layers like
        <strong>ShelfPress</strong> for public libraries and <strong>PawPress</strong> for specialty
        businesses)—a fast, model-agnostic Nuxt 3 + PocketBase stack with an admin interface tailored to
        the organization's actual content, paired with an automated migration pipeline that moves sites off
        WordPress without billing for a hundred hours of hand-copying.
    </MudText>
    <MudButton Href="/projects#claudepress" Variant="Variant.Text" Color="Color.Primary"
               EndIcon="@Icons.Material.Filled.ArrowForward" Class="px-0">
        See the web platforms &amp; migrations
    </MudButton>
</MudPaper>

<MudPaper Class="pa-6 mb-8" Elevation="1">
    <h2 class="mud-typography mud-typography-h5 mud-typography-gutterbottom">How we can work together</h2>
    <MudText Typo="Typo.body1" Class="mb-3">
        <strong>Contract &amp; Architecture:</strong> Available for scoped system builds, ERP and AI harness
        architecture, fractional technical leadership, and single-site or fleet web migrations.
    </MudText>
    <MudText Typo="Typo.body1">
        <strong>Full-Time Leadership:</strong> Open to Lead, Staff, or Principal engineering roles (remote
        or around Upstate New York) with teams that value clear architecture and pragmatic AI adoption over hype.
    </MudText>
</MudPaper>

<h2 class="mud-typography mud-typography-h5 mud-typography-gutterbottom">Get in touch</h2>
<MudText Typo="Typo.body1" Class="mb-3">The fastest way to reach me is email.</MudText>
<MudList T="string" Dense="true">
    <MudListItem T="string" Icon="@Icons.Material.Filled.Email">
        <MudLink Href="mailto:jasonrstjohn@gmail.com">jasonrstjohn@gmail.com</MudLink>
    </MudListItem>
    <MudListItem T="string" Icon="@Icons.Custom.Brands.GitHub">
        <MudLink Href="https://github.com/JasonRStJohn" Target="_blank">github.com/JasonRStJohn</MudLink>
    </MudListItem>
    <MudListItem T="string" Icon="@Icons.Custom.Brands.LinkedIn">
        <MudLink Href="https://www.linkedin.com/in/jasonrstjohn" Target="_blank">linkedin.com/in/jasonrstjohn</MudLink>
    </MudListItem>
</MudList>
```

- [ ] **Step 2: Build and run tests to verify compilation**

Run: `dotnet test MeDotNet.sln`
Expected: PASS

- [ ] **Step 3: Commit**

```bash
git add src/MeDotNet/Components/Pages/Work.razor
git commit -m "feat(work): structure /work around three problem-diagnosis buckets"
```

---

### Task 2: Group `/projects` Into Anchored Sections & Add 2026 Work (`Projects.razor`)

**Files:**
- Modify: `src/MeDotNet/Components/Pages/Projects.razor`

**Interfaces:**
- Consumes: Anchor links `/projects#ai-systems`, `/projects#platforms`, `/projects#claudepress`
- Produces: Grouped project cards on `/projects` with matching section `id` attributes and updated former-employer caption

- [ ] **Step 1: Update `src/MeDotNet/Components/Pages/Projects.razor`**

```razor
@page "/projects"

<PageTitle>Projects — JasonObject</PageTitle>

<h1 class="mud-typography mud-typography-h3 mud-typography-gutterbottom">Projects</h1>

<MudText Typo="Typo.body1" Class="mb-6">
    Grouped by the kind of problem they solve. Public repositories link straight to GitHub; client and
    employer systems are described by their architecture so proprietary details stay private.
</MudText>

@foreach (var group in _groups)
{
    <section id="@group.Id" class="mb-8">
        <h2 class="mud-typography mud-typography-h5 mb-1">@group.Title</h2>
        <MudText Typo="Typo.body2" Class="mud-text-secondary mb-4">@group.Subtitle</MudText>

        @foreach (var project in group.Projects)
        {
            <MudCard Class="mb-4" Elevation="1">
                <MudCardContent>
                    <h3 class="mud-typography mud-typography-h6">@project.Title</h3>
                    <MudText Typo="Typo.caption" Class="d-block mb-2">@project.Meta</MudText>
                    <MudText Typo="Typo.body1">@project.Body</MudText>
                    @if (project.Employer)
                    {
                        <MudText Typo="Typo.caption" Class="d-block mt-2 mud-text-secondary">
                            Built during an 8-year tenure as Lead Developer — proprietary system, ask me about the architecture.
                        </MudText>
                    }
                </MudCardContent>
                @if (project.LinkHref is not null || project.RecordProject is not null)
                {
                    <MudCardActions>
                        @if (project.LinkHref is not null)
                        {
                            <MudButton Href="@project.LinkHref" Target="_blank" Variant="Variant.Text"
                                       Color="Color.Primary" EndIcon="@Icons.Material.Filled.OpenInNew">
                                @project.LinkText
                            </MudButton>
                        }
                        @if (project.RecordProject is not null)
                        {
                            <MudButton Href="@($"/notes?project={Uri.EscapeDataString(project.RecordProject!)}")" Variant="Variant.Text"
                                       Color="Color.Primary" EndIcon="@Icons.Material.Filled.ArrowForward">
                                Read the record
                            </MudButton>
                        }
                    </MudCardActions>
                }
            </MudCard>
        }
    </section>
}

@code {
    private record ProjectEntry(string Title, string Meta, string Body, string? LinkText, string? LinkHref, bool Employer, string? RecordProject = null);
    private record ProjectGroup(string Id, string Title, string Subtitle, ProjectEntry[] Projects);

    private static readonly ProjectGroup[] _groups =
    [
        new("ai-systems",
            "AI Harnesses & Agent-Directed Systems",
            "Constrained, multi-stage workflows that put structure and verification around model output.",
            [
                new("Enterprise Workforce & ERP AI Harness",
                    "Contract Architecture, 2026 · Enterprise ERP Integration, Curated AI Harness",
                    "Scoping and architecting a workforce management platform that bridges enterprise ERP data (Oracle Fusion) with a curated AI evaluation harness—giving operational leaders real-time, grounded feedback on personnel moves across multiple projects and regions without exposing raw data to unconstrained prompts.",
                    null, null, false),

                new("Site-Creator 2.0 — Agentic Site Factory",
                    "Internal Production Pipeline, 2026 · Multi-Agent Workflow, Nuxt 3, PocketBase, Vitest",
                    "A five-department, model-agnostic agentic pipeline (Intake, Architecture, Design, Implementation, and QA) that ingests client briefs, design mockups, or legacy WordPress exports and produces tested, production-ready web applications. Each stage operates from isolated Markdown context files with automated verification gates between handoffs.",
                    null, null, false),

                new("Agentic Job Search",
                    "Personal / Open Source, 2026 · Multi-Agent Harness, Python, Markdown",
                    "A portable multi-department AI workflow that separates generation (resume tailoring and drafting) from cold verification (zero-context screener simulation and source-of-truth auditing). Built entirely in plain Markdown context files with no framework lock-in.",
                    "Source on GitHub", "https://github.com/JasonRStJohn/agentic-job-search", false),

                new("jasonobject.work — this site",
                    "Personal, 2026 · Blazor Web App, .NET 9, EF Core, ASP.NET Identity, MudBlazor, Docker",
                    "The site you're reading, built as a working demonstration of AI-assisted engineering: every phase ran brainstorm → spec → implementation plan → agent-executed tasks, with code review gating each one. Blazor Web App with a hand-rolled CMS, EF Core, Identity auth, and MudBlazor, shipping via Docker.",
                    "Source on GitHub", "https://github.com/JasonRStJohn/jason-object-dotnet", false, RecordProject: "JasonObject"),
            ]),

        new("platforms",
            "Line-of-Business, Airspace & Real-Time Systems",
            "Custom operational software, workflow engines, and real-time applications.",
            [
                new("Civilian VTOL Airspace & ATC Tooling",
                    "Contract, 2026 · Aviation & Airspace Operations Software",
                    "Contract engineering for an emerging aviation technology company building mid-range air traffic control and operational software to support the rollout of civilian VTOL aircraft.",
                    null, null, false),

                new("Job Management Platform 2.0",
                    "Previous Employer · .NET Core, Vue, Nuxt",
                    "Lead developer and architect of the company's flagship rebuild — job creation, dynamic workflow management, and progress tracking on a modern stack, with inventory and production reporting on the roadmap.",
                    null, null, true),

                new("Automation scripting engine",
                    "Previous Employer · C#, JSON instruction sets",
                    "A modular automation language that consumes JSON instruction sets to process mailing lists and artwork into personalized, USPS-sorted mail files for the production floor. My favorite part: a workflow that generates its own instructions from defined criteria.",
                    null, null, true),

                new("Ballot production & tracking",
                    "Previous Employer · Vue, .NET Framework",
                    "Primary developer on a high-velocity system carrying election ballot orders from intake through production, then tracking mail pieces to voters and back to boards of elections. It leaned hard on the automation engine above — and the pace taught me a lot about technical debt, by way of the flexibility it cost that engine.",
                    null, null, true),

                new("Client Portal 2.0",
                    "Previous Employer · Vue",
                    "One of three developers who salvaged an outsourced client portal — inherited stack choices and all — reorganizing the codebase into something maintainable enough to hand off for continued development.",
                    null, null, true),

                new("Multi-workflow job system",
                    "Previous Employer · C#",
                    "My first major C# project: re-architected a single-workflow job system so one job could run multiple template-generated workflows, with steps and ordering editable by management on the fly.",
                    null, null, true),

                new("Reactive invoicing system",
                    "Previous Employer · Vue, .NET Framework",
                    "Asked to make invoicing \"look like PayPal\" in a jQuery shop, I introduced Vue instead — CDN-loaded, no build step, but reactive. It seeded the modern front-end stack the company now uses everywhere.",
                    null, null, true),

                new("Chasma & CYOA",
                    "Personal / Open Source, 2026 · Python, WebSockets, Rust, Docker",
                    "Two explorations in real-time state and systems programming: Chasma, an online multiplayer 2D space arena game running over WebSockets, and CYOA, an interactive branching narrative engine built in Rust while learning the language alongside an agent.",
                    "Chasma on GitHub", "https://github.com/JasonRStJohn/Chasma", false),
            ]),

        new("claudepress",
            "Web Platforms & Legacy Migrations",
            "Replacing fragile, plugin-heavy WordPress installs with fast, self-hosted Nuxt + PocketBase applications.",
            [
                new("ClaudePress, ShelfPress & PawPress",
                    "Open Source Core + Vertical Layers, 2026 · Nuxt 3, Vue, TypeScript, PocketBase, Tailwind, Docker",
                    "A modular, model-agnostic Nuxt 3 + PocketBase content and application layer designed to replace bloated WordPress stacks. Extended into specialized vertical platforms including ShelfPress (for public library systems with catalog, event, and board-governance needs) and PawPress (for specialty breeders and kennels).",
                    "ClaudePress on GitHub", "https://github.com/JasonRStJohn/ClaudePress", false),

                new("Municipal & Small-Business Platform Migrations",
                    "Contract, 2026 · Nuxt 3, PocketBase, Automated Content Migration, VPS Infrastructure",
                    "Migrating municipal village government and commercial sites off fragile legacy WordPress installs onto self-hosted ClaudePress stacks—preserving years of public records, meeting minutes, and search indexing while giving non-technical staff a clean, foolproof publishing workflow.",
                    null, null, false),

                new("CateredTo",
                    "Infinity Graphics · WordPress, PHP",
                    "A menu-editing plugin for a catering business that ballooned into a full quoting system with invoice generation. In the long run it shouldn't have been a WordPress plugin — and it should have taught me more about scope creep than it did.",
                    "Source on GitHub", "https://github.com/JasonRStJohn/catered-to", false),

                new("AGFTC website",
                    "Infinity Graphics · WordPress (underscores base)",
                    "My first theme written mostly from scratch, with hand-built support for multiple post types, for the Adirondack/Glens Falls Transportation Council. The organization has since been reorganized, so the live site's days may be numbered.",
                    "agftc.org", "https://agftc.org", false),
            ]),
    ];
}
```

- [ ] **Step 2: Build and run tests**

Run: `dotnet test MeDotNet.sln`
Expected: PASS

- [ ] **Step 3: Commit**

```bash
git add src/MeDotNet/Components/Pages/Projects.razor
git commit -m "feat(projects): group projects by problem domain and add 2026 systems"
```

---

### Task 3: Update Homepage JSON Card & Add `/work` Signpost (`Home.razor`)

**Files:**
- Modify: `src/MeDotNet/Components/Pages/Home.razor:37-62`

**Interfaces:**
- Consumes: `/work` route
- Produces: Updated `"builds"` JSON property and signpost paragraph linking to `/work`

- [ ] **Step 1: Update `src/MeDotNet/Components/Pages/Home.razor`**

Update `"builds"` in the JSON card and add the signpost `<MudText>` block below the bio paragraph:

```razor
  <span class="j-k">"builds"</span><span class="j-p">:</span>
    <span class="j-s">"platforms, AI harnesses, and the tools others work in"</span><span class="j-p">,</span>
```

And under the bio `<MudText Typo="Typo.body1" Class="mt-8">` block:

```razor
<MudText Typo="Typo.body1" Class="mt-4">
    Taking on contract architecture, platform builds, and legacy migrations now—and open to the
    right lead engineering role. <MudLink Href="/work">See the problems I solve →</MudLink>
</MudText>
```

- [ ] **Step 2: Build, run tests, and rebuild live container**

Run:
```bash
dotnet test MeDotNet.sln
docker compose up -d --build
```
Expected: All tests pass, container rebuilds and serves updated `/`, `/work`, and `/projects` on `:5000`.

- [ ] **Step 3: Commit**

```bash
git add src/MeDotNet/Components/Pages/Home.razor
git commit -m "feat(home): update JSON card and add signpost link to /work"
```
