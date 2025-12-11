using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.ViewModels.IOEditor;
using KdxDesigner.Views.Common;
using System.Windows;

namespace KdxDesigner.ViewModels.PropertyWindow
{
    /// <summary>
    /// 前提条件3プロパティウィンドウのViewModel
    /// </summary>
    public partial class InterlockPrecondition3PropertiesViewModel : ObservableObject
    {
        private readonly Window _window;
        private readonly SupabaseRepository _supabaseRepository;
        private readonly int _plcId;
        private readonly InterlockPrecondition3 _originalPrecondition;

        [ObservableProperty]
        private string? _conditionType;

        [ObservableProperty]
        private string? _ioAddress;

        [ObservableProperty]
        private string? _deviceAddress;

        [ObservableProperty]
        private bool _isOnCondition;

        [ObservableProperty]
        private string? _description;

        /// <summary>
        /// 編集結果のInterlockPrecondition3
        /// </summary>
        public InterlockPrecondition3 Result { get; private set; }

        /// <summary>
        /// 新規作成モードかどうか
        /// </summary>
        public bool IsNewMode { get; }

        public InterlockPrecondition3PropertiesViewModel(
            Window window,
            SupabaseRepository supabaseRepository,
            int plcId,
            InterlockPrecondition3? precondition)
        {
            _window = window;
            _supabaseRepository = supabaseRepository;
            _plcId = plcId;
            IsNewMode = precondition == null;

            if (precondition == null)
            {
                // 新規作成モード
                _originalPrecondition = new InterlockPrecondition3
                {
                    ConditionType = "IO",
                    IOAddress = "",
                    DeviceAddress = "",
                    IsOnCondition = true,
                    Description = ""
                };
            }
            else
            {
                // 編集モード
                _originalPrecondition = precondition;
            }

            // プロパティを初期化
            ConditionType = _originalPrecondition.ConditionType;
            IoAddress = _originalPrecondition.IOAddress;
            DeviceAddress = _originalPrecondition.DeviceAddress;
            IsOnCondition = _originalPrecondition.IsOnCondition;
            Description = _originalPrecondition.Description;

            Result = _originalPrecondition;
        }

        [RelayCommand]
        private async Task SearchIO()
        {
            var viewModel = new IOSearchViewModel(_supabaseRepository, _plcId, IoAddress);
            var window = new IOSearchWindow
            {
                DataContext = viewModel,
                Owner = _window
            };

            if (window.ShowDialog() == true && viewModel.SelectedIO != null)
            {
                IoAddress = viewModel.SelectedIO.Address;
                ConditionType = "IO";
                // 説明も更新（IO名があれば）
                if (!string.IsNullOrEmpty(viewModel.SelectedIO.IOName))
                {
                    Description = viewModel.SelectedIO.IOName;
                }
            }
        }

        [RelayCommand]
        private void Ok()
        {
            // 結果を更新
            Result = new InterlockPrecondition3
            {
                Id = _originalPrecondition.Id,
                ConditionType = ConditionType,
                IOAddress = IoAddress,
                DeviceAddress = DeviceAddress,
                IsOnCondition = IsOnCondition,
                Description = Description
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
