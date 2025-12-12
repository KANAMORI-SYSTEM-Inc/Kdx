using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.ViewModels.ErrorMessage;
using System.Windows;
using System.Windows.Controls;

namespace KdxDesigner.Views.ErrorMessage
{
    /// <summary>
    /// ErrorMessageEditorWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ErrorMessageEditorWindow : Window
    {
        private ErrorMessageEditorViewModel? _viewModel;

        public ErrorMessageEditorWindow(ISupabaseRepository repository)
        {
            InitializeComponent();
            _viewModel = new ErrorMessageEditorViewModel(repository);
            DataContext = _viewModel;
        }

        private void BaseMessage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            _viewModel.CurrentEditField = "BaseMessage";
            RadioBaseMessage.IsChecked = true;
        }

        private void BaseAlarm_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            _viewModel.CurrentEditField = "BaseAlarm";
            RadioBaseAlarm.IsChecked = true;
        }

        private void Category1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            _viewModel.CurrentEditField = "Category1";
            RadioCategory1.IsChecked = true;
        }

        private void Category2_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            _viewModel.CurrentEditField = "Category2";
            RadioCategory2.IsChecked = true;
        }

        private void Category3_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            _viewModel.CurrentEditField = "Category3";
            RadioCategory3.IsChecked = true;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // 初期化中はViewModelがまだnullの可能性があるため確認
            if (_viewModel == null) return;

            if (sender is RadioButton radioButton && radioButton.Content is string content)
            {
                _viewModel.CurrentEditField = content;
            }
        }
    }
}
