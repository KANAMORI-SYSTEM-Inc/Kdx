public class ViewInterlockConditions 
{    public int Id { get; set; }    public int PlcId { get; set; }    public int CylinderId { get; set; }    public int InterlockId { get; set; }    public string InterlockName { get; set; } = string.Empty;    public int ConditionNumber { get; set; }    public int ConditionTypeId { get; set; }    public string ConditionTypeName { get; set; } = string.Empty;
}

