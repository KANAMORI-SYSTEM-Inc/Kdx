using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Collections.ObjectModel;

namespace KdxDesigner.ViewModels
{
    /// <summary>
    /// インターロック条件プロパティウィンドウのViewModel
    /// </summary>
    public partial class InterlockConditionPropertiesViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository;
        private readonly InterlockConditionDTO _condition;

        // 複合主キー（読み取り専用）
        [ObservableProperty] private int _cylinderId;
        [ObservableProperty] private int _conditionNumber;
        [ObservableProperty] private int _interlockSortId;

        // 編集可能フィールド
        [ObservableProperty] private int? _conditionTypeId;
        [ObservableProperty] private string? _name;
        [ObservableProperty] private string? _device;
        [ObservableProperty] private bool? _isOnCondition;
        [ObservableProperty] private string? _comment1;
        [ObservableProperty] private string? _comment2;

        // マスターデータ
        [ObservableProperty] private ObservableCollection<InterlockConditionType> _conditionTypes = new();

        public bool DialogResult { get; private set; }

        public InterlockConditionPropertiesViewModel(ISupabaseRepository repository, InterlockConditionDTO condition, IEnumerable<InterlockConditionType> conditionTypes)
        {
            _repository = repository;
            _condition = condition;

            // 条件タイプリストを設定
            ConditionTypes = new ObservableCollection<InterlockConditionType>(conditionTypes);

            // プロパティを読み込み
            LoadProperties();
        }

        private void LoadProperties()
        {
            CylinderId = _condition.CylinderId;
            ConditionNumber = _condition.ConditionNumber;
            InterlockSortId = _condition.InterlockSortId;
            ConditionTypeId = _condition.ConditionTypeId;
            Name = _condition.Name;
            Device = _condition.Device;
            IsOnCondition = _condition.IsOnCondition;
            Comment1 = _condition.Comment1;
            Comment2 = _condition.Comment2;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                // プロパティを更新
                _condition.ConditionTypeId = ConditionTypeId;
                _condition.Name = Name;
                _condition.Device = Device;
                _condition.IsOnCondition = IsOnCondition;
                _condition.Comment1 = Comment1;
                _condition.Comment2 = Comment2;

                // ConditionTypeナビゲーションプロパティを更新
                _condition.ConditionType = ConditionTypes.FirstOrDefault(ct => ct.Id == ConditionTypeId);

                // データベースに保存
                await _repository.UpdateInterlockConditionAsync(_condition);

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
        /// 更新されたInterlockConditionDTOを取得
        /// </summary>
        public InterlockConditionDTO GetUpdatedCondition() => _condition;
    }
}
