# The Record — Phase 5 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Reframe jasonobject.work from a portfolio-plus-blog into a working record of directing AI agents, so one unforked site serves both hiring managers and contract clients.

**Architecture:** The existing `Post` entity gains three columns (`Kind`, `Project`, `Summary`) and becomes the record's storage — no new content system, no new infrastructure. The blog pages at `/posts` are rewritten as `/notes` with server-side filtering driven by query-string state, so filtered views are linkable. Home is rewritten around a typography-first hero with a sidebar carrying both asks. Kind-to-presentation mapping lives in one static class consumed by every page that renders an entry.

**Tech Stack:** .NET 9 (`net9.0`), Blazor Server with global `InteractiveServer` render mode, MudBlazor 9.6.0, EF Core 9.0.0 (SQL Server), ASP.NET Identity, xunit 2.9.2 + FluentAssertions 6.12.2 + EF Core InMemory 9.0.6, Docker Compose.

**Spec:** `docs/superpowers/specs/2026-08-05-jasonobject-record-design.md`

## Global Constraints

- **Do not change the stack.** No framework migration, no replacing MudBlazor, no renaming the `MeDotNet` solution or namespaces.
- **Brand name is `JasonObject`** in all user-visible text; `MeDotNet` stays internal (namespaces, solution, project files).
- **Display name is `Jason St. John`.**
- **No entry-level images.** No featured-image column, no upload path, no file-storage volume. Markdown bodies may reference images already committed to `wwwroot/img/`.
- **No pricing and no engagement-shape catalogue** anywhere on the site.
- **Facts must trace to** `docs/Jason_StJohn_Resume_AI_Angle.txt` and `docs/Resume_Raw_Materials.txt`. No invented claims. Never state or imply the "nobody was laid off" outcome — the C-suite story is a description of the role played, never a claim about results.
- **Rail colours are fixed:** Spec `#7a5cff`, Plan `#7a5cff`, PostMortem `#d4763a`, Note `#2fb47c`.
- **Copy marked "verbatim" is implemented exactly as written.** Do not paraphrase, re-punctuate, or "improve" it.
- **Existing tests must keep passing:** `dotnet test` is green before every commit.
- **`PersistentLoader` wiring is preserved** on every page that already uses it — the prerender/circuit dedupe must not regress.

## Prerequisites (do these before Task 1)

- [ ] **Merge `feature/site-content` into `main`.** It is 6 commits ahead and unmerged, carrying the JasonObject rebrand, About, Projects, and the Phase 5 spec. Then branch: `git checkout main && git pull && git checkout -b feature/the-record`
- [ ] **Install image tooling** (needed only by Task 8): `sudo apt install imagemagick`
- [ ] **Resolve the working tree:** commit or revert the stray `// MubBlazor Components` comment in `src/MeDotNet/Program.cs:17` (note the typo — it reads "MubBlazor"), and decide on the untracked resume/headshot files in `docs/`.

## File Structure

**Created:**

| File | Responsibility |
|---|---|
| `src/MeDotNet/Models/PostKind.cs` | The four-value enum |
| `src/MeDotNet/Models/PostKindDisplay.cs` | Kind → label and rail colour. The single source of presentation truth for entry kinds |
| `src/MeDotNet/Components/Pages/Record.razor` | `/notes` — filterable stream |
| `src/MeDotNet/Components/Pages/RecordEntry.razor` | `/notes/{slug}` — single entry |
| `src/MeDotNet/Components/Pages/Work.razor` | `/work` — both asks |
| `src/MeDotNet/Components/RecordEntryRow.razor` | One entry in a list: rail, chip, meta, title, summary. Used by both Record and Home. Placed directly in `Components/` — matching `RedirectToLogin.razor` and `PersistentLoader.cs` — because `_Imports.razor` already has `@using MeDotNet.Components` but no `.Shared` namespace |
| `src/MeDotNet/Data/Migrations/*_AddPostKindProjectSummary.cs` | EF migration (generated) |
| `tests/MeDotNet.Tests/Models/PostKindDisplayTests.cs` | Label and colour mapping |
| `wwwroot/img/*` | Shipped images |

**Modified:**

| File | Change |
|---|---|
| `src/MeDotNet/Models/Post.cs` | Three properties added |
| `src/MeDotNet/Services/Posts/PostService.cs` | Filtered query + distinct-projects query |
| `src/MeDotNet/Components/Pages/Home.razor` | Full rewrite (layout C) |
| `src/MeDotNet/Components/Pages/About.razor` | C-suite beat, photos |
| `src/MeDotNet/Components/Pages/Projects.razor` | Reorder, record links |
| `src/MeDotNet/Components/Pages/Admin/PostEdit.razor` | Kind / Project / Summary inputs |
| `src/MeDotNet/Components/Pages/Admin/Posts.razor` | Kind column |
| `src/MeDotNet/Components/Layout/MainLayout.razor` | Nav rename, Work button |
| `src/MeDotNet/Components/App.razor` | `og:image` |
| `src/MeDotNet/Program.cs` | `/posts` redirect endpoints |
| `tests/MeDotNet.Tests/Services/Posts/PostServiceTests.cs` | Filtering tests |

**Deleted:** `src/MeDotNet/Components/Pages/Posts.razor`, `src/MeDotNet/Components/Pages/PostDetail.razor` (replaced by `Record.razor` / `RecordEntry.razor`).

## A note on test coverage

The spec's testing section asks for a test that `/posts` redirects to `/notes` preserving the slug. The test project has no `Microsoft.AspNetCore.Mvc.Testing` reference and no `WebApplicationFactory` harness, and `Program.cs` runs `db.Database.MigrateAsync()` plus admin seeding at startup — so standing up an in-process host would require a live SQL Server. Building that harness is disproportionate to verifying two redirect lines.

**Decision:** redirects are verified manually with `curl` in Task 4, Step 12. Every other item in the spec's testing section is covered by automated tests. This is a deliberate, documented deviation.

---

### Task 1: Post gains Kind, Project and Summary

**Files:**
- Create: `src/MeDotNet/Models/PostKind.cs`
- Modify: `src/MeDotNet/Models/Post.cs`
- Create: `src/MeDotNet/Data/Migrations/*_AddPostKindProjectSummary.cs` (generated)
- Test: `tests/MeDotNet.Tests/Services/Posts/PostServiceTests.cs`

