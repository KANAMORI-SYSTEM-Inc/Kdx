using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using KdxDesigner.ViewModels.IOEditor;
using KdxDesigner.Views;
using System.Text;
using System.Windows;
using System.Windows.Input;

// Views名前空間エイリアス
using ViewsCommon = KdxDesigner.Views.Common;
using ViewsInterlock = KdxDesigner.Views.Interlock;

namespace KdxDesigner.ViewModels
{
    public partial class InterlockSettingsViewModel
    {
        public ICommand AddInterlockCommand { get; }
        public ICommand DeleteInterlockCommand { get; }
        public ICommand EditPreConditionsCommand { get; }
        public ICommand AddConditionCommand { get; }
        public ICommand DeleteConditionCommand { get; }
        public ICommand AddIOCommand { get; }
        public ICommand DeleteIOCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ClearCylinderSearchCommand { get; }
        public ICommand ReloadCommand { get; }

        private void InitializeCommands(
            out ICommand addInterlockCommand,
            out ICommand deleteInterlockCommand,
            out ICommand editPreConditionsCommand,
            out ICommand addConditionCommand,
            out ICommand deleteConditionCommand,
            out ICommand addIOCommand,
            out ICommand deleteIOCommand,
            out ICommand saveCommand,
            out ICommand cancelCommand,
            out ICommand clearCylinderSearchCommand,
            out ICommand reloadCommand)
        {
            addInterlockCommand = new RelayCommand(() => AddInterlock(null), () => CanAddInterlock(null));
            deleteInterlockCommand = new RelayCommand(() => DeleteInterlock(null), () => CanDeleteInterlock(null));
            editPreConditionsCommand = new RelayCommand(() => EditPreConditions(null), () => CanEditPreConditions(null));
            addConditionCommand = new RelayCommand(() => AddCondition(null), () => CanAddCondition(null));
            deleteConditionCommand = new RelayCommand(() => DeleteCondition(null), () => CanDeleteCondition(null));
            addIOCommand = new RelayCommand(() => AddIO(null), () => CanAddIO(null));
            deleteIOCommand = new RelayCommand(() => DeleteIO(null), () => CanDeleteIO(null));
            saveCommand = new RelayCommand(async () => await SaveAsync());
            cancelCommand = new RelayCommand(() => Cancel(null));
            clearCylinderSearchCommand = new RelayCommand(() => CylinderSearchText = string.Empty);
            reloadCommand = new RelayCommand(async () => await ReloadAsync());
        }

        private bool CanAddInterlock(object? parameter) => SelectedCylinder != null;

        private async void AddInterlock(object? parameter)
        {
            if (_selectedCylinder == null)
            {
                return;
            }

            var newInterlock = new Interlock
            {
                CylinderId = _selectedCylinder.Id,
                PlcId = _selectedCylinder.PlcId,
                SortId = Interlocks.Count + 1,
                ConditionCylinderId = _selectedCylinder.Id
            };

            try
            {
                // データベースに保存
                await _supabaseRepository.UpsertInterlocksAsync(new List<Interlock> { newInterlock });

                var interlockViewModel = new InterlockViewModel(newInterlock)
                {
                    ConditionCylinderNum = _selectedCylinder.CYNum
                };

                Interlocks.Add(interlockViewModel);
                SelectedInterlock = interlockViewModel;
            }
            catch (Exception ex)
            {
                ErrorDialog.Show($"インターロックの追加に失敗しました: {ex.Message}", "エラー", _window);
            }
        }

        private bool CanDeleteInterlock(object? parameter) => SelectedInterlock != null;

        private async void DeleteInterlock(object? parameter)
        {
            if (SelectedInterlock == null)
            {
                return;
            }

            // 非同期処理中にSelectedInterlockがnullにならないよう局所変数に保存
            var interlockToDelete = SelectedInterlock;

            var result = MessageBox.Show("選択したインターロックを削除しますか？\n関連する条件とIOも削除されます。", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var interlockKey = (interlockToDelete.CylinderId, interlockToDelete.SortId);

                    // 関連するIOと条件を先に削除（外部キー制約のため）
                    if (_allConditionsByInterlockKey.TryGetValue(interlockKey, out var relatedConditions))
                    {
                        // リストのコピーを作成（列挙中の変更を回避）
                        var conditionsToDelete = relatedConditions.ToList();
                        foreach (var condition in conditionsToDelete)
                        {
                            if (condition == null) continue;

                            // IOを削除
                            var conditionKey = (condition.CylinderId, condition.InterlockSortId, condition.ConditionNumber);
                            if (_allIOsByConditionKey.TryGetValue(conditionKey, out var relatedIOs))
                            {
                                var iosToDelete = relatedIOs.ToList();
                                foreach (var ioVm in iosToDelete)
                                {
                                    if (ioVm != null)
                                    {
                                        var io = ioVm.GetInterlockIO();
                                        if (io != null)
                                        {
                                            await _supabaseRepository.DeleteInterlockIOAsync(io);
                                        }
                                    }
                                }
                                _allIOsByConditionKey.Remove(conditionKey);
                            }

                            // 条件を削除
                            await _supabaseRepository.DeleteInterlockConditionAsync(condition);
                        }
                        _allConditionsByInterlockKey.Remove(interlockKey);
                    }

                    // インターロック本体を削除
                    var interlock = interlockToDelete.GetInterlock();
                    if (interlock != null)
                    {
                        await _supabaseRepository.DeleteInterlockAsync(interlock);
                    }

                    // UIから削除
                    Interlocks.Remove(interlockToDelete);
                    SelectedInterlock = null;
                }
                catch (Exception ex)
                {
                    ErrorDialog.Show($"インターロックの削除に失敗しました: {ex.Message}", "エラー", _window);
                }
            }
        }

