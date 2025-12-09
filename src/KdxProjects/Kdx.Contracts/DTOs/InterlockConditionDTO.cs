using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Kdx.Contracts.DTOs
{
    public class InterlockConditionDTO : INotifyPropertyChanged
    {
        // 複合主キー: (CylinderId, ConditionNumber, InterlockSortId)
        private int _cylinderId;
        private int _conditionNumber;
        private int _interlockSortId;
        private int? _conditionTypeId;
        private string? _name;
        private InterlockConditionType? _conditionType;

        public int CylinderId
        {
            get => _cylinderId;
            set
            {
                _cylinderId = value;
                OnPropertyChanged();
            }
        }

        public int ConditionNumber
        {
            get => _conditionNumber;
            set
            {
                _conditionNumber = value;
                OnPropertyChanged();
            }
        }

        public int InterlockSortId
        {
            get => _interlockSortId;
            set
            {
                _interlockSortId = value;
                OnPropertyChanged();
            }
        }

        public int? ConditionTypeId
        {
            get => _conditionTypeId;
            set
            {
                _conditionTypeId = value;
                OnPropertyChanged();
            }
        }

        public string? Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string? _device;
        public string? Device
        {
            get => _device;
            set
            {
                _device = value;
                OnPropertyChanged();
            }
        }

        private bool? _isOnCondition;
        public bool? IsOnCondition
        {
            get => _isOnCondition;
            set
            {
                _isOnCondition = value;
                OnPropertyChanged();
            }
        }

        private string? _comment;

        public string? Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged();
            }
        }

        // Navigation property (not mapped to database)
        [JsonIgnore]
        public InterlockConditionType? ConditionType
        {
            get => _conditionType;
            set
            {
                _conditionType = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
