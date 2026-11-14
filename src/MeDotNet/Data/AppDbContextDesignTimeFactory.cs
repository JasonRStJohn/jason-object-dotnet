using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MeDotNet.Data;

// Used ONLY by `dotnet ef` at design time. The app itself registers AppDbContext
// through AddDbContext/AddDbContextFactory in Program.cs; those two registrations
// together fail EF's design-time DI validation, so the tools need a way to build
// the context without spinning up the full host. Point it at the host-exposed
// SQL Server on localhost:1433 (the docker-compose.override.yml port), overridable
// via the ConnectionStrings__DefaultConnection environment variable.
public class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=localhost,1433;Database=MeDotNet;User Id=sa;Password=DevPassword123!;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
