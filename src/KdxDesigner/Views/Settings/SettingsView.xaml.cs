using System.Windows;

using KdxDesigner.ViewModels;

namespace KdxDesigner.Views.Settings
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel(this);
        }
    }
}