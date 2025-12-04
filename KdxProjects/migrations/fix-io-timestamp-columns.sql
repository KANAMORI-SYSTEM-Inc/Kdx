-- IOテーブルのCreatedAtとUpdatedAtカラムをtimestamptz型に変更
-- 実行日: 2025-10-08
-- 理由: time型ではタイムスタンプ値を保存できないため

-- CreatedAtカラムの型を変更
ALTER TABLE "IO"
ALTER COLUMN "CreatedAt" TYPE timestamptz
USING "CreatedAt"::timestamptz;

-- UpdatedAtカラムの型を変更
ALTER TABLE "IO"
ALTER COLUMN "UpdatedAt" TYPE timestamptz
USING "UpdatedAt"::timestamptz;

-- デフォルト値を設定（新規レコード用）
ALTER TABLE "IO"
ALTER COLUMN "CreatedAt" SET DEFAULT now();

ALTER TABLE "IO"
ALTER COLUMN "UpdatedAt" SET DEFAULT now();

-- 確認
SELECT column_name, data_type, column_default
FROM information_schema.columns
WHERE table_name = 'IO'
AND column_name IN ('CreatedAt', 'UpdatedAt');
