using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Griffins.Map.CtlMapCell.Generic.Convert;
using Griffins.Map.CtlMapCell.Generic.ViewModel;
using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Griffins.Map.CtlMapCell.Generic;

public partial class ButtonView : ReactiveUserControl<ButtonViewModel>
{
	public ButtonView()
	{
		InitializeComponent();
		this.WhenActivated(disposables =>
		{
			if (ViewModel == null) return;

			this.Bind(
				ViewModel,
				vm => vm.ButtonName,          // ViewModel 属性
				v => v.CustomButton.Content   // View 控件属性
			).DisposeWith(disposables);

			this.OneWayBind(
				viewModel: ViewModel,
				vmProperty: vm => vm.TextColor, // VM属性：Color类型
				viewProperty: v => v.CustomButton.Foreground, // View属性：IBrush类型
				selector: color =>
				{
					// 单向转换逻辑（Color → IBrush），直接复用你的转换器
					var converter = new ColorToBrushConverter();
					return converter.Convert(color, typeof(IBrush), null, CultureInfo.CurrentCulture) as IBrush
						   ?? new SolidColorBrush(Colors.Transparent); // 默认值兜底
				}
			).DisposeWith(disposables);

			// 字体家族（TextFont.FontFamily）
			this.Bind(
				ViewModel,
				vm => vm.TextFont.FontFamily,
				v => v.CustomButton.FontFamily
			).DisposeWith(disposables);

			// 字体大小（TextFont.FontSize）
			this.Bind(
				ViewModel,
				vm => vm.TextFont.FontSize,
				v => v.CustomButton.FontSize
			).DisposeWith(disposables);

			// 字体粗细（TextFont.FontWeight）
			this.Bind(
				ViewModel,
				vm => vm.TextFont.FontWeight,
				v => v.CustomButton.FontWeight
			).DisposeWith(disposables);

			this.BindCommand<ButtonView, ButtonViewModel, ReactiveCommand<Point, Unit>, Avalonia.Controls.Button, Point>(
				viewModel: ViewModel,
				propertyName: vm => vm.ButtonClickCommand,
				controlName: v => v.CustomButton,
				withParameter: Observable.FromEventPattern<RoutedEventArgs>(
						addHandler: h => CustomButton.Click += h, // 泛型事件，类型匹配
						removeHandler: h => CustomButton.Click -= h
					).Select(_ =>
					{
						PixelPoint screenPixel = CustomButton.PointToScreen(new Point(0, 0));
						return new Point(screenPixel.X, screenPixel.Y);
					})).DisposeWith(disposables);

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
			CustomButton.Opacity = opacity;
		}
		else
		{
			Dispatcher.UIThread.Post(() =>
			{
				CustomButton.Opacity = opacity;
			});
		}
	}
}