using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace KdxDesigner.ViewModels
{
    // Cylinder用のラッパークラス
    public class CylinderViewModel : INotifyPropertyChanged
    {
        private readonly Cylinder _cylinder;
        private string? _machineNameFullName;

        public CylinderViewModel(Cylinder cylinder)
        {
            _cylinder = cylinder;
        }

        // Cylinderのプロパティをプロキシ
        public int Id => _cylinder.Id;
        public int PlcId => _cylinder.PlcId;
        public string? PUCO => _cylinder.PUCO;
        public string CYNum => _cylinder.CYNum;
        public string? Go => _cylinder.Go;
        public string? Back => _cylinder.Back;
        public string? OilNum => _cylinder.OilNum;
        public int? MachineNameId => _cylinder.MachineNameId;

        // MachineNameのFullNameを保持（表示用）
        public string? MachineNameFullName
        {
            get => _machineNameFullName;
            set
            {
                _machineNameFullName = value;
                OnPropertyChanged();
            }
        }

        // 内部のCylinderオブジェクトを取得
        public Cylinder GetCylinder() => _cylinder;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // ViewModel内で使用するラッパークラス
    public class InterlockViewModel : INotifyPropertyChanged
    {
        private readonly Interlock _interlock;
        private string? _conditionCylinderNum;
        private string? _cylinderNum;
        private string? _preCondition1Name;
        private string? _preCondition2Name;

        public InterlockViewModel(Interlock interlock)
        {
            _interlock = interlock;
        }

        // Interlockのプロパティをプロキシ（Idプロパティは削除、複合キーを使用）
        public int PlcId
        {
            get => _interlock.PlcId;
            set
            {
                _interlock.PlcId = value;
                OnPropertyChanged();
            }
        }

        public int CylinderId
        {
            get => _interlock.CylinderId;
            set
            {
                _interlock.CylinderId = value;
                OnPropertyChanged();
            }
        }

        public int SortId
        {
            get => _interlock.SortId;
            set
            {
                _interlock.SortId = value;
                OnPropertyChanged();
            }
        }

        public int ConditionCylinderId
        {
            get => _interlock.ConditionCylinderId;
            set
            {
                _interlock.ConditionCylinderId = value;
                OnPropertyChanged();
            }
        }

        public int? PreConditionID1
        {
            get => _interlock.PreConditionID1;
            set
            {
                _interlock.PreConditionID1 = value;
                OnPropertyChanged();
            }
        }

        public int? PreConditionID2
        {
            get => _interlock.PreConditionID2;
            set
            {
                _interlock.PreConditionID2 = value;
                OnPropertyChanged();
            }
        }

        public int GoOrBack
        {
            get => _interlock.GoOrBack;
            set
            {
                _interlock.GoOrBack = value;
                OnPropertyChanged();
            }
        }

        // 表示用のCYNumプロパティ（このインターロックが属するシリンダーのCYNum）
        public string? CylinderNum
        {
            get => _cylinderNum;
            set
            {
                _cylinderNum = value;
                OnPropertyChanged();
            }
        }

        // 表示用のCYNumプロパティ（条件シリンダーのCYNum）
        public string? ConditionCylinderNum
        {
            get => _conditionCylinderNum;
            set
            {
                _conditionCylinderNum = value;
                OnPropertyChanged();
            }
        }

        // 表示用の前提条件1名
        public string? PreCondition1Name
        {
            get => _preCondition1Name;
            set
            {
                _preCondition1Name = value;
                OnPropertyChanged();
            }
        }

        // 表示用の前提条件2名
        public string? PreCondition2Name
        {
            get => _preCondition2Name;
            set
            {
                _preCondition2Name = value;
                OnPropertyChanged();
            }
        }

        // 内部のInterlockオブジェクトを取得
        public Interlock GetInterlock() => _interlock;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 外部からプロパティ変更を通知するためのpublicメソッド
        public void NotifyPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }

    // InterlockIO用のラッパークラス
    public class InterlockIOViewModel : INotifyPropertyChanged
    {
        private readonly InterlockIO _interlockIO;
        private string? _ioName;
        private bool _isNew; // 新規作成フラグ

        public InterlockIOViewModel(InterlockIO interlockIO, bool isNew = false)
        {
            _interlockIO = interlockIO;
            _isNew = isNew;
        }

        // InterlockIOのプロパティをプロキシ（複合キー対応）
        public int InterlockId
        {
            get => _interlockIO.InterlockId;
            set
            {
                _interlockIO.InterlockId = value;
                OnPropertyChanged();
            }
        }

        public int PlcId
        {
            get => _interlockIO.PlcId;
            set
            {
                _interlockIO.PlcId = value;
                OnPropertyChanged();
            }
        }

        public string? IOAddress
        {
            get => _interlockIO.IOAddress;
            set
            {
                _interlockIO.IOAddress = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public int InterlockSortId
        {
            get => _interlockIO.InterlockSortId;
            set
            {
                _interlockIO.InterlockSortId = value;
                OnPropertyChanged();
            }
        }

        public int ConditionNumber
        {
            get => _interlockIO.ConditionNumber;
            set
            {
                _interlockIO.ConditionNumber = value;
                OnPropertyChanged();
            }
        }

        public bool IsOnCondition
        {
            get => _interlockIO.IsOnCondition;
            set
            {
                _interlockIO.IsOnCondition = value;
                OnPropertyChanged();
            }
        }

        // 表示用のIONameプロパティ
        public string? IOName
        {
            get => _ioName;
            set
            {
                _ioName = value;
                OnPropertyChanged();
            }
        }

        // 新規作成フラグ
        public bool IsNew
        {
            get => _isNew;
            set
            {
                _isNew = value;
                OnPropertyChanged();
            }
        }

        // 内部のInterlockIOオブジェクトを取得
        public InterlockIO GetInterlockIO() => _interlockIO;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class InterlockSettingsViewModel : INotifyPropertyChanged
    {
        public InterlockSettingsViewModel(SupabaseRepository supabaseRepository, ISupabaseRepository accessRepository, int plcId, int cycleId, Window window)
        {
            _supabaseRepository = supabaseRepository;
            _accessRepository = accessRepository;
            _plcId = plcId;
            _cycleId = cycleId;
            _window = window;

            // Initialize collections
            Interlocks = new ObservableCollection<InterlockViewModel>();
            InterlockConditions = new ObservableCollection<InterlockConditionDTO>();
            InterlockIOs = new ObservableCollection<InterlockIOViewModel>();
            ConditionTypes = new ObservableCollection<InterlockConditionType>();

            // Initialize cylinder list and filtering
            _allCylinders = new ObservableCollection<CylinderViewModel>();
            _filteredCylinders = CollectionViewSource.GetDefaultView(_allCylinders);
            _filteredCylinders.Filter = FilterCylinder;

            // Subscribe to collection changes to sync ConditionType when ConditionTypeId changes
            InterlockConditions.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (InterlockConditionDTO condition in e.NewItems)
                    {
                        condition.PropertyChanged += OnConditionPropertyChanged;
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (InterlockConditionDTO condition in e.OldItems)
                    {
                        condition.PropertyChanged -= OnConditionPropertyChanged;
                    }
                }
            };

            // Initialize commands
            AddInterlockCommand = new RelayCommand(() => AddInterlock(null), () => CanAddInterlock(null));
            DeleteInterlockCommand = new RelayCommand(() => DeleteInterlock(null), () => CanDeleteInterlock(null));
            EditPreConditionsCommand = new RelayCommand(() => EditPreConditions(null), () => CanEditPreConditions(null));
            AddConditionCommand = new RelayCommand(() => AddCondition(null), () => CanAddCondition(null));
            DeleteConditionCommand = new RelayCommand(() => DeleteCondition(null), () => CanDeleteCondition(null));
            AddIOCommand = new RelayCommand(() => AddIO(null), () => CanAddIO(null));
            DeleteIOCommand = new RelayCommand(() => DeleteIO(null), () => CanDeleteIO(null));
            SaveCommand = new RelayCommand(async () => await SaveAsync());
            CancelCommand = new RelayCommand(() => Cancel(null));
            ClearCylinderSearchCommand = new RelayCommand(() => CylinderSearchText = string.Empty);
            ReloadCommand = new RelayCommand(async () => await ReloadAsync());

            _ = LoadCylindersAsync();
            _ = LoadConditionTypesAsync();
            _ = LoadPreConditionsAsync();
        }

        private async Task LoadCylindersAsync()
        {
            // 選択されたサイクルに紐づくCylinderのみを取得
            var allCylinders = await _accessRepository.GetCyListAsync(_plcId);
            var cylinderCycles = await _supabaseRepository.GetCylinderCyclesByPlcIdAsync(_plcId);

            // 指定されたcycleIdに紐づくCylinderIdのセットを作成
            var cylinderIdsInCycle = cylinderCycles
                .Where(cc => cc.CycleId == _cycleId)
                .Select(cc => cc.CylinderId)
                .ToHashSet();

            // MachineNameを取得してIDでマッピング
            var machineNames = await _supabaseRepository.GetMachineNamesAsync();
            var machineNameDict = machineNames.ToDictionary(mn => mn.Id, mn => mn.FullName);

            _allCylinders.Clear();
            foreach (var cylinder in allCylinders.Where(c => cylinderIdsInCycle.Contains(c.Id)))
            {
                // CylinderViewModelを作成
                var cylinderViewModel = new CylinderViewModel(cylinder);

                // MachineNameIdからFullNameを取得してViewModelに設定
                if (cylinder.MachineNameId.HasValue && machineNameDict.TryGetValue(cylinder.MachineNameId.Value, out var fullName))
                {
                    cylinderViewModel.MachineNameFullName = fullName;
                }

                _allCylinders.Add(cylinderViewModel);
            }
        }

        private bool FilterCylinder(object obj)
        {
            if (obj is not CylinderViewModel cylinderVm) { return false; }
            if (string.IsNullOrWhiteSpace(CylinderSearchText)) { return true; }

            var searchLower = CylinderSearchText.ToLower();
            return (cylinderVm.CYNum?.ToLower().Contains(searchLower) ?? false) ||
                   (cylinderVm.PUCO?.ToLower().Contains(searchLower) ?? false) ||
                   (cylinderVm.Go?.ToLower().Contains(searchLower) ?? false) ||
                   (cylinderVm.Back?.ToLower().Contains(searchLower) ?? false) ||
                   (cylinderVm.OilNum?.ToLower().Contains(searchLower) ?? false) ||
                   (cylinderVm.MachineNameFullName?.ToLower().Contains(searchLower) ?? false) ||
                   cylinderVm.Id.ToString().Contains(searchLower);
        }

        private void OnConditionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(InterlockConditionDTO.ConditionTypeId) && sender is InterlockConditionDTO condition)
            {
                // Update the ConditionType navigation property when ConditionTypeId changes
                condition.ConditionType = ConditionTypes.FirstOrDefault(ct => ct.Id == condition.ConditionTypeId);
            }
        }

        private async Task LoadConditionTypesAsync()
        {
            try
            {
                var types = await _supabaseRepository.GetInterlockConditionTypesAsync();
                ConditionTypes.Clear();
                foreach (var type in types)
                {
                    ConditionTypes.Add(type);
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show($"条件タイプの読み込みに失敗しました: {ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", "エラー", _window);
            }
        }

        private async Task LoadPreConditionsAsync()
        {
            try
            {
                var preCondition1List = await _supabaseRepository.GetInterlockPrecondition1ListAsync();
                _preCondition1Dict = preCondition1List.ToDictionary(p => p.Id, p => p.ConditionName ?? string.Empty);

                var preCondition2List = await _supabaseRepository.GetInterlockPrecondition2ListAsync();
                _preCondition2Dict = preCondition2List.ToDictionary(p => p.Id, p => p.InterlockMode ?? string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"前提条件の読み込みに失敗しました: {ex.Message}");
            }
        }

        private async Task LoadInterlocksAsync()
        {
            if (_selectedCylinder == null)
            {
                Interlocks.Clear();
                return;
            }

            try
            {
                var interlocks = await _supabaseRepository.GetInterlocksByCylindrIdAsync(_selectedCylinder.Id);
                Interlocks.Clear();
                foreach (var interlock in interlocks)
                {
                    var interlockViewModel = new InterlockViewModel(interlock);

                    // CylinderIdに対応するCYNumを取得（このインターロックが属するシリンダー）
                    if (_selectedCylinder != null)
                    {
                        interlockViewModel.CylinderNum = _selectedCylinder.CYNum;
                    }

                    // ConditionCylinderIdに対応するCYNumを取得
                    var conditionCylinder = _allCylinders.FirstOrDefault(c => c.Id == interlock.ConditionCylinderId);
                    if (conditionCylinder != null)
                    {
                        interlockViewModel.ConditionCylinderNum = conditionCylinder.CYNum;
                    }

                    // 前提条件1の名前を設定
                    if (interlock.PreConditionID1.HasValue && _preCondition1Dict.TryGetValue(interlock.PreConditionID1.Value, out var preCondition1Name))
                    {
                        interlockViewModel.PreCondition1Name = preCondition1Name;
                    }

                    // 前提条件2の名前を設定
                    if (interlock.PreConditionID2.HasValue && _preCondition2Dict.TryGetValue(interlock.PreConditionID2.Value, out var preCondition2Name))
                    {
                        interlockViewModel.PreCondition2Name = preCondition2Name;
                    }

                    Interlocks.Add(interlockViewModel);
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show($"インターロックの読み込みに失敗しました: {ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", "エラー", _window);
            }
        }

        private async Task LoadInterlockConditionsAsync()
        {
            InterlockConditions.Clear();
            InterlockIOs.Clear();

            if (SelectedInterlock == null)
            {
                return;
            }

            try
            {
                var interlockKey = (SelectedInterlock.CylinderId, SelectedInterlock.SortId);

                // キャッシュから取得するか、データベースから読み込む
                List<InterlockConditionDTO> conditions;
                if (_allConditionsByInterlockKey.TryGetValue(interlockKey, out var cachedConditions))
                {
                    conditions = cachedConditions;
                }
                else
                {
                    conditions = await _supabaseRepository.GetInterlockConditionsByInterlockIdAsync(SelectedInterlock.CylinderId);
                    _allConditionsByInterlockKey[interlockKey] = conditions;
                }

                // Populate the ConditionType navigation property for each condition
                foreach (var condition in conditions)
                {
                    var conditionType = ConditionTypes.FirstOrDefault(ct => ct.Id == condition.ConditionTypeId);
                    condition.ConditionType = conditionType;
                    // Subscribe to property changes for existing conditions
                    condition.PropertyChanged += OnConditionPropertyChanged;
                    InterlockConditions.Add(condition);
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show($"インターロック条件の読み込みに失敗しました: {ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", "エラー", _window);
            }
        }

        private async Task LoadInterlockIOsAsync()
        {
            InterlockIOs.Clear();

            if (SelectedCondition == null || SelectedInterlock == null)
            {
                return;
            }

            try
            {
                var conditionKey = (SelectedCondition.InterlockId, SelectedCondition.InterlockSortId, SelectedCondition.ConditionNumber);

                // キャッシュから取得するか、データベースから読み込む
                List<InterlockIOViewModel> ioViewModels;
                if (_allIOsByConditionKey.TryGetValue(conditionKey, out var cachedIOs))
                {
                    ioViewModels = cachedIOs;
                    System.Diagnostics.Debug.WriteLine($"キャッシュからIO読み込み: Key={conditionKey}, 件数={ioViewModels.Count}");
                }
                else
                {
                    var ios = await _supabaseRepository.GetInterlockIOsByInterlockIdAsync(SelectedCondition.InterlockId);
                    ioViewModels = new List<InterlockIOViewModel>();

                    foreach (var io in ios)
                    {
                        var ioViewModel = new InterlockIOViewModel(io, false); // 既存データ

                        // PlcIdとIOAddressに対応するIONameを取得
                        if (!string.IsNullOrEmpty(io.IOAddress))
                        {
                            var allIOs = await _accessRepository.GetIoListAsync();
                            var ioData = allIOs.FirstOrDefault(i => i.Address == io.IOAddress && i.PlcId == io.PlcId);
                            if (ioData != null)
                            {
                                ioViewModel.IOName = ioData.IOName;
                            }
                        }

                        ioViewModels.Add(ioViewModel);
                    }

                    _allIOsByConditionKey[conditionKey] = ioViewModels;
                    System.Diagnostics.Debug.WriteLine($"DBからIO読み込みしてキャッシュ: Key={conditionKey}, 件数={ioViewModels.Count}");
                }

                foreach (var ioViewModel in ioViewModels)
                {
                    InterlockIOs.Add(ioViewModel);
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show($"インターロックIOの読み込みに失敗しました: {ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", "エラー", _window);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
