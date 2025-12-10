using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Windows;

namespace KdxDesigner.ViewModels.ProjectInfo
{
    /// <summary>
    /// サイクルプロパティウィンドウのViewModel
    /// Cycleの新規作成・編集を行う
    /// </summary>
    public partial class CyclePropertiesViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository;
        private readonly Cycle? _originalCycle;

        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string? _cycleName;

        [ObservableProperty]
        private int _plcId;

        [ObservableProperty]
        private string _startDevice = "L1000";

        [ObservableProperty]
        private string _resetDevice = "L1001";

        [ObservableProperty]
        private string _pauseDevice = "L1002";

        [ObservableProperty]
        private ObservableCollection<PLC> _pLCs = new();

        [ObservableProperty]
        private bool _isEditMode;

        public bool DialogResult { get; private set; }
        public event Action? RequestClose;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="repository">リポジトリ</param>
        /// <param name="cycle">編集対象のサイクル（新規作成時はnull）</param>
        /// <param name="plcs">PLCリスト</param>
        public CyclePropertiesViewModel(ISupabaseRepository repository, Cycle? cycle, IEnumerable<PLC> plcs)
        {
            _repository = repository;
            _originalCycle = cycle;
            IsEditMode = cycle != null;
            PLCs = new ObservableCollection<PLC>(plcs);

            if (cycle != null)
            {
                Id = cycle.Id;
                CycleName = cycle.CycleName;
                PlcId = cycle.PlcId;
                StartDevice = cycle.StartDevice;
                ResetDevice = cycle.ResetDevice;
                PauseDevice = cycle.PauseDevice;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(CycleName))
            {
                MessageBox.Show("サイクル名を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PlcId == 0)
            {
                MessageBox.Show("PLCを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (IsEditMode)
                {
                    // 更新
                    var cycle = new Cycle
                    {
                        Id = Id,
                        CycleName = CycleName,
                        PlcId = PlcId,
                        StartDevice = StartDevice,
                        ResetDevice = ResetDevice,
                        PauseDevice = PauseDevice
                    };
                    await _repository.UpdateCycleAsync(cycle);
                    MessageBox.Show("サイクル情報を更新しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // 新規作成
                    var cycle = new Cycle
                    {
                        CycleName = CycleName,
                        PlcId = PlcId,
                        StartDevice = StartDevice,
                        ResetDevice = ResetDevice,
                        PauseDevice = PauseDevice
                    };
                    await _repository.AddCycleAsync(cycle);
                    MessageBox.Show("サイクルを追加しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存エラー: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
            RequestClose?.Invoke();
        }
    }
}
