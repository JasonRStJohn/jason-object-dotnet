# Phase 3 — MudBlazor Adoption Design Spec

**Date:** 2026-07-07
**Branch:** feature/mudblazor (first feature-branch phase; no more direct-to-main work)
**Base:** main @ 8739cae (end of Phase 2 CMS)

## Goal

Adopt MudBlazor as the app-wide component library: a Material shell (app bar, nav drawer, dark mode) wrapping every page, Mud components replacing hand-written markup on public and admin pages, Bootstrap removed entirely. Three review findings that live in the same files are bundled in: POST logout, login redirect for unauthorized visitors, and a real Home page.

## Decisions (settled during brainstorming)

| Decision | Choice |
|---|---|
| Scope | Whole app: shell + admin + public pages |
| Bootstrap | Removed entirely (assets, template CSS, markup classes) |
| Theme | Stock MudTheme, dark-mode aware via system preference + manual toggle, no persistence of manual choice |
| Render model | Global InteractiveServer at the router, prerendering on (Approach 1) |
| Double-query mitigation | `PersistentComponentState` via a shared `PersistentLoader` helper |
| Bundled fixes | POST logout, RedirectToLogin + ReturnUrl handling, real Home page |
| Explicitly out of scope | Custom palette/branding, markdown rendering of post bodies, bUnit/component tests, persisting the dark-mode toggle, the PostEdit stale-state fix (separate branch), pagination/projection efficiency fixes (separate branch) |

## Architecture

### Render modes

`<Routes @rendermode="InteractiveServer" />` in `App.razor` makes the whole app Interactive Server with prerendering (the default). Per-page `@rendermode InteractiveServer` directives on admin pages are removed as redundant. Public pages gain interactivity; SEO is preserved because every page still prerenders complete HTML into the initial response.

Consequences accepted:
- Each visitor holds a SignalR circuit. Trivial at personal-blog traffic.
- Components initialize twice (prerender pass + circuit start). Mitigated by `PersistentLoader` (below) so the database is queried once per page view.
- The cascading `HttpContext` parameter is non-null only during the prerender pass — which is exactly when `PostDetail` sets its 404 status, so that code keeps working unchanged. The existing null guard is the seam between the two passes.

### Package & registration

- `MudBlazor` NuGet package (latest stable 8.x for .NET 9) in `MeDotNet.csproj`.
- `builder.Services.AddMudServices();` in `Program.cs`.
- `@using MudBlazor` in `Components/_Imports.razor`.
- `App.razor`: MudBlazor stylesheet + script and its two font links replace the Bootstrap stylesheet references.

### PersistentLoader

One small helper (`Components/PersistentLoader.cs`) wrapping the `PersistentComponentState` register/take/dispose boilerplate, exposing a load-once semantic: on the prerender pass it runs the query and registers the result for persistence; on the circuit pass it takes the persisted JSON instead of re-querying. Used by all four data-loading pages. `PostEdit` keys its persisted state by `Id`. Persisted state rides in the initial HTML as JSON (a post body appears once rendered and once serialized — acceptable size cost for blog content).

### Bootstrap removal

