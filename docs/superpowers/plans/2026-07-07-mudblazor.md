# Phase 3 — MudBlazor Adoption Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Adopt MudBlazor app-wide — Material shell with dark mode, Mud components on all pages, Bootstrap removed — plus three bundled auth/UX fixes (POST logout, login redirect, real Home page).

**Architecture:** Global InteractiveServer render mode with prerendering (SEO preserved via prerendered HTML). A `PersistentLoader` helper carries prerender query results into the circuit so each page view costs one DB query. Public and admin pages share one Mud shell.

**Tech Stack:** .NET 9, Blazor Web App (global InteractiveServer + prerender), MudBlazor 8.x, EF Core 9 + SQL Server, ASP.NET Core Identity

**Spec:** `docs/superpowers/specs/2026-07-07-mudblazor-design.md`

## Global Constraints

- Branch: `feature/mudblazor` (never commit to main)
- Target framework: `net9.0`
- `dotnet` is at `$HOME/.dotnet/dotnet`; always `export PATH="$HOME/.dotnet/tools:$HOME/.dotnet:$PATH"` and `export DOTNET_ROOT="$HOME/.dotnet"` before dotnet commands
- Run tests with: `dotnet test tests/MeDotNet.Tests/ --nologo --logger "console;verbosity=minimal"` — expect `Passed: 14` after every task
- Build with: `dotnet build src/MeDotNet/MeDotNet.csproj --nologo` — expect 0 warnings, 0 errors after every task
- Every task leaves the app building and tests green; one commit per task
- No bUnit/component tests this phase; `PostService` and all existing tests are unchanged

---

### Task 1: MudBlazor package, global InteractiveServer, Bootstrap removal

**Files:**
- Modify: `src/MeDotNet/MeDotNet.csproj` (via `dotnet add package`)
- Modify: `src/MeDotNet/Program.cs`
- Modify: `src/MeDotNet/Components/App.razor`
- Modify: `src/MeDotNet/Components/_Imports.razor`
- Modify: `src/MeDotNet/Components/Pages/Admin/Posts.razor` (remove rendermode line only)
- Modify: `src/MeDotNet/Components/Pages/Admin/PostEdit.razor` (remove rendermode line only)
- Modify: `src/MeDotNet/wwwroot/app.css` (replace contents)
- Delete: `src/MeDotNet/wwwroot/lib/bootstrap/` (entire directory)

**Interfaces:**
- Produces: MudBlazor services registered; app globally InteractiveServer with prerender; Bootstrap gone. Later tasks assume `@using MudBlazor` is in `_Imports.razor` and `AddMudServices()` is registered.

- [ ] **Step 1: Add the MudBlazor package**

```bash
export PATH="$HOME/.dotnet/tools:$HOME/.dotnet:$PATH" && export DOTNET_ROOT="$HOME/.dotnet"
dotnet add src/MeDotNet/ package MudBlazor
```

Expected: latest stable MudBlazor 8.x added to `MeDotNet.csproj`.

- [ ] **Step 2: Register MudBlazor services in Program.cs**

In `src/MeDotNet/Program.cs`, add to the using block at the top:

```csharp
using MudBlazor.Services;
```

Then directly after `builder.Services.AddRazorPages();`, add:

```csharp
builder.Services.AddMudServices();
```

- [ ] **Step 3: Update App.razor — Mud assets and global render mode**

Replace the entire contents of `src/MeDotNet/Components/App.razor` with:

```razor
<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet" />
    <link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="@Assets["app.css"]" />
    <link rel="stylesheet" href="@Assets["MeDotNet.styles.css"]" />
    <ImportMap />
    <link rel="icon" type="image/png" href="favicon.png" />
    <HeadOutlet @rendermode="InteractiveServer" />
</head>

<body>
    <Routes @rendermode="InteractiveServer" />
    <script src="_framework/blazor.web.js"></script>
    <script src="_content/MudBlazor/MudBlazor.min.js"></script>
</body>

</html>
```

- [ ] **Step 4: Add MudBlazor to _Imports.razor**

Append to `src/MeDotNet/Components/_Imports.razor`:

```razor
@using MudBlazor
```

- [ ] **Step 5: Remove now-redundant per-page render modes**

In `src/MeDotNet/Components/Pages/Admin/Posts.razor`, delete the line:
```razor
@rendermode InteractiveServer
```

In `src/MeDotNet/Components/Pages/Admin/PostEdit.razor`, delete the line:
```razor
@rendermode InteractiveServer
```

(Global render mode on `<Routes>` now covers them; leaving them in causes a runtime error about redundant render modes.)

- [ ] **Step 6: Delete Bootstrap and gut app.css**

```bash
rm -rf src/MeDotNet/wwwroot/lib/bootstrap
```

If `src/MeDotNet/wwwroot/lib/` is now empty, remove it too. Replace the entire contents of `src/MeDotNet/wwwroot/app.css` with:

