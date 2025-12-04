# Kdx Monorepo

KANAMORI SYSTEM Inc.のKdxプロジェクト統合リポジトリ

## 📁 プロジェクト構成

```
Kdx/
├── src/
│   ├── KdxProjects/           # NuGetパッケージライブラリ
│   │   ├── Kdx.Contracts/
│   │   ├── Kdx.Core/
│   │   ├── Kdx.Infrastructure/
│   │   └── Kdx.Infrastructure.Supabase/
│   └── KdxDesigner/          # WPFアプリケーション
├── tests/
│   └── Kdx.Infrastructure.Supabase.Tests/
├── build/                    # 共通ビルド設定
├── .github/workflows/        # CI/CD設定
└── Kdx.sln                   # 統合ソリューション
```

## 🚀 クイックスタート

### 必要な環境

- .NET 8.0 SDK
- Visual Studio 2022 または VS Code
- Git

### ビルド

```bash
# すべてのプロジェクトをビルド
dotnet build Kdx.sln -c Debug -p:UseLocalProjects=true

# KdxDesignerのみビルド
dotnet build src/KdxDesigner/KdxDesigner.csproj -c Debug -p:UseLocalProjects=true

# テスト実行
dotnet test Kdx.sln
```

### VS Codeでのデバッグ

1. VS Codeでリポジトリを開く
2. F5キーを押すか、「Run and Debug」から「KdxDesigner (Debug - Local Projects)」を選択
3. デバッグモードでは自動的にローカルプロジェクト参照が使用されます

## 🔧 開発ワークフロー

### Debugモード（開発時）

- **プロジェクト参照**: `UseLocalProjects=true`
- KdxProjectsライブラリの変更が即座に反映
- ブレークポイントがライブラリコードで使用可能

```bash
dotnet build -c Debug -p:UseLocalProjects=true
```

### Releaseモード（本番環境）

- **NuGetパッケージ参照**: `UseLocalProjects=false`
- 公開されたNuGetパッケージを使用
- 本番環境と同じ依存関係

```bash
dotnet build -c Release -p:UseLocalProjects=false
```

## 📦 NuGetパッケージ発行

### ローカルでのパッケージビルド

```bash
dotnet pack Kdx.sln -c Release -o ./artifacts/packages
```

### 自動発行（GitHub Actions）

バージョンタグをプッシュすると自動的にNuGet.orgに発行されます：

```bash
git tag -a v3.2.5 -m "Release v3.2.5"
git push origin v3.2.5
```

## 🏗️ プロジェクト詳細

### KdxProjects（ライブラリ）

独立したNuGetパッケージ群で、他のプロジェクトでも使用可能：

- **Kdx.Contracts**: DTOとインターフェース
- **Kdx.Core**: ビジネスロジック
- **Kdx.Infrastructure**: インフラストラクチャサービス
- **Kdx.Infrastructure.Supabase**: Supabase実装

### KdxDesigner（アプリケーション）

PLCラダープログラム生成用WPFアプリケーション

- .NET 8.0 WPF
- MVVM パターン
- Microsoft Access データベース連携

## 🔄 CI/CD

### GitHub Actions ワークフロー

1. **CI (`ci.yml`)**: PR/push時のビルド・テスト
2. **NuGet発行 (`publish-nuget.yml`)**: `v*.*.*`タグでNuGet.org発行
3. **Designer リリース (`release-designer.yml`)**: `designer-v*.*.*`タグでアプリリリース

### 必要なシークレット

- `NUGET_API_KEY`: NuGet.org APIキー

## 📝 貢献ガイド

詳細は`CLAUDE.md`を参照してください。

## 📄 ライセンス

MIT License - Copyright © KANAMORI SYSTEM Inc.
