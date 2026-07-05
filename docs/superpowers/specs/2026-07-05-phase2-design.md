# medotnet — Phase 2 Design Spec
**Date:** 2026-07-05
**Status:** Approved

## Overview

Phase 2 completes the core content loop: an authenticated admin can create and manage posts, and anonymous visitors can read published posts. Auth is hardened by removing open registration and seeding a single admin account from environment variables.

No new EF Core migrations are needed — the Phase 1 Post schema already has all required fields.

---

## Scope

| Area | Included |
|------|----------|
| Admin seeding | Yes |
| Remove public registration | Yes |
| PostService (CRUD) | Yes |
| CMS admin UI (list, create, edit, delete) | Yes |
| Public post list (`/posts`) | Yes |
| Public post detail (`/posts/{slug}`) | Yes |
| Markdown rendering | No — Phase 3 |
| Email confirmation / password reset | No — Phase 3 |
| Roles / claims-based auth | No — not needed; logged in = admin |
| Soft delete | No — hard delete only |

---

## Section 1 — Admin Seeding & Registration Removal

### New Environment Variables

Two vars added to `.env`, `.env.example`, and `docker-compose.yml`:

```
ADMIN_EMAIL=you@example.com
ADMIN_PASSWORD=YourSecurePassword1!
```

### Startup Seeding

After the existing auto-migrate block in `Program.cs`, a new seeding block runs:

```csharp
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var adminEmail = app.Configuration["ADMIN_EMAIL"];
    var adminPassword = app.Configuration["ADMIN_PASSWORD"];

    if (adminEmail is not null && adminPassword is not null)
    {
        var existing = await userManager.FindByEmailAsync(adminEmail);
        if (existing is null)
        {
            var user = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
            await userManager.CreateAsync(user, adminPassword);
        }
    }
}
```

Idempotent — if the user already exists, the block no-ops. If either env var is missing, seeding is skipped (safe for CI/test environments).

### Registration Removal

- Delete `Pages/Account/Register.cshtml` and `Register.cshtml.cs`
- Remove the register link from `Pages/Account/Login.cshtml`
- No route change needed — the Razor Page is simply gone; hitting `/account/register` returns 404

---

## Section 2 — PostService

### Location

`src/MeDotNet/Services/Posts/PostService.cs`

### Interface Decision

No interface — unlike `IAuthService` (designed for provider swapping), the post data layer will always be EF Core. A concrete class is sufficient per YAGNI.

### Methods

```csharp
public class PostService(AppDbContext db)
{
    Task<List<Post>> GetAllAsync();           // All posts, CreatedAt desc — for admin list
    Task<List<Post>> GetPublishedAsync();     // PublishedAt != null only, CreatedAt desc — for public list
    Task<Post?> GetByIdAsync(int id);         // Single post by PK — for edit form
    Task<Post?> GetBySlugAsync(string slug);  // Single published post by slug — for public detail
    Task CreateAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(int id);
}
```

### Registration

```csharp
builder.Services.AddScoped<PostService>();
```

### Tests

`tests/MeDotNet.Tests/Services/Posts/PostServiceTests.cs`

Uses EF Core's `UseInMemoryDatabase` provider — no mocking needed. Tests cover: GetAll returns all posts ordered correctly, GetPublished excludes drafts, GetBySlug returns null for unpublished, Create persists, Update mutates, Delete removes.

---

## Section 3 — CMS Admin UI

### Location

`src/MeDotNet/Components/Pages/Admin/`

All pages use `@attribute [Authorize]` and `@rendermode InteractiveServer`.

### Post List — `/admin/posts`

File: `Posts.razor`

- Calls `PostService.GetAllAsync()` on load
- Table columns: Title, Slug, Status (Draft / Published), Created, Actions
- Actions: Edit (link to `/admin/posts/{id}`) and Delete
- Delete triggers `window.confirm()` before calling `PostService.DeleteAsync()`; list refreshes after
- "New Post" button links to `/admin/posts/new`

### Post Form — `/admin/posts/new` and `/admin/posts/{Id:int}`

File: `PostEdit.razor`

Single component handles both create and edit:
- If `Id` parameter is absent → create mode
- If `Id` parameter is present → edit mode, loads post via `GetByIdAsync` on init

**Form fields:**

| Field | Control | Behaviour |
|-------|---------|-----------|
| Title | `<input>` | Typing auto-generates slug while slug is unedited |
| Slug | `<input>` | Pre-filled from title; once manually edited, auto-gen stops |
| Body | `<textarea>` | Full width, ~15 rows |
| Published | `<input type="checkbox">` | Checked = published; sets `PublishedAt = DateTime.UtcNow` on save (or null if unchecked) |

**Slug generation logic (server-side, runs on title change):**

1. Lowercase the title
2. Replace spaces with hyphens
3. Strip characters that are not alphanumeric or hyphens
4. Trim leading/trailing hyphens

**Save behaviour:**
- Create mode → `PostService.CreateAsync()` → redirect to `/admin/posts`
- Edit mode → `PostService.UpdateAsync()` → redirect to `/admin/posts`
- Cancel → redirect to `/admin/posts` without saving

**AuthorId:** Set to the current user's ID via `UserManager.GetUserId(principal)` on create. Not changed on update.

### NavMenu Updates

Authorized section:
- Home (existing)
- Posts → `/admin/posts` (new)
- Logout (existing)

---

## Section 4 — Public Post Views

Both pages are static SSR (no `@rendermode` directive) for fast load and SEO.

### Post List — `/posts`

File: `Components/Pages/Posts.razor`

- Calls `PostService.GetPublishedAsync()` on render
- Renders a list of published posts: title (linked to `/posts/{slug}`), published date, first ~150 chars of body as excerpt
- If no published posts: displays "No posts yet."

### Post Detail — `/posts/{Slug}`

File: `Components/Pages/PostDetail.razor`

- Calls `PostService.GetBySlugAsync(Slug)` on render
- If null: injects `HttpContext` (available in SSR components) to set `Response.StatusCode = 404`, then renders a "Post not found." message on the same page
- Renders: `<h1>` title, published date, body in `<p>` tags (plain text — markdown rendering deferred to Phase 3)

### NavMenu Updates

A "Blog" link to `/posts` added to the public (non-authorized) section, visible to all users.

---

## File Checklist

### New files
- `src/MeDotNet/Services/Posts/PostService.cs`
- `src/MeDotNet/Components/Pages/Admin/Posts.razor`
- `src/MeDotNet/Components/Pages/Admin/PostEdit.razor`
- `src/MeDotNet/Components/Pages/Posts.razor`
- `src/MeDotNet/Components/Pages/PostDetail.razor`
- `tests/MeDotNet.Tests/Services/Posts/PostServiceTests.cs`

### Modified files
- `Program.cs` — admin seeding block, PostService DI registration
- `docker-compose.yml` — ADMIN_EMAIL, ADMIN_PASSWORD env vars
- `.env.example` — ADMIN_EMAIL, ADMIN_PASSWORD vars
- `Components/Layout/NavMenu.razor` — Blog link (public), Posts link (authorized)
- `Pages/Account/Login.cshtml` — remove register link

### Deleted files
- `Pages/Account/Register.cshtml`
- `Pages/Account/Register.cshtml.cs`

---

## Phase Roadmap (Updated)

| Phase | Scope |
|-------|-------|
| 1 | ✅ Scaffold, Identity, EF Core, Post entity, /admin stub, Docker |
| 2 | Admin seeding, remove registration, PostService, CMS UI, public post views |
| 3 | Markdown rendering, email confirmation, password reset, portfolio section, roles |
