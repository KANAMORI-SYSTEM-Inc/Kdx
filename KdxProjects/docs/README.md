# Kdx クラスライブラリ ドキュメント

このディレクトリには、Kdxクラスライブラリプロジェクトの包括的なドキュメントが含まれています。

## 📚 目次

### 開発者向けガイド

#### 貢献・開発フロー
- **[貢献ガイド](contribution-guide.md)** ⭐
  - Git運用方針（ブランチ戦略、コミット規約）
  - Issue運用ルール（テンプレート、ラベル体系）
  - Pull Request手順（レビュープロセス、マージ条件）
  - コーディング規約
  - バージョニングポリシー
  - リリースプロセス

#### クイックリファレンス
- **[CONTRIBUTING.md](../CONTRIBUTING.md)** - 英語版クイックガイド

### アーキテクチャ・設計

- **[クラスライブラリ化のメリット](class-library-benefits.md)**
  - 再利用性、保守性、テスト容易性の向上
  - チーム開発の効率化
  - アーキテクチャの柔軟性
  - 現在のソリューション構成
  - ベストプラクティス

### 使用方法ガイド

- **[Webテンプレートガイド](web-template-guide.md)**
  - Kdx.Web.Templateの使い方
  - セットアップ手順
  - カスタマイズ方法
  - トラブルシューティング
  - デプロイメント

- **[NuGetパッケージガイド](nuget-packages-guide.md)**
  - パッケージのインストール方法
  - 基本的な使用例
  - 各パッケージの詳細

## 🚀 クイックスタート

### 新規貢献者の方

1. **[貢献ガイド](contribution-guide.md)** を読む
2. **[CONTRIBUTING.md](../CONTRIBUTING.md)** でワークフローを確認
3. GitHub Issueテンプレートを使用して Issue を作成
4. ブランチを作成して開発開始

### Kdxパッケージを使いたい方

1. **[NuGetパッケージガイド](nuget-packages-guide.md)** でNuGet.orgからのインストール方法を確認
2. **[Webテンプレートガイド](web-template-guide.md)** でテンプレートの使い方を学ぶ
3. `src/Kdx.Web.Template/` をForkして開発開始

### アーキテクチャを理解したい方

1. **[クラスライブラリ化のメリット](class-library-benefits.md)** で設計思想を理解
2. 各パッケージのREADMEを確認
   - [Kdx.Contracts](../src/Kdx.Contracts/README.md)
   - [Kdx.Infrastructure.Supabase](../src/Kdx.Infrastructure.Supabase/README.md)

## 📋 Issue・Pull Requestテンプレート

プロジェクトには以下のテンプレートが用意されています：

### Issueテンプレート
- **[機能リクエスト](../.github/ISSUE_TEMPLATE/feature_request.md)** - 新機能の提案
- **[バグレポート](../.github/ISSUE_TEMPLATE/bug_report.md)** - バグの報告
- **[破壊的変更](../.github/ISSUE_TEMPLATE/breaking_change.md)** - 破壊的変更の提案

### Pull Requestテンプレート
- **[PRテンプレート](../.github/PULL_REQUEST_TEMPLATE.md)** - Pull Request作成時のテンプレート

## 🔄 開発フロー概要

```
1. Issue作成
   ↓
2. ブランチ作成 (feature/*, fix/*, docs/*)
   ↓
3. 開発・コミット (Conventional Commits)
   ↓
4. Pull Request作成
   ↓
5. コードレビュー
   ↓
6. マージ (Squash and Merge推奨)
   ↓
7. リリース（必要に応じて）
```

## 📦 パッケージ構成

