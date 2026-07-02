using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using GKG.Map.MapCell.Generic.Control.MapCell_ComboBox.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ComboBox.Views
{
    public partial class ComboBoxView : ReactiveUserControl<ComboBoxViewModel>
    {
        public ComboBoxView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                if (ViewModel == null) return;

                var suppressSync = false;

                // 强制同步下拉宽度与主控件等宽
                void SyncDropDownWidth()
                {
                    var bw = RootBorder.Bounds.Width;
                    DropDownBorder.Width = bw > 0 ? bw : double.NaN;
                }

                void OnLayoutUpdated(object sender, EventArgs e)
                {
                    SyncDropDownWidth();
                    if (DropDownPopup.IsOpen)
                    {
                        // 强制重绘 Popup 位置
                        DropDownPopup.PlacementTarget = null;
                        DropDownPopup.PlacementTarget = RootBorder;
                    }
                }

                RootBorder.LayoutUpdated += OnLayoutUpdated;
                Disposable.Create(() => RootBorder.LayoutUpdated -= OnLayoutUpdated).DisposeWith(disposables);

                // IsDropDownOpen 状态通知
                ViewModel.WhenAnyValue(vm => vm.IsDropDownOpen)
                    .DistinctUntilChanged()
                    .Subscribe(isOpen =>
                    {
                        if (isOpen) ViewModel?.NotifyDropDownOpened();
                        else ViewModel?.NotifyDropDownClosed();
                    })
                    .DisposeWith(disposables);

                // UI 下拉列表选中 -> VM 状态更新
                void OnListSelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                    if (ViewModel == null || suppressSync) return;

                    // 拦截数据源刷新导致的假事件
                    // 如果 AddedItems 是空的，说明这是 Avalonia 底层在重置列表，而不是用户的真实点击。直接忽略！
                    if (e.AddedItems == null || e.AddedItems.Count == 0) return;

                    suppressSync = true;
                    try
                    {
                        ViewModel.SelectedIndex = ItemsListBox.SelectedIndex;
                        ViewModel.SelectedItem = ItemsListBox.SelectedItem as string ?? string.Empty;
                        InputTextBox.Text = ViewModel.SelectedItem;
                    }
                    finally
                    {
                        suppressSync = false;
                    }

                    ViewModel?.NotifySelectionChanged();
                    ViewModel.IsDropDownOpen = false; // 选择后自动关闭
                }

                ItemsListBox.SelectionChanged += OnListSelectionChanged;
                Disposable.Create(() => ItemsListBox.SelectionChanged -= OnListSelectionChanged).DisposeWith(disposables);

                // VM 状态更新 -> UI 同步
                void SyncFromVmToUi()
                {
                    if (suppressSync) return;

                    suppressSync = true;
                    try
                    {
                        if (ViewModel.SelectedIndex >= 0 && ViewModel.SelectedIndex < ViewModel.Items.Count)
                        {
                            ItemsListBox.SelectedIndex = ViewModel.SelectedIndex;
                            InputTextBox.Text = ViewModel.Items[ViewModel.SelectedIndex];
                        }
                        else
                        {
                            ItemsListBox.SelectedIndex = -1;
                            InputTextBox.Text = ViewModel.SelectedItem ?? string.Empty;
                        }
                    }
                    finally
                    {
                        suppressSync = false;
                    }
                }

                ViewModel.WhenAnyValue(vm => vm.SelectedIndex).Subscribe(_ => SyncFromVmToUi()).DisposeWith(disposables);
                ViewModel.WhenAnyValue(vm => vm.SelectedItem).Subscribe(_ => SyncFromVmToUi()).DisposeWith(disposables);
                ViewModel.WhenAnyValue(vm => vm.Items).Subscribe(_ => SyncFromVmToUi()).DisposeWith(disposables);

                // 输入框手动编辑同步回 VM（支持 Editable 时）
                InputTextBox.GetObservable(TextBox.TextProperty)
                    .Skip(1)
                    .Subscribe(text =>
                    {
                        if (ViewModel == null || suppressSync || !ViewModel.IsEditable) return;

                        suppressSync = true;
                        try
                        {
                            ViewModel.SelectedItem = text ?? string.Empty;
                            ViewModel.SelectedIndex = ViewModel.Items?.IndexOf(ViewModel.SelectedItem) ?? -1;
                            ItemsListBox.SelectedIndex = ViewModel.SelectedIndex;
                        }
                        finally
                        {
                            suppressSync = false;
                        }
                    })
                    .DisposeWith(disposables);

                // 点击图元时获取焦点方便输入
                void OnPointerPressed(object sender, PointerPressedEventArgs e)
                {
                    if (e.GetCurrentPoint(RootBorder).Properties.IsLeftButtonPressed)
                        InputTextBox.Focus();
                }

                RootBorder.PointerPressed += OnPointerPressed;
                Disposable.Create(() => RootBorder.PointerPressed -= OnPointerPressed).DisposeWith(disposables);
            });
        }
    }
}