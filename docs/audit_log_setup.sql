-- =========================================
-- 監査ログ（変更履歴）機能のセットアップスクリプト
-- Supabase PostgreSQL用
-- =========================================

-- 1. 監査ログテーブルの作成
CREATE TABLE IF NOT EXISTS audit_log (
    id BIGSERIAL PRIMARY KEY,
    table_name TEXT NOT NULL,
    record_id TEXT NOT NULL,
    operation TEXT NOT NULL CHECK (operation IN ('INSERT', 'UPDATE', 'DELETE')),
    old_data JSONB,
    new_data JSONB,
    changed_by UUID,
    changed_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- インデックスの作成
CREATE INDEX IF NOT EXISTS idx_audit_log_table_name ON audit_log(table_name);
CREATE INDEX IF NOT EXISTS idx_audit_log_changed_at ON audit_log(changed_at DESC);
CREATE INDEX IF NOT EXISTS idx_audit_log_record_id ON audit_log(record_id);

-- RLS (Row Level Security) の設定
ALTER TABLE audit_log ENABLE ROW LEVEL SECURITY;

-- 認証済みユーザーに読み取り権限を付与
CREATE POLICY "Allow authenticated users to read audit_log"
    ON audit_log
    FOR SELECT
    TO authenticated
    USING (true);

-- 2. 監査ログを記録する汎用トリガー関数
-- 注意: id列がないテーブル（複合主キーを使用するテーブル）にも対応
CREATE OR REPLACE FUNCTION log_audit_changes()
RETURNS TRIGGER AS $$
DECLARE
    record_id_value TEXT;
    user_id_value UUID;
    old_json JSONB;
    new_json JSONB;
BEGIN
    -- ユーザーIDを取得（Supabase認証）- UUID型として取得
    user_id_value := auth.uid();

    -- レコードをJSONBに変換（idの有無に関わらず動作）
    IF TG_OP = 'DELETE' THEN
        old_json := to_jsonb(OLD);
        -- record_idはJSONから主キー候補を抽出
        record_id_value := COALESCE(
            old_json->>'id',
            old_json->>'Id',
            CONCAT_WS('_',
                NULLIF(old_json->>'CylinderId', ''),
                NULLIF(old_json->>'SortId', ''),
                NULLIF(old_json->>'PlcId', ''),
                NULLIF(old_json->>'Address', '')
            ),
            'composite'
        );
    ELSE
        new_json := to_jsonb(NEW);
        -- record_idはJSONから主キー候補を抽出
        record_id_value := COALESCE(
            new_json->>'id',
            new_json->>'Id',
            CONCAT_WS('_',
                NULLIF(new_json->>'CylinderId', ''),
                NULLIF(new_json->>'SortId', ''),
                NULLIF(new_json->>'PlcId', ''),
                NULLIF(new_json->>'Address', '')
            ),
            'composite'
        );
    END IF;

    -- 操作に応じてログを記録
    IF TG_OP = 'INSERT' THEN
        INSERT INTO audit_log (table_name, record_id, operation, old_data, new_data, changed_by)
        VALUES (TG_TABLE_NAME, record_id_value, 'INSERT', NULL, new_json, user_id_value);
        RETURN NEW;
    ELSIF TG_OP = 'UPDATE' THEN
        old_json := to_jsonb(OLD);
        -- 変更がある場合のみログを記録
        IF old_json IS DISTINCT FROM new_json THEN
            INSERT INTO audit_log (table_name, record_id, operation, old_data, new_data, changed_by)
            VALUES (TG_TABLE_NAME, record_id_value, 'UPDATE', old_json, new_json, user_id_value);
        END IF;
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        INSERT INTO audit_log (table_name, record_id, operation, old_data, new_data, changed_by)
        VALUES (TG_TABLE_NAME, record_id_value, 'DELETE', old_json, NULL, user_id_value);
        RETURN OLD;
    END IF;

    RETURN NULL;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- 3. 古い監査ログを削除するクリーンアップ関数（10,000件を超えた場合）
CREATE OR REPLACE FUNCTION cleanup_old_audit_logs()
RETURNS TRIGGER AS $$
DECLARE
    max_records INTEGER := 10000;
    current_count INTEGER;
    delete_count INTEGER;
BEGIN
    -- 現在のレコード数を取得
    SELECT COUNT(*) INTO current_count FROM audit_log;

    -- 最大件数を超えている場合、古いレコードを削除
    IF current_count > max_records THEN
        delete_count := current_count - max_records;

        DELETE FROM audit_log
        WHERE id IN (
            SELECT id FROM audit_log
            ORDER BY changed_at ASC
            LIMIT delete_count
        );
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- クリーンアップトリガーの作成（監査ログへの挿入後に実行）
DROP TRIGGER IF EXISTS trigger_cleanup_audit_logs ON audit_log;
CREATE TRIGGER trigger_cleanup_audit_logs
    AFTER INSERT ON audit_log
    FOR EACH STATEMENT
    EXECUTE FUNCTION cleanup_old_audit_logs();

-- 4. 各テーブルにトリガーを作成する関数
CREATE OR REPLACE FUNCTION create_audit_trigger(target_table TEXT)
RETURNS VOID AS $$
DECLARE
    trigger_name TEXT;
BEGIN
    trigger_name := 'audit_trigger_' || target_table;

    -- 既存のトリガーを削除
    EXECUTE format('DROP TRIGGER IF EXISTS %I ON %I', trigger_name, target_table);

    -- 新しいトリガーを作成
    EXECUTE format('
        CREATE TRIGGER %I
            AFTER INSERT OR UPDATE OR DELETE ON %I
            FOR EACH ROW
            EXECUTE FUNCTION log_audit_changes()
    ', trigger_name, target_table);
END;
$$ LANGUAGE plpgsql;

-- 5. 追跡対象テーブルにトリガーを設定
-- ※ テーブルが存在しない場合はエラーになるので、存在確認後に実行してください

-- 基本マスタテーブル
SELECT create_audit_trigger('Company');
SELECT create_audit_trigger('Model');
SELECT create_audit_trigger('PLC');
SELECT create_audit_trigger('Cycle');

-- シリンダー関連
SELECT create_audit_trigger('Cylinder');

-- 操作・工程関連
SELECT create_audit_trigger('Operation');
SELECT create_audit_trigger('Process');
SELECT create_audit_trigger('ProcessDetail');

-- IO関連
SELECT create_audit_trigger('IO');

-- タイマー関連
SELECT create_audit_trigger('Timer');

-- 機械関連
SELECT create_audit_trigger('Machine');
SELECT create_audit_trigger('MachineName');
SELECT create_audit_trigger('DriveMain');
SELECT create_audit_trigger('DriveSub');

-- インターロック関連
SELECT create_audit_trigger('Interlock');
SELECT create_audit_trigger('InterlockCondition');

-- メモリ関連
SELECT create_audit_trigger('Memory');
SELECT create_audit_trigger('MemoryProfile');

-- 6. ユーザーメールアドレスを含む監査ログビューを作成
CREATE OR REPLACE VIEW audit_log_with_user AS
SELECT
    al.id,
    al.table_name,
    al.record_id,
    al.operation,
    al.old_data,
    al.new_data,
    al.changed_by,
    COALESCE(u.email, al.changed_by::TEXT) AS changed_by_email,
    al.changed_at
FROM audit_log al
LEFT JOIN auth.users u ON al.changed_by = u.id;

-- =========================================
-- 確認用クエリ
-- =========================================

-- トリガーの一覧を確認
-- SELECT trigger_name, event_object_table
-- FROM information_schema.triggers
-- WHERE trigger_name LIKE 'audit_trigger_%';

-- 監査ログの件数を確認
-- SELECT COUNT(*) FROM audit_log;

-- 最新の監査ログを確認
-- SELECT * FROM audit_log ORDER BY changed_at DESC LIMIT 10;

-- =========================================
-- トリガー削除用（必要な場合）
-- =========================================

-- 全ての監査トリガーを削除する関数
-- CREATE OR REPLACE FUNCTION drop_all_audit_triggers()
-- RETURNS VOID AS $$
-- DECLARE
--     r RECORD;
-- BEGIN
--     FOR r IN SELECT trigger_name, event_object_table
--              FROM information_schema.triggers
--              WHERE trigger_name LIKE 'audit_trigger_%'
--     LOOP
--         EXECUTE format('DROP TRIGGER IF EXISTS %I ON %I', r.trigger_name, r.event_object_table);
--     END LOOP;
-- END;
-- $$ LANGUAGE plpgsql;