**Interfaces:**
- Consumes: nothing (first task)
- Produces: `MeDotNet.Models.PostKind` enum with members `Note = 0, Spec = 1, Plan = 2, PostMortem = 3`; `Post.Kind` (`PostKind`, defaults `Note`), `Post.Project` (`string?`), `Post.Summary` (`string?`)

- [ ] **Step 1: Write the failing test**

Append to `tests/MeDotNet.Tests/Services/Posts/PostServiceTests.cs`:

```csharp
    [Fact]
    public async Task CreateAsync_DefaultsKindToNote()
    {
        var db = CreateDb();
        var svc = new PostService(db);

        await svc.CreateAsync(MakePost("No Kind Given"));

        var all = await svc.GetAllAsync();
        all[0].Kind.Should().Be(PostKind.Note);
        all[0].Project.Should().BeNull();
        all[0].Summary.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_PersistsKindProjectAndSummary()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        var post = MakePost("Phase 4 Spec", published: true);
        post.Kind = PostKind.Spec;
        post.Project = "JasonObject";
        post.Summary = "The spec that decided what this site says.";

        await svc.CreateAsync(post);

        var all = await svc.GetAllAsync();
        all[0].Kind.Should().Be(PostKind.Spec);
        all[0].Project.Should().Be("JasonObject");
        all[0].Summary.Should().Be("The spec that decided what this site says.");
    }
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~PostServiceTests"`
Expected: FAIL to compile — `'Post' does not contain a definition for 'Kind'`

- [ ] **Step 3: Create the enum**

Create `src/MeDotNet/Models/PostKind.cs`:

```csharp
namespace MeDotNet.Models;

public enum PostKind
{
    Note = 0,
    Spec = 1,
    Plan = 2,
    PostMortem = 3
}
```

- [ ] **Step 4: Add the properties to Post**

In `src/MeDotNet/Models/Post.cs`, add after the `Body` property:

```csharp
    public PostKind Kind { get; set; } = PostKind.Note;
    public string? Project { get; set; }
    public string? Summary { get; set; }
```

- [ ] **Step 5: Run the test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~PostServiceTests"`
Expected: PASS, all tests green

- [ ] **Step 6: Generate the migration**

Run: `dotnet ef migrations add AddPostKindProjectSummary --project src/MeDotNet --startup-project src/MeDotNet`

- [ ] **Step 7: Verify the migration is additive and safe**

Open the generated file in `src/MeDotNet/Data/Migrations/`. Confirm three `AddColumn` calls, that `Kind` is `int` with `defaultValue: 0`, and that `Project` and `Summary` are `nullable: true`. There must be no `DropColumn` and no data-destroying operation. If `Kind` has no default value, add `defaultValue: 0` so pre-existing rows land on `Note`.

- [ ] **Step 8: Commit**

```bash
git add src/MeDotNet/Models src/MeDotNet/Data/Migrations tests/MeDotNet.Tests/Services/Posts/PostServiceTests.cs
git commit -m "feat: add Kind, Project and Summary to Post"
```

---

### Task 2: Kind presentation mapping

**Files:**
- Create: `src/MeDotNet/Models/PostKindDisplay.cs`
- Test: `tests/MeDotNet.Tests/Models/PostKindDisplayTests.cs`

**Interfaces:**
- Consumes: `PostKind` from Task 1
- Produces: extension methods `PostKind.Label()` → `string` (uppercase chip text) and `PostKind.RailColor()` → `string` (hex with leading `#`); static `PostKindDisplay.FilterName(PostKind)` → `string` (lowercase query-string token) and `PostKindDisplay.TryParseFilter(string?, out PostKind)` → `bool`

Every page that renders an entry uses this class. Rail colours must not be hardcoded anywhere else.

- [ ] **Step 1: Write the failing test**

Create `tests/MeDotNet.Tests/Models/PostKindDisplayTests.cs`:

```csharp
using FluentAssertions;
using MeDotNet.Models;

namespace MeDotNet.Tests.Models;

public class PostKindDisplayTests
{
    [Theory]
    [InlineData(PostKind.Note, "NOTE")]
    [InlineData(PostKind.Spec, "SPEC")]
    [InlineData(PostKind.Plan, "PLAN")]
    [InlineData(PostKind.PostMortem, "POST-MORTEM")]
    public void Label_ReturnsChipText(PostKind kind, string expected)
    {
        kind.Label().Should().Be(expected);
    }

    [Theory]
    [InlineData(PostKind.Spec, "#7a5cff")]
    [InlineData(PostKind.Plan, "#7a5cff")]
    [InlineData(PostKind.PostMortem, "#d4763a")]
    [InlineData(PostKind.Note, "#2fb47c")]
    public void RailColor_ReturnsSpecifiedHex(PostKind kind, string expected)
    {
        kind.RailColor().Should().Be(expected);
    }

    [Theory]
    [InlineData(PostKind.Note, "note")]
    [InlineData(PostKind.PostMortem, "postmortem")]
    public void FilterName_IsLowercaseToken(PostKind kind, string expected)
    {
        PostKindDisplay.FilterName(kind).Should().Be(expected);
    }

    [Theory]
    [InlineData("postmortem", PostKind.PostMortem)]
    [InlineData("POSTMORTEM", PostKind.PostMortem)]
    [InlineData("spec", PostKind.Spec)]
    public void TryParseFilter_AcceptsKnownTokensCaseInsensitively(string input, PostKind expected)
    {
        PostKindDisplay.TryParseFilter(input, out var kind).Should().BeTrue();
        kind.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("nonsense")]
    public void TryParseFilter_RejectsUnknownTokens(string? input)
    {
        PostKindDisplay.TryParseFilter(input, out _).Should().BeFalse();
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~PostKindDisplayTests"`
Expected: FAIL to compile — `PostKindDisplay` does not exist

- [ ] **Step 3: Write the implementation**

Create `src/MeDotNet/Models/PostKindDisplay.cs`:

```csharp
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
```

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~PostKindDisplayTests"`
Expected: PASS, 16 tests

- [ ] **Step 5: Commit**

```bash
git add src/MeDotNet/Models/PostKindDisplay.cs tests/MeDotNet.Tests/Models/PostKindDisplayTests.cs
git commit -m "feat: add kind label and rail colour mapping"
```

---

### Task 3: PostService filtering

**Files:**
- Modify: `src/MeDotNet/Services/Posts/PostService.cs`
- Test: `tests/MeDotNet.Tests/Services/Posts/PostServiceTests.cs`

