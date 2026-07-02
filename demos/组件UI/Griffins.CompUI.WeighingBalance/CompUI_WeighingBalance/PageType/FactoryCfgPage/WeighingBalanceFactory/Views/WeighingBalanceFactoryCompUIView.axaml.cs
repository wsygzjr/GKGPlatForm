using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.FactoryCfgPage.WeighingBalanceFactory.ViewModels;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.FactoryCfgPage.WeighingBalanceFactory.Views
{
    public partial class WeighingBalanceFactoryCompUIView : ReactiveUserControl<WeighingBalanceFactoryCompUIViewModel>
    {
        public WeighingBalanceFactoryCompUIView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
