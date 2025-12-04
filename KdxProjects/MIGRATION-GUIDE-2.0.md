# KdxProjects 2.0 移行ガイド

## 概要

KdxProjects 2.0 では、`IAccessRepository` と `SupabaseRepositoryAdapter` を削除し、`ISupabaseRepository` を直接使用するように変更されました。これにより、アーキテクチャがシンプルになり、保守性が向上します。

## 破壊的変更

### 1. IAccessRepository の削除

**削除されたインターフェース:**
- `Kdx.Contracts.Interfaces.IAccessRepository`

**削除されたアダプター:**
- `Kdx.Infrastructure.Adapters.SupabaseRepositoryAdapter`

### 2. ISupabaseRepository への直接アクセス

すべてのサービスが `ISupabaseRepository` を直接使用するようになりました。

## クライアント側の移行手順

### ステップ 1: NuGetパッケージの更新

```bash
# 既存のパッケージを削除
dotnet remove package Kdx.Contracts --version 1.x.x
dotnet remove package Kdx.Core --version 1.x.x
dotnet remove package Kdx.Infrastructure --version 1.x.x
dotnet remove package Kdx.Infrastructure.Supabase --version 1.x.x
dotnet remove package Kdx.Contracts.ViewModels --version 1.x.x

# 2.0.0-alpha バージョンをインストール
dotnet add package Kdx.Contracts --version 2.0.0-alpha
dotnet add package Kdx.Core --version 2.0.0-alpha
dotnet add package Kdx.Infrastructure --version 2.0.0-alpha
dotnet add package Kdx.Infrastructure.Supabase --version 2.0.0-alpha
dotnet add package Kdx.Contracts.ViewModels --version 2.0.0-alpha
```

### ステップ 2: DI登録の変更

**変更前 (v1.x):**

```csharp
using Kdx.Contracts.Interfaces;
using Kdx.Infrastructure.Adapters;
using Kdx.Infrastructure.Supabase.Repositories;

services.AddScoped<ISupabaseRepository, SupabaseRepository>();
services.AddScoped<IAccessRepository, SupabaseRepositoryAdapter>();
```

**変更後 (v2.0):**

```csharp
using Kdx.Infrastructure.Supabase.Repositories;

services.AddScoped<ISupabaseRepository, SupabaseRepository>();
// IAccessRepository の登録は不要
```

### ステップ 3: コンストラクタの変更

**変更前 (v1.x):**

```csharp
using Kdx.Contracts.Interfaces;

public class MyService
{
    private readonly IAccessRepository _repository;

    public MyService(IAccessRepository repository)
    {
        _repository = repository;
    }
}
```

**変更後 (v2.0):**

```csharp
using Kdx.Infrastructure.Supabase.Repositories;

public class MyService
{
    private readonly ISupabaseRepository _repository;

    public MyService(ISupabaseRepository repository)
    {
        _repository = repository;
    }
}
```

### ステップ 4: メソッド呼び出しの変更

**変更前 (v1.x):**

```csharp
// 同期メソッド
var companies = _repository.GetCompanies();
var plcs = _repository.GetPLCs();
```

**変更後 (v2.0):**

**オプション A: 非同期メソッドを使用（推奨）**

```csharp
// 非同期メソッド
var companies = await _repository.GetCompaniesAsync();
var plcs = await _repository.GetPLCsAsync();
```

**オプション B: 同期コンテキストで使用する場合**

```csharp
// Task.Run().GetAwaiter().GetResult() を使用
var companies = Task.Run(async () => await _repository.GetCompaniesAsync()).GetAwaiter().GetResult();
var plcs = Task.Run(async () => await _repository.GetPLCsAsync()).GetAwaiter().GetResult();
```

### ステップ 5: using ディレクティブの更新

不要になった using を削除し、必要な using を追加します。

**削除:**

```csharp
using Kdx.Contracts.Interfaces; // IAccessRepository を使っていた場合
using Kdx.Infrastructure.Adapters; // SupabaseRepositoryAdapter を使っていた場合
```

**追加:**

```csharp
using Kdx.Infrastructure.Supabase.Repositories; // ISupabaseRepository を使う場合
```

## 移行例

### 完全な例: WPFアプリケーション

**変更前 (v1.x):**

```csharp
using Kdx.Contracts.Interfaces;
using Kdx.Infrastructure.Adapters;
using Kdx.Infrastructure.Supabase.Repositories;
using Microsoft.Extensions.DependencyInjection;

public class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // Repository登録
        services.AddScoped<ISupabaseRepository, SupabaseRepository>();
        services.AddScoped<IAccessRepository, SupabaseRepositoryAdapter>();

        // Service登録
        services.AddScoped<IProcessFlowService, ProcessFlowService>();

        var serviceProvider = services.BuildServiceProvider();
    }
}

public class MyViewModel
{
    private readonly IAccessRepository _repository;

    public MyViewModel(IAccessRepository repository)
    {
        _repository = repository;
    }

    public void LoadData()
    {
        var companies = _repository.GetCompanies();
        // データ処理...
    }
}
```