**Interfaces:**
- Consumes: `PostKind` (Task 1)
- Produces:
  - `Task<List<Post>> GetPublishedAsync(PostKind? kind, string? project)` — published only, newest first, filters applied when non-null
  - `Task<List<string>> GetProjectsAsync()` — distinct non-null, non-empty `Project` values from **published** posts, alphabetical
  - The existing no-argument `GetPublishedAsync()` is retained and delegates, so `Home.razor` keeps working

- [ ] **Step 1: Write the failing tests**

Append to `tests/MeDotNet.Tests/Services/Posts/PostServiceTests.cs`:

```csharp
    private static Post MakeEntry(string title, PostKind kind, string? project, bool published = true)
    {
        var post = MakePost(title, published);
        post.Kind = kind;
        post.Project = project;
        return post;
    }

    [Fact]
    public async Task GetPublishedAsync_FiltersByKind()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("A Spec", PostKind.Spec, "JasonObject"));
        await svc.CreateAsync(MakeEntry("A Note", PostKind.Note, null));

        var result = await svc.GetPublishedAsync(PostKind.Spec, null);

        result.Should().ContainSingle();
        result[0].Title.Should().Be("A Spec");
    }

    [Fact]
    public async Task GetPublishedAsync_FiltersByProject()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("Site Spec", PostKind.Spec, "JasonObject"));
        await svc.CreateAsync(MakeEntry("Press Spec", PostKind.Spec, "ClaudePress"));

        var result = await svc.GetPublishedAsync(null, "ClaudePress");

        result.Should().ContainSingle();
        result[0].Title.Should().Be("Press Spec");
    }

    [Fact]
    public async Task GetPublishedAsync_FiltersByKindAndProjectTogether()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("Right One", PostKind.PostMortem, "JasonObject"));
        await svc.CreateAsync(MakeEntry("Wrong Kind", PostKind.Spec, "JasonObject"));
        await svc.CreateAsync(MakeEntry("Wrong Project", PostKind.PostMortem, "ClaudePress"));

        var result = await svc.GetPublishedAsync(PostKind.PostMortem, "JasonObject");

        result.Should().ContainSingle();
        result[0].Title.Should().Be("Right One");
    }

    [Fact]
    public async Task GetPublishedAsync_WithNoFilters_ReturnsAllPublished()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("Published Spec", PostKind.Spec, "JasonObject"));
        await svc.CreateAsync(MakeEntry("Draft Note", PostKind.Note, null, published: false));

        var result = await svc.GetPublishedAsync(null, null);

        result.Should().ContainSingle();
        result[0].Title.Should().Be("Published Spec");
    }

    [Fact]
    public async Task GetPublishedAsync_ExcludesDraftsWhenFiltering()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("Draft Spec", PostKind.Spec, "JasonObject", published: false));

        var result = await svc.GetPublishedAsync(PostKind.Spec, null);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProjectsAsync_ReturnsDistinctSortedNames()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("One", PostKind.Spec, "Whindancer"));
        await svc.CreateAsync(MakeEntry("Two", PostKind.Plan, "ClaudePress"));
        await svc.CreateAsync(MakeEntry("Three", PostKind.Note, "ClaudePress"));
        await svc.CreateAsync(MakeEntry("Four", PostKind.Note, null));

        var result = await svc.GetProjectsAsync();

        result.Should().Equal("ClaudePress", "Whindancer");
    }

    [Fact]
    public async Task GetProjectsAsync_ExcludesDrafts()
    {
        var db = CreateDb();
        var svc = new PostService(db);
        await svc.CreateAsync(MakeEntry("Hidden", PostKind.Spec, "SecretProject", published: false));

        var result = await svc.GetProjectsAsync();

        result.Should().BeEmpty();
    }
```

- [ ] **Step 2: Run the tests to verify they fail**

Run: `dotnet test --filter "FullyQualifiedName~PostServiceTests"`
Expected: FAIL to compile — no overload for `GetPublishedAsync` takes 2 arguments; `GetProjectsAsync` not found

- [ ] **Step 3: Write the implementation**

In `src/MeDotNet/Services/Posts/PostService.cs`, replace the existing `GetPublishedAsync` method with:

```csharp
    public Task<List<Post>> GetPublishedAsync() => GetPublishedAsync(null, null);

    public async Task<List<Post>> GetPublishedAsync(PostKind? kind, string? project)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var query = db.Posts.AsNoTracking().Where(p => p.PublishedAt != null);

        if (kind is not null)
            query = query.Where(p => p.Kind == kind);

        if (!string.IsNullOrWhiteSpace(project))
            query = query.Where(p => p.Project == project);

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<List<string>> GetProjectsAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Posts.AsNoTracking()
            .Where(p => p.PublishedAt != null && p.Project != null && p.Project != "")
            .Select(p => p.Project!)
            .Distinct()
            .OrderBy(p => p)
            .ToListAsync();
    }
```

- [ ] **Step 4: Run the tests to verify they pass**

Run: `dotnet test`
Expected: PASS — all tests including the pre-existing suite

- [ ] **Step 5: Commit**

```bash
git add src/MeDotNet/Services/Posts/PostService.cs tests/MeDotNet.Tests/Services/Posts/PostServiceTests.cs
git commit -m "feat: filter published posts by kind and project"
```

---

### Task 4: The Record pages

**Files:**
- Create: `src/MeDotNet/Components/RecordEntryRow.razor`
- Create: `src/MeDotNet/Components/Pages/Record.razor`
- Create: `src/MeDotNet/Components/Pages/RecordEntry.razor`
- Delete: `src/MeDotNet/Components/Pages/Posts.razor`, `src/MeDotNet/Components/Pages/PostDetail.razor`
- Modify: `src/MeDotNet/Components/Layout/MainLayout.razor`, `src/MeDotNet/Program.cs`

**Interfaces:**
- Consumes: `PostService.GetPublishedAsync(PostKind?, string?)`, `PostService.GetProjectsAsync()`, `PostService.GetBySlugAsync(string)` (Task 3); `PostKind.Label()`, `PostKind.RailColor()`, `PostKindDisplay.FilterName`, `PostKindDisplay.TryParseFilter` (Task 2)
- Produces: `RecordEntryRow` component with parameters `[Parameter, EditorRequired] public Post Entry { get; set; }` and `[Parameter] public bool ShowSummary { get; set; } = true` — reused by Home in Task 5

