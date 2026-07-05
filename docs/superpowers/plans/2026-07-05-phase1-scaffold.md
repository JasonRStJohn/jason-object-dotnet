# Phase 1 Scaffold Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Scaffold a working Blazor Web App (.NET 9) with SQL Server, EF Core code-first migrations, ASP.NET Core Identity behind an `IAuthService` interface, login/register Razor Pages, and a protected `/admin` stub — all running in Docker behind an existing Nginx Proxy Manager.

**Architecture:** Single Blazor Web App project (SSR-first, opt-in InteractiveServer). SQL Server runs as a sidecar container on an internal Docker network. Auth uses ASP.NET Core Identity wrapped behind `IAuthService` for provider-swappability. Razor Pages handle login/register (they need HttpContext for cookie writing); Blazor handles everything else.

**Tech Stack:** .NET 9 SDK, Blazor Web App, ASP.NET Core Identity, EF Core 9 + SQL Server provider, xUnit + Moq, Docker + docker-compose

---

## File Map

Files created or modified by this plan:

```
medotnet/
├── MeDotNet.sln
├── src/
│   └── MeDotNet/
│       ├── MeDotNet.csproj                          # packages added in Task 3
│       ├── Program.cs                               # configured in Tasks 3, 8
│       ├── appsettings.json                         # Task 1 (generated)
│       ├── appsettings.Development.json             # Task 2 (dev connection string)
│       ├── Components/
│       │   ├── _Imports.razor                       # Task 1 (generated, minor edit)
│       │   ├── App.razor                            # Task 1 (generated, untouched)
│       │   ├── Routes.razor                         # Task 1 (generated, untouched)
│       │   ├── Layout/
│       │   │   ├── MainLayout.razor                 # Task 1 (generated, untouched)
│       │   │   └── NavMenu.razor                    # Task 9 (add login/logout links)
│       │   └── Pages/
│       │       ├── Home.razor                       # Task 1 (generated, untouched)
│       │       ├── Admin.razor                      # Task 10
│       │       └── Error.razor                      # Task 1 (generated, untouched)
│       ├── Data/
│       │   ├── AppDbContext.cs                      # Task 5
│       │   └── Migrations/                          # Task 6 (auto-generated, committed)
│       ├── Models/
│       │   ├── ApplicationUser.cs                   # Task 4
│       │   └── Post.cs                              # Task 4
│       ├── Pages/
│       │   ├── _ViewImports.cshtml                  # Task 9
│       │   ├── Account/
│       │   │   ├── Login.cshtml                     # Task 9
│       │   │   ├── Login.cshtml.cs                  # Task 9
│       │   │   ├── Register.cshtml                  # Task 9
│       │   │   ├── Register.cshtml.cs               # Task 9
│       │   │   ├── Logout.cshtml                    # Task 9
│       │   │   └── Logout.cshtml.cs                 # Task 9
│       └── Services/
│           └── Auth/
│               ├── AuthResult.cs                    # Task 7
│               ├── IAuthService.cs                  # Task 7
│               └── IdentityAuthService.cs           # Task 7
├── tests/
│   └── MeDotNet.Tests/
│       ├── MeDotNet.Tests.csproj                    # Task 1
│       └── Services/
│           └── Auth/
│               └── IdentityAuthServiceTests.cs      # Task 7
├── Dockerfile                                       # Task 2
├── docker-compose.yml                               # Task 2
├── docker-compose.override.yml                      # Task 2 (dev: exposes SQL port)
├── .env.example                                     # Task 2
└── .gitignore                                       # Task 1
```

---

## Task 1: Environment Check & Solution Scaffold

**Files:**
- Create: `MeDotNet.sln`
- Create: `src/MeDotNet/` (Blazor Web App project)
- Create: `tests/MeDotNet.Tests/` (xUnit project)
- Create: `.gitignore`

> **What's happening:** `dotnet new blazor` creates a Blazor Web App — the .NET 9 template that starts with static server-side rendering and lets components opt into interactivity. `--interactivity Server` means components can use `@rendermode InteractiveServer` to become server-interactive. `--no-https` skips HTTPS since NPM handles SSL upstream.