        private bool CanAddCondition(object? parameter) => SelectedInterlock != null;

        private async void AddCondition(object? parameter)
        {
            if (SelectedInterlock == null)
            {
                return;
            }

            // Set default ConditionTypeId and populate ConditionType
            var defaultTypeId = ConditionTypes.FirstOrDefault()?.Id ?? 1;
            var newCondition = new InterlockConditionDTO
            {
                CylinderId = SelectedInterlock.CylinderId,
                InterlockSortId = SelectedInterlock.SortId,
                ConditionNumber = InterlockConditions.Count + 1,
                ConditionTypeId = defaultTypeId,
                ConditionType = ConditionTypes.FirstOrDefault(ct => ct.Id == defaultTypeId)
            };

            try
            {
                // データベースに保存
                await _supabaseRepository.UpsertInterlockConditionsAsync(new List<InterlockConditionDTO> { newCondition });

                InterlockConditions.Add(newCondition);

                // キャッシュにも追加
                var interlockKey = (SelectedInterlock.CylinderId, SelectedInterlock.SortId);
                if (!_allConditionsByInterlockKey.ContainsKey(interlockKey))
                {
                    _allConditionsByInterlockKey[interlockKey] = new List<InterlockConditionDTO>();
                }
                _allConditionsByInterlockKey[interlockKey].Add(newCondition);

                SelectedCondition = newCondition;
            }
            catch (Exception ex)
            {
                ErrorDialog.Show($"インターロック条件の追加に失敗しました: {ex.Message}", "エラー", _window);
            }
        }

        private bool CanDeleteCondition(object? parameter) => SelectedCondition != null;

