# Phase 4 — Site Content & Identity Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Rebrand the site from MeDotNet to JasonObject and add real content — identity hero on Home, an About narrative page, and a nine-project Projects page.

**Architecture:** Pure content/markup work on the existing MudBlazor shell. Two new static Razor pages (no services, no PersistentLoader), one page rewrite that preserves its data loading, and a branding sweep across layout, page titles, and the login page. All copy comes verbatim from the spec.

**Tech Stack:** .NET 9, Blazor Web App (global InteractiveServer + prerender), MudBlazor 9.6.0

**Spec:** `docs/superpowers/specs/2026-07-07-site-content-design.md` — the copy in this plan is transcribed from it and must be implemented verbatim (typos included, if any; do not editorialize).

## Global Constraints

- Branch: `feature/site-content` (never commit to main)
- `dotnet` is at `$HOME/.dotnet/dotnet`; always `export PATH="$HOME/.dotnet/tools:$HOME/.dotnet:$PATH"` and `export DOTNET_ROOT="$HOME/.dotnet"` before dotnet commands
- Build: `dotnet build src/MeDotNet/MeDotNet.csproj --nologo` → 0 warnings, 0 errors after every task
- Tests: `dotnet test tests/MeDotNet.Tests/ --nologo --logger "console;verbosity=minimal"` → Passed: 14 after every task
- Every page keeps exactly one `<h1 class="mud-typography mud-typography-h3 ...">` (the Phase 3 FocusOnNavigate pattern); section headings are `<h2 class="mud-typography mud-typography-h5 ...">`
- Solution/namespaces stay `MeDotNet` — the rebrand is user-visible strings only
- No new NuGet packages, no DB changes, no new tests (content-only phase; suite must stay green)

---

### Task 1: Branding sweep — layout, titles, login, contact icons

**Files:**
- Modify: `src/MeDotNet/Components/Layout/MainLayout.razor`
- Modify: `src/MeDotNet/Components/Pages/Posts.razor` (PageTitle line only)
- Modify: `src/MeDotNet/Components/Pages/PostDetail.razor` (PageTitle line only)
- Modify: `src/MeDotNet/Pages/Account/Login.cshtml` (title + heading only)

**Interfaces:**
- Produces: nav links `about` and `projects` pointing at routes created in Tasks 3 and 2's pages do not exist yet after this task — the two `MudNavLink`s will 404 until Tasks 2–3 land. That is expected mid-plan state; note it in your report rather than "fixing" it.

- [ ] **Step 1: Update MainLayout.razor**

In `src/MeDotNet/Components/Layout/MainLayout.razor`:

(a) Replace the app-bar title line:

```razor
        <MudText Typo="Typo.h6" Class="ml-2">MeDotNet</MudText>
```

with:

```razor
        <MudText Typo="Typo.h6" Class="ml-2">JasonObject</MudText>
```

(b) Directly after `<MudSpacer />` and before the dark-mode toggle `MudIconButton`, insert the three contact icon links:

```razor
        <MudIconButton Icon="@Icons.Custom.Brands.GitHub" Color="Color.Inherit"
                       Href="https://github.com/JasonRStJohn" Target="_blank" aria-label="GitHub" />
        <MudIconButton Icon="@Icons.Custom.Brands.LinkedIn" Color="Color.Inherit"
                       Href="https://www.linkedin.com/in/jasonrstjohn" Target="_blank" aria-label="LinkedIn" />
        <MudIconButton Icon="@Icons.Material.Filled.Email" Color="Color.Inherit"
                       Href="mailto:jasonrstjohn@gmail.com" aria-label="Email" />
```

(c) In the `MudNavMenu`, insert About and Projects between the Home and Blog links:

```razor
            <MudNavLink Href="about" Icon="@Icons.Material.Filled.Person">About</MudNavLink>
            <MudNavLink Href="projects" Icon="@Icons.Material.Filled.Code">Projects</MudNavLink>
```

so the top of the menu reads Home, About, Projects, Blog. The `AuthorizeView` block below stays untouched.

- [ ] **Step 2: Update the public blog PageTitles**

In `src/MeDotNet/Components/Pages/Posts.razor`, replace:

```razor
<PageTitle>Blog</PageTitle>
```

with:

```razor
<PageTitle>Blog — JasonObject</PageTitle>
```

In `src/MeDotNet/Components/Pages/PostDetail.razor`, replace:

```razor
<PageTitle>@(_post?.Title ?? "Not Found")</PageTitle>
```

with:

```razor
<PageTitle>@($"{_post?.Title ?? "Not Found"} — JasonObject")</PageTitle>
```

- [ ] **Step 3: Update Login.cshtml branding**

