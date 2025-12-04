# KdxProjects 更新クイックガイド

## 🚀 クイックスタート（最も一般的なケース）

### バグフィックス（2.0.0 → 2.0.1）

```bash
# 1. KdxProjectsで変更を加える
cd kdxprojects
# コードを編集...

# 2. バージョン番号を更新
# Directory.Build.props または各.csprojファイルの<Version>タグを更新

# 3. CHANGELOG.mdを編集
# CHANGELOG.md を開いて変更内容を追加

# 4. コミット & プッシュ
git add .
git commit -m "fix: [変更内容の説明]"
git push origin master

# 5. タグ作成 & プッシュ（GitHub Actionsが自動的にNuGetパッケージを公開）
git tag -a v2.0.1 -m "Release v2.0.1"
git push origin v2.0.1

# 6. GitHub Releaseが自動作成される
# NuGet.orgにパッケージが自動公開される

# 7. 使用側プロジェクトでパッケージ更新
dotnet add package Kdx.Contracts --version 2.0.1
dotnet add package Kdx.Infrastructure.Supabase --version 2.0.1
```

## 📋 標準的な更新フロー（Issue → PR → リリース）

### 1. Issue作成
```bash
# GitHubでIssueを作成
# テンプレート: Bug Report または Feature Request
# ラベル: bug, feature, enhancement など
```

### 2. ブランチ作成
```bash
git checkout master
git pull origin master
git checkout -b fix/123-bug-description  # または feature/123-feature-name
```

### 3. 開発 & コミット
```bash
# コードを変更
# テスト追加・実行
dotnet test

# コミット（Conventional Commits形式）
git commit -m "fix(contracts): resolve null reference in GetCompanyAsync

Closes #123"
```

### 4. Pull Request作成
```bash
git push origin fix/123-bug-description

# GitHubでPull Request作成
# PRテンプレートに従って記入
# レビュアーを指定
```

### 5. コードレビュー
- レビュアーがコードをレビュー
- フィードバックに対応
- Approve取得

### 6. マージ & リリース
```bash
# PRがmasterにマージされる（Squash and Merge推奨）

# バージョンタグを作成してプッシュ
git checkout master
git pull origin master
git tag -a v2.0.1 -m "Release v2.0.1"
git push origin v2.0.1

# GitHub Actionsが自動実行:
# - ビルド
# - テスト
# - NuGetパッケージ作成
# - NuGet.orgに公開
# - GitHub Releaseを作成
```

## 📊 バージョニングチートシート

| 変更タイプ | バージョン | Issue例 | タグ例 |
|-----------|----------|---------|--------|
| 🐛 バグ修正 | `2.0.0` → `2.0.1` | `[BUG] Null reference in GetCompanyAsync` | `git tag -a v2.0.1 -m "Release v2.0.1"` |
| ✨ 新機能（互換） | `2.0.1` → `2.1.0` | `[FEATURE] Add pagination support` | `git tag -a v2.1.0 -m "Release v2.1.0"` |
| 💥 破壊的変更 | `2.1.0` → `3.0.0` | `[BREAKING] Change repository interface` | `git tag -a v3.0.0 -m "Release v3.0.0"` |
| 🧪 ベータ版 | `2.1.0` → `2.1.1-beta` | `[FEATURE] Experimental feature` | `git tag -a v2.1.1-beta -m "Release v2.1.1-beta"` |
| 🧪 アルファ版 | `2.0.0` → `2.0.1-alpha` | `[WIP] Work in progress` | `git tag -a v2.0.1-alpha -m "Release v2.0.1-alpha"` |

**⚠️ 重要**: タグには必ず`v`プレフィックスとハイフン`-`を使用してください。
- ✅ 正しい: `v2.1.1-beta`, `v2.1.1-alpha.1`, `v2.1.1-rc.1`
- ❌ 間違い: `v2.1.1beta`, `v2.1.1_beta`, `2.1.1-beta` (vなし)

## 🎯 よくあるシナリオ

### シナリオ1: 単一パッケージのみ変更

