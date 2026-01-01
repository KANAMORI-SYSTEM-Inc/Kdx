using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Contracts.Enums;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Collections.ObjectModel;
using System.Windows;

// 名前空間とクラス名の衝突を回避するためのエイリアス
using ErrorMessageDto = Kdx.Contracts.DTOs.ErrorMessage;

namespace KdxDesigner.ViewModels.ErrorMessage
{
    /// <summary>
    /// エラーメッセージベーステンプレート編集画面のViewModel
    /// Interlock/Operation用のエラーメッセージテンプレートを編集・保存する
    /// </summary>
    public partial class ErrorMessageEditorViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository;

        /// <summary>
        /// 選択可能なMnemonicType
        /// </summary>
        public ObservableCollection<MnemonicTypeItem> MnemonicTypes { get; } = new()
        {
            new MnemonicTypeItem { Id = (int)MnemonicType.Operation, Name = "Operation (動作)", Type = MnemonicType.Operation },
            new MnemonicTypeItem { Id = (int)MnemonicType.Interlock, Name = "Interlock (インターロック)", Type = MnemonicType.Interlock }
        };

        /// <summary>
        /// 選択可能なConditionType（Interlock専用）
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ConditionTypeItem> _conditionTypes = new();

        /// <summary>
        /// 選択されたConditionType
        /// </summary>
        [ObservableProperty]
        private ConditionTypeItem? _selectedConditionType;

        /// <summary>
        /// ConditionType選択が有効かどうか（Interlock選択時のみ）
        /// </summary>
        [ObservableProperty]
        private bool _isConditionTypeEnabled;

        /// <summary>
        /// 利用可能なプレースホルダー（選択されたMnemonicTypeに応じて変化）
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<PlaceholderItem> _availablePlaceholders = new();

        /// <summary>
        /// IO情報用プレースホルダー（Interlock専用）
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<PlaceholderItem> _ioPlaceholders = new();

        /// <summary>
        /// IO情報用プレースホルダーが存在するか（Interlock選択時のみtrue）
        /// </summary>
        [ObservableProperty]
        private bool _hasIoPlaceholders;

        /// <summary>
        /// 選択されたMnemonicType
        /// </summary>
        [ObservableProperty]
        private MnemonicTypeItem? _selectedMnemonicType;

        /// <summary>
        /// 選択されたプレースホルダー
        /// </summary>
        [ObservableProperty]
        private PlaceholderItem? _selectedPlaceholder;

        /// <summary>
        /// 選択されたIO用プレースホルダー
        /// </summary>
        [ObservableProperty]
        private PlaceholderItem? _selectedIoPlaceholder;

        /// <summary>
        /// 既存のエラーメッセージリスト
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ErrorMessageDto> _errorMessages = new();

        /// <summary>
        /// 選択された既存のエラーメッセージ
        /// </summary>
        [ObservableProperty]
        private ErrorMessageDto? _selectedErrorMessage;

        /// <summary>
        /// AlarmId
        /// </summary>
        [ObservableProperty]
        private int _alarmId = 1;

        /// <summary>
        /// ベースメッセージ
        /// </summary>
        [ObservableProperty]
        private string _baseMessage = "";

        /// <summary>
        /// ベースアラーム
        /// </summary>
        [ObservableProperty]
        private string _baseAlarm = "";

        /// <summary>
        /// カテゴリ1
        /// </summary>
        [ObservableProperty]
        private string _category1 = "";

        /// <summary>
        /// カテゴリ2
        /// </summary>
        [ObservableProperty]
        private string _category2 = "";

        /// <summary>
        /// カテゴリ3
        /// </summary>
        [ObservableProperty]
        private string _category3 = "";

        /// <summary>
        /// デフォルトカウント時間(ms)
        /// </summary>
        [ObservableProperty]
        private int _defaultCountTime = 1000;

        /// <summary>
        /// 現在編集中のフィールド（プレースホルダー挿入先）
        /// </summary>
        [ObservableProperty]
        private string _currentEditField = "BaseMessage";

        /// <summary>
        /// 読み込み中フラグ
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary>
        /// プレビューテキスト
        /// </summary>
        [ObservableProperty]
        private string _previewText = "";

