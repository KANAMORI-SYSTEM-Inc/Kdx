using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kdx.Contracts.DTOs;
using Kdx.Contracts.Enums;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.Models;
using KdxDesigner.Services.MnemonicDevice;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MnemonicDevice = Kdx.Contracts.DTOs.MnemonicDevice;
using MnemonicSpeedDevice = Kdx.Contracts.DTOs.MnemonicSpeedDevice;

namespace KdxDesigner.ViewModels
{
    /// <summary>
    /// メモリデバイスリスト表示用ViewModel
    /// </summary>
    public partial class MemoryDeviceListViewModel : ObservableObject
    {
        private readonly IMnemonicDeviceMemoryStore _memoryStore;
        private readonly SupabaseRepository? _supabaseRepository;
        private readonly int? _plcId;
        private readonly int? _cycleId;

        [ObservableProperty] private ObservableCollection<MnemonicDevice> _mnemonicDevices = new();
        [ObservableProperty] private ObservableCollection<MnemonicTimerDevice> _timerDevices = new();
        [ObservableProperty] private ObservableCollection<MnemonicSpeedDevice> _speedDevices = new();
        [ObservableProperty] private ObservableCollection<InterlockDisplayItem> _interlocks = new();

        [ObservableProperty] private int _selectedTabIndex = 0;
        [ObservableProperty] private string _statusMessage = string.Empty;

        // フィルタリング用
        [ObservableProperty] private string _filterText = string.Empty;
        [ObservableProperty] private int? _selectedMnemonicType;

        // 統計情報
        [ObservableProperty] private int _totalMnemonicDeviceCount;
        [ObservableProperty] private int _totalTimerDeviceCount;
        [ObservableProperty] private int _totalSpeedDeviceCount;
        [ObservableProperty] private int _totalInterlockCount;
        [ObservableProperty] private int _processCount;
        [ObservableProperty] private int _detailCount;
        [ObservableProperty] private int _operationCount;
        [ObservableProperty] private int _cylinderCount;

        public ObservableCollection<MnemonicTypeItem> MnemonicTypes { get; } = new()
        {
            new MnemonicTypeItem { Id = null, Name = "すべて" },
            new MnemonicTypeItem { Id = (int)MnemonicType.Process, Name = "工程" },
            new MnemonicTypeItem { Id = (int)MnemonicType.ProcessDetail, Name = "工程詳細" },
            new MnemonicTypeItem { Id = (int)MnemonicType.Operation, Name = "操作" },
            new MnemonicTypeItem { Id = (int)MnemonicType.CY, Name = "シリンダ" }
        };

        public MemoryDeviceListViewModel(
            IMnemonicDeviceMemoryStore memoryStore,
            SupabaseRepository? supabaseRepository = null,
            int? plcId = null,
            int? cycleId = null)
        {
            _memoryStore = memoryStore ?? throw new ArgumentNullException(nameof(memoryStore));
            _supabaseRepository = supabaseRepository;
            _plcId = plcId;
            _cycleId = cycleId;

            LoadData();
        }

        partial void OnFilterTextChanged(string value)
        {
            ApplyFilter();
        }

        partial void OnSelectedMnemonicTypeChanged(int? value)
        {
            ApplyFilter();
        }

