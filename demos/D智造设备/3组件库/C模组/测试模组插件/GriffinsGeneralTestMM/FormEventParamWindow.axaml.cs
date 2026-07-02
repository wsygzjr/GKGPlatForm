using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace GriffinsGeneralTestMM
{
    /// <summary>
    /// 事件参数读写页面（zgl该代码不完整）
    /// </summary>
    public partial class FormEventParamWindow : Window
    {  
        public FormEventParamWindow( )
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools(); // 调试模式开启Avalonia开发者工具
#endif 
        } 

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}