```
kdxprojects/
├── src/
│   ├── Kdx.Contracts/              # 契約層（DTO、インターフェース）
│   ├── Kdx.Infrastructure.Supabase/ # Supabaseデータアクセス実装
│   └── Kdx.Web.Template/           # Fork用Webアプリテンプレート
├── docs/                           # このディレクトリ
├── .github/
│   ├── ISSUE_TEMPLATE/             # Issueテンプレート
│   ├── PULL_REQUEST_TEMPLATE.md    # PRテンプレート
│   └── workflows/                  # CI/CD設定
└── CONTRIBUTING.md                 # 貢献ガイド（英語）
```

## 🏷️ バージョニング

Semantic Versioning（セマンティックバージョニング）を採用：

```
MAJOR.MINOR.PATCH-PRERELEASE

例: 2.0.0-alpha → 2.0.0 → 2.1.0 → 2.1.1
```

- **MAJOR**: 破壊的変更
- **MINOR**: 後方互換性のある機能追加
- **PATCH**: 後方互換性のあるバグ修正
- **PRERELEASE**: alpha, beta, rc

## 🔖 Git ブランチ戦略

### メインブランチ
- `master` - 本番リリース用（直接コミット禁止）

### 開発ブランチ
- `feature/<issue>-<description>` - 新機能
- `fix/<issue>-<description>` - バグ修正
- `docs/<issue>-<description>` - ドキュメント
- `refactor/<issue>-<description>` - リファクタリング
- `hotfix/<version>-<description>` - 緊急修正

## 💡 コミットメッセージ規約

Conventional Commitsに従う：

```
<type>(<scope>): <subject>

例:
feat(contracts): add Company CRUD methods
fix(infrastructure): resolve null reference error
docs: update contribution guide
```

**Type:**
- `feat`: 新機能
- `fix`: バグ修正
- `docs`: ドキュメント
- `style`: フォーマット
- `refactor`: リファクタリング
- `perf`: パフォーマンス改善
- `test`: テスト
- `chore`: ビルド・ツール

## 🧪 テスト

### テストカバレッジ目標
- **最低ライン**: 70%
- **推奨**: 80%以上
- **クリティカルパス**: 100%

### テスト実行

```bash
# すべてのテストを実行
dotnet test

# カバレッジ測定
dotnet test /p:CollectCoverage=true
```

## 🚀 リリースプロセス

1. バージョン番号決定（SemVer）
2. `.csproj`ファイル更新
3. `CHANGELOG.md`更新
4. リリースブランチ作成
5. ビルド＆テスト
6. Pull Request作成
7. マージ＆タグ付け
8. GitHub Release作成
9. NuGetパッケージ公開

詳細は[貢献ガイド - リリースプロセス](contribution-guide.md#リリースプロセス)を参照。

## 🆘 サポート

### 質問・問題
- **GitHub Discussions**: 一般的な質問
- **GitHub Issues**: バグ報告、機能リクエスト
- **Slack**: 緊急の問い合わせ（チーム内）

### よくある質問

**Q: Issueを作らずにPRを出せますか？**
A: 小さな修正（タイポ、ドキュメント修正）は可能ですが、機能追加やバグ修正は必ずIssueを作成してください。

**Q: 破壊的変更を含む場合は？**
A: `BREAKING CHANGE:`をコミットメッセージに含め、メジャーバージョンを上げ、マイグレーションガイドを作成してください。

詳細は[貢献ガイド - よくある質問](contribution-guide.md#よくある質問)を参照。

## 📖 参考リンク

### 外部リソース
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [GitHub Flow](https://guides.github.com/introduction/flow/)
- [Keep a Changelog](https://keepachangelog.com/)

### プロジェクト内リンク
- [メインREADME](../README.md)
- [パッケージ更新ガイド](../QUICK-UPDATE-GUIDE.md)
- [GitHub Actions設定](../.github/workflows/)

## 📝 ドキュメント更新履歴

| 日付 | 内容 |
|------|------|
| 2025-10-03 | 初版作成 - 貢献ガイド、Webテンプレートガイド、クラスライブラリメリット文書化 |

---

**メンテナンス**: このドキュメントは定期的に更新されます。質問や改善提案があれば、Issueを作成してください。