        private void LoadData()
        {
            try
            {
                // MnemonicDevice データの読み込み
                if (_plcId.HasValue)
                {
                    var devices = _memoryStore.GetMnemonicDevices(_plcId.Value);
                    // KdxDesigner.Models から Kdx.Contracts.DTOs へマッピング
                    var dtoDevices = devices.Select(d => new MnemonicDevice
                    {
                        ID = d.ID,
                        MnemonicId = d.MnemonicId,
                        RecordId = d.RecordId,
                        DeviceLabel = d.DeviceLabel,
                        StartNum = d.StartNum,
                        OutCoilCount = d.OutCoilCount,
                        PlcId = d.PlcId,
                        Comment1 = d.Comment1,
                        Comment2 = d.Comment2
                    }).ToList();
                    MnemonicDevices = new ObservableCollection<MnemonicDevice>(dtoDevices);
                    
                    // 統計情報の更新
                    TotalMnemonicDeviceCount = devices.Count;
                    ProcessCount = devices.Count(d => d.MnemonicId == (int)MnemonicType.Process);
                    DetailCount = devices.Count(d => d.MnemonicId == (int)MnemonicType.ProcessDetail);
                    OperationCount = devices.Count(d => d.MnemonicId == (int)MnemonicType.Operation);
                    CylinderCount = devices.Count(d => d.MnemonicId == (int)MnemonicType.CY);
                }
                
                // TimerDevice データの読み込み
                if (_plcId.HasValue && _cycleId.HasValue)
                {
                    var timers = _memoryStore.GetTimerDevices(_plcId.Value, _cycleId.Value);
                    TimerDevices = new ObservableCollection<MnemonicTimerDevice>(timers);
                    TotalTimerDeviceCount = timers.Count;
                }
                
                // SpeedDevice データの読み込み
                if (_plcId.HasValue)
                {
                    var speeds = _memoryStore.GetSpeedDevices(_plcId.Value);
                    // KdxDesigner.Models から Kdx.Contracts.DTOs へマッピング
                    var dtoSpeeds = speeds.Select(s => new MnemonicSpeedDevice
                    {
                        ID = s.ID,
                        CylinderId = s.CylinderId,
                        Device = s.Device,
                        PlcId = s.PlcId
                    }).ToList();
                    SpeedDevices = new ObservableCollection<MnemonicSpeedDevice>(dtoSpeeds);
                    TotalSpeedDeviceCount = speeds.Count;
                }

                // Interlock データの読み込み
                _ = LoadInterlocksAsync();

                StatusMessage = $"データ読み込み完了: {DateTime.Now:yyyy/MM/dd HH:mm:ss}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"エラー: {ex.Message}";
                MessageBox.Show($"データの読み込みに失敗しました。\n{ex.Message}", "エラー", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            // フィルタリング実装
            // 本実装では、CollectionViewSourceを使用してフィルタリングすることを推奨
            // ここでは簡易的な実装を示す
            if (_plcId.HasValue)
            {
                var devices = _memoryStore.GetMnemonicDevices(_plcId.Value);
                
                // Mnemonicタイプでフィルタ
                if (SelectedMnemonicType.HasValue)
                {
                    devices = devices.Where(d => d.MnemonicId == SelectedMnemonicType.Value).ToList();
                }
                
                // テキストフィルタ
                if (!string.IsNullOrWhiteSpace(FilterText))
                {
                    var filterLower = FilterText.ToLower();
                    devices = devices.Where(d => 
                        d.DeviceLabel?.ToLower().Contains(filterLower) == true ||
                        d.StartNum.ToString().Contains(FilterText) ||
                        d.Comment1?.ToLower().Contains(filterLower) == true ||
                        d.Comment2?.ToLower().Contains(filterLower) == true
                    ).ToList();
                }
                
                // KdxDesigner.Models から Kdx.Contracts.DTOs へマッピング
                var dtoDevices = devices.Select(d => new MnemonicDevice
                {
                    ID = d.ID,
                    MnemonicId = d.MnemonicId,
                    RecordId = d.RecordId,
                    DeviceLabel = d.DeviceLabel,
                    StartNum = d.StartNum,
                    OutCoilCount = d.OutCoilCount,
                    PlcId = d.PlcId,
                    Comment1 = d.Comment1,
                    Comment2 = d.Comment2
                }).ToList();
                MnemonicDevices = new ObservableCollection<MnemonicDevice>(dtoDevices);
            }
        }

        [RelayCommand]
        private void Refresh()
        {
            LoadData();
        }

        [RelayCommand]
        private void ClearFilter()
        {
            FilterText = string.Empty;
            SelectedMnemonicType = null;
            LoadData();
        }

        [RelayCommand]
        private void ExportToCsv()
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    DefaultExt = ".csv",
                    FileName = $"MemoryDevices_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (dialog.ShowDialog() == true)
                {
                    using (var writer = new System.IO.StreamWriter(dialog.FileName, false, System.Text.Encoding.UTF8))
                    {
                        // ヘッダー行
                        writer.WriteLine("ID,MnemonicId,RecordId,DeviceLabel,StartNum,OutCoilCount,PlcId,Comment1,Comment2");
                        
                        // データ行
                        foreach (var device in MnemonicDevices)
                        {
                            writer.WriteLine($"{device.ID},{device.MnemonicId},{device.RecordId}," +
                                $"{device.DeviceLabel},{device.StartNum},{device.OutCoilCount}," +
                                $"{device.PlcId}," +
                                $"\"{device.Comment1}\",\"{device.Comment2}\"");
                        }
                    }
                    
                    MessageBox.Show($"CSVファイルを出力しました。\n{dialog.FileName}", "完了", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CSV出力に失敗しました。\n{ex.Message}", "エラー", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Close(Window window)
        {
            window?.Close();
        }

        /// <summary>
        /// インターロックデータを非同期で読み込み
        /// </summary>
        private async Task LoadInterlocksAsync()
        {
            if (_supabaseRepository == null || !_plcId.HasValue)
            {
                Interlocks = new ObservableCollection<InterlockDisplayItem>();
                TotalInterlockCount = 0;
                return;
            }

            try
            {
                // インターロック一覧を取得
                var interlocks = await _supabaseRepository.GetInterlocksByPlcIdAsync(_plcId.Value);

                // シリンダー名を取得するために全シリンダーを取得
                var cylinders = await _supabaseRepository.GetCYsAsync();
                var cylinderDict = cylinders.ToDictionary(c => c.Id, c => !string.IsNullOrEmpty(c.CYNum) ? c.CYNum : $"CY{c.Id}");

                // 表示用アイテムに変換
                var displayItems = interlocks.Select(i => new InterlockDisplayItem
                {
                    CylinderId = i.CylinderId,
                    CylinderName = cylinderDict.TryGetValue(i.CylinderId, out var name) ? name : $"CY{i.CylinderId}",
                    SortId = i.SortId,
                    ConditionCylinderId = i.ConditionCylinderId,
                    ConditionCylinderName = cylinderDict.TryGetValue(i.ConditionCylinderId, out var condName) ? condName : $"CY{i.ConditionCylinderId}",
                    GoOrBack = i.GoOrBack,
                    GoOrBackDisplay = i.GoOrBack switch
                    {
                        0 => "Go&Back",
                        1 => "GoOnly",
                        2 => "BackOnly",
                        _ => $"不明({i.GoOrBack})"
                    },
                    PreConditionID1 = i.PreConditionID1,
                    PreConditionID2 = i.PreConditionID2,
                    PreConditionID3 = i.PreConditionID3,
                    PlcId = i.PlcId
                }).OrderBy(i => i.CylinderId).ThenBy(i => i.SortId).ToList();

                Interlocks = new ObservableCollection<InterlockDisplayItem>(displayItems);
                TotalInterlockCount = displayItems.Count;
            }
            catch (Exception ex)
            {
                StatusMessage = $"インターロック読み込みエラー: {ex.Message}";
                Interlocks = new ObservableCollection<InterlockDisplayItem>();
                TotalInterlockCount = 0;
            }
        }
    }

    public class MnemonicTypeItem
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// インターロック表示用アイテム
    /// </summary>
    public class InterlockDisplayItem
    {
        public int CylinderId { get; set; }
        public string CylinderName { get; set; } = string.Empty;
        public int SortId { get; set; }
        public int ConditionCylinderId { get; set; }
        public string ConditionCylinderName { get; set; } = string.Empty;
        public int GoOrBack { get; set; }
        public string GoOrBackDisplay { get; set; } = string.Empty;
        public int? PreConditionID1 { get; set; }
        public int? PreConditionID2 { get; set; }
        public int? PreConditionID3 { get; set; }
        public int PlcId { get; set; }
    }
}
