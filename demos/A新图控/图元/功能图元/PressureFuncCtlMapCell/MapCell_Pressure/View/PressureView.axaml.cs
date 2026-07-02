using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Griffins.Map.PressureFuncCtlMapCell.Convert;
using Griffins.Map.PressureFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Griffins.Map.PressureFuncCtlMapCell.View;

public partial class PressureView : ReactiveUserControl<PressureViewModel>
{
	public PressureView()
	{
		InitializeComponent();
		this.WhenActivated(disposables =>
		{
			if (ViewModel == null) return;

			this.OneWayBind(
				viewModel: ViewModel,
				vmProperty: vm => vm.TextColor, // VM属性：Color类型
				viewProperty: v => v.PressureValue.Foreground, // View属性：IBrush类型
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
				v => v.PressureValue.FontFamily
			).DisposeWith(disposables);

			// 字体大小（TextFont.FontSize）
			this.Bind(
				ViewModel,
				vm => vm.TextFont.FontSize,
				v => v.PressureValue.FontSize
			).DisposeWith(disposables);

			// 字体粗细（TextFont.FontWeight）
			this.Bind(
				ViewModel,
				vm => vm.TextFont.FontWeight,
				v => v.PressureValue.FontWeight
			).DisposeWith(disposables);

			// 气压值（PressureValue）
			this.Bind(
				ViewModel,
				vm => vm.PressureValue,
				v => v.PressureValue.Text
			).DisposeWith(disposables);

			Observable.FromEventPattern<PointerPressedEventArgs>(
							addHandler: h => PressureValue.PointerPressed += h,
							removeHandler: h => PressureValue.PointerPressed -= h
						)
						.Select(args =>
						{
							// 转换参数：获取 TextBlock 左上角的屏幕坐标（保持原逻辑）
							PixelPoint screenPixel = PressureValue.PointToScreen(new Point(0, 0));
							return new Point(screenPixel.X, screenPixel.Y);
						})
						.InvokeCommand(ViewModel, vm => vm.PointerPressedCommand) // 事件流绑定到命令
						.DisposeWith(disposables); // 生命周期管理，避免内存泄漏
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
			PressureValue.Opacity = opacity;
		}
		else
		{
			Dispatcher.UIThread.Post(() =>
			{
				PressureValue.Opacity = opacity;
			});
		}
	}
}