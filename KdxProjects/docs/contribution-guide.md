# Kdx クラスライブラリ 貢献ガイド

## 目次

- [概要](#概要)
- [開発フロー](#開発フロー)
- [Issue運用方針](#issue運用方針)
- [Pull Request手順](#pull-request手順)
- [コーディング規約](#コーディング規約)
- [テストガイドライン](#テストガイドライン)
- [バージョニングポリシー](#バージョニングポリシー)
- [リリースプロセス](#リリースプロセス)

## 概要

このドキュメントは、Kdxクラスライブラリ（Kdx.Contracts、Kdx.Infrastructure.Supabase）への貢献方法を定義します。すべての貢献者は、このガイドラインに従ってください。

## 開発フロー

### 基本的なワークフロー

```
1. Issue作成
   ↓
2. ブランチ作成
   ↓
3. 開発・コミット
   ↓
4. Pull Request作成
   ↓
5. コードレビュー
   ↓
6. マージ
   ↓
7. リリース（必要に応じて）
```

### ブランチ戦略

#### メインブランチ

- **master** - 本番リリース用の安定ブランチ
  - 常にリリース可能な状態を維持
  - 直接コミット禁止
  - Pull Requestのみマージ可能

#### 開発ブランチ

- **develop** - 開発統合用ブランチ（オプション）
  - 次回リリース用の機能統合
  - masterへのマージ前の最終テスト

#### フィーチャーブランチ

機能開発用のブランチ。命名規則：

```
feature/<issue-number>-<short-description>
例: feature/123-add-company-crud
    feature/124-update-repository-interface
```

#### バグフィックスブランチ

バグ修正用のブランチ。命名規則：

```
fix/<issue-number>-<short-description>
例: fix/125-null-reference-error
    fix/126-connection-timeout
```

#### ホットフィックスブランチ

本番環境の緊急修正用。命名規則：

```
hotfix/<version>-<short-description>
例: hotfix/2.0.1-critical-security-fix
```

#### リファクタリングブランチ

コードリファクタリング用。命名規則：

```
refactor/<issue-number>-<short-description>
例: refactor/127-repository-pattern
```

#### ドキュメントブランチ

ドキュメント更新用。命名規則：

```
docs/<issue-number>-<short-description>
例: docs/128-update-readme
```

## Issue運用方針

### Issueテンプレート

#### 機能追加リクエスト

```markdown
## 概要
[新機能の概要を簡潔に説明]

## 背景・目的
[なぜこの機能が必要か]

## 詳細
[実装したい機能の詳細]

## 受け入れ基準
- [ ] 条件1
- [ ] 条件2
- [ ] ドキュメント更新
- [ ] テスト追加

## 技術的考慮事項
[パフォーマンス、セキュリティ、互換性など]

## 優先度
- [ ] 高
- [ ] 中
- [ ] 低
```

#### バグレポート

```markdown
## バグの概要
[バグの簡潔な説明]

## 再現手順
1. ステップ1
2. ステップ2
3. ステップ3

## 期待される動作
[本来どうあるべきか]

## 実際の動作
[実際に何が起こるか]

## 環境
- OS: [例: Windows 11]
- .NET バージョン: [例: 8.0]
- パッケージバージョン: [例: Kdx.Contracts 2.0.0-alpha]

## エラーログ・スクリーンショット
```
[ログやスクリーンショットを添付]
```

## 影響範囲
- [ ] クリティカル（本番環境停止）
- [ ] 高（主要機能に影響）
- [ ] 中（一部機能に影響）
- [ ] 低（軽微な問題）
```

#### リファクタリング提案

```markdown
## 対象コード
[リファクタリング対象のコード・ファイル]

## 現状の問題点
[なぜリファクタリングが必要か]

## 提案する改善策
[どのように改善するか]

## 期待される効果
- パフォーマンス向上
- 可読性向上
- 保守性向上
- その他

## 影響範囲
[変更による影響範囲]

## 破壊的変更
- [ ] あり
- [ ] なし
```

### Issueラベル体系

#### 種類ラベル
- `feature` - 新機能
- `bug` - バグ修正
- `enhancement` - 既存機能の改善
- `refactor` - リファクタリング
- `docs` - ドキュメント
- `test` - テスト関連
- `ci/cd` - CI/CD関連

#### 優先度ラベル
- `priority: critical` - 緊急対応が必要
- `priority: high` - 優先度高
- `priority: medium` - 優先度中
- `priority: low` - 優先度低

#### ステータスラベル
- `status: triage` - トリアージ待ち
- `status: accepted` - 承認済み
- `status: in progress` - 作業中
- `status: blocked` - ブロック中
- `status: review` - レビュー待ち

#### 対象パッケージラベル
- `pkg: contracts` - Kdx.Contracts関連
- `pkg: infrastructure` - Kdx.Infrastructure.Supabase関連
- `pkg: all` - 複数パッケージに影響

#### その他ラベル
- `breaking-change` - 破壊的変更を含む
- `good first issue` - 初心者向け
- `help wanted` - 協力者募集
- `duplicate` - 重複Issue
- `wontfix` - 対応しない

### Issue管理プロセス

#### 1. Issue作成時
- 適切なテンプレートを使用
- わかりやすいタイトルを付ける
- 必要な情報をすべて記載

#### 2. トリアージ
- メンテナーが`status: triage`ラベルを付与
- 週次トリアージミーティングで優先度を決定
- 承認されたIssueに`status: accepted`ラベルを付与

#### 3. アサイン
- 担当者を決定
- `status: in progress`ラベルに変更
- 対応ブランチを作成

#### 4. 完了
- Pull Requestでクローズ
- Issue番号をコミットメッセージに含める

## Pull Request手順

### 1. 準備

#### ブランチの作成

```bash
# 最新のmasterブランチを取得
git checkout master
git pull origin master

# 新しいブランチを作成
git checkout -b feature/123-add-company-crud
```

### 2. 開発

#### コミットメッセージ規約

Conventional Commitsに従う：

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Type（種類）:**
- `feat`: 新機能
- `fix`: バグ修正
- `docs`: ドキュメントのみの変更
- `style`: コードの意味に影響しない変更（空白、フォーマットなど）
- `refactor`: バグ修正や機能追加を含まないコード変更
- `perf`: パフォーマンス改善
- `test`: テストの追加や修正
- `chore`: ビルドプロセスやツールの変更

**例:**

```bash
# 新機能追加
git commit -m "feat(contracts): add Company CRUD methods to ISupabaseRepository

- Add GetCompanyByIdAsync method
- Add AddCompanyAsync method
- Add UpdateCompanyAsync method
- Add DeleteCompanyAsync method

Closes #123"

# バグ修正
git commit -m "fix(infrastructure): resolve null reference in GetCompaniesAsync

Fixed NullReferenceException when response.Models is null

Closes #125"

# 破壊的変更
git commit -m "feat(contracts): change return type of AddAsync methods

BREAKING CHANGE: AddAsync methods now return Task<int> instead of Task

Closes #130"
```

#### コミット粒度
- 1つのコミットで1つの論理的な変更
- 意味のある単位でコミット
- 大きすぎる変更は避ける

### 3. Pull Request作成

#### PRテンプレート

```markdown
## 概要
[このPRが何をするか簡潔に説明]

## 関連Issue
Closes #123

## 変更内容
- [ ] 変更1
- [ ] 変更2
- [ ] 変更3

## 変更の種類
- [ ] 新機能 (feature)
- [ ] バグ修正 (fix)
- [ ] 破壊的変更 (breaking change)
- [ ] ドキュメント更新 (docs)
- [ ] リファクタリング (refactor)
- [ ] パフォーマンス改善 (perf)
- [ ] テスト追加 (test)

## チェックリスト
- [ ] コードがコーディング規約に従っている
- [ ] 自己レビューを実施した
- [ ] コメントを追加した（特に複雑な部分）
- [ ] ドキュメントを更新した
- [ ] 変更により新しい警告が発生していない
- [ ] テストを追加/更新した
- [ ] すべてのテストが通過している
- [ ] ビルドが成功している

## テスト方法
[この変更をテストする方法]

## スクリーンショット（該当する場合）
[スクリーンショットを添付]

## パフォーマンスへの影響
[パフォーマンスに影響がある場合、詳細を記載]

## セキュリティへの影響
[セキュリティに影響がある場合、詳細を記載]

## 追加コンテキスト
[その他の重要な情報]
```

#### PRのタイトル規約

```
<type>(<scope>): <short summary> (#issue-number)

例:
feat(contracts): add Company CRUD methods (#123)
fix(infrastructure): resolve null reference error (#125)
docs: update contribution guide (#128)
```

### 4. コードレビュー

#### レビュアーの責任

**必須チェック項目:**
- [ ] コードがコーディング規約に従っている
- [ ] ロジックが正しい
- [ ] テストが適切にカバーされている
- [ ] パフォーマンスへの影響が考慮されている
- [ ] セキュリティリスクがない
- [ ] ドキュメントが更新されている
- [ ] 破壊的変更が適切に文書化されている

**レビューコメントのガイドライン:**

```markdown
# 必須修正
**Must Fix:** ここはnullチェックが必要です

# 推奨修正
**Should Fix:** この部分はリファクタリングした方が良いです

# 提案
**Nit:** 変数名を`data`より`companyData`の方が明確です

# 質問
**Question:** この処理の意図を教えてください

# 承認
**LGTM** (Looks Good To Me): コードレビュー完了、問題なし
```

#### 作成者の責任

- レビューコメントに48時間以内に対応
- 質問には明確に回答
- 修正後は再レビューを依頼

### 5. マージ

#### マージ条件

- [ ] 最低1名のApprove取得
- [ ] すべてのコメントが解決済み
- [ ] CIが成功
- [ ] コンフリクトが解消されている
- [ ] masterブランチの最新を取り込んでいる

#### マージ方法

**Squash and Merge（推奨）:**
- 複数コミットを1つにまとめる
- コミット履歴が整理される

```bash
# GitHubのUI上で「Squash and merge」を選択
```

**Rebase and Merge:**
- 各コミットを保持したままマージ
- 履歴が直線的になる

**Merge Commit:**
- マージコミットを作成
- ブランチの履歴を保持

### 6. ブランチのクリーンアップ

```bash
# マージ後、ローカルブランチを削除
git checkout master
git pull origin master
git branch -d feature/123-add-company-crud

# リモートブランチは自動削除（GitHub設定で有効化）
```

## コーディング規約

### C# コーディングスタイル

#### 命名規則

```csharp
// クラス名: PascalCase
public class SupabaseRepository { }

// インターフェース名: IPascalCase
public interface ISupabaseRepository { }

// メソッド名: PascalCase
public async Task<Company> GetCompanyByIdAsync(int id) { }

// プロパティ名: PascalCase
public string CompanyName { get; set; }

// プライベートフィールド: _camelCase
private readonly Client _supabaseClient;

// ローカル変数: camelCase
var companyData = await GetData();

// 定数: UPPER_CASE
private const int MAX_RETRY_COUNT = 3;
```

#### XMLドキュメントコメント

```csharp
/// <summary>
/// 指定されたIDの会社情報を取得します
/// </summary>
/// <param name="id">会社ID</param>
/// <returns>会社情報。見つからない場合はnull</returns>
/// <exception cref="ArgumentException">IDが0以下の場合</exception>
public async Task<Company?> GetCompanyByIdAsync(int id)
{
    if (id <= 0)
        throw new ArgumentException("ID must be greater than 0", nameof(id));

    // Implementation
}
```

#### 非同期メソッド

```csharp
// メソッド名に必ずAsyncサフィックスを付ける
public async Task<List<Company>> GetCompaniesAsync()
{
    // async/awaitを使用
    var response = await _supabaseClient
        .From<CompanyEntity>()
        .Get();

    return response.Models.Select(e => e.ToDto()).ToList();
}
```

#### エラーハンドリング

```csharp
public async Task<Company?> GetCompanyByIdAsync(int id)
{
    try
    {
        var response = await _supabaseClient
            .From<CompanyEntity>()
            .Where(c => c.Id == id)
            .Single();

        return response?.ToDto();
    }
    catch (Exception ex)
    {
        // ログ出力
        _logger.LogError(ex, "Failed to get company with ID: {CompanyId}", id);
        throw; // 再スロー
    }
}
```

### プロジェクト構成規約

#### ファイル配置

```
Kdx.Contracts/
├── DTOs/              # データ転送オブジェクト
│   ├── Company.cs
│   └── Model.cs
└── ISupabaseRepository.cs

Kdx.Infrastructure.Supabase/
├── Entities/          # Supabaseエンティティ
│   ├── CompanyEntity.cs
│   └── ModelEntity.cs
└── Repositories/      # リポジトリ実装
    ├── ISupabaseRepository.cs
    ├── SupabaseRepository.cs
    └── SupabaseRepository.NewMethods.cs  # Partialファイル
```

#### Partial クラスの使用

```csharp
// SupabaseRepository.cs - メインファイル
public partial class SupabaseRepository : ISupabaseRepository
{
    private readonly Client _supabaseClient;

    // 既存実装
}

// SupabaseRepository.NewMethods.cs - 新規メソッド用
public partial class SupabaseRepository
{
    // 新規追加メソッド
    public async Task<Company?> GetCompanyByIdAsync(int id) { }
}
```

## テストガイドライン

### 統合テスト

統合テストは実際のSupabaseデータベースに対してクエリを実行し、WHERE句の正常動作やCRUD操作を検証します。

詳細は **[tests/Kdx.Infrastructure.Supabase.Tests/README.md](../tests/Kdx.Infrastructure.Supabase.Tests/README.md)** を参照してください。

```bash
# すべてのテストを実行
dotnet test

# 特定のテストを実行
dotnet test --filter FullyQualifiedName~SupabaseRepositoryTests
```

### テスト作成のベストプラクティス

#### Arrange-Act-Assertパターン

```csharp
[Fact]
public async Task GetCylinderIOsAsync_WithMultipleConditions_ShouldNotThrowParseError()
{
    // Arrange - テストデータを準備
    int testCylinderId = 1;
    int testPlcId = 1;

    // Act - メソッドを実行
    var exception = await Record.ExceptionAsync(async () =>
    {
        var cylinderIOs = await _repository.GetCylinderIOsAsync(testCylinderId, testPlcId);
        Assert.NotNull(cylinderIOs);
    });

    // Assert - 結果を検証
    Assert.Null(exception);
}
```

#### テストデータの命名規則

```csharp
var testCompany = new Company
{
    CompanyName = $"Test Company {Guid.NewGuid()}", // 一意性を保証
    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
};
```

#### クリーンアップの実施

```csharp
[Fact]
public async Task AddAndDeleteCompany_ShouldSucceed()
{
    // Arrange & Act - 追加
    var newId = await _repository.AddCompanyAsync(testCompany);

    // Cleanup - 削除
    await _repository.DeleteCompanyAsync(newId);
}
```

### テストカバレッジ目標

- **最低ライン**: 70%
- **推奨**: 80%以上
- **クリティカルパス**: 100%

### 新機能追加時のテスト要件

新しいリポジトリメソッドを追加する際は、必ず以下のテストを含めてください:

1. **パーサーエラー検証**: 複数条件のWHERE句が正常に動作すること
2. **CRUD操作検証**: データの作成・取得・更新・削除が正常に機能すること
3. **エッジケース検証**: 空データ、存在しないID等の境界値テスト

## バージョニングポリシー

### セマンティックバージョニング

```
MAJOR.MINOR.PATCH-PRERELEASE

例: 2.0.0-alpha
    2.0.0
    2.1.0
    2.1.1
```

#### バージョン番号の変更ルール

**MAJOR（メジャー）バージョン:**
- 破壊的変更を含む
- 既存コードが動作しなくなる可能性がある
- 例: `1.x.x` → `2.0.0`

**MINOR（マイナー）バージョン:**
- 後方互換性のある機能追加
- 既存機能の拡張
- 例: `2.0.x` → `2.1.0`

**PATCH（パッチ）バージョン:**
- 後方互換性のあるバグ修正
- 小さな改善
- 例: `2.1.0` → `2.1.1`

**PRERELEASE（プレリリース）:**
- alpha: 初期開発版
- beta: テスト版
- rc: リリース候補版
- 例: `2.0.0-alpha`, `2.0.0-beta.1`, `2.0.0-rc.1`

### バージョン更新例

```xml
<!-- Before: 破壊的変更 -->
<Version>2.0.0-alpha</Version>

<!-- After: 正式リリース -->
<Version>2.0.0</Version>

<!-- After: 機能追加 -->
<Version>2.1.0</Version>

<!-- After: バグ修正 -->
<Version>2.1.1</Version>
```

## リリースプロセス

### 1. リリース準備

#### バージョン番号の決定

```bash
# CHANGELOG.mdを確認
# 変更内容に基づいてバージョンを決定
```

#### .csprojファイルの更新

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Version>2.1.0</Version>
    <PackageReleaseNotes>
      - Add Company CRUD methods
      - Fix null reference bug in GetModelsAsync
      - Update documentation
    </PackageReleaseNotes>
  </PropertyGroup>
</Project>
```

### 2. CHANGELOG更新

```markdown
# Changelog

All notable changes to this project will be documented in this file.

## [2.1.0] - 2025-10-15

### Added
- Company CRUD methods (#123)
- Model CRUD methods (#124)

### Fixed
- Null reference error in GetModelsAsync (#125)

### Changed
- Update repository interface documentation

## [2.0.0] - 2025-10-03

### Added
- Initial release
- Supabase integration
```

### 3. リリースブランチ作成

```bash
git checkout master
git pull origin master
git checkout -b release/2.1.0
```

### 4. ビルド＆テスト

```bash
# クリーンビルド
dotnet clean
dotnet restore
dotnet build --configuration Release

# テスト実行
dotnet test

# パッケージ作成
dotnet pack --configuration Release
```

### 5. Pull Request作成

```markdown
## Release 2.1.0

### 変更内容
- Company CRUD methods追加
- バグ修正

### チェックリスト
- [x] バージョン番号更新
- [x] CHANGELOG更新
- [x] ビルド成功
- [x] テスト通過
- [x] ドキュメント更新
```

### 6. マージとタグ付け

```bash
# masterにマージ後
git checkout master
git pull origin master

# タグ作成
git tag -a v2.1.0 -m "Release version 2.1.0"
git push origin v2.1.0
```

### 7. GitHub Release作成

1. GitHubのReleaseページへ移動
2. 「Create a new release」をクリック
3. タグを選択: `v2.1.0`
4. リリースノートを記載:

```markdown
## Kdx.Contracts v2.1.0

### 新機能
- Company CRUD methods (#123)
- Model CRUD methods (#124)

### バグ修正
- Null reference error in GetModelsAsync (#125)

### ドキュメント
- Update API documentation

### インストール
```bash
dotnet add package Kdx.Contracts --version 2.1.0
```

**Full Changelog**: https://github.com/org/kdxprojects/compare/v2.0.0...v2.1.0
```

### 8. NuGetパッケージ公開

GitHub Actionsで自動公開される場合:
- タグプッシュで自動実行
- `.github/workflows/release.yml`を確認

手動公開の場合:

```bash
# NuGet.orgにプッシュ
dotnet nuget push bin/Release/Kdx.Contracts.2.1.0.nupkg \
    --api-key YOUR_API_KEY \
    --source https://api.nuget.org/v3/index.json
```

## 緊急対応（Hotfix）プロセス

### 1. Hotfixブランチ作成

```bash
git checkout master
git checkout -b hotfix/2.1.1-critical-fix
```

### 2. 修正とテスト

```bash
# 修正実装
# テスト追加
dotnet test
```

### 3. バージョン更新

```xml
<Version>2.1.1</Version>
```

### 4. Pull Request & マージ

```bash
# PRを作成し、緊急レビュー
# Approve後、即座にマージ
```

### 5. リリース

```bash
git tag -a v2.1.1 -m "Hotfix: Critical security fix"
git push origin v2.1.1
```

## よくある質問

### Q: Issueを作らずにPull Requestを出せますか？

A: 小さな修正（タイポ、ドキュメント修正など）は可能ですが、機能追加やバグ修正は必ずIssueを作成してください。

### Q: レビューに時間がかかりすぎる場合は？

A: 48時間以上経過してもレビューされない場合、Slackやメンションで催促してください。

### Q: 破壊的変更を含む場合の注意点は？

A:
1. `BREAKING CHANGE:`をコミットメッセージに含める
2. メジャーバージョンを上げる
3. マイグレーションガイドを作成
4. 十分な移行期間を設ける

### Q: テストがないコードはマージできますか？

A: 原則としてテストが必須です。ただし、ドキュメントのみの変更などは例外です。

## サポート

質問や問題がある場合:
- GitHub Discussions: 一般的な質問
- GitHub Issues: バグ報告、機能リクエスト
- Slack: 緊急の問い合わせ

## 参考リンク

- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [GitHub Flow](https://guides.github.com/introduction/flow/)
- [Keep a Changelog](https://keepachangelog.com/)