```css
html, body {
    margin: 0;
}

#blazor-error-ui {
    color-scheme: light only;
    background: lightyellow;
    bottom: 0;
    box-shadow: 0 -1px 2px rgba(0, 0, 0, 0.2);
    box-sizing: border-box;
    display: none;
    left: 0;
    padding: 0.6rem 1.25rem 0.7rem 1.25rem;
    position: fixed;
    width: 100%;
    z-index: 1000;
}

#blazor-error-ui .dismiss {
    cursor: pointer;
    position: absolute;
    right: 0.75rem;
    top: 0.5rem;
}
```

- [ ] **Step 7: Build and test**

```bash
dotnet build src/MeDotNet/MeDotNet.csproj --nologo
dotnet test tests/MeDotNet.Tests/ --nologo --logger "console;verbosity=minimal"
```

Expected: build 0 warnings 0 errors; `Passed: 14`. (The app will look unstyled until Task 2 — that's expected.)

- [ ] **Step 8: Commit**

```bash
git add -A src/MeDotNet/
git commit -m "feat: add MudBlazor, switch to global InteractiveServer, remove Bootstrap"
```

---

### Task 2: Mud shell — MainLayout, drawer, theme; delete NavMenu

**Files:**
- Modify: `src/MeDotNet/Components/Layout/MainLayout.razor` (full rewrite)
- Delete: `src/MeDotNet/Components/Layout/MainLayout.razor.css`
- Delete: `src/MeDotNet/Components/Layout/NavMenu.razor`
- Delete: `src/MeDotNet/Components/Layout/NavMenu.razor.css`

**Interfaces:**
- Consumes: MudBlazor services/assets from Task 1.
- Produces: the shell every page renders inside. The drawer's Logout entry remains a GET link in this task (the existing `/account/logout` MapGet endpoint still serves it); Task 6 converts it to a POST form.

- [ ] **Step 1: Rewrite MainLayout.razor**

Replace the entire contents of `src/MeDotNet/Components/Layout/MainLayout.razor` with:

```razor
@inherits LayoutComponentBase

<MudThemeProvider @ref="_themeProvider" @bind-IsDarkMode="_isDarkMode" />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <MudAppBar Elevation="1">
        <MudIconButton Icon="@Icons.Material.Filled.Menu" Color="Color.Inherit" Edge="Edge.Start"
                       OnClick="ToggleDrawer" aria-label="Toggle navigation" />
        <MudText Typo="Typo.h6" Class="ml-2">MeDotNet</MudText>
        <MudSpacer />
        <MudIconButton Icon="@(_isDarkMode ? Icons.Material.Filled.LightMode : Icons.Material.Filled.DarkMode)"
                       Color="Color.Inherit" OnClick="ToggleDarkMode" aria-label="Toggle dark mode" />
    </MudAppBar>
    <MudDrawer @bind-Open="_drawerOpen" Elevation="1">
        <MudNavMenu>
            <MudNavLink Href="" Match="NavLinkMatch.All" Icon="@Icons.Material.Filled.Home">Home</MudNavLink>
            <MudNavLink Href="posts" Icon="@Icons.Material.Filled.Article">Blog</MudNavLink>
            <AuthorizeView>
                <Authorized>
                    <MudNavLink Href="admin/posts" Icon="@Icons.Material.Filled.EditNote">Manage Posts</MudNavLink>
                    <MudNavLink Href="account/logout" Icon="@Icons.Material.Filled.Logout">Logout</MudNavLink>
                </Authorized>
                <NotAuthorized>
                    <MudNavLink Href="account/login" Icon="@Icons.Material.Filled.Login">Login</MudNavLink>
                </NotAuthorized>
            </AuthorizeView>
        </MudNavMenu>
    </MudDrawer>
    <MudMainContent>
        <MudContainer MaxWidth="MaxWidth.Medium" Class="py-8">
            @Body
        </MudContainer>
    </MudMainContent>
</MudLayout>

<div id="blazor-error-ui" data-nosnippet>
    An unhandled error has occurred.
    <a href="." class="reload">Reload</a>
    <span class="dismiss">🗙</span>
</div>

@code {
    private MudThemeProvider _themeProvider = default!;
    private bool _isDarkMode;
    private bool _drawerOpen = true;

    private void ToggleDrawer() => _drawerOpen = !_drawerOpen;

    private void ToggleDarkMode() => _isDarkMode = !_isDarkMode;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _isDarkMode = await _themeProvider.GetSystemDarkModeAsync();
            await _themeProvider.WatchSystemDarkModeAsync(OnSystemDarkModeChanged);
            StateHasChanged();
        }
    }

    private Task OnSystemDarkModeChanged(bool newValue)
    {
        _isDarkMode = newValue;
        return InvokeAsync(StateHasChanged);
    }
}
```

Note: `GetSystemDarkModeAsync`/`WatchSystemDarkModeAsync` are the MudBlazor v7+ names. If the build reports these methods don't exist on `MudThemeProvider`, check the installed version's `MudThemeProvider` API (older versions call them `GetSystemPreference()`/`WatchSystemPreference()`); use whichever compiles — the semantics are identical.

- [ ] **Step 2: Delete the template layout leftovers**

```bash
git rm src/MeDotNet/Components/Layout/MainLayout.razor.css \
       src/MeDotNet/Components/Layout/NavMenu.razor \
       src/MeDotNet/Components/Layout/NavMenu.razor.css
```

- [ ] **Step 3: Build and test**

```bash
dotnet build src/MeDotNet/MeDotNet.csproj --nologo
dotnet test tests/MeDotNet.Tests/ --nologo --logger "console;verbosity=minimal"
```

Expected: 0 warnings 0 errors; `Passed: 14`.

- [ ] **Step 4: Smoke check (optional if Docker unavailable — note in report)**

```bash
docker compose up --build -d
```

Visit http://localhost:5000 — Mud app bar and drawer render; nav links work; dark-mode toggle flips the palette.

- [ ] **Step 5: Commit**

```bash
git add -A src/MeDotNet/Components/Layout/
git commit -m "feat: replace template layout with MudBlazor shell (appbar, drawer, dark mode)"
```

---

### Task 3: PersistentLoader — one query per page view

**Files:**
- Create: `src/MeDotNet/Components/PersistentLoader.cs`
- Modify: `src/MeDotNet/Components/Pages/Posts.razor` (@code block + injections only)
- Modify: `src/MeDotNet/Components/Pages/PostDetail.razor` (@code block + injections only)
- Modify: `src/MeDotNet/Components/Pages/Admin/Posts.razor` (@code block + injections only)
- Modify: `src/MeDotNet/Components/Pages/Admin/PostEdit.razor` (@code block + injections only)

**Interfaces:**
- Consumes: `PersistentComponentState` (framework), `PostService` methods.
- Produces: `PersistentLoader` with `Task<T> LoadAsync<T>(string key, Func<Task<T>> loader)` and `void Dispose()`. Tasks 4–5 keep these exact call patterns when rewriting page markup.

No unit tests for this helper: `PersistentComponentState` cannot be constructed in a plain xUnit test and the spec excludes bUnit this phase. Verification is behavioral (Step 7).

- [ ] **Step 1: Create PersistentLoader.cs**

Create `src/MeDotNet/Components/PersistentLoader.cs`:

```csharp
using Microsoft.AspNetCore.Components;

namespace MeDotNet.Components;

/// <summary>
/// Loads data once across the prerender and interactive passes of a component.
/// On prerender: runs the loader and registers the result for persistence.
/// On the interactive pass: takes the persisted value instead of re-querying.
/// A null loaded value is not persisted usefully, so not-found lookups query twice;
/// that only affects 404 paths.
/// </summary>
public sealed class PersistentLoader(PersistentComponentState state) : IDisposable
{
    private readonly List<PersistingComponentStateSubscription> _subscriptions = [];

    public async Task<T> LoadAsync<T>(string key, Func<Task<T>> loader)
    {
        if (state.TryTakeFromJson<T>(key, out var persisted) && persisted is not null)
            return persisted;

        var data = await loader();
        _subscriptions.Add(state.RegisterOnPersisting(() =>
        {
            state.PersistAsJson(key, data);
            return Task.CompletedTask;
        }));
        return data;
    }

    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
            subscription.Dispose();
    }
}
```

- [ ] **Step 2: Wire into public Posts.razor**

In `src/MeDotNet/Components/Pages/Posts.razor`: after the `@inject PostService PostService` line, add:

```razor
@inject PersistentComponentState ApplicationState
@implements IDisposable
```

Replace the `@code` block with:

```razor
@code {
    private List<Post>? _posts;
    private PersistentLoader? _loader;

    protected override async Task OnInitializedAsync()
    {
        _loader = new PersistentLoader(ApplicationState);
        _posts = await _loader.LoadAsync("public-posts", () => PostService.GetPublishedAsync());
    }

    private static string Excerpt(string body) =>
        body.Length <= 150 ? body : body[..150] + "…";

    public void Dispose() => _loader?.Dispose();
}
```

- [ ] **Step 3: Wire into PostDetail.razor**

In `src/MeDotNet/Components/Pages/PostDetail.razor`, after the `@inject PostService PostService` line, add:

```razor
@inject PersistentComponentState ApplicationState
@implements IDisposable
```

Replace the `@code` block with:

```razor
@code {
    [Parameter] public string Slug { get; set; } = "";
    [CascadingParameter] HttpContext? HttpContext { get; set; }
    private Post? _post;
    private PersistentLoader? _loader;

    protected override async Task OnInitializedAsync()
    {
        _loader = new PersistentLoader(ApplicationState);
        _post = await _loader.LoadAsync($"post-{Slug}", () => PostService.GetBySlugAsync(Slug));
        if (_post is null && HttpContext is not null)
            HttpContext.Response.StatusCode = 404;
    }

    public void Dispose() => _loader?.Dispose();
}
```

- [ ] **Step 4: Wire into Admin/Posts.razor**

In `src/MeDotNet/Components/Pages/Admin/Posts.razor`, after the existing `@inject` lines, add:

```razor
@inject PersistentComponentState ApplicationState
@implements IDisposable
```

In its `@code` block, add the `_loader` field, change `OnInitializedAsync`, and add `Dispose` as follows:

```csharp
    private PersistentLoader? _loader;

    protected override async Task OnInitializedAsync()
    {
        _loader = new PersistentLoader(ApplicationState);
        _posts = await _loader.LoadAsync("admin-posts", () => PostService.GetAllAsync());
    }

    public void Dispose() => _loader?.Dispose();
```

Leave `DeleteAsync` as-is (its post-delete refetch runs on the live circuit and should query directly).

- [ ] **Step 5: Wire into Admin/PostEdit.razor**

In `src/MeDotNet/Components/Pages/Admin/PostEdit.razor`, after the existing `@inject` lines, add:

```razor
@inject PersistentComponentState ApplicationState
@implements IDisposable
```

In `OnInitializedAsync`, replace the direct `GetByIdAsync` call so the method and new members read:

```csharp
    private PersistentLoader? _loader;

    protected override async Task OnInitializedAsync()
    {
        _loader = new PersistentLoader(ApplicationState);
        if (Id is not null)
        {
            _existing = await _loader.LoadAsync($"post-edit-{Id}", () => PostService.GetByIdAsync(Id.Value));
            if (_existing is not null)
            {
                _title = _existing.Title;
                _slug = _existing.Slug;
                _body = _existing.Body;
                _published = _existing.PublishedAt.HasValue;
                _slugManuallyEdited = true;
            }
            else
            {
                _error = "This post no longer exists.";
            }
        }
    }

    public void Dispose() => _loader?.Dispose();
```

(All other logic in the file — validation, save, slug generation — stays exactly as it is.)

- [ ] **Step 6: Build and test**

```bash
dotnet build src/MeDotNet/MeDotNet.csproj --nologo
dotnet test tests/MeDotNet.Tests/ --nologo --logger "console;verbosity=minimal"
```

Expected: 0 warnings 0 errors; `Passed: 14`.

- [ ] **Step 7: Behavioral check (optional if Docker unavailable — note in report)**

With `docker compose up --build -d` running and EF query logging visible (`docker compose logs -f app`), load `/posts` once: the SELECT for published posts must appear once per browser page load, not twice.

- [ ] **Step 8: Commit**

```bash
git add src/MeDotNet/Components/PersistentLoader.cs \
        src/MeDotNet/Components/Pages/Posts.razor \
        src/MeDotNet/Components/Pages/PostDetail.razor \
        src/MeDotNet/Components/Pages/Admin/Posts.razor \
        src/MeDotNet/Components/Pages/Admin/PostEdit.razor
git commit -m "feat: add PersistentLoader to dedupe prerender+circuit queries"
```

---

### Task 4: Public pages — Posts, PostDetail, new Home

**Files:**
- Modify: `src/MeDotNet/Components/Pages/Posts.razor` (markup only; keep Task 3's @code)
- Modify: `src/MeDotNet/Components/Pages/PostDetail.razor` (markup only; keep Task 3's @code)
- Modify: `src/MeDotNet/Components/Pages/Home.razor` (full rewrite)

**Interfaces:**
- Consumes: `PersistentLoader.LoadAsync` (Task 3), `PostService.GetPublishedAsync()`.
- Produces: no new interfaces.

- [ ] **Step 1: Rewrite Posts.razor markup**

Replace everything above the `@code` block in `src/MeDotNet/Components/Pages/Posts.razor` with:

```razor
@page "/posts"
@inject PostService PostService
@inject PersistentComponentState ApplicationState
@implements IDisposable

<PageTitle>Blog</PageTitle>
<MudText Typo="Typo.h3" GutterBottom="true">Blog</MudText>

@if (_posts is null)
{
    <MudProgressCircular Indeterminate="true" />
}
else if (_posts.Count == 0)
{
    <MudText>No posts yet.</MudText>
}
else
{
    @foreach (var post in _posts)
    {
        <MudCard Class="mb-6" Elevation="1">
            <MudCardContent>
                <MudLink Href="@($"/posts/{post.Slug}")" Typo="Typo.h5">@post.Title</MudLink>
                <MudText Typo="Typo.caption" Class="d-block mb-2">
                    @post.PublishedAt!.Value.ToString("MMMM d, yyyy")
                </MudText>
                <MudText Typo="Typo.body1">@Excerpt(post.Body)</MudText>
            </MudCardContent>
        </MudCard>
    }
}
```

The `@code` block from Task 3 stays byte-for-byte unchanged.

- [ ] **Step 2: Rewrite PostDetail.razor markup**

Replace everything above the `@code` block in `src/MeDotNet/Components/Pages/PostDetail.razor` with:

```razor
@page "/posts/{Slug}"
@inject PostService PostService
@inject PersistentComponentState ApplicationState
@implements IDisposable

<PageTitle>@(_post?.Title ?? "Not Found")</PageTitle>

@if (_post is null)
{
    <MudText Typo="Typo.h4">Post not found.</MudText>
    <MudButton Href="/posts" Variant="Variant.Text" StartIcon="@Icons.Material.Filled.ArrowBack" Class="mt-4">
        Back to blog
    </MudButton>
}
else
{
    <MudText Typo="Typo.h3" GutterBottom="true">@_post.Title</MudText>
    <MudText Typo="Typo.caption" Class="d-block mb-6">
        @_post.PublishedAt!.Value.ToString("MMMM d, yyyy")
    </MudText>
    <MudText Typo="Typo.body1" Style="white-space: pre-wrap">@_post.Body</MudText>
}
```

The `@code` block from Task 3 stays unchanged.

- [ ] **Step 3: Rewrite Home.razor as the landing page**

Replace the entire contents of `src/MeDotNet/Components/Pages/Home.razor` with:

```razor
@page "/"
@inject PostService PostService
@inject PersistentComponentState ApplicationState
@implements IDisposable

<PageTitle>MeDotNet</PageTitle>

<MudPaper Class="pa-6 mb-8" Elevation="1">
    <MudText Typo="Typo.h3" GutterBottom="true">MeDotNet</MudText>
    <MudText Typo="Typo.body1">Personal site and blog, built with Blazor.</MudText>
</MudPaper>

<MudText Typo="Typo.h5" GutterBottom="true">Recent posts</MudText>

@if (_recent is null)
{
    <MudProgressCircular Indeterminate="true" />
}
else if (_recent.Count == 0)
{
    <MudText>No posts yet.</MudText>
}
else
{
    @foreach (var post in _recent)
    {
        <MudCard Class="mb-4" Elevation="1">
            <MudCardContent>
                <MudLink Href="@($"/posts/{post.Slug}")" Typo="Typo.h6">@post.Title</MudLink>
                <MudText Typo="Typo.caption">@post.PublishedAt!.Value.ToString("MMMM d, yyyy")</MudText>
            </MudCardContent>
        </MudCard>
    }
    <MudButton Href="/posts" Variant="Variant.Text" EndIcon="@Icons.Material.Filled.ArrowForward" Class="mt-2">
        View all posts
    </MudButton>
}

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

- [ ] **Step 4: Build and test**

```bash
dotnet build src/MeDotNet/MeDotNet.csproj --nologo
dotnet test tests/MeDotNet.Tests/ --nologo --logger "console;verbosity=minimal"
```

Expected: 0 warnings 0 errors; `Passed: 14`.

- [ ] **Step 5: Commit**

```bash
git add src/MeDotNet/Components/Pages/Posts.razor \
        src/MeDotNet/Components/Pages/PostDetail.razor \
        src/MeDotNet/Components/Pages/Home.razor
git commit -m "feat: restyle public pages with MudBlazor, add real Home landing"
```

---

### Task 5: Admin pages — MudTable list, Mud form

**Files:**
- Modify: `src/MeDotNet/Components/Pages/Admin/Posts.razor` (full rewrite)
- Modify: `src/MeDotNet/Components/Pages/Admin/PostEdit.razor` (full rewrite)

**Interfaces:**
- Consumes: `PersistentLoader` (Task 3), `PostService` CRUD, `IDialogService`, `ISnackbar` (registered by `AddMudServices` in Task 1).
- Produces: no new interfaces. `IJSRuntime` injection is removed from Admin/Posts.razor.

- [ ] **Step 1: Rewrite Admin/Posts.razor**

Replace the entire contents of `src/MeDotNet/Components/Pages/Admin/Posts.razor` with:

```razor
@page "/admin/posts"
@attribute [Authorize]
@inject PostService PostService
@inject IDialogService DialogService
@inject ISnackbar Snackbar
@inject PersistentComponentState ApplicationState
@implements IDisposable

<PageTitle>Posts — Admin</PageTitle>

<div class="d-flex align-center mb-4">
    <MudText Typo="Typo.h4">Posts</MudText>
    <MudSpacer />
    <MudButton Href="/admin/posts/new" Variant="Variant.Filled" Color="Color.Primary"
               StartIcon="@Icons.Material.Filled.Add">New Post</MudButton>
</div>

@if (_posts is null)
{
    <MudProgressCircular Indeterminate="true" />
}
else if (_posts.Count == 0)
{
    <MudText>No posts yet.</MudText>
}
else
{
    <MudTable Items="_posts" Hover="true" Elevation="1">
        <HeaderContent>
            <MudTh>Title</MudTh>
            <MudTh>Slug</MudTh>
            <MudTh>Status</MudTh>
            <MudTh>Created</MudTh>
            <MudTh></MudTh>
        </HeaderContent>
        <RowTemplate>
            <MudTd DataLabel="Title">@context.Title</MudTd>
            <MudTd DataLabel="Slug">@context.Slug</MudTd>
            <MudTd DataLabel="Status">
                <MudChip T="string" Size="Size.Small"
                         Color="@(context.PublishedAt.HasValue ? Color.Success : Color.Default)">
                    @(context.PublishedAt.HasValue ? "Published" : "Draft")
                </MudChip>
            </MudTd>
            <MudTd DataLabel="Created">@context.CreatedAt.ToString("yyyy-MM-dd")</MudTd>
            <MudTd>
                <MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small"
                               Href="@($"/admin/posts/{context.Id}")" aria-label="Edit" />
                <MudIconButton Icon="@Icons.Material.Filled.Delete" Size="Size.Small" Color="Color.Error"
                               OnClick="() => DeleteAsync(context)" aria-label="Delete" />
            </MudTd>
        </RowTemplate>
    </MudTable>
}

@code {
    private List<Post>? _posts;
    private PersistentLoader? _loader;

    protected override async Task OnInitializedAsync()
    {
        _loader = new PersistentLoader(ApplicationState);
        _posts = await _loader.LoadAsync("admin-posts", () => PostService.GetAllAsync());
    }

    private async Task DeleteAsync(Post post)
    {
        var confirmed = await DialogService.ShowMessageBox(
            "Delete post",
            $"Delete \"{post.Title}\"? This cannot be undone.",
            yesText: "Delete", cancelText: "Cancel");

        if (confirmed == true)
        {
            await PostService.DeleteAsync(post.Id);
            _posts = await PostService.GetAllAsync();
            Snackbar.Add("Post deleted.", Severity.Success);
        }
    }

    public void Dispose() => _loader?.Dispose();
}
```

- [ ] **Step 2: Rewrite Admin/PostEdit.razor**

Replace the entire contents of `src/MeDotNet/Components/Pages/Admin/PostEdit.razor` with:

```razor
@page "/admin/posts/new"
@page "/admin/posts/{Id:int}"
@attribute [Authorize]
@using Microsoft.EntityFrameworkCore
@inject PostService PostService
@inject NavigationManager Nav
@inject AuthenticationStateProvider AuthStateProvider
@inject PersistentComponentState ApplicationState
@implements IDisposable

<PageTitle>@(_isNew ? "New Post" : "Edit Post") — Admin</PageTitle>
<MudText Typo="Typo.h4" GutterBottom="true">@(_isNew ? "New Post" : "Edit Post")</MudText>

@if (_error is not null)
{
    <MudAlert Severity="Severity.Error" Class="mb-4">@_error</MudAlert>
}

<MudTextField T="string" Label="Title" Value="_title" ValueChanged="OnTitleChanged"
              Immediate="true" Variant="Variant.Outlined" Class="mb-4" />
<MudTextField T="string" Label="Slug" Value="_slug" ValueChanged="OnSlugChanged"
              Immediate="true" Variant="Variant.Outlined" Class="mb-4" />
<MudTextField T="string" Label="Body" @bind-Value="_body" Lines="15"
              Variant="Variant.Outlined" Class="mb-4" />
<MudSwitch @bind-Value="_published" Label="Published" Color="Color.Primary" Class="mb-4" />

<div class="d-flex gap-4">
    <MudButton OnClick="SaveAsync" Variant="Variant.Filled" Color="Color.Primary">Save</MudButton>
    <MudButton Href="/admin/posts" Variant="Variant.Text">Cancel</MudButton>
</div>

@code {
    [Parameter] public int? Id { get; set; }

    private string _title = "";
    private string _slug = "";
    private string _body = "";
    private bool _published;
    private bool _slugManuallyEdited;
    private string? _error;
    private bool _isNew => Id is null;
    private Post? _existing;
    private PersistentLoader? _loader;

    protected override async Task OnInitializedAsync()
    {
        _loader = new PersistentLoader(ApplicationState);
        if (Id is not null)
        {
            _existing = await _loader.LoadAsync($"post-edit-{Id}", () => PostService.GetByIdAsync(Id.Value));
            if (_existing is not null)
            {
                _title = _existing.Title;
                _slug = _existing.Slug;
                _body = _existing.Body;
                _published = _existing.PublishedAt.HasValue;
                _slugManuallyEdited = true;
            }
            else
            {
                _error = "This post no longer exists.";
            }
        }
    }

    private void OnTitleChanged(string value)
    {
        _title = value;
        if (!_slugManuallyEdited)
            _slug = GenerateSlug(_title);
    }

    private void OnSlugChanged(string value)
    {
        _slug = value;
        _slugManuallyEdited = true;
    }

    private static string GenerateSlug(string title) =>
        System.Text.RegularExpressions.Regex.Replace(
            title.ToLowerInvariant().Replace(' ', '-'),
            @"[^a-z0-9-]", "").Trim('-');

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(_title)) { _error = "Title is required."; return; }
        if (string.IsNullOrWhiteSpace(_slug))  { _error = "Slug is required."; return; }
        if (string.IsNullOrWhiteSpace(_body))  { _error = "Body is required."; return; }

        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userId = authState.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

        try
        {
            if (_isNew)
            {
                var post = new Post
                {
                    Title = _title,
                    Slug = _slug,
                    Body = _body,
                    AuthorId = userId,
                    PublishedAt = _published ? DateTime.UtcNow : null
                };
                await PostService.CreateAsync(post);
            }
            else if (_existing is not null)
            {
                _existing.Title = _title;
                _existing.Slug = _slug;
                _existing.Body = _body;
                _existing.PublishedAt = _published
                    ? (_existing.PublishedAt ?? DateTime.UtcNow)
                    : null;
                await PostService.UpdateAsync(_existing);
            }
            else
            {
                return;
            }
        }
        catch (DbUpdateException)
        {
            _error = "A post with this slug already exists. Please choose a different slug.";
            return;
        }

        Nav.NavigateTo("/admin/posts");
    }

    public void Dispose() => _loader?.Dispose();
}
```

(This is a visual re-skin: validation, slug generation, publish-date preservation, and the two error-handling paths are identical to the current file — do not change their behavior.)

- [ ] **Step 3: Build and test**

```bash
dotnet build src/MeDotNet/MeDotNet.csproj --nologo
dotnet test tests/MeDotNet.Tests/ --nologo --logger "console;verbosity=minimal"
```

Expected: 0 warnings 0 errors; `Passed: 14`.

- [ ] **Step 4: Commit**

```bash
git add src/MeDotNet/Components/Pages/Admin/Posts.razor \
        src/MeDotNet/Components/Pages/Admin/PostEdit.razor
git commit -m "feat: restyle admin pages with MudTable, Mud dialog delete, Mud form"
```

---

### Task 6: Auth fixes — POST logout, RedirectToLogin + ReturnUrl, login styling

**Files:**
- Modify: `src/MeDotNet/Program.cs` (delete MapGet logout endpoint)
- Modify: `src/MeDotNet/Components/Layout/MainLayout.razor` (logout NavLink → POST form)
- Modify: `src/MeDotNet/Pages/Account/Logout.cshtml.cs` (add OnGet redirect)
- Create: `src/MeDotNet/Components/RedirectToLogin.razor`
- Modify: `src/MeDotNet/Components/Routes.razor`
- Modify: `src/MeDotNet/Pages/Account/Login.cshtml.cs` (ReturnUrl support)
- Modify: `src/MeDotNet/Pages/Account/Login.cshtml` (ReturnUrl field + styled markup)
- Create: `src/MeDotNet/wwwroot/css/login.css`

**Interfaces:**
- Consumes: `Logout.cshtml.cs`'s existing `OnPostAsync` (becomes the only logout path); `LoginModel.OnPostAsync`.
- Produces: `/account/logout` responds to POST only (GET redirects home); login honors `?ReturnUrl=`.

- [ ] **Step 1: Delete the GET logout endpoint**

In `src/MeDotNet/Program.cs`, delete this entire block (currently around line 109):

```csharp
app.MapGet("/account/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.LocalRedirect("/");
});
```

- [ ] **Step 2: Swap the drawer's logout link for a POST form**

In `src/MeDotNet/Components/Layout/MainLayout.razor`, replace:

```razor
                    <MudNavLink Href="account/logout" Icon="@Icons.Material.Filled.Logout">Logout</MudNavLink>
