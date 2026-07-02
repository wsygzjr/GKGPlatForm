using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.SubTemplate;
using ReactiveUI;
using System.Reactive.Linq;
using System;

namespace DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;

public partial class SubTemplateItemEditWindow : Window
{
    public SubTemplateItemEditWindow()
    {
        InitializeComponent();
        // 监听 ViewModel 的 DialogResult 变化
        this.WhenAnyValue(x => x.DataContext!)
            .Where(ctx => ctx is SubTemplateItemEditWindowViewModel)
            .Cast<SubTemplateItemEditWindowViewModel>()
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
}