using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Objects;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

using AControls = Avalonia.Controls;
using APrimitives = Avalonia.Controls.Primitives;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.MapOprtCellParamCfgView
{
    internal static class DisplayListOprtCellCfgJson
    {
        internal static byte[] ToBytes<T>(T vm) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vm));

        internal static T? FromBytes<T>(byte[] data)
        {
            if (data == null || data.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
        }
    }

    #region ViewModels

    internal sealed class CommonInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private ObservableCollection<DisplayListColumnInfo> _columns = new();
        public ObservableCollection<DisplayListColumnInfo> Columns { get => _columns; set => this.RaiseAndSetIfChanged(ref _columns, value); }

        private bool _enableSelectAll = true;
        public bool EnableSelectAll { get => _enableSelectAll; set => this.RaiseAndSetIfChanged(ref _enableSelectAll, value); }

        private string _sortField = string.Empty;
        public string SortField { get => _sortField; set => this.RaiseAndSetIfChanged(ref _sortField, value ?? string.Empty); }

        private DisplayListSortDirection _sortDirection = DisplayListSortDirection.Asc;
        public DisplayListSortDirection SortDirection { get => _sortDirection; set => this.RaiseAndSetIfChanged(ref _sortDirection, value); }

        public void FromBytes(byte[] data)
        {
            var temp = DisplayListOprtCellCfgJson.FromBytes<CommonInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;

            Columns ??= new ObservableCollection<DisplayListColumnInfo>();
            Columns.Clear();
            if (temp.Columns != null)
            {
                foreach (var c in temp.Columns)
                {
                    if (c == null) continue;
                    Columns.Add(new DisplayListColumnInfo { FieldID = c.FieldID, DisplayName = c.DisplayName });
                }
            }

            EnableSelectAll = temp.EnableSelectAll;
            SortField = temp.SortField ?? SortField;
            SortDirection = temp.SortDirection;
        }

        public byte[] ToBytes() => DisplayListOprtCellCfgJson.ToBytes(this);
    }

    #endregion

    #region Views

    internal abstract class DisplayListOprtCellParamViewBase : AControls.UserControl
    {
        protected static AControls.StackPanel CreateRoot() => new AControls.StackPanel { Margin = new Thickness(20), Spacing = 12 };

        protected static AControls.StackPanel CreateRow(string label, AControls.Control editor)
        {
            var row = new AControls.StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center };
            row.Children.Add(new AControls.TextBlock { Text = label, Width = 140, VerticalAlignment = VerticalAlignment.Center });
            row.Children.Add(editor);
            return row;
        }

        protected static AControls.TextBox CreateTextBox(string bindingPath, double width = 220)
        {
            var tb = new AControls.TextBox { Width = width };
            tb.Bind(AControls.TextBox.TextProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return tb;
        }

        protected static AControls.CheckBox CreateCheckBox(string content, string bindingPath)
        {
            var cb = new AControls.CheckBox { Content = content };
            cb.Bind(APrimitives.ToggleButton.IsCheckedProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return cb;
        }

        protected static AControls.ComboBox CreateEnumComboBox<TEnum>(string bindingPath, double width = 200) where TEnum : struct, Enum
        {
            var cb = new AControls.ComboBox { Width = width, ItemsSource = Enum.GetValues(typeof(TEnum)) };
            cb.Bind(APrimitives.SelectingItemsControl.SelectedItemProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return cb;
        }
    }

    internal sealed class CommonInfoMapOprtCellParamView : DisplayListOprtCellParamViewBase
    {
        private readonly AControls.TextBlock _columnsInfo;
        private readonly AControls.Button _editColumns;

        public CommonInfoMapOprtCellParamView()
        {
            var root = CreateRoot();

            _columnsInfo = new AControls.TextBlock { Text = "列数量: 0" };
            _editColumns = new AControls.Button { Content = "编辑列配置...", HorizontalAlignment = HorizontalAlignment.Left };
            _editColumns.Click += EditColumns_Click;

            root.Children.Add(_columnsInfo);
            root.Children.Add(_editColumns);

            root.Children.Add(CreateRow("支持全选", CreateCheckBox("启用", nameof(CommonInfoMapOprtCellParamViewModel.EnableSelectAll))));
            root.Children.Add(CreateRow("排序字段", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.SortField), 260)));
            root.Children.Add(CreateRow("排序类型", CreateEnumComboBox<DisplayListSortDirection>(nameof(CommonInfoMapOprtCellParamViewModel.SortDirection), 200)));

            Content = root;

            DataContextChanged += (_, __) => UpdateColumnsInfoText();
        }

        private void UpdateColumnsInfoText()
        {
            try
            {
                var vm = DataContext as CommonInfoMapOprtCellParamViewModel;
                var count = vm?.Columns?.Count ?? 0;
                _columnsInfo.Text = $"列数量: {count}";
            }
            catch
            {
            }
        }

        private async void EditColumns_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var vm = DataContext as CommonInfoMapOprtCellParamViewModel;
            if (vm == null) return;

            try
            {
                var editCopy = new System.Collections.Generic.List<DisplayListColumnInfo>();
                if (vm.Columns != null)
                {
                    foreach (var c in vm.Columns)
                    {
                        if (c == null) continue;
                        editCopy.Add(new DisplayListColumnInfo { FieldID = c.FieldID, DisplayName = c.DisplayName });
                    }
                }

                var win = new GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.UI.DisplayListColumnsWindow
                {
                    Columns = editCopy,
                };

                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel is Window parent)
                    await win.ShowDialog(parent);
                else
                {
                    win.Show();
                    return;
                }

                if (win.DialogResult)
                {
                    vm.Columns ??= new ObservableCollection<DisplayListColumnInfo>();
                    vm.Columns.Clear();
                    foreach (var c in win.Columns)
                    {
                        if (c == null) continue;
                        vm.Columns.Add(new DisplayListColumnInfo { FieldID = c.FieldID, DisplayName = c.DisplayName });
                    }
                    UpdateColumnsInfoText();
                }
            }
            catch
            {
            }
        }
    }

    #endregion

    #region CfgView wrappers

    internal sealed class CommonInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly CommonInfoMapOprtCellParamView _view;
        private readonly CommonInfoMapOprtCellParamViewModel _vm;

        public CommonInfoMapOprtCellParamCfgView()
        {
            _vm = new CommonInfoMapOprtCellParamViewModel();
            _view = new CommonInfoMapOprtCellParamView { DataContext = _vm };
        }

        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();
        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    #endregion
}
