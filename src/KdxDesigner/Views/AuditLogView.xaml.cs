using KdxDesigner.ViewModels;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Windows;

namespace KdxDesigner.Views
{
    public partial class AuditLogView : Window
    {
        public AuditLogView(ISupabaseRepository repository)
        {
            InitializeComponent();
            var viewModel = new AuditLogViewModel(repository);
            DataContext = viewModel;

            Loaded += async (s, e) =>
            {
                await viewModel.LoadDataCommand.ExecuteAsync(null);
            };
        }
    }
}