```bash
# Issue作成: [BUG] Fix null reference in Kdx.Contracts
# ブランチ作成
git checkout -b fix/125-null-reference-contracts

# Kdx.Contractsのみ変更
# 全パッケージのバージョンを統一して更新（推奨）
# src/Kdx.Contracts/Kdx.Contracts.csproj の<Version>を更新
# src/Kdx.Infrastructure.Supabase/Kdx.Infrastructure.Supabase.csproj も同じバージョンに

# PR作成 → レビュー → マージ → タグ作成
```

**重要**: 1つのパッケージだけ変更しても、関連パッケージは同じバージョンに統一することを推奨します。

### シナリオ2: 新しいメソッド追加（後方互換）

```bash
# 1. Issue作成: [FEATURE] Add GetCompanyByIdAsync method
# 2. ブランチ作成
git checkout -b feature/126-add-company-getbyid

# 3. 新しいメソッドを追加
# src/Kdx.Contracts/ISupabaseRepository.cs に新メソッド追加
# src/Kdx.Infrastructure.Supabase/Repositories/SupabaseRepository.cs に実装

# 4. バージョンをマイナーアップ（2.0.0 → 2.1.0）
# 5. PR作成 → レビュー → マージ
# 6. タグ作成: v2.1.0

# 7. 使用側プロジェクトで更新
dotnet add package Kdx.Contracts --version 2.1.0
# 既存コードはそのまま動作（後方互換性あり）
```

### シナリオ3: インターフェース変更（破壊的）

```bash
# 1. Issue作成: [BREAKING] Change repository method signatures
# 2. ブランチ作成
git checkout -b feature/127-breaking-repository-change

# 3. インターフェースを変更
# src/Kdx.Infrastructure.Supabase/Repositories/ISupabaseRepository.cs を編集

# 4. バージョンをメジャーアップ（2.1.0 → 3.0.0）
# 5. CHANGELOG.mdに破壊的変更を明記
# 6. Migration Guideを作成

# 7. PR作成（破壊的変更を明記）→ レビュー → マージ
# 8. タグ作成: v3.0.0

# 9. 使用側プロジェクトで更新前にコード修正が必要
# - Migration Guideに従ってコード更新
# - その後パッケージ更新
dotnet add package Kdx.Contracts --version 3.0.0
```

## 🔧 バージョン更新手順

### .csprojファイルの更新

```xml
<!-- src/Kdx.Contracts/Kdx.Contracts.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <Version>2.0.1</Version>  <!-- ここを更新 -->
    <PackageReleaseNotes>
      - Fix null reference in GetCompanyAsync
      - Update documentation
    </PackageReleaseNotes>
  </PropertyGroup>
</Project>
```

### ビルド確認

```bash
# ローカルでビルド
dotnet clean
dotnet restore
dotnet build -c Release

# テスト実行
dotnet test

# パッケージ生成確認
ls src/Kdx.Contracts/bin/Release/*.nupkg
```

## ⚠️ トラブルシューティング

### エラー: パッケージが見つからない（NuGet.org）

```bash
# 解決策1: パッケージが公開されているか確認
# https://www.nuget.org/packages/Kdx.Contracts/

# 解決策2: NuGetキャッシュをクリア
dotnet nuget locals all --clear
dotnet restore

# 解決策3: 明示的にバージョン指定
dotnet add package Kdx.Contracts --version 2.0.1
```

### エラー: ビルド失敗（破壊的変更後）

```bash
# 1. CHANGELOG.mdで破壊的変更を確認
cat CHANGELOG.md | grep -A 10 "Breaking"

# 2. Migration Guideを確認
cat MIGRATION-GUIDE-*.md

# 3. エラーメッセージから変更箇所を特定
# 例: "メソッド 'GetProcessDetails' が見つかりません"

# 4. コードを修正
# Migration Guideに従ってコード更新

# 5. 再ビルド
dotnet build -c Release
```

### GitHub Actionsが失敗する

```bash
# 1. GitHub Actionsのログを確認
# https://github.com/kanamori-system-inc/kdxprojects/actions

# 2. ローカルで同じビルドを実行
dotnet build -c Release
dotnet test

# 3. .csprojのバージョンが正しいか確認
grep "<Version>" src/*/*.csproj

# 4. タグが正しいか確認
git tag -l
# タグ形式: v2.0.1 （vプレフィックス必須、ハイフン必須）
```

### エラー: "PackageVersion string specified 'X.X.Xbeta' is invalid"

