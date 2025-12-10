using System.Windows;
using KdxDesigner.ViewModels.ProjectInfo;

namespace KdxDesigner.Views.ProjectInfo
{
    /// <summary>
    /// 会社プロパティウィンドウ
    /// Companyの新規作成・編集を行う
    /// </summary>
    public partial class CompanyPropertiesWindow : Window
    {
        public CompanyPropertiesWindow(CompanyPropertiesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.RequestClose += () => Close();
        }
    }
}
