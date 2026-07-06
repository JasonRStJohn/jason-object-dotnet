using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MeDotNet.Components;
using MeDotNet.Data;
using MeDotNet.Models;
using MeDotNet.Services.Auth;
using MeDotNet.Services.Posts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/login";
});

builder.Services.AddScoped<IAuthService, IdentityAuthService>();
builder.Services.AddScoped<PostService>();

var app = builder.Build();

// Apply pending EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
    try
    {
        await db.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed on startup");
        throw;
    }
}

// Seed admin user on first run
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var adminEmail = app.Configuration["ADMIN_EMAIL"];
    var adminPassword = app.Configuration["ADMIN_PASSWORD"];

    if (adminEmail is not null && adminPassword is not null)
    {
        try
        {
            var existing = await userManager.FindByEmailAsync(adminEmail);
            if (existing is null)
            {
                var user = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
                var result = await userManager.CreateAsync(user, adminPassword);
                if (!result.Succeeded)
                    logger.LogError("Admin seed failed: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                else
                    logger.LogInformation("Admin user seeded: {Email}", adminEmail);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Admin seeding failed");
        }
    }
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

app.MapGet("/account/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.LocalRedirect("/");
});

app.Run();
