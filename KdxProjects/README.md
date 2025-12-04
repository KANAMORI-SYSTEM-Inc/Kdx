# KdxProjects

NuGetパッケージ群として提供されるKDXシステムのコアライブラリ。

## パッケージ一覧

| パッケージ名 | バージョン | 説明 |
|-------------|----------|------|
| **Kdx.Contracts** | 2.0.1 | DTOとインターフェースの定義 |
| **Kdx.Core** | 2.0.1 | ビジネスロジックとアプリケーションサービス |
| **Kdx.Infrastructure** | 2.0.1 | インフラストラクチャサービスの実装 |
| **Kdx.Infrastructure.Supabase** | 2.0.1 | Supabase固有のリポジトリ実装 |

## アーキテクチャ

```
Kdx.Infrastructure.Supabase
  └─ Kdx.Core
      └─ Kdx.Contracts
  └─ supabase-csharp (0.16.2)
  └─ postgrest-csharp (3.5.1)

Kdx.Infrastructure
  └─ Kdx.Core
      └─ Kdx.Contracts
  └─ Kdx.Infrastructure.Supabase

```

## ビルド

```bash
# 依存関係の復元
dotnet restore

# デバッグビルド
dotnet build

# リリースビルド（NuGetパッケージ生成）
dotnet build -c Release

# テスト実行
dotnet test
```

## NuGetパッケージの使用

### インストール

```bash
# NuGet.orgから最新版をインストール
dotnet add package Kdx.Contracts --version 2.0.0-alpha
dotnet add package Kdx.Infrastructure.Supabase --version 2.0.0-alpha
```

### Webアプリテンプレート（推奨）

```bash
# Kdx.Web.Templateを使用
cd src/Kdx.Web.Template
dotnet run
```

詳細は以下を参照:
- **[docs/web-template-guide.md](docs/web-template-guide.md)** - Webテンプレート使用ガイド
- **[docs/nuget-packages-guide.md](docs/nuget-packages-guide.md)** - パッケージ詳細ガイド

## 開発

### 必要な環境

- .NET 8.0 SDK
- Visual Studio 2022 または VS Code

### プロジェクト構造

```
KdxProjects/
├── src/
│   ├── Kdx.Contracts/          # DTOとインターフェース
│   ├── Kdx.Core/                # ビジネスロジック
│   ├── Kdx.Infrastructure/      # インフラストラクチャ実装
│   ├── Kdx.Infrastructure.Supabase/  # Supabase実装
├── tests/                       # テストプロジェクト
│   └── Kdx.Infrastructure.Supabase.Tests/  # 統合テスト
└── docs/                        # ドキュメント
```

### テスト

統合テストが用意されており、Supabaseクエリの正常動作を確認できます。

```bash
# すべてのテストを実行
dotnet test

# 詳細出力で実行
dotnet test --logger "console;verbosity=detailed"
```

テストの詳細は **[tests/Kdx.Infrastructure.Supabase.Tests/README.md](tests/Kdx.Infrastructure.Supabase.Tests/README.md)** を参照してください。

### バージョニング

このプロジェクトは **Semantic Versioning (SemVer)** を採用しています。

- **MAJOR**: 破壊的変更
- **MINOR**: 後方互換性のある機能追加
- **PATCH**: 後方互換性のあるバグフィックス

### パッケージ更新

KdxProjectsパッケージの更新と公開方法:

**🚀 クイックスタート:**
```bash
# 1. Issue作成 → ブランチ作成 → 開発
# 2. Pull Request → コードレビュー → マージ
# 3. リリースタグ作成
git tag -a v2.0.1 -m "Release v2.0.1"
git push origin v2.0.1

# GitHub Actionsが自動的に:
# - NuGetパッケージをビルド
# - NuGet.orgに公開
# - GitHub Releaseを作成
```

**📚 詳細ガイド:**
- **[QUICK-UPDATE-GUIDE.md](QUICK-UPDATE-GUIDE.md)** - 更新手順のクイックリファレンス
- **[CONTRIBUTING.md](CONTRIBUTING.md)** - 貢献ガイド（英語）
- **[docs/contribution-guide.md](docs/contribution-guide.md)** - 詳細な開発フロー

## ライセンス

MIT License

## 関連リポジトリ

- **KdxDesigner**: WPFデスクトップアプリケーション（このパッケージの利用者）
  - https://github.com/KANAMORI-SYSTEM-Inc/KdxDesigner

## 貢献

プルリクエストは歓迎します。大きな変更の場合は、まずissueを開いて変更内容について議論してください。

### 貢献ガイド

- **[CONTRIBUTING.md](CONTRIBUTING.md)** - 貢献の手順（クイックリファレンス）
- **[貢献ガイド詳細](docs/contribution-guide.md)** - 詳細な開発フロー、Issue運用、PR手順

### 主要ドキュメント

- **[クラスライブラリ化のメリット](docs/class-library-benefits.md)** - アーキテクチャ設計の背景
- **[Webテンプレートガイド](docs/web-template-guide.md)** - Fork用Webアプリの使い方
- **[NuGetパッケージガイド](docs/nuget-packages-guide.md)** - パッケージ使用方法

---

**作成日**: 2025-10-02
**ステータス**: Active Development
