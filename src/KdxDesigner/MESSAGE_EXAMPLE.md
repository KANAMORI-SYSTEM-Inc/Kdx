# エラーメッセージベーステンプレート作成ガイド

## 概要

エラーメッセージは`error_message`テーブルに保存されたベーステンプレートを使用して自動生成されます。
テンプレートには`{プレースホルダー}`を使用でき、実行時に実際の値に置換されます。

## 利用可能なプレースホルダー

### 基本フィールド

| プレースホルダー | 説明 | 例 |
|---|---|---|
| `{CylinderName}` | シリンダー名 (CYNum + CYNameSub) | `CY01A` |
| `{GoBack}` | 動作方向 | `Go&Back`, `GoOnly`, `BackOnly` |
| `{ConditionCylinderName}` | 条件シリンダー名 | `CY02B` |
| `{ConditionType}` | 条件タイプ名 | `インターロック`, `センサー確認` |
| `{Comment1}` | 条件コメント1 | `前進確認` |
| `{Comment2}` | 条件コメント2 | `後退確認` |
| `{ConditionNumber}` | 条件番号 | `1`, `2`, `3` |

### 拡張フィールド

| プレースホルダー | 説明 | 例 |
|---|---|---|
| `{Precondition1}` | 前提条件1の情報 | `サイクル待ち` |
| `{Precondition2}` | 前提条件2の情報 | `Mode:常時 開始工程:1 終了工程:5` |
| `{IOConditions}` | IO条件リスト（簡易） | `LS1:ON, LS2:OFF` |
| `{DetailedIOConditions}` | IO条件リスト（詳細・説明付き） | `LS1(前進確認):ON, LS2(後退確認):OFF` |
| `{Device}` | 割り当てMデバイス | `M50000` |
| `{DeviceNumber}` | デバイス番号 | `50000` |
| `{InterlockNumber}` | インターロック番号 | `1` |

## テンプレート例

### Interlock用ベースメッセージ

```sql
-- シンプルなインターロックエラー
INSERT INTO error_message (mnemonic_id, alarm_id, base_message, base_alarm, category1, category2, category3, default_count_time)
VALUES (
    6,  -- MnemonicType.Interlock
    1,  -- AlarmId
    '{CylinderName}の{GoBack}インターロック異常',
    'ｲﾝﾀﾛｯｸ異常',
    'ｲﾝﾀﾛｯｸｴﾗｰ',
    '{GoBack}',
    '{ConditionType}',
    1000
);
```

### 詳細情報を含むテンプレート

```sql
-- IO条件を含む詳細メッセージ
INSERT INTO error_message (mnemonic_id, alarm_id, base_message, base_alarm, category1, category2, category3, default_count_time)
VALUES (
    6,
    2,
    '{CylinderName}{GoBack}のインターロック異常（{ConditionType}）確認IO: {IOConditions}',
    '{CylinderName}ｲﾝﾀﾛｯｸ',
    'ｲﾝﾀﾛｯｸｴﾗｰ',
    '{Comment1}',
    '{DetailedIOConditions}',
    2000
);
```

### 前提条件を含むテンプレート

```sql
-- 前提条件情報を含むメッセージ
INSERT INTO error_message (mnemonic_id, alarm_id, base_message, base_alarm, category1, category2, category3, default_count_time)
VALUES (
    6,
    3,
    '{CylinderName}の{GoBack}動作異常 {Precondition2}',
    '{ConditionType}異常',
    'ｲﾝﾀﾛｯｸ',
    '{Precondition1}',
    '{ConditionCylinderName}',
    1500
);
```

## Operation用ベースメッセージ

### 利用可能なプレースホルダー（Operation）

| プレースホルダー | 説明 | 例 |
|---|---|---|
| `{OperationName}` | オペレーション名 | `搬送動作` |
| `{Valve1}` | バルブ1 | `SOL1` |
| `{Valve2}` | バルブ2 | `SOL2` |
| `{GoBack}` | 動作方向 | `Go`, `Back` |
| `{CategoryName}` | カテゴリ名 | `速度制御INV1` |

### テンプレート例