- [ ] **Step 1: Create the shared entry row**

Create `src/MeDotNet/Components/RecordEntryRow.razor` (directly in `Components/`, **not** a `Shared/` subfolder — `_Imports.razor` imports `MeDotNet.Components` only, so a subfolder would need an extra `@using`):

```razor
@using MeDotNet.Models

<div style="border-left: 3px solid @Entry.Kind.RailColor(); padding: 2px 0 2px 16px;" class="mb-6">
    <div class="d-flex align-center gap-3 mb-1">
        <MudChip T="string" Size="Size.Small" Style="@($"background-color:{Entry.Kind.RailColor()};color:#fff;font-weight:700;letter-spacing:.08em;")">
            @Entry.Kind.Label()
        </MudChip>
        <MudText Typo="Typo.caption">
            @(Entry.Project is not null ? $"{Entry.Project} · " : "")@Entry.PublishedAt!.Value.ToString("d MMMM yyyy")
        </MudText>
    </div>
    <MudText Typo="Typo.h6" Class="mb-1">
        <MudLink Href="@($"/notes/{Entry.Slug}")" Typo="Typo.h6">@Entry.Title</MudLink>
    </MudText>
    @if (ShowSummary && !string.IsNullOrWhiteSpace(Entry.Summary))
    {
        <MudText Typo="Typo.body2" Class="mud-text-secondary">@Entry.Summary</MudText>
    }
</div>

@code {
    [Parameter, EditorRequired] public Post Entry { get; set; } = default!;
    [Parameter] public bool ShowSummary { get; set; } = true;
}
```

- [ ] **Step 2: Create the Record page**

Create `src/MeDotNet/Components/Pages/Record.razor`:

```razor
@page "/notes"
@using MeDotNet.Models
@inject PostService PostService
@inject NavigationManager Nav
@inject PersistentComponentState ApplicationState
@implements IDisposable

<PageTitle>The Record — JasonObject</PageTitle>

<h1 class="mud-typography mud-typography-h3 mud-typography-gutterbottom">The Record</h1>
<MudText Typo="Typo.body1" Class="mb-6">
    Specs, plans, post-mortems and notes from real builds — including the parts that went wrong.
</MudText>

<div class="d-flex flex-wrap gap-2 mb-2">
    <MudChip T="string" Variant="@(_kind is null ? Variant.Filled : Variant.Outlined)"
             Color="Color.Primary" OnClick="@(() => ApplyFilter(null, _project))">All</MudChip>
    @foreach (var kind in _allKinds)
    {
        var captured = kind;
        <MudChip T="string" Variant="@(_kind == captured ? Variant.Filled : Variant.Outlined)"
                 Color="Color.Primary" OnClick="@(() => ApplyFilter(captured, _project))">
            @captured.Label()
        </MudChip>
    }
</div>

@if (_projects is { Count: > 0 })
{
    <div class="d-flex flex-wrap gap-2 mb-6">
        @foreach (var project in _projects)
        {
            var captured = project;
            <MudChip T="string" Size="Size.Small"
                     Variant="@(_project == captured ? Variant.Filled : Variant.Outlined)"
                     Color="Color.Default" OnClick="@(() => ApplyFilter(_kind, _project == captured ? null : captured))">
                @captured
            </MudChip>
        }
    </div>
}

@if (_entries is null)
{
    <MudProgressCircular Indeterminate="true" />
}
else if (_entries.Count == 0)
{
    <MudText Class="my-8">
        Nothing here with those filters yet.
        <MudLink Href="/notes">Clear the filters</MudLink> to see everything in the record.
    </MudText>
}
else
{
    @foreach (var entry in _entries)
    {
        <RecordEntryRow Entry="entry" />
    }
}

@code {
    [SupplyParameterFromQuery(Name = "kind")] public string? KindFilter { get; set; }
    [SupplyParameterFromQuery(Name = "project")] public string? ProjectFilter { get; set; }

    private static readonly PostKind[] _allKinds =
        [PostKind.Spec, PostKind.Plan, PostKind.PostMortem, PostKind.Note];

    private List<Post>? _entries;
    private List<string>? _projects;
    private PostKind? _kind;
    private string? _project;
    private PersistentLoader? _loader;

    protected override async Task OnInitializedAsync()
    {
        _loader = new PersistentLoader(ApplicationState);
        _projects = await _loader.LoadAsync("record-projects", () => PostService.GetProjectsAsync());
    }

    protected override async Task OnParametersSetAsync()
    {
        _kind = PostKindDisplay.TryParseFilter(KindFilter, out var parsed) ? parsed : null;
        _project = string.IsNullOrWhiteSpace(ProjectFilter) ? null : ProjectFilter;
        _entries = await PostService.GetPublishedAsync(_kind, _project);
    }

    private void ApplyFilter(PostKind? kind, string? project)
    {
        var query = new List<string>();
        if (kind is not null) query.Add($"kind={PostKindDisplay.FilterName(kind.Value)}");
        if (!string.IsNullOrWhiteSpace(project)) query.Add($"project={Uri.EscapeDataString(project)}");
        Nav.NavigateTo(query.Count == 0 ? "/notes" : $"/notes?{string.Join("&", query)}");
    }

    public void Dispose() => _loader?.Dispose();
}
```

Note: `_entries` is loaded in `OnParametersSetAsync` rather than through `PersistentLoader`, because it must re-query whenever the query string changes. Only the project chip list, which is filter-independent, uses the loader.

- [ ] **Step 3: Create the single-entry page**

Create `src/MeDotNet/Components/Pages/RecordEntry.razor`:

