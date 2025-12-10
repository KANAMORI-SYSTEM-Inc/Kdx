using System.Windows;
using KdxDesigner.ViewModels.ProjectInfo;

namespace KdxDesigner.Views.ProjectInfo
{
    /// <summary>
    /// プロジェクト情報管理ウィンドウ
    /// Company、Model、PLC、Cycleの一覧表示と管理を行う
    /// </summary>
    public partial class ProjectInfoWindow : Window
    {
        public ProjectInfoWindow(ProjectInfoViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // ViewModelからのウィンドウクローズ要求を処理
            viewModel.RequestClose += () => Close();
        }
    }
}
