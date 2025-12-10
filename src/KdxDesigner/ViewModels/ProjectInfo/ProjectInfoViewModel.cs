using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.Views.ProjectInfo;

namespace KdxDesigner.ViewModels.ProjectInfo
{
    /// <summary>
    /// プロジェクト情報管理ウィンドウのViewModel
    /// Company、Model、PLC、Cycleの一覧表示とCRUD操作を管理
    /// </summary>
    public partial class ProjectInfoViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository;

        #region Company Properties

        [ObservableProperty]
        private ObservableCollection<Company> _companies = new();

        [ObservableProperty]
        private Company? _selectedCompany;

        #endregion

        #region Model Properties

        [ObservableProperty]
        private ObservableCollection<ModelDisplayItem> _models = new();

        [ObservableProperty]
        private ModelDisplayItem? _selectedModel;

        #endregion

        #region PLC Properties

        [ObservableProperty]
        private ObservableCollection<PLCDisplayItem> _pLCs = new();

        [ObservableProperty]
        private PLCDisplayItem? _selectedPLC;

        #endregion

        #region Cycle Properties

        [ObservableProperty]
        private ObservableCollection<CycleDisplayItem> _cycles = new();

        [ObservableProperty]
        private CycleDisplayItem? _selectedCycle;

        #endregion

        public event Action? RequestClose;

        public ProjectInfoViewModel(ISupabaseRepository repository)
        {
            _repository = repository;
            _ = LoadDataAsync();
        }

        /// <summary>
        /// 全データを読み込む
        /// </summary>
        private async Task LoadDataAsync()
        {
            await LoadCompaniesAsync();
            await LoadModelsAsync();
            await LoadPLCsAsync();
            await LoadCyclesAsync();
        }

        #region Company Methods

        private async Task LoadCompaniesAsync()
        {
            var companies = await _repository.GetCompaniesAsync();
            Companies = new ObservableCollection<Company>(companies);
        }

        [RelayCommand]
        private async Task AddCompanyAsync()
        {
            var viewModel = new CompanyPropertiesViewModel(_repository, null);
            var window = new CompanyPropertiesWindow(viewModel);
            window.ShowDialog();

            if (viewModel.DialogResult)
            {
                await LoadCompaniesAsync();
                await LoadModelsAsync(); // 会社変更時に機種リストも更新
            }
        }

