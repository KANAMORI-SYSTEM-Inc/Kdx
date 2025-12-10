using System.Windows;
using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.ViewModels;

namespace KdxDesigner.Views.Interlock
{
    public partial class InterlockConditionPropertiesWindow : Window
    {
        private readonly InterlockConditionPropertiesViewModel _viewModel;

        public InterlockConditionPropertiesWindow(ISupabaseRepository repository, InterlockConditionDTO condition, IEnumerable<InterlockConditionType> conditionTypes)
        {
            InitializeComponent();
            _viewModel = new InterlockConditionPropertiesViewModel(repository, condition, conditionTypes);
            DataContext = _viewModel;

            _viewModel.RequestClose += () =>
            {
                DialogResult = _viewModel.DialogResult;
                Close();
            };
        }

        /// <summary>
        /// 更新されたInterlockConditionDTOを取得
        /// </summary>
        public InterlockConditionDTO GetUpdatedCondition() => _viewModel.GetUpdatedCondition();
    }
}
