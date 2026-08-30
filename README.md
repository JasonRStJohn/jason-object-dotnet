# JasonObject (.NET)

A personal portfolio and content platform built with **ASP.NET Core (.NET 9)**, **Blazor**, **MudBlazor**, and **Entity Framework Core**.

## 🏗️ Architecture & Features

- **Frontend & UI:** Blazor Server with [MudBlazor](https://mudblazor.com/) component framework for a responsive, accessible interface.
- **Backend & Data Access:** ASP.NET Core 9 with Entity Framework Core and SQL Server / SQLite.
- **Authentication:** Custom Identity-backed authentication service with complete xUnit test coverage.
- **Testing:** Unit and service test suite using **xUnit**, **FluentAssertions**, and **Moq** (`tests/MeDotNet.Tests`).
- **Engineering Specs:** Detailed architectural design specifications and phase plans maintained under `docs/superpowers/`.
- **Containerization:** Dockerfile and Docker Compose setup for consistent local and deployment environments.

## 📁 Project Structure

```
├── docs/superpowers/       # Architecture design specs & phased implementation plans
├── src/
│   └── MeDotNet/           # Blazor web application, services, and models
│       ├── Components/     # Blazor Razor components & layouts
│       ├── Data/           # EF Core DbContext and migrations
│       ├── Models/         # Data and domain models
│       ├── Pages/          # Identity and account management pages
│       └── Services/       # Auth and content service abstractions
└── tests/
    └── MeDotNet.Tests/     # Service and unit tests (xUnit + FluentAssertions + Moq)
```

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Docker & Docker Compose (optional)

### Local Development

1. **Clone the repository:**
   ```bash
   git clone https://github.com/JasonRStJohn/jason-object-dotnet.git
   cd jason-object-dotnet
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Run the application:**
   ```bash
   dotnet run --project src/MeDotNet/MeDotNet.csproj
   ```

### Running Tests

Execute the unit test suite:
```bash
dotnet test
```

## 📄 License

MIT
