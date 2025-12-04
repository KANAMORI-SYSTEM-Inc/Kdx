# KDX Projects リポジトリ分離戦略

## 概要

現在の単一リポジトリ（モノリポ）を、以下の2つの独立したリポジトリに分離します：

1. **KdxProjects** - NuGetパッケージ群（ライブラリ）
2. **KdxDesigner** - WPFデスクトップアプリケーション（クライアント）

## リポジトリ分離の目的

### ビジネス上のメリット
- ✅ **独立したリリースサイクル**: ライブラリとアプリケーションを個別にバージョン管理
- ✅ **明確な責任分離**: コアロジックとUIの開発を分離
- ✅ **スケーラビリティ**: 将来的に他のクライアントアプリケーション追加が容易
- ✅ **セキュリティ**: ライブラリとアプリケーションで異なるアクセス権限を設定可能

### 技術上のメリット
- ✅ **ビルド時間の短縮**: 変更があったリポジトリのみビルド
- ✅ **依存関係の明確化**: NuGetパッケージ経由の明示的な依存関係
- ✅ **CI/CDの最適化**: 各リポジトリに特化したパイプライン
- ✅ **コードレビューの効率化**: 変更範囲が限定される

## 新しいリポジトリ構造

### Repository 1: KdxProjects (ライブラリ)

**URL**: `https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects`

```
KdxProjects/
├── .github/
│   └── workflows/
│       ├── build.yml          # ビルド・テストワークフロー
│       └── publish.yml        # NuGetパッケージ公開ワークフロー
├── src/
│   ├── Kdx.Contracts/
│   ├── Kdx.Core/
│   ├── Kdx.Infrastructure/
│   ├── Kdx.Infrastructure.Supabase/
│   └── Kdx.Contracts.ViewModels/
├── tests/
│   ├── Kdx.Core.Tests/
│   ├── Kdx.Infrastructure.Tests/
│   └── Kdx.Infrastructure.Supabase.Tests/
├── docs/
│   ├── api/                   # APIドキュメント
│   ├── architecture/          # アーキテクチャドキュメント
│   └── nuget-packages-guide.md
├── .gitignore
├── README.md
├── CHANGELOG.md
├── KdxProjects.sln
└── Directory.Build.props       # 共通ビルドプロパティ
```

**役割**:
- NuGetパッケージの開発・管理
- コアビジネスロジック
- データアクセス層
- 共通インターフェース

**リリース成果物**:
- NuGetパッケージ（GitHub Packages / Azure Artifacts）

### Repository 2: KdxDesigner (アプリケーション)

**URL**: `https://github.com/KANAMORI-SYSTEM-Inc/KdxDesigner`

```
KdxDesigner/
├── .github/
│   └── workflows/
│       ├── build.yml          # ビルドワークフロー
│       └── release.yml        # アプリケーションリリースワークフロー
├── src/
│   ├── KdxDesigner/           # WPFアプリケーション
│   │   ├── Views/
│   │   ├── ViewModels/
│   │   ├── Models/
│   │   ├── Services/
│   │   ├── Utils/
│   │   └── Resources/
│   └── KdxDesigner.Installer/ # インストーラープロジェクト（将来的）
├── tests/
│   └── KdxDesigner.Tests/
├── docs/
│   ├── user-guide/            # ユーザーマニュアル
│   └── development/           # 開発者ドキュメント
├── .gitignore
├── README.md
├── CHANGELOG.md
├── nuget.config               # NuGetソース設定
└── KdxDesigner.sln
```

**役割**:
- WPFデスクトップアプリケーション
- UI/UX
- ユーザー向け機能
- KdxProjects NuGetパッケージの消費

**リリース成果物**:
- Windows実行ファイル（.exe）
- MSIインストーラー（将来的）

## バージョニング戦略

### KdxProjects（ライブラリ）

**セマンティックバージョニング** (SemVer 2.0.0) を採用

- `MAJOR.MINOR.PATCH`
  - **MAJOR**: 破壊的変更
  - **MINOR**: 後方互換性のある機能追加
  - **PATCH**: 後方互換性のあるバグフィックス