- [ ] **Step 1: Verify .NET 9 SDK**

```bash
dotnet --version
```

Expected: `9.0.x`. If you see `8.x` or older, install .NET 9 SDK from https://dotnet.microsoft.com/download/dotnet/9.0 (Linux: use the install script or your package manager).

- [ ] **Step 2: Create solution and scaffold projects**

```bash
cd /home/jason/sites/medotnet
dotnet new sln -n MeDotNet
dotnet new blazor -n MeDotNet -o src/MeDotNet --interactivity Server --no-https
dotnet new xunit -n MeDotNet.Tests -o tests/MeDotNet.Tests
dotnet sln add src/MeDotNet/MeDotNet.csproj
dotnet sln add tests/MeDotNet.Tests/MeDotNet.Tests.csproj
dotnet add tests/MeDotNet.Tests/MeDotNet.Tests.csproj reference src/MeDotNet/MeDotNet.csproj
```

- [ ] **Step 3: Delete the generated Weather example page**

```bash
rm src/MeDotNet/Components/Pages/Weather.razor
```

- [ ] **Step 4: Verify the project builds**

```bash
dotnet build
```

Expected: `Build succeeded.` with 0 errors.

- [ ] **Step 5: Create .gitignore**

Create `/home/jason/sites/medotnet/.gitignore`:

```gitignore
# .NET build output
bin/
obj/

# User-specific files
*.user
*.suo
.vs/
.idea/
.vscode/

# Secrets (never commit)
.env
appsettings.Local.json

# EF Core migrations backup
*.bak

# OS
.DS_Store
Thumbs.db
```

- [ ] **Step 6: Commit**

```bash
git add .
git commit -m "feat: scaffold Blazor Web App solution with xUnit test project"
```

---

## Task 2: Docker Infrastructure

**Files:**
- Create: `Dockerfile`
- Create: `docker-compose.yml`
- Create: `docker-compose.override.yml`
- Create: `.env.example`
- Modify: `src/MeDotNet/appsettings.Development.json`

> **What's happening:** The Dockerfile uses a multi-stage build — the `sdk` stage compiles and publishes the app, the `aspnet` stage is the lean runtime image. Final image is ~200MB instead of ~800MB. The `docker-compose.override.yml` is a Docker Compose convention: it's automatically merged with `docker-compose.yml` when you run `docker compose up` locally. We use it to expose SQL Server's port for local `dotnet ef` commands.

- [ ] **Step 1: Write the Dockerfile**

Create `/home/jason/sites/medotnet/Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

COPY src/MeDotNet/*.csproj src/MeDotNet/
RUN dotnet restore src/MeDotNet/MeDotNet.csproj

COPY src/ src/
RUN dotnet publish src/MeDotNet/MeDotNet.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MeDotNet.dll"]
```

- [ ] **Step 2: Write docker-compose.yml**

Create `/home/jason/sites/medotnet/docker-compose.yml`:

```yaml
services:
  app:
    build: .
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=MeDotNet;User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True
    depends_on:
      sqlserver:
        condition: service_healthy
    restart: unless-stopped
    networks:
      - internal

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${SA_PASSWORD}
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P \"$SA_PASSWORD\" -Q \"SELECT 1\" -C"]
      interval: 10s
      timeout: 5s
      retries: 10
      start_period: 30s
    volumes:
      - sqlserver_data:/var/lib/mssql/data
    restart: unless-stopped
    networks:
      - internal

networks:
  internal:
    driver: bridge

volumes:
  sqlserver_data:
```

- [ ] **Step 3: Write docker-compose.override.yml (dev only)**

Create `/home/jason/sites/medotnet/docker-compose.override.yml`:

```yaml
# This file is auto-merged by docker compose locally.
# Exposes SQL Server port so you can run `dotnet ef` commands from your host machine.
services:
  sqlserver:
    ports:
      - "1433:1433"
```

- [ ] **Step 4: Write .env.example**

Create `/home/jason/sites/medotnet/.env.example`:

