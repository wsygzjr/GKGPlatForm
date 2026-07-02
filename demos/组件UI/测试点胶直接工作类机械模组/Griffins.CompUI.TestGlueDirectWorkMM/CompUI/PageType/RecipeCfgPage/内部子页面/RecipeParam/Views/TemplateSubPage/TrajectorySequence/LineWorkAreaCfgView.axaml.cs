using Avalonia.Controls;
using Avalonia.Markup.Xaml; 
using ReactiveUI; // 必须引用 ReactiveUI
using Avalonia; 
using Avalonia.Data;
using Avalonia.Media; 
using System;
using System.Collections;
using System.Reactive.Disposables;

namespace DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation
{
    public partial class LineWorkAreaCfgView : UserControl
    {
        public LineWorkAreaCfgView()
        {
            InitializeComponent();

            //// 激活 ReactiveUI 绑定，确保命令正常工作
            //this.WhenActivated(disposables =>
            //{
            //    // 空实现即可，主要用于激活 ViewModel 的 Reactive 上下文
            //});
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
