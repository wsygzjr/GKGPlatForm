using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Views;

public partial class PreDispensingView : UserControl
{
    public PreDispensingView()
    {
        InitializeComponent();
        DataContextChanged += (_, __) => trySetViewReference();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        trySetViewReference();
    }

    private void trySetViewReference()
    {
        if (DataContext is PreDispensingViewModel vm)
        {
            vm.SetViewReference(this);
        }
    }
}