```razor
@page "/notes/{Slug}"
@using MeDotNet.Models
@inject PostService PostService
@inject PersistentComponentState ApplicationState
@implements IDisposable

<PageTitle>@($"{_post?.Title ?? "Not Found"} — JasonObject")</PageTitle>

@if (_post is null)
{
    <h1 class="mud-typography mud-typography-h4">Entry not found.</h1>
    <MudButton Href="/notes" Variant="Variant.Text" StartIcon="@Icons.Material.Filled.ArrowBack" Class="mt-4">
        Back to the record
    </MudButton>
}
else
{
    <div class="d-flex align-center gap-3 mb-2">
        <MudChip T="string" Size="Size.Small" Style="@($"background-color:{_post.Kind.RailColor()};color:#fff;font-weight:700;letter-spacing:.08em;")">
            @_post.Kind.Label()
        </MudChip>
        <MudText Typo="Typo.caption">
            @(_post.Project is not null ? $"{_post.Project} · " : "")@_post.PublishedAt!.Value.ToString("d MMMM yyyy")
        </MudText>
    </div>
    <h1 class="mud-typography mud-typography-h3 mud-typography-gutterbottom">@_post.Title</h1>
    @if (!string.IsNullOrWhiteSpace(_post.Summary))
    {
        <MudText Typo="Typo.h6" Class="mud-text-secondary mb-6">@_post.Summary</MudText>
    }
    <MudText Typo="Typo.body1" Style="white-space: pre-wrap">@_post.Body</MudText>
}

@code {
    [Parameter] public string Slug { get; set; } = "";
    [CascadingParameter] HttpContext? HttpContext { get; set; }
    private Post? _post;
    private PersistentLoader? _loader;

    protected override async Task OnInitializedAsync()
    {
        _loader = new PersistentLoader(ApplicationState);
        _post = await _loader.LoadAsync($"note-{Slug}", () => PostService.GetBySlugAsync(Slug));
        if (_post is null && HttpContext is not null)
            HttpContext.Response.StatusCode = 404;
    }

    public void Dispose() => _loader?.Dispose();
}
```

- [ ] **Step 4: Delete the old blog pages**

```bash
git rm src/MeDotNet/Components/Pages/Posts.razor src/MeDotNet/Components/Pages/PostDetail.razor
```

- [ ] **Step 5: Add redirect endpoints**

In `src/MeDotNet/Program.cs`, immediately **before** the `app.MapRazorPages();` line, add:

```csharp
// Legacy blog routes — preserved so existing links survive the rename to /notes
app.MapGet("/posts", () => Results.Redirect("/notes", permanent: true));
app.MapGet("/posts/{slug}", (string slug) => Results.Redirect($"/notes/{slug}", permanent: true));
```

- [ ] **Step 6: Update the nav**

In `src/MeDotNet/Components/Layout/MainLayout.razor`, replace this line:

```razor
            <MudNavLink Href="posts" Icon="@Icons.Material.Filled.Article">Blog</MudNavLink>
```

with:

```razor
            <MudNavLink Href="notes" Icon="@Icons.Material.Filled.Article">The Record</MudNavLink>
```

Then reorder the drawer links so the order is Home, The Record, Projects, About — move the `notes` link directly after the `Home` link.

- [ ] **Step 7: Build**

Run: `dotnet build`
Expected: SUCCESS, no warnings about unresolved components

- [ ] **Step 8: Run the full test suite**

Run: `dotnet test`
Expected: PASS

- [ ] **Step 9: Start the app**

Run: `docker compose up -d --build`
Wait for the container to become healthy: `docker compose logs -f app` until you see the "Now listening on" line, then Ctrl-C out of the log tail.

- [ ] **Step 10: Verify the record page renders**

Run: `curl -s localhost:5000/notes | grep -c "The Record"`
Expected: a count of 1 or more

- [ ] **Step 11: Verify filtering is linkable**

Run: `curl -s "localhost:5000/notes?kind=postmortem" | grep -qi "post-mortem\|Nothing here" && echo OK`
Expected: `OK` — the page renders either matching entries or the empty-filter message, not an error

- [ ] **Step 12: Verify the legacy redirects (manual, see "A note on test coverage")**

```bash
curl -s -o /dev/null -w '%{http_code} %{redirect_url}\n' localhost:5000/posts
curl -s -o /dev/null -w '%{http_code} %{redirect_url}\n' localhost:5000/posts/some-slug
```

Expected:
```
301 http://localhost:5000/notes
301 http://localhost:5000/notes/some-slug
```

- [ ] **Step 13: Commit**

```bash
git add -A src/MeDotNet
git commit -m "feat: replace blog with The Record, filterable by kind and project"
```

---

### Task 5: Home rewrite (layout C)

**Files:**
- Modify: `src/MeDotNet/Components/Pages/Home.razor` (full rewrite)

**Interfaces:**
- Consumes: `RecordEntryRow` (Task 4), `PostService.GetPublishedAsync()` (Task 3)
- Produces: nothing consumed by later tasks

The hero copy below is **verbatim** — implement it exactly.

- [ ] **Step 1: Rewrite the page**

Replace the entire contents of `src/MeDotNet/Components/Pages/Home.razor`:

```razor
@page "/"
@using MeDotNet.Models
@inject PostService PostService
@inject PersistentComponentState ApplicationState
@implements IDisposable

<PageTitle>JasonObject — Jason St. John</PageTitle>

<MudText Typo="Typo.subtitle1" Class="mud-text-secondary mb-1">
    Jason St. John — lead full-stack developer, Upstate New York
</MudText>

<h1 class="mud-typography mud-typography-h3 mud-typography-gutterbottom" style="max-width: 22ch">
    I sit between the people paying for software and the machines writing it.
</h1>

<MudText Typo="Typo.body1" Class="mb-10" Style="max-width: 68ch">
    Eight years architecting production .NET and Vue/Nuxt platforms, and a decade of teaching
    before that. Directing AI agents is an instruction problem: decomposition, scaffolding,
    feedback, and telling the difference between a model that misunderstood and one that simply
    wasn't given enough. Below is the record of doing it.
</MudText>

<MudText Typo="Typo.overline" Class="mud-text-secondary d-block mb-4">Latest from the record</MudText>

<MudGrid Spacing="6">
    <MudItem xs="12" md="8">
        @if (_recent is null)
        {
            <MudProgressCircular Indeterminate="true" />
        }
        else if (_recent.Count == 0)
        {
            <MudText>The record starts soon.</MudText>
        }
        else
        {
            @foreach (var entry in _recent)
            {
                <RecordEntryRow Entry="entry" />
            }
            <MudButton Href="/notes" Variant="Variant.Text" EndIcon="@Icons.Material.Filled.ArrowForward">
                Read the whole record
            </MudButton>
        }
    </MudItem>

    <MudItem xs="12" md="4">
        <MudPaper Class="pa-4" Elevation="1">
            <MudImage Src="/img/jason-st-john.jpg" Alt="Jason St. John" Fluid="true" Class="rounded mb-3" />
            <MudText Typo="Typo.body2" Class="mb-3">
                Open to lead and staff engineering roles — and taking on contract work now.
            </MudText>
            <MudButton Href="/work" Variant="Variant.Filled" Color="Color.Primary" FullWidth="true">
                Work with me
            </MudButton>
        </MudPaper>
    </MudItem>
</MudGrid>

@code {
    private List<Post>? _recent;
    private PersistentLoader? _loader;

    protected override async Task OnInitializedAsync()
    {
        _loader = new PersistentLoader(ApplicationState);
        _recent = await _loader.LoadAsync("home-recent",
            async () => (await PostService.GetPublishedAsync()).Take(3).ToList());
    }

    public void Dispose() => _loader?.Dispose();
}
```