```
# Copy to .env and fill in values. Never commit .env.
# The dev password here matches appsettings.Development.json.
SA_PASSWORD=DevPassword123!
```

- [ ] **Step 5: Create your .env file from the example**

```bash
cp .env.example .env
```

The default dev password (`DevPassword123!`) is fine locally. Change it for any public-facing deployment.

- [ ] **Step 6: Add dev connection string to appsettings.Development.json**

Edit `src/MeDotNet/appsettings.Development.json` — replace the entire file contents with:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MeDotNet;User Id=sa;Password=DevPassword123!;TrustServerCertificate=True"
  }
}
```

> **Note:** This password is only for your local dev database. It's fine to commit because it's a dev-only credential for a container that only accepts local connections.

- [ ] **Step 7: Verify SQL Server starts**

```bash
docker compose up sqlserver -d
docker compose ps
```

Expected: `sqlserver` shows `healthy` after ~30 seconds. If it shows `starting`, wait and run `docker compose ps` again.

- [ ] **Step 8: Commit**

```bash
git add Dockerfile docker-compose.yml docker-compose.override.yml .env.example src/MeDotNet/appsettings.Development.json
git commit -m "feat: add Docker infrastructure with multi-stage build and SQL Server sidecar"
```

---

## Task 3: NuGet Packages & Initial Program.cs Configuration

**Files:**
- Modify: `src/MeDotNet/MeDotNet.csproj`
- Modify: `tests/MeDotNet.Tests/MeDotNet.Tests.csproj`
- Modify: `src/MeDotNet/Program.cs`

> **What's happening:** `Microsoft.AspNetCore.Identity.EntityFrameworkCore` bundles Identity + EF Core together. The `Microsoft.EntityFrameworkCore.Tools` package is only needed at build time (for running `dotnet ef` commands) — that's why it has `PrivateAssets="all"`. We also add `dotnet-ef` as a global tool so you can run migration commands from the terminal.

- [ ] **Step 1: Add EF Core and Identity packages**

```bash
dotnet add src/MeDotNet/MeDotNet.csproj package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 9.0.0
dotnet add src/MeDotNet/MeDotNet.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add src/MeDotNet/MeDotNet.csproj package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
```

- [ ] **Step 2: Add Moq and FluentAssertions to test project**

```bash
dotnet add tests/MeDotNet.Tests/MeDotNet.Tests.csproj package Moq --version 4.20.72
dotnet add tests/MeDotNet.Tests/MeDotNet.Tests.csproj package FluentAssertions --version 6.12.2
```

- [ ] **Step 3: Install EF Core CLI tools globally**

```bash
dotnet tool install --global dotnet-ef
```

If already installed: `dotnet tool update --global dotnet-ef`

Verify:
```bash
dotnet ef --version
```

Expected: `Entity Framework Core .NET Command-line Tools 9.0.x`

- [ ] **Step 4: Replace Program.cs with initial wiring**

Replace `src/MeDotNet/Program.cs` with:

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MeDotNet.Components;
using MeDotNet.Data;
using MeDotNet.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/login";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

> **Note:** `AppDbContext`, `ApplicationUser`, and `IAuthService` don't exist yet — the project won't build until Tasks 4–7 are complete. That's expected.

- [ ] **Step 5: Commit**

```bash
git add src/MeDotNet/MeDotNet.csproj tests/MeDotNet.Tests/MeDotNet.Tests.csproj src/MeDotNet/Program.cs
git commit -m "feat: add EF Core, Identity, and Moq packages; wire up initial Program.cs"
```

---

## Task 4: Domain Models

**Files:**
- Create: `src/MeDotNet/Models/ApplicationUser.cs`
- Create: `src/MeDotNet/Models/Post.cs`

> **What's happening:** In code-first, your C# classes ARE your schema. `ApplicationUser` extends `IdentityUser` (which brings all the Identity columns: Id, Email, PasswordHash, etc.). `Post` is your first custom entity — EF Core will generate a `Posts` table from it. The `required` keyword enforces non-null at both the C# and database level.

- [ ] **Step 1: Create ApplicationUser.cs**

Create `src/MeDotNet/Models/ApplicationUser.cs`:

```csharp
using Microsoft.AspNetCore.Identity;

