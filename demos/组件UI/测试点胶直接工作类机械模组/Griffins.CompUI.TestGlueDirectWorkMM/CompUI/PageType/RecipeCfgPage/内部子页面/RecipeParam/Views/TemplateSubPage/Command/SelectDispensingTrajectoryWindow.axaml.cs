using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command;
using ReactiveUI;
using System.Reactive.Linq;
using System;

namespace DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;

public partial class SelectDispensingTrajectoryWindow : Window
{
    public SelectDispensingTrajectoryWindow()
    {
        InitializeComponent();
        // 监听 ViewModel 的 DialogResult 变化
        this.WhenAnyValue(x => x.DataContext!)
            .Where(ctx => ctx is SelectDispensingTrajectoryWindowViewModel)
            .Cast<SelectDispensingTrajectoryWindowViewModel>()
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