Delete `wwwroot/lib/bootstrap/`, `NavMenu.razor` + `NavMenu.razor.css`, `MainLayout.razor.css` template styles, and `app.css` rules that only served the old template. Remove Bootstrap utility classes from any surviving markup. The login Razor Page (outside Blazor, can't use Mud components) gets a small standalone stylesheet approximating the Mud default look (centered card, matching typography/colors).

## Shell & theming

`MainLayout.razor` becomes the standard Mud shell:

- Providers at top: `MudThemeProvider`, `MudPopoverProvider`, `MudDialogProvider`, `MudSnackbarProvider` — all fully functional everywhere since the app is globally interactive.
- `MudLayout` &gt; `MudAppBar` (drawer toggle, site title, dark-mode sun/moon toggle) + `MudDrawer` (`MudNavMenu`/`MudNavLink`) + `MudMainContent` &gt; `MudContainer` for the page body.
- Theme: stock `MudTheme`. `@bind-IsDarkMode` with system-preference detection on first render, plus the manual toggle. Manual choice not persisted (refresh returns to system preference). Known cosmetic limitation: prerendered HTML is light-themed; dark-preference visitors see a brief light flash before the circuit applies dark mode. Accepted; a future inline-script fix is possible.
- Nav content: Home and Blog links always; `<AuthorizeView>` adds admin Posts + Logout for authenticated users, Login for anonymous — same structure as today.

## Page migrations

### Public

- **`Posts.razor` (/posts):** each post rendered as a `MudCard`/`MudPaper` — `Typo.h5` title link, `Typo.caption` date, excerpt text. Data via `PersistentLoader`. Logic otherwise unchanged.
- **`PostDetail.razor` (/posts/{slug}):** `Typo.h3` title, caption date, body still plain text with `white-space: pre-wrap` (no markdown this phase). 404 logic untouched. Data via `PersistentLoader`.
- **`Home.razor` (bundled fix):** replaced with a real landing — intro `MudPaper`, the 3 most recent published posts (reusing `GetPublishedAsync`, taking 3 in the component), and a "View all posts" button to `/posts`.

### Admin

- **`Admin/Posts.razor`:** HTML table → `MudTable<Post>`. Delete confirmation: `IDialogService.ShowMessageBox` replaces the JS `confirm()`; `IJSRuntime` injection removed. `MudSnackbar` confirms deletion. Data via `PersistentLoader`.
- **`Admin/PostEdit.razor`:** `MudTextField` for title and slug, multiline `MudTextField` for body, `MudSwitch` for published, `MudButton` save/cancel, `MudAlert` for `_error`. Slug auto-generation and all save logic move over unchanged — visual re-skin only; the stale-state bug fix is a separate branch. Data via `PersistentLoader`, keyed by `Id`.

## Auth flow fixes (bundled)

- **POST logout:** the drawer's logout control becomes a `<form method="post" action="/account/logout">` with `<AntiforgeryToken />`, styled to match the nav links. `Logout.cshtml.cs`'s existing `OnPostAsync` (the `IAuthService` path) becomes the only logout path. The `MapGet("/account/logout")` endpoint in `Program.cs` is deleted. This closes the CSRF-exposed state-changing GET and removes the orphaned-code split.
- **RedirectToLogin:** `Routes.razor` gets a `<NotAuthorized>` template containing a small `RedirectToLogin` component that navigates to `/account/login?ReturnUrl=<current relative URL>`. `Login.cshtml.cs` honors `ReturnUrl` on successful sign-in via `LocalRedirect` (which rejects absolute/external URLs), falling back to `/admin/posts` when absent.
- **Login page look:** `Login.cshtml` keeps plain HTML inputs; a small standalone stylesheet gives it a Mud-consistent centered-card appearance.

## Testing & verification

- Existing 14 unit tests must stay green; `PostService` is unchanged by this work.
- No bUnit/component-test infrastructure this phase.
- Build must be clean (0 warnings on the app project).
- Docker smoke checklist: every page renders in the Mud shell; dark toggle works; drawer nav works; delete shows a Mud dialog and works; create/edit flow works; logout POSTs and signs out; anonymous `/admin/posts` redirects to login and returns to the requested page after auth; `/posts/nonexistent` returns HTTP 404; view-source on a post page shows prerendered content; EF logs show one query per page view (PersistentLoader working).

## Migration order (basis for the implementation plan)

1. Package, `AddMudServices`, `App.razor` references, global InteractiveServer, Bootstrap asset removal — app builds and runs in a functional-but-unstyled state.
2. `MainLayout` shell + drawer + theme; delete `NavMenu.razor`.
3. `PersistentLoader` helper; wire into the four data pages.
4. Public pages: `Posts`, `PostDetail`, new `Home`.
5. Admin pages: `MudTable` list + dialog delete, `PostEdit` form.
6. Auth fixes: POST logout, `RedirectToLogin` + `ReturnUrl`, login stylesheet.

Each step leaves the app working and is one commit, executed on `feature/mudblazor`.
