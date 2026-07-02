using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace GriffinsGeneralTestMM
{
    public partial class FormAllSubMMWindow : Window
    {
        public FormAllSubMMViewModel ViewModel => (FormAllSubMMViewModel)DataContext!;

        public FormAllSubMMWindow(int execPercent)
        {   
            // 仅在调试模式下附加开发者工具，避免发布模式报错
            #if DEBUG
                        this.AttachDevTools();
            #endif
            InitializeComponent();
            DataContext = new FormAllSubMMViewModel(execPercent);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// 添加子模组控件
        /// </summary>
        public void AddTestSubMM(UctlTestSubMMView uctlTestSubMM)
        {
            // 添加到UI和ViewModel
            WrapPanel SubMMWrapPanel =  this.FindControl<WrapPanel>("SubMMWrapPanel");
            SubMMWrapPanel.Children.Add(uctlTestSubMM);
            ViewModel.AddTestSubMM(uctlTestSubMM.ViewModel);
        }

        /// <summary>
        /// 调整执行延迟百分比
        /// </summary>
        public void AdjustCurExecPercent(int execPercent)
        {
            ViewModel.AdjustCurExecPercent(execPercent);
        }

        /// <summary>
        /// 窗体关闭时隐藏（禁止关闭）
        /// </summary>
        private void Window_Closing(object? sender, WindowClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}