namespace MeDotNet.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Post> Posts { get; set; } = [];
}
```

- [ ] **Step 2: Create Post.cs**

Create `src/MeDotNet/Models/Post.cs`:

```csharp
namespace MeDotNet.Models;

public class Post
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Body { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required string AuthorId { get; set; }
    public ApplicationUser Author { get; set; } = null!;
}
```

- [ ] **Step 3: Commit**

```bash
git add src/MeDotNet/Models/
git commit -m "feat: add ApplicationUser and Post domain models"
```

---

## Task 5: AppDbContext

**Files:**
- Create: `src/MeDotNet/Data/AppDbContext.cs`

> **What's happening:** `AppDbContext` is the gateway between your C# code and the database. It inherits from `IdentityDbContext<ApplicationUser>` which tells EF Core "this context also manages all the Identity tables." `DbSet<Post>` tells EF Core that `Post` is a table. `OnModelCreating` is where you configure things that can't be expressed by conventions alone — like the unique index on `Slug`.

- [ ] **Step 1: Create AppDbContext.cs**

Create `src/MeDotNet/Data/AppDbContext.cs`:

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MeDotNet.Models;

namespace MeDotNet.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Post>(post =>
        {
            post.HasIndex(p => p.Slug).IsUnique();
            post.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
```

- [ ] **Step 2: Verify the project builds**

```bash
dotnet build
```

Expected: `Build succeeded.` If you see errors about missing `IAuthService`, those are resolved in Task 7.

Actually — Program.cs references `IAuthService` which doesn't exist yet. If there are compile errors, that's fine. Move directly to Task 7 to resolve them. The build will succeed after Task 7.

- [ ] **Step 3: Commit**

```bash
git add src/MeDotNet/Data/AppDbContext.cs
git commit -m "feat: add AppDbContext with Identity and Post entity configuration"
```

---

## Task 6: First EF Core Migration

**Files:**
- Create: `src/MeDotNet/Data/Migrations/` (auto-generated — commit the contents)

> **Code-first migrations explained:** When you add a migration, EF Core compares your current C# models against a snapshot of the last known state, figures out what changed, and generates a C# file describing those changes (create tables, add columns, etc.). When you run `database update`, EF Core executes those changes against your database. You never write SQL DDL by hand. The migration files live in your repo — they're your schema change history.

- [ ] **Step 1: Make sure SQL Server is running**

```bash
docker compose up sqlserver -d && docker compose ps
```

Expected: `sqlserver` is `healthy`.

- [ ] **Step 2: Add the initial migration**

```bash
dotnet ef migrations add InitialCreate --project src/MeDotNet --startup-project src/MeDotNet
```

Expected output ends with: `Done. To undo this action, use 'ef migrations remove'`

This creates three files in `src/MeDotNet/Data/Migrations/`:
- `YYYYMMDDHHMMSS_InitialCreate.cs` — the migration (Up/Down methods)
- `YYYYMMDDHHMMSS_InitialCreate.Designer.cs` — EF Core metadata
- `AppDbContextModelSnapshot.cs` — snapshot of current model state

- [ ] **Step 3: Review the generated migration (educational)**

```bash
cat src/MeDotNet/Data/Migrations/*_InitialCreate.cs
```

You'll see `CreateTable` calls for all the Identity tables (`AspNetUsers`, `AspNetRoles`, etc.) plus your `Posts` table. The `Down()` method drops them — that's how you roll back. You don't need to edit this file.

- [ ] **Step 4: Apply the migration to your local SQL Server**

```bash
dotnet ef database update --project src/MeDotNet --startup-project src/MeDotNet
```

Expected: ends with `Done.`

- [ ] **Step 5: Verify the schema was created**

```bash
docker exec -it $(docker compose ps -q sqlserver) /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "DevPassword123!" -Q "SELECT TABLE_NAME FROM MeDotNet.INFORMATION_SCHEMA.TABLES" -C
```

