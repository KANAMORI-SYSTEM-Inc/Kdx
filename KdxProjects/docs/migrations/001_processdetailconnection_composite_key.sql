-- Migration: ProcessDetailConnection複合主キー変更 + CycleId追加
-- Date: 2025-10-16
-- Description: IdからFromProcessDetailId+ToProcessDetailIdの複合主キーに変更し、CycleIdカラムを追加

-- Step 1: CycleIdカラムを追加（nullable）
ALTER TABLE "ProcessDetailConnection"
ADD COLUMN IF NOT EXISTS "CycleId" INTEGER;

-- Step 2: 既存データにCycleIdを設定（ProcessDetailから取得）
UPDATE "ProcessDetailConnection" pdc
SET "CycleId" = pd."CycleId"
FROM "ProcessDetail" pd
WHERE pdc."FromProcessDetailId" = pd."ID";

-- Step 3: CycleIdをNOT NULLに変更
ALTER TABLE "ProcessDetailConnection"
ALTER COLUMN "CycleId" SET NOT NULL;

-- Step 4: 既存の重複データを削除（同じFromとToの組み合わせで最も古いものを残す）
DELETE FROM "ProcessDetailConnection" a
USING "ProcessDetailConnection" b
WHERE a."Id" > b."Id"
  AND a."FromProcessDetailId" = b."FromProcessDetailId"
  AND a."ToProcessDetailId" = b."ToProcessDetailId";

-- Step 5: 既存の主キー制約を削除
ALTER TABLE "ProcessDetailConnection"
DROP CONSTRAINT IF EXISTS "ProcessDetailConnection_pkey";

-- Step 6: 新しい複合主キーを作成
ALTER TABLE "ProcessDetailConnection"
ADD CONSTRAINT "ProcessDetailConnection_pkey"
PRIMARY KEY ("FromProcessDetailId", "ToProcessDetailId");

-- Step 7: Idカラムを削除
-- 注意: 削除する場合、既存のアプリケーションとの互換性が失われます
ALTER TABLE "ProcessDetailConnection"
DROP COLUMN IF EXISTS "Id";

-- Step 8: インデックスの最適化
-- 逆方向の検索用インデックス
CREATE INDEX IF NOT EXISTS "ProcessDetailConnection_to_idx"
ON "ProcessDetailConnection" ("ToProcessDetailId");

-- CycleIdでのフィルタリング用インデックス
CREATE INDEX IF NOT EXISTS "ProcessDetailConnection_cycle_idx"
ON "ProcessDetailConnection" ("CycleId");

-- Step 9: 外部キー制約の追加（データ整合性の確保）
ALTER TABLE "ProcessDetailConnection"
ADD CONSTRAINT "ProcessDetailConnection_from_fkey"
FOREIGN KEY ("FromProcessDetailId") REFERENCES "ProcessDetail"("ID") ON DELETE CASCADE;

ALTER TABLE "ProcessDetailConnection"
ADD CONSTRAINT "ProcessDetailConnection_to_fkey"
FOREIGN KEY ("ToProcessDetailId") REFERENCES "ProcessDetail"("ID") ON DELETE CASCADE;

-- 確認用クエリ
-- SELECT * FROM "ProcessDetailConnection" ORDER BY "CycleId", "FromProcessDetailId", "ToProcessDetailId";
