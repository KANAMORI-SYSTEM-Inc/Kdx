-- Check if the Operation table exists and if the Stay column exists
DO $$
BEGIN
    -- Check if the Operation table exists
    IF NOT EXISTS (
        SELECT FROM information_schema.tables
        WHERE table_schema = 'public'
        AND table_name = 'Operation'
    ) THEN
        RAISE NOTICE 'Operation table does not exist!';
    ELSE
        RAISE NOTICE 'Operation table exists';

        -- Check if Stay column exists
        IF NOT EXISTS (
            SELECT FROM information_schema.columns
            WHERE table_schema = 'public'
            AND table_name = 'Operation'
            AND column_name = 'Stay'
        ) THEN
            RAISE NOTICE 'Stay column does not exist. Adding it now...';
            ALTER TABLE "Operation" ADD COLUMN "Stay" TEXT;
            RAISE NOTICE 'Stay column added successfully';
        ELSE
            RAISE NOTICE 'Stay column already exists';
        END IF;
    END IF;
END $$;

-- Verify the Operation table structure
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'public'
AND table_name = 'Operation'
ORDER BY ordinal_position;