**例**:
- `1.0.0` - 初回リリース
- `1.1.0` - 新機能追加
- `1.1.1` - バグフィックス
- `2.0.0` - 破壊的変更

### KdxDesigner（アプリケーション）

**CalVer** (Calendar Versioning) を採用

- `YYYY.MM.PATCH`
  - **YYYY**: 年
  - **MM**: 月
  - **PATCH**: その月のパッチ番号

**例**:
- `2025.10.0` - 2025年10月の最初のリリース
- `2025.10.1` - 2025年10月の1回目のパッチ

## 依存関係管理

### KdxDesigner → KdxProjects

KdxDesignerは、KdxProjectsのNuGetパッケージを参照：

```xml
<ItemGroup>
  <PackageReference Include="Kdx.Contracts" Version="1.0.0" />
  <PackageReference Include="Kdx.Core" Version="1.0.0" />
  <PackageReference Include="Kdx.Infrastructure" Version="1.0.0" />
  <PackageReference Include="Kdx.Infrastructure.Supabase" Version="1.0.0" />
  <PackageReference Include="Kdx.Contracts.ViewModels" Version="1.0.0" />
</ItemGroup>
```

### バージョン更新戦略

1. **メジャーアップデート**: KdxProjectsの破壊的変更時
   - KdxDesignerのコード修正が必要
   - 計画的に実施

2. **マイナーアップデート**: 新機能追加時
   - KdxDesignerは必要に応じて新機能を使用
   - 後方互換性あり

3. **パッチアップデート**: バグフィックス時
   - すぐに最新版に更新推奨

## CI/CDパイプライン

### KdxProjects CI/CD

**ビルド・テストワークフロー** (`.github/workflows/build.yml`):
```yaml
trigger:
  - main
  - develop
  - feature/*

steps:
  1. チェックアウト
  2. .NET 8.0セットアップ
  3. 依存関係の復元
  4. ビルド
  5. 単体テスト実行
  6. コードカバレッジレポート生成
```

**NuGet公開ワークフロー** (`.github/workflows/publish.yml`):
```yaml
trigger:
  - tags: 'v*'  # v1.0.0形式のタグ

steps:
  1. チェックアウト
  2. .NET 8.0セットアップ
  3. Releaseビルド
  4. NuGetパッケージ作成
  5. GitHub Packagesへ公開
  6. GitHubリリース作成
  7. CHANGELOG.md更新
```

### KdxDesigner CI/CD

**ビルド・テストワークフロー** (`.github/workflows/build.yml`):
```yaml
trigger:
  - main
  - develop
  - feature/*

steps:
  1. チェックアウト
  2. .NET 8.0セットアップ
  3. NuGet復元（KdxProjectsパッケージ含む）
  4. ビルド
  5. UIテスト実行
  6. 成果物のアップロード
```

**リリースワークフロー** (`.github/workflows/release.yml`):
```yaml
trigger:
  - tags: 'v*'  # v2025.10.0形式のタグ

steps:
  1. チェックアウト
  2. .NET 8.0セットアップ
  3. Releaseビルド
  4. 実行ファイルの作成
  5. コード署名
  6. GitHubリリース作成
  7. インストーラー作成（将来的）
```

## ブランチ戦略

### Git Flow採用

両リポジトリで同じブランチ戦略を採用：

```
main           # 本番環境用（リリースのみ）
  └── develop  # 開発統合ブランチ
       ├── feature/xxx  # 機能開発
       ├── bugfix/xxx   # バグフィックス
       └── hotfix/xxx   # 緊急修正
```

**ブランチ運用ルール**:
1. `main`: リリースタグのみ
2. `develop`: 開発中の最新コード
3. `feature/*`: `develop`からブランチ、`develop`へマージ
4. `hotfix/*`: `main`からブランチ、`main`と`develop`へマージ

## 移行プロセス

### フェーズ1: 準備（1日）

1. ✅ 現在のコミットを確実にプッシュ
2. ✅ 完全なバックアップ作成
3. ✅ 移行計画の最終レビュー

