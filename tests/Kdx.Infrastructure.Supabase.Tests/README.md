# Kdx.Infrastructure.Supabase テストガイド

## 概要

このプロジェクトは、`Kdx.Infrastructure.Supabase` パッケージの統合テストを提供します。Supabaseデータベースに対する実際のクエリを実行し、WHERE句の修正や新機能が正常に動作することを確認します。

## テストの目的

1. **クエリ構文の検証**: Postgrestのパーサーエラーを防ぐため、複雑なWHERE句が正しく動作することを確認
2. **CRUD操作の検証**: データの作成、読み取り、更新、削除が正常に機能することを確認
3. **リグレッション防止**: 既存機能が壊れていないことを確認

## セットアップ

### 1. テスト用Supabaseプロジェクトの準備

本番環境とは**別の**テスト用Supabaseプロジェクトを作成することを強く推奨します。

1. [Supabase](https://supabase.com/)でテスト用プロジェクトを作成
2. 必要なテーブルスキーマをセットアップ（本番と同じスキーマ）
3. テストデータを用意（オプション）

### 2. 設定ファイルの準備

`appsettings.test.json.template` をコピーして `appsettings.test.json` を作成:

```bash
cd tests/Kdx.Infrastructure.Supabase.Tests
cp appsettings.test.json.template appsettings.test.json
```

`appsettings.test.json` を編集してSupabase認証情報を設定:

```json
{
  "Supabase": {
    "Url": "https://your-test-project.supabase.co",
    "Key": "your-test-anon-key"
  }
}
```

**重要**: `appsettings.test.json` は `.gitignore` に含まれており、コミットされません。

### 3. 環境変数での設定（オプション）

CI/CD環境では、環境変数で設定することも可能:

```bash
export Supabase__Url="https://your-test-project.supabase.co"
export Supabase__Key="your-test-anon-key"
```

## テストの実行

### すべてのテストを実行

```bash
dotnet test
```

### 特定のテストクラスを実行

```bash
dotnet test --filter FullyQualifiedName~SupabaseRepositoryTests
```

### 特定のテストメソッドを実行

```bash
dotnet test --filter FullyQualifiedName~GetCompaniesAsync_ShouldReturnList
```

### 詳細な出力で実行

```bash
dotnet test --logger "console;verbosity=detailed"
```

## テストの種類

### 1. 基本的なCRUDテスト

```csharp
[Fact]
public async Task AddAndDeleteCompany_ShouldSucceed()
```

- データの追加、取得、削除の一連の流れをテスト
- 実際にデータベースに影響を与えるため、テスト後はクリーンアップを実施

### 2. WHERE句パーサーエラー検証テスト

```csharp
[Fact]
public async Task GetCylinderControlBoxesAsync_WithMultipleConditions_ShouldNotThrowParseError()
```

- 複数条件のWHERE句がPostgrestパーサーエラーを発生させないことを確認
- 修正前は `PGRST100` エラーが発生していたクエリをテスト

### 3. 削除操作の検証テスト

```csharp
[Fact]
public async Task DeleteInterlockIOAsync_WithThreeConditions_ShouldNotThrowParseError()
```

- 複合キーを持つテーブルの削除操作が正常に動作することを確認
- 存在しないレコードの削除でもエラーが発生しないことを検証

## テストのベストプラクティス

### 1. テストデータの命名

```csharp
var testCompany = new Company
{
    CompanyName = $"Test Company {Guid.NewGuid()}",
    // ...
};
```

- `Test` プレフィックスを使用
- GUIDを使って一意性を保証
- テスト後は必ずクリーンアップ

### 2. アサーションパターン

```csharp
// Act
var result = await _repository.SomeMethod();

// Assert
Assert.NotNull(result);
Assert.IsType<List<SomeType>>(result);
```

- Arrange-Act-Assertパターンに従う
- nullチェックと型チェックを含める

### 3. 例外テスト

```csharp
var exception = await Record.ExceptionAsync(async () =>
{
    await _repository.SomeMethod();
});

Assert.Null(exception); // 例外が発生しないことを確認
```

## トラブルシューティング

### エラー: "Supabase:Url not configured"

**原因**: `appsettings.test.json` が見つからない、または設定が不正

**解決策**:
1. `appsettings.test.json.template` をコピーして `appsettings.test.json` を作成
2. 正しいSupabase URLとキーを設定
3. ファイルがプロジェクトルートに配置されていることを確認

### エラー: "PGRST100" (Postgrest parse error)

**原因**: WHERE句の構文エラー

**解決策**:
1. 複数条件を `&&` で結合せず、チェーンされた `.Where()` 呼び出しを使用
2. 該当するリポジトリメソッドを修正

**修正前**:
```csharp
.Where(i => i.PlcId == plcId && i.IOAddress == ioAddress)
```

**修正後**:
```csharp
.Where(i => i.PlcId == plcId)
.Where(i => i.IOAddress == ioAddress)
```

### エラー: "new row violates row-level security policy"

**原因**: SupabaseのRow Level Security (RLS)が有効で、匿名キーでは書き込みができない

**解決策**:

**オプション1: RLSポリシーを設定（推奨）**
```sql
-- Supabase SQL Editorで実行
-- テスト用ポリシー（本番では使用しないこと）
CREATE POLICY "Enable insert for anon users" ON "Company"
  FOR INSERT TO anon
  WITH CHECK (true);

CREATE POLICY "Enable delete for anon users" ON "Company"
  FOR DELETE TO anon
  USING (true);
```

**オプション2: Service Role Keyを使用**
```json
{
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "Key": "your-service-role-key-here"  // anon keyの代わりにservice_role keyを使用
  }
}
```

**⚠️ 注意**: Service Role Keyは全権限を持つため、テスト環境のみで使用し、本番環境では絶対に公開しないでください。

**オプション3: テストをスキップ**

書き込みテストは `[Fact(Skip = "reason")]` でスキップされています。読み取り専用のテストは実行されます。

### エラー: "Could not find the table 'X' in the schema cache"

**原因**: テーブルがテストデータベースに存在しない

**解決策**:
1. テスト対象のテーブルスキーマを確認
2. 存在しないテーブルを使用するテストは自動的にスキップされます
3. または、テストデータベースに必要なテーブルを作成

### テストが遅い

**原因**: 実際のSupabaseサーバーとの通信

**解決策**:
- ネットワーク接続を確認
- テスト用Supabaseプロジェクトのリージョンを確認（近い場所を選択）
- 並列実行を制限（`dotnet test --parallel 1`）

## CI/CD統合

### GitHub Actions例

```yaml
name: Test

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Run tests
        env:
          Supabase__Url: ${{ secrets.SUPABASE_TEST_URL }}
          Supabase__Key: ${{ secrets.SUPABASE_TEST_KEY }}
        run: dotnet test --configuration Release
```

**注意**: GitHub Secretsに `SUPABASE_TEST_URL` と `SUPABASE_TEST_KEY` を設定してください。

## テストカバレッジ

現在のテストカバレッジ:

- ✅ Company CRUD操作
- ✅ PLC、Cycle、Model取得操作
- ✅ 複雑なWHERE句（複数条件）
- ✅ 複合キー削除操作
- ⏳ Timer関連操作（今後追加予定）
- ⏳ Process関連操作（今後追加予定）

## 新しいテストの追加

1. `SupabaseRepositoryTests.cs` に新しいテストメソッドを追加
2. `[Fact]` 属性を使用
3. Arrange-Act-Assertパターンに従う
4. テスト後のクリーンアップを忘れずに

例:

```csharp
[Fact]
public async Task YourNewTest_ShouldWork()
{
    // Arrange
    var testData = new SomeEntity { /* ... */ };

    // Act
    var result = await _repository.SomeMethod(testData);

    // Assert
    Assert.NotNull(result);

    // Cleanup
    await _repository.DeleteSomeEntity(result.Id);
}
```

## 参考資料

- [xUnit Documentation](https://xunit.net/)
- [Supabase C# Documentation](https://supabase.com/docs/reference/csharp/introduction)
- [Postgrest API Documentation](https://postgrest.org/en/stable/api.html)

---

**メンテナンス**: テストは定期的に実行し、新機能追加時には対応するテストも追加してください。