```

with:

```razor
                    <form method="post" action="/account/logout" class="d-flex px-4 py-2">
                        <AntiforgeryToken />
                        <MudButton ButtonType="ButtonType.Submit" StartIcon="@Icons.Material.Filled.Logout"
                                   FullWidth="true" Class="justify-start">Logout</MudButton>
                    </form>
```

Note: this is a plain browser POST (no `@onsubmit` handler), so it performs a full-page navigation to the Razor Page — that is intentional. If the docker smoke test hits an antiforgery token mismatch on logout, do NOT revert to a GET endpoint; report DONE_WITH_CONCERNS with the exact error so the controller can decide (fallback design: a logout confirmation page that renders its own form).

- [ ] **Step 3: Add a GET redirect on the Logout page**

In `src/MeDotNet/Pages/Account/Logout.cshtml.cs`, add above `OnPostAsync`:

```csharp
    public IActionResult OnGet() => LocalRedirect("/");
```

(Stray GETs to /account/logout — old bookmarks — now bounce home without signing anyone out.)

- [ ] **Step 4: Create RedirectToLogin.razor**

Create `src/MeDotNet/Components/RedirectToLogin.razor`:

```razor
@inject NavigationManager Navigation

@code {
    protected override void OnInitialized()
    {
        var relative = "/" + Navigation.ToBaseRelativePath(Navigation.Uri);
        Navigation.NavigateTo(
            $"account/login?ReturnUrl={Uri.EscapeDataString(relative)}",
            forceLoad: true);
    }
}
```

(`forceLoad: true` is required — the login page is a Razor Page outside the Blazor router.)

- [ ] **Step 5: Add the NotAuthorized template to Routes.razor**

Replace the entire contents of `src/MeDotNet/Components/Routes.razor` with:

```razor
<CascadingAuthenticationState>
    <Router AppAssembly="typeof(Program).Assembly">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)">
                <NotAuthorized>
                    <RedirectToLogin />
                </NotAuthorized>
            </AuthorizeRouteView>
            <FocusOnNavigate RouteData="routeData" Selector="h1" />
        </Found>
    </Router>