The "This site is the demo" callout is deliberately gone — its argument is now made by the record itself. The `/img/jason-st-john.jpg` asset is produced in Task 8; until then the image renders broken, which is expected.

- [ ] **Step 2: Build**

Run: `dotnet build`
Expected: SUCCESS

- [ ] **Step 3: Run the test suite**

Run: `dotnet test`
Expected: PASS

- [ ] **Step 4: Verify the hero renders**

Run: `docker compose up -d --build && sleep 15 && curl -s localhost:5000/ | grep -c "machines writing it"`
Expected: 1 or more

- [ ] **Step 5: Commit**

```bash
git add src/MeDotNet/Components/Pages/Home.razor
git commit -m "feat: rewrite Home around the record and both asks"
```

---

### Task 6: The Work page and nav button

**Files:**
- Create: `src/MeDotNet/Components/Pages/Work.razor`
- Modify: `src/MeDotNet/Components/Layout/MainLayout.razor`

**Interfaces:**
- Consumes: nothing
- Produces: route `/work`

All three blocks of copy are **verbatim**. No pricing, no engagement-shape list.

- [ ] **Step 1: Create the page**

Create `src/MeDotNet/Components/Pages/Work.razor`:

```razor
@page "/work"

<PageTitle>Work with me — JasonObject</PageTitle>

<h1 class="mud-typography mud-typography-h3 mud-typography-gutterbottom">Work with me</h1>

<MudPaper Class="pa-6 mb-4" Elevation="1">
    <h2 class="mud-typography mud-typography-h5 mud-typography-gutterbottom">Full-time</h2>
    <MudText Typo="Typo.body1">
        Open to lead and staff engineering roles — .NET, Vue/Nuxt, platform architecture.
        Remote, or on-site around Upstate New York.
    </MudText>
</MudPaper>

<MudPaper Class="pa-6 mb-4" Elevation="1">
    <h2 class="mud-typography mud-typography-h5 mud-typography-gutterbottom">Contract</h2>
    <MudText Typo="Typo.body1">
        Available now. Small-business sites and CMS work, .NET platform and integration work,
        and full builds where directing agents is how the work gets done.
    </MudText>
</MudPaper>

<MudPaper Class="pa-6 mb-8" Elevation="1">
    <h2 class="mud-typography mud-typography-h5 mud-typography-gutterbottom">The thing I'm most interested in</h2>
    <MudText Typo="Typo.body1">
        If your team is working out how to use AI agents well — not the tooling, the practice —
        I'd like to talk. It's the work I care most about, and the record on this site is me
        building the case in public rather than claiming a track record I haven't earned yet.
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

- [ ] **Step 2: Add the app bar button**

In `src/MeDotNet/Components/Layout/MainLayout.razor`, directly after the `<MudSpacer />` line in the app bar, add:

```razor
        <MudButton Href="/work" Variant="Variant.Outlined" Color="Color.Inherit" Class="mr-3">
            Work with me
        </MudButton>
```

- [ ] **Step 3: Build**

Run: `dotnet build`
Expected: SUCCESS

- [ ] **Step 4: Verify the page renders and the third block is present**

Run: `docker compose up -d --build && sleep 15 && curl -s localhost:5000/work | grep -c "not the tooling, the practice"`
Expected: 1

- [ ] **Step 5: Run the test suite**

Run: `dotnet test`
Expected: PASS

- [ ] **Step 6: Commit**

```bash
git add src/MeDotNet/Components/Pages/Work.razor src/MeDotNet/Components/Layout/MainLayout.razor
git commit -m "feat: add /work page carrying both the full-time and contract asks"
```

---

### Task 7: Admin editor gains Kind, Project and Summary

**Files:**
- Modify: `src/MeDotNet/Components/Pages/Admin/PostEdit.razor`
- Modify: `src/MeDotNet/Components/Pages/Admin/Posts.razor`

**Interfaces:**
- Consumes: `PostKind`, `PostKind.Label()` (Tasks 1–2)
- Produces: nothing consumed by later tasks

- [ ] **Step 1: Add the inputs**

In `src/MeDotNet/Components/Pages/Admin/PostEdit.razor`, add `@using MeDotNet.Models` below the existing `@using` line. Then, directly **after** the Slug `MudTextField` and **before** the Body field, add:

```razor
<MudSelect T="PostKind" Label="Kind" @bind-Value="_kind" Variant="Variant.Outlined" Class="mb-4">
    <MudSelectItem T="PostKind" Value="PostKind.Note">Note</MudSelectItem>
    <MudSelectItem T="PostKind" Value="PostKind.Spec">Spec</MudSelectItem>
    <MudSelectItem T="PostKind" Value="PostKind.Plan">Plan</MudSelectItem>
    <MudSelectItem T="PostKind" Value="PostKind.PostMortem">Post-mortem</MudSelectItem>
</MudSelect>
<MudTextField T="string" Label="Project" @bind-Value="_project"
              HelperText="Optional. e.g. JasonObject, ClaudePress, Whindancer"
              Variant="Variant.Outlined" Class="mb-4" />
<MudTextField T="string" Label="Summary" @bind-Value="_summary"
              HelperText="One line shown under the title in the record."
              Variant="Variant.Outlined" Class="mb-4" />
```

- [ ] **Step 2: Add the backing fields**

In the same file's `@code` block, add after `private bool _published;`:

```csharp
    private PostKind _kind = PostKind.Note;
    private string _project = "";
    private string _summary = "";
```

- [ ] **Step 3: Load existing values**

In `OnInitializedAsync`, inside the `if (_existing is not null)` block, add after `_published = _existing.PublishedAt.HasValue;`:

```csharp
                _kind = _existing.Kind;
                _project = _existing.Project ?? "";
                _summary = _existing.Summary ?? "";
