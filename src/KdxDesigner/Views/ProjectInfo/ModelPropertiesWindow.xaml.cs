using System.Windows;
using KdxDesigner.ViewModels.ProjectInfo;

namespace KdxDesigner.Views.ProjectInfo
{
    /// <summary>
    /// 機種プロパティウィンドウ
    /// Modelの新規作成・編集を行う
    /// </summary>
    public partial class ModelPropertiesWindow : Window
    {
        public ModelPropertiesWindow(ModelPropertiesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.RequestClose += () => Close();
        }
    }
}
