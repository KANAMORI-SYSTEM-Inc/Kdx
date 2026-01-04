# エラーメッセージテンプレート作成ガイド

KdxDesignerのエラーメッセージ生成システムでは、プレースホルダーを使用してカスタマイズ可能なエラーメッセージを作成できます。

## 目次
- [概要](#概要)
- [エラーメッセージの種類](#エラーメッセージの種類)
- [Interlock用プレースホルダー](#interlock用プレースホルダー)
- [Operation用プレースホルダー](#operation用プレースホルダー)
- [エラーメッセージの作成方法](#エラーメッセージの作成方法)
- [実用例](#実用例)

---

## 概要

エラーメッセージは、Supabaseの`ErrorMessage`テーブルに保存されたテンプレートから生成されます。テンプレート内に`{プレースホルダー名}`の形式で記述すると、実際のデータに置き換えられます。

### システムフロー
```
Supabase ErrorMessageテンプレート
    ↓ プレースホルダー置換
GeneratedError（生成されたエラー）
    ↓ CSV出力
PLCエラーメッセージ
```

---

## エラーメッセージの種類

### 1. Interlock用エラーメッセージ
- **用途**: シリンダーのインターロック条件が不成立の場合
- **MnemonicId**: `4` (Interlock)
- **テーブル**: `ErrorMessage` (ConditionTypeIdで分岐可能)

### 2. Operation用エラーメッセージ
- **用途**: 操作の開始・完了・速度変化のタイムアウト
- **MnemonicId**: `3` (Operation)
- **AlarmId**: 1～5（エラー種類を識別）
  - **1**: 開始 (M6 ON, M7 OFF)
  - **2**: 開始確認 (M6 ON, M7 OFF, M19 ON)
  - **3**: 途中TO (M6 ON, M10+n OFF, M19 OFF)
  - **4**: 取り込みTO (M6 ON, M10+n OFF, M19 ON)
  - **5**: 完了TO (M6 ON, M19 OFF)

---

## Interlock用プレースホルダー

### 基本情報
| プレースホルダー | 説明 | 例 |
|-----------------|------|-----|
| `{CylinderName}` | インターロック対象シリンダー名 | `CY01前進` |
| `{GoBack}` | 動作方向 | `Go`, `Back`, `All` |
| `{ConditionCylinderName}` | 条件シリンダー名 | `CY02後退` |
| `{ConditionType}` | 条件種別名 | `前進中`, `後退中`, `前進端` |
| `{ConditionName}` | 条件名 | `前進LS` |
| `{ConditionDevice}` | 条件デバイス | `M100` |
| `{Comment1}` | コメント1 | - |
| `{Comment2}` | コメント2 | - |
| `{ConditionNumber}` | 条件番号 | `1`, `2`, `3` |
| `{Device}` | 割り当てデバイス | `M1000` |
| `{DeviceNumber}` | デバイス番号 | `1000` |
| `{InterlockNumber}` | インターロック番号 | `1` |

### 前提条件情報
| プレースホルダー | 説明 | 例 |
|-----------------|------|-----|
| `{Precondition1}` | 前提条件1の情報 | `工程1実行中` |
| `{Precondition2}` | 前提条件2の情報 | `Mode:Auto 開始工程:1 終了工程:5` |

### IO条件情報
| プレースホルダー | 説明 | 出力例 |
|-----------------|------|--------|
| `{IOConditions}` | 全IO条件（カンマ区切り） | `X000(前進LS), NOT X001(後退LS)` |
| `{DetailedIOConditions}` | 詳細IO条件（説明付き） | `X000(前進LS) - 前進リミットスイッチ` |

### IO詳細情報（最初のIO）
| プレースホルダー | 説明 | 例 |
|-----------------|------|-----|
| `{IO.Address}` | IOアドレス | `X000` |
| `{IO.IOName}` | IO名 | `前進LS` |
| `{IO.IOExplanation}` | IO説明 | `前進リミットスイッチ` |
| `{IO.XComment}` | Xコメント | `前進端検出` |
| `{IO.YComment}` | Yコメント | - |
| `{IO.FComment}` | Fコメント | - |
| `{IO.IOSpot}` | IO箇所 | `ステージ1` |
| `{IO.UnitName}` | ユニット名 | `搬送ユニットA` |
| `{IO.System}` | システム | `Main` |
| `{IO.StationNumber}` | 局番 | `1` |
| `{IO.LinkDevice}` | リンクデバイス | `U0\G0` |
| `{IO.IsOnCondition}` | ON条件か | `ON` or `OFF` |

### IO詳細情報（インデックス指定）
複数のIO条件がある場合、インデックスで個別に指定できます。

```
{IO[0].Address}      → 1番目のIOアドレス
{IO[0].IOName}       → 1番目のIO名
{IO[1].Address}      → 2番目のIOアドレス
{IO[1].IOName}       → 2番目のIO名
...
```

**利用可能な全プロパティ:**
`Address`, `IOName`, `IOExplanation`, `XComment`, `YComment`, `FComment`, `IOSpot`, `UnitName`, `System`, `StationNumber`, `LinkDevice`, `IsOnCondition`

---

## Operation用プレースホルダー

### 基本情報
| プレースホルダー | 説明 | 例 |
|-----------------|------|-----|
| `{OperationName}` | 操作名 | `CY01前進` |
| `{Valve1}` | バルブ1 | `Y010` |
| `{Valve2}` | バルブ2 | `Y011` |
| `{GoBack}` | 動作方向 | `前進`, `後退` |
| `{CategoryName}` | カテゴリ名 | `速度制御INV2`, `保持` |

### 開始条件IO（Start）
M7の条件（`operation.Start`から取得）

| プレースホルダー | 説明 | 出力例 |
|-----------------|------|--------|
| `{StartCondition}` | 全開始条件（カンマ区切り） | `X000(前進LS), NOT X001(後退LS)` |
| `{StartIO.Address}` | 最初のIOアドレス | `X000` |
| `{StartIO.IOName}` | 最初のIO名 | `前進LS` |
| `{StartIO.IOExplanation}` | 最初のIO説明 | `前進リミットスイッチ` |
| `{StartIO.DisplayCondition}` | 最初のIO条件表示 | `X000(前進LS)` or `NOT X000(前進LS)` |

**インデックス指定:**
```
{StartIO[0].Address}          → 1番目の開始IOアドレス
{StartIO[0].IOName}           → 1番目の開始IO名
{StartIO[0].DisplayCondition} → 1番目の開始IO条件
{StartIO[1].Address}          → 2番目の開始IOアドレス
...
```

### 完了条件IO（Finish）
M16/M19の条件（`operation.Finish`から取得）

| プレースホルダー | 説明 | 出力例 |
|-----------------|------|--------|
| `{FinishCondition}` | 全完了条件（カンマ区切り） | `X002(完了LS)` |
| `{FinishIO.Address}` | 最初のIOアドレス | `X002` |
| `{FinishIO.IOName}` | 最初のIO名 | `完了LS` |
| `{FinishIO.IOExplanation}` | 最初のIO説明 | `完了リミットスイッチ` |
| `{FinishIO.DisplayCondition}` | 最初のIO条件表示 | `X002(完了LS)` |

**インデックス指定:**
```
{FinishIO[0].Address}
{FinishIO[0].IOName}
{FinishIO[0].DisplayCondition}
{FinishIO[1].Address}
...
```

### 速度センサー情報
エラーごとに対応する速度センサーの情報（AlarmId=3,4の場合にSpeedNumberで特定）

| プレースホルダー | 説明 | 出力例 |
|-----------------|------|--------|
| `{SpeedNumber}` | 速度センサー番号（1-4） | `1`, `2`, `3`, `4` |
| `{SpeedSensorName}` | 速度センサー名 | `SS1`, `SS2`, `SS3`, `SS4` |
| `{SpeedSensorAddress}` | 速度センサーのIOアドレス | `X120` |
| `{SpeedSensorExplain}` | 速度センサーの説明 | `速度センサー1` |

**注意:** AlarmId=3（途中TO）やAlarmId=4（取込TO）の場合、`SpeedNumber`によって対象の速度センサーが特定されます。例えば、CategoryId=4（速度制御INV2）では以下のエラーが生成されます：
- AlarmId=3, SpeedNumber=1 → SS1の途中TO
- AlarmId=4, SpeedNumber=1 → SS1の取込TO
- AlarmId=3, SpeedNumber=2 → SS2の途中TO
- AlarmId=4, SpeedNumber=2 → SS2の取込TO

### 速度センサーIO（Speed）
M10+nの条件（CategoryIdに応じて`SS1`, `SS2`, `SS3`, `SS4`から取得）

| プレースホルダー | 説明 | 出力例 |
|-----------------|------|--------|
| `{SpeedCondition}` | 全速度条件（カンマ区切り） | `X010(速度LS1), X011(速度LS2)` |
| `{SpeedIO.Address}` | 最初のIOアドレス | `X010` |
| `{SpeedIO.IOName}` | 最初のIO名 | `速度LS1` |
| `{SpeedIO.IOExplanation}` | 最初のIO説明 | `第1速度検出センサー` |
| `{SpeedIO.DisplayCondition}` | 最初のIO条件表示 | `X010(速度LS1)` |

**インデックス指定:**
```
{SpeedIO[0].Address}          → 1番目の速度IOアドレス
{SpeedIO[0].IOName}           → 1番目の速度IO名
{SpeedIO[0].DisplayCondition} → 1番目の速度IO条件
{SpeedIO[1].Address}          → 2番目の速度IOアドレス
...
```

**CategoryId別の速度IO数:**
| CategoryId | 速度変化回数 | 速度センサー | SpeedIOの数 |
|------------|-------------|-------------|-------------|
| 3, 9, 15, 27 | 1回 | SS1 | 1個 |
| 4, 10, 16, 28 | 2回 | SS1, SS2 | 2個 |
| 5, 11, 17 | 3回 | SS1, SS2, SS3 | 3個 |
| 6, 12, 18 | 4回 | SS1, SS2, SS3, SS4 | 4個 |
| 7, 13, 19 | 5回 | SS1, SS2, SS3, SS4 | 4個 ※ |

※ CategoryId 7/13/19は本来5回ですが、SS5プロパティが存在しないため4つまで

### 制御センサーIO（Con）
M9の条件（`operation.Con`から取得）

| プレースホルダー | 説明 | 出力例 |
|-----------------|------|--------|
| `{ConIO}` | 制御センサー条件 | `X020(制御LS)` |
| `{ConIO.Address}` | IOアドレス | `X020` |
| `{ConIO.IOName}` | IO名 | `制御LS` |
| `{ConIO.IOExplanation}` | IO説明 | `制御用リミットスイッチ` |

---

## エラーメッセージの作成方法

### 1. Supabaseの設定

#### ErrorMessageテーブル構造
```sql
CREATE TABLE "ErrorMessage" (
    "Id" SERIAL PRIMARY KEY,
    "MnemonicId" INTEGER NOT NULL,      -- 3=Operation, 4=Interlock
    "AlarmId" INTEGER,                   -- Operation用（1～5）
    "ConditionTypeId" INTEGER,           -- Interlock用（条件種別）
    "BaseMessage" TEXT,                  -- 基本メッセージ
    "BaseAlarm" TEXT,                    -- アラーム表示
    "Category1" TEXT,                    -- カテゴリ1
    "Category2" TEXT,                    -- カテゴリ2
    "Category3" TEXT,                    -- カテゴリ3
    "DefaultCountTime" INTEGER           -- デフォルトタイマー値
);
```

### 2. テンプレート作成の基本ルール

#### ルール1: プレースホルダーは `{}`で囲む
```
正: {CylinderName}のインターロック異常
誤: [CylinderName]のインターロック異常
誤: $CylinderName$のインターロック異常
```

#### ルール2: 存在しないプレースホルダーは空文字に置換される
```
テンプレート: {OperationName}の{NonExistent}エラー
出力:         前進動作のエラー
```

#### ルール3: UI装飾記号は自動削除される
以下の文字は自動的に削除されます：
- `●` (塗りつぶし丸)
- `○` (白丸)
- 半角スペース
- 全角スペース

### 3. KdxDesignerでの設定方法

#### ステップ1: エラーメッセージ生成ウィンドウを開く
```
メインメニュー → エラーメッセージ生成
```

#### ステップ2: 設定を入力
- **エラー開始番号**: 生成するエラーの開始番号
- **Mデバイス開始**: Mデバイスの開始番号
- **Tデバイス開始**: Tデバイスの開始番号
- **PLC ID**: 対象PLC ID（自動設定）
- **Cycle ID**: 対象Cycle ID（Operation用のみ）

#### ステップ3: プレビュー生成
- **Interlockプレビュー**: Interlock用エラーメッセージを生成
- **Operationプレビュー**: Operation用エラーメッセージを生成

#### ステップ4: 保存
- プレビュー確認後、「保存」ボタンでSupabaseに保存

---

## 実用例

### Interlock用エラーメッセージ例

#### 例1: シンプルなインターロックエラー
```json
{
  "MnemonicId": 4,
  "ConditionTypeId": 0,
  "AlarmId": 1,
  "BaseMessage": "{CylinderName}のインターロック異常",
  "BaseAlarm": "ｲﾝﾀﾛｯｸ異常",
  "Category1": "ｲﾝﾀﾛｯｸｴﾗｰ",
  "Category2": "{GoBack}",
  "Category3": "{ConditionType}",
  "DefaultCountTime": 1000
}
```

**出力例:**
```
BaseMessage: CY01前進のインターロック異常
BaseAlarm: ｲﾝﾀﾛｯｸ異常
Category1: ｲﾝﾀﾛｯｸｴﾗｰ
Category2: Go
Category3: 前進中
```

#### 例2: IO条件を含む詳細エラー
```json
{
  "MnemonicId": 4,
  "ConditionTypeId": 1,
  "AlarmId": 1,
  "BaseMessage": "{CylinderName}{GoBack}時の{ConditionType}異常: {IOConditions}",
  "BaseAlarm": "{ConditionCylinderName}ｲﾝﾀﾛｯｸ",
  "Category1": "{IO[0].IOName}",
  "Category2": "{IO[0].Address}",
  "Category3": "{ConditionType}",
  "DefaultCountTime": 1000
}
```

**出力例:**
```
BaseMessage: CY01前進Go時の前進中異常: X000(前進LS), NOT X001(後退LS)
BaseAlarm: CY02後退ｲﾝﾀﾛｯｸ
Category1: 前進LS
Category2: X000
Category3: 前進中
```

#### 例3: 前提条件を含むエラー
```json
{
  "MnemonicId": 4,
  "ConditionTypeId": 2,
  "AlarmId": 1,
  "BaseMessage": "{CylinderName}ｲﾝﾀﾛｯｸ異常[{Precondition1}]",
  "BaseAlarm": "ｲﾝﾀﾛｯｸ({Precondition2})",
  "Category1": "{ConditionName}",
  "Category2": "{ConditionDevice}",
  "Category3": "{DetailedIOConditions}",
  "DefaultCountTime": 1000
}
```

**出力例:**
```
BaseMessage: CY01前進ｲﾝﾀﾛｯｸ異常[工程1実行中]
BaseAlarm: ｲﾝﾀﾛｯｸ(Mode:Auto 開始工程:1 終了工程:5)
Category1: 前進LS
Category2: M100
Category3: X000(前進LS) - 前進リミットスイッチ, NOT X001(後退LS) - 後退リミットスイッチ
```

---

### Operation用エラーメッセージ例

#### 例1: 開始エラー (AlarmId=1)
```json
{
  "MnemonicId": 3,
  "AlarmId": 1,
  "BaseMessage": "{OperationName}開始異常: {StartCondition}",
  "BaseAlarm": "開始異常",
  "Category1": "{CategoryName}",
  "Category2": "{Valve1}{GoBack}",
  "Category3": "開始TO",
  "DefaultCountTime": 3000
}
```

**出力例:**
```
BaseMessage: CY01前進開始異常: NOT X000(前進LS)
BaseAlarm: 開始異常
Category1: 速度制御INV2
Category2: Y010前進
Category3: 開始TO
```

#### 例2: 開始確認エラー (AlarmId=2)
```json
{
  "MnemonicId": 3,
  "AlarmId": 2,
  "BaseMessage": "{OperationName}開始確認異常: 開始[{StartCondition}] 完了[{FinishCondition}]",
  "BaseAlarm": "開始確認TO",
  "Category1": "{OperationName}",
  "Category2": "{Valve1}{GoBack}",
  "Category3": "START: {StartIO.Address}",
  "DefaultCountTime": 5000
}
```

**出力例:**
```
BaseMessage: CY01前進開始確認異常: 開始[NOT X000(前進LS)] 完了[X002(完了LS)]
BaseAlarm: 開始確認TO
Category1: CY01前進
Category2: Y010前進
Category3: START: X000
```

#### 例3: 途中タイムアウト (AlarmId=3) - SpeedNumber使用
```json
{
  "MnemonicId": 3,
  "AlarmId": 3,
  "BaseMessage": "{OperationName}途中TO: {SpeedSensorName}({SpeedSensorAddress})未検出",
  "BaseAlarm": "途中TO({SpeedSensorName})",
  "Category1": "{CategoryName}",
  "Category2": "{Valve1}{GoBack}",
  "Category3": "{SpeedSensorExplain}",
  "DefaultCountTime": 10000
}
```

**出力例 (CategoryId=4, AlarmId=3, SpeedNumber=1):**
```
BaseMessage: CY01前進途中TO: SS1(X120)未検出
BaseAlarm: 途中TO(SS1)
Category1: 速度制御INV2
Category2: Y010前進
Category3: 速度センサー1
```

**出力例 (CategoryId=4, AlarmId=3, SpeedNumber=2):**
```
BaseMessage: CY01前進途中TO: SS2(X121)未検出
BaseAlarm: 途中TO(SS2)
Category1: 速度制御INV2
Category2: Y010前進
Category3: 速度センサー2
```

#### 例4: 取り込みタイムアウト (AlarmId=4) - SpeedNumber使用
```json
{
  "MnemonicId": 3,
  "AlarmId": 4,
  "BaseMessage": "{OperationName}取込TO: {SpeedSensorName}({SpeedSensorAddress})+完了{FinishCondition}",
  "BaseAlarm": "取込TO({SpeedSensorName})",
  "Category1": "{CategoryName}",
  "Category2": "{Valve1}{GoBack}",
  "Category3": "{SpeedSensorExplain}",
  "DefaultCountTime": 8000
}
```

**出力例 (CategoryId=4, AlarmId=4, SpeedNumber=1):**
```
BaseMessage: CY01前進取込TO: SS1(X120)+完了X002(完了LS)
BaseAlarm: 取込TO(SS1)
Category1: 速度制御INV2
Category2: Y010前進
Category3: 速度センサー1
```

**出力例 (CategoryId=4, AlarmId=4, SpeedNumber=2):**
```
BaseMessage: CY01前進取込TO: SS2(X121)+完了X002(完了LS)
BaseAlarm: 取込TO(SS2)
Category1: 速度制御INV2
Category2: Y010前進
Category3: 速度センサー2
```

#### 例5: 完了タイムアウト (AlarmId=5)
```json
{
  "MnemonicId": 3,
  "AlarmId": 5,
  "BaseMessage": "{OperationName}完了TO: {FinishCondition}未検出",
  "BaseAlarm": "完了TO",
  "Category1": "{CategoryName}",
  "Category2": "{Valve1}{GoBack}",
  "Category3": "{FinishIO.IOName}({FinishIO.Address})",
  "DefaultCountTime": 15000
}
```

**出力例:**
```
BaseMessage: CY01前進完了TO: X002(完了LS)未検出
BaseAlarm: 完了TO
Category1: 速度制御INV2
Category2: Y010前進
Category3: 完了LS(X002)
```

#### 例6: 複数IO条件を個別表示
```json
{
  "MnemonicId": 3,
  "AlarmId": 1,
  "BaseMessage": "{OperationName}開始異常: 1={StartIO[0].DisplayCondition}, 2={StartIO[1].DisplayCondition}",
  "BaseAlarm": "開始異常(複数)",
  "Category1": "1:{StartIO[0].IOName}",
  "Category2": "2:{StartIO[1].IOName}",
  "Category3": "{CategoryName}",
  "DefaultCountTime": 3000
}
```

**出力例 (複数開始条件がある場合):**
```
BaseMessage: CY01前進開始異常: 1=NOT X000(前進LS), 2=X003(安全SW)
BaseAlarm: 開始異常(複数)
Category1: 1:前進LS
Category2: 2:安全SW
Category3: 速度制御INV2
```

#### 例7: 制御センサーエラー
```json
{
  "MnemonicId": 3,
  "AlarmId": 3,
  "BaseMessage": "{OperationName}制御ｾﾝｻｰ異常: {ConIO}",
  "BaseAlarm": "制御ｾﾝｻｰTO",
  "Category1": "{ConIO.IOName}",
  "Category2": "{ConIO.Address}",
  "Category3": "{ConIO.IOExplanation}",
  "DefaultCountTime": 5000
}
```

**出力例:**
```
BaseMessage: CY01前進制御ｾﾝｻｰ異常: X020(制御LS)
BaseAlarm: 制御ｾﾝｻｰTO
Category1: 制御LS
Category2: X020
Category3: 制御用リミットスイッチ
```

---

## ベストプラクティス

### 1. エラーメッセージは簡潔に
❌ **悪い例:**
```
{CylinderName}の{GoBack}動作時において、{ConditionType}の状態が期待される条件を満たしていないため、インターロック異常が発生しました。詳細は{IOConditions}を確認してください。
```

✅ **良い例:**
```
{CylinderName}{GoBack}時{ConditionType}異常: {IOConditions}
```

### 2. オペレーターが理解しやすい情報を優先
✅ **良い例:**
- 「どのシリンダーの」→ `{CylinderName}`
- 「どの動作で」→ `{GoBack}`
- 「どのIOが」→ `{StartIO.IOName}`
- 「どんな状態が」→ `{StartIO.DisplayCondition}`

### 3. CategoryやAlarmIdに応じて適切なプレースホルダーを選択

**保持動作 (CategoryId=2):**
- 速度センサーは不要 → `{SpeedCondition}`は使用しない
- 開始と完了のみ → `{StartCondition}`, `{FinishCondition}`を使用

**速度制御 (CategoryId=3-7):**
- 速度センサーが重要 → `{SpeedCondition}`や`{SpeedIO[n].xxx}`を使用
- AlarmId=3,4で速度IOを明示

### 4. インデックス指定時は範囲に注意
```
❌ 危険: {SpeedIO[4].Address}  # CategoryId=3では存在しない可能性
✅ 安全: {SpeedIO[0].Address}  # 最初の速度IOは常に存在（速度制御の場合）
✅ 安全: {SpeedCondition}      # 全速度IOをまとめて表示
```

### 5. デフォルト値の設定
プレースホルダーが空の場合も考慮してメッセージを設計：

```
✅ 良い例: {OperationName}異常: {StartCondition}
出力例（IOがある場合）: CY01前進異常: X000(前進LS)
出力例（IOがない場合）: CY01前進異常:

❌ 悪い例: {OperationName}の{StartIO.IOName}が
出力例（IOがない場合）: CY01前進のが  # 不自然
```

---

## トラブルシューティング

### Q1: プレースホルダーが置換されない
**原因:**
- プレースホルダー名のスペルミス
- `{}`が全角になっている

**解決策:**
```
❌ {OperationNam}    # スペルミス
❌ ｛OperationName｝ # 全角
✅ {OperationName}   # 正しい
```

### Q2: IOアドレスが表示されない
**原因:**
- IOリストにIONameが一致するデータがない
- `operation.Start`, `operation.Finish`などが空

**解決策:**
1. Supabaseの`IO`テーブルを確認
2. `Operation`テーブルの`Start`, `Finish`, `SS1`等のフィールドを確認
3. IONameの検索は部分一致（Contains）で行われるため、厳密な一致は不要

### Q3: 速度IOが取得されない
**原因:**
- CategoryIdが速度制御ではない（1, 2, 14, 20, 31-35など）
- SS1-SS4プロパティが空

**解決策:**
1. `operation.CategoryId`を確認
2. CategoryIdに応じた`SS1`, `SS2`, `SS3`, `SS4`の値を確認

### Q4: インデックス指定で空になる
**原因:**
- 指定したインデックスのIOが存在しない

**例:**
```
CategoryId=3（速度変化1回）で {SpeedIO[1].Address} を使用
→ SpeedIOは1個しかないため、[1]は存在しない（[0]のみ）
```

**解決策:**
- `{SpeedCondition}` など集約プレースホルダーを使用
- または CategoryId別にメッセージを分ける（ConditionTypeIdを活用）

---

## 関連ドキュメント

- **実装コード**: `src/KdxDesigner/Services/ErrorMessageGenerator/ErrorMessageGenerator.cs`
- **DTO定義**:
  - `src/KdxProjects/Kdx.Contracts/DTOs/InterlockErrorInput.cs`
  - `src/KdxProjects/Kdx.Contracts/DTOs/OperationErrorInput.cs`
- **UI実装**: `src/KdxDesigner/Views/ErrorMessage/ErrorMessageGeneratorWindow.xaml`
- **ViewModel**: `src/KdxDesigner/ViewModels/ErrorMessage/ErrorMessageGeneratorViewModel.cs`

---

## 更新履歴

| 日付 | バージョン | 変更内容 |
|------|----------|---------|
| 2025-12-24 | 1.0 | 初版作成（Operation用IO情報統合対応） |
| 2026-01-01 | 1.1 | SpeedNumber関連プレースホルダー追加（SpeedNumber, SpeedSensorName, SpeedSensorAddress, SpeedSensorExplain） |

---

**作成者:** Claude Code
**最終更新:** 2026-01-01
