using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Collections.ObjectModel;

namespace KdxDesigner.ViewModels
{
    /// <summary>
    /// インターロックプロパティウィンドウのViewModel
    /// </summary>
    public partial class InterlockPropertiesViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository;
        private readonly Interlock _interlock;
        private readonly List<Cylinder> _allCylinders;

        // 複合主キー（読み取り専用）
        [ObservableProperty] private int _cylinderId;
        [ObservableProperty] private int _sortId;

        // 編集可能フィールド
        [ObservableProperty] private int _plcId;
        [ObservableProperty] private int _conditionCylinderId;
        [ObservableProperty] private int? _preConditionID1;
        [ObservableProperty] private int? _preConditionID2;
        [ObservableProperty] private int _goOrBack;

        // マスターデータ
        [ObservableProperty] private ObservableCollection<Cylinder> _cylinders = new();
        [ObservableProperty] private ObservableCollection<InterlockPrecondition1> _preConditions1 = new();
        [ObservableProperty] private ObservableCollection<InterlockPrecondition2> _preConditions2 = new();
        [ObservableProperty] private ObservableCollection<GoOrBackOption> _goOrBackOptions = new();

        // 表示用
        [ObservableProperty] private string? _cylinderDisplayName;

        public bool DialogResult { get; private set; }

        public InterlockPropertiesViewModel(ISupabaseRepository repository, Interlock interlock, List<Cylinder> allCylinders)
        {
            _repository = repository;
            _interlock = interlock;
            _allCylinders = allCylinders;

            // GoOrBackオプションを初期化
            GoOrBackOptions = new ObservableCollection<GoOrBackOption>
            {
                new GoOrBackOption { Value = 0, DisplayName = "Go&Back" },
                new GoOrBackOption { Value = 1, DisplayName = "GoOnly" },
                new GoOrBackOption { Value = 2, DisplayName = "BackOnly" }
            };

            // マスターデータを読み込み
            LoadMasterData();

            // プロパティを読み込み
            LoadProperties();
        }

        private async void LoadMasterData()
        {
            try
            {
                // シリンダーリスト
                Cylinders = new ObservableCollection<Cylinder>(_allCylinders);

                // 前提条件1リスト
                var preConditions1 = await _repository.GetInterlockPrecondition1ListAsync();
                PreConditions1 = new ObservableCollection<InterlockPrecondition1>(preConditions1);

                // 前提条件2リスト
                var preConditions2 = await _repository.GetInterlockPrecondition2ListAsync();
                PreConditions2 = new ObservableCollection<InterlockPrecondition2>(preConditions2);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"マスターデータの読み込み中にエラーが発生しました: {ex.Message}", "エラー",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void LoadProperties()
        {
            CylinderId = _interlock.CylinderId;
            SortId = _interlock.SortId;
            PlcId = _interlock.PlcId;
            ConditionCylinderId = _interlock.ConditionCylinderId;
            PreConditionID1 = _interlock.PreConditionID1;
            PreConditionID2 = _interlock.PreConditionID2;
            GoOrBack = _interlock.GoOrBack;

            // 表示用のシリンダー名を取得
            var cylinder = _allCylinders.FirstOrDefault(c => c.Id == _interlock.CylinderId);
            CylinderDisplayName = cylinder != null ? $"{cylinder.CYNum} - {cylinder.PUCO}" : _interlock.CylinderId.ToString();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                // プロパティを更新
                _interlock.PlcId = PlcId;
                _interlock.ConditionCylinderId = ConditionCylinderId;
                _interlock.PreConditionID1 = PreConditionID1;
                _interlock.PreConditionID2 = PreConditionID2;
                _interlock.GoOrBack = GoOrBack;

                // データベースに保存
                await _repository.UpdateInterlockAsync(_interlock);

                DialogResult = true;
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"保存中にエラーが発生しました: {ex.Message}", "エラー",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
            RequestClose?.Invoke();
        }

        public event Action? RequestClose;

        /// <summary>
        /// 更新されたInterlockを取得
        /// </summary>
        public Interlock GetUpdatedInterlock() => _interlock;
    }
}
