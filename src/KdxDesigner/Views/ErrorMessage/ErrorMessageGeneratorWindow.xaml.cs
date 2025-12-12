using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.Services.MnemonicDevice;
using KdxDesigner.ViewModels.ErrorMessage;
using System.Windows;

namespace KdxDesigner.Views.ErrorMessage
{
    /// <summary>
    /// ErrorMessageGeneratorWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ErrorMessageGeneratorWindow : Window
    {
        public ErrorMessageGeneratorWindow(
            ISupabaseRepository repository,
            IMnemonicDeviceMemoryStore memoryStore,
            int plcId)
        {
            InitializeComponent();
            DataContext = new ErrorMessageGeneratorViewModel(repository, memoryStore, plcId);
        }
    }
}