        private async void DeleteCondition(object? parameter)
        {
            if (SelectedCondition == null)
            {
                return;
            }

            // 非同期処理中にSelectedConditionがnullにならないよう局所変数に保存
            var conditionToDelete = SelectedCondition;
            var currentInterlock = SelectedInterlock;

            var result = MessageBox.Show("選択した条件を削除しますか？\n関連するIOも削除されます。", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // 関連するIOを先に削除（外部キー制約のため）
                    var conditionKey = (conditionToDelete.CylinderId, conditionToDelete.InterlockSortId, conditionToDelete.ConditionNumber);
                    if (_allIOsByConditionKey.TryGetValue(conditionKey, out var relatedIOs))
                    {
                        // リストのコピーを作成してから削除（列挙中の変更を回避）
                        var iosToDelete = relatedIOs.ToList();
                        foreach (var ioVm in iosToDelete)
                        {
                            if (ioVm != null)
                            {
                                var io = ioVm.GetInterlockIO();
                                if (io != null)
                                {
                                    await _supabaseRepository.DeleteInterlockIOAsync(io);
                                }
                            }
                        }
                        _allIOsByConditionKey.Remove(conditionKey);
                    }

                    // DBから条件を削除
                    await _supabaseRepository.DeleteInterlockConditionAsync(conditionToDelete);

                    // UIから削除
                    InterlockConditions.Remove(conditionToDelete);

                    // キャッシュからも削除（複合キーで一致するアイテムを検索して削除）
                    if (currentInterlock != null)
                    {
                        var interlockKey = (currentInterlock.CylinderId, currentInterlock.SortId);
                        if (_allConditionsByInterlockKey.TryGetValue(interlockKey, out var conditions))
                        {
                            var toRemove = conditions.FirstOrDefault(c =>
                                c != null &&
                                c.CylinderId == conditionToDelete.CylinderId &&
                                c.ConditionNumber == conditionToDelete.ConditionNumber &&
                                c.InterlockSortId == conditionToDelete.InterlockSortId);

                            if (toRemove != null)
                            {
                                conditions.Remove(toRemove);
                            }
                        }
                    }

                    SelectedCondition = null;
                }
                catch (Exception ex)
                {
                    ErrorDialog.Show($"インターロック条件の削除に失敗しました: {ex.Message}", "エラー", _window);
                }
            }
        }

        private bool CanAddIO(object? parameter) => SelectedCondition != null;

        private async void AddIO(object? parameter)
        {
            if (SelectedCondition == null || _selectedCylinder == null)
            {
                return;
            }

            // Open IO search window with cylinder CYNum as initial search
            var ioSearchWindow = new ViewsCommon.IOSearchWindow();
            var ioSearchViewModel = new IOSearchViewModel(_accessRepository, _selectedCylinder.PlcId, _selectedCylinder.CYNum);
            ioSearchWindow.DataContext = ioSearchViewModel;
            ioSearchWindow.Owner = _window;

            if (ioSearchWindow.ShowDialog() == true && ioSearchViewModel.SelectedIO != null)
            {
                var selectedIO = ioSearchViewModel.SelectedIO;

                // Determine the IOAddress based on PlcId
                string ioAddress;
                int plcId;

                if (selectedIO.PlcId == _selectedCylinder.PlcId)
                {
                    // Same PLC - use the direct address
                    ioAddress = selectedIO.Address;
                    plcId = selectedIO.PlcId;
                }
                else
                {
                    // Different PLC - use LinkDevice if available
                    ioAddress = !string.IsNullOrEmpty(selectedIO.LinkDevice)
                        ? selectedIO.LinkDevice
                        : selectedIO.Address;
                    plcId = selectedIO.PlcId;
                }

                var newIO = new InterlockIO
                {
                    CylinderId = SelectedCondition.CylinderId,
                    InterlockSortId = SelectedCondition.InterlockSortId,
                    ConditionNumber = SelectedCondition.ConditionNumber,
                    PlcId = plcId,
                    IOAddress = ioAddress,
                    IsOnCondition = false
                };

                try
                {
                    // データベースに保存
                    await _supabaseRepository.AddInterlockIOAssociationAsync(newIO);

                    var ioViewModel = new InterlockIOViewModel(newIO, false) // 保存済みなのでIsNew=false
                    {
                        IOName = selectedIO.IOName  // 選択されたIOからIONameを設定
                    };

                    InterlockIOs.Add(ioViewModel);

                    // キャッシュにも追加
                    var conditionKey = (SelectedCondition.CylinderId, SelectedCondition.InterlockSortId, SelectedCondition.ConditionNumber);
                    if (!_allIOsByConditionKey.ContainsKey(conditionKey))
                    {
                        _allIOsByConditionKey[conditionKey] = new List<InterlockIOViewModel>();
                    }
                    _allIOsByConditionKey[conditionKey].Add(ioViewModel);

                    SelectedIO = ioViewModel;
                }
                catch (Exception ex)
                {
                    ErrorDialog.Show($"インターロックIOの追加に失敗しました: {ex.Message}", "エラー", _window);
                }
            }
        }

        private bool CanDeleteIO(object? parameter) => SelectedIO != null;

        private async void DeleteIO(object? parameter)
        {
            if (SelectedIO == null)
            {
                return;
            }

            // 非同期処理中にSelectedIOがnullにならないよう局所変数に保存
            var ioToDelete = SelectedIO;
            var currentCondition = SelectedCondition;

            var result = MessageBox.Show("選択したIOを削除しますか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // DBから削除（新規作成されたもの以外）
                    if (!ioToDelete.IsNew)
                    {
                        var io = ioToDelete.GetInterlockIO();
                        if (io != null)
                        {
                            await _supabaseRepository.DeleteInterlockIOAsync(io);
                        }
                    }

                    // UIから削除
                    InterlockIOs.Remove(ioToDelete);

                    // キャッシュからも削除（複合キーで一致するアイテムを検索して削除）
                    if (currentCondition != null)
                    {
                        var conditionKey = (currentCondition.CylinderId, currentCondition.InterlockSortId, currentCondition.ConditionNumber);
                        if (_allIOsByConditionKey.TryGetValue(conditionKey, out var ios))
                        {
                            var toRemove = ios.FirstOrDefault(io =>
                                io != null &&
                                io.CylinderId == ioToDelete.CylinderId &&
                                io.PlcId == ioToDelete.PlcId &&
                                io.IOAddress == ioToDelete.IOAddress &&
                                io.InterlockSortId == ioToDelete.InterlockSortId &&
                                io.ConditionNumber == ioToDelete.ConditionNumber);

                            if (toRemove != null)
                            {
                                ios.Remove(toRemove);
                            }
                        }
                    }

                    SelectedIO = null;
                }
                catch (Exception ex)
                {
                    ErrorDialog.Show($"インターロックIOの削除に失敗しました: {ex.Message}", "エラー", _window);
                }
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                // 削除は各削除メソッドで即座にDBに反映されるため、ここでは保存のみ行う

                // Save Interlocks
                var interlocksToSave = Interlocks.Select(vm => vm.GetInterlock()).ToList();

                // Upsert前のIdとViewModelをマッピング
                // Interlocksを保存（複合キー対応）
                await _supabaseRepository.UpsertInterlocksAsync(interlocksToSave);

                // 保存されたInterlocksのキーセットを作成
                var savedInterlockKeys = new HashSet<(int cylinderId, int sortId)>(
                    interlocksToSave.Select(i => (i.CylinderId, i.SortId))
                );

                // キャッシュから全てのInterlockConditionsを収集して保存
                // ただし、実際に保存されたInterlocksに対応するもののみ
                var allConditionsToSave = new List<InterlockConditionDTO>();
                foreach (var kvp in _allConditionsByInterlockKey)
                {
                    // InterlockConditionのキーがsavedInterlockKeysに存在するか確認
                    if (savedInterlockKeys.Contains(kvp.Key))
                    {
                        allConditionsToSave.AddRange(kvp.Value);
                    }
                }

                if (allConditionsToSave.Any())
                {
                    await _supabaseRepository.UpsertInterlockConditionsAsync(allConditionsToSave);
                }

                // キャッシュから全てのInterlockIOsを収集して保存 (新規作成されたもののみ)
                foreach (var kvp in _allIOsByConditionKey)
                {
                    var iosToSave = kvp.Value.Where(io => io.IsNew).ToList();

                    foreach (var ioVm in iosToSave)
                    {
                        if (!string.IsNullOrEmpty(ioVm.IOAddress)) // Only save if IOAddress is set
                        {
                            try
                            {
                                await _supabaseRepository.AddInterlockIOAssociationAsync(ioVm.GetInterlockIO());
                                ioVm.IsNew = false; // 保存成功後は既存データとして扱う
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Failed to save InterlockIO: {ex.Message}");
                            }
                        }
                    }
                }

                MessageBox.Show("保存しました。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine($"保存に失敗しました: {ex.Message}");
                errorMessage.AppendLine();
                errorMessage.AppendLine("スタックトレース:");
                errorMessage.AppendLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    errorMessage.AppendLine();
                    errorMessage.AppendLine("内部例外:");
                    errorMessage.AppendLine(ex.InnerException.Message);
                    errorMessage.AppendLine(ex.InnerException.StackTrace);
                }

                ErrorDialog.Show(errorMessage.ToString(), "エラー", _window);
            }
        }

        private bool CanEditPreConditions(object? parameter) => SelectedInterlock != null;

        private async void EditPreConditions(object? parameter)
        {
            if (SelectedInterlock == null)
            {
                return;
            }

            var preConditionWindow = new ViewsInterlock.InterlockPreConditionWindow();
            var preConditionViewModel = new InterlockPreConditionViewModel(
                _supabaseRepository,
                SelectedInterlock.GetInterlock(),
                _plcId,
                preConditionWindow);
            preConditionWindow.DataContext = preConditionViewModel;
            preConditionWindow.Owner = _window;

            if (preConditionWindow.ShowDialog() == true)
            {
                // ダイアログが保存で閉じられた場合、関連データを再読み込み
                await ReloadAsync();
            }
        }

        private async Task ReloadAsync()
        {
            // 現在の選択状態を保存
            var selectedCylinderId = _selectedCylinder?.Id;
            var selectedInterlockSortId = _selectedInterlock?.SortId;

            // キャッシュをクリア
            _allConditionsByInterlockKey.Clear();
            _allIOsByConditionKey.Clear();

            // 削除トラッキングをクリア
            _deletedIOs.Clear();
            _deletedConditions.Clear();
            _deletedInterlocks.Clear();

            // データを再読み込み
            await LoadCylindersAsync();
            await LoadConditionTypesAsync();
            await LoadPreConditionsAsync();

            // 選択状態を復元
            if (selectedCylinderId.HasValue)
            {
                SelectedCylinder = _allCylinders.FirstOrDefault(c => c.Id == selectedCylinderId.Value);

                // インターロックの選択も復元
                if (selectedInterlockSortId.HasValue && SelectedCylinder != null)
                {
                    await LoadInterlocksAsync();
                    SelectedInterlock = Interlocks.FirstOrDefault(i => i.SortId == selectedInterlockSortId.Value);
                }
            }
        }

        private void Cancel(object? parameter)
        {
            _window.DialogResult = false;
            _window.Close();
        }
    }
}
