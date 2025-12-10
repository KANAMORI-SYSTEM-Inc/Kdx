using System.Windows;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.ViewModels;

namespace KdxDesigner.Views.Interlock
{
    public partial class InterlockIOPropertiesWindow : Window
    {
        private readonly InterlockIOPropertiesViewModel _viewModel;

        public InterlockIOPropertiesWindow(ISupabaseRepository repository, InterlockIO interlockIO, string? ioName)
        {
            InitializeComponent();
            _viewModel = new InterlockIOPropertiesViewModel(repository, interlockIO, ioName);
            DataContext = _viewModel;

            _viewModel.RequestClose += () =>
            {
                DialogResult = _viewModel.DialogResult;
                Close();
            };
        }

        /// <summary>
        /// 更新されたInterlockIOを取得
        /// </summary>
        public InterlockIO GetUpdatedInterlockIO() => _viewModel.GetUpdatedInterlockIO();
    }
}