</CascadingAuthenticationState>
```

- [ ] **Step 6: Honor ReturnUrl in LoginModel**

In `src/MeDotNet/Pages/Account/Login.cshtml.cs`, add after the `Input` property:

```csharp
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }
```

and replace:

```csharp
        if (result.Success)
            return LocalRedirect("/admin/posts");
```

with:

```csharp
        if (result.Success)
            return LocalRedirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl! : "/admin/posts");
```

(`Url.IsLocalUrl` rejects absolute/external and protocol-relative URLs, preventing open redirects.)

- [ ] **Step 7: Restyle Login.cshtml with a standalone stylesheet**

Replace the entire contents of `src/MeDotNet/Pages/Account/Login.cshtml` with:

```html
@page
@model MeDotNet.Pages.Account.LoginModel
@{
    Layout = null;
}
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Login — MeDotNet</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@400;500&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="~/css/login.css" />
</head>
<body>
    <main class="login-card">
        <h1>MeDotNet</h1>
        <form method="post">
            <input type="hidden" asp-for="ReturnUrl" />
            <div asp-validation-summary="ModelOnly" class="login-error"></div>

            <label asp-for="Input.Email">Email</label>
            <input asp-for="Input.Email" type="email" autocomplete="username" />
            <span asp-validation-for="Input.Email" class="login-error"></span>

            <label asp-for="Input.Password">Password</label>
            <input asp-for="Input.Password" type="password" autocomplete="current-password" />
            <span asp-validation-for="Input.Password" class="login-error"></span>

            <button type="submit">Sign In</button>
        </form>
    </main>
