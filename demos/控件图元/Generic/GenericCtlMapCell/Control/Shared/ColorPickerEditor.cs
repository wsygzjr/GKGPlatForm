using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

using AControls = Avalonia.Controls;

namespace GKG.Map.MapCell.Generic.Control.Shared
{
    internal sealed class ColorPickerEditor : AControls.UserControl
    {
        private readonly AControls.TextBox _textBox;
        private readonly Border _colorPreview;
        private readonly CompositeDisposable _disposables = new();

        public static readonly StyledProperty<string> ColorStringProperty =
            AvaloniaProperty.Register<ColorPickerEditor, string>(nameof(ColorString), "#FF000000");

        public string ColorString
        {
            get => GetValue(ColorStringProperty);
            set => SetValue(ColorStringProperty, value);
        }

        public ColorPickerEditor()
        {
            var panel = new AControls.StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 };

            _colorPreview = new Border
            {
                Width = 24,
                Height = 24,
                CornerRadius = new CornerRadius(4),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
                Background = new SolidColorBrush(Colors.Black),
            };
            _colorPreview.PointerPressed += OnColorPreviewClick;

            _textBox = new AControls.TextBox { Width = 120, VerticalAlignment = VerticalAlignment.Center };
            _textBox.Bind(AControls.TextBox.TextProperty, new Binding(nameof(ColorString)) { Source = this, Mode = BindingMode.TwoWay });

            panel.Children.Add(_colorPreview);
            panel.Children.Add(_textBox);
            Content = panel;

            this.GetObservable(ColorStringProperty).Subscribe(UpdateColorPreview).DisposeWith(_disposables);
        }

        private void UpdateColorPreview(string colorStr)
        {
            try
            {
                if (Color.TryParse(colorStr, out var color))
                    _colorPreview.Background = new SolidColorBrush(color);
            }
            catch
            {
            }
        }

        private async void OnColorPreviewClick(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            try
            {
                var currentColor = Colors.Black;
                if (Color.TryParse(ColorString, out var parsed))
                    currentColor = parsed;

                var colorPicker = new AControls.ColorPicker { Color = currentColor, Width = 300, Height = 400 };
                var dialog = new Window
                {
                    Title = "选择颜色",
                    Width = 340,
                    Height = 480,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = new AControls.StackPanel
                    {
                        Margin = new Thickness(10),
                        Spacing = 10,
                        Children =
                        {
                            colorPicker,
                            new AControls.StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                HorizontalAlignment = HorizontalAlignment.Right,
                                Spacing = 10,
                                Children =
                                {
                                    new AControls.Button { Content = "确定", Tag = "OK", Width = 80 },
                                    new AControls.Button { Content = "取消", Tag = "Cancel", Width = 80 }
                                }
                            }
                        }
                    }
                };

                Color? selectedColor = null;
                var buttons = ((AControls.StackPanel)((AControls.StackPanel)dialog.Content).Children[1]).Children;
                ((AControls.Button)buttons[0]).Click += (s, args) => { selectedColor = colorPicker.Color; dialog.Close(); };
                ((AControls.Button)buttons[1]).Click += (s, args) => dialog.Close();

                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel is Window parentWindow)
                    await dialog.ShowDialog(parentWindow);
                else
                {
                    dialog.Show();
                    return;
                }

                if (selectedColor.HasValue)
                    ColorString = $"#{selectedColor.Value.A:X2}{selectedColor.Value.R:X2}{selectedColor.Value.G:X2}{selectedColor.Value.B:X2}";
            }
            catch
            {
            }
        }

        protected override void OnUnloaded(Avalonia.Interactivity.RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            _disposables.Dispose();
        }
    }
}
