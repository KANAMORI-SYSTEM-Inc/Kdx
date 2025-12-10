using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

// Views名前空間エイリアス
using ViewsToolsCylinderManagement = KdxDesigner.Views.Tools.CylinderManagement;

namespace KdxDesigner.ViewModels
{
    /// <summary>
    /// シリンダー管理ウィンドウのViewModel
    /// </summary>
    public partial class CylinderManagementViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository;
        private readonly int _plcId;
        private Dictionary<int, string> _machineNameMap = new();
        private Dictionary<int, string> _driveSubMap = new();
        private string? _cylinderSearchText;

        private readonly ObservableCollection<CylinderViewModel> _allCylinders;
        private readonly ICollectionView _filteredCylinders;

        public ICollectionView FilteredCylinders => _filteredCylinders;

        [ObservableProperty]
        private CylinderViewModel? _selectedCylinder;

        public string? CylinderSearchText
        {
            get => _cylinderSearchText;
            set
            {
                _cylinderSearchText = value;
                OnPropertyChanged();
                _filteredCylinders.Refresh();
            }
        }

        public CylinderManagementViewModel(ISupabaseRepository repository, int plcId)
        {
            _repository = repository;
            _plcId = plcId;

            // Initialize cylinder list and filtering
            _allCylinders = new ObservableCollection<CylinderViewModel>();
            _filteredCylinders = CollectionViewSource.GetDefaultView(_allCylinders);
            _filteredCylinders.Filter = FilterCylinder;

            // シリンダーのリストを読み込み
            LoadCylinders();
        }

        private bool FilterCylinder(object obj)
        {
            if (obj is not CylinderViewModel cylinderVm) { return false; }
            if (string.IsNullOrWhiteSpace(CylinderSearchText)) { return true; }

            var searchLower = CylinderSearchText.ToLower();
            return (cylinderVm.CYNum?.ToLower().Contains(searchLower) ?? false) ||
                   (cylinderVm.PUCO?.ToLower().Contains(searchLower) ?? false) ||
                   (cylinderVm.Go?.ToLower().Contains(searchLower) ?? false) ||
                   (cylinderVm.Back?.ToLower().Contains(searchLower) ?? false) ||
                   (cylinderVm.OilNum?.ToLower().Contains(searchLower) ?? false) ||
                   (cylinderVm.MachineNameFullName?.ToLower().Contains(searchLower) ?? false) ||
                   cylinderVm.Id.ToString().Contains(searchLower);
        }

