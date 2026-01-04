-- ProcessErrorテーブルの作成
-- エラー情報の中間データを保存し、これを元にラダープログラムとエラーメッセージを生成する

-- テーブルが存在しない場合のみ作成
CREATE TABLE IF NOT EXISTS "ProcessError" (
    "PlcId" INTEGER NOT NULL,
    "CycleId" INTEGER,
    "Device" VARCHAR(50),
    "MnemonicId" INTEGER,
    "RecordId" INTEGER,
    "AlarmId" INTEGER,
    "AlarmCount" INTEGER,
    "SpeedNumber" INTEGER,
    "ErrorNum" INTEGER NOT NULL,
    "Comment1" VARCHAR(255),
    "Comment2" VARCHAR(255),
    "Comment3" VARCHAR(255),
    "Comment4" VARCHAR(255),
    "AlarmComment" VARCHAR(255),
    "MessageComment" TEXT,
    "ErrorTime" INTEGER,
    "ErrorTimeDevice" VARCHAR(50),
    "io_addresses" TEXT,
    "io_names" TEXT,
    "io_conditions" TEXT,
    PRIMARY KEY ("PlcId", "ErrorNum")
);

-- カラムにコメントを追加
COMMENT ON TABLE "ProcessError" IS 'エラー情報の中間データを保存し、これを元にラダープログラムとエラーメッセージを生成する';
COMMENT ON COLUMN "ProcessError"."PlcId" IS 'PLC ID（複合主キー）';
COMMENT ON COLUMN "ProcessError"."CycleId" IS 'Cycle ID';
COMMENT ON COLUMN "ProcessError"."Device" IS '割り当てデバイス（例: M50000）';
COMMENT ON COLUMN "ProcessError"."MnemonicId" IS 'ニーモニック種別（3=Operation, 4=CY, 5=Interlock）';
COMMENT ON COLUMN "ProcessError"."RecordId" IS '元レコードのID（OperationId, CylinderId等）';
COMMENT ON COLUMN "ProcessError"."AlarmId" IS 'アラーム種別ID（1=開始, 2=開始確認, 3=途中TO, 4=取込TO, 5=完了TO）';
COMMENT ON COLUMN "ProcessError"."AlarmCount" IS '同一レコード内でのエラー順序（0から開始）';
COMMENT ON COLUMN "ProcessError"."SpeedNumber" IS '速度センサー番号（1=SS1, 2=SS2, 3=SS3, 4=SS4）AlarmId=3,4の場合に使用';
COMMENT ON COLUMN "ProcessError"."ErrorNum" IS 'エラー番号（複合主キー）';
COMMENT ON COLUMN "ProcessError"."Comment1" IS 'コメント1（カテゴリ）';
COMMENT ON COLUMN "ProcessError"."Comment2" IS 'コメント2（バルブ+方向）';
COMMENT ON COLUMN "ProcessError"."Comment3" IS 'コメント3';
COMMENT ON COLUMN "ProcessError"."Comment4" IS 'コメント4';
COMMENT ON COLUMN "ProcessError"."AlarmComment" IS 'アラームコメント';
COMMENT ON COLUMN "ProcessError"."MessageComment" IS 'メッセージコメント（テンプレート展開後）';
COMMENT ON COLUMN "ProcessError"."ErrorTime" IS 'エラー検出時間（ms）';
COMMENT ON COLUMN "ProcessError"."ErrorTimeDevice" IS 'タイマーデバイス（例: T50000）';
COMMENT ON COLUMN "ProcessError"."io_addresses" IS '要因となるIOアドレスのリスト（カンマ区切り）例: X100,X101,X102';
COMMENT ON COLUMN "ProcessError"."io_names" IS '要因となるIO名のリスト（カンマ区切り）例: G,B,SS1';
COMMENT ON COLUMN "ProcessError"."io_conditions" IS 'IO条件の説明（カンマ区切り）例: G:ON, B:OFF, SS1:ON';

-- インデックスの作成
CREATE INDEX IF NOT EXISTS "idx_ProcessError_MnemonicId" ON "ProcessError" ("MnemonicId");
CREATE INDEX IF NOT EXISTS "idx_ProcessError_RecordId" ON "ProcessError" ("RecordId");
CREATE INDEX IF NOT EXISTS "idx_ProcessError_CycleId" ON "ProcessError" ("CycleId");
CREATE INDEX IF NOT EXISTS "idx_ProcessError_PlcId_CycleId_MnemonicId" ON "ProcessError" ("PlcId", "CycleId", "MnemonicId");
