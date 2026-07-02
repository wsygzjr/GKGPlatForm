using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.PropertyGrid.Controls;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.UI
{
    public partial class DisplayListColumnsWindow : Avalonia.Controls.Window
    {
        private readonly ObservableCollection<DisplayListColumnInfo> _items;
        private bool _dialogResult;

        private PropertyGrid _hostPropertyGrid;

        private Avalonia.Controls.ListBox _columnsListBox;
        private Avalonia.Controls.Button _btnAdd;
        private Avalonia.Controls.Button _btnDelete;
        private Avalonia.Controls.Button _btnMoveUp;
        private Avalonia.Controls.Button _btnMoveDown;
        private Avalonia.Controls.Button _btnOK;
        private Avalonia.Controls.Button _btnCancel;

        public DisplayListColumnsWindow()
        {
            InitializeComponent();

            _columnsListBox = this.FindControl<Avalonia.Controls.ListBox>("ColumnsListBox");
            _btnAdd = this.FindControl<Avalonia.Controls.Button>("BtnAdd");
            _btnDelete = this.FindControl<Avalonia.Controls.Button>("BtnDelete");
            _btnMoveUp = this.FindControl<Avalonia.Controls.Button>("BtnMoveUp");
            _btnMoveDown = this.FindControl<Avalonia.Controls.Button>("BtnMoveDown");
            _btnOK = this.FindControl<Avalonia.Controls.Button>("BtnOK");
            _btnCancel = this.FindControl<Avalonia.Controls.Button>("BtnCancel");

            _items = new ObservableCollection<DisplayListColumnInfo>();
            _columnsListBox.ItemsSource = _items;

            _btnAdd.Click += BtnAdd_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnMoveUp.Click += BtnMoveUp_Click;
            _btnMoveDown.Click += BtnMoveDown_Click;
            _btnOK.Click += BtnOK_Click;
            _btnCancel.Click += BtnCancel_Click;
            _columnsListBox.SelectionChanged += ColumnsListBox_SelectionChanged;

            UpdateButtonStates();
        }

        public PropertyGrid HostPropertyGrid
        {
            get => _hostPropertyGrid;
            set
            {
                _hostPropertyGrid = value;
                ApplyFactoriesFromHostPropertyGrid();
            }
        }

        private void ApplyFactoriesFromHostPropertyGrid()
        {
            if (_hostPropertyGrid == null)
                return;

            try
            {
                var dstFactoriesProp = HostPropertyGrid?.GetType().GetProperty("Factories", BindingFlags.Instance | BindingFlags.Public);
                if (dstFactoriesProp == null)
                    return;

                _ = dstFactoriesProp.GetValue(_hostPropertyGrid);
            }
            catch
            {
            }
        }

        public bool DialogResult => _dialogResult;

        public List<DisplayListColumnInfo> Columns
        {
            get
            {
                var result = new List<DisplayListColumnInfo>();
                foreach (var item in _items)
                {
                    result.Add(new DisplayListColumnInfo
                    {
                        FieldID = item?.FieldID ?? string.Empty,
                        DisplayName = item?.DisplayName ?? string.Empty
                    });
                }
                return result;
            }
            set
            {
                _items.Clear();
                if (value != null)
                {
                    foreach (var c in value)
                    {
                        _items.Add(new DisplayListColumnInfo
                        {
                            FieldID = c?.FieldID ?? string.Empty,
                            DisplayName = c?.DisplayName ?? string.Empty
                        });
                    }
                }

                if (_items.Count > 0)
                    _columnsListBox.SelectedIndex = 0;
                else
                    _columnsListBox.SelectedIndex = -1;

                UpdateButtonStates();
            }
        }

        private void ColumnsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            var hasSelection = _columnsListBox?.SelectedItem != null;
            var idx = _columnsListBox?.SelectedIndex ?? -1;

            if (_btnDelete != null) _btnDelete.IsEnabled = hasSelection;
            if (_btnMoveUp != null) _btnMoveUp.IsEnabled = hasSelection && idx > 0;
            if (_btnMoveDown != null) _btnMoveDown.IsEnabled = hasSelection && idx >= 0 && idx < _items.Count - 1;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var item = new DisplayListColumnInfo
            {
                FieldID = string.Empty,
                DisplayName = string.Empty
            };

            _items.Add(item);
            _columnsListBox.SelectedIndex = _items.Count - 1;
            UpdateButtonStates();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_columnsListBox.SelectedItem is not DisplayListColumnInfo item)
                return;

            var idx = _columnsListBox.SelectedIndex;
            _items.Remove(item);

            if (_items.Count > 0)
                _columnsListBox.SelectedIndex = Math.Min(idx, _items.Count - 1);
            else
                _columnsListBox.SelectedIndex = -1;

            UpdateButtonStates();
        }

        private void BtnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            var idx = _columnsListBox.SelectedIndex;
            if (idx > 0)
            {
                var item = _items[idx];
                _items.RemoveAt(idx);
                _items.Insert(idx - 1, item);
                _columnsListBox.SelectedIndex = idx - 1;
            }
            UpdateButtonStates();
        }

        private void BtnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            var idx = _columnsListBox.SelectedIndex;
            if (idx >= 0 && idx < _items.Count - 1)
            {
                var item = _items[idx];
                _items.RemoveAt(idx);
                _items.Insert(idx + 1, item);
                _columnsListBox.SelectedIndex = idx + 1;
            }
            UpdateButtonStates();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            _dialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _dialogResult = false;
            Close();
        }
    }
}
