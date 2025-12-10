using Kdx.Contracts.DTOs;
using KdxDesigner.ViewModels.IOEditor;
using KdxDesigner.Views.Common;
using System.Windows;
using System.Windows.Input;

namespace KdxDesigner.ViewModels
{
    public partial class InterlockPreConditionViewModel
    {
        public ICommand AddPreCondition1Command { get; }
        public ICommand DeletePreCondition1Command { get; }
        public ICommand AddPreCondition2Command { get; }
        public ICommand DeletePreCondition2Command { get; }
        public ICommand AddPreCondition3Command { get; }
        public ICommand DeletePreCondition3Command { get; }
        public ICommand SearchIOCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void AddPreCondition1(object? parameter)
        {
            var newCondition = new InterlockPrecondition1
            {
                ConditionName = "新規条件",
                Description = ""
            };
            PreCondition1List.Add(newCondition);
            SelectedPreCondition1 = newCondition;
        }

        private bool CanDeletePreCondition1(object? parameter) => SelectedPreCondition1 != null;

        private void DeletePreCondition1(object? parameter)
        {
            if (SelectedPreCondition1 == null)
            {
                return;
            }

            var result = MessageBox.Show("選択した前提条件1を削除しますか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PreCondition1List.Remove(SelectedPreCondition1);
                SelectedPreCondition1 = null;
                IsPreCondition1Selected = false;
            }
        }

        private void AddPreCondition2(object? parameter)
        {
            var newCondition = new InterlockPrecondition2
            {
                InterlockMode = "新規モード",
                IsEnableProcess = false
            };
            PreCondition2List.Add(newCondition);
            SelectedPreCondition2 = newCondition;
        }

        private bool CanDeletePreCondition2(object? parameter) => SelectedPreCondition2 != null;

        private void DeletePreCondition2(object? parameter)
        {
            if (SelectedPreCondition2 == null)
            {
                return;
            }

            var result = MessageBox.Show("選択した前提条件2を削除しますか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PreCondition2List.Remove(SelectedPreCondition2);
                SelectedPreCondition2 = null;
                IsPreCondition2Selected = false;
            }
        }

        private void AddPreCondition3(object? parameter)
        {
            var newCondition = new InterlockPrecondition3
            {
                ConditionType = "IO",
                IOAddress = "",
                DeviceAddress = "",
                IsOnCondition = true,
                Description = "新規IO/デバイス条件"
            };
            PreCondition3List.Add(newCondition);
            SelectedPreCondition3 = newCondition;
        }

        private bool CanDeletePreCondition3(object? parameter) => SelectedPreCondition3 != null;

        private void DeletePreCondition3(object? parameter)
        {
            if (SelectedPreCondition3 == null)
            {
                return;
            }

            var result = MessageBox.Show("選択した前提条件3を削除しますか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PreCondition3List.Remove(SelectedPreCondition3);
                SelectedPreCondition3 = null;
                IsPreCondition3Selected = false;
            }
        }

        /// <summary>
        /// IO検索ウィンドウを開いてIOアドレスを選択
        /// </summary>
        private async Task SearchIOAsync()
        {
            if (SelectedPreCondition3 == null)
            {
                MessageBox.Show("先に前提条件3を選択してください。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var viewModel = new IOSearchViewModel(_supabaseRepository, _plcId, SelectedPreCondition3.IOAddress);
            var window = new IOSearchWindow
            {
                DataContext = viewModel,
                Owner = _window
            };

            if (window.ShowDialog() == true && viewModel.SelectedIO != null)
            {
                SelectedPreCondition3.IOAddress = viewModel.SelectedIO.Address;
                SelectedPreCondition3.ConditionType = "IO";
                // 説明も更新（IO名があれば）
                if (!string.IsNullOrEmpty(viewModel.SelectedIO.IOName))
                {
                    SelectedPreCondition3.Description = viewModel.SelectedIO.IOName;
                }
                OnPropertyChanged(nameof(SelectedPreCondition3));
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                // PreCondition1の保存/更新
                if (PreCondition1List.Any())
                {
                    await _supabaseRepository.UpsertInterlockPrecondition1ListAsync(PreCondition1List.ToList());
                }

                // PreCondition2の保存/更新
                if (PreCondition2List.Any())
                {
                    await _supabaseRepository.UpsertInterlockPrecondition2ListAsync(PreCondition2List.ToList());
                }

                // PreCondition3の保存/更新
                if (PreCondition3List.Any())
                {
                    await _supabaseRepository.UpsertInterlockPrecondition3ListAsync(PreCondition3List.ToList());
                }

                // InterlockのPreConditionIDを更新
                if (IsPreCondition1Selected && SelectedPreCondition1 != null)
                {
                    _interlock.PreConditionID1 = SelectedPreCondition1.Id;
                }
                else
                {
                    _interlock.PreConditionID1 = null;
                }

                if (IsPreCondition2Selected && SelectedPreCondition2 != null)
                {
                    _interlock.PreConditionID2 = SelectedPreCondition2.Id;
                }
                else
                {
                    _interlock.PreConditionID2 = null;
                }

                if (IsPreCondition3Selected && SelectedPreCondition3 != null)
                {
                    _interlock.PreConditionID3 = SelectedPreCondition3.Id;
                }
                else
                {
                    _interlock.PreConditionID3 = null;
                }

                // Interlockをデータベースに保存
                await _supabaseRepository.UpsertInterlocksAsync(new List<Interlock> { _interlock });

                MessageBox.Show("前提条件を保存しました。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                _window.DialogResult = true;
                _window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存に失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object? parameter)
        {
            _window.DialogResult = false;
            _window.Close();
        }
    }
}
