using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Contracts.Enums;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.Models;
using KdxDesigner.Services.ErrorMessageGenerator;
using KdxDesigner.Services.MnemonicDevice;
using System.Collections.ObjectModel;
using System.Windows;

namespace KdxDesigner.ViewModels.ErrorMessage
{
    /// <summary>
    /// エラーメッセージ生成画面のViewModel
    /// PLCごとにエラー番号1~9999を割り当て、GeneratedErrorテーブルに保存
    /// </summary>
    public partial class ErrorMessageGeneratorViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository;
        private readonly IMnemonicDeviceMemoryStore? _memoryStore;
        private readonly IErrorMessageGenerator _errorMessageGenerator;
        private readonly List<CylinderInterlockData>? _cylinderInterlockDataList;

        [ObservableProperty]
        private int _plcId;

        [ObservableProperty]
        private int _startErrorNum = 1;

        [ObservableProperty]
        private int _deviceStartM = 1000;

        [ObservableProperty]
        private int _deviceStartT = 500;

        [ObservableProperty]
        private bool _generateInterlock = true;

        [ObservableProperty]
        private bool _generateOperation = false;

        [ObservableProperty]
        private int _interlockInputCount;

        [ObservableProperty]
        private int _generatedErrorCount;

        [ObservableProperty]
        private ObservableCollection<GeneratedError> _previewErrors = new();

        [ObservableProperty]
        private bool _canSave;

        [ObservableProperty]
        private string _dataSourceInfo = "";

        private List<InterlockErrorInput> _interlockInputs = new();

        /// <summary>
        /// メモリストアからデータを取得するコンストラクタ
        /// </summary>
        public ErrorMessageGeneratorViewModel(
            ISupabaseRepository repository,
            IMnemonicDeviceMemoryStore memoryStore,
            int plcId)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _memoryStore = memoryStore ?? throw new ArgumentNullException(nameof(memoryStore));
            _errorMessageGenerator = new ErrorMessageGenerator(repository);
            _cylinderInterlockDataList = null;
            PlcId = plcId;
            DataSourceInfo = "メモリストアからデータを取得";

            // 初期化時にメモリストアからデータをロードし、次のエラー番号を取得
            _ = InitializeAsync();
        }

        /// <summary>
        /// CylinderInterlockDataから直接データを取得するコンストラクタ
        /// より詳細な情報を使用したエラーメッセージ生成が可能
        /// </summary>
        public ErrorMessageGeneratorViewModel(
            ISupabaseRepository repository,
            List<CylinderInterlockData> cylinderInterlockDataList,
            int plcId)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _memoryStore = null;
            _errorMessageGenerator = new ErrorMessageGenerator(repository);
            _cylinderInterlockDataList = cylinderInterlockDataList ?? throw new ArgumentNullException(nameof(cylinderInterlockDataList));
            PlcId = plcId;
            DataSourceInfo = "CylinderInterlockDataから直接取得（詳細情報付き）";

            // 初期化時にCylinderInterlockDataからデータをロードし、次のエラー番号を取得
            _ = InitializeFromCylinderDataAsync();
        }

        /// <summary>
        /// 初期化処理（メモリストアから）
        /// </summary>
        private async Task InitializeAsync()
        {
            await LoadMemoryDataAsync();
            await LoadNextErrorNumAsync();
        }

        /// <summary>
        /// 初期化処理（CylinderInterlockDataから）
        /// </summary>
        private async Task InitializeFromCylinderDataAsync()
        {
            LoadFromCylinderData();
            await LoadNextErrorNumAsync();
        }

        /// <summary>
        /// CylinderInterlockDataからInterlockErrorInputを構築
        /// </summary>
        private void LoadFromCylinderData()
        {
            if (_cylinderInterlockDataList == null)
            {
                return;
            }

            _interlockInputs = _errorMessageGenerator.BuildInterlockErrorInputsFromCylinderData(
                _cylinderInterlockDataList,
                PlcId);

            InterlockInputCount = _interlockInputs.Count;
        }

        /// <summary>
        /// 次のエラー番号を取得
        /// </summary>
        private async Task LoadNextErrorNumAsync()
        {
            try
            {
                StartErrorNum = await _repository.GetNextErrorNumForPlcAsync(PlcId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadNextErrorNumAsync error: {ex.Message}");
                StartErrorNum = 1;
            }
        }

        /// <summary>
        /// メモリストアからデータをロード
        /// </summary>
        private async Task LoadMemoryDataAsync()
        {
            if (_memoryStore == null)
            {
                return;
            }

            try
            {
                // キャッシュされたメモリデータを取得
                var memories = _memoryStore.GetCachedMemories(PlcId);

                // Interlock関連のメモリを取得
                var interlockMemories = memories
                    .Where(m => m.MnemonicId == (int)MnemonicType.Interlock)
                    .ToList();

                // シリンダー、インターロック情報を取得
                var cylinders = await _repository.GetCYsAsync();
                var interlocks = await _repository.GetInterlocksByPlcIdAsync(PlcId);
                var conditionTypes = await _repository.GetInterlockConditionTypesAsync();

                // InterlockErrorInputを構築
                // Note: InterlockConditionsはメモリ情報から解析するため、空リストを渡す
                _interlockInputs = _errorMessageGenerator.BuildInterlockErrorInputs(
                    interlockMemories,
                    cylinders,
                    interlocks,
                    new List<InterlockCondition>(),
                    conditionTypes);

                InterlockInputCount = _interlockInputs.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadMemoryDataAsync error: {ex.Message}");
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// プレビュー生成
        /// </summary>
        [RelayCommand]
        private async Task GeneratePreviewAsync()
        {
            try
            {
                var allErrors = new List<GeneratedError>();

                if (GenerateInterlock && _interlockInputs.Count > 0)
                {
                    var interlockErrors = await _errorMessageGenerator.GenerateInterlockErrorsAsync(
                        _interlockInputs,
                        StartErrorNum,
                        DeviceStartM,
                        DeviceStartT);
                    allErrors.AddRange(interlockErrors);
                }

                // Operation用の生成は将来実装
                // if (GenerateOperation) { ... }

                PreviewErrors = new ObservableCollection<GeneratedError>(allErrors);
                GeneratedErrorCount = allErrors.Count;
                CanSave = allErrors.Count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GeneratePreviewAsync error: {ex.Message}");
                MessageBox.Show($"プレビュー生成に失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// エラーメッセージを保存
        /// </summary>
        [RelayCommand]
        private async Task SaveErrorsAsync()
        {
            if (PreviewErrors == null || PreviewErrors.Count == 0)
            {
                MessageBox.Show("保存するエラーメッセージがありません。先にプレビューを生成してください。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"{PreviewErrors.Count}件のエラーメッセージを保存しますか？\n" +
                $"PLC ID: {PlcId}\n" +
                $"エラー番号: {StartErrorNum} ～ {StartErrorNum + PreviewErrors.Count - 1}\n" +
                $"既存の同一エラー番号は上書きされます。",
                "確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                // GeneratedErrorテーブルに保存
                await _repository.SaveGeneratedErrorsBatchAsync(PreviewErrors.ToList());

                MessageBox.Show(
                    $"{PreviewErrors.Count}件のエラーメッセージを保存しました。\n" +
                    $"エラー番号: {StartErrorNum} ～ {StartErrorNum + PreviewErrors.Count - 1}",
                    "完了",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // 次のエラー番号を更新
                await LoadNextErrorNumAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveErrorsAsync error: {ex.Message}");
                MessageBox.Show($"保存に失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        [RelayCommand]
        private void Close(Window? window)
        {
            window?.Close();
        }
    }
}
