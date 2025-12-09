using Kdx.Contracts.DTOs;

namespace KdxDesigner.Models
{
    /// <summary>
    /// Cylinderに紐づくInterlock関連データを一括で管理するクラス
    /// </summary>
    public class CylinderInterlockData
    {
        /// <summary>
        /// 対象のCylinder
        /// </summary>
        public Cylinder Cylinder { get; set; } = new();

        /// <summary>
        /// このCylinderに紐づくInterlockのリスト
        /// </summary>
        public List<InterlockData> Interlocks { get; set; } = new();
    }

    /// <summary>
    /// 個別のInterlockとその関連データを管理するクラス
    /// </summary>
    public class InterlockData
    {
        /// <summary>
        /// Interlock本体
        /// </summary>
        public Interlock Interlock { get; set; } = new();

        /// <summary>
        /// 前提条件1（PreConditionID1に対応）
        /// </summary>
        public InterlockPrecondition1? Precondition1 { get; set; }

        /// <summary>
        /// 前提条件2（PreConditionID2に対応）
        /// </summary>
        public InterlockPrecondition2? Precondition2 { get; set; }

        /// <summary>
        /// 条件シリンダー（ConditionCylinderIdに対応）
        /// </summary>
        public Cylinder? ConditionCylinder { get; set; }

        /// <summary>
        /// このInterlockに紐づくConditionのリスト
        /// </summary>
        public List<InterlockConditionData> Conditions { get; set; } = new();

        /// <summary>
        /// Interlock.GoOrBackの表示名を取得
        /// </summary>
        public string GoOrBackDisplayName => Interlock.GoOrBack switch
        {
            0 => "Go&Back",
            1 => "GoOnly",
            2 => "BackOnly",
            _ => "不明"
        };
    }

    /// <summary>
    /// InterlockConditionとその関連IOを管理するクラス
    /// </summary>
    public class InterlockConditionData
    {
        /// <summary>
        /// InterlockCondition本体
        /// </summary>
        public InterlockCondition Condition { get; set; } = new();

        /// <summary>
        /// 条件タイプ（ConditionTypeIdに対応）
        /// </summary>
        public InterlockConditionType? ConditionType { get; set; }

        /// <summary>
        /// このConditionに紐づくIOのリスト
        /// </summary>
        public List<InterlockIOData> IOs { get; set; } = new();

        /// <summary>
        /// インターロック番号（メモリ設定時に割り当て）
        /// </summary>
        public int InterlockNumber { get; set; }

        /// <summary>
        /// 割り当てられたMデバイス（例: "M50000"）
        /// </summary>
        public string? Device { get; set; }

        /// <summary>
        /// デバイス番号（InterlockDeviceStartM + InterlockNumber）
        /// </summary>
        public int DeviceNumber { get; set; }
    }

    /// <summary>
    /// InterlockIOとその関連情報を管理するクラス
    /// </summary>
    public class InterlockIOData
    {
        /// <summary>
        /// InterlockIO本体
        /// </summary>
        public InterlockIO IO { get; set; } = new();

        /// <summary>
        /// IOの名前（IOテーブルから取得）
        /// </summary>
        public string? IOName { get; set; }

        /// <summary>
        /// IsOnConditionの表示名を取得
        /// </summary>
        public string IsOnConditionDisplayName => IO.IsOnCondition ? "ON" : "OFF";
    }
}