        public ErrorMessageEditorViewModel(ISupabaseRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            // デフォルトでInterlockを選択
            SelectedMnemonicType = MnemonicTypes.FirstOrDefault(m => m.Type == MnemonicType.Interlock);

            // ConditionTypeを読み込み
            _ = LoadConditionTypesAsync();
        }

        /// <summary>
        /// ConditionTypeを読み込み
        /// </summary>
        private async Task LoadConditionTypesAsync()
        {
            try
            {
                var types = await _repository.GetInterlockConditionTypesAsync();
                ConditionTypes.Clear();

                // 「共通（全てのConditionTypeで使用）」を追加（ConditionTypeId=0は共通テンプレート）
                ConditionTypes.Add(new ConditionTypeItem { Id = 0, Name = "共通（全てのConditionTypeで使用）" });

                foreach (var type in types.OrderBy(t => t.Id))
                {
                    ConditionTypes.Add(new ConditionTypeItem
                    {
                        Id = type.Id,
                        Name = $"{type.Id}: {type.ConditionTypeName}"
                    });
                }

                // デフォルトで「共通」を選択
                SelectedConditionType = ConditionTypes.FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadConditionTypesAsync error: {ex.Message}");
            }
        }

        /// <summary>
        /// MnemonicType変更時の処理
        /// </summary>
        partial void OnSelectedMnemonicTypeChanged(MnemonicTypeItem? value)
        {
            // ConditionType選択の有効/無効を切り替え
            IsConditionTypeEnabled = value?.Type == MnemonicType.Interlock;

            // Interlock以外の場合は「共通」を選択
            if (!IsConditionTypeEnabled)
            {
                SelectedConditionType = ConditionTypes.FirstOrDefault(c => c.Id == 0);
            }

            UpdateAvailablePlaceholders();
            _ = LoadErrorMessagesAsync();
        }

        /// <summary>
        /// ConditionType変更時の処理
        /// </summary>
        partial void OnSelectedConditionTypeChanged(ConditionTypeItem? value)
        {
            // ConditionTypeが変わったらエラーメッセージを再読み込み
            _ = LoadErrorMessagesAsync();
        }

        /// <summary>
        /// 既存メッセージ選択時の処理
        /// </summary>
        partial void OnSelectedErrorMessageChanged(ErrorMessageDto? value)
        {
            if (value != null)
            {
                AlarmId = value.AlarmId;
                BaseMessage = value.BaseMessage ?? "";
                BaseAlarm = value.BaseAlarm ?? "";
                Category1 = value.Category1 ?? "";
                Category2 = value.Category2 ?? "";
                Category3 = value.Category3 ?? "";
                DefaultCountTime = value.DefaultCountTime;

                // ConditionTypeIdに対応するConditionTypeを選択
                SelectedConditionType = ConditionTypes.FirstOrDefault(c => c.Id == value.ConditionTypeId)
                    ?? ConditionTypes.FirstOrDefault(c => c.Id == 0);

                UpdatePreview();
            }
        }

        /// <summary>
        /// BaseMessage変更時のプレビュー更新
        /// </summary>
        partial void OnBaseMessageChanged(string value)
        {
            UpdatePreview();
        }

