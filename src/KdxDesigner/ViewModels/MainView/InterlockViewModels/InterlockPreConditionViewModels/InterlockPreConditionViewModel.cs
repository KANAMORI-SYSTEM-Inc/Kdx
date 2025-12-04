using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace KdxDesigner.ViewModels
{
    public partial class InterlockPreConditionViewModel : INotifyPropertyChanged
    {
        public InterlockPreConditionViewModel(SupabaseRepository supabaseRepository, Interlock interlock, Window window)
        {
            _supabaseRepository = supabaseRepository;
            _interlock = interlock;
            _window = window;

            PreCondition1List = new ObservableCollection<InterlockPrecondition1>();
            PreCondition2List = new ObservableCollection<InterlockPrecondition2>();

            AddPreCondition1Command = new RelayCommand(() => AddPreCondition1(null));
            DeletePreCondition1Command = new RelayCommand(() => DeletePreCondition1(null), () => CanDeletePreCondition1(null));
            AddPreCondition2Command = new RelayCommand(() => AddPreCondition2(null));
            DeletePreCondition2Command = new RelayCommand(() => DeletePreCondition2(null), () => CanDeletePreCondition2(null));
            SaveCommand = new RelayCommand(async () => await SaveAsync());
            CancelCommand = new RelayCommand(() => Cancel(null));

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // PreCondition1のリストを取得
                var preCondition1List = await _supabaseRepository.GetInterlockPrecondition1ListAsync();
                PreCondition1List.Clear();
                foreach (var item in preCondition1List)
                {
                    PreCondition1List.Add(item);
                }

                // PreCondition2のリストを取得
                var preCondition2List = await _supabaseRepository.GetInterlockPrecondition2ListAsync();
                PreCondition2List.Clear();
                foreach (var item in preCondition2List)
                {
                    PreCondition2List.Add(item);
                }

                // 現在のInterlockに設定されているPreConditionを選択
                if (_interlock.PreConditionID1.HasValue)
                {
                    SelectedPreCondition1 = PreCondition1List.FirstOrDefault(p => p.Id == _interlock.PreConditionID1.Value);
                    IsPreCondition1Selected = SelectedPreCondition1 != null;
                }

                if (_interlock.PreConditionID2.HasValue)
                {
                    SelectedPreCondition2 = PreCondition2List.FirstOrDefault(p => p.Id == _interlock.PreConditionID2.Value);
                    IsPreCondition2Selected = SelectedPreCondition2 != null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
