using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace KdxDesigner.ViewModels.PropertyWindow
{
    /// <summary>
    /// 前提条件2プロパティウィンドウのViewModel
    /// </summary>
    public partial class InterlockPrecondition2PropertiesViewModel : ObservableObject
    {
        private readonly Window _window;
        private readonly InterlockPrecondition2 _originalPrecondition;

        [ObservableProperty]
        private bool _isEnableProcess;

        [ObservableProperty]
        private string? _interlockMode;

        [ObservableProperty]
        private ProcessDetail? _selectedStartProcessDetail;

        [ObservableProperty]
        private ProcessDetail? _selectedEndProcessDetail;

        [ObservableProperty]
        private string? _searchText;

        public ObservableCollection<ProcessDetail> ProcessDetails { get; }

        /// <summary>
        /// フィルタされたProcessDetailのビュー
        /// </summary>
        public ICollectionView FilteredProcessDetails { get; }

        /// <summary>
        /// 編集結果のInterlockPrecondition2
        /// </summary>
        public InterlockPrecondition2 Result { get; private set; }

        /// <summary>
        /// 新規作成モードかどうか
        /// </summary>
        public bool IsNewMode { get; }

        public InterlockPrecondition2PropertiesViewModel(
            Window window,
            InterlockPrecondition2? precondition,
            ObservableCollection<ProcessDetail> processDetails)
        {
            _window = window;
            ProcessDetails = processDetails;
            IsNewMode = precondition == null;

            // フィルタ用のCollectionViewを作成
            FilteredProcessDetails = CollectionViewSource.GetDefaultView(ProcessDetails);
            FilteredProcessDetails.Filter = FilterProcessDetails;

            if (precondition == null)
            {
                // 新規作成モード
                _originalPrecondition = new InterlockPrecondition2
                {
                    InterlockMode = "新規モード",
                    IsEnableProcess = false
                };
            }
            else
            {
                // 編集モード - コピーを作成
                _originalPrecondition = precondition;
            }

            // プロパティを初期化
            IsEnableProcess = _originalPrecondition.IsEnableProcess;
            InterlockMode = _originalPrecondition.InterlockMode;

            // ProcessDetailの選択を復元
            if (_originalPrecondition.StartDetailId.HasValue)
            {
                SelectedStartProcessDetail = ProcessDetails.FirstOrDefault(p => p.Id == _originalPrecondition.StartDetailId.Value);
            }
            if (_originalPrecondition.EndDetailId.HasValue)
            {
                SelectedEndProcessDetail = ProcessDetails.FirstOrDefault(p => p.Id == _originalPrecondition.EndDetailId.Value);
            }

            Result = _originalPrecondition;
        }

        /// <summary>
        /// ProcessDetailのフィルタ条件
        /// </summary>
        private bool FilterProcessDetails(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            if (obj is ProcessDetail pd)
            {
                var searchLower = SearchText.ToLower();
                return (pd.DetailName?.ToLower().Contains(searchLower) ?? false) ||
                       pd.Id.ToString().Contains(searchLower) ||
                       pd.CycleId.ToString().Contains(searchLower);
            }
            return false;
        }

        partial void OnSearchTextChanged(string? value)
        {
            FilteredProcessDetails.Refresh();
        }

        [RelayCommand]
        private void ClearStartDetail()
        {
            SelectedStartProcessDetail = null;
        }

        [RelayCommand]
        private void ClearEndDetail()
        {
            SelectedEndProcessDetail = null;
        }

        [RelayCommand]
        private void Ok()
        {
            // 結果を更新
            Result = new InterlockPrecondition2
            {
                Id = _originalPrecondition.Id,
                IsEnableProcess = IsEnableProcess,
                InterlockMode = InterlockMode,
                StartDetailId = SelectedStartProcessDetail?.Id,
                EndDetailId = SelectedEndProcessDetail?.Id
            };

            _window.DialogResult = true;
            _window.Close();
        }

        [RelayCommand]
        private void Cancel()
        {
            _window.DialogResult = false;
            _window.Close();
        }
    }
}