```sql
-- 動作開始エラー (AlarmId=1)
INSERT INTO error_message (mnemonic_id, alarm_id, base_message, base_alarm, category1, category2, category3, default_count_time)
VALUES (6, 1, '{OperationName}動作開始異常', '動作開始異常', '操作ｴﾗｰ', '{CategoryName}', '開始', 2000);

-- 動作完了エラー (AlarmId=2)
INSERT INTO error_message (mnemonic_id, alarm_id, base_message, base_alarm, category1, category2, category3, default_count_time)
VALUES (6, 2, '{OperationName}動作完了異常', '動作完了異常', '操作ｴﾗｰ', '{CategoryName}', '完了', 3000);

-- INV起動エラー (AlarmId=3)
INSERT INTO error_message (mnemonic_id, alarm_id, base_message, base_alarm, category1, category2, category3, default_count_time)
VALUES (6, 3, '{OperationName}INV起動異常', 'INV起動異常', '操作ｴﾗｰ', '{CategoryName}', 'INV起動', 1500);

-- INV停止エラー (AlarmId=4)
INSERT INTO error_message (mnemonic_id, alarm_id, base_message, base_alarm, category1, category2, category3, default_count_time)
VALUES (6, 4, '{OperationName}INV停止異常', 'INV停止異常', '操作ｴﾗｰ', '{CategoryName}', 'INV停止', 1500);

-- 復帰エラー (AlarmId=5)
INSERT INTO error_message (mnemonic_id, alarm_id, base_message, base_alarm, category1, category2, category3, default_count_time)
VALUES (6, 5, '{OperationName}復帰異常', '復帰異常', '操作ｴﾗｰ', '{CategoryName}', '復帰', 2000);
```

## 生成結果の例

### 入力データ

```
CylinderName: CY01A
GoBack: Go&Back
ConditionType: インターロック
IOConditions: [LS1:ON, LS2:OFF]
DetailedIOConditions: [LS1(前進確認):ON, LS2(後退確認):OFF]
```

### テンプレート

```
BaseMessage: {CylinderName}の{GoBack}インターロック異常（{ConditionType}）確認IO: {IOConditions}
```

### 生成結果

```
MessageComment: CY01Aの Go&Backインターロック異常（インターロック）確認IO: LS1:ON, LS2:OFF
```

## テーブル構造

### error_message テーブル

```sql
CREATE TABLE error_message (
    mnemonic_id INTEGER NOT NULL,        -- MnemonicType (3=Operation, 6=Interlock)
    alarm_id INTEGER NOT NULL,           -- エラー種別
    base_message TEXT,                   -- メッセージテンプレート
    base_alarm TEXT,                     -- アラーム表示テンプレート
    category1 TEXT,                      -- カテゴリ1
    category2 TEXT,                      -- カテゴリ2（プレースホルダー可）
    category3 TEXT,                      -- カテゴリ3（プレースホルダー可）
    default_count_time INTEGER DEFAULT 0, -- エラー検出時間(ms)
    PRIMARY KEY (mnemonic_id, alarm_id)
);
```

### generated_error テーブル（出力先）

```sql
CREATE TABLE generated_error (
    plc_id INTEGER NOT NULL,             -- PLC ID
    error_num INTEGER NOT NULL,          -- エラー番号 (1-9999)
    mnemonic_id INTEGER NOT NULL,        -- MnemonicType
    alarm_id INTEGER NOT NULL,           -- AlarmId
    record_id INTEGER,                   -- 元レコードID
    device_m TEXT,                       -- Mデバイス
    device_t TEXT,                       -- Tデバイス
    comment1 TEXT,                       -- コメント1（category1から生成）
    comment2 TEXT,                       -- コメント2（category2から生成）
    comment3 TEXT,                       -- コメント3（category3から生成）
    comment4 TEXT,                       -- コメント4
    alarm_comment TEXT,                  -- アラームコメント（base_alarmから生成）
    message_comment TEXT,                -- メッセージコメント（base_messageから生成）
    error_time INTEGER DEFAULT 0,        -- エラー検出時間
    cycle_id INTEGER,                    -- サイクルID
    PRIMARY KEY (plc_id, error_num)
);
```

## 注意事項

1. **プレースホルダーの大文字小文字**: プレースホルダーは大文字小文字を区別します。`{CylinderName}`と`{cylindername}`は異なります。

2. **空のプレースホルダー**: 値が存在しない場合、プレースホルダーは空文字に置換されます。

3. **複数のAlarmId**: 1つのMnemonicIdに対して複数のAlarmIdを設定できます。それぞれ異なるエラーメッセージテンプレートを定義できます。

4. **デフォルトメッセージ**: `error_message`テーブルにデータがない場合、コード内のデフォルトメッセージが使用されます。
