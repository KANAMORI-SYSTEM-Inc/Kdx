# ProcessDetailConnection 複合主キー移行ガイド

## 概要

ProcessDetailConnectionテーブルをId主キーから(FromProcessDetailId, ToProcessDetailId)の複合主キーに変更し、CycleIdカラムを追加します。

## 実行手順

### 1. データベースマイグレーション

Supabase SQL Editorで以下のスクリプトを実行してください：

```bash
KdxProjects/docs/migrations/001_processdetailconnection_composite_key.sql
```

### 2. KdxProjectsのビルドと公開

```bash
cd KdxProjects
dotnet build -c Release
```

バージョン番号を **v3.0.0** に更新（Breaking Change）:
- Kdx.Contracts.csproj
- Kdx.Infrastructure.Supabase.csproj
- Kdx.Infrastructure.csproj
- Kdx.Core.csproj

### 3. KdxDesignerの更新

以下の変更を適用してください：

**ProcessFlowDetailViewModel.cs:**
- Line 1206: デバッグ出力を更新
- Line 1414: `DeleteProcessDetailConnectionAsync(connection.FromProcessDetailId, connection.ToProcessDetailId)`
- Line 1487: `DeleteProcessDetailConnectionAsync(conn.FromProcessDetailId, conn.ToProcessDetailId)`
- Line 1186-1190: CycleIdの設定を追加

**ProcessDetailPropertiesViewModel.cs:**
- Line 187-193: CycleIdの設定を追加

### 4. ビルドとテスト

```bash
cd KdxDesigner
dotnet build
dotnet run
```

## 主な変更点

### データベース

- **削除**: `Id` カラム（auto-increment主キー）
- **追加**: `CycleId` カラム（NOT NULL）
- **変更**: 主キーを `(FromProcessDetailId, ToProcessDetailId)` に変更
- **追加**: 外部キー制約とインデックス

### コード

**DTO (ProcessDetailConnection):**
```csharp
// 削除
public int Id { get; set; }

// CycleId はデータベースカラムに対応
public int? CycleId { get; set; }
```

**Entity (ProcessDetailConnectionEntity):**
```csharp
[PrimaryKey("FromProcessDetailId")]
public int FromProcessDetailId { get; set; }

[PrimaryKey("ToProcessDetailId")]
public int? ToProcessDetailId { get; set; }

[Column("CycleId")]
public int CycleId { get; set; }
```

**Repository:**
```csharp
// Before
Task DeleteProcessDetailConnectionAsync(int id);

// After
Task DeleteProcessDetailConnectionAsync(int fromProcessDetailId, int toProcessDetailId);
```

## ロールバック手順

もし問題が発生した場合：

```sql
-- 主キーを削除
ALTER TABLE "ProcessDetailConnection"
DROP CONSTRAINT "ProcessDetailConnection_pkey";

-- Id カラムを追加
ALTER TABLE "ProcessDetailConnection"
ADD COLUMN "Id" SERIAL PRIMARY KEY;

-- CycleId を削除（オプション）
ALTER TABLE "ProcessDetailConnection"
DROP COLUMN "CycleId";
```

## 検証

マイグレーション後、以下を確認してください：

1. ProcessDetailConnectionテーブルの主キーが変更されている
2. 重複した接続が存在しない
3. KdxDesignerで接続の作成・削除が正常に動作する
4. エラー「duplicate key value violates unique constraint」が解消されている

## トラブルシューティング

### Q: マイグレーション実行時にエラーが発生する
A: 既存の重複データが存在する可能性があります。Step 4の削除クエリを手動で確認してください。

### Q: ビルドエラーが発生する
A: すべてのファイルが正しく更新されているか確認してください。特にISupabaseRepositoryとProcessFlowServiceのインターフェースと実装が一致しているか確認してください。

### Q: アプリケーション実行時にエラーが発生する
A: KdxDesignerがKdxProjectsの最新版（v3.0.0）を参照しているか確認してください。NuGetパッケージのキャッシュをクリアする必要がある場合があります。