Expected: a list including `AspNetUsers`, `AspNetRoles`, `Posts`, and other Identity tables.

- [ ] **Step 6: Commit**

```bash
git add src/MeDotNet/Data/Migrations/
git commit -m "feat: add InitialCreate migration (Identity + Posts schema)"
```

---

## Task 7: Auth Abstraction (TDD)

**Files:**
- Create: `src/MeDotNet/Services/Auth/AuthResult.cs`
- Create: `src/MeDotNet/Services/Auth/IAuthService.cs`
- Create: `src/MeDotNet/Services/Auth/IdentityAuthService.cs`
- Create: `tests/MeDotNet.Tests/Services/Auth/IdentityAuthServiceTests.cs`

> **What's happening:** We write the tests first (they'll fail because the class doesn't exist yet), then write just enough implementation to make them pass. Moq creates "fake" versions of `UserManager` and `SignInManager` so we can test our service in isolation — no real database needed.

- [ ] **Step 1: Create AuthResult.cs**

Create `src/MeDotNet/Services/Auth/AuthResult.cs`:

```csharp
namespace MeDotNet.Services.Auth;

public record AuthResult(bool Success, string? ErrorMessage = null);
```

- [ ] **Step 2: Create IAuthService.cs**

Create `src/MeDotNet/Services/Auth/IAuthService.cs`:

```csharp
using System.Security.Claims;
using MeDotNet.Models;

namespace MeDotNet.Services.Auth;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password);
    Task<AuthResult> SignInAsync(string email, string password);
    Task SignOutAsync();
    Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal principal);
}
```

- [ ] **Step 3: Write the failing tests**

Create `tests/MeDotNet.Tests/Services/Auth/IdentityAuthServiceTests.cs`:

```csharp
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using MeDotNet.Models;
using MeDotNet.Services.Auth;

namespace MeDotNet.Tests.Services.Auth;

public class IdentityAuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly IdentityAuthService _authService;

    public IdentityAuthServiceTests()
    {
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
            null, null, null, null);

        _authService = new IdentityAuthService(_userManagerMock.Object, _signInManagerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsSuccess_WhenIdentitySucceeds()
    {
        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _authService.RegisterAsync("test@example.com", "Password123!");

        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_ReturnsFailure_WhenIdentityFails()
    {
        var errors = new[] { new IdentityError { Description = "Email already taken." } };
        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(errors));

        var result = await _authService.RegisterAsync("test@example.com", "Password123!");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Email already taken.");
    }

    [Fact]
    public async Task SignInAsync_ReturnsSuccess_WhenCredentialsAreValid()
    {
        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        var result = await _authService.SignInAsync("test@example.com", "Password123!");

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task SignInAsync_ReturnsFailure_WhenCredentialsAreInvalid()
    {
        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var result = await _authService.SignInAsync("test@example.com", "wrongpassword");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsUser_WhenPrincipalIsAuthenticated()
    {
        var user = new ApplicationUser { Id = "1", Email = "test@example.com" };
        _userManagerMock
            .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var result = await _authService.GetCurrentUserAsync(new ClaimsPrincipal());

        result.Should().Be(user);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsNull_WhenUserNotFound()
    {
        _userManagerMock
            .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _authService.GetCurrentUserAsync(new ClaimsPrincipal());

        result.Should().BeNull();
    }
}
```

- [ ] **Step 4: Run tests — verify they fail**

```bash
dotnet test tests/MeDotNet.Tests/
```

Expected: compile error — `IdentityAuthService` does not exist yet. That's correct.

- [ ] **Step 5: Create IdentityAuthService.cs**

Create `src/MeDotNet/Services/Auth/IdentityAuthService.cs`:

```csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MeDotNet.Models;

namespace MeDotNet.Services.Auth;

public class IdentityAuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityAuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<AuthResult> RegisterAsync(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded
            ? new AuthResult(true)
            : new AuthResult(false, string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<AuthResult> SignInAsync(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password,
            isPersistent: false, lockoutOnFailure: false);
        return result.Succeeded
            ? new AuthResult(true)
            : new AuthResult(false, "Invalid email or password.");
    }

    public async Task SignOutAsync() =>
        await _signInManager.SignOutAsync();

    public async Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal principal) =>
        await _userManager.GetUserAsync(principal);
}
```

