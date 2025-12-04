# Changelog

このファイルは、KdxProjectsの注目すべき変更をすべて記録します。

フォーマットは [Keep a Changelog](https://keepachangelog.com/ja/1.0.0/) に基づいており、
このプロジェクトは [Semantic Versioning](https://semver.org/lang/ja/) に準拠しています。

## [Unreleased]
### Changed (BREAKING CHANGES)

- [[BREAKING] IOとCylinderを密接に関連付ける #1](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/issues/8)

### Added

- [[FEATURE] InterlockCondition Deviceの活用 #8](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/issues/1)

### Fixed

### Technical


## [2.0.0] - 2025-10-04

### Changed (BREAKING CHANGES)
- **データベーススキーマとの整合性対応**: Interlock関連テーブルを複合主キーに変更
  - `Interlock`: 単一`Id`から複合主キー `(CylinderId, SortId)` に変更
  - `InterlockCondition`: 単一`Id`から複合主キー `(InterlockId, ConditionNumber, InterlockSortId)` に変更
  - `InterlockIO`: `InterlockConditionId`を削除し、複合主キー `(InterlockId, PlcId, IOAddress, InterlockSortId, ConditionNumber)` に変更

### Added
- `InterlockCondition`に不足していたフィールドを追加
  - `InterlockSortId`: インターロックソートID（複合主キーの一部）
  - `Name`: 条件名

### Fixed
- Upsert操作での重複キーエラー「ON CONFLICT DO UPDATE command cannot affect row a second time」を修正
  - 複合主キーに基づく適切な重複検出ロジックを実装
  - Insert/Update判定を複合キーでの既存レコード検索に変更
- すべてのDelete操作を複合主キーに対応
  - `DeleteInterlockAsync/DeleteInterlocksAsync`
  - `DeleteInterlockConditionAsync/DeleteInterlockConditionsAsync`
  - `DeleteInterlockIOAsync`

### Technical
- リポジトリメソッドの更新
  - `UpsertInterlocksAsync`: 複合キー `(PlcId, CylinderId, SortId)` での重複検出
  - `UpsertInterlockConditionsAsync`: 複合キー `(InterlockId, ConditionNumber, InterlockSortId)` での重複検出
  - `GetInterlockIOsByInterlockIdAsync`: パラメータ名を`interlockConditionId`から`interlockId`に変更
- エンティティマッピングの更新
  - `InterlockEntity`, `InterlockConditionEntity`, `InterlockConditionDTOEntity`, `InterlockIOEntity`
  - Postgrest `[PrimaryKey]` 属性を複合主キー対応に修正（各カラムに個別の属性を設定）
- 依存コードの更新
  - Strategy クラス (Off1, On1, On2, OnM, OnOr): エラーメッセージを複合キー情報に更新
  - `InterlockValidationService`: 複合キーベースの重複チェックに変更

### Migration Notes
- **重要**: この変更は破壊的変更です
- `Interlock`, `InterlockCondition`, `InterlockConditionDTO` から `Id` フィールドが削除されました
- 既存のコードで `Id` フィールドを参照している場合は、複合主キーのフィールドに置き換える必要があります
- データベーススキーマは既に複合主キーを使用しているため、データベース側の変更は不要です

## [1.0.0] - 2025-10-02

### Added
- 初回リリース
- Kdx.Contracts v1.0.0 - DTOとインターフェースの定義
- Kdx.Core v1.0.0 - ビジネスロジックとアプリケーションサービス
  - IProcessFlowService - プロセスフロー管理サービス
  - IInterlockValidationService - インターロック検証サービス
- Kdx.Infrastructure v1.0.0 - インフラストラクチャサービスの実装
  - ProcessFlowService - プロセスフロー管理の実装
  - InterlockValidationService - インターロック検証の実装
- Kdx.Infrastructure.Supabase v1.0.0 - Supabase固有のリポジトリ実装
  - SupabaseRepository - Supabase接続とデータアクセス
- Kdx.Contracts.ViewModels v1.0.0 - WPF ViewModelsの契約

### Changed
- モノリポから独立したNuGetパッケージリポジトリに分離

### Technical
- .NET 8.0対応
- NuGetパッケージ生成機能を追加
- CI/CDパイプライン設定（GitHub Actions）
