using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Views;

public partial class TwoDAdvancedParamWindow : Window
{
    public TwoDAdvancedParamWindow()
    {
        InitializeComponent();
        CanResize = false;
        // 监听 ViewModel 的 DialogResult 变化
        this.WhenAnyValue(x => x.DataContext!)
            .Where(ctx => ctx is TwoDAdvancedParamWindowViewModel)
            .Cast<TwoDAdvancedParamWindowViewModel>()
            .Subscribe(vm =>
            {
                // 当 DialogResult 被设置时关闭窗口
                vm.WhenAnyValue(x => x.DialogResult)
                    .Where(result => result.HasValue)
                    .Subscribe(result =>
                    {
                        Close(result!.Value); // 关闭并返回结果
                    });
            });
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Window_Activated(object? sender, System.EventArgs e)
    {
    }
}