        /// <summary>
        /// 利用可能なプレースホルダーを更新
        /// </summary>
        private void UpdateAvailablePlaceholders()
        {
            AvailablePlaceholders.Clear();
            IoPlaceholders.Clear();
            HasIoPlaceholders = false;

            if (SelectedMnemonicType?.Type == MnemonicType.Interlock)
            {
                // Interlock用プレースホルダー
                AvailablePlaceholders.Add(new PlaceholderItem("{CylinderName}", "シリンダー名 (CYNum + CYNameSub)", "CY01A"));
                AvailablePlaceholders.Add(new PlaceholderItem("{GoBack}", "動作方向", "Go&Back"));
                AvailablePlaceholders.Add(new PlaceholderItem("{ConditionCylinderName}", "条件シリンダー名", "CY02B"));
                AvailablePlaceholders.Add(new PlaceholderItem("{ConditionType}", "条件タイプ名", "インターロック"));
                AvailablePlaceholders.Add(new PlaceholderItem("{ConditionName}", "条件名", "前進後確認"));
                AvailablePlaceholders.Add(new PlaceholderItem("{ConditionDevice}", "条件デバイス", "M100"));
                AvailablePlaceholders.Add(new PlaceholderItem("{Comment1}", "条件コメント1", "前進確認"));
                AvailablePlaceholders.Add(new PlaceholderItem("{Comment2}", "条件コメント2", "後退確認"));
                AvailablePlaceholders.Add(new PlaceholderItem("{ConditionNumber}", "条件番号", "1"));
                AvailablePlaceholders.Add(new PlaceholderItem("{Precondition1}", "前提条件1の情報", "サイクル待ち"));
                AvailablePlaceholders.Add(new PlaceholderItem("{Precondition2}", "前提条件2の情報", "Mode:常時 開始工程:1 終了工程:5"));
                AvailablePlaceholders.Add(new PlaceholderItem("{IOConditions}", "IO条件リスト（簡易）", "LS1:ON, LS2:OFF"));
                AvailablePlaceholders.Add(new PlaceholderItem("{DetailedIOConditions}", "IO条件リスト（詳細）", "LS1(前進確認):ON"));
                AvailablePlaceholders.Add(new PlaceholderItem("{Device}", "割り当てMデバイス", "M50000"));
                AvailablePlaceholders.Add(new PlaceholderItem("{DeviceNumber}", "デバイス番号", "50000"));
                AvailablePlaceholders.Add(new PlaceholderItem("{InterlockNumber}", "インターロック番号", "1"));

                // IO情報用プレースホルダー（後方互換性: 最初のIOを使用）
                IoPlaceholders.Add(new PlaceholderItem("{IO.Address}", "IOアドレス(最初のIO)", "X100"));
                IoPlaceholders.Add(new PlaceholderItem("{IO.IOName}", "IO名(最初のIO)", "LS1"));
                IoPlaceholders.Add(new PlaceholderItem("{IO.IOExplanation}", "IO説明(最初のIO)", "前進確認センサー"));
                IoPlaceholders.Add(new PlaceholderItem("{IO.XComment}", "Xコメント(最初のIO)", "シリンダー前進"));
                IoPlaceholders.Add(new PlaceholderItem("{IO.YComment}", "Yコメント(最初のIO)", "出力コメント"));
                IoPlaceholders.Add(new PlaceholderItem("{IO.FComment}", "Fコメント(最初のIO)", "Fコメント"));
                IoPlaceholders.Add(new PlaceholderItem("{IO.IOSpot}", "ユニット設置場所(最初のIO)", "A号機"));
                IoPlaceholders.Add(new PlaceholderItem("{IO.UnitName}", "ユニット名称(最初のIO)", "搬送ユニット"));
                IoPlaceholders.Add(new PlaceholderItem("{IO.System}", "系統(最初のIO)", "系統1"));
                IoPlaceholders.Add(new PlaceholderItem("{IO.StationNumber}", "局番(最初のIO)", "1"));
                IoPlaceholders.Add(new PlaceholderItem("{IO.LinkDevice}", "リンクデバイス(最初のIO)", "B100"));
                IoPlaceholders.Add(new PlaceholderItem("{IO.IsOnCondition}", "ON/OFF条件(最初のIO)", "ON"));

                // インデックス付きIO情報用プレースホルダー（IO[0], IO[1], IO[2]...）
                for (int i = 0; i < 5; i++) // 最大5つのIOをサポート
                {
                    IoPlaceholders.Add(new PlaceholderItem($"{{IO[{i}].Address}}", $"IO[{i}]のアドレス", i == 0 ? "X100" : $"X{100 + i}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{IO[{i}].IOName}}", $"IO[{i}]のIO名", i == 0 ? "LS1" : $"LS{i + 1}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{IO[{i}].IOExplanation}}", $"IO[{i}]の説明", i == 0 ? "前進確認センサー" : $"センサー{i + 1}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{IO[{i}].XComment}}", $"IO[{i}]のXコメント", $"Xコメント{i}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{IO[{i}].YComment}}", $"IO[{i}]のYコメント", $"Yコメント{i}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{IO[{i}].FComment}}", $"IO[{i}]のFコメント", $"Fコメント{i}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{IO[{i}].IOSpot}}", $"IO[{i}]の設置場所", "A号機"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{IO[{i}].UnitName}}", $"IO[{i}]のユニット名称", "搬送ユニット"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{IO[{i}].System}}", $"IO[{i}]の系統", "系統1"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{IO[{i}].StationNumber}}", $"IO[{i}]の局番", "1"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{IO[{i}].LinkDevice}}", $"IO[{i}]のリンクデバイス", $"B{100 + i}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{IO[{i}].IsOnCondition}}", $"IO[{i}]のON/OFF条件", i % 2 == 0 ? "ON" : "OFF"));
                }

