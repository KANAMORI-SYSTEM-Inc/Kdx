namespace Kdx.Contracts.DTOs
{
    public class ProsTime
    {
        public int PlcId { get; set; }
        public int MnemonicId { get; set; }
        public int RecordId { get; set; }
        public int SortId { get; set; }
        public string CurrentDevice { get; set; } = "ZR0";
        public string PreviousDevice { get; set; } = "ZR0";
        public string CylinderDevice { get; set; } = "ZR0";
        public int CategoryId { get; set; } // OperationDefinitionsのID
        public int OutcoilNumber { get; set; } // OperationDefinitionsIdを格納
    }
}
