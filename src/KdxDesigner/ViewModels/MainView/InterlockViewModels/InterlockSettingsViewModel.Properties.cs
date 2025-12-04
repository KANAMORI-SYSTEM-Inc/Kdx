using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace KdxDesigner.ViewModels
{
    public partial class InterlockSettingsViewModel
    {
        // Private fields
        private readonly SupabaseRepository _supabaseRepository;
        private readonly ISupabaseRepository _accessRepository;
        private readonly Window _window;
        private readonly int _plcId;
        private readonly int _cycleId;
        private CylinderViewModel? _selectedCylinder;
        private InterlockViewModel? _selectedInterlock;
        private InterlockConditionDTO? _selectedCondition;
        private InterlockIOViewModel? _selectedIO;
        private string? _cylinderSearchText;

        // Cylinder filtering
        private readonly ObservableCollection<CylinderViewModel> _allCylinders;
        private readonly ICollectionView _filteredCylinders;

        // Track deleted items for database cleanup
        private readonly List<InterlockIO> _deletedIOs = new List<InterlockIO>();
        private readonly List<InterlockConditionDTO> _deletedConditions = new List<InterlockConditionDTO>();
        private readonly List<InterlockViewModel> _deletedInterlocks = new List<InterlockViewModel>();

        // Cache all conditions and IOs for all interlocks (複合キーを使用)
        private readonly Dictionary<(int cylinderId, int sortId), List<InterlockConditionDTO>> _allConditionsByInterlockKey = new Dictionary<(int, int), List<InterlockConditionDTO>>();
        private readonly Dictionary<(int interlockId, int sortId, int conditionNumber), List<InterlockIOViewModel>> _allIOsByConditionKey = new Dictionary<(int, int, int), List<InterlockIOViewModel>>();

        // 前提条件のキャッシュ
        private Dictionary<int, string> _preCondition1Dict = new Dictionary<int, string>();
        private Dictionary<int, string> _preCondition2Dict = new Dictionary<int, string>();

        // Public collections
        public ObservableCollection<InterlockViewModel> Interlocks { get; set; }
        public ObservableCollection<InterlockConditionDTO> InterlockConditions { get; set; }
        public ObservableCollection<InterlockIOViewModel> InterlockIOs { get; set; }
        public ObservableCollection<InterlockConditionType> ConditionTypes { get; set; }

        public ICollectionView FilteredCylinders => _filteredCylinders;

        public string? CylinderSearchText
        {
            get => _cylinderSearchText;
            set
            {
                _cylinderSearchText = value;
                OnPropertyChanged();
                _filteredCylinders.Refresh();
            }
        }

        public CylinderViewModel? SelectedCylinder
        {
            get => _selectedCylinder;
            set
            {
                _selectedCylinder = value;
                OnPropertyChanged();
                // CanExecuteの状態を更新
                (AddInterlockCommand as RelayCommand)?.NotifyCanExecuteChanged();
                if (value != null)
                {
                    _ = LoadInterlocksAsync();
                }
            }
        }

        public InterlockViewModel? SelectedInterlock
        {
            get => _selectedInterlock;
            set
            {
                _selectedInterlock = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsInterlockSelected));
                // CanExecuteの状態を更新
                (DeleteInterlockCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (EditPreConditionsCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (AddConditionCommand as RelayCommand)?.NotifyCanExecuteChanged();
                _ = LoadInterlockConditionsAsync();
            }
        }

        public InterlockConditionDTO? SelectedCondition
        {
            get => _selectedCondition;
            set
            {
                _selectedCondition = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsConditionSelected));
                // CanExecuteの状態を更新
                (DeleteConditionCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (AddIOCommand as RelayCommand)?.NotifyCanExecuteChanged();
                _ = LoadInterlockIOsAsync();
            }
        }

        public InterlockIOViewModel? SelectedIO
        {
            get => _selectedIO;
            set
            {
                _selectedIO = value;
                OnPropertyChanged();
                // CanExecuteの状態を更新
                (DeleteIOCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public bool IsInterlockSelected => SelectedInterlock != null;
        public bool IsConditionSelected => SelectedCondition != null;
    }
}