- [ ] **Step 6: Run tests — verify they pass**

```bash
dotnet test tests/MeDotNet.Tests/ --logger "console;verbosity=normal"
```

Expected: `Passed! - 6 test(s)`

- [ ] **Step 7: Commit**

```bash
git add src/MeDotNet/Services/ tests/MeDotNet.Tests/Services/
git commit -m "feat: add IAuthService abstraction and IdentityAuthService with passing tests"
```

---

## Task 8: Complete Program.cs Wiring

**Files:**
- Modify: `src/MeDotNet/Program.cs`

> **What's happening:** We add the `IAuthService` DI registration and the auto-migrate call. Auto-migrating on startup means you never need to manually run `dotnet ef database update` in production — the app handles it. The `using` block ensures the scoped DbContext is disposed properly after migration.

- [ ] **Step 1: Add IAuthService registration and auto-migrate to Program.cs**

Replace `src/MeDotNet/Program.cs` with the complete final version:

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MeDotNet.Components;
using MeDotNet.Data;
using MeDotNet.Models;
using MeDotNet.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/login";
});

builder.Services.AddScoped<IAuthService, IdentityAuthService>();

var app = builder.Build();

// Apply pending EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

- [ ] **Step 2: Verify the project builds cleanly**

```bash
dotnet build
```

Expected: `Build succeeded.` — 0 errors now that all referenced types exist.

- [ ] **Step 3: Commit**

```bash
git add src/MeDotNet/Program.cs
git commit -m "feat: register IAuthService in DI and add auto-migrate on startup"
```

---

## Task 9: Auth Razor Pages (Login, Register, Logout)

**Files:**
- Create: `src/MeDotNet/Pages/_ViewImports.cshtml`
- Create: `src/MeDotNet/Pages/Account/Login.cshtml`
- Create: `src/MeDotNet/Pages/Account/Login.cshtml.cs`
- Create: `src/MeDotNet/Pages/Account/Register.cshtml`
- Create: `src/MeDotNet/Pages/Account/Register.cshtml.cs`
- Create: `src/MeDotNet/Pages/Account/Logout.cshtml`
- Create: `src/MeDotNet/Pages/Account/Logout.cshtml.cs`
- Modify: `src/MeDotNet/Components/Layout/NavMenu.razor`

> **Why Razor Pages for auth?** Blazor components don't have direct access to `HttpContext` in the way needed to write auth cookies. Razor Pages do — they're the right tool for actions that need to write to the HTTP response (like setting a session cookie after sign-in). The rest of the site stays Blazor.

- [ ] **Step 1: Create _ViewImports.cshtml**

Create `src/MeDotNet/Pages/_ViewImports.cshtml`:

```cshtml
@using MeDotNet
@using MeDotNet.Pages
@namespace MeDotNet.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

- [ ] **Step 2: Create Login.cshtml**

Create `src/MeDotNet/Pages/Account/Login.cshtml`:

```cshtml
@page
@model MeDotNet.Pages.Account.LoginModel
@{
    Layout = null;
}
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Login</title>
</head>
<body>
    <h1>Login</h1>

    <form method="post">
        <div asp-validation-summary="ModelOnly" style="color:red"></div>

        <div>
            <label asp-for="Input.Email">Email</label>
            <input asp-for="Input.Email" type="email" />
            <span asp-validation-for="Input.Email" style="color:red"></span>
        </div>
        <div>
            <label asp-for="Input.Password">Password</label>
            <input asp-for="Input.Password" type="password" />
            <span asp-validation-for="Input.Password" style="color:red"></span>
        </div>

        <button type="submit">Sign In</button>
        <a href="/account/register">Register</a>
    </form>
</body>
</html>
```

- [ ] **Step 3: Create Login.cshtml.cs**

Create `src/MeDotNet/Pages/Account/Login.cshtml.cs`:

```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MeDotNet.Services.Auth;

