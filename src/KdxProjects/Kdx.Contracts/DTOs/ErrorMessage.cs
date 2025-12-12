namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// エラーメッセージのベーステンプレート
    /// 複合キー: (MnemonicId, ConditionTypeId, AlarmId)
    /// プレースホルダー({CylinderName}など)を含むテンプレート文字列を保持
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        /// 対象MnemonicType (Operation=3, CY=4, Interlock=6 など)
        /// 複合キーの一部
        /// </summary>
        public int MnemonicId { get; set; }

        /// <summary>
        /// 条件タイプID (InterlockConditionType.Idに対応)
        /// 複合キーの一部
        /// 0 = 共通テンプレート（全てのConditionTypeで使用）
        /// </summary>
        public int ConditionTypeId { get; set; } = 0;

        /// <summary>
        /// エラー種別ID (1=動作開始, 2=動作完了, 3=INV起動 など)
        /// 複合キーの一部
        /// </summary>
        public int AlarmId { get; set; }

        /// <summary>
        /// ベースメッセージ (プレースホルダー置換可能)
        /// 例: "{CylinderName}の{GoBack}動作が開始しません"
        /// </summary>
        public string? BaseMessage { get; set; }

        /// <summary>
        /// アラーム表示用ベースメッセージ
        /// </summary>
        public string? BaseAlarm { get; set; }

        /// <summary>
        /// カテゴリ1 (例: "ｲﾝﾀﾛｯｸｴﾗｰ", "操作ｴﾗｰ")
        /// </summary>
        public string? Category1 { get; set; }

        /// <summary>
        /// カテゴリ2 (プレースホルダー置換可能)
        /// </summary>
        public string? Category2 { get; set; }

        /// <summary>
        /// カテゴリ3 (プレースホルダー置換可能)
        /// </summary>
        public string? Category3 { get; set; }

        /// <summary>
        /// デフォルトのエラー検出時間 (ms)
        /// </summary>
        public int DefaultCountTime { get; set; } = 0;
    }
}