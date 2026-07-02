using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using GKG.Map.AuxiliaryInfoFuncCtlMapCell.Convert;
using GKG.Map.AuxiliaryInfoFuncCtlMapCell.ViewModel;
using Griffins.UI2;
using ReactiveUI;
using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.AuxiliaryInfoFuncCtlMapCell.View;

/// <summary>
/// 辅助信息功能图元视图类
/// 负责显示辅助信息图元的用户界面，包括背景颜色、文本颜色等属性的绑定
/// </summary>
public partial class AuxiliaryInfoView : ReactiveUserControl<AuxiliaryInfoViewModel>
{
    #region 构造函数
    /// <summary>
    /// 初始化辅助信息视图
    /// 设置组件初始化和数据绑定
    /// </summary>
    public AuxiliaryInfoView()
    {
        InitializeComponent();

        #region 视图激活时的数据绑定
        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) return;

            #region 背景颜色绑定
            // Bind background color
            this.OneWayBind(
                viewModel: ViewModel,
                vmProperty: vm => vm.BackColor,
                viewProperty: v => v.MainBorder.Background,
                selector: color =>
                {
                    var brushConverter = new ColorToBrushConverter();
                    return brushConverter.Convert(color, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush
                           ?? new SolidColorBrush(Colors.Transparent);
                }
            ).DisposeWith(disposables);
            #endregion

            // 注册弹窗交互
            this.BindInteraction(ViewModel, vm => vm.ShowTipDialog, async interaction =>
            {
                var tipView = new TipDialogView
                {
                    DataContext = interaction.Input
                };

                if(this.GetVisualRoot() is Window parentWindow)
                {
                    var result = await tipView.ShowDialog<DialogResultType>(parentWindow);
                    interaction.SetOutput(result);
                }
                else
                {
                    
                    interaction.SetOutput(DialogResultType.Cancel);
                }
            }).DisposeWith(disposables);

            #region 文本颜色绑定
            //this.WhenAnyValue(x => x.ViewModel!.TextColor)
            //    .Subscribe(c => ApplyTextColor(new SolidColorBrush(c)))
            //    .DisposeWith(disposables);

            //this.WhenAnyValue(x => x.ViewModel!.TextFont)
            //    .Subscribe(f => ApplyTextFont(f))
            //    .DisposeWith(disposables);

            //this.WhenAnyValue(x => x.ViewModel!.TextFont.FontFamily)
            //    .Subscribe(_ => ApplyTextFont(ViewModel.TextFont))
            //    .DisposeWith(disposables);

            //this.WhenAnyValue(x => x.ViewModel!.TextFont.FontSize)
            //    .Subscribe(_ => ApplyTextFont(ViewModel.TextFont))
            //    .DisposeWith(disposables);

            //this.WhenAnyValue(x => x.ViewModel!.TextFont.FontWeight)
            //    .Subscribe(_ => ApplyTextFont(ViewModel.TextFont))
            //    .DisposeWith(disposables);

            //this.WhenAnyValue(x => x.ViewModel!.TextFont.FontStyle)
            //    .Subscribe(_ => ApplyTextFont(ViewModel.TextFont))
            //    .DisposeWith(disposables);

            //ApplyTextColor(new SolidColorBrush(ViewModel.TextColor));
            //ApplyTextFont(ViewModel.TextFont);
            #endregion
        });
        #endregion
    }
    #endregion

    private void ApplyTextColor(IBrush brush)
    {
        if (brush == null) return;
        foreach (var tb in MainBorder.GetVisualDescendants().OfType<TextBlock>())
        {
            if (tb.FindAncestorOfType<Button>() != null)
                continue;
            tb.Foreground = brush;
        }
        foreach (var txt in MainBorder.GetVisualDescendants().OfType<TextBox>())
            txt.Foreground = brush;
    }

    private void ApplyTextFont(FontInfo font)
    {
        if (font == null) return;

        foreach (var tb in MainBorder.GetVisualDescendants().OfType<TextBlock>())
        {
            tb.FontFamily = font.FontFamily;
            tb.FontSize = font.FontSize;
            tb.FontWeight = font.FontWeight;
            tb.FontStyle = font.FontStyle;
        }

        foreach (var txt in MainBorder.GetVisualDescendants().OfType<TextBox>())
        {
            txt.FontFamily = font.FontFamily;
            txt.FontSize = font.FontSize;
            txt.FontWeight = font.FontWeight;
            txt.FontStyle = font.FontStyle;
        }

        foreach (var btn in MainBorder.GetVisualDescendants().OfType<Button>())
        {
            btn.FontFamily = font.FontFamily;
            btn.FontSize = font.FontSize;
            btn.FontWeight = font.FontWeight;
            btn.FontStyle = font.FontStyle;
        }
    }
}
