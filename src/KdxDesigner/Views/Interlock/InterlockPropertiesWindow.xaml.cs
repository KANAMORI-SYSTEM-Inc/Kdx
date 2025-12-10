using System.Windows;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.ViewModels;

namespace KdxDesigner.Views.Interlock
{
    public partial class InterlockPropertiesWindow : Window
    {
        private readonly InterlockPropertiesViewModel _viewModel;

        public InterlockPropertiesWindow(ISupabaseRepository repository, Kdx.Contracts.DTOs.Interlock interlock, List<Cylinder> allCylinders)
        {
            InitializeComponent();
            _viewModel = new InterlockPropertiesViewModel(repository, interlock, allCylinders);
            DataContext = _viewModel;

            _viewModel.RequestClose += () =>
            {
                DialogResult = _viewModel.DialogResult;
                Close();
            };
        }

        /// <summary>
        /// 更新されたInterlockを取得
        /// </summary>
        public Kdx.Contracts.DTOs.Interlock GetUpdatedInterlock() => _viewModel.GetUpdatedInterlock();
    }
}