```

- [ ] **Step 4: Persist on save**

In `SaveAsync`, in the `_isNew` branch, add these three lines to the `new Post { ... }` initializer (after `Body = _body,`):

```csharp
                    Kind = _kind,
                    Project = string.IsNullOrWhiteSpace(_project) ? null : _project.Trim(),
                    Summary = string.IsNullOrWhiteSpace(_summary) ? null : _summary.Trim(),
```

And in the `else if (_existing is not null)` branch, add after `_existing.Body = _body;`:

```csharp
                _existing.Kind = _kind;
                _existing.Project = string.IsNullOrWhiteSpace(_project) ? null : _project.Trim();
                _existing.Summary = string.IsNullOrWhiteSpace(_summary) ? null : _summary.Trim();
```

- [ ] **Step 5: Add the Kind column to the admin table**

In `src/MeDotNet/Components/Pages/Admin/Posts.razor`, add `@using MeDotNet.Models` at the top. Add a header cell after `<MudTh>Slug</MudTh>`:

```razor
            <MudTh>Kind</MudTh>
```

And a matching row cell after the Slug `MudTd`:

```razor
            <MudTd DataLabel="Kind">@context.Kind.Label()</MudTd>
```

- [ ] **Step 6: Build**

Run: `dotnet build`
Expected: SUCCESS

- [ ] **Step 7: Verify round-tripping by hand**

Start the app (`docker compose up -d --build`), log in at `/account/login`, create a post with Kind = Post-mortem, Project = `JasonObject`, Summary = `Test summary`, published. Save, reopen it from `/admin/posts`, and confirm all three fields came back populated. Then load `/notes` and confirm the entry renders with an orange rail and a `POST-MORTEM` chip. Delete the test post when done.

- [ ] **Step 8: Run the test suite**

Run: `dotnet test`
Expected: PASS

- [ ] **Step 9: Commit**

```bash
git add src/MeDotNet/Components/Pages/Admin
git commit -m "feat: edit kind, project and summary in the admin editor"
```

---

### Task 8: About and Projects

**Files:**
- Modify: `src/MeDotNet/Components/Pages/About.razor`
- Modify: `src/MeDotNet/Components/Pages/Projects.razor`

**Interfaces:**
- Consumes: nothing
- Produces: nothing consumed by later tasks

Images referenced here are produced in Task 9; they render broken until then.

- [ ] **Step 1: Add the C-suite paragraph to About**

In `src/MeDotNet/Components/Pages/About.razor`, inside the "The thesis" section, append this paragraph after the existing thesis text and before the `Get in touch` heading:

```razor
<MudText Typo="Typo.body1" Class="mb-4">
    I've done a version of this in a room full of executives. At my last employer I was the one
    briefing the C-suite on what AI could and couldn't do for us — neither overselling it nor
    burying them in detail. When the company later brought in a dedicated specialist, the raw
    technical depth went up and the translation went down. Sitting between the people making
    the decisions and the systems doing the work is a distinct skill, and it's the one I keep
    being asked for.
</MudText>
```

**Constraint check:** this text describes the role played. It must not claim an outcome — do not add any version of "and nobody was laid off."

- [ ] **Step 2: Add the portrait to About**

Directly below the `<h1>About</h1>` line in the same file, add:

```razor
<MudImage Src="/img/jason-cabin.jpg" Alt="Jason St. John" Fluid="true" Class="rounded mb-6"
          Style="max-width: 420px" />
```

- [ ] **Step 3: Confirm the ordering — no change required**

The spec calls for AI-directed builds to lead. Verified against the current array: the nine entries are `jasonobject.work — this site`, Job Management Platform 2.0, Automation scripting engine, Ballot production & tracking, Client Portal 2.0, Multi-workflow job system, Reactive invoicing system, CateredTo, AGFTC website. `jasonobject.work` is already first, and **there is no ClaudePress or Whindancer entry on this page at all**.

So there is nothing to reorder. Make no change in this step.

**Open item for Jason, not for the implementer:** the ClaudePress and Whindancer client sites are live freelance work that directly supports the contract pitch, and they are missing from Projects entirely. Adding them needs source facts he supplies — do not invent write-ups. Flag this and move on.

- [ ] **Step 4: Add the record link field**

In `Projects.razor`, change the `ProjectEntry` record declaration to:

```csharp
    private record ProjectEntry(string Title, string Meta, string Body, string? LinkText, string? LinkHref, bool Employer, string? RecordProject = null);
```

Then set `RecordProject: "JasonObject"` on the `jasonobject.work — this site` entry by appending it as a named argument to that entry's constructor call.

- [ ] **Step 5: Render the record link**

In the card markup, inside the `<MudCardActions>` block (add the block if the card has none for entries without `LinkHref`), add:

```razor
            @if (project.RecordProject is not null)
            {
                <MudButton Href="@($"/notes?project={project.RecordProject}")" Variant="Variant.Text"
                           Color="Color.Primary" EndIcon="@Icons.Material.Filled.ArrowForward">
                    Read the record
                </MudButton>
            }
```

Make sure the `@if (project.LinkHref is not null)` guard around `MudCardActions` is widened to `@if (project.LinkHref is not null || project.RecordProject is not null)` so the actions block renders for record-only entries.

- [ ] **Step 6: Update the Projects intro line**

Replace the intro `MudText` body with:

```
    Newest and most AI-directed first. The employer-built systems can't be linked, but they're
    the bulk of the story — ask me about any of them.