**原因**: タグのバージョン形式が間違っている（ハイフンなし）

**解決策**:
```bash
# 間違ったタグを削除
git tag -d v2.1.1beta
git push origin :refs/tags/v2.1.1beta

# 正しい形式でタグを再作成
git tag -a v2.1.1-beta -m "Release v2.1.1-beta"
git push origin v2.1.1-beta
```

**正しいタグ形式**:
- ✅ `v2.1.1` - 正式リリース
- ✅ `v2.1.1-beta` - ベータ版
- ✅ `v2.1.1-alpha` - アルファ版
- ✅ `v2.1.1-rc.1` - リリース候補
- ❌ `v2.1.1beta` - 間違い（ハイフンなし）
- ❌ `v2.1.1_beta` - 間違い（アンダースコア）

## 📝 CHANGELOG.md テンプレート

### パッチリリース（2.0.0 → 2.0.1）

```markdown
## [2.0.1] - 2025-10-15

### Fixed
- Fix null reference exception in GetCompanyAsync (#125)
- Resolve connection timeout issue in Supabase client (#126)

### Changed
- Update error messages for better clarity
```

### マイナーリリース（2.0.1 → 2.1.0）

```markdown
## [2.1.0] - 2025-10-20

### Added
- Add GetCompanyByIdAsync method to ISupabaseRepository (#130)
- Add pagination support for GetCompaniesAsync (#131)

### Changed
- Improve query performance for large datasets

### Fixed
- Fix cache invalidation issue (#132)
```

### メジャーリリース（2.1.0 → 3.0.0）

```markdown
## [3.0.0] - 2025-11-01

### Breaking Changes
⚠️ **このバージョンには破壊的変更が含まれています**

- Change return type of AddAsync methods from Task to Task<int> (#140)
  - 変更前: `Task AddCompanyAsync(Company company)`
  - 変更後: `Task<int> AddCompanyAsync(Company company)`
- Remove deprecated GetProcessDetails method (#141)
  - 代替: `GetProcessDetailsAsync`を使用

### Migration Guide
1. AddAsync メソッドの戻り値を受け取るように変更
   ```csharp
   // 変更前
   await repository.AddCompanyAsync(company);

   // 変更後
   var newId = await repository.AddCompanyAsync(company);
   ```
2. GetProcessDetails を GetProcessDetailsAsync に置換
3. ビルドしてコンパイルエラーを確認

### Added
- Add transaction support for batch operations

詳細: [MIGRATION-GUIDE-3.0.md](MIGRATION-GUIDE-3.0.md)
```

## ✅ リリースチェックリスト

### 開発フェーズ
- [ ] Issue作成
- [ ] ブランチ作成（feature/*, fix/*）
- [ ] コード変更完了
- [ ] テスト追加・更新
- [ ] ローカルビルド成功
- [ ] ローカルテスト通過

### Pull Requestフェーズ
- [ ] .csprojのバージョン更新
- [ ] CHANGELOG.md更新
- [ ] 破壊的変更がある場合、Migration Guide作成
- [ ] PR作成（テンプレート使用）
- [ ] コードレビュー依頼
- [ ] レビューフィードバック対応
- [ ] Approve取得

### リリースフェーズ
- [ ] PRをmasterにマージ
- [ ] masterブランチを最新化
- [ ] リリースタグ作成（v2.0.1形式）
- [ ] タグをプッシュ
- [ ] GitHub Actions成功確認
- [ ] NuGet.orgで公開確認
- [ ] GitHub Release確認

### 使用側プロジェクト更新
- [ ] パッケージバージョン更新
- [ ] 破壊的変更がある場合、コード修正
- [ ] ビルド成功
- [ ] テスト通過
- [ ] 動作確認

## 🔗 関連ドキュメント

より詳しい情報は以下を参照：
- **[CONTRIBUTING.md](CONTRIBUTING.md)** - 貢献ガイド（クイックリファレンス）
- **[docs/contribution-guide.md](docs/contribution-guide.md)** - 詳細な開発フロー
- **[CHANGELOG.md](CHANGELOG.md)** - 変更履歴
- **[README.md](README.md)** - プロジェクト概要

---

**最終更新**: 2025-10-03
