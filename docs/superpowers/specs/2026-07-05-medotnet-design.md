# medotnet — Personal Website Design Spec
**Date:** 2026-07-05
**Status:** Approved

## Overview

A personal website built with C#, Blazor Web App (.NET 9), Entity Framework Core (code-first), SQL Server 2022, and Docker. Primary goals: serve as a public portfolio/blog/lab hybrid, and keep C# skills sharp during a career transition. Designed to evolve across phases rather than be fully specified upfront.

---

## Architecture

A single **Blazor Web App (.NET 9)** project. Pages default to server-side rendering (fast, SEO-friendly). Components opt into interactivity on a per-component basis using Blazor's interactive render modes.

No internal nginx — Kestrel serves the app directly. SSL termination and domain routing are handled upstream by Nginx Proxy Manager (NPM).

```
Internet
  └── NPM (SSL termination, domain routing)
        └── localhost:5000
              └── medotnet container (Kestrel :8080)
                    └── sqlserver container (internal Docker network)
```

**External dependencies:**
- NPM already running on the host, routes by domain to `localhost:5000`
- `.env` file supplies secrets; never committed to git

---

## Project Structure

```
medotnet/
├── src/
│   └── MeDotNet/
│       ├── Components/
│       │   ├── Pages/
│       │   └── Layout/
│       ├── Services/
│       │   └── Auth/
│       │       ├── IAuthService.cs
│       │       └── IdentityAuthService.cs
│       ├── Data/
│       │   ├── AppDbContext.cs
│       │   └── Migrations/
│       ├── Models/
│       └── Program.cs
├── docker-compose.yml
├── Dockerfile
├── .env.example
├── .gitignore
└── docs/
    └── superpowers/specs/
```

---

## Data Model (Phase 1)

Code-first: C# entity classes define the schema. EF Core generates and applies migrations via CLI.

### ApplicationUser
Extends `IdentityUser` (provided by ASP.NET Core Identity). Add custom profile fields here in later phases.

### Post
```
Id          int           PK, auto-increment
Title       string        required
Slug        string        required, unique — URL-friendly identifier
Body        string        required — stored as plain text/markdown; rendering format decided in phase 2
PublishedAt DateTime?     null = draft, non-null = published
CreatedAt   DateTime      set on insert
AuthorId    string        FK → ApplicationUser.Id
```

`Post` is the first custom entity — enough to exercise the full code-first workflow without over-engineering before the site's content needs are clear.

---

## Authentication

**Abstracted behind an interface** so the implementation can be swapped (e.g., Identity → Cognito) without touching the rest of the app.

```csharp
// Simple value object: bool Success + string? ErrorMessage
public record AuthResult(bool Success, string? ErrorMessage = null);

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password);
    Task<AuthResult> SignInAsync(string email, string password);
    Task SignOutAsync();
    Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal principal);
}
```

**Phase 1 implementation:** `IdentityAuthService : IAuthService` wrapping ASP.NET Core Identity.
Registered in `Program.cs`:
```csharp
builder.Services.AddScoped<IAuthService, IdentityAuthService>();
```

**Phase 1 scope:** register, login, logout, `/admin` stub protected by `[Authorize]`.
**Deferred to phase 2:** email confirmation, password reset, roles, external providers.

---

## Docker Setup

Two containers, one `docker-compose.yml`:

| Container   | Image                                      | Exposed        |
|-------------|--------------------------------------------|----------------|
| `app`       | built from `Dockerfile` (multi-stage)      | host port 5000 |
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | internal only  |

- `app` depends on `sqlserver` with a health check (SQL Server takes ~15s to be ready)
- `sqlserver` data persisted to a named Docker volume (`sqlserver_data`)
- Secrets (`SA_PASSWORD`, connection string) supplied via `.env`, never hardcoded
- `Dockerfile`: multi-stage — `sdk` image compiles, `aspnet` runtime image runs (lean final image)

---

## Phase Roadmap

| Phase | Scope |
|-------|-------|
| 1 | Project scaffold, EF Core + Identity, Post entity, `/admin` stub, Docker working end-to-end |
| 2 | CMS admin UI (create/edit posts), email confirmation, password reset |
| 3 | Portfolio section, public blog, extended auth (roles, external providers) |

---

## What's Explicitly Out of Scope for Phase 1

- CMS / admin content management UI
- Email confirmation or password reset
- Roles or claims-based authorization
- Public-facing blog or portfolio pages (beyond scaffolded defaults)
- CI/CD pipeline