In `src/MeDotNet/Pages/Account/Login.cshtml`, replace:

```html
    <title>Login — MeDotNet</title>
```

with:

```html
    <title>Login — JasonObject</title>
```

and replace:

```html
        <h1>MeDotNet</h1>
```

with:

```html
        <h1>JasonObject</h1>
```

- [ ] **Step 4: Build and test**

```bash
export PATH="$HOME/.dotnet/tools:$HOME/.dotnet:$PATH" && export DOTNET_ROOT="$HOME/.dotnet"
dotnet build src/MeDotNet/MeDotNet.csproj --nologo
dotnet test tests/MeDotNet.Tests/ --nologo --logger "console;verbosity=minimal"
```

Expected: 0 warnings 0 errors; `Passed: 14`.

- [ ] **Step 5: Commit**

```bash
git add src/MeDotNet/Components/Layout/MainLayout.razor \
        src/MeDotNet/Components/Pages/Posts.razor \
        src/MeDotNet/Components/Pages/PostDetail.razor \
        src/MeDotNet/Pages/Account/Login.cshtml
git commit -m "feat: rebrand to JasonObject, add About/Projects nav and contact icons"
```

---

### Task 2: Home rewrite — hero, demo callout, pointers

**Files:**
- Modify: `src/MeDotNet/Components/Pages/Home.razor`