namespace MeDotNet.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IAuthService _authService;

    public LoginModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _authService.SignInAsync(Input.Email, Input.Password);
        if (result.Success)
            return LocalRedirect("/admin");

        ModelState.AddModelError(string.Empty, result.ErrorMessage!);
        return Page();
    }
}
```

- [ ] **Step 4: Create Register.cshtml**

Create `src/MeDotNet/Pages/Account/Register.cshtml`:

```cshtml
@page
@model MeDotNet.Pages.Account.RegisterModel
@{
    Layout = null;
}
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Register</title>
</head>
<body>
    <h1>Register</h1>

    <form method="post">
        <div asp-validation-summary="ModelOnly" style="color:red"></div>

        <div>
            <label asp-for="Input.Email">Email</label>
            <input asp-for="Input.Email" type="email" />
            <span asp-validation-for="Input.Email" style="color:red"></span>
        </div>
        <div>
            <label asp-for="Input.Password">Password</label>
            <input asp-for="Input.Password" type="password" />
            <span asp-validation-for="Input.Password" style="color:red"></span>
        </div>
        <div>
            <label asp-for="Input.ConfirmPassword">Confirm Password</label>
            <input asp-for="Input.ConfirmPassword" type="password" />
            <span asp-validation-for="Input.ConfirmPassword" style="color:red"></span>
        </div>

        <button type="submit">Register</button>
        <a href="/account/login">Login</a>
    </form>
</body>
</html>
```

- [ ] **Step 5: Create Register.cshtml.cs**

Create `src/MeDotNet/Pages/Account/Register.cshtml.cs`:

```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MeDotNet.Services.Auth;

namespace MeDotNet.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IAuthService _authService;

    public RegisterModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(8)]
        public string Password { get; set; } = "";

        [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _authService.RegisterAsync(Input.Email, Input.Password);
        if (result.Success)
            return LocalRedirect("/account/login");

        ModelState.AddModelError(string.Empty, result.ErrorMessage!);
        return Page();
    }
}
```

- [ ] **Step 6: Create Logout.cshtml**

Create `src/MeDotNet/Pages/Account/Logout.cshtml`:

```cshtml
@page
@model MeDotNet.Pages.Account.LogoutModel
@{
    Layout = null;
}
```

- [ ] **Step 7: Create Logout.cshtml.cs**

Create `src/MeDotNet/Pages/Account/Logout.cshtml.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MeDotNet.Services.Auth;

namespace MeDotNet.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly IAuthService _authService;

    public LogoutModel(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _authService.SignOutAsync();
        return LocalRedirect("/");
    }
}
```

- [ ] **Step 8: Add login/logout links to NavMenu**

Edit `src/MeDotNet/Components/Layout/NavMenu.razor` — add the following block at the bottom of the `<nav>` element, just before the closing `</nav>` tag:

```razor
<AuthorizeView>
    <Authorized>
        <form action="/account/logout" method="post">
            <AntiForgeryToken />
            <button type="submit">Logout</button>
        </form>
    </Authorized>
    <NotAuthorized>
        <a href="/account/login">Login</a>
    </NotAuthorized>
</AuthorizeView>
```

> **Note:** `<AuthorizeView>` is a built-in Blazor component that renders different content based on auth state. It reads the cookie that Identity set during sign-in.

- [ ] **Step 9: Verify the project builds**

```bash
dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 10: Commit**

```bash
git add src/MeDotNet/Pages/ src/MeDotNet/Components/Layout/NavMenu.razor
git commit -m "feat: add Login, Register, and Logout Razor Pages; wire auth links in nav"
```

---

## Task 10: Admin Stub Page

**Files:**
- Create: `src/MeDotNet/Components/Pages/Admin.razor`

> **What's happening:** `@attribute [Authorize]` tells ASP.NET Core's authorization middleware that this page requires an authenticated user. Unauthenticated requests are redirected to the `LoginPath` configured in `Program.cs` (`/account/login`). This exercises the full auth flow end-to-end.

