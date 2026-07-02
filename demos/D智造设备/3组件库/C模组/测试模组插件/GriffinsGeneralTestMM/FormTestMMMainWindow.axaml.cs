using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace GriffinsGeneralTestMM
{
    public partial class FormTestMMMainWindow : Window
    {
        // 暴露ViewModel供全局类访问
        public FormTestMMMainViewModel ViewModel  ; 
        public FormTestMMMainWindow()
        {
#if DEBUG
            this.AttachDevTools();
#endif
            ViewModel = (FormTestMMMainViewModel)DataContext!; 
            // 1. 启用 DevTools（关键：注册调试工具服务） 
            // 仅在调试模式下附加开发者工具，避免发布模式报错


            InitializeComponent();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// 窗口关闭事件：保留原WinForms的“禁止关闭”逻辑
        /// </summary>
        private void Window_Closing(object? sender, WindowClosingEventArgs e)
        {
            //e.Cancel = true; // 禁止窗口关闭（和原FormPrinting_FormClosing一致）
        }

		private void Border_PointerPressed(object sender, PointerPressedEventArgs e)
		{
			if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
			{
				BeginMoveDrag(e);
			}
		}
	}
}