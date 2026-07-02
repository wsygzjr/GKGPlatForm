using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.InitCfgPage.WeighingBalance.ViewModels;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.InitCfgPage.WeighingBalance.Views
{
    public partial class WeighingBalanceInitCfgCompUIView : ReactiveUserControl<WeighingBalanceInitCfgCompUIViewModel>
    {
        public WeighingBalanceInitCfgCompUIView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