        [RelayCommand]
        private async Task EditCompanyAsync()
        {
            if (SelectedCompany == null)
            {
                MessageBox.Show("編集する会社を選択してください。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var viewModel = new CompanyPropertiesViewModel(_repository, SelectedCompany);
            var window = new CompanyPropertiesWindow(viewModel);
            window.ShowDialog();

            if (viewModel.DialogResult)
            {
                await LoadCompaniesAsync();
                await LoadModelsAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteCompanyAsync()
        {
            if (SelectedCompany == null)
            {
                MessageBox.Show("削除する会社を選択してください。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"会社「{SelectedCompany.CompanyName}」を削除しますか？\n関連する機種、PLC、サイクルも削除される可能性があります。",
                "削除確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _repository.DeleteCompanyAsync(SelectedCompany.Id);
                    await LoadDataAsync();
                    MessageBox.Show("会社を削除しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除エラー: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Model Methods

        private async Task LoadModelsAsync()
        {
            var models = await _repository.GetModelsAsync();
            var companies = await _repository.GetCompaniesAsync();

            var displayItems = models.Select(m => new ModelDisplayItem
            {
                Id = m.Id,
                ModelName = m.ModelName,
                CompanyId = m.CompanyId,
                CompanyName = companies.FirstOrDefault(c => c.Id == m.CompanyId)?.CompanyName
            }).ToList();

            Models = new ObservableCollection<ModelDisplayItem>(displayItems);
        }

        [RelayCommand]
        private async Task AddModelAsync()
        {
            var companies = await _repository.GetCompaniesAsync();
            var viewModel = new ModelPropertiesViewModel(_repository, null, companies);
            var window = new ModelPropertiesWindow(viewModel);
            window.ShowDialog();

            if (viewModel.DialogResult)
            {
                await LoadModelsAsync();
                await LoadPLCsAsync();
            }
        }

        [RelayCommand]
        private async Task EditModelAsync()
        {
            if (SelectedModel == null)
            {
                MessageBox.Show("編集する機種を選択してください。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var companies = await _repository.GetCompaniesAsync();
            var model = new Model
            {
                Id = SelectedModel.Id,
                ModelName = SelectedModel.ModelName,
                CompanyId = SelectedModel.CompanyId
            };

            var viewModel = new ModelPropertiesViewModel(_repository, model, companies);
            var window = new ModelPropertiesWindow(viewModel);
            window.ShowDialog();

            if (viewModel.DialogResult)
            {
                await LoadModelsAsync();
                await LoadPLCsAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteModelAsync()
        {
            if (SelectedModel == null)
            {
                MessageBox.Show("削除する機種を選択してください。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"機種「{SelectedModel.ModelName}」を削除しますか？\n関連するPLC、サイクルも削除される可能性があります。",
                "削除確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _repository.DeleteModelAsync(SelectedModel.Id);
                    await LoadModelsAsync();
                    await LoadPLCsAsync();
                    await LoadCyclesAsync();
                    MessageBox.Show("機種を削除しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除エラー: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region PLC Methods

        private async Task LoadPLCsAsync()
        {
            var plcs = await _repository.GetPLCsAsync();
            var models = await _repository.GetModelsAsync();

            var displayItems = plcs.Select(p => new PLCDisplayItem
            {
                Id = p.Id,
                PlcName = p.PlcName,
                Maker = p.Maker,
                ModelId = p.ModelId,
                ModelName = models.FirstOrDefault(m => m.Id == p.ModelId)?.ModelName
            }).ToList();

            PLCs = new ObservableCollection<PLCDisplayItem>(displayItems);
        }

        [RelayCommand]
        private async Task AddPLCAsync()
        {
            var models = await _repository.GetModelsAsync();
            var viewModel = new PLCPropertiesViewModel(_repository, null, models);
            var window = new PLCPropertiesWindow(viewModel);
            window.ShowDialog();

            if (viewModel.DialogResult)
            {
                await LoadPLCsAsync();
                await LoadCyclesAsync();
            }
        }

        [RelayCommand]
        private async Task EditPLCAsync()
        {
            if (SelectedPLC == null)
            {
                MessageBox.Show("編集するPLCを選択してください。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var models = await _repository.GetModelsAsync();
            var plc = new PLC
            {
                Id = SelectedPLC.Id,
                PlcName = SelectedPLC.PlcName,
                Maker = SelectedPLC.Maker,
                ModelId = SelectedPLC.ModelId
            };

            var viewModel = new PLCPropertiesViewModel(_repository, plc, models);
            var window = new PLCPropertiesWindow(viewModel);
            window.ShowDialog();

            if (viewModel.DialogResult)
            {
                await LoadPLCsAsync();
                await LoadCyclesAsync();
            }
        }

        [RelayCommand]
        private async Task DeletePLCAsync()
        {
            if (SelectedPLC == null)
            {
                MessageBox.Show("削除するPLCを選択してください。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"PLC「{SelectedPLC.PlcName}」を削除しますか？\n関連するサイクルも削除される可能性があります。",
                "削除確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _repository.DeletePLCAsync(SelectedPLC.Id);
                    await LoadPLCsAsync();
                    await LoadCyclesAsync();
                    MessageBox.Show("PLCを削除しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除エラー: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Cycle Methods

        private async Task LoadCyclesAsync()
        {
            var cycles = await _repository.GetCyclesAsync();
            var plcs = await _repository.GetPLCsAsync();

            var displayItems = cycles.Select(c => new CycleDisplayItem
            {
                Id = c.Id,
                CycleName = c.CycleName,
                PlcId = c.PlcId,
                PlcName = plcs.FirstOrDefault(p => p.Id == c.PlcId)?.PlcName,
                StartDevice = c.StartDevice,
                ResetDevice = c.ResetDevice,
                PauseDevice = c.PauseDevice
            }).ToList();

            Cycles = new ObservableCollection<CycleDisplayItem>(displayItems);
        }

        [RelayCommand]
        private async Task AddCycleAsync()
        {
            var plcs = await _repository.GetPLCsAsync();
            var viewModel = new CyclePropertiesViewModel(_repository, null, plcs);
            var window = new CyclePropertiesWindow(viewModel);
            window.ShowDialog();

            if (viewModel.DialogResult)
            {
                await LoadCyclesAsync();
            }
        }

        [RelayCommand]
        private async Task EditCycleAsync()
        {
            if (SelectedCycle == null)
            {
                MessageBox.Show("編集するサイクルを選択してください。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var plcs = await _repository.GetPLCsAsync();
            var cycle = new Cycle
            {
                Id = SelectedCycle.Id,
                CycleName = SelectedCycle.CycleName,
                PlcId = SelectedCycle.PlcId,
                StartDevice = SelectedCycle.StartDevice ?? "L1000",
                ResetDevice = SelectedCycle.ResetDevice ?? "L1001",
                PauseDevice = SelectedCycle.PauseDevice ?? "L1002"
            };

            var viewModel = new CyclePropertiesViewModel(_repository, cycle, plcs);
            var window = new CyclePropertiesWindow(viewModel);
            window.ShowDialog();

            if (viewModel.DialogResult)
            {
                await LoadCyclesAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteCycleAsync()
        {
            if (SelectedCycle == null)
            {
                MessageBox.Show("削除するサイクルを選択してください。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"サイクル「{SelectedCycle.CycleName}」を削除しますか？\n関連する工程、工程詳細、操作も削除される可能性があります。",
                "削除確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _repository.DeleteCycleAsync(SelectedCycle.Id);
                    await LoadCyclesAsync();
                    MessageBox.Show("サイクルを削除しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除エラー: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        [RelayCommand]
        private void Close()
        {
            RequestClose?.Invoke();
        }
    }

    #region Display Item Classes

    /// <summary>
    /// Model表示用のアイテムクラス（会社名を含む）
    /// </summary>
    public class ModelDisplayItem
    {
        public int Id { get; set; }
        public string? ModelName { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }

    /// <summary>
    /// PLC表示用のアイテムクラス（機種名を含む）
    /// </summary>
    public class PLCDisplayItem
    {
        public int Id { get; set; }
        public string? PlcName { get; set; }
        public string? Maker { get; set; }
        public int? ModelId { get; set; }
        public string? ModelName { get; set; }
    }

    /// <summary>
    /// Cycle表示用のアイテムクラス（PLC名を含む）
    /// </summary>
    public class CycleDisplayItem
    {
        public int Id { get; set; }
        public string? CycleName { get; set; }
        public int PlcId { get; set; }
        public string? PlcName { get; set; }
        public string? StartDevice { get; set; }
        public string? ResetDevice { get; set; }
        public string? PauseDevice { get; set; }
    }

    #endregion
}
