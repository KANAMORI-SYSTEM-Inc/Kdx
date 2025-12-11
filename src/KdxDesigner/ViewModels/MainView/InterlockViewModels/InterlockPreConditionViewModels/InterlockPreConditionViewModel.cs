using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace KdxDesigner.ViewModels
{
    public partial class InterlockPreConditionViewModel : INotifyPropertyChanged
    {
        public InterlockPreConditionViewModel(SupabaseRepository supabaseRepository, Interlock interlock, int plcId, Window window)
        {
            _supabaseRepository = supabaseRepository;
            _interlock = interlock;
            _plcId = plcId;
            _window = window;

            PreCondition1List = new ObservableCollection<InterlockPrecondition1>();
            PreCondition2List = new ObservableCollection<InterlockPrecondition2>();
            PreCondition3List = new ObservableCollection<InterlockPrecondition3>();
            ProcessDetails = new ObservableCollection<ProcessDetail>();

            AddPreCondition1Command = new RelayCommand(() => AddPreCondition1(null));
            AddPreCondition2Command = new RelayCommand(OpenAddPreCondition2Window);
            AddPreCondition3Command = new RelayCommand(OpenAddPreCondition3Window);
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

                // PreCondition3のリストを取得
                var preCondition3List = await _supabaseRepository.GetInterlockPrecondition3ListAsync();
                PreCondition3List.Clear();
                foreach (var item in preCondition3List)
                {
                    PreCondition3List.Add(item);
                }

                // ProcessDetailsを読み込み（PlcIdに紐づくCycleから取得）
                await LoadProcessDetailsAsync();

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

                if (_interlock.PreConditionID3.HasValue)
                {
                    SelectedPreCondition3 = PreCondition3List.FirstOrDefault(p => p.Id == _interlock.PreConditionID3.Value);
                    IsPreCondition3Selected = SelectedPreCondition3 != null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadProcessDetailsAsync()
        {
            try
            {
                ProcessDetails.Clear();

                // PlcIdに紐づくCycleを取得
                var cycles = await _supabaseRepository.GetCyclesByPlcIdAsync(_plcId);

                // 各CycleのProcessDetailを取得
                foreach (var cycle in cycles)
                {
                    var processDetails = await _supabaseRepository.GetProcessDetailsByCycleIdAsync(cycle.Id);
                    foreach (var pd in processDetails)
                    {
                        ProcessDetails.Add(pd);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ProcessDetailsの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
