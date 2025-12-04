# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

This is a monorepo for KANAMORI SYSTEM Inc. containing two main projects:

1. **KdxDesigner** - WPF desktop application for PLC ladder program generation
2. **KdxProjects** - NuGet package libraries for KDX systems

## Build and Development Commands

### KdxDesigner (.NET 8.0 WPF Application)

```bash
# Navigate to KdxDesigner
cd KdxDesigner

# Build the project
dotnet build

# Run the application
dotnet run

# Clean build artifacts
dotnet clean

# Restore NuGet packages
dotnet restore
```

### KdxProjects (.NET 8.0 Class Libraries)

```bash
# Navigate to KdxProjects
cd KdxProjects

# Restore dependencies
dotnet restore

# Debug build
dotnet build

# Release build (generates NuGet packages)
dotnet build -c Release

# Run tests
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"
```

**NuGet Package Publishing:**
```bash
# Create release tag (triggers automated publishing via GitHub Actions)
git tag -a v2.0.1 -m "Release v2.0.1"
git push origin v2.0.1
```

## Architecture Overview

### KdxDesigner Architecture

**Technology Stack:**
- .NET 8.0, WPF
- MVVM with CommunityToolkit.Mvvm
- Microsoft Access database via OleDb
- Dapper for data access

**Core Layers:**
1. **Data Layer** (`Services/Access/`)
   - `IAccessRepository` interface
   - `AccessRepository` implementation for Access database queries

2. **Models** (`Models/`)
   - Database entities with composite key support
   - MnemonicId values: 1=Process, 2=ProcessDetail, 3=Operation, 4=CY

3. **Services** (`Services/`)
   - `MnemonicDeviceService` - Device management
   - `MnemonicTimerDeviceService` - Timer device management
   - `IOAddressService` - IO address resolution
   - `ErrorService` - Error handling

4. **ViewModels** (`ViewModels/`)
   - Uses `ObservableObject` and `[ObservableProperty]`
   - Commands use `[RelayCommand]`

5. **Views** (`Views/`)
   - Entry point: `MainView.xaml`

**Key Design Patterns:**
- **Composite Primary Keys**: Many tables use (Field1, Field2) as composite keys
- **Intermediate Tables**: CylinderIO, OperationIO for many-to-many relationships
- **Graceful Error Handling**: Returns empty collections when tables don't exist
- **Process Flow Connections**: ProcessDetailConnection (normal), ProcessDetailFinish (finish conditions)

**Database Considerations:**
- Tables may not exist until features are used
- Always check table existence before operations
- See `docs/` folder for migration scripts

### KdxProjects Architecture

**Technology Stack:**
- .NET 8.0 class libraries
- Supabase/PostgreSQL integration
- NuGet package distribution

**Package Structure:**
```
Kdx.Infrastructure.Supabase (v2.0.1)
  └─ Kdx.Core (v2.0.1)
      └─ Kdx.Contracts (v2.0.1)
  └─ supabase-csharp (0.16.2)
  └─ postgrest-csharp (3.5.1)

Kdx.Infrastructure (v2.0.1)
  └─ Kdx.Core
  └─ Kdx.Infrastructure.Supabase
```

**Packages:**
- **Kdx.Contracts**: DTOs and interfaces
- **Kdx.Core**: Business logic and application services
- **Kdx.Infrastructure**: Infrastructure service implementations
- **Kdx.Infrastructure.Supabase**: Supabase-specific repository implementations

**Testing:**
- Integration tests in `tests/Kdx.Infrastructure.Supabase.Tests/`
- Tests verify Supabase query functionality

**Versioning:**
- Follows Semantic Versioning (SemVer)
- MAJOR: Breaking changes
- MINOR: Backward-compatible features
- PATCH: Backward-compatible bug fixes

## Development Guidelines

### KdxDesigner

**Working with Composite Keys:**
- Use Dapper parameters in correct order matching composite key definition
- Include all key parts in WHERE clauses

**Adding New Features:**
1. Create View (.xaml) and code-behind in `Views/`
2. Create ViewModel in `ViewModels/` using `ObservableObject`
3. Add navigation command in `MainViewModel`
4. Add menu item in `MainView.xaml`

**UI Conventions:**
- Node height in process flow: 40px
- Use `ICollectionView` for filtering large datasets
- Modal dialogs receive repository and parent ViewModel in constructor

**Documentation Requirements:**
Create documentation for major changes in `docs/`:
- Chat history: `docs/chat-history-[feature]-[date].md`
- Wiki summary: `docs/wiki-[feature].md`

### KdxProjects

**Adding New Packages:**
1. Create project in `src/` with proper dependencies
2. Set `GeneratePackageOnBuild` to true in .csproj
3. Add README.md for package documentation
4. Update version numbers across all packages when making changes

**Publishing Flow:**
1. Create Issue → Create branch → Develop
2. Create Pull Request → Code review → Merge
3. Create release tag: `git tag -a v2.0.1 -m "Release v2.0.1"`
4. Push tag: `git push origin v2.0.1`
5. GitHub Actions automatically builds and publishes to NuGet.org

**Testing:**
- Always run `dotnet test` before creating PR
- Integration tests require Supabase connection

## Common Workflows

### Working Across Projects

This monorepo contains independent projects. Navigate to the appropriate directory before running commands:

```bash
# For KdxDesigner
cd KdxDesigner
dotnet build

# For KdxProjects
cd KdxProjects
dotnet build -c Release
```

### Building Everything

```bash
# Build .NET solutions
cd KdxDesigner && dotnet build && cd ..
cd KdxProjects && dotnet build && cd ..
```

## Important Notes

**KdxDesigner:**
- このプロジェクトでは、KdxProjectsで提供されるNuGetパッケージのクラスとSupabaseデータベースを元に、それをPLCで動作するCSVファイル形式のニモニックプログラムに変換することを目的としています
- コードにはコメントを入れ、適切な変数名・関数名を使用
- クラス化やメソッド化でコードの可読性を高める
- DRY原則に従いコードの重複を避ける

**KdxProjects:**
- パッケージ間の依存関係を常に意識する
- Breaking changesはメジャーバージョンを上げる
- 全パッケージのバージョンを同期して更新する
