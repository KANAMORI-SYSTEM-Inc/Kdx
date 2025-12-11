using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace KdxDesigner.ViewModels
{
    public partial class InterlockPreConditionViewModel
    {
        // Private fields
        private readonly SupabaseRepository _supabaseRepository;
        private readonly Interlock _interlock;
        private readonly Window _window;
        private readonly int _plcId;

        private InterlockPrecondition1? _selectedPreCondition1;
        private InterlockPrecondition2? _selectedPreCondition2;
        private InterlockPrecondition3? _selectedPreCondition3;
        private bool _isPreCondition1Selected;
        private bool _isPreCondition2Selected;
        private bool _isPreCondition3Selected;

        // Public collections
        public ObservableCollection<InterlockPrecondition1> PreCondition1List { get; set; }
        public ObservableCollection<InterlockPrecondition2> PreCondition2List { get; set; }
        public ObservableCollection<InterlockPrecondition3> PreCondition3List { get; set; }
        public ObservableCollection<ProcessDetail> ProcessDetails { get; set; }

        public InterlockPrecondition1? SelectedPreCondition1
        {
            get => _selectedPreCondition1;
            set
            {
                _selectedPreCondition1 = value;
                OnPropertyChanged();
                // 選択されたPreCondition1がInterlockに設定されているものと一致するか確認
                if (_selectedPreCondition1 != null && _interlock.PreConditionID1 == _selectedPreCondition1.Id)
                {
                    IsPreCondition1Selected = true;
                }
                // コマンドの状態を更新
                NotifyPreCondition1CommandsCanExecuteChanged();
            }
        }

        public InterlockPrecondition2? SelectedPreCondition2
        {
            get => _selectedPreCondition2;
            set
            {
                _selectedPreCondition2 = value;
                OnPropertyChanged();
                // 選択されたPreCondition2がInterlockに設定されているものと一致するか確認
                if (_selectedPreCondition2 != null && _interlock.PreConditionID2 == _selectedPreCondition2.Id)
                {
                    IsPreCondition2Selected = true;
                }
                // コマンドの状態を更新
                NotifyPreCondition2CommandsCanExecuteChanged();
            }
        }

        public bool IsPreCondition1Selected
        {
            get => _isPreCondition1Selected;
            set
            {
                _isPreCondition1Selected = value;
                OnPropertyChanged();
            }
        }

        public bool IsPreCondition2Selected
        {
            get => _isPreCondition2Selected;
            set
            {
                _isPreCondition2Selected = value;
                OnPropertyChanged();
            }
        }

        public InterlockPrecondition3? SelectedPreCondition3
        {
            get => _selectedPreCondition3;
            set
            {
                _selectedPreCondition3 = value;
                OnPropertyChanged();
                // 選択されたPreCondition3がInterlockに設定されているものと一致するか確認
                if (_selectedPreCondition3 != null && _interlock.PreConditionID3 == _selectedPreCondition3.Id)
                {
                    IsPreCondition3Selected = true;
                }
                // コマンドの状態を更新
                NotifyPreCondition3CommandsCanExecuteChanged();
            }
        }

        public bool IsPreCondition3Selected
        {
            get => _isPreCondition3Selected;
            set
            {
                _isPreCondition3Selected = value;
                OnPropertyChanged();
            }
        }
    }
}
