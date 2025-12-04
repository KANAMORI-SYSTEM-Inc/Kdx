---
name: 💥 破壊的変更の提案
about: 既存APIの破壊的変更を提案する（メジャーバージョンアップ）
title: '[BREAKING] '
labels: 'breaking-change, enhancement'
assignees: ''
---

## 💥 破壊的変更の概要

<!-- 提案する破壊的変更を簡潔に説明してください -->


## ⚠️ 警告

この変更は**メジャーバージョンアップ**（例: 1.x.x → 2.0.0）が必要です。

## 📦 影響を受けるパッケージ

<!-- 該当するパッケージにチェック [x] を入れてください -->

- [ ] Kdx.Contracts
- [ ] Kdx.Core
- [ ] Kdx.Infrastructure
- [ ] Kdx.Infrastructure.Supabase
- [ ] Kdx.Contracts.ViewModels

## 🎯 変更の理由

<!-- なぜ破壊的変更が必要なのか詳しく説明してください -->


## 📋 現在の実装

### 現在のAPI

```csharp
// 現在のインターフェースやクラスの定義

public interface ICurrentService
{
    List<Item> GetItems(int id);
}
```

### 現在の使用例

```csharp
// KdxDesignerでの現在の使い方

var items = _service.GetItems(1);
```

## ✨ 提案する新しい実装

### 新しいAPI

```csharp
// 新しいインターフェースやクラスの定義

public interface INewService
{
    Task<IEnumerable<Item>> GetItemsAsync(int id, CancellationToken cancellationToken = default);
}
```

### 新しい使用例

```csharp
// KdxDesignerでの新しい使い方

var items = await _service.GetItemsAsync(1);
```

## 🔄 変更内容の詳細

### 変更されるAPI一覧

| 現在のAPI | 新しいAPI | 変更内容 |
|----------|----------|---------|
| `GetItems(int)` | `GetItemsAsync(int)` | 非同期化 |
| | | |

### 削除されるAPI

- `OldMethod()` - 理由:
-

### 名前変更

- `OldName` → `NewName` - 理由:
-

## 📚 Migration Guide

### KdxDesignerでの対応手順

#### ステップ1: 名前空間の更新

```csharp
// 変更前
using Kdx.OldNamespace;

// 変更後
using Kdx.NewNamespace;
```

#### ステップ2: メソッド呼び出しの更新

```csharp
// 変更前
var items = _service.GetItems(1);

// 変更後
var items = await _service.GetItemsAsync(1);
```

#### ステップ3: その他の変更

<!-- 必要に応じて追加手順を記載 -->

### 推定作業時間

KdxDesignerでの対応に必要な推定時間:
- [ ] 1時間未満
- [ ] 1-4時間
- [ ] 4-8時間
- [ ] 1日以上

## 🎯 メリット

<!-- この破壊的変更によって得られるメリットを記載してください -->

1.
2.
3.

## ⚡ 影響範囲の分析

### KdxDesignerへの影響

- 影響を受けるファイル数（推定）:
- 影響を受ける機能:
  - [ ] プロセスフロー
  - [ ] シリンダー設定
  - [ ] インターロック設定
  - [ ] その他:

### 既存ユーザーへの影響

<!-- この変更が既存ユーザーにどのような影響を与えるか記載 -->


## 🗓️ 提案するリリース時期

<!-- いつこの変更をリリースすべきか提案してください -->

- [ ] 次のメジャーバージョン（計画的リリース）
- [ ] 緊急（重大な問題の修正）
- [ ] その他:

## 🔗 関連情報

<!-- 関連するIssue、PR、ドキュメントがあれば記載してください -->

-

## 📝 補足情報

<!-- その他、参考になる情報があれば記載してください -->


## ✅ レビューチェックリスト

### 提案者
- [ ] 破壊的変更の理由を明確に記載した
- [ ] 現在と新しい実装の両方を記載した
- [ ] Migration Guideを記載した
- [ ] KdxDesignerへの影響を分析した
- [ ] メリットを明確に記載した

### レビュワー
- [ ] 破壊的変更が本当に必要か検討した
- [ ] 代替案（非破壊的）を検討した
- [ ] Migration Guideが明確で実行可能
- [ ] 影響範囲が適切に評価されている
- [ ] リリース時期が適切

## 💬 ディスカッション

<!-- この提案についての議論をここに記載してください -->
