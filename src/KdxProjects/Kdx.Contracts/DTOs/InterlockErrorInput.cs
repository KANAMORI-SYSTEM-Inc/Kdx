namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// Interlockエラーメッセージ生成用の入力データ
    /// メモリストアから取得したInterlock関連のMemoryデータをこの形式に変換して使用
    /// </summary>
    public class InterlockErrorInput
    {
        /// <summary>
        /// 対象シリンダーID
        /// </summary>
        public int CylinderId { get; set; }

        /// <summary>
        /// シリンダー名 (CYNum + CYNameSub)
        /// </summary>
        public string? CylinderName { get; set; }

        /// <summary>
        /// インターロックSortId
        /// </summary>
        public int InterlockSortId { get; set; }

        /// <summary>
        /// GoOrBack (0=Go&Back, 1=GoOnly, 2=BackOnly)
        /// </summary>
        public int GoOrBack { get; set; }

        /// <summary>
        /// GoOrBack表示名 (All, Go, Back など)
        /// </summary>
        public string? GoOrBackDisplayName { get; set; }

        /// <summary>
        /// 条件シリンダーID
        /// </summary>
        public int ConditionCylinderId { get; set; }

        /// <summary>
        /// 条件シリンダー名
        /// </summary>
        public string? ConditionCylinderName { get; set; }

        /// <summary>
        /// 条件番号
        /// </summary>
        public int ConditionNumber { get; set; }

        /// <summary>
        /// 条件タイプID
        /// </summary>
        public int? ConditionTypeId { get; set; }

        /// <summary>
        /// 条件タイプ名 (インターロック, センサー確認 など)
        /// </summary>
        public string? ConditionTypeName { get; set; }

        /// <summary>
        /// 条件名 (InterlockCondition.Name)
        /// </summary>
        public string? ConditionName { get; set; }

        /// <summary>
        /// 条件デバイス (InterlockCondition.Device)
        /// </summary>
        public string? ConditionDevice { get; set; }

        /// <summary>
        /// 条件コメント1
        /// </summary>
        public string? ConditionComment1 { get; set; }

        /// <summary>
        /// 条件コメント2
        /// </summary>
        public string? ConditionComment2 { get; set; }

        /// <summary>
        /// 割り当て済みデバイス (M100 など)
        /// </summary>
        public string? Device { get; set; }

        /// <summary>
        /// デバイス番号
        /// </summary>
        public int DeviceNumber { get; set; }

        /// <summary>
        /// インターロック番号
        /// </summary>
        public int InterlockNumber { get; set; }

        /// <summary>
        /// PLC ID
        /// </summary>
        public int PlcId { get; set; }

        /// <summary>
        /// 前提条件1の情報 (例: "サイクル待ち")
        /// </summary>
        public string? Precondition1Info { get; set; }

        /// <summary>
        /// 前提条件2の情報 (例: "工程3完了後")
        /// </summary>
        public string? Precondition2Info { get; set; }

        /// <summary>
        /// IO条件のリスト (例: ["LS1:ON", "LS2:OFF"])
        /// </summary>
        public List<string> IOConditions { get; set; } = new();

        /// <summary>
        /// 詳細IO条件のリスト（説明付き）
        /// (例: ["LS1(シリンダ前進確認):ON", "LS2(後退確認):OFF"])
        /// </summary>
        public List<string> DetailedIOConditions { get; set; } = new();

        /// <summary>
        /// Interlock ID
        /// </summary>
        public int InterlockId { get; set; }

        /// <summary>
        /// InterlockCondition ID
        /// </summary>
        public int InterlockConditionId { get; set; }

        /// <summary>
        /// IO詳細情報リスト (InterlockIOData相当の情報)
        /// </summary>
        public List<InterlockIoInfo> IoInfoList { get; set; } = new();
    }

    /// <summary>
    /// InterlockIOの詳細情報（IO.csの情報を含む）
    /// </summary>
    public class InterlockIoInfo
    {
        /// <summary>
        /// IO番号/インデックス (0から始まる)
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// IOアドレス (X100, Y200など)
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// IO名
        /// </summary>
        public string? IOName { get; set; }

        /// <summary>
        /// IO説明
        /// </summary>
        public string? IOExplanation { get; set; }

        /// <summary>
        /// Xコメント
        /// </summary>
        public string? XComment { get; set; }

        /// <summary>
        /// Yコメント
        /// </summary>
        public string? YComment { get; set; }

        /// <summary>
        /// Fコメント
        /// </summary>
        public string? FComment { get; set; }

        /// <summary>
        /// ユニット設置場所
        /// </summary>
        public string? IOSpot { get; set; }

        /// <summary>
        /// ユニット名称
        /// </summary>
        public string? UnitName { get; set; }

        /// <summary>
        /// 系統
        /// </summary>
        public string? System { get; set; }

        /// <summary>
        /// 局番
        /// </summary>
        public string? StationNumber { get; set; }

        /// <summary>
        /// リンクデバイス
        /// </summary>
        public string? LinkDevice { get; set; }

        /// <summary>
        /// ON/OFF条件
        /// </summary>
        public bool IsOnCondition { get; set; }
    }
}
