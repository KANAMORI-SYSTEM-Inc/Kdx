using System.Windows;
using KdxDesigner.ViewModels.ProjectInfo;

namespace KdxDesigner.Views.ProjectInfo
{
    /// <summary>
    /// PLCプロパティウィンドウ
    /// PLCの新規作成・編集を行う
    /// </summary>
    public partial class PLCPropertiesWindow : Window
    {
        public PLCPropertiesWindow(PLCPropertiesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.RequestClose += () => Close();
        }
    }
}
