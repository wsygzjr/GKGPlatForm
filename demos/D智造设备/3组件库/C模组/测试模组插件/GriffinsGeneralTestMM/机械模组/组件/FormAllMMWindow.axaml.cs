using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;


namespace GriffinsGeneralTestMM
{
    public partial class FormAllMMWindow : Window
    {
        private FormAllMMViewModel _viewModel;

        public FormAllMMWindow(int execPercent)
        {   
            // 仅在调试模式下附加开发者工具，避免发布模式报错
            #if DEBUG
                        this.AttachDevTools();
            #endif
            InitializeComponent();
            _viewModel = new FormAllMMViewModel(execPercent);
            DataContext = _viewModel;

            // 绑定窗口关闭事件（隐藏而非关闭）
            Closing += OnClosing;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // 关闭时隐藏窗口
        private void OnClosing(object sender, WindowClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        internal void AddTestMM(UctlTestMMViewModel uctlTestMMViewModel)
        {
            _viewModel.AddTestMM(uctlTestMMViewModel);
        }

        public void AdjustCurExecPercent(int execPercent)
        {
            _viewModel.AdjustCurExecPercent(execPercent);
        }

       
    }
}