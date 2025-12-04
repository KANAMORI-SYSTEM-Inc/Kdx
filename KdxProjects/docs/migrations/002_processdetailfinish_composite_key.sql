-- Migration: ProcessDetailFinish複合主キー変更 + CycleId追加
-- Date: 2025-10-16
-- Description: IdからProcessDetailId+FinishProcessDetailIdの複合主キーに変更し、CycleIdカラムを追加

-- Step 1: CycleIdカラムを追加（nullable）
ALTER TABLE "ProcessDetailFinish"
ADD COLUMN IF NOT EXISTS "CycleId" INTEGER;

-- Step 2: 既存データにCycleIdを設定（ProcessDetailから取得）
UPDATE "ProcessDetailFinish" pdf
SET "CycleId" = pd."CycleId"
FROM "ProcessDetail" pd
WHERE pdf."ProcessDetailId" = pd."ID";

-- Step 3: CycleIdをNOT NULLに変更
ALTER TABLE "ProcessDetailFinish"
ALTER COLUMN "CycleId" SET NOT NULL;

-- Step 4: 既存の重複データを削除（同じProcessDetailIdとFinishProcessDetailIdの組み合わせで最も古いものを残す）
DELETE FROM "ProcessDetailFinish" a
USING "ProcessDetailFinish" b
WHERE a."Id" > b."Id"
  AND a."ProcessDetailId" = b."ProcessDetailId"
  AND a."FinishProcessDetailId" = b."FinishProcessDetailId";

-- Step 5: 既存の主キー制約を削除
ALTER TABLE "ProcessDetailFinish"
DROP CONSTRAINT IF EXISTS "ProcessDetailFinish_pkey";

-- Step 6: 新しい複合主キーを作成
ALTER TABLE "ProcessDetailFinish"
ADD CONSTRAINT "ProcessDetailFinish_pkey"
PRIMARY KEY ("ProcessDetailId", "FinishProcessDetailId");

-- Step 7: Idカラムを削除
-- 注意: 削除する場合、既存のアプリケーションとの互換性が失われます
ALTER TABLE "ProcessDetailFinish"
DROP COLUMN IF EXISTS "Id";

-- Step 8: インデックスの最適化
-- FinishProcessDetailIdでの検索用インデックス
CREATE INDEX IF NOT EXISTS "ProcessDetailFinish_finish_idx"
ON "ProcessDetailFinish" ("FinishProcessDetailId");

-- CycleIdでのフィルタリング用インデックス
CREATE INDEX IF NOT EXISTS "ProcessDetailFinish_cycle_idx"
ON "ProcessDetailFinish" ("CycleId");

-- Step 9: 外部キー制約の追加（データ整合性の確保）
ALTER TABLE "ProcessDetailFinish"
ADD CONSTRAINT "ProcessDetailFinish_processdetail_fkey"
FOREIGN KEY ("ProcessDetailId") REFERENCES "ProcessDetail"("ID") ON DELETE CASCADE;

ALTER TABLE "ProcessDetailFinish"
ADD CONSTRAINT "ProcessDetailFinish_finish_fkey"
FOREIGN KEY ("FinishProcessDetailId") REFERENCES "ProcessDetail"("ID") ON DELETE CASCADE;

-- 確認用クエリ
-- SELECT * FROM "ProcessDetailFinish" ORDER BY "CycleId", "ProcessDetailId", "FinishProcessDetailId";
