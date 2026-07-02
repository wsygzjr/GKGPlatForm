using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NonMainFrameViewModel.ViewModels;

namespace NonMainFrameView.Views
{
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
			this.Loaded += ConfigWindow_Loaded;
		}

		//private void ConfigWindow_Closing(object? sender, WindowClosingEventArgs e)
		//{
		//    if (DataContext is ConfigViewModel vm)
		//    {
		//        vm.CloseWindow();
		//    }
		//}

		private void ConfigWindow_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			// 获取命名的 TabControl
			var tabControl = this.FindControl<TabControl>("ConfigTabControl");
			if (tabControl == null)
				return;
			// 关闭命令绑定
			if (DataContext is ConfigViewModel vm)
			{
				// 订阅选项卡切换事件
				tabControl.SelectionChanged += (s, args) =>
				{
					int selectedIndex = tabControl.SelectedIndex;
					//vm.TabSelectionChangedCommand.Execute(selectedIndex);
					vm.OnTabSelected(selectedIndex);
				};
			}
		}

		private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
