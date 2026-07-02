using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Objects;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Views
{
    public partial class DisplayListView : ReactiveUserControl<DisplayListViewModel>
    {
        public DisplayListView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                if (ViewModel == null)
                    return;
                ViewModel.EnsureTestRows();

                void SyncHeaderLayout()
                {
                    try
                    {
                        // 将表头横向偏移与内容滚动条对齐，保证列标题与内容列对齐
                        var offsetX = BodyScrollViewer?.Offset.X ?? 0;
                        HeaderGrid.RenderTransform = new TranslateTransform(-offsetX, 0);

                        // 当出现纵向滚动条时，为表头右侧预留滚动条宽度，避免标题与内容错位
                        var hasVScroll = BodyScrollViewer != null &&
                                         BodyScrollViewer.Extent.Height > BodyScrollViewer.Viewport.Height + 0.5;
                        HeaderBorder.Padding = hasVScroll ? new Thickness(0, 0, 16, 0) : new Thickness(0);
                    }
                    catch
                    {
                    }
                }

                void RebuildColumns()
                {
                    // 按 CommonInfo.Columns 动态重建 HeaderGrid
                    HeaderGrid.ColumnDefinitions.Clear();
                    HeaderGrid.Children.Clear();

                    var cols = ViewModel.CommonInfo.Columns;
                    var hasSelect = ViewModel.CommonInfo.EnableSelectAll;
                    if (hasSelect)
                        HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(0, GridUnitType.Auto));

                    var headers = (cols == null || cols.Count == 0)
                        ? new List<(string FieldID, string DisplayName)>
                        {
                            ("Name", "姓名"),
                            ("Age", "年龄"),
                            ("Gender", "性别"),
                        }
                        : cols.Select(c => (c.FieldID, c.DisplayName)).ToList();

                    for (var i = 0; i < headers.Count; i++)
                        HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));

                    if (hasSelect)
                    {
                        var placeHolder = new Avalonia.Controls.CheckBox
                        {
                            Opacity = 0,
                            IsHitTestVisible = false,
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Margin = new Thickness(6, 4, 6, 4),
                        };
                        Grid.SetColumn(placeHolder, 0);
                        HeaderGrid.Children.Add(placeHolder);
                    }

                    for (var i = 0; i < headers.Count; i++)
                    {
                        var c = headers[i];
                        var tb = new TextBlock
                        {
                            Text = string.IsNullOrEmpty(c.DisplayName) ? c.FieldID : c.DisplayName,
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Margin = new Thickness(8, 6, 8, 6),
                            Foreground = Brushes.White,
                            FontFamily = FontFamily.Default,
                            FontSize = 14,
                            FontStyle = FontStyle.Normal,
                            FontWeight = FontWeight.Normal,
                        };
                        Grid.SetColumn(tb, hasSelect ? i + 1 : i);
                        HeaderGrid.Children.Add(tb);
                    }

                    SyncHeaderLayout();
                }

                void RebuildRows()
                {
                    // 按 ViewModel.Rows 和列 key 列表动态重建 RowsPanel
                    RowsPanel.Children.Clear();

                    var cols = ViewModel.CommonInfo.Columns;
                    var keys = (cols == null || cols.Count == 0)
                        ? new List<string> { "Name", "Age", "Gender" }
                        : cols.Select(c => string.IsNullOrWhiteSpace(c.FieldID) ? (c.DisplayName ?? string.Empty) : c.FieldID)
                              .Where(s => !string.IsNullOrWhiteSpace(s))
                              .ToList();

                    if (keys.Count == 0)
                        keys.AddRange(new[] { "Name", "Age", "Gender" });

                    var hasSelect = ViewModel.CommonInfo.EnableSelectAll;
                    if (hasSelect)
                    {
                        // 第一行：全选行
                        var selectAllRow = BuildSelectAllRow(keys);
                        RowsPanel.Children.Add(selectAllRow);
                    }

                    foreach (var row in ViewModel.Rows)
                        RowsPanel.Children.Add(BuildDataRow(row, keys, hasSelect));

                    UpdateSelectAllState();
                }

                Avalonia.Controls.Control BuildSelectAllRow(IReadOnlyList<string> keys)
                {
                    // 全选行：勾选后将所有数据行的 IsSelected 设为统一值
                    var grid = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitions("Auto," + string.Join(",", Enumerable.Repeat("*", keys.Count))),
                        Background = Brushes.Transparent,
                    };

                    var cb = new Avalonia.Controls.CheckBox
                    {
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Margin = new Thickness(6, 4, 6, 4),
                        IsThreeState = false,
                    };

                    cb.Click += (_, __) =>
                    {
                        var total = ViewModel.Rows.Count;
                        var selected = ViewModel.Rows.Count(r => r.IsSelected);
                        var newVal = total > 0 && selected != total;
                        foreach (var r in ViewModel.Rows)
                            r.IsSelected = newVal;
                        UpdateSelectAllState();
                    };

                    Grid.SetColumn(cb, 0);
                    grid.Children.Add(cb);

                    var tb = new TextBlock
                    {
                        Text = "全选",
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                        Margin = new Thickness(8, 6, 8, 6),
                        Foreground = Brushes.White,
                    };
                    Grid.SetColumn(tb, 1);
                    Grid.SetColumnSpan(tb, Math.Max(1, keys.Count));
                    grid.Children.Add(tb);

                    return grid;
                }

                Avalonia.Controls.Control BuildDataRow(DisplayListTestRow row, IReadOnlyList<string> keys, bool hasSelect)
                {
                    // 普通数据行：可选增加选择列（CheckBox），其余按 key 显示文本
                    var grid = new Grid
                    {
                        ColumnDefinitions = hasSelect
                            ? new ColumnDefinitions("Auto," + string.Join(",", Enumerable.Repeat("*", keys.Count)))
                            : new ColumnDefinitions(string.Join(",", Enumerable.Repeat("*", keys.Count))),
                        Background = Brushes.Transparent,
                    };

                    grid.Bind(BackgroundProperty, new Binding
                    {
                        Source = row,
                        Path = nameof(DisplayListTestRow.IsHighlighted),
                        Mode = BindingMode.OneWay,
                        Converter = new FuncValueConverter<bool, IBrush>(v => v
                            ? new SolidColorBrush(Color.Parse("#553399FF"))
                            : Brushes.Transparent),
                    });

                    grid.PointerPressed += (_, e) =>
                    {
                        if (e?.GetCurrentPoint(grid).Properties.IsLeftButtonPressed != true)
                            return;
                        if (e.Source is Avalonia.Controls.CheckBox)
                            return;
                        foreach (var r in ViewModel.Rows)
                            r.IsCurrent = false;
                        row.IsCurrent = true;
                        e.Handled = true;
                    };

                    var startCol = 0;
                    if (hasSelect)
                    {
                        var cb = new Avalonia.Controls.CheckBox
                        {
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Margin = new Thickness(6, 4, 6, 4),
                        };

                        cb.Bind(ToggleButton.IsCheckedProperty, new Binding
                        {
                            Source = row,
                            Path = nameof(DisplayListTestRow.IsSelected),
                            Mode = BindingMode.TwoWay,
                        });

                        cb.Checked += (_, __) =>
                        {
                            foreach (var r in ViewModel.Rows)
                                r.IsCurrent = false;
                            row.IsCurrent = true;
                            UpdateSelectAllState();
                        };
                        cb.Unchecked += (_, __) =>
                        {
                            foreach (var r in ViewModel.Rows)
                                r.IsCurrent = false;
                            row.IsCurrent = true;
                            UpdateSelectAllState();
                        };

                        Grid.SetColumn(cb, 0);
                        grid.Children.Add(cb);
                        startCol = 1;
                    }

                    for (var i = 0; i < keys.Count; i++)
                    {
                        var key = keys[i];
                        var tb = new TextBlock
                        {
                            Text = row.Get(key),
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Margin = new Thickness(8, 6, 8, 6),
                            Foreground = Brushes.White,
                            FontFamily = FontFamily.Default,
                            FontSize = 14,
                            FontStyle = FontStyle.Normal,
                            FontWeight = FontWeight.Normal,
                        };
                        Grid.SetColumn(tb, i + startCol);
                        grid.Children.Add(tb);
                    }

                    return grid;
                }

                void UpdateSelectAllState()
                {
                    // 根据当前选中数量刷新“全选”CheckBox 状态
                    if (!ViewModel.CommonInfo.EnableSelectAll)
                        return;
                    if (RowsPanel.Children.Count == 0)
                        return;

                    var first = RowsPanel.Children[0];
                    if (first is not Grid grid)
                        return;
                    var cb = grid.Children.OfType<Avalonia.Controls.CheckBox>().FirstOrDefault();
                    if (cb == null)
                        return;

                    var total = ViewModel.Rows.Count;
                    var selected = ViewModel.Rows.Count(r => r.IsSelected);
                    cb.IsThreeState = false;
                    cb.IsChecked = total > 0 && selected == total;
                }

                // Columns 变化：重建表头
                ViewModel.WhenAnyValue(vm => vm.CommonInfo.Columns)
                    .Subscribe(_ => RebuildColumns())
                    .DisposeWith(disposables);

                // Columns 变化：可能影响数据结构，重建测试数据并重建行
                ViewModel.WhenAnyValue(vm => vm.CommonInfo.Columns)
                    .Subscribe(_ =>
                    {
                        ViewModel.EnsureTestRows();
                        RebuildRows();
                    })
                    .DisposeWith(disposables);

                // 是否启用“全选”：需要重建表头与行（多一列 CheckBox）
                ViewModel.WhenAnyValue(vm => vm.CommonInfo.EnableSelectAll)
                    .Subscribe(_ =>
                    {
                        RebuildColumns();
                        RebuildRows();
                    })
                    .DisposeWith(disposables);

                // 排序字段/方向变化：先排序再重建行
                ViewModel.WhenAnyValue(vm => vm.CommonInfo.SortField)
                    .Subscribe(_ =>
                    {
                        ViewModel.ApplySort();
                        RebuildRows();
                    })
                    .DisposeWith(disposables);

                ViewModel.WhenAnyValue(vm => vm.CommonInfo.SortDirection)
                    .Subscribe(_ =>
                    {
                        ViewModel.ApplySort();
                        RebuildRows();
                    })
                    .DisposeWith(disposables);

                if (BodyScrollViewer != null)
                {
                    // 监听滚动：同步表头偏移
                    void OnScrollChanged(object sender, ScrollChangedEventArgs e) => SyncHeaderLayout();
                    BodyScrollViewer.ScrollChanged += OnScrollChanged;
                    Disposable.Create(() => BodyScrollViewer.ScrollChanged -= OnScrollChanged)
                        .DisposeWith(disposables);
                }

                RebuildColumns();
                RebuildRows();
                SyncHeaderLayout();

                void OnPointerPressed(object sender, PointerPressedEventArgs e)
                {
                    // 点击获取焦点，便于键盘交互
                    if (e.GetCurrentPoint(RootBorder).Properties.IsLeftButtonPressed)
                        RootBorder.Focus();
                }

                RootBorder.PointerPressed += OnPointerPressed;
                Disposable.Create(() => RootBorder.PointerPressed -= OnPointerPressed)
                    .DisposeWith(disposables);
            });
        }
    }
}
