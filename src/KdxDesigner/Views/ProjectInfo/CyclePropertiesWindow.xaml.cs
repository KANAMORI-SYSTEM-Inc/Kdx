using System.Windows;
using KdxDesigner.ViewModels.ProjectInfo;

namespace KdxDesigner.Views.ProjectInfo
{
    /// <summary>
    /// サイクルプロパティウィンドウ
    /// Cycleの新規作成・編集を行う
    /// </summary>
    public partial class CyclePropertiesWindow : Window
    {
        public CyclePropertiesWindow(CyclePropertiesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.RequestClose += () => Close();
        }
    }
}
