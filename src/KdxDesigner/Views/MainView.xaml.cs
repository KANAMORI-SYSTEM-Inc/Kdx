using Kdx.Contracts.DTOs;
using KdxDesigner.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace KdxDesigner.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = App.Services!.GetRequiredService<MainViewModel>();
        }

        /// <summary>
        /// ProcessListControlの選択変更イベントハンドラ
        /// </summary>
        private void ProcessListControl_ProcessSelectionChanged(object? sender, Process? selectedProcess)
        {
            if (DataContext is MainViewModel vm && selectedProcess != null)
            {
                vm.UpdateSelectedProcesses(new List<Process> { selectedProcess });
            }
        }

        /// <summary>
        /// ProcessListControlからの工程フロー詳細を開くリクエストハンドラ
        /// </summary>
        private void ProcessListControl_OpenProcessFlowDetailRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.OpenProcessFlowDetailCommand.CanExecute(null))
            {
                vm.OpenProcessFlowDetailCommand.Execute(null);
            }
        }

        /// <summary>
        /// ProcessDetailListControlの選択変更イベントハンドラ
        /// </summary>
        private void ProcessDetailListControl_SelectionChanged(object? sender, ProcessDetail? selectedProcessDetail)
        {
            if (DataContext is MainViewModel vm && selectedProcessDetail != null)
            {
                vm.OnProcessDetailSelected(selectedProcessDetail);
            }
        }
    }
}