</body>
</html>
```

Create `src/MeDotNet/wwwroot/css/login.css`:

```css
:root {
    color-scheme: light dark;
    --mud-primary: #594ae2;
    --surface: #ffffff;
    --text: rgba(0, 0, 0, 0.87);
    --border: rgba(0, 0, 0, 0.23);
    --page-bg: #f5f5f5;
}

@media (prefers-color-scheme: dark) {
    :root {
        --surface: #1e1e2d;
        --text: rgba(255, 255, 255, 0.87);
        --border: rgba(255, 255, 255, 0.3);
        --page-bg: #151521;
    }
}

body {
    margin: 0;
    min-height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    font-family: 'Roboto', sans-serif;
    background: var(--page-bg);
    color: var(--text);
}

.login-card {
    background: var(--surface);
    border-radius: 8px;
    box-shadow: 0 3px 6px rgba(0, 0, 0, 0.15);
    padding: 2.5rem;
    width: 100%;
    max-width: 24rem;
}

.login-card h1 {
    margin: 0 0 1.5rem;
    font-size: 1.5rem;
    font-weight: 500;
    text-align: center;
}

.login-card form {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.login-card label {
    font-size: 0.875rem;
    margin-top: 0.5rem;
}

.login-card input:not([type="hidden"]) {
    background: transparent;
    color: var(--text);
    border: 1px solid var(--border);
    border-radius: 4px;
    font-size: 1rem;
    padding: 0.75rem;
}

.login-card input:focus {
    border-color: var(--mud-primary);
    outline: 1px solid var(--mud-primary);
}

.login-card button {
    background: var(--mud-primary);
    border: none;
    border-radius: 4px;
    color: #fff;
    cursor: pointer;
    font-size: 0.875rem;
    font-weight: 500;
    letter-spacing: 0.05em;
    margin-top: 1rem;
    padding: 0.75rem;
    text-transform: uppercase;
}

.login-card button:hover {
    filter: brightness(1.1);
}

.login-error {
    color: #f44336;
    font-size: 0.8rem;
}
```

- [ ] **Step 8: Build and test**

```bash
dotnet build src/MeDotNet/MeDotNet.csproj --nologo
dotnet test tests/MeDotNet.Tests/ --nologo --logger "console;verbosity=minimal"
```

Expected: 0 warnings 0 errors; `Passed: 14`.

- [ ] **Step 9: Full smoke checklist (optional if Docker unavailable — note in report)**

```bash
docker compose up --build -d
```

- Every page renders inside the Mud shell; drawer and dark toggle work
- Anonymous visit to `/admin/posts` → redirected to `/account/login?ReturnUrl=%2Fadmin%2Fposts`; after login, lands back on `/admin/posts`
- Logout button POSTs, signs out, redirects home; GET `/account/logout` just redirects home without signing out
- Create/edit/delete flow works; delete shows a Mud dialog; snackbar appears
- `/posts/nonexistent` returns HTTP 404 (DevTools Network tab)
- View-source on a post page shows the full prerendered article HTML
- `docker compose logs app`: one posts query per page load

- [ ] **Step 10: Commit**

```bash
git add -A src/MeDotNet/
git commit -m "fix: POST logout with antiforgery, login redirect with ReturnUrl, styled login page"
```
