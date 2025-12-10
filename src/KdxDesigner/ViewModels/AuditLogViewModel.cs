using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;

namespace KdxDesigner.ViewModels
{
    public partial class AuditLogViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository;
        private const int PageSize = 50;

        [ObservableProperty]
        private ObservableCollection<AuditLog> _auditLogs = new();

        [ObservableProperty]
        private AuditLog? _selectedLog;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _totalPages = 1;

        [ObservableProperty]
        private string _selectedTableFilter = "すべて";

        [ObservableProperty]
        private string _formattedOldData = string.Empty;

        [ObservableProperty]
        private string _formattedNewData = string.Empty;

        public ObservableCollection<string> TableFilters { get; } = new()
        {
            "すべて",
            "会社 (Company)",
            "機種 (Model)",
            "PLC (PLC)",
            "サイクル (Cycle)",
            "シリンダー (Cylinder)",
            "操作 (Operation)",
            "工程 (Process)",
            "工程詳細 (ProcessDetail)",
            "IO (IO)",
            "タイマー (Timer)",
            "機械 (Machine)",
            "機械名称 (MachineName)",
            "駆動部(主) (DriveMain)",
            "駆動部(副) (DriveSub)",
            "インターロック (Interlock)",
            "インターロック条件 (InterlockCondition)",
            "メモリ (Memory)",
            "メモリプロファイル (MemoryProfile)"
        };

        public AuditLogViewModel(ISupabaseRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        partial void OnSelectedLogChanged(AuditLog? value)
        {
            if (value != null)
            {
                FormattedOldData = FormatJson(value.OldData);
                FormattedNewData = FormatJson(value.NewData);
            }
            else
            {
                FormattedOldData = string.Empty;
                FormattedNewData = string.Empty;
            }
        }

        private string FormatJson(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return "(なし)";

            try
            {
                var doc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return json;
            }
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                TotalCount = await _repository.GetAuditLogCountAsync();
                TotalPages = Math.Max(1, (int)Math.Ceiling((double)TotalCount / PageSize));

                var offset = (CurrentPage - 1) * PageSize;
                List<AuditLog> logs;

                if (SelectedTableFilter == "すべて")
                {
                    logs = await _repository.GetAuditLogsAsync(PageSize, offset);
                }
                else
                {
                    // テーブル名を抽出 (例: "会社 (companies)" -> "companies")
                    var tableName = ExtractTableName(SelectedTableFilter);
                    logs = await _repository.GetAuditLogsByTableAsync(tableName, PageSize);
                }

                AuditLogs.Clear();
                foreach (var log in logs)
                {
                    AuditLogs.Add(log);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"監査ログの読み込みに失敗: {ex.Message}");
                MessageBox.Show($"監査ログの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private string ExtractTableName(string filter)
        {
            var start = filter.IndexOf('(');
            var end = filter.IndexOf(')');
            if (start >= 0 && end > start)
            {
                return filter.Substring(start + 1, end - start - 1);
            }
            return filter.ToLower();
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            CurrentPage = 1;
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task NextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task FirstPageAsync()
        {
            if (CurrentPage != 1)
            {
                CurrentPage = 1;
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task LastPageAsync()
        {
            if (CurrentPage != TotalPages)
            {
                CurrentPage = TotalPages;
                await LoadDataAsync();
            }
        }

        partial void OnSelectedTableFilterChanged(string value)
        {
            CurrentPage = 1;
            _ = LoadDataAsync();
        }
    }
}
