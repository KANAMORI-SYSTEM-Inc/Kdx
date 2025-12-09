using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KdxDesigner.Models
{
    /// <summary>
    /// Cycle用メモリプロファイル
    /// ProcessDetail/Operationデバイスなど、Cycleごとに複数回適用可能な設定
    /// </summary>
    public class CycleMemoryProfile : INotifyPropertyChanged
    {
        private string _id = Guid.NewGuid().ToString();
        private string _name = string.Empty;
        private string _description = string.Empty;
        private DateTime _createdAt = DateTime.Now;
        private DateTime _updatedAt = DateTime.Now;
        private int _plcId = 2;
        private int _cycleId;
        private int _processDeviceStartL = 14000;
        private int _detailDeviceStartL = 15000;
        private int _operationDeviceStartM = 20000;
        private bool _isDefault;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set { _createdAt = value; OnPropertyChanged(); }
        }

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set { _updatedAt = value; OnPropertyChanged(); }
        }

        public int PlcId
        {
            get => _plcId;
            set { _plcId = value; OnPropertyChanged(); }
        }

        public int CycleId
        {
            get => _cycleId;
            set { _cycleId = value; OnPropertyChanged(); }
        }

        // Process/ProcessDetailデバイス設定
        public int ProcessDeviceStartL
        {
            get => _processDeviceStartL;
            set { _processDeviceStartL = value; OnPropertyChanged(); }
        }

        public int DetailDeviceStartL
        {
            get => _detailDeviceStartL;
            set { _detailDeviceStartL = value; OnPropertyChanged(); }
        }

        // Operationデバイス設定
        public int OperationDeviceStartM
        {
            get => _operationDeviceStartM;
            set { _operationDeviceStartM = value; OnPropertyChanged(); }
        }

        // デフォルトプロファイルかどうか
        public bool IsDefault
        {
            get => _isDefault;
            set { _isDefault = value; OnPropertyChanged(); }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