```

- [ ] **Step 7: Build**

Run: `dotnet build`
Expected: SUCCESS

- [ ] **Step 8: Verify**

Run: `docker compose up -d --build && sleep 15 && curl -s localhost:5000/about | grep -c "briefing the C-suite"`
Expected: 1

Run: `curl -s localhost:5000/projects | grep -c "Read the record"`
Expected: 1 or more

- [ ] **Step 9: Run the test suite**

Run: `dotnet test`
Expected: PASS

- [ ] **Step 10: Commit**

```bash
git add src/MeDotNet/Components/Pages/About.razor src/MeDotNet/Components/Pages/Projects.razor
git commit -m "feat: add C-suite beat to About, link Projects into the record"
```

---

### Task 9: Ship the images

**Files:**
- Create: `src/MeDotNet/wwwroot/img/jason-st-john.jpg`, `src/MeDotNet/wwwroot/img/jason-cabin.jpg`, `src/MeDotNet/wwwroot/img/adirondacks.jpg`
- Modify: `src/MeDotNet/Components/App.razor`, `src/MeDotNet/Components/Pages/About.razor`

**Interfaces:**
- Consumes: the `/img/*` paths referenced in Tasks 5 and 8
- Produces: nothing consumed by later tasks

Requires ImageMagick (see Prerequisites). Source files are in `ImagePool/`, which is gitignored — only the derived files under `wwwroot/img/` are committed.

- [ ] **Step 1: Verify tooling**

Run: `convert -version`
Expected: prints an ImageMagick version. If "command not found", stop and run `sudo apt install imagemagick`.

- [ ] **Step 2: Produce the primary headshot**

```bash
mkdir -p src/MeDotNet/wwwroot/img
convert ImagePool/Headshot1crop.jpg -resize 800x -quality 82 -strip \
  src/MeDotNet/wwwroot/img/jason-st-john.jpg
```

- [ ] **Step 3: Produce the About portrait**

```bash
convert ImagePool/Headshot2crop.jpg -resize 900x -quality 82 -strip \
  src/MeDotNet/wwwroot/img/jason-cabin.jpg
```

- [ ] **Step 4: Produce the banner, rotating destructively**

The source is a 4032×3024 iPhone photo with EXIF orientation `upper-right`. `-auto-orient` bakes the rotation into the pixels; `-strip` then removes the EXIF so nothing double-rotates it later.

```bash
convert "ImagePool/IMG_20260707_162952 (2).jpg" -auto-orient -resize 1600x \
  -gravity center -crop 1600x600+0+0 +repage -quality 82 -strip \
  src/MeDotNet/wwwroot/img/adirondacks.jpg
```

- [ ] **Step 5: Verify dimensions and file sizes**

```bash
identify src/MeDotNet/wwwroot/img/*.jpg
du -h src/MeDotNet/wwwroot/img/*.jpg
```

Expected: `jason-st-john.jpg` 800px wide, `jason-cabin.jpg` 900px wide, `adirondacks.jpg` exactly 1600×600 and **landscape, not sideways**. Every file must be **under 250 KB**. If `adirondacks.jpg` is portrait-shaped, the auto-orient did not take — re-run Step 4 with an explicit `-rotate 90` in place of `-auto-orient`.

- [ ] **Step 6: Open the banner and confirm it is right way up**

View `src/MeDotNet/wwwroot/img/adirondacks.jpg` directly. A sideways banner passes every automated check and still looks broken — this step is a human eyeball, not a command.

- [ ] **Step 7: Add the banner to About**

In `src/MeDotNet/Components/Pages/About.razor`, add directly above the `Get in touch` heading:

```razor
<MudImage Src="/img/adirondacks.jpg" Alt="A foggy morning in the Adirondacks" Fluid="true"
          Class="rounded my-8" />
```

- [ ] **Step 8: Add the og:image**

In `src/MeDotNet/Components/App.razor`, inside `<head>`, add after the existing `<meta>` tags:

```html
    <meta property="og:image" content="/img/jason-st-john.jpg" />
    <meta property="og:title" content="Jason St. John — JasonObject" />
    <meta property="og:description" content="A working record of directing AI agents to build production software." />
    <meta property="og:type" content="website" />
```

- [ ] **Step 9: Verify the images serve**

```bash
docker compose up -d --build && sleep 15
curl -s -o /dev/null -w '%{http_code} %{content_type} %{size_download}\n' localhost:5000/img/jason-st-john.jpg
```

Expected: `200 image/jpeg` and a size under 250000

- [ ] **Step 10: Run the test suite**

Run: `dotnet test`
Expected: PASS

- [ ] **Step 11: Commit**

```bash
git add src/MeDotNet/wwwroot/img src/MeDotNet/Components/App.razor src/MeDotNet/Components/Pages/About.razor
git commit -m "feat: ship resized site imagery and social card metadata"
```

---

### Task 10: Publish the first batch of record entries

**Files:** none — this is a content task performed through the admin UI at `/admin/posts`.

**Interfaces:**
- Consumes: the admin editor from Task 7

This task is **not** automatable and must not be scripted against the database. Each entry passes a redaction gate, and Jason approves each one before it publishes.

- [ ] **Step 1: Read each source document in full**

| Source | Kind | Project |
|---|---|---|
| `docs/superpowers/specs/2026-07-07-mudblazor-design.md` | Spec | JasonObject |
| `docs/superpowers/specs/2026-07-07-site-content-design.md` | Spec | JasonObject |
| `docs/superpowers/plans/2026-07-05-phase2-cms.md` | Plan | JasonObject |
| `/mnt/external/medotnet-review-post-mortem.md` | PostMortem | JasonObject |

- [ ] **Step 2: Run the redaction gate on each**

For each document, search for and remove or genericise: the previous employer's name, names of their internal systems, and any internal decisions or personnel details. Note that `2026-07-07-site-content-design.md` contains the full text of the site's own copy, which is already public — that needs no redaction.

- [ ] **Step 3: Write a one-line summary for each**

The summary is what makes an artifact legible to someone who won't open it. Draft one per entry and get Jason's approval on the wording before publishing.

- [ ] **Step 4: Create each entry in the admin UI**

For each: Title, Slug, Kind, Project, Summary, Body (the redacted markdown), Published = on.

- [ ] **Step 5: Write the fresh note**

Title: `Reviewing every task before it lands`. Kind: Note. Project: leave empty. Draw on the eleven task briefs and reports in `.superpowers/sdd/` for concrete detail about where agent output needed correction.

- [ ] **Step 6: Verify the record**

Load `/notes` and confirm: five entries, correct rail colours, the project filter shows `JasonObject`, and `/notes?kind=postmortem` returns exactly one entry.

- [ ] **Step 7: Verify Home**

Load `/` and confirm the three most recent entries appear in the left column with summaries.

---

## Definition of done

- [ ] `dotnet test` green
- [ ] `/`, `/notes`, `/notes/{slug}`, `/projects`, `/about`, `/work` all render
- [ ] `/posts` and `/posts/{slug}` return 301 to their `/notes` equivalents
- [ ] Nav reads Home · The Record · Projects · About, with a Work with me button in the app bar
- [ ] No page contains the words "Blog" or "This site is the demo"
- [ ] All three images under 250 KB, banner right way up
- [ ] Five entries live in the record
- [ ] `ImagePool/` is gitignored and no image originals are tracked
