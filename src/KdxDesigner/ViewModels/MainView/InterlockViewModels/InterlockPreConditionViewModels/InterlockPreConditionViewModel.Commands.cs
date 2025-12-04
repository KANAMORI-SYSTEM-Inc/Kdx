using Kdx.Contracts.DTOs;
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
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void AddPreCondition1(object? parameter)
        {
            var newCondition = new InterlockPrecondition1
            {
                ConditionName = "新規条件",
                Discription = ""
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
