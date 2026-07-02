using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.RecipeCfgPage.MaterialBoxConfig.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.RecipeCfgPage.MaterialBoxConfig.Views
{
    public partial class MaterialBoxConfigCompUIView : UserControl
    {
        public MaterialBoxConfigCompUIView()
        {
            InitializeComponent();
            RegisterMoveButtonHandlers("BtnLoadUpperMoveUp", LoadUpperMoveUp_OnPointerPressed, LoadUpperMove_OnPointerReleased);
            RegisterMoveButtonHandlers("BtnLoadUpperMoveDown", LoadUpperMoveDown_OnPointerPressed, LoadUpperMove_OnPointerReleased);
            RegisterMoveButtonHandlers("BtnLoadLowerMoveUp", LoadLowerMoveUp_OnPointerPressed, LoadLowerMove_OnPointerReleased);
            RegisterMoveButtonHandlers("BtnLoadLowerMoveDown", LoadLowerMoveDown_OnPointerPressed, LoadLowerMove_OnPointerReleased);
            RegisterMoveButtonHandlers("BtnUnloadUpperMoveUp", UnloadUpperMoveUp_OnPointerPressed, UnloadUpperMove_OnPointerReleased);
            RegisterMoveButtonHandlers("BtnUnloadUpperMoveDown", UnloadUpperMoveDown_OnPointerPressed, UnloadUpperMove_OnPointerReleased);
            RegisterMoveButtonHandlers("BtnUnloadLowerMoveUp", UnloadLowerMoveUp_OnPointerPressed, UnloadLowerMove_OnPointerReleased);
            RegisterMoveButtonHandlers("BtnUnloadLowerMoveDown", UnloadLowerMoveDown_OnPointerPressed, UnloadLowerMove_OnPointerReleased);
        }

        private void RegisterMoveButtonHandlers(string buttonName, Delegate onPressed, Delegate onReleased)
        {
            var button = this.FindControl<Button>(buttonName);
            ButtonAddPressedHandler(button, onPressed);
            ButtonAddReleasedHandler(button, onReleased);
        }

        private void ButtonAddPressedHandler(Button button, Delegate @delegate)
        {
            button.AddHandler(
                InputElement.PointerPressedEvent,
                @delegate,
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble,
                handledEventsToo: true);
        }

        private void ButtonAddReleasedHandler(Button button, Delegate @delegate)
        {
            button.AddHandler(
                InputElement.PointerReleasedEvent,
                @delegate,
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble,
                handledEventsToo: true);
        }

        private void LoadUpperMoveUp_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is MaterialBoxConfigCompUIViewModel viewModel)
            {
                viewModel.LoadUpperMoveUpCommand.Execute().Subscribe();
            }
        }

        private void LoadUpperMoveDown_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is MaterialBoxConfigCompUIViewModel viewModel)
            {
                viewModel.LoadUpperMoveDownCommand.Execute().Subscribe();
            }
        }

        private void LoadLowerMoveUp_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is MaterialBoxConfigCompUIViewModel viewModel)
            {
                viewModel.LoadLowerMoveUpCommand.Execute().Subscribe();
            }
        }

        private void LoadLowerMoveDown_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is MaterialBoxConfigCompUIViewModel viewModel)
            {
                viewModel.LoadLowerMoveDownCommand.Execute().Subscribe();
            }
        }

        private void UnloadUpperMoveUp_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is MaterialBoxConfigCompUIViewModel viewModel)
            {
                viewModel.UnloadUpperMoveUpCommand.Execute().Subscribe();
            }
        }

        private void UnloadUpperMoveDown_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is MaterialBoxConfigCompUIViewModel viewModel)
            {
                viewModel.UnloadUpperMoveDownCommand.Execute().Subscribe();
            }
        }

        private void UnloadLowerMoveUp_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is MaterialBoxConfigCompUIViewModel viewModel)
            {
                viewModel.UnloadLowerMoveUpCommand.Execute().Subscribe();
            }
        }

        private void UnloadLowerMoveDown_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is MaterialBoxConfigCompUIViewModel viewModel)
            {
                viewModel.UnloadLowerMoveDownCommand.Execute().Subscribe();
            }
        }

        private void LoadUpperMove_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            StopLoadUpperMove();
        }

        private void LoadLowerMove_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            StopLoadLowerMove();
        }

        private void UnloadUpperMove_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            StopUnloadUpperMove();
        }

        private void UnloadLowerMove_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            StopUnloadLowerMove();
        }

        private void StopLoadUpperMove()
        {
            if (DataContext is MaterialBoxConfigCompUIViewModel viewModel)
            {
                viewModel.LoadUpperAxisStopCommand.Execute().Subscribe();
            }
        }

        private void StopLoadLowerMove()
        {
            if (DataContext is MaterialBoxConfigCompUIViewModel viewModel)
            {
                viewModel.LoadLowerAxisStopCommand.Execute().Subscribe();
            }
        }

        private void StopUnloadUpperMove()
        {
            if (DataContext is MaterialBoxConfigCompUIViewModel viewModel)
            {
                viewModel.UnloadUpperAxisStopCommand.Execute().Subscribe();
            }
        }

        private void StopUnloadLowerMove()
        {
            if (DataContext is MaterialBoxConfigCompUIViewModel viewModel)
            {
                viewModel.UnloadLowerAxisStopCommand.Execute().Subscribe();
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void OnOpenDisableSlotDialog(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not MaterialBoxConfigCompUIViewModel vm)
                return;

            var layerTag = (sender as Control)?.Tag?.ToString();
            var layer = layerTag switch
            {
                "LoadLower" => vm.LoadLowerLayerBox,
                "UnloadUpper" => vm.UnloadUpperLayerBox,
                "UnloadLower" => vm.UnloadLowerLayerBox,
                _ => vm.LoadUpperLayerBox,
            };

            var slotCount = ParseNonNegativeInt(layer.SlotCountViewModel.Text);
            var initialDisabledSlots = ParseDisabledSlots(layer.DisabledSlotIndexes, layer.DisabledSlotCountViewModel.Text, slotCount);

            var rows = Enumerable.Range(1, slotCount)
                .Select(i => new DisableSlotRowState
                {
                    SlotNo = i,
                    IsDisabled = initialDisabledSlots.Contains(i)
                })
                .ToList();

            var rowsPanel = new StackPanel { Spacing = 0 };
            BuildRows(rows, rowsPanel, layer);

            var disableBatchButton = CreateDangerButton("禁用");
            disableBatchButton.Click += (_, _) =>
            {
                foreach (var row in rows.Where(x => x.IsChecked && !x.IsDisabled))
                {
                    row.IsChecked = false;
                    row.IsDisabled = true;
                    RefreshRowActionState(row);
                    if (row.CheckBox != null)
                        row.CheckBox.IsChecked = false;
                }

                UpdateDisabledState(layer, rows);
            };

            var enableBatchButton = CreatePrimaryOutlineButton("启用");
            enableBatchButton.Click += (_, _) =>
            {
                foreach (var row in rows.Where(x => x.IsChecked && x.IsDisabled))
                {
                    row.IsChecked = false;
                    row.IsDisabled = false;
                    RefreshRowActionState(row);
                    if (row.CheckBox != null)
                        row.CheckBox.IsChecked = false;
                }

                UpdateDisabledState(layer, rows);
            };

            var actionsPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                Spacing = 10,
                Margin = new Avalonia.Thickness(0, 0, 0, 10)
            };
            actionsPanel.Children.Add(disableBatchButton);
            actionsPanel.Children.Add(enableBatchButton);

            var body = new Grid
            {
                RowDefinitions = new RowDefinitions("Auto,*")
            };
            body.Children.Add(actionsPanel);
            Grid.SetRow(actionsPanel, 0);

            var scroll = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = rowsPanel
            };
            body.Children.Add(scroll);
            Grid.SetRow(scroll, 1);

            var owner = this.GetVisualRoot() as Window;
            var dialog = new Window
            {
                Width = 620,
                Height = 620,
                MinWidth = 560,
                MinHeight = 420,
                Title = "禁用槽数配置",
                Content = new Border
                {
                    Padding = new Avalonia.Thickness(14),
                    Background = Brushes.White,
                    Child = body
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (owner != null)
            {
                await dialog.ShowDialog(owner);
                return;
            }

            dialog.Show();
        }

        private static void BuildRows(
            List<DisableSlotRowState> rows,
            StackPanel rowsPanel,
            MaterialBoxLayerViewModel layer)
        {
            var syncingAll = false;

            var header = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("90,*,120,220"),
                Height = 36,
                Background = Brush.Parse("#F1F4F8")
            };

            var selectAllCheckBox = new CheckBox
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            selectAllCheckBox.Checked += (_, _) =>
            {
                if (syncingAll)
                    return;

                foreach (var row in rows)
                {
                    row.IsChecked = true;
                    if (row.CheckBox != null)
                        row.CheckBox.IsChecked = true;
                }
            };
            selectAllCheckBox.Unchecked += (_, _) =>
            {
                if (syncingAll)
                    return;

                foreach (var row in rows)
                {
                    row.IsChecked = false;
                    if (row.CheckBox != null)
                        row.CheckBox.IsChecked = false;
                }
            };
            header.Children.Add(selectAllCheckBox);

            var slotHeader = new TextBlock
            {
                Text = "料槽位置",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            header.Children.Add(slotHeader);
            Grid.SetColumn(slotHeader, 1);

            var statusHeader = new TextBlock
            {
                Text = "启用状态",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            header.Children.Add(statusHeader);
            Grid.SetColumn(statusHeader, 2);

            var actionHeader = new TextBlock
            {
                Text = "操作",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            header.Children.Add(actionHeader);
            Grid.SetColumn(actionHeader, 3);

            rowsPanel.Children.Add(header);

            foreach (var row in rows)
            {
                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitions("90,*,120,220"),
                    Height = 38,
                    Background = Brushes.White
                };

                var checkBox = new CheckBox
                {
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    IsChecked = row.IsChecked
                };
                checkBox.Checked += (_, _) =>
                {
                    row.IsChecked = true;
                    syncingAll = true;
                    selectAllCheckBox.IsChecked = rows.Count > 0 && rows.All(x => x.IsChecked);
                    syncingAll = false;
                };
                checkBox.Unchecked += (_, _) =>
                {
                    row.IsChecked = false;
                    syncingAll = true;
                    selectAllCheckBox.IsChecked = rows.Count > 0 && rows.All(x => x.IsChecked);
                    syncingAll = false;
                };
                row.CheckBox = checkBox;
                grid.Children.Add(checkBox);

                var slotName = new TextBlock
                {
                    Text = $"槽{row.SlotNo}",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };
                grid.Children.Add(slotName);
                Grid.SetColumn(slotName, 1);

                var statusText = new TextBlock
                {
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };
                row.StatusText = statusText;
                grid.Children.Add(statusText);
                Grid.SetColumn(statusText, 2);

                var actions = new StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    Spacing = 8,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };

                var disableButton = CreateDangerButton("禁用");
                disableButton.Click += (_, _) =>
                {
                    row.IsDisabled = true;
                    RefreshRowActionState(row);
                    UpdateDisabledState(layer, rows);
                };
                row.DisableButton = disableButton;
                actions.Children.Add(disableButton);

                var enableButton = CreatePrimaryOutlineButton("启用");
                enableButton.Click += (_, _) =>
                {
                    row.IsDisabled = false;
                    RefreshRowActionState(row);
                    UpdateDisabledState(layer, rows);
                };
                row.EnableButton = enableButton;
                actions.Children.Add(enableButton);

                grid.Children.Add(actions);
                Grid.SetColumn(actions, 3);

                rowsPanel.Children.Add(new Border
                {
                    BorderBrush = Brush.Parse("#E8EAEE"),
                    BorderThickness = new Avalonia.Thickness(0, 0, 0, 1),
                    Child = grid
                });

                RefreshRowActionState(row);
            }

            UpdateDisabledState(layer, rows);
            syncingAll = true;
            selectAllCheckBox.IsChecked = rows.Count > 0 && rows.All(x => x.IsChecked);
            syncingAll = false;
        }

        private static Button CreateDangerButton(string text)
        {
            return new Button
            {
                Content = text,
                Width = 88,
                Height = 30,
                Background = Brushes.White,
                Foreground = Brush.Parse("#FF3D3D"),
                BorderBrush = Brush.Parse("#FF3D3D"),
                BorderThickness = new Avalonia.Thickness(1)
            };
        }

        private static Button CreatePrimaryOutlineButton(string text)
        {
            return new Button
            {
                Content = text,
                Width = 88,
                Height = 30,
                Background = Brushes.White,
                Foreground = Brush.Parse("#1C5ED6"),
                BorderBrush = Brush.Parse("#1C5ED6"),
                BorderThickness = new Avalonia.Thickness(1)
            };
        }

        private static void RefreshRowActionState(DisableSlotRowState row)
        {
            if (row.DisableButton != null)
                row.DisableButton.IsEnabled = !row.IsDisabled;

            if (row.EnableButton != null)
                row.EnableButton.IsEnabled = row.IsDisabled;

            if (row.StatusText != null)
            {
                row.StatusText.Text = row.IsDisabled ? "禁用" : "启用";
                row.StatusText.Foreground = row.IsDisabled
                    ? Brush.Parse("#FF3D3D")
                    : Brush.Parse("#1C5ED6");
            }
        }

        private static void UpdateDisabledState(MaterialBoxLayerViewModel layer, IEnumerable<DisableSlotRowState> rows)
        {
            var disabledSlots = rows
                .Where(x => x.IsDisabled)
                .Select(x => x.SlotNo)
                .OrderBy(x => x)
                .ToList();

            layer.DisabledSlotCountViewModel.Text = disabledSlots.Count.ToString();
            layer.DisabledSlotIndexes = string.Join(",", disabledSlots);
        }

        private static int ParseNonNegativeInt(string? text)
        {
            if (!int.TryParse(text?.Trim(), out var value))
                return 0;

            return Math.Max(0, value);
        }

        private static HashSet<int> ParseDisabledSlots(string? indexesText, string? countText, int slotCount)
        {
            var result = new HashSet<int>();
            if (slotCount <= 0)
                return result;

            var tokens = (indexesText ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var token in tokens)
            {
                if (int.TryParse(token, out var slotNo) && slotNo >= 1 && slotNo <= slotCount)
                    result.Add(slotNo);
            }

            if (result.Count > 0)
                return result;

            var count = ParseNonNegativeInt(countText);
            for (var i = 1; i <= Math.Min(count, slotCount); i++)
                result.Add(i);

            return result;
        }

        private sealed class DisableSlotRowState
        {
            public int SlotNo { get; set; }
            public bool IsChecked { get; set; }
            public bool IsDisabled { get; set; }
            public CheckBox? CheckBox { get; set; }
            public Button? DisableButton { get; set; }
            public Button? EnableButton { get; set; }
            public TextBlock? StatusText { get; set; }
        }
    }
}
