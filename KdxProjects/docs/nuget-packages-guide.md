# KDX Projects NuGetパッケージ使用ガイド

## 概要

KDX Projectsは、NuGet.orgで公開されている以下のパッケージとして提供されています：

1. **Kdx.Contracts** - DTOとインターフェース
2. **Kdx.Infrastructure.Supabase** - Supabase固有のリポジトリ実装

## インストール

### NuGet.orgからのインストール

```bash
# 最新版をインストール
dotnet add package Kdx.Contracts
dotnet add package Kdx.Infrastructure.Supabase

# 特定バージョンをインストール
dotnet add package Kdx.Contracts --version 2.0.0-alpha
dotnet add package Kdx.Infrastructure.Supabase --version 2.0.0-alpha
```

### Visual Studioから

1. ソリューションエクスプローラーでプロジェクトを右クリック
2. 「NuGetパッケージの管理」を選択
3. 「参照」タブで「Kdx.Contracts」を検索
4. インストールをクリック

### パッケージマネージャーコンソールから

```powershell
Install-Package Kdx.Contracts -Version 2.0.0-alpha
Install-Package Kdx.Infrastructure.Supabase -Version 2.0.0-alpha
```

## クイックスタート - Webアプリテンプレート（推奨）

最も簡単な方法は、用意されているWebアプリテンプレートを使用することです：

```bash
# リポジトリをクローン
git clone https://github.com/kanamori-system-inc/kdxprojects.git
cd kdxprojects/src/Kdx.Web.Template

# Supabase設定を更新
# appsettings.json を編集してSupabase認証情報を設定

# 実行
dotnet run
```

詳細は **[Webテンプレートガイド](web-template-guide.md)** を参照してください。

## 新しいプロジェクトでの使用方法

### ASP.NET Core Webアプリケーション

```bash
# 新しいWebアプリの作成
dotnet new webapp -n MyKdxApp
cd MyKdxApp

# NuGetパッケージの追加
dotnet add package Kdx.Contracts --version 2.0.0-alpha
dotnet add package Kdx.Infrastructure.Supabase --version 2.0.0-alpha
```

### プロジェクトファイルの例

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Kdx.Contracts" Version="2.0.0-alpha" />
    <PackageReference Include="Kdx.Infrastructure.Supabase" Version="2.0.0-alpha" />
  </ItemGroup>
</Project>
```

## パッケージの依存関係

```
Kdx.Infrastructure.Supabase (v2.0.0-alpha)
  └─ supabase-csharp (0.16.2)
  └─ postgrest-csharp (3.5.1)

Kdx.Contracts (v2.0.0-alpha)
  └─ （外部依存なし）
```

**注意**: パッケージは自動的に依存関係を解決します。`Kdx.Infrastructure.Supabase`をインストールすると、`Kdx.Contracts`も自動的にインストールされます。

## 使用例

### 1. DIコンテナの設定

```csharp
using Microsoft.Extensions.DependencyInjection;
using Kdx.Contracts.Interfaces;
using Kdx.Infrastructure.Supabase.Repositories;
using Kdx.Infrastructure.Adapters;
using Kdx.Core.Application;
using Kdx.Infrastructure.Services;

var services = new ServiceCollection();

// Repository層の登録
services.AddScoped<ISupabaseRepository, SupabaseRepository>();

// Service層の登録
services.AddScoped<IProcessFlowService, ProcessFlowService>();
services.AddScoped<IInterlockValidationService, InterlockValidationService>();

var serviceProvider = services.BuildServiceProvider();
```

### 2. リポジトリの使用

```csharp
using Kdx.Infrastructure.Supabase.Repositories;
using Kdx.Contracts.DTOs;

public class MyService
{
    private readonly ISupabaseRepository _repository;

    public MyService(ISupabaseRepository repository)
    {
        _repository = repository;
    }

    public async Task ProcessDataAsync()
    {
        var companies = await _repository.GetCompaniesAsync();
        // ビジネスロジックの実装...
    }
}
```

### 3. ビジネスサービスの使用

```csharp
using Kdx.Core.Application;

public class MyViewModel
{
    private readonly IProcessFlowService _processFlowService;
    private readonly IInterlockValidationService _validationService;

    public MyViewModel(
        IProcessFlowService processFlowService,
        IInterlockValidationService validationService)
    {
        _processFlowService = processFlowService;
        _validationService = validationService;
    }

    public void LoadProcessFlow(int cycleId)
    {
        var processDetails = _processFlowService.GetProcessDetailsByCycle(cycleId);
        var connections = _processFlowService.GetConnections(cycleId);

        // ビジネスロジックの実装...
    }
}
```

## パッケージの更新

### プロジェクトでのパッケージ更新

```bash
# 最新版に更新
dotnet add package Kdx.Contracts
dotnet add package Kdx.Infrastructure.Supabase

# 特定バージョンに更新
dotnet add package Kdx.Contracts --version 2.0.1
dotnet add package Kdx.Infrastructure.Supabase --version 2.0.1
```

### Visual Studioから更新

1. ソリューションエクスプローラーでプロジェクトを右クリック
2. 「NuGetパッケージの管理」を選択
3. 「更新」タブで更新可能なパッケージを確認
4. 「更新」をクリック

## トラブルシューティング

### パッケージが見つからない（NuGet.org）

```bash
# 1. NuGet.orgで公開されているか確認
# https://www.nuget.org/packages/Kdx.Contracts/

# 2. キャッシュをクリア
dotnet nuget locals all --clear

# 3. パッケージを再インストール
dotnet restore

# 4. 明示的にバージョン指定
dotnet add package Kdx.Contracts --version 2.0.0-alpha
```

### 依存関係の競合

```bash
# パッケージの依存関係を確認
dotnet list package --include-transitive

# 特定バージョンを明示的に指定
dotnet add package Kdx.Contracts --version 2.0.1
dotnet add package Kdx.Infrastructure.Supabase --version 2.0.1
```

**推奨**: すべてのKdxパッケージは同じバージョンを使用してください。

### ビルドエラー（破壊的変更後）

```bash
# 1. CHANGELOG.mdで破壊的変更を確認
curl https://raw.githubusercontent.com/kanamori-system-inc/kdxprojects/master/CHANGELOG.md

# 2. Migration Guideを確認
# リポジトリのMIGRATION-GUIDE-*.mdを参照

# 3. コードを修正してから再ビルド
dotnet build
```

## CI/CD統合

### GitHub Actionsでの使用

```yaml
# .github/workflows/build.yml
name: Build

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Test
        run: dotnet test --no-build
```

## まとめ

KDX ProjectsのNuGetパッケージ化により、以下のメリットが得られます：

- ✅ NuGet.orgからの簡単なインストール
- ✅ バージョン管理が容易
- ✅ 他のプロジェクトからの再利用が可能
- ✅ 依存関係の自動解決
- ✅ GitHub Actionsによる自動公開
- ✅ Webアプリテンプレートで即座に開始可能

## 参考資料

- **[Webテンプレートガイド](web-template-guide.md)** - Kdx.Web.Templateの使い方
- **[貢献ガイド](contribution-guide.md)** - パッケージ開発への貢献方法
- **[QUICK-UPDATE-GUIDE.md](../QUICK-UPDATE-GUIDE.md)** - パッケージ更新手順
- [NuGet Documentation](https://docs.microsoft.com/en-us/nuget/)
- [ASP.NET Core Dependency Injection](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
