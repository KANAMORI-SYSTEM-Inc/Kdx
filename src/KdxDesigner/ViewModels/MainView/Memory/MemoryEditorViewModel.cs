// ViewModel: MemoryEditorViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.Models;
using Kdx.Contracts.DTOs;
using Kdx.Contracts.Interfaces;


using Microsoft.Win32;
using Microsoft.Extensions.DependencyInjection;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace KdxDesigner.ViewModels
{
    public partial class MemoryEditorViewModel : ObservableObject
    {
        private readonly ISupabaseRepository _repository = null!; // コンストラクタで初期化される
        public int _plcId;

        public MemoryEditorViewModel(ISupabaseRepository repository)
        {
            _repository = repository;
            FilteredMemories = CollectionViewSource.GetDefaultView(Memories);
            FilteredMemories.Filter = FilterByCategory;
        }

        // アプリ側でのデバイス一覧変数を一次保存する
        [ObservableProperty]
        private ObservableCollection<Memory> _memories = new();

        // メモリカテゴリ（M, L, D...）を格納
        [ObservableProperty]
        private ObservableCollection<MemoryCategory> _memoryCategories = new();

        // ドロップダウンで選択されたメモリカテゴリ
        [ObservableProperty]
        private MemoryCategory? _selectedMemoryCategory;

        // ステータスメッセージ表示用
        [ObservableProperty]
        private string _saveStatusMessage = string.Empty;

        // Category絞り込み用
        [ObservableProperty]
        private ObservableCollection<string> _categoryFilterItems = new() { "(すべて)" };

        [ObservableProperty]
        private string _selectedCategoryFilter = "(すべて)";

        // フィルタ用CollectionView
        public ICollectionView FilteredMemories { get; private set; }

        // 初期化メソッド
        public MemoryEditorViewModel(int plcId, ISupabaseRepository repository)
        {
            _plcId = plcId;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            // メモリカテゴリドロップダウンのリスト取得
            var memoryService = App.Services?.GetService<IMemoryService>()
                ?? new Kdx.Infrastructure.Services.MemoryService(_repository);
            MemoryCategories = new ObservableCollection<MemoryCategory>(memoryService.GetMemoryCategories());
            Memories = new ObservableCollection<Memory>();

            // フィルタ用CollectionViewの初期化
            FilteredMemories = CollectionViewSource.GetDefaultView(Memories);
            FilteredMemories.Filter = FilterByCategory;
        }

        /// <summary>
        /// Categoryフィルタ条件
        /// </summary>
        private bool FilterByCategory(object obj)
        {
            if (string.IsNullOrEmpty(SelectedCategoryFilter) || SelectedCategoryFilter == "(すべて)")
                return true;

            if (obj is Memory memory)
            {
                return memory.Category == SelectedCategoryFilter;
            }
            return false;
        }

        /// <summary>
        /// Category絞り込み選択変更時にフィルタを更新
        /// </summary>
        partial void OnSelectedCategoryFilterChanged(string value)
        {
            FilteredMemories.Refresh();
        }

        /// <summary>
        /// Memoriesが更新されたときにCategoryFilterItemsを更新
        /// </summary>
        partial void OnMemoriesChanged(ObservableCollection<Memory> value)
        {
            UpdateCategoryFilterItems();
            // CollectionViewを再設定
            FilteredMemories = CollectionViewSource.GetDefaultView(value);
            FilteredMemories.Filter = FilterByCategory;
            OnPropertyChanged(nameof(FilteredMemories));
        }

        /// <summary>
        /// MemoriesからユニークなCategoryを抽出してフィルタリストを更新
        /// </summary>
        private void UpdateCategoryFilterItems()
        {
            var categories = Memories
                .Where(m => !string.IsNullOrEmpty(m.Category))
                .Select(m => m.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            CategoryFilterItems.Clear();
            CategoryFilterItems.Add("(すべて)");
            foreach (var category in categories)
            {
                CategoryFilterItems.Add(category);
            }

            // 選択をリセット
            SelectedCategoryFilter = "(すべて)";
        }


        [RelayCommand]
        private async Task DBSave()
        {
            SaveStatusMessage = "保存中...";

            await Task.Run(() =>
            {
                var memoryService = App.Services?.GetService<IMemoryService>()
                ?? new Kdx.Infrastructure.Services.MemoryService(_repository);
                memoryService.SaveMemories(_plcId, Memories.ToList(), msg =>
                {
                    // UIスレッドに戻してメッセージ更新
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        SaveStatusMessage = msg;
                    });
                });
            });

            SaveStatusMessage = "保存完了！";
        }


        [RelayCommand]
        private void Cancel()
        {
            Memories = new();
        }

        [RelayCommand]
        private void ImportCsv()
        {
            // 最初にDBから読み出されたデータをクリア
            Memories.Clear();
            var dialog = new OpenFileDialog { Filter = "CSV files (*.csv)|*.csv" };


            if (dialog.ShowDialog() == true)
            {
                var lines = File.ReadAllLines(dialog.FileName);
                int? categoryId = SelectedMemoryCategory?.ID;

                var imported = lines.Skip(1) // ヘッダーをスキップ
                    .Select(line =>
                    {
                        var cols = line.Split(',');
                        string device;
                        if (cols.ElementAtOrDefault(2) == null || cols.ElementAtOrDefault(2) == "")
                        {
                            if(cols.ElementAtOrDefault(1) != null)
                            {
                                device = cols.ElementAtOrDefault(1)!;
                            }
                            else
                            {
                                device = "";
                            }
                        }
                        else
                        {
                            device = cols.ElementAtOrDefault(1) + "." + cols.ElementAtOrDefault(2);
                        }

                        return new Memory
                        {
                            PlcId = _plcId,
                            MemoryCategory = categoryId,
                            DeviceNumber = TryParseInt(cols.ElementAtOrDefault(0)),
                            DeviceNumber1 = cols.ElementAtOrDefault(1),
                            DeviceNumber2 = cols.ElementAtOrDefault(2),
                            Device = device!,
                            Category = cols.ElementAtOrDefault(3),
                            Row_1 = cols.ElementAtOrDefault(4),
                            Row_2 = cols.ElementAtOrDefault(5),
                            Row_3 = cols.ElementAtOrDefault(6),
                            Row_4 = cols.ElementAtOrDefault(7),
                            Direct_Input = cols.ElementAtOrDefault(8),
                            Confirm = cols.ElementAtOrDefault(9),
                            Note = cols.ElementAtOrDefault(10)
                        };
                    }).ToList();

                foreach (var mem in imported)
                {
                    Memories.Add(mem);
                }
            }
        }

        [RelayCommand]
        public void DBImport()
        {
            try
            {
                // DBからすべてのメモリを取得
                var memoryService = App.Services?.GetService<IMemoryService>()
                    ?? new Kdx.Infrastructure.Services.MemoryService(_repository);
                var allMemories = memoryService.GetMemories(_plcId);

                // デバッグ情報をステータスメッセージに表示
                SaveStatusMessage = $"取得データ数: {allMemories.Count}件";

                // フィルタされたリストを一時変数に
                IEnumerable<Memory> filteredMemories = allMemories;

                if (SelectedMemoryCategory != null)
                {
                    int id = SelectedMemoryCategory.ID;
                    var beforeCount = filteredMemories.Count();

                    // MemoryCategoryがnullのデータも含めて、選択されたカテゴリまたはnullのデータを表示
                    // これにより、MnemonicDeviceから生成されたMemoryCategoryがnullのデータも表示される
                    filteredMemories = filteredMemories.Where(m => m.MemoryCategory == id || m.MemoryCategory == null);

                    var afterCount = filteredMemories.Count();
                    SaveStatusMessage = $"取得: {allMemories.Count}件, フィルタ後: {afterCount}件 (カテゴリID: {id})";
                }
                else
                {
                    SaveStatusMessage = $"取得: {allMemories.Count}件 (フィルタなし)";
                }

                // ObservableCollection に再代入（ここで UI に変更が通知される）
                Memories = new ObservableCollection<Memory>(filteredMemories);
            }
            catch (Exception ex)
            {
                SaveStatusMessage = $"エラー: {ex.Message}";
                MessageBox.Show($"データの取得中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private int? TryParseInt(string? input)
        {
            return int.TryParse(input, out var result) ? result : null;
        }
    }
}