### フェーズ2: KdxProjects リポジトリ作成（半日）

1. 新しいGitHubリポジトリ作成
2. 必要なファイルのコピー
3. ソリューション構造の整理
4. README.md、CHANGELOG.md作成
5. CI/CD設定
6. 初回コミット

### フェーズ3: KdxDesigner リポジトリ作成（半日）

1. 新しいGitHubリポジトリ作成
2. KdxDesigner関連ファイルのコピー
3. NuGet参照への変更
4. ソリューション構造の整理
5. README.md、CHANGELOG.md作成
6. CI/CD設定
7. 初回コミット

### フェーズ4: 検証（半日）

1. 両リポジトリのビルド確認
2. NuGetパッケージの動作確認
3. CI/CDパイプラインのテスト
4. ドキュメントの最終確認

### フェーズ5: 本番移行（1時間）

1. タグ付け（KdxProjects: v1.0.0、KdxDesigner: v2025.10.0）
2. 旧リポジトリをアーカイブまたはREADMEで新リポジトリへ誘導
3. チームへのアナウンス

## リスク管理

### 潜在的リスクと対策

| リスク | 影響 | 対策 |
|--------|------|------|
| Gitヒストリーの喪失 | 中 | 旧リポジトリはアーカイブとして保持 |
| ビルドエラー | 高 | 移行前に全テスト実行、段階的移行 |
| NuGet依存関係の問題 | 中 | ローカルフィードでテスト後に実施 |
| CI/CD設定ミス | 中 | 手動ビルドで事前検証 |
| チーム混乱 | 低 | 詳細なドキュメント、移行アナウンス |

## 成功基準

- ✅ KdxProjectsがNuGetパッケージとして公開可能
- ✅ KdxDesignerがNuGetパッケージを参照して正常にビルド
- ✅ CI/CDパイプラインが両リポジトリで正常動作
- ✅ すべての既存機能が動作
- ✅ ドキュメントが完備

## ディレクトリ構造比較

### Before（現在）
```
kdx_projects/
├── Kdx.Contracts/
├── Kdx.Core/
├── Kdx.Infrastructure/
├── Kdx.Infrastructure.Supabase/
├── Kdx.Contracts.ViewModels/
├── KdxDesigner/
├── Kdx.API/              # 未使用
└── KdxMigrationAPI/      # 未使用
```

### After（移行後）

**KdxProjects/**
```
src/
├── Kdx.Contracts/
├── Kdx.Core/
├── Kdx.Infrastructure/
├── Kdx.Infrastructure.Supabase/
└── Kdx.Contracts.ViewModels/
```

**KdxDesigner/**
```
src/
└── KdxDesigner/
```

## 推奨アクション

### 即座に実施
1. ✅ KdxProjectsリポジトリ作成
2. ✅ KdxDesignerリポジトリ作成
3. ✅ CI/CD基本設定

### 短期（1-2週間）
1. ⏳ 単体テストの追加
2. ⏳ API ドキュメント生成
3. ⏳ コードカバレッジ測定

### 中期（1ヶ月）
1. ⏳ KdxDesignerインストーラー作成
2. ⏳ 自動リリースノート生成
3. ⏳ パフォーマンステスト

### 長期（3ヶ月）
1. ⏳ 他のクライアントアプリケーション開発
2. ⏳ プライベートNuGetサーバー構築
3. ⏳ セキュリティ監査

## まとめ

この戦略により、KDX Projectsは以下を達成します：

- 🎯 **明確な責任分離**: ライブラリとアプリケーションの独立
- 🎯 **効率的な開発**: 並行開発が可能
- 🎯 **柔軟なリリース**: 独立したバージョン管理
- 🎯 **スケーラビリティ**: 新しいクライアントアプリ追加が容易
- 🎯 **保守性向上**: 変更影響範囲の限定化

---

**作成日**: 2025-10-02
**最終更新**: 2025-10-02
**バージョン**: 1.0
**ステータス**: 承認待ち
