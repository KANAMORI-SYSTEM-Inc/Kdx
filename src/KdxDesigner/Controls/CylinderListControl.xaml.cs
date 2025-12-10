using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KdxDesigner.Controls
{
    /// <summary>
    /// シリンダー一覧を表示する共通コントロール
    /// </summary>
    public partial class CylinderListControl : UserControl
    {
        public CylinderListControl()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        // ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(CylinderListControl),
                new PropertyMetadata(null));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        // SelectedItem
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(CylinderListControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        // SearchText
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(CylinderListControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        // Commands
        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register(nameof(AddCommand), typeof(ICommand), typeof(CylinderListControl),
                new PropertyMetadata(null));

        public ICommand AddCommand
        {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty CopyCommandProperty =
            DependencyProperty.Register(nameof(CopyCommand), typeof(ICommand), typeof(CylinderListControl),
                new PropertyMetadata(null));

        public ICommand CopyCommand
        {
            get => (ICommand)GetValue(CopyCommandProperty);
            set => SetValue(CopyCommandProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(CylinderListControl),
                new PropertyMetadata(null));

        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(CylinderListControl),
                new PropertyMetadata(null));

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public static readonly DependencyProperty RefreshCommandProperty =
            DependencyProperty.Register(nameof(RefreshCommand), typeof(ICommand), typeof(CylinderListControl),
                new PropertyMetadata(null));

        public ICommand RefreshCommand
        {
            get => (ICommand)GetValue(RefreshCommandProperty);
            set => SetValue(RefreshCommandProperty, value);
        }

        public static readonly DependencyProperty ClearSearchCommandProperty =
            DependencyProperty.Register(nameof(ClearSearchCommand), typeof(ICommand), typeof(CylinderListControl),
                new PropertyMetadata(null));

        public ICommand ClearSearchCommand
        {
            get => (ICommand)GetValue(ClearSearchCommandProperty);
            set => SetValue(ClearSearchCommandProperty, value);
        }

        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.Register(nameof(DoubleClickCommand), typeof(ICommand), typeof(CylinderListControl),
                new PropertyMetadata(null));

        public ICommand DoubleClickCommand
        {
            get => (ICommand)GetValue(DoubleClickCommandProperty);
            set => SetValue(DoubleClickCommandProperty, value);
        }

        // Visibility flags
        public static readonly DependencyProperty ShowToolbarProperty =
            DependencyProperty.Register(nameof(ShowToolbar), typeof(bool), typeof(CylinderListControl),
                new PropertyMetadata(true));

        public bool ShowToolbar
        {
            get => (bool)GetValue(ShowToolbarProperty);
            set => SetValue(ShowToolbarProperty, value);
        }

        public static readonly DependencyProperty ShowAddButtonProperty =
            DependencyProperty.Register(nameof(ShowAddButton), typeof(bool), typeof(CylinderListControl),
                new PropertyMetadata(true));

        public bool ShowAddButton
        {
            get => (bool)GetValue(ShowAddButtonProperty);
            set => SetValue(ShowAddButtonProperty, value);
        }

        public static readonly DependencyProperty ShowCopyButtonProperty =
            DependencyProperty.Register(nameof(ShowCopyButton), typeof(bool), typeof(CylinderListControl),
                new PropertyMetadata(true));

        public bool ShowCopyButton
        {
            get => (bool)GetValue(ShowCopyButtonProperty);
            set => SetValue(ShowCopyButtonProperty, value);
        }

        public static readonly DependencyProperty ShowEditButtonProperty =
            DependencyProperty.Register(nameof(ShowEditButton), typeof(bool), typeof(CylinderListControl),
                new PropertyMetadata(true));

        public bool ShowEditButton
        {
            get => (bool)GetValue(ShowEditButtonProperty);
            set => SetValue(ShowEditButtonProperty, value);
        }

        public static readonly DependencyProperty ShowDeleteButtonProperty =
            DependencyProperty.Register(nameof(ShowDeleteButton), typeof(bool), typeof(CylinderListControl),
                new PropertyMetadata(true));

        public bool ShowDeleteButton
        {
            get => (bool)GetValue(ShowDeleteButtonProperty);
            set => SetValue(ShowDeleteButtonProperty, value);
        }

        public static readonly DependencyProperty ShowRefreshButtonProperty =
            DependencyProperty.Register(nameof(ShowRefreshButton), typeof(bool), typeof(CylinderListControl),
                new PropertyMetadata(true));

        public bool ShowRefreshButton
        {
            get => (bool)GetValue(ShowRefreshButtonProperty);
            set => SetValue(ShowRefreshButtonProperty, value);
        }

        public static readonly DependencyProperty ShowSearchBoxProperty =
            DependencyProperty.Register(nameof(ShowSearchBox), typeof(bool), typeof(CylinderListControl),
                new PropertyMetadata(false));

        public bool ShowSearchBox
        {
            get => (bool)GetValue(ShowSearchBoxProperty);
            set => SetValue(ShowSearchBoxProperty, value);
        }

        #endregion
    }
}
