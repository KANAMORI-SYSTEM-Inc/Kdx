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
        private List<CylinderInterlockData> _cylinderInterlockDataList = new();

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
        [NotifyPropertyChangedFor(nameof(IsNotInitialized))]
        private bool _isInitialized;

        public bool IsNotInitialized => !IsInitialized;

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
            PlcId = plcId;
            DataSourceInfo = "メモリストアからデータを取得";

            // 初期化時にメモリストアからデータをロードし、次のエラー番号を取得
            // エラーハンドリング付きで実行
            _ = Task.Run(async () =>
            {
                try
                {
                    await InitializeAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"初期化エラー (メモリストアから): {ex}");
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(
                            $"データの初期化に失敗しました:\n{ex.Message}\n\nメモリ設定を実行してInterlockデバイスを生成してください。",
                            "初期化エラー",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    });
                }
            });
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
            _cylinderInterlockDataList = cylinderInterlockDataList ?? new List<CylinderInterlockData>();
            PlcId = plcId;
            DataSourceInfo = $"CylinderInterlockDataから直接取得（{_cylinderInterlockDataList.Count}件）";

            // 初期化時にCylinderInterlockDataからデータをロードし、次のエラー番号を取得
            // エラーハンドリング付きで実行
            _ = Task.Run(async () =>
            {
                try
                {
                    await InitializeFromCylinderDataAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"初期化エラー (CylinderInterlockDataから): {ex}");
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(
                            $"データの初期化に失敗しました:\n{ex.Message}",
                            "初期化エラー",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    });
                }
            });
        }

        /// <summary>
        /// 初期化処理（メモリストアから）
        /// </summary>
        private async Task InitializeAsync()
        {
            await LoadMemoryDataAsync();
            await LoadNextErrorNumAsync();
            IsInitialized = true;
        }

        /// <summary>
        /// 初期化処理（CylinderInterlockDataから）
        /// </summary>
        private async Task InitializeFromCylinderDataAsync()
        {
            LoadFromCylinderData();
            await LoadNextErrorNumAsync();
            IsInitialized = true;
        }

        /// <summary>
        /// CylinderInterlockDataからInterlockErrorInputを構築
        /// </summary>
        private void LoadFromCylinderData()
        {
            if (_cylinderInterlockDataList == null || _cylinderInterlockDataList.Count == 0)
            {
                InterlockInputCount = 0;
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
        /// メモリストアからデータをロード（InterlockBuilder.csと同じ方法）
        /// </summary>
        private async Task LoadMemoryDataAsync()
        {
            if (_memoryStore == null)
            {
                return;
            }

            try
            {
                // メモリストアからCylinderInterlockDataを取得（メモリ保存時に構築済み）
                _cylinderInterlockDataList = _memoryStore.GetCylinderInterlockData(PlcId)
                    ?? new List<CylinderInterlockData>();

                if (_cylinderInterlockDataList.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("CylinderInterlockDataが見つかりません。メモリ設定を実行してください。");
                    InterlockInputCount = 0;
                    return;
                }

                // CylinderInterlockDataからInterlockErrorInputを構築
                _interlockInputs = _errorMessageGenerator.BuildInterlockErrorInputsFromCylinderData(
                    _cylinderInterlockDataList,
                    PlcId);

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
                // 初期化チェック
                if (!IsInitialized)
                {
                    MessageBox.Show("データの初期化中です。しばらくお待ちください。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // データ存在チェック
                if (GenerateInterlock && _interlockInputs.Count == 0)
                {
                    MessageBox.Show(
                        "Interlockデータが見つかりません。\n" +
                        "メモリ設定を実行してInterlockデバイスを生成してください。",
                        "情報",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

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

                if (allErrors.Count == 0)
                {
                    MessageBox.Show("生成対象のエラーメッセージがありません。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GeneratePreviewAsync error: {ex.Message}");
                MessageBox.Show($"プレビュー生成に失敗しました: {ex.Message}\n\nスタックトレース:\n{ex.StackTrace}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
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
