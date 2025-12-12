using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;

namespace KdxDesigner.ViewModels
{
    /// <summary>
    /// インターロックIOプロパティウィンドウのViewModel
    /// </summary>
    public partial class InterlockIOPropertiesViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository;
        private readonly InterlockIO _interlockIO;

        // 複合主キー（読み取り専用）
        [ObservableProperty] private int _cylinderId;
        [ObservableProperty] private int _plcId;
        [ObservableProperty] private string _ioAddress = string.Empty;
        [ObservableProperty] private int _interlockSortId;
        [ObservableProperty] private int _conditionNumber;

        // 編集可能フィールド
        [ObservableProperty] private bool _isOnCondition;

        /// <summary>
        /// IO番号/インデックス (0から始まる)
        /// 同一InterlockCondition内での順序を示す
        /// </summary>
        [ObservableProperty] private int _ioIndex;

        // 表示用
        [ObservableProperty] private string? _ioName;

        public bool DialogResult { get; private set; }

        public InterlockIOPropertiesViewModel(ISupabaseRepository repository, InterlockIO interlockIO, string? ioName)
        {
            _repository = repository;
            _interlockIO = interlockIO;
            _ioName = ioName;

            // プロパティを読み込み
            LoadProperties();
        }

        private void LoadProperties()
        {
            CylinderId = _interlockIO.CylinderId;
            PlcId = _interlockIO.PlcId;
            IoAddress = _interlockIO.IOAddress;
            InterlockSortId = _interlockIO.InterlockSortId;
            ConditionNumber = _interlockIO.ConditionNumber;
            IsOnCondition = _interlockIO.IsOnCondition;
            IoIndex = _interlockIO.IoIndex;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                // プロパティを更新
                _interlockIO.IsOnCondition = IsOnCondition;
                _interlockIO.IoIndex = IoIndex;

                // データベースに保存
                await _repository.UpdateInterlockIOAsync(_interlockIO);

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
        /// 更新されたInterlockIOを取得
        /// </summary>
        public InterlockIO GetUpdatedInterlockIO() => _interlockIO;
    }
}
