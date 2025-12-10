using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Windows;

namespace KdxDesigner.ViewModels.ProjectInfo
{
    /// <summary>
    /// 機種プロパティウィンドウのViewModel
    /// Modelの新規作成・編集を行う
    /// </summary>
    public partial class ModelPropertiesViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository;
        private readonly Model? _originalModel;

        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string? _modelName;

        [ObservableProperty]
        private int? _companyId;

        [ObservableProperty]
        private ObservableCollection<Company> _companies = new();

        [ObservableProperty]
        private bool _isEditMode;

        public bool DialogResult { get; private set; }
        public event Action? RequestClose;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="repository">リポジトリ</param>
        /// <param name="model">編集対象の機種（新規作成時はnull）</param>
        /// <param name="companies">会社リスト</param>
        public ModelPropertiesViewModel(ISupabaseRepository repository, Model? model, IEnumerable<Company> companies)
        {
            _repository = repository;
            _originalModel = model;
            IsEditMode = model != null;
            Companies = new ObservableCollection<Company>(companies);

            if (model != null)
            {
                Id = model.Id;
                ModelName = model.ModelName;
                CompanyId = model.CompanyId;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(ModelName))
            {
                MessageBox.Show("機種名を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (IsEditMode)
                {
                    // 更新
                    var model = new Model
                    {
                        Id = Id,
                        ModelName = ModelName,
                        CompanyId = CompanyId
                    };
                    await _repository.UpdateModelAsync(model);
                    MessageBox.Show("機種情報を更新しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // 新規作成
                    var model = new Model
                    {
                        ModelName = ModelName,
                        CompanyId = CompanyId
                    };
                    await _repository.AddModelAsync(model);
                    MessageBox.Show("機種を追加しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
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