- [ ] **Step 1: Create Admin.razor**

Create `src/MeDotNet/Components/Pages/Admin.razor`:

```razor
@page "/admin"
@attribute [Authorize]
@using Microsoft.AspNetCore.Authorization

<PageTitle>Admin</PageTitle>

<h1>Admin</h1>

<p>You are logged in. Content management coming in phase 2.</p>
```

- [ ] **Step 2: Add the Authorize using to _Imports.razor**

Edit `src/MeDotNet/Components/_Imports.razor` — add this line at the bottom:

```razor
@using Microsoft.AspNetCore.Authorization
```

- [ ] **Step 3: Verify build**

```bash
dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```bash
git add src/MeDotNet/Components/Pages/Admin.razor src/MeDotNet/Components/_Imports.razor
git commit -m "feat: add protected /admin stub page with [Authorize]"
```

---

## Task 11: End-to-End Docker Verification

**Goal:** Confirm the complete stack runs in Docker — app starts, migrations apply, register/login/admin/logout works.

- [ ] **Step 1: Build and start the full stack**

```bash
docker compose up --build -d
docker compose logs -f app
```

Watch the logs until you see `Application started. Press Ctrl+C to shut down.`

If you see a migration error, check the SQL Server health: `docker compose ps`

- [ ] **Step 2: Verify the app is reachable**

```bash
curl -s -o /dev/null -w "%{http_code}" http://localhost:5000
```

Expected: `200`

- [ ] **Step 3: Manual smoke test — register a user**

Open a browser to `http://localhost:5000/account/register` (or use curl/httpie if headless).

If headless, use curl:

```bash
# Get the antiforgery token first
curl -c cookies.txt -s http://localhost:5000/account/register -o /dev/null
TOKEN=$(curl -c cookies.txt -b cookies.txt -s http://localhost:5000/account/register | grep '__RequestVerificationToken' | grep -oP 'value="\K[^"]+')

# Submit registration
curl -c cookies.txt -b cookies.txt -X POST http://localhost:5000/account/register \
  -d "Input.Email=test@example.com&Input.Password=Password123!&Input.ConfirmPassword=Password123!&__RequestVerificationToken=$TOKEN" \
  -L -v 2>&1 | grep "< HTTP"
```

Expected: HTTP 302 redirect to `/account/login`

- [ ] **Step 4: Manual smoke test — login and access /admin**

```bash
# Login
LOGIN_TOKEN=$(curl -c cookies.txt -b cookies.txt -s http://localhost:5000/account/login | grep '__RequestVerificationToken' | grep -oP 'value="\K[^"]+')
curl -c cookies.txt -b cookies.txt -X POST http://localhost:5000/account/login \
  -d "Input.Email=test@example.com&Input.Password=Password123!&__RequestVerificationToken=$LOGIN_TOKEN" \
  -L -v 2>&1 | grep "< HTTP"
```

Expected: HTTP 302 redirect to `/admin`

```bash
# Access /admin with the auth cookie
curl -b cookies.txt http://localhost:5000/admin -s | grep -i "admin"
```

Expected: HTML containing `Admin` page content.

- [ ] **Step 5: Verify unauthenticated /admin redirects to login**

```bash
curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/admin
```

Expected: `302` (redirect to login, not 200 or 401).

- [ ] **Step 6: Clean up test artifacts**

```bash
rm -f cookies.txt
```

- [ ] **Step 7: Final commit**

```bash
git add .
git commit -m "chore: phase 1 complete — Blazor Web App with Identity, EF Core, and Docker running end-to-end"
```

---

## Phase 1 Complete

At this point you have:
- A working Blazor Web App (.NET 9) running in Docker
- SQL Server 2022 as a sidecar with a named volume for persistence
- EF Core code-first migrations with auto-apply on startup
- `IAuthService` abstraction with a tested `IdentityAuthService` implementation
- Register, Login, Logout pages
- A protected `/admin` stub
- NPM-ready: proxy `your-domain.com` → `localhost:5000`

**Next steps (Phase 2):** CMS admin UI, email confirmation, password reset.
