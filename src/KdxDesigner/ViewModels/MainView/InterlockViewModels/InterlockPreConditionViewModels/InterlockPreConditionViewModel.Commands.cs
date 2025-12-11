using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using KdxDesigner.ViewModels.PropertyWindow;
using KdxDesigner.Views.Interlock;
using System.Windows;
using System.Windows.Input;

namespace KdxDesigner.ViewModels
{
    public partial class InterlockPreConditionViewModel
    {
        // PreCondition1のコマンド
        public ICommand AddPreCondition1Command { get; }
        private RelayCommand? _deletePreCondition1Command;
        public ICommand DeletePreCondition1Command => _deletePreCondition1Command ??= new RelayCommand(() => DeletePreCondition1(null), () => CanDeletePreCondition1(null));

        // PreCondition2のコマンド
        public ICommand AddPreCondition2Command { get; }
        private RelayCommand? _editPreCondition2Command;
        public ICommand EditPreCondition2Command => _editPreCondition2Command ??= new RelayCommand(OpenEditPreCondition2Window, () => SelectedPreCondition2 != null);
        private RelayCommand? _deletePreCondition2Command;
        public ICommand DeletePreCondition2Command => _deletePreCondition2Command ??= new RelayCommand(() => DeletePreCondition2(null), () => CanDeletePreCondition2(null));

        // PreCondition3のコマンド
        public ICommand AddPreCondition3Command { get; }
        private RelayCommand? _editPreCondition3Command;
        public ICommand EditPreCondition3Command => _editPreCondition3Command ??= new RelayCommand(OpenEditPreCondition3Window, () => SelectedPreCondition3 != null);
        private RelayCommand? _deletePreCondition3Command;
        public ICommand DeletePreCondition3Command => _deletePreCondition3Command ??= new RelayCommand(() => DeletePreCondition3(null), () => CanDeletePreCondition3(null));

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        /// <summary>
        /// PreCondition1関連コマンドのCanExecuteを更新
        /// </summary>
        private void NotifyPreCondition1CommandsCanExecuteChanged()
        {
            _deletePreCondition1Command?.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// PreCondition2関連コマンドのCanExecuteを更新
        /// </summary>
        private void NotifyPreCondition2CommandsCanExecuteChanged()
        {
            _editPreCondition2Command?.NotifyCanExecuteChanged();
            _deletePreCondition2Command?.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// PreCondition3関連コマンドのCanExecuteを更新
        /// </summary>
        private void NotifyPreCondition3CommandsCanExecuteChanged()
        {
            _editPreCondition3Command?.NotifyCanExecuteChanged();
            _deletePreCondition3Command?.NotifyCanExecuteChanged();
        }

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

        /// <summary>
        /// 前提条件2を新規追加するプロパティウィンドウを開く
        /// </summary>
        private void OpenAddPreCondition2Window()
        {
            var window = new InterlockPrecondition2PropertiesWindow { Owner = _window };
            var viewModel = new InterlockPrecondition2PropertiesViewModel(window, null, ProcessDetails);
            window.DataContext = viewModel;

            if (window.ShowDialog() == true)
            {
                PreCondition2List.Add(viewModel.Result);
                SelectedPreCondition2 = viewModel.Result;
            }
        }

        /// <summary>
        /// 選択中の前提条件2を編集するプロパティウィンドウを開く
        /// </summary>
        private void OpenEditPreCondition2Window()
        {
            if (SelectedPreCondition2 == null) return;

            var window = new InterlockPrecondition2PropertiesWindow { Owner = _window };
            var viewModel = new InterlockPrecondition2PropertiesViewModel(window, SelectedPreCondition2, ProcessDetails);
            window.DataContext = viewModel;

            if (window.ShowDialog() == true)
            {
                // 編集結果を反映
                var index = PreCondition2List.IndexOf(SelectedPreCondition2);
                if (index >= 0)
                {
                    PreCondition2List[index] = viewModel.Result;
                    SelectedPreCondition2 = viewModel.Result;
                }
            }
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

        /// <summary>
        /// 前提条件3を新規追加するプロパティウィンドウを開く
        /// </summary>
        private void OpenAddPreCondition3Window()
        {
            var window = new InterlockPrecondition3PropertiesWindow { Owner = _window };
            var viewModel = new InterlockPrecondition3PropertiesViewModel(window, _supabaseRepository, _plcId, null);
            window.DataContext = viewModel;

            if (window.ShowDialog() == true)
            {
                PreCondition3List.Add(viewModel.Result);
                SelectedPreCondition3 = viewModel.Result;
            }
        }

        /// <summary>
        /// 選択中の前提条件3を編集するプロパティウィンドウを開く
        /// </summary>
        private void OpenEditPreCondition3Window()
        {
            if (SelectedPreCondition3 == null) return;

            var window = new InterlockPrecondition3PropertiesWindow { Owner = _window };
            var viewModel = new InterlockPrecondition3PropertiesViewModel(window, _supabaseRepository, _plcId, SelectedPreCondition3);
            window.DataContext = viewModel;

            if (window.ShowDialog() == true)
            {
                // 編集結果を反映
                var index = PreCondition3List.IndexOf(SelectedPreCondition3);
                if (index >= 0)
                {
                    PreCondition3List[index] = viewModel.Result;
                    SelectedPreCondition3 = viewModel.Result;
                }
            }
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
