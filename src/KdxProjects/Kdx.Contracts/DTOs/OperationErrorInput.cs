namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// Operationエラーメッセージ生成用の入力データ
    /// メモリストアから取得したOperation関連のMemoryデータをこの形式に変換して使用
    /// </summary>
    public class OperationErrorInput
    {
        /// <summary>
        /// Operation ID
        /// </summary>
        public int OperationId { get; set; }

        /// <summary>
        /// Operation名
        /// </summary>
        public string? OperationName { get; set; }

        /// <summary>
        /// AlarmId (ProcessErrorから取得)
        /// エラーメッセージテンプレートの選択に使用
        /// </summary>
        public int AlarmId { get; set; }

        /// <summary>
        /// AlarmCount (ProcessErrorから取得)
        /// 同一Operation内でのエラー順序を識別するために使用
        /// </summary>
        public int AlarmCount { get; set; }

        /// <summary>
        /// カテゴリID
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// カテゴリ名
        /// </summary>
        public string? CategoryName { get; set; }

        /// <summary>
        /// バルブ1
        /// </summary>
        public string? Valve1 { get; set; }

        /// <summary>
        /// バルブ2
        /// </summary>
        public string? Valve2 { get; set; }

        /// <summary>
        /// GoBack
        /// </summary>
        public string? GoBack { get; set; }

        /// <summary>
        /// 入力デバイス (operation.Start)
        /// </summary>
        public string? InputDevice { get; set; }

        /// <summary>
        /// 出力デバイス (operation.Finish)
        /// </summary>
        public string? OutputDevice { get; set; }

        /// <summary>
        /// 割り当て済みデバイス
        /// </summary>
        public string? Device { get; set; }

        /// <summary>
        /// デバイス番号
        /// </summary>
        public int DeviceNumber { get; set; }

        /// <summary>
        /// PLC ID
        /// </summary>
        public int PlcId { get; set; }

        /// <summary>
        /// Cycle ID
        /// </summary>
        public int CycleId { get; set; }

        /// <summary>
        /// 開始条件のIOリスト (M7の条件)
        /// operation.StartからマッピングされたIOアドレス
        /// </summary>
        public List<OperationIoInfo> StartIOs { get; set; } = new();

        /// <summary>
        /// 完了条件のIOリスト (M19の条件)
        /// operation.FinishからマッピングされたIOアドレス
        /// </summary>
        public List<OperationIoInfo> FinishIOs { get; set; } = new();

        /// <summary>
        /// 制御センサーのIO (M9の条件)
        /// operation.ConからマッピングされたIOアドレス
        /// </summary>
        public OperationIoInfo? ConIO { get; set; }

        /// <summary>
        /// 速度センサーのIOリスト (M10+nの条件)
        /// CategoryIdに応じて異なる速度センサーのIOアドレス
        /// </summary>
        public List<OperationIoInfo> SpeedIOs { get; set; } = new();

        /// <summary>
        /// Start条件の表示文字列（IO条件のカンマ区切り）
        /// </summary>
        public string StartConditionDisplay => StartIOs.Count > 0
            ? string.Join(", ", StartIOs.Select(io => io.DisplayCondition))
            : "";

        /// <summary>
        /// Finish条件の表示文字列（IO条件のカンマ区切り）
        /// </summary>
        public string FinishConditionDisplay => FinishIOs.Count > 0
            ? string.Join(", ", FinishIOs.Select(io => io.DisplayCondition))
            : "";

        /// <summary>
        /// Speed条件の表示文字列（IO条件のカンマ区切り）
        /// </summary>
        public string SpeedConditionDisplay => SpeedIOs.Count > 0
            ? string.Join(", ", SpeedIOs.Select(io => io.DisplayCondition))
            : "";
    }

    /// <summary>
    /// Operation用のIO情報
    /// エラーメッセージのプレースホルダー展開に使用
    /// </summary>
    public class OperationIoInfo
    {
        /// <summary>
        /// IOインデックス（リスト内での順番）
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// IOアドレス (例: X000, Y010)
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
        /// IO箇所
        /// </summary>
        public string? IOSpot { get; set; }

        /// <summary>
        /// ユニット名
        /// </summary>
        public string? UnitName { get; set; }

        /// <summary>
        /// システム
        /// </summary>
        public string? System { get; set; }

        /// <summary>
        /// ステーション番号
        /// </summary>
        public string? StationNumber { get; set; }

        /// <summary>
        /// リンクデバイス
        /// </summary>
        public string? LinkDevice { get; set; }

        /// <summary>
        /// ON条件かどうか
        /// </summary>
        public bool IsOnCondition { get; set; }

        /// <summary>
        /// 表示用の条件文字列（例: "X000(前進LS)", "NOT X001(後退LS)"）
        /// </summary>
        public string DisplayCondition => $"{(IsOnCondition ? "" : "NOT ")}{Address}{(string.IsNullOrEmpty(IOName) ? "" : $"({IOName})")}";

        /// <summary>
        /// 詳細表示用の条件文字列（説明付き）
        /// </summary>
        public string DetailedDisplayCondition
        {
            get
            {
                var parts = new List<string> { DisplayCondition };
                if (!string.IsNullOrEmpty(IOExplanation))
                {
                    parts.Add(IOExplanation);
                }
                return string.Join(" - ", parts);
            }
        }
    }
}