**変更後 (v2.0):**

```csharp
using Kdx.Infrastructure.Supabase.Repositories;
using Microsoft.Extensions.DependencyInjection;

public class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // Repository登録（IAccessRepository は不要）
        services.AddScoped<ISupabaseRepository, SupabaseRepository>();

        // Service登録
        services.AddScoped<IProcessFlowService, ProcessFlowService>();

        var serviceProvider = services.BuildServiceProvider();
    }
}

public class MyViewModel
{
    private readonly ISupabaseRepository _repository;

    public MyViewModel(ISupabaseRepository repository)
    {
        _repository = repository;
    }

    public async Task LoadDataAsync()
    {
        var companies = await _repository.GetCompaniesAsync();
        // データ処理...
    }

    // または同期メソッドとして実装する場合
    public void LoadData()
    {
        var companies = Task.Run(async () => await _repository.GetCompaniesAsync()).GetAwaiter().GetResult();
        // データ処理...
    }
}
```

## よくある問題と解決方法

### 問題 1: "IAccessRepository が見つかりません"

**エラー:**
```
error CS0246: 型または名前空間の名前 'IAccessRepository' が見つかりませんでした
```

**解決方法:**
`IAccessRepository` を `ISupabaseRepository` に置き換えてください。

### 問題 2: "GetXXX メソッドが見つかりません"

**エラー:**
```
error CS1061: 'ISupabaseRepository' に 'GetCompanies' の定義が含まれていません
```

**解決方法:**
非同期メソッド `GetCompaniesAsync()` を使用してください。

### 問題 3: 循環参照エラー

**エラー:**
```
error MSB4006: ターゲット依存グラフに循環する依存関係が存在します
```

**解決方法:**
Kdx.Core と Kdx.Infrastructure.Supabase の間で循環参照が発生していないか確認してください。v2.0では循環参照が解消されています。

## メソッド名対応表

主要なメソッドの変更一覧:

| v1.x (IAccessRepository) | v2.0 (ISupabaseRepository) |
|--------------------------|----------------------------|
| GetCompanies() | GetCompaniesAsync() |
| GetPLCs() | GetPLCsAsync() |
| GetCYs() | GetCYsAsync() |
| GetProcessDetails() | GetProcessDetailsAsync() |
| GetProcesses() | GetProcessesAsync() |
| GetOperations() | GetOperationsAsync() |
| GetMemories(plcId) | GetMemoriesAsync(plcId) |
| GetMemoryCategories() | GetMemoryCategoriesAsync() |
| GetProsTimeByPlcId(plcId) | GetProsTimeByPlcIdAsync(plcId) |
| GetDefinitions(category) | GetDefinitionsAsync(category) |
| GetCylinderIOs(cylinderId, plcId) | GetCylinderIOsAsync(cylinderId, plcId) |
| GetOperationIOs(operationId) | GetOperationIOsAsync(operationId) |

完全なメソッド一覧は `ISupabaseRepository` インターフェースのドキュメントを参照してください。

## アーリーアクセス版の注意事項

### バージョン: 2.0.0-alpha

- これはアーリーアクセス版（アルファ版）です
- 本番環境での使用は推奨されません
- フィードバックを歓迎します: https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/issues

### インストール方法

```bash
# GitHub Packages からインストール
dotnet add package Kdx.Contracts --version 2.0.0-alpha --source https://nuget.pkg.github.com/KANAMORI-SYSTEM-Inc/index.json
dotnet add package Kdx.Core --version 2.0.0-alpha --source https://nuget.pkg.github.com/KANAMORI-SYSTEM-Inc/index.json
dotnet add package Kdx.Infrastructure --version 2.0.0-alpha --source https://nuget.pkg.github.com/KANAMORI-SYSTEM-Inc/index.json
dotnet add package Kdx.Infrastructure.Supabase --version 2.0.0-alpha --source https://nuget.pkg.github.com/KANAMORI-SYSTEM-Inc/index.json
dotnet add package Kdx.Contracts.ViewModels --version 2.0.0-alpha --source https://nuget.pkg.github.com/KANAMORI-SYSTEM-Inc/index.json
```

### NuGet.Config の設定

プロジェクトルートまたはユーザーディレクトリに `NuGet.Config` を作成:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="github" value="https://nuget.pkg.github.com/KANAMORI-SYSTEM-Inc/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="YOUR_GITHUB_USERNAME" />
      <add key="ClearTextPassword" value="YOUR_GITHUB_PAT" />
    </github>
  </packageSourceCredentials>
</configuration>
```

## サポート

質問や問題がある場合は、以下で報告してください:
- Issues: https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/issues
- Discussions: https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/discussions

## 次のステップ

1. テスト環境で移行を実施
2. 動作確認
3. フィードバックの提供
4. 正式版 (2.0.0) のリリースを待つ
