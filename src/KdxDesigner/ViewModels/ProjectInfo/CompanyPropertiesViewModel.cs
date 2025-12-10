using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Windows;

namespace KdxDesigner.ViewModels.ProjectInfo
{
    /// <summary>
    /// 会社プロパティウィンドウのViewModel
    /// Companyの新規作成・編集を行う
    /// </summary>
    public partial class CompanyPropertiesViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository;
        private readonly Company? _originalCompany;

        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string? _companyName;

        [ObservableProperty]
        private string? _address;

        [ObservableProperty]
        private bool _isEditMode;

        public bool DialogResult { get; private set; }
        public event Action? RequestClose;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="repository">リポジトリ</param>
        /// <param name="company">編集対象の会社（新規作成時はnull）</param>
        public CompanyPropertiesViewModel(ISupabaseRepository repository, Company? company)
        {
            _repository = repository;
            _originalCompany = company;
            IsEditMode = company != null;

            if (company != null)
            {
                Id = company.Id;
                CompanyName = company.CompanyName;
                Address = company.Address;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                MessageBox.Show("会社名を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (IsEditMode)
                {
                    // 更新
                    var company = new Company
                    {
                        Id = Id,
                        CompanyName = CompanyName,
                        Address = Address,
                        CreatedAt = _originalCompany?.CreatedAt
                    };
                    await _repository.UpdateCompanyAsync(company);
                    MessageBox.Show("会社情報を更新しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // 新規作成
                    var company = new Company
                    {
                        CompanyName = CompanyName,
                        Address = Address,
                        CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    await _repository.AddCompanyAsync(company);
                    MessageBox.Show("会社を追加しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
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
