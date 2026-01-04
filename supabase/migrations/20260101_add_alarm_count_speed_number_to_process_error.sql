-- ProcessErrorテーブルにAlarmCountとSpeedNumberカラムを追加
-- AlarmCount: 同一Operation/Interlock内でのエラー順序を識別するために使用
-- SpeedNumber: AlarmId=3,4の場合に、どの速度センサー（SS1-SS5）に対応するかを識別

-- AlarmCountカラムを追加（既に存在する場合はスキップ）
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_name = 'ProcessError' AND column_name = 'AlarmCount'
    ) THEN
        ALTER TABLE "ProcessError" ADD COLUMN "AlarmCount" INTEGER;
        COMMENT ON COLUMN "ProcessError"."AlarmCount" IS '同一レコード内でのエラー順序（0から開始）';
    END IF;
END $$;

-- SpeedNumberカラムを追加（既に存在する場合はスキップ）
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_name = 'ProcessError' AND column_name = 'SpeedNumber'
    ) THEN
        ALTER TABLE "ProcessError" ADD COLUMN "SpeedNumber" INTEGER;
        COMMENT ON COLUMN "ProcessError"."SpeedNumber" IS '速度センサー番号（1=SS1, 2=SS2, 3=SS3, 4=SS4, 5=SS5）AlarmId=3,4の場合に使用';
    END IF;
END $$;
