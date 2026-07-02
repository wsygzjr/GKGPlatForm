using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Griffins.Map.CtlMapCell.Generic.TextButton.ViewModel;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Griffins.Map.CtlMapCell.Generic.TextButton.View;

public partial class TextButtonView : ReactiveUserControl<TextButtonViewModel>
{
    public TextButtonView()
    {
        InitializeComponent();
		this.WhenActivated(disposables =>
		{
			if (ViewModel == null) return;

			this.Bind(ViewModel,
				vm => vm.TextValue,
				view => view.TextValueTextBox.Text)
				.DisposeWith(disposables);

			this.Bind(ViewModel,
				vm => vm.ColumnSpacing,
				view => view.MainGrid.ColumnSpacing)
				.DisposeWith(disposables);

			this.BindCommand<TextButtonView, TextButtonViewModel, ReactiveCommand<Point, Unit>, Avalonia.Controls.Button, Point>(
				viewModel: ViewModel,
				propertyName: vm => vm.ButtonClickCommand,
				controlName: v => v.ActionButton,
				withParameter: Observable.FromEventPattern<RoutedEventArgs>(
						addHandler: h => ActionButton.Click += h, // 泛型事件，类型匹配
						removeHandler: h => ActionButton.Click -= h
					).Select(_ =>
					{
						PixelPoint screenPixel = ActionButton.PointToScreen(new Point(0, 0));
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
			ActionButton.Opacity = opacity;
		}
		else
		{
			Dispatcher.UIThread.Post(() =>
			{
				ActionButton.Opacity = opacity;
			});
		}
	}
}