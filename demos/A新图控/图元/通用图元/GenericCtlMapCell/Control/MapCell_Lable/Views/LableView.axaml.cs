using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Griffins.Map.CtlMapCell.Generic.Lable.Convert;
using Griffins.Map.CtlMapCell.Generic.Lable.ViewModel;
using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Griffins.Map.CtlMapCell.Generic.Lable.View;

public partial class LableView : ReactiveUserControl<LableViewModel>
{
    public LableView()
    {
        InitializeComponent();
		this.WhenActivated(disposables =>
		{
			if (ViewModel == null) return;

			this.Bind(ViewModel,
				vm => vm.LableText,
				view => view.Lable.Text)
				.DisposeWith(disposables);

			this.OneWayBind(
				viewModel: ViewModel,
				vmProperty: vm => vm.LableColor, // VM属性：Color类型
				viewProperty: v => v.Lable.Foreground, // View属性：IBrush类型
				selector: color =>
				{
					// 单向转换逻辑（Color → IBrush），直接复用你的转换器
					var converter = new ColorToBrushConverter();
					return converter.Convert(color, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush
						   ?? new SolidColorBrush(Colors.Transparent); // 默认值兜底
				}
			).DisposeWith(disposables);

			this.OneWayBind(
				viewModel: ViewModel,
				vmProperty: vm => vm.BackColor, // VM属性：Color类型
				viewProperty: v => v.Lable.Background, // View属性：IBrush类型
				selector: color =>
				{
					// 单向转换逻辑（Color → IBrush），直接复用你的转换器
					var converter = new ColorToBrushConverter();
					return converter.Convert(color, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush
						   ?? new SolidColorBrush(Colors.Transparent); // 默认值兜底
				}
			).DisposeWith(disposables);

			// --------------------------
			// 添加交互效果（如颜色变化时闪烁）
			// --------------------------
			ViewModel.WhenAnyValue(vm => vm.BackColor)
			.Throttle(TimeSpan.FromMilliseconds(200))
			.Subscribe(_ =>
			{
				setOpacity(0.8);
				Observable.Timer(TimeSpan.FromMilliseconds(200))
					.Subscribe(_ => setOpacity(1))
					.DisposeWith(disposables);
			}).DisposeWith(disposables);
		});
	}

	private void setOpacity(double opacity)
	{
		if (Dispatcher.UIThread.CheckAccess())
		{
			Lable.Opacity = opacity;
		}
		else
		{
			Dispatcher.UIThread.Post(() =>
			{
				Lable.Opacity = opacity;
			});
		}
	}
}