**Interfaces:**
- Consumes: existing `PersistentLoader` + recent-posts `@code` block (must remain byte-for-byte unchanged).
- Produces: buttons linking to `/about` and `/projects` (Task 3 creates those pages; if Task 3 hasn't run yet they 404 — expected).

- [ ] **Step 1: Rewrite the markup above "Recent posts"**

In `src/MeDotNet/Components/Pages/Home.razor`, replace everything from `<PageTitle>` through the closing `</MudPaper>` of the hero (currently lines 6–11) with:

```razor
<PageTitle>JasonObject — Jason St. John</PageTitle>

<MudPaper Class="pa-6 mb-6" Elevation="1">
    <h1 class="mud-typography mud-typography-h3 mud-typography-gutterbottom">Jason St. John</h1>
    <p class="mud-typography mud-typography-h5 mud-typography-gutterbottom">
        Lead full-stack developer. I teach machines to build software.
    </p>
    <MudText Typo="Typo.body1">
        Eight years architecting production .NET and Vue/Nuxt platforms — and a teaching background
        that turns out to be the sharpest tool in AI-assisted engineering. Directing coding agents is
        an instruction problem: decomposition, scaffolding, feedback, and knowing whether the system
        misunderstood or just lacked context. I've spent my whole career getting good at exactly that.
    </MudText>
</MudPaper>

<MudPaper Class="pa-4 mb-4" Elevation="1">
    <MudText Typo="Typo.body1">
        <b>This site is the demo.</b> A Blazor Web App — EF Core, Identity, MudBlazor, Docker —
        designed, specced, and built by directing AI coding agents through plan-driven development,
        with every task reviewed before it landed.
        <MudLink Href="https://github.com/JasonRStJohn/jason-object-dotnet" Target="_blank">Read the code →</MudLink>
    </MudText>
</MudPaper>

<div class="d-flex gap-4 mb-8">
    <MudButton Href="/about" Variant="Variant.Filled" Color="Color.Primary">About me</MudButton>
    <MudButton Href="/projects" Variant="Variant.Outlined" Color="Color.Primary">See the projects</MudButton>
</div>
```

Everything from the `<h2 ...>Recent posts</h2>` line to the end of the file — including the whole `@code` block — stays byte-for-byte unchanged.

- [ ] **Step 2: Build and test**

```bash
export PATH="$HOME/.dotnet/tools:$HOME/.dotnet:$PATH" && export DOTNET_ROOT="$HOME/.dotnet"
dotnet build src/MeDotNet/MeDotNet.csproj --nologo
dotnet test tests/MeDotNet.Tests/ --nologo --logger "console;verbosity=minimal"
```

Expected: 0 warnings 0 errors; `Passed: 14`.

- [ ] **Step 3: Commit**

```bash
git add src/MeDotNet/Components/Pages/Home.razor
git commit -m "feat: real Home hero with identity, thesis, and demo callout"
```

---

### Task 3: About and Projects pages

**Files:**
- Create: `src/MeDotNet/Components/Pages/About.razor`
- Create: `src/MeDotNet/Components/Pages/Projects.razor`

**Interfaces:**
- Consumes: routes `/about` and `/projects` already linked from MainLayout (Task 1) and Home (Task 2).
- Produces: nothing consumed later.

- [ ] **Step 1: Create About.razor**

Create `src/MeDotNet/Components/Pages/About.razor`:

```razor
@page "/about"

<PageTitle>About — JasonObject</PageTitle>

<h1 class="mud-typography mud-typography-h3 mud-typography-gutterbottom">About</h1>

<h2 class="mud-typography mud-typography-h5 mud-typography-gutterbottom">The short version</h2>
<MudText Typo="Typo.body1" Class="mb-6">
    I'm Jason St. John — lead full-stack developer and software architect in Upstate New York.
    For the last eight-plus years I've built and led development on production .NET and Vue/Nuxt
    platforms: job management systems, automation engines, election ballot tracking. Before that
    I ran a web development business, worked IT, and taught social studies. The teaching part
    matters more than it looks.
</MudText>

<h2 class="mud-typography mud-typography-h5 mud-typography-gutterbottom">The arc</h2>
<MudText Typo="Typo.body1" Class="mb-6">
    My first computer was an early Mac that wouldn't play any games, so I taught myself BASIC to
    have some fun with it. That instinct — if the thing doesn't do what you want, learn to make
    it — never left. College was a tour through engineering, psychology, creative writing, and
    music before landing in education, because the constant was that I loved learning itself. I
    taught social studies for a few years, then came back to tech: IT and networking at a small
    shop in Glens Falls, then four years building custom WordPress themes and plugins under
    Infinity Graphics. When a C# developer job came along, I took it — and went developer →
    senior → lead by shipping the systems on the <MudLink Href="/projects">Projects</MudLink> page.
</MudText>

<h2 class="mud-typography mud-typography-h5 mud-typography-gutterbottom">The thesis</h2>
<MudText Typo="Typo.body1" Class="mb-6">
    Directing AI coding agents well is an instruction problem. You decompose the work, scaffold
    the context, give formative feedback, and diagnose whether the model misunderstood or simply
    didn't have what it needed. That is teaching. A graduate degree in education plus a decade of
    shipping production software is a strange combination that happens to be exactly right for
    this moment: I can architect the system <i>and</i> design the instructions that get it built.
    This site is the working demo — specced, planned, and implemented by directing AI agents task
    by task, with review gating every merge.
</MudText>

<h2 class="mud-typography mud-typography-h5 mud-typography-gutterbottom">Get in touch</h2>
<MudText Typo="Typo.body1" Class="mb-2">
    The fastest way to reach me is email. I'm also on GitHub and LinkedIn.
</MudText>
<MudList T="string" ReadOnly="true">
    <MudListItem Icon="@Icons.Material.Filled.Email" Href="mailto:jasonrstjohn@gmail.com">
        jasonrstjohn@gmail.com
    </MudListItem>
    <MudListItem Icon="@Icons.Custom.Brands.GitHub" Href="https://github.com/JasonRStJohn" Target="_blank">
        github.com/JasonRStJohn
    </MudListItem>
    <MudListItem Icon="@Icons.Custom.Brands.LinkedIn" Href="https://www.linkedin.com/in/jasonrstjohn" Target="_blank">
        linkedin.com/in/jasonrstjohn
    </MudListItem>
</MudList>
```

Note: `MudList`/`MudListItem` are generic in MudBlazor ≥7 — the `T="string"` on `MudList` is required. If `Target` is not a parameter on `MudListItem` in 9.6.0, drop the two `Target="_blank"` attributes from list items rather than restructuring (external links opening in the same tab is acceptable); note any such adjustment in your report.

- [ ] **Step 2: Create Projects.razor**

Create `src/MeDotNet/Components/Pages/Projects.razor`:

```razor
@page "/projects"

<PageTitle>Projects — JasonObject</PageTitle>

<h1 class="mud-typography mud-typography-h3 mud-typography-gutterbottom">Projects</h1>

<MudText Typo="Typo.body1" Class="mb-6">
    Nine projects, newest first. The employer-built systems can't be linked, but they're the bulk
    of the story — ask me about any of them.
</MudText>

@foreach (var project in _projects)
{
    <MudCard Class="mb-4" Elevation="1">
        <MudCardContent>
            <h2 class="mud-typography mud-typography-h6">@project.Title</h2>
            <MudText Typo="Typo.caption" Class="d-block mb-2">@project.Meta</MudText>
            <MudText Typo="Typo.body1">@project.Body</MudText>
            @if (project.Employer)
            {
                <MudText Typo="Typo.caption" Class="d-block mt-2 mud-text-secondary">
                    Built at my current employer — not publicly available.
                </MudText>
            }
        </MudCardContent>
        @if (project.LinkHref is not null)
        {
            <MudCardActions>
                <MudButton Href="@project.LinkHref" Target="_blank" Variant="Variant.Text"
                           Color="Color.Primary" EndIcon="@Icons.Material.Filled.OpenInNew">
                    @project.LinkText
                </MudButton>
            </MudCardActions>
        }
    </MudCard>
}

@code {
    private record ProjectEntry(string Title, string Meta, string Body, string? LinkText, string? LinkHref, bool Employer);

    private static readonly ProjectEntry[] _projects =
    [
        new("jasonobject.work — this site",
            "Personal, 2026 · Blazor Web App, .NET 9, EF Core, ASP.NET Identity, MudBlazor, Docker",
            "The site you're reading, built as a working demonstration of AI-assisted engineering: every phase ran brainstorm → spec → implementation plan → agent-executed tasks, with code review gating each one. Blazor Web App with a hand-rolled CMS, EF Core, Identity auth, and MudBlazor, shipping via Docker.",
            "Source on GitHub", "https://github.com/JasonRStJohn/jason-object-dotnet", false),

        new("Job Management Platform 2.0",
            "Employer · .NET Core, Vue, Nuxt",
            "Lead developer and architect of the company's flagship rebuild — job creation, dynamic workflow management, and progress tracking on a modern stack, with inventory and production reporting on the roadmap.",
            null, null, true),

        new("Automation scripting engine",
            "Employer · C#, JSON instruction sets",
            "A modular automation language that consumes JSON instruction sets to process mailing lists and artwork into personalized, USPS-sorted mail files for the production floor. My favorite part: a workflow that generates its own instructions from defined criteria.",
            null, null, true),

        new("Ballot production & tracking",
            "Employer · Vue, .NET Framework",
            "Primary developer on a high-velocity system carrying election ballot orders from intake through production, then tracking mail pieces to voters and back to boards of elections. It leaned hard on the automation engine above — and the pace taught me a lot about technical debt, by way of the flexibility it cost that engine.",
            null, null, true),

        new("Client Portal 2.0",
            "Employer · Vue",
            "One of three developers who salvaged an outsourced client portal — inherited stack choices and all — reorganizing the codebase into something maintainable enough to hand off for continued development.",
            null, null, true),

        new("Multi-workflow job system",
            "Employer · C#",
            "My first major C# project: re-architected a single-workflow job system so one job could run multiple template-generated workflows, with steps and ordering editable by management on the fly.",
            null, null, true),

        new("Reactive invoicing system",
            "Employer · Vue, .NET Framework",
            "Asked to make invoicing \"look like PayPal\" in a jQuery shop, I introduced Vue instead — CDN-loaded, no build step, but reactive. It seeded the modern front-end stack the company now uses everywhere.",
            null, null, true),

        new("CateredTo",
            "Infinity Graphics · WordPress, PHP",
            "A menu-editing plugin for a catering business that ballooned into a full quoting system with invoice generation. In the long run it shouldn't have been a WordPress plugin — and it should have taught me more about scope creep than it did.",
            "Source on GitHub", "https://github.com/JasonRStJohn/catered-to", false),

        new("AGFTC website",
            "Infinity Graphics · WordPress (underscores base)",
            "My first theme written mostly from scratch, with hand-built support for multiple post types, for the Adirondack/Glens Falls Transportation Council. The organization has since been reorganized, so the live site's days may be numbered.",
            "agftc.org", "https://agftc.org", false),
    ];
}
```

- [ ] **Step 3: Build and test**

```bash
export PATH="$HOME/.dotnet/tools:$HOME/.dotnet:$PATH" && export DOTNET_ROOT="$HOME/.dotnet"
dotnet build src/MeDotNet/MeDotNet.csproj --nologo
dotnet test tests/MeDotNet.Tests/ --nologo --logger "console;verbosity=minimal"
```

Expected: 0 warnings 0 errors; `Passed: 14`.

- [ ] **Step 4: Smoke check (optional if Docker unavailable — note in report)**

```bash
docker compose up --build -d
```

Verify: `/`, `/about`, `/projects`, `/posts` all render with correct titles and exactly one `h1` each; no user-visible "MeDotNet" remains (`curl -s http://localhost:5000/ http://localhost:5000/about http://localhost:5000/projects http://localhost:5000/posts http://localhost:5000/account/login | grep -c MeDotNet` → 0); employer cards show the muted caption and no link button; the four public links (repo ×2, agftc.org, LinkedIn/GitHub/mailto in app bar) are present in the HTML.

- [ ] **Step 5: Commit**

```bash
git add src/MeDotNet/Components/Pages/About.razor \
        src/MeDotNet/Components/Pages/Projects.razor
git commit -m "feat: add About narrative page and nine-project Projects page"
```
