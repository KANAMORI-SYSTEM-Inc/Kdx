using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.Services.MnemonicDevice;
using KdxDesigner.ViewModels;
using System.Windows;

namespace KdxDesigner.Views.Memory
{
    /// <summary>
    /// MemoryDeviceListWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MemoryDeviceListWindow : Window
    {
        public MemoryDeviceListWindow(
            IMnemonicDeviceMemoryStore memoryStore,
            SupabaseRepository? supabaseRepository = null,
            int? plcId = null,
            int? cycleId = null)
        {
            InitializeComponent();
            DataContext = new MemoryDeviceListViewModel(memoryStore, supabaseRepository, plcId, cycleId);
        }
    }
}