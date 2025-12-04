using CommunityToolkit.Mvvm.Input;
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

        private InterlockPrecondition1? _selectedPreCondition1;
        private InterlockPrecondition2? _selectedPreCondition2;
        private bool _isPreCondition1Selected;
        private bool _isPreCondition2Selected;

        // Public collections
        public ObservableCollection<InterlockPrecondition1> PreCondition1List { get; set; }
        public ObservableCollection<InterlockPrecondition2> PreCondition2List { get; set; }

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
    }
}