        /// <summary>
        /// シリンダーのリストを読み込み
        /// </summary>
        private async void LoadCylinders()
        {
            try
            {
                // マスターデータを読み込み
                var machineNames = await _repository.GetMachineNamesAsync();
                _machineNameMap = machineNames.ToDictionary(m => m.Id, m => m.FullName);

                var driveSubs = await _repository.GetDriveSubsAsync();
                _driveSubMap = driveSubs.ToDictionary(d => d.Id, d => d.DriveSubName ?? "");

                // シリンダーを読み込み
                var cylinders = await _repository.GetCYsAsync();
                var filteredCylinders = cylinders
                    .Where(c => c.PlcId == _plcId)
                    .OrderBy(c => c.SortNumber)
                    .ToList();

                _allCylinders.Clear();
                foreach (var cylinder in filteredCylinders)
                {
                    var cylinderViewModel = new CylinderViewModel(cylinder);
                    if (cylinder.MachineNameId.HasValue && _machineNameMap.TryGetValue(cylinder.MachineNameId.Value, out var machineName))
                    {
                        cylinderViewModel.MachineNameFullName = machineName;
                    }
                    if (cylinder.DriveSubId.HasValue && _driveSubMap.TryGetValue(cylinder.DriveSubId.Value, out var driveSubName))
                    {
                        cylinderViewModel.DriveSubName = driveSubName;
                    }
                    _allCylinders.Add(cylinderViewModel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"シリンダーの読み込み中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// シリンダー追加コマンド
        /// </summary>
        [RelayCommand]
        private async Task AddCylinder()
        {
            try
            {
                // 既存のシリンダー数を取得してSortNumberを設定
                int nextSortNumber = _allCylinders.Count > 0 ? _allCylinders.Max(c => c.SortNumber ?? 0) + 1 : 1;

                // 新しいCylinderオブジェクトを作成
                var newCylinder = new Cylinder
                {
                    PlcId = _plcId,
                    CYNum = "新規シリンダ",
                    PUCO = "PU",
                    SortNumber = nextSortNumber
                };

                // データベースに追加
                int newId = await _repository.AddCylinderAsync(newCylinder);
                newCylinder.Id = newId;

                // リストを再読み込み
                LoadCylinders();

                MessageBox.Show($"新しいシリンダーを追加しました。(ID: {newId})", "追加完了", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"シリンダーの追加中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// シリンダーコピーコマンド
        /// </summary>
        [RelayCommand]
        private async Task CopyCylinder()
        {
            if (SelectedCylinder == null)
            {
                MessageBox.Show("コピーするシリンダーを選択してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 既存のシリンダー数を取得してSortNumberを設定
                int nextSortNumber = _allCylinders.Count > 0 ? _allCylinders.Max(c => c.SortNumber ?? 0) + 1 : 1;

                // 選択されたシリンダーをコピー
                var sourceCylinder = SelectedCylinder.GetCylinder();
                var copiedCylinder = new Cylinder
                {
                    PlcId = _plcId,
                    CYNum = sourceCylinder.CYNum + " (コピー)",
                    PUCO = sourceCylinder.PUCO,
                    Go = sourceCylinder.Go,
                    Back = sourceCylinder.Back,
                    OilNum = sourceCylinder.OilNum,
                    MachineNameId = sourceCylinder.MachineNameId,
                    DriveSubId = sourceCylinder.DriveSubId,
                    PlaceId = sourceCylinder.PlaceId,
                    CYNameSub = sourceCylinder.CYNameSub,
                    SensorId = sourceCylinder.SensorId,
                    FlowType = sourceCylinder.FlowType,
                    GoSensorCount = sourceCylinder.GoSensorCount,
                    BackSensorCount = sourceCylinder.BackSensorCount,
                    RetentionSensorGo = sourceCylinder.RetentionSensorGo,
                    RetentionSensorBack = sourceCylinder.RetentionSensorBack,
                    SortNumber = nextSortNumber,
                    FlowCount = sourceCylinder.FlowCount,
                    FlowCYGo = sourceCylinder.FlowCYGo,
                    FlowCYBack = sourceCylinder.FlowCYBack
                };

                // データベースに追加
                int newId = await _repository.AddCylinderAsync(copiedCylinder);
                copiedCylinder.Id = newId;

                // リストを再読み込み
                LoadCylinders();

                MessageBox.Show($"シリンダーをコピーして追加しました。(ID: {newId})", "追加完了", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"シリンダーのコピー中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// シリンダー編集コマンド
        /// </summary>
        [RelayCommand]
        private void EditCylinder()
        {
            if (SelectedCylinder == null)
            {
                MessageBox.Show("編集するシリンダーを選択してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var window = new ViewsToolsCylinderManagement.CylinderPropertiesWindow(_repository, SelectedCylinder.GetCylinder());
                var mainWindow = Application.Current.Windows.OfType<MainView>().FirstOrDefault();
                if (mainWindow != null)
                {
                    window.Owner = mainWindow;
                }

                if (window.ShowDialog() == true)
                {
                    // シリンダーリストを再読み込み
                    LoadCylinders();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"シリンダーの編集中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// シリンダー削除コマンド
        /// </summary>
        [RelayCommand]
        private async Task DeleteCylinder()
        {
            if (SelectedCylinder == null)
            {
                MessageBox.Show("削除するシリンダーを選択してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"シリンダー「{SelectedCylinder.CYNum}」を削除しますか？",
                "確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _repository.DeleteCylinderAsync(SelectedCylinder.Id);
                    _allCylinders.Remove(SelectedCylinder);
                    MessageBox.Show("シリンダーを削除しました。", "削除完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"シリンダーの削除中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 検索クリアコマンド
        /// </summary>
        [RelayCommand]
        private void ClearCylinderSearch()
        {
            CylinderSearchText = string.Empty;
        }

        /// <summary>
        /// 更新コマンド
        /// </summary>
        [RelayCommand]
        private void Refresh()
        {
            LoadCylinders();
        }
    }
}