                HasIoPlaceholders = true;
            }
            else if (SelectedMnemonicType?.Type == MnemonicType.Operation)
            {
                // Operation用プレースホルダー
                AvailablePlaceholders.Add(new PlaceholderItem("{OperationName}", "オペレーション名", "搬送動作"));
                AvailablePlaceholders.Add(new PlaceholderItem("{Valve1}", "バルブ1", "SOL1"));
                AvailablePlaceholders.Add(new PlaceholderItem("{Valve2}", "バルブ2", "SOL2"));
                AvailablePlaceholders.Add(new PlaceholderItem("{GoBack}", "動作方向", "Go"));
                AvailablePlaceholders.Add(new PlaceholderItem("{CategoryName}", "カテゴリ名", "速度制御INV1"));
                AvailablePlaceholders.Add(new PlaceholderItem("{StartCondition}", "Start条件の表示文字列", "LS1:ON"));
                AvailablePlaceholders.Add(new PlaceholderItem("{FinishCondition}", "Finish条件の表示文字列", "LS2:ON"));
                AvailablePlaceholders.Add(new PlaceholderItem("{SpeedCondition}", "Speed条件の表示文字列", "SS1:ON"));

                // Start IO情報用プレースホルダー（後方互換性: 最初のStartIOを使用）
                IoPlaceholders.Add(new PlaceholderItem("{StartIO.Address}", "StartIOアドレス(最初)", "X100"));
                IoPlaceholders.Add(new PlaceholderItem("{StartIO.IOName}", "StartIO名(最初)", "LS1"));
                IoPlaceholders.Add(new PlaceholderItem("{StartIO.IOExplanation}", "StartIO説明(最初)", "前進開始センサー"));
                IoPlaceholders.Add(new PlaceholderItem("{StartIO.DisplayCondition}", "StartIO条件(最初)", "LS1:ON"));

                // Finish IO情報用プレースホルダー（後方互換性: 最初のFinishIOを使用）
                IoPlaceholders.Add(new PlaceholderItem("{FinishIO.Address}", "FinishIOアドレス(最初)", "X101"));
                IoPlaceholders.Add(new PlaceholderItem("{FinishIO.IOName}", "FinishIO名(最初)", "LS2"));
                IoPlaceholders.Add(new PlaceholderItem("{FinishIO.IOExplanation}", "FinishIO説明(最初)", "前進完了センサー"));
                IoPlaceholders.Add(new PlaceholderItem("{FinishIO.DisplayCondition}", "FinishIO条件(最初)", "LS2:ON"));

                // Speed IO情報用プレースホルダー（後方互換性: 最初のSpeedIOを使用）
                IoPlaceholders.Add(new PlaceholderItem("{SpeedIO.Address}", "SpeedIOアドレス(最初)", "X102"));
                IoPlaceholders.Add(new PlaceholderItem("{SpeedIO.IOName}", "SpeedIO名(最初)", "SS1"));
                IoPlaceholders.Add(new PlaceholderItem("{SpeedIO.IOExplanation}", "SpeedIO説明(最初)", "速度センサー1"));
                IoPlaceholders.Add(new PlaceholderItem("{SpeedIO.DisplayCondition}", "SpeedIO条件(最初)", "SS1:ON"));

                // Con IO情報用プレースホルダー
                IoPlaceholders.Add(new PlaceholderItem("{ConIO.Address}", "ConIOアドレス", "X103"));
                IoPlaceholders.Add(new PlaceholderItem("{ConIO.IOName}", "ConIO名", "CON1"));
                IoPlaceholders.Add(new PlaceholderItem("{ConIO.IOExplanation}", "ConIO説明", "制御センサー"));

                // インデックス付きStartIO情報用プレースホルダー
                for (int i = 0; i < 5; i++)
                {
                    IoPlaceholders.Add(new PlaceholderItem($"{{StartIO[{i}].Address}}", $"StartIO[{i}]のアドレス", $"X{100 + i}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{StartIO[{i}].IOName}}", $"StartIO[{i}]のIO名", $"LS{i + 1}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{StartIO[{i}].IOExplanation}}", $"StartIO[{i}]の説明", $"開始センサー{i + 1}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{StartIO[{i}].DisplayCondition}}", $"StartIO[{i}]の条件", $"LS{i + 1}:ON"));
                }

                // インデックス付きFinishIO情報用プレースホルダー
                for (int i = 0; i < 5; i++)
                {
                    IoPlaceholders.Add(new PlaceholderItem($"{{FinishIO[{i}].Address}}", $"FinishIO[{i}]のアドレス", $"X{110 + i}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{FinishIO[{i}].IOName}}", $"FinishIO[{i}]のIO名", $"LF{i + 1}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{FinishIO[{i}].IOExplanation}}", $"FinishIO[{i}]の説明", $"完了センサー{i + 1}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{FinishIO[{i}].DisplayCondition}}", $"FinishIO[{i}]の条件", $"LF{i + 1}:ON"));
                }

                // インデックス付きSpeedIO情報用プレースホルダー
                for (int i = 0; i < 5; i++)
                {
                    IoPlaceholders.Add(new PlaceholderItem($"{{SpeedIO[{i}].Address}}", $"SpeedIO[{i}]のアドレス", $"X{120 + i}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{SpeedIO[{i}].IOName}}", $"SpeedIO[{i}]のIO名", $"SS{i + 1}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{SpeedIO[{i}].IOExplanation}}", $"SpeedIO[{i}]の説明", $"速度センサー{i + 1}"));
                    IoPlaceholders.Add(new PlaceholderItem($"{{SpeedIO[{i}].DisplayCondition}}", $"SpeedIO[{i}]の条件", $"SS{i + 1}:ON"));
                }

                HasIoPlaceholders = true;
            }
        }

        /// <summary>
        /// 既存のエラーメッセージを読み込み
        /// </summary>
        private async Task LoadErrorMessagesAsync()
        {
            if (SelectedMnemonicType == null) return;

            try
            {
                IsLoading = true;

                // ConditionTypeIdを取得（Interlock以外またはSelectedConditionTypeがnullの場合は0=共通）
                int? conditionTypeId = 0;
                if (SelectedMnemonicType.Type == MnemonicType.Interlock && SelectedConditionType != null)
                {
                    conditionTypeId = SelectedConditionType.Id;
                }

                var messages = await _repository.GetErrorMessagesAsync(SelectedMnemonicType.Id, conditionTypeId);
                ErrorMessages = new ObservableCollection<ErrorMessageDto>(messages);

                // 次のAlarmIdを設定
                if (messages.Any())
                {
                    AlarmId = messages.Max(m => m.AlarmId) + 1;
                }
                else
                {
                    AlarmId = 1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadErrorMessagesAsync error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// プレースホルダーを挿入
        /// </summary>
        [RelayCommand]
        private void InsertPlaceholder()
        {
            if (SelectedPlaceholder == null) return;

            var placeholder = SelectedPlaceholder.Placeholder;

            switch (CurrentEditField)
            {
                case "BaseMessage":
                    BaseMessage += placeholder;
                    break;
                case "BaseAlarm":
                    BaseAlarm += placeholder;
                    break;
                case "Category1":
                    Category1 += placeholder;
                    break;
                case "Category2":
                    Category2 += placeholder;
                    break;
                case "Category3":
                    Category3 += placeholder;
                    break;
            }
        }

        /// <summary>
        /// IO用プレースホルダーを挿入
        /// </summary>
        [RelayCommand]
        private void InsertIoPlaceholder()
        {
            if (SelectedIoPlaceholder == null) return;

            var placeholder = SelectedIoPlaceholder.Placeholder;

            switch (CurrentEditField)
            {
                case "BaseMessage":
                    BaseMessage += placeholder;
                    break;
                case "BaseAlarm":
                    BaseAlarm += placeholder;
                    break;
                case "Category1":
                    Category1 += placeholder;
                    break;
                case "Category2":
                    Category2 += placeholder;
                    break;
                case "Category3":
                    Category3 += placeholder;
                    break;
            }
        }

        /// <summary>
        /// プレビューを更新
        /// </summary>
        private void UpdatePreview()
        {
            if (string.IsNullOrEmpty(BaseMessage))
            {
                PreviewText = "";
                return;
            }

            // サンプルデータでプレビュー生成
            var preview = BaseMessage;

            if (SelectedMnemonicType?.Type == MnemonicType.Interlock)
            {
                preview = preview.Replace("{CylinderName}", "CY01A");
                preview = preview.Replace("{GoBack}", "Go&Back");
                preview = preview.Replace("{ConditionCylinderName}", "CY02B");
                preview = preview.Replace("{ConditionType}", "インターロック");
                preview = preview.Replace("{ConditionName}", "前進後確認");
                preview = preview.Replace("{ConditionDevice}", "M100");
                preview = preview.Replace("{Comment1}", "前進確認");
                preview = preview.Replace("{Comment2}", "後退確認");
                preview = preview.Replace("{ConditionNumber}", "1");
                preview = preview.Replace("{Precondition1}", "サイクル待ち");
                preview = preview.Replace("{Precondition2}", "Mode:常時 開始:1 終了:5");
                preview = preview.Replace("{IOConditions}", "LS1:ON, LS2:OFF");
                preview = preview.Replace("{DetailedIOConditions}", "LS1(前進確認):ON, LS2(後退確認):OFF");
                preview = preview.Replace("{Device}", "M50000");
                preview = preview.Replace("{DeviceNumber}", "50000");
                preview = preview.Replace("{InterlockNumber}", "1");

                // IO情報用プレースホルダーのサンプル置換（後方互換性: 最初のIO）
                preview = preview.Replace("{IO.Address}", "X100");
                preview = preview.Replace("{IO.IOName}", "LS1");
                preview = preview.Replace("{IO.IOExplanation}", "前進確認センサー");
                preview = preview.Replace("{IO.XComment}", "シリンダー前進");
                preview = preview.Replace("{IO.YComment}", "出力コメント");
                preview = preview.Replace("{IO.FComment}", "Fコメント");
                preview = preview.Replace("{IO.IOSpot}", "A号機");
                preview = preview.Replace("{IO.UnitName}", "搬送ユニット");
                preview = preview.Replace("{IO.System}", "系統1");
                preview = preview.Replace("{IO.StationNumber}", "1");
                preview = preview.Replace("{IO.LinkDevice}", "B100");
                preview = preview.Replace("{IO.IsOnCondition}", "ON");

                // インデックス付きIO情報用プレースホルダーのサンプル置換
                for (int i = 0; i < 5; i++)
                {
                    preview = preview.Replace($"{{IO[{i}].Address}}", i == 0 ? "X100" : $"X{100 + i}");
                    preview = preview.Replace($"{{IO[{i}].IOName}}", i == 0 ? "LS1" : $"LS{i + 1}");
                    preview = preview.Replace($"{{IO[{i}].IOExplanation}}", i == 0 ? "前進確認センサー" : $"センサー{i + 1}");
                    preview = preview.Replace($"{{IO[{i}].XComment}}", $"Xコメント{i}");
                    preview = preview.Replace($"{{IO[{i}].YComment}}", $"Yコメント{i}");
                    preview = preview.Replace($"{{IO[{i}].FComment}}", $"Fコメント{i}");
                    preview = preview.Replace($"{{IO[{i}].IOSpot}}", "A号機");
                    preview = preview.Replace($"{{IO[{i}].UnitName}}", "搬送ユニット");
                    preview = preview.Replace($"{{IO[{i}].System}}", "系統1");
                    preview = preview.Replace($"{{IO[{i}].StationNumber}}", "1");
                    preview = preview.Replace($"{{IO[{i}].LinkDevice}}", $"B{100 + i}");
                    preview = preview.Replace($"{{IO[{i}].IsOnCondition}}", i % 2 == 0 ? "ON" : "OFF");
                }
            }
            else if (SelectedMnemonicType?.Type == MnemonicType.Operation)
            {
                // 基本プレースホルダー
                preview = preview.Replace("{OperationName}", "搬送動作");
                preview = preview.Replace("{Valve1}", "SOL1");
                preview = preview.Replace("{Valve2}", "SOL2");
                preview = preview.Replace("{GoBack}", "Go");
                preview = preview.Replace("{CategoryName}", "速度制御INV1");
                preview = preview.Replace("{StartCondition}", "LS1:ON");
                preview = preview.Replace("{FinishCondition}", "LS2:ON");
                preview = preview.Replace("{SpeedCondition}", "SS1:ON");

                // StartIO情報用プレースホルダー（後方互換性: 最初のStartIO）
                preview = preview.Replace("{StartIO.Address}", "X100");
                preview = preview.Replace("{StartIO.IOName}", "LS1");
                preview = preview.Replace("{StartIO.IOExplanation}", "前進開始センサー");
                preview = preview.Replace("{StartIO.DisplayCondition}", "LS1:ON");

                // FinishIO情報用プレースホルダー（後方互換性: 最初のFinishIO）
                preview = preview.Replace("{FinishIO.Address}", "X101");
                preview = preview.Replace("{FinishIO.IOName}", "LS2");
                preview = preview.Replace("{FinishIO.IOExplanation}", "前進完了センサー");
                preview = preview.Replace("{FinishIO.DisplayCondition}", "LS2:ON");

                // SpeedIO情報用プレースホルダー（後方互換性: 最初のSpeedIO）
                preview = preview.Replace("{SpeedIO.Address}", "X102");
                preview = preview.Replace("{SpeedIO.IOName}", "SS1");
                preview = preview.Replace("{SpeedIO.IOExplanation}", "速度センサー1");
                preview = preview.Replace("{SpeedIO.DisplayCondition}", "SS1:ON");

                // ConIO情報用プレースホルダー
                preview = preview.Replace("{ConIO.Address}", "X103");
                preview = preview.Replace("{ConIO.IOName}", "CON1");
                preview = preview.Replace("{ConIO.IOExplanation}", "制御センサー");

                // インデックス付きStartIO情報用プレースホルダー
                for (int i = 0; i < 5; i++)
                {
                    preview = preview.Replace($"{{StartIO[{i}].Address}}", $"X{100 + i}");
                    preview = preview.Replace($"{{StartIO[{i}].IOName}}", $"LS{i + 1}");
                    preview = preview.Replace($"{{StartIO[{i}].IOExplanation}}", $"開始センサー{i + 1}");
                    preview = preview.Replace($"{{StartIO[{i}].DisplayCondition}}", $"LS{i + 1}:ON");
                }

                // インデックス付きFinishIO情報用プレースホルダー
                for (int i = 0; i < 5; i++)
                {
                    preview = preview.Replace($"{{FinishIO[{i}].Address}}", $"X{110 + i}");
                    preview = preview.Replace($"{{FinishIO[{i}].IOName}}", $"LF{i + 1}");
                    preview = preview.Replace($"{{FinishIO[{i}].IOExplanation}}", $"完了センサー{i + 1}");
                    preview = preview.Replace($"{{FinishIO[{i}].DisplayCondition}}", $"LF{i + 1}:ON");
                }

                // インデックス付きSpeedIO情報用プレースホルダー
                for (int i = 0; i < 5; i++)
                {
                    preview = preview.Replace($"{{SpeedIO[{i}].Address}}", $"X{120 + i}");
                    preview = preview.Replace($"{{SpeedIO[{i}].IOName}}", $"SS{i + 1}");
                    preview = preview.Replace($"{{SpeedIO[{i}].IOExplanation}}", $"速度センサー{i + 1}");
                    preview = preview.Replace($"{{SpeedIO[{i}].DisplayCondition}}", $"SS{i + 1}:ON");
                }
            }

            PreviewText = preview;
        }

        /// <summary>
        /// 新規作成
        /// </summary>
        [RelayCommand]
        private void CreateNew()
        {
            SelectedErrorMessage = null;
            if (ErrorMessages.Any())
            {
                AlarmId = ErrorMessages.Max(m => m.AlarmId) + 1;
            }
            else
            {
                AlarmId = 1;
            }
            BaseMessage = "";
            BaseAlarm = "";
            Category1 = "";
            Category2 = "";
            Category3 = "";
            DefaultCountTime = 1000;
            PreviewText = "";
        }

        /// <summary>
        /// 保存
        /// </summary>
        [RelayCommand]
        private async Task SaveAsync()
        {
            if (SelectedMnemonicType == null)
            {
                MessageBox.Show("MnemonicTypeを選択してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(BaseMessage) && string.IsNullOrWhiteSpace(BaseAlarm))
            {
                MessageBox.Show("BaseMessageまたはBaseAlarmを入力してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;

                // ConditionTypeIdを取得（Interlock以外またはSelectedConditionTypeがnullの場合は0=共通）
                int conditionTypeId = 0;
                if (SelectedMnemonicType.Type == MnemonicType.Interlock && SelectedConditionType != null)
                {
                    conditionTypeId = SelectedConditionType.Id;
                }

                var errorMessage = new ErrorMessageDto
                {
                    MnemonicId = SelectedMnemonicType.Id,
                    ConditionTypeId = conditionTypeId,
                    AlarmId = AlarmId,
                    BaseMessage = BaseMessage,
                    BaseAlarm = BaseAlarm,
                    Category1 = Category1,
                    Category2 = Category2,
                    Category3 = Category3,
                    DefaultCountTime = DefaultCountTime
                };

                await _repository.UpsertErrorMessageAsync(errorMessage);

                var conditionTypeText = conditionTypeId != 0 ? $", ConditionTypeId: {conditionTypeId}" : " (共通)";
                MessageBox.Show($"エラーメッセージテンプレートを保存しました。\nMnemonicId: {SelectedMnemonicType.Id}{conditionTypeText}, AlarmId: {AlarmId}",
                    "完了", MessageBoxButton.OK, MessageBoxImage.Information);

                // リストを再読み込み
                await LoadErrorMessagesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveAsync error: {ex.Message}");
                MessageBox.Show($"保存に失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// 削除
        /// </summary>
        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (SelectedMnemonicType == null || SelectedErrorMessage == null)
            {
                MessageBox.Show("削除するエラーメッセージを選択してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var conditionTypeText = SelectedErrorMessage.ConditionTypeId != 0
                ? $"\nConditionTypeId: {SelectedErrorMessage.ConditionTypeId}"
                : "\nConditionTypeId: (共通)";

            var result = MessageBox.Show(
                $"以下のエラーメッセージテンプレートを削除しますか？\n\nMnemonicId: {SelectedMnemonicType.Id}{conditionTypeText}\nAlarmId: {SelectedErrorMessage.AlarmId}",
                "確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;

                await _repository.DeleteErrorMessageAsync(
                    SelectedMnemonicType.Id,
                    SelectedErrorMessage.ConditionTypeId,
                    SelectedErrorMessage.AlarmId);

                MessageBox.Show("エラーメッセージテンプレートを削除しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);

                // リストを再読み込み
                CreateNew();
                await LoadErrorMessagesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteAsync error: {ex.Message}");
                MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        [RelayCommand]
        private void Close(Window? window)
        {
            window?.Close();
        }
    }

    /// <summary>
    /// MnemonicType選択用アイテム
    /// </summary>
    public class MnemonicTypeItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public MnemonicType Type { get; set; }
    }

    /// <summary>
    /// ConditionType選択用アイテム（Interlock専用）
    /// </summary>
    public class ConditionTypeItem
    {
        /// <summary>
        /// ConditionTypeId (0の場合は共通テンプレート)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 表示名
        /// </summary>
        public string Name { get; set; } = "";
    }

    /// <summary>
    /// プレースホルダー選択用アイテム
    /// </summary>
    public class PlaceholderItem
    {
        public string Placeholder { get; set; }
        public string Description { get; set; }
        public string Example { get; set; }
        public string DisplayText => $"{Placeholder} - {Description} (例: {Example})";

        public PlaceholderItem(string placeholder, string description, string example)
        {
            Placeholder = placeholder;
            Description = description;
            Example = example;
        }
    }
}
