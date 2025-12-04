# Kdx Web Template - クイックスタートガイド

## 概要

Kdx Web TemplateはKdxパッケージ（Kdx.Contracts、Kdx.Infrastructure.Supabase）を使用したASP.NET Core Razor Pagesアプリケーションのテンプレートです。このテンプレートをForkして、独自のWebアプリケーション開発をすぐに開始できます。

## プロジェクト情報

- **フレームワーク**: ASP.NET Core 8.0 (Razor Pages)
- **使用パッケージ**:
  - Kdx.Contracts v2.0.0-alpha
  - Kdx.Infrastructure.Supabase v2.0.0-alpha
- **パターン**: Repository Pattern with Dependency Injection

## セットアップ手順

### 1. リポジトリのFork/クローン

```bash
# HTTPSでクローン
git clone https://github.com/kanamori-system-inc/kdxprojects.git

# またはSSHでクローン
git clone git@github.com:kanamori-system-inc/kdxprojects.git

# プロジェクトディレクトリに移動
cd kdxprojects/src/Kdx.Web.Template
```

### 2. Supabase設定の構成

#### Supabase認証情報の取得

1. [Supabase Dashboard](https://app.supabase.com)にログイン
2. プロジェクトを選択
3. Settings → API に移動
4. 以下をコピー:
   - **Project URL**: `https://xxxxx.supabase.co`
   - **anon/public key**: `eyJhbGci...`

#### 設定ファイルの更新

`appsettings.Development.json`を編集:

```json
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "Key": "your-anon-key-here"
  }
}
```

本番環境用は`appsettings.json`を更新してください。

### 3. アプリケーションの実行

#### コマンドラインから

```bash
# パッケージの復元
dotnet restore

# アプリケーションの実行
dotnet run
```

ブラウザで `https://localhost:5001` にアクセスします。

#### Visual Studioから

1. `Kdx.Web.Template.csproj`を開く
2. F5キーを押して実行
3. ブラウザが自動的に開きます

#### Visual Studio Codeから

1. プロジェクトフォルダを開く
2. F5キーを押して実行（または`dotnet run`をターミナルで実行）

## プロジェクト構成

```
Kdx.Web.Template/
│
├── Pages/                              # Razor Pages
│   ├── Companies.cshtml                # サンプルページ（企業一覧）
│   ├── Companies.cshtml.cs             # PageModel（リポジトリ使用例）
│   ├── Index.cshtml                    # ホームページ
│   ├── Privacy.cshtml                  # プライバシーページ
│   └── Shared/
│       ├── _Layout.cshtml              # 共通レイアウト
│       └── _ValidationScriptsPartial.cshtml
│
├── wwwroot/                            # 静的ファイル
│   ├── css/
│   ├── js/
│   └── lib/
│
├── appsettings.json                    # アプリケーション設定
├── appsettings.Development.json        # 開発環境設定
├── Program.cs                          # エントリーポイント
├── .gitignore                          # Git除外設定
└── README.md                           # プロジェクトREADME
```

## 主な機能

### 1. 依存性注入の設定

`Program.cs`でSupabaseクライアントとリポジトリが設定されています:

```csharp
// Supabaseクライアントの設定
var supabaseUrl = builder.Configuration["Supabase:Url"]
    ?? throw new InvalidOperationException("Supabase URL not configured");
var supabaseKey = builder.Configuration["Supabase:Key"]
    ?? throw new InvalidOperationException("Supabase Key not configured");

var options = new SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true
};

// サービスの登録
builder.Services.AddSingleton(provider => new Client(supabaseUrl, supabaseKey, options));
builder.Services.AddScoped<ISupabaseRepository, SupabaseRepository>();
```

### 2. リポジトリの使用例（Companies Page）

`Pages/Companies.cshtml.cs`:

```csharp
public class CompaniesModel : PageModel
{
    private readonly ISupabaseRepository _repository;
    private readonly ILogger<CompaniesModel> _logger;

    public List<Company>? Companies { get; set; }
    public string? ErrorMessage { get; set; }

    public CompaniesModel(ISupabaseRepository repository, ILogger<CompaniesModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            _logger.LogInformation("Fetching companies from Supabase...");
            Companies = await _repository.GetCompaniesAsync();
            _logger.LogInformation("Successfully fetched {Count} companies", Companies?.Count ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching companies");
            ErrorMessage = $"Failed to fetch companies: {ex.Message}";
        }
    }
}
```

### 3. ビューでのデータ表示

`Pages/Companies.cshtml`:

```cshtml
@page
@model Kdx.Web.Template.Pages.CompaniesModel

<div class="container mt-4">
    @if (Model.Companies != null && Model.Companies.Any())
    {
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Company Name</th>
                    <th>Created At</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var company in Model.Companies)
                {
                    <tr>
                        <td>@company.Id</td>
                        <td>@company.CompanyName</td>
                        <td>@company.CreatedAt</td>
                    </tr>
                }
            </tbody>
        </table>
    }
    else
    {
        <div class="alert alert-info">
            <p>No companies found.</p>
        </div>
    }
</div>
```

## カスタマイズガイド

### 新しいページの追加

1. **Razorページの作成**

```bash
# Pages/フォルダに新しいページを追加
# 例: Models.cshtml と Models.cshtml.cs
```

2. **PageModelの実装**

```csharp
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kdx.Web.Template.Pages
{
    public class ModelsModel : PageModel
    {
        private readonly ISupabaseRepository _repository;

        public ModelsModel(ISupabaseRepository repository)
        {
            _repository = repository;
        }

        public List<Model>? Models { get; set; }

        public async Task OnGetAsync()
        {
            Models = await _repository.GetModelsAsync();
        }
    }
}
```

3. **ナビゲーションリンクの追加**

`Pages/Shared/_Layout.cshtml`を編集:

```cshtml
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-page="/Models">Models</a>
</li>
```

### データソースの変更

Supabaseから別のデータソースに変更する場合:

1. 新しいインフラストラクチャライブラリを作成（例: `Kdx.Infrastructure.SqlServer`）
2. `ISupabaseRepository`インターフェースを実装
3. `Program.cs`の依存性注入を更新:

```csharp
// Supabaseの代わりにSQL Serverを使用
builder.Services.AddScoped<ISupabaseRepository, SqlServerRepository>();
```

アプリケーション層のコードは変更不要です。

## 利用可能なリポジトリメソッド

### Company（企業）操作
- `GetCompaniesAsync()` - 全企業取得
- `GetCompanyByIdAsync(int id)` - ID指定で企業取得
- `AddCompanyAsync(Company company)` - 企業追加
- `UpdateCompanyAsync(Company company)` - 企業更新
- `DeleteCompanyAsync(int id)` - 企業削除

### Model（モデル）操作
- `GetModelsAsync()` - 全モデル取得
- `GetModelByIdAsync(int id)` - ID指定でモデル取得
- `AddModelAsync(Model model)` - モデル追加
- `UpdateModelAsync(Model model)` - モデル更新
- `DeleteModelAsync(int id)` - モデル削除

### PLC操作
- `GetPLCsAsync()` - 全PLC取得
- `GetPLCByIdAsync(int id)` - ID指定でPLC取得
- その他CRUD操作

### Machine（機械）操作
- `GetMachinesAsync()` - 全機械取得
- `GetMachineByIdAsync(int id)` - ID指定で機械取得
- その他CRUD操作

その他多数のメソッドが利用可能です。詳細は`ISupabaseRepository.cs`を参照してください。

## トラブルシューティング

### エラー: "Supabase URL not configured"

**原因**: Supabase設定が`appsettings.json`に存在しない

**解決方法**:
```json
{
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "Key": "your-anon-key-here"
  }
}
```

### データベース接続エラー

**確認事項**:
1. Supabase URLとKeyが正しいか確認
2. Supabaseプロジェクトが起動しているか確認
3. データベーステーブルが存在し、DTOと一致しているか確認
4. ネットワーク接続を確認

### ビルドエラー

**解決方法**:
1. `dotnet restore`を実行してNuGetパッケージを復元
2. .NET 8.0 SDK以降がインストールされているか確認
3. 参照プロジェクトが正常にビルドされているか確認

```bash
# クリーンビルド
dotnet clean
dotnet restore
dotnet build
```

### 404 Not Found（ページが見つからない）

**確認事項**:
1. Razorページのルーティングが正しいか確認
2. `@page`ディレクティブがページの先頭にあるか確認
3. ファイル名が正しいか確認（例: `Companies.cshtml`）

## デプロイメント

### Azure App Serviceへのデプロイ

1. Azure App Serviceリソースを作成
2. アプリケーション設定にSupabase認証情報を追加
3. Visual StudioまたはAzure CLIでデプロイ

```bash
# Azure CLIでのデプロイ例
az webapp up --name your-app-name --resource-group your-rg
```

### Dockerコンテナとしてデプロイ

Dockerfileを作成:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Kdx.Web.Template.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kdx.Web.Template.dll"]
```

## ベストプラクティス

### 1. エラーハンドリング

```csharp
public async Task OnGetAsync()
{
    try
    {
        Companies = await _repository.GetCompaniesAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching companies");
        ErrorMessage = "データの取得中にエラーが発生しました";
    }
}
```

### 2. ロギング

```csharp
_logger.LogInformation("Fetching data from repository");
_logger.LogWarning("No data found for query: {Query}", query);
_logger.LogError(ex, "Database error occurred");
```

### 3. 環境別設定

- 開発環境: `appsettings.Development.json`
- ステージング環境: `appsettings.Staging.json`
- 本番環境: `appsettings.Production.json`

### 4. セキュリティ

- Supabase Keyは環境変数または Azure Key Vaultに保存
- `.gitignore`で機密情報を除外
- HTTPS使用を強制

## 関連リンク

- [Kdx.Contracts パッケージ](../Kdx.Contracts/)
- [Kdx.Infrastructure.Supabase パッケージ](../Kdx.Infrastructure.Supabase/)
- [クラスライブラリ化のメリット](../../docs/class-library-benefits.md)
- [ASP.NET Core ドキュメント](https://learn.microsoft.com/aspnet/core/)
- [Supabase ドキュメント](https://supabase.com/docs)

## サポート

問題や質問がある場合:
- GitHubイシュー: [kdxprojects/issues](https://github.com/kanamori-system-inc/kdxprojects/issues)
- ドキュメント: [docs/](../../docs/)

## バージョン履歴

### 1.0.0（初回リリース）
- ASP.NET Core 8.0 Razor Pagesテンプレート
- Kdx.Contracts v2.0.0-alpha統合
- Kdx.Infrastructure.Supabase v2.0.0-alpha統合
- Companiesサンプルページ
- Supabase設定セットアップ
- 依存性注入の構成
