using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Windows;

namespace KdxDesigner.ViewModels.ProjectInfo
{
    /// <summary>
    /// PLCプロパティウィンドウのViewModel
    /// PLCの新規作成・編集を行う
    /// </summary>
    public partial class PLCPropertiesViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository;
        private readonly PLC? _originalPLC;

        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string? _plcName;

        [ObservableProperty]
        private string? _maker;

        [ObservableProperty]
        private int? _modelId;

        [ObservableProperty]
        private ObservableCollection<Model> _models = new();

        [ObservableProperty]
        private bool _isEditMode;

        public bool DialogResult { get; private set; }
        public event Action? RequestClose;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="repository">リポジトリ</param>
        /// <param name="plc">編集対象のPLC（新規作成時はnull）</param>
        /// <param name="models">機種リスト</param>
        public PLCPropertiesViewModel(ISupabaseRepository repository, PLC? plc, IEnumerable<Model> models)
        {
            _repository = repository;
            _originalPLC = plc;
            IsEditMode = plc != null;
            Models = new ObservableCollection<Model>(models);

            if (plc != null)
            {
                Id = plc.Id;
                PlcName = plc.PlcName;
                Maker = plc.Maker;
                ModelId = plc.ModelId;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(PlcName))
            {
                MessageBox.Show("PLC名を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (IsEditMode)
                {
                    // 更新
                    var plc = new PLC
                    {
                        Id = Id,
                        PlcName = PlcName,
                        Maker = Maker,
                        ModelId = ModelId
                    };
                    await _repository.UpdatePLCAsync(plc);
                    MessageBox.Show("PLC情報を更新しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // 新規作成
                    var plc = new PLC
                    {
                        PlcName = PlcName,
                        Maker = Maker,
                        ModelId = ModelId
                    };
                    await _repository.AddPLCAsync(plc);
                    MessageBox.Show("PLCを追加しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
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
