# Kdx Web Template

A starter ASP.NET Core Razor Pages web application template that demonstrates the usage of Kdx NuGet packages.

## Features

- **ASP.NET Core 8.0** Razor Pages application
- **Kdx.Contracts (v2.0.0-alpha)** - DTOs and repository interfaces
- **Kdx.Infrastructure.Supabase (v2.0.0-alpha)** - Supabase data access implementation
- **Dependency Injection** - Pre-configured with repository pattern
- **Sample Pages** - Demonstrates data retrieval from Supabase

## Prerequisites

- .NET 8.0 SDK or later
- Supabase account and project
- Visual Studio 2022 / Visual Studio Code / JetBrains Rider (optional)

## Getting Started

### 1. Clone or Fork the Project

```bash
git clone https://github.com/your-org/kdxprojects.git
cd kdxprojects/src/Kdx.Web.Template
```

### 2. Configure Supabase

Update `appsettings.json` or `appsettings.Development.json` with your Supabase credentials:

```json
{
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "Key": "your-anon-key-here"
  }
}
```

To get your Supabase credentials:
1. Go to [Supabase Dashboard](https://app.supabase.com)
2. Select your project
3. Go to Settings → API
4. Copy the **Project URL** and **anon/public key**

### 3. Run the Application

```bash
dotnet restore
dotnet run
```

Or using Visual Studio:
1. Open `Kdx.Web.Template.csproj`
2. Press F5 to run

The application will start at `https://localhost:5001` (or another port shown in console).

## Project Structure

```
Kdx.Web.Template/
├── Pages/
│   ├── Companies.cshtml          # Sample page showing company data
│   ├── Companies.cshtml.cs       # Page model with repository usage
│   ├── Index.cshtml               # Home page
│   ├── Privacy.cshtml             # Privacy page
│   └── Shared/
│       └── _Layout.cshtml         # Shared layout
├── wwwroot/                       # Static files
├── appsettings.json               # Application configuration
├── appsettings.Development.json   # Development configuration
├── Program.cs                     # Application entry point
└── README.md                      # This file
```

## Usage Examples

### Injecting Repository in Page Models

```csharp
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CompaniesModel : PageModel
{
    private readonly ISupabaseRepository _repository;

    public CompaniesModel(ISupabaseRepository repository)
    {
        _repository = repository;
    }

    public List<Company>? Companies { get; set; }

    public async Task OnGetAsync()
    {
        Companies = await _repository.GetCompaniesAsync();
    }
}
```

### Using in Controllers (if using MVC)

```csharp
using Kdx.Infrastructure.Supabase.Repositories;
using Microsoft.AspNetCore.Mvc;

public class CompaniesController : Controller
{
    private readonly ISupabaseRepository _repository;

    public CompaniesController(ISupabaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        var companies = await _repository.GetCompaniesAsync();
        return View(companies);
    }
}
```

## Available Repository Methods

The `ISupabaseRepository` interface provides access to various data entities:

### Company Operations
- `GetCompaniesAsync()` - Get all companies
- `GetCompanyByIdAsync(int id)` - Get company by ID
- `AddCompanyAsync(Company company)` - Add new company
- `UpdateCompanyAsync(Company company)` - Update company
- `DeleteCompanyAsync(int id)` - Delete company

### Model Operations
- `GetModelsAsync()` - Get all models
- `GetModelByIdAsync(int id)` - Get model by ID
- Additional CRUD operations

### PLC Operations
- `GetPLCsAsync()` - Get all PLCs
- `GetPLCByIdAsync(int id)` - Get PLC by ID
- Additional CRUD operations

### Machine Operations
- `GetMachinesAsync()` - Get all machines
- `GetMachineByIdAsync(int id)` - Get machine by ID
- Additional CRUD operations

...and many more. See `ISupabaseRepository.cs` for the complete list.

## Configuration

### Dependency Injection Setup

The repository is configured in `Program.cs`:

```csharp
// Configure Supabase Client
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];

var options = new SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true
};

builder.Services.AddSingleton(provider => new Client(supabaseUrl, supabaseKey, options));
builder.Services.AddScoped<ISupabaseRepository, SupabaseRepository>();
```

### Environment-Specific Configuration

Use different configuration files for different environments:

- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production overrides

## Troubleshooting

### "Supabase URL not configured" error

**Solution:** Make sure you've updated `appsettings.json` with your Supabase credentials.

### Database connection errors

**Solution:**
1. Verify your Supabase URL and Key are correct
2. Check that your Supabase project is running
3. Verify your database tables exist and match the DTOs

### Build errors

**Solution:**
1. Run `dotnet restore` to restore NuGet packages
2. Ensure you're using .NET 8.0 SDK or later
3. Check that all referenced projects are built successfully

## Customization

### Adding New Pages

1. Create a new `.cshtml` file in the `Pages` folder
2. Create the corresponding `.cshtml.cs` page model
3. Inject `ISupabaseRepository` in the constructor
4. Add navigation link in `_Layout.cshtml`

### Using with Different Data Sources

To switch from Supabase to another data source:

1. Create a new infrastructure library (e.g., `Kdx.Infrastructure.SqlServer`)
2. Implement `ISupabaseRepository` interface
3. Update dependency injection in `Program.cs`:

```csharp
builder.Services.AddScoped<ISupabaseRepository, SqlServerRepository>();
```

## Related Packages

- **Kdx.Contracts** - DTOs and interfaces ([NuGet](https://www.nuget.org/packages/Kdx.Contracts))
- **Kdx.Infrastructure.Supabase** - Supabase implementation ([NuGet](https://www.nuget.org/packages/Kdx.Infrastructure.Supabase))

## Documentation

For more information, see:
- [Class Library Benefits](../../docs/class-library-benefits.md)
- [Kdx.Contracts Documentation](../Kdx.Contracts/README.md)
- [Kdx.Infrastructure.Supabase Documentation](../Kdx.Infrastructure.Supabase/README.md)

## License

This template is provided as-is for use with Kdx packages.

## Support

For issues or questions:
- GitHub Issues: [kdxprojects/issues](https://github.com/your-org/kdxprojects/issues)
- Documentation: [docs/](../../docs/)

## Version History

### 1.0.0 (Initial Release)
- ASP.NET Core 8.0 Razor Pages template
- Integration with Kdx.Contracts v2.0.0-alpha
- Integration with Kdx.Infrastructure.Supabase v2.0.0-alpha
- Sample Companies page
- Supabase configuration setup
