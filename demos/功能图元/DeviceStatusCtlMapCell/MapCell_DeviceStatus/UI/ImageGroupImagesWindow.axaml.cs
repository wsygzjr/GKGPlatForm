using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.PropertyGrid.Controls;
using Avalonia.Threading;
using GF_Gereric;
using Griffins.UI2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.UI
{
    public partial class ImageGroupImagesWindow : Avalonia.Controls.Window
    {
        private readonly ObservableCollection<BitmapData> _items;
        private bool _dialogResult;

        private bool _indexOnly;

        private PropertyGrid _hostPropertyGrid;

        private Avalonia.Controls.ListBox _itemsListBox;
        private Avalonia.Controls.Button _btnAdd;
        private Avalonia.Controls.Button _btnReplace;
        private Avalonia.Controls.Button _btnDelete;
        private Avalonia.Controls.Button _btnMoveUp;
        private Avalonia.Controls.Button _btnMoveDown;
        private Avalonia.Controls.Button _btnOK;
        private Avalonia.Controls.Button _btnCancel;

        private Avalonia.Controls.NumericUpDown _numCurrentIndex;
        private Avalonia.Controls.Image _previewImage;

        private readonly object _previewLock = new object();
        private CancellationTokenSource _previewCts;

        public ImageGroupImagesWindow()
        {
            InitializeComponent();

            _itemsListBox = this.FindControl<Avalonia.Controls.ListBox>("ItemsListBox");
            _btnAdd = this.FindControl<Avalonia.Controls.Button>("BtnAdd");
            _btnReplace = this.FindControl<Avalonia.Controls.Button>("BtnReplace");
            _btnDelete = this.FindControl<Avalonia.Controls.Button>("BtnDelete");
            _btnMoveUp = this.FindControl<Avalonia.Controls.Button>("BtnMoveUp");
            _btnMoveDown = this.FindControl<Avalonia.Controls.Button>("BtnMoveDown");
            _btnOK = this.FindControl<Avalonia.Controls.Button>("BtnOK");
            _btnCancel = this.FindControl<Avalonia.Controls.Button>("BtnCancel");

            _numCurrentIndex = this.FindControl<Avalonia.Controls.NumericUpDown>("NumCurrentIndex");
            _previewImage = this.FindControl<Avalonia.Controls.Image>("PreviewImage");

            _items = new ObservableCollection<BitmapData>();
            _itemsListBox.ItemsSource = _items;

            _btnAdd.Click += BtnAdd_Click;
            _btnReplace.Click += BtnReplace_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnMoveUp.Click += BtnMoveUp_Click;
            _btnMoveDown.Click += BtnMoveDown_Click;
            _btnOK.Click += BtnOK_Click;
            _btnCancel.Click += BtnCancel_Click;
            _itemsListBox.SelectionChanged += ItemsListBox_SelectionChanged;

            if (_numCurrentIndex != null)
            {
                _numCurrentIndex.ValueChanged += NumCurrentIndex_ValueChanged;
            }

            UpdateButtonStates();
        }

        public bool IndexOnly
        {
            get => _indexOnly;
            set
            {
                _indexOnly = value;
                ApplyIndexOnlyMode();
            }
        }

        private void ApplyIndexOnlyMode()
        {
            if (_btnAdd != null) _btnAdd.IsEnabled = !_indexOnly;
            if (_btnReplace != null) _btnReplace.IsEnabled = !_indexOnly && (_itemsListBox?.SelectedItem != null);
            if (_btnDelete != null) _btnDelete.IsEnabled = !_indexOnly && (_itemsListBox?.SelectedItem != null);
            if (_btnMoveUp != null) _btnMoveUp.IsEnabled = !_indexOnly && (_itemsListBox?.SelectedIndex ?? -1) > 0;
            if (_btnMoveDown != null) _btnMoveDown.IsEnabled = !_indexOnly && (_itemsListBox?.SelectedIndex ?? -1) >= 0 && (_itemsListBox?.SelectedIndex ?? -1) < _items.Count - 1;
        }

        public int CurrentIndex
        {
            get => _numCurrentIndex?.Value != null ? (int)_numCurrentIndex.Value.Value : 0;
            set
            {
                if (_numCurrentIndex == null) return;
                var v = value;
                if (v < 0) v = 0;
                if (_numCurrentIndex.Value == v) return;
                _numCurrentIndex.Value = v;
                SyncSelectionFromCurrentIndex();
            }
        }

        public PropertyGrid HostPropertyGrid
        {
            get => HostPropertyGrid2;
            set
            {
                HostPropertyGrid2 = value;
                ApplyFactoriesFromHostPropertyGrid();
            }
        }

        private void ApplyFactoriesFromHostPropertyGrid()
        {
            if (HostPropertyGrid2 == null)
                return;

            try
            {
                var dstFactoriesProp = HostPropertyGrid?.GetType().GetProperty("Factories", BindingFlags.Instance | BindingFlags.Public);
                if (dstFactoriesProp == null)
                    return;

                _ = dstFactoriesProp.GetValue(HostPropertyGrid2);
            }
            catch
            {
            }
        }

        public bool DialogResult => _dialogResult;

        public List<BitmapData> ImageSources
        {
            get
            {
                var result = new List<BitmapData>();
                foreach (var item in _items)
                {
                    if (item == null)
                    {
                        result.Add(new BitmapData());
                        continue;
                    }

                    var bytes = item.ToBytes();
                    var copy = new BitmapData();
                    copy.FromBytes(bytes);
                    result.Add(copy);
                }
                return result;
            }
            set
            {
                _items.Clear();
                if (value != null)
                {
                    foreach (var bmp in value)
                    {
                        _items.Add(bmp ?? new BitmapData());
                    }
                }

                if (_numCurrentIndex != null)
                {
                    _numCurrentIndex.Maximum = Math.Max(0, _items.Count - 1);
                }

                SyncSelectionFromCurrentIndex();

                UpdateButtonStates();
            }
        }

        public PropertyGrid HostPropertyGrid1 { get => HostPropertyGrid2; set => HostPropertyGrid2 = value; }
        public PropertyGrid HostPropertyGrid2 { get => _hostPropertyGrid; set => _hostPropertyGrid = value; }

        private void ItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonStates();

            if (_itemsListBox?.SelectedIndex >= 0 && _numCurrentIndex != null)
            {
                var idx = _itemsListBox.SelectedIndex;
                if (_numCurrentIndex.Value != idx)
                    _numCurrentIndex.Value = idx;
            }

            ScheduleUpdatePreview();
        }

        private void NumCurrentIndex_ValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {
            SyncSelectionFromCurrentIndex();
        }

        private void SyncSelectionFromCurrentIndex()
        {
            if (_itemsListBox == null) return;
            var idx = CurrentIndex;
            if (_items.Count <= 0)
            {
                _itemsListBox.SelectedIndex = -1;
                SetPreviewImage(null);
                return;
            }

            if (idx < 0) idx = 0;
            if (idx >= _items.Count) idx = _items.Count - 1;

            if (_itemsListBox.SelectedIndex != idx)
                _itemsListBox.SelectedIndex = idx;

            ScheduleUpdatePreview();
        }

        private void UpdateButtonStates()
        {
            var hasSelection = _itemsListBox?.SelectedItem != null;
            var idx = _itemsListBox?.SelectedIndex ?? -1;

            if (_indexOnly)
            {
                if (_btnAdd != null) _btnAdd.IsEnabled = false;
                if (_btnReplace != null) _btnReplace.IsEnabled = false;
                if (_btnDelete != null) _btnDelete.IsEnabled = false;
                if (_btnMoveUp != null) _btnMoveUp.IsEnabled = false;
                if (_btnMoveDown != null) _btnMoveDown.IsEnabled = false;
                return;
            }

            if (_btnAdd != null) _btnAdd.IsEnabled = true;
            if (_btnReplace != null) _btnReplace.IsEnabled = hasSelection;
            if (_btnDelete != null) _btnDelete.IsEnabled = hasSelection;
            if (_btnMoveUp != null) _btnMoveUp.IsEnabled = hasSelection && idx > 0;
            if (_btnMoveDown != null) _btnMoveDown.IsEnabled = hasSelection && idx >= 0 && idx < _items.Count - 1;
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_indexOnly) return;
            var bmp = await PickBitmapAsync();
            if (bmp == null) return;

            _items.Add(bmp);
            if (_numCurrentIndex != null)
                _numCurrentIndex.Maximum = Math.Max(0, _items.Count - 1);

            CurrentIndex = _items.Count - 1;
        }

        private async void BtnReplace_Click(object sender, RoutedEventArgs e)
        {
            if (_indexOnly) return;
            if (_itemsListBox.SelectedIndex < 0) return;

            var bmp = await PickBitmapAsync();
            if (bmp == null) return;

            var idx = _itemsListBox.SelectedIndex;
            _items[idx] = bmp;
            _itemsListBox.SelectedIndex = idx;
            ScheduleUpdatePreview();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_indexOnly) return;
            if (_itemsListBox.SelectedItem is not BitmapData item)
                return;

            var idx = _itemsListBox.SelectedIndex;
            _items.Remove(item);

            if (_numCurrentIndex != null)
                _numCurrentIndex.Maximum = Math.Max(0, _items.Count - 1);

            if (_items.Count > 0)
                CurrentIndex = Math.Min(idx, _items.Count - 1);
            else
                CurrentIndex = 0;

            UpdateButtonStates();
        }

        private void BtnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (_indexOnly) return;
            var idx = _itemsListBox.SelectedIndex;
            if (idx > 0)
            {
                var item = _items[idx];
                _items.RemoveAt(idx);
                _items.Insert(idx - 1, item);
                CurrentIndex = idx - 1;
            }
        }

        private void BtnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (_indexOnly) return;
            var idx = _itemsListBox.SelectedIndex;
            if (idx >= 0 && idx < _items.Count - 1)
            {
                var item = _items[idx];
                _items.RemoveAt(idx);
                _items.Insert(idx + 1, item);
                CurrentIndex = idx + 1;
            }
        }

        private void ScheduleUpdatePreview()
        {
            var idx = _itemsListBox?.SelectedIndex ?? -1;
            BitmapData src = null;
            if (idx >= 0 && idx < _items.Count)
                src = _items[idx];

            CancellationTokenSource newCts;
            lock (_previewLock)
            {
                _previewCts?.Cancel();
                _previewCts?.Dispose();
                _previewCts = new CancellationTokenSource();
                newCts = _previewCts;
            }

            var token = newCts.Token;

            if (src == null)
            {
                SetPreviewImage(null);
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(60, token);

                    var bmp = TryDecodeBitmap(src);
                    if (token.IsCancellationRequested)
                        return;

                    Dispatcher.UIThread.Post(() =>
                    {
                        if (!token.IsCancellationRequested)
                            SetPreviewImage(bmp);
                    }, DispatcherPriority.Background);
                }
                catch
                {
                }
            }, token);
        }

        private static Bitmap TryDecodeBitmap(BitmapData bitmapData)
        {
            try
            {
                if (bitmapData == null)
                    return null;

                var bytes = bitmapData.ToBytes();
                if (bytes == null || bytes.Length == 0)
                    return null;

                using var ms = new MemoryStream(bytes);
                return new Bitmap(ms);
            }
            catch
            {
                return null;
            }
        }

        private void SetPreviewImage(IImage image)
        {
            if (_previewImage == null) return;
            _previewImage.Source = image;
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            lock (_previewLock)
            {
                _previewCts?.Cancel();
                _previewCts?.Dispose();
                _previewCts = null;
            }
        }

        private async Task<BitmapData> PickBitmapAsync()
        {
            try
            {
                if (StorageProvider == null)
                    return null;

                var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    AllowMultiple = false,
                    Title = "选择图片",
                    FileTypeFilter = new List<FilePickerFileType>
                    {
                        new FilePickerFileType("Images")
                        {
                            Patterns = new List<string> { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif", "*.webp" }
                        }
                    }
                });

                if (files == null || files.Count == 0)
                    return null;

                var file = files[0];
                await using var stream = await file.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);

                var bytes = ms.ToArray();
                if (bytes == null || bytes.Length == 0)
                    return null;

                var bmp = new BitmapData();
                bmp.FromBytes(bytes);
                return bmp;
            }
            catch
            {
                return null;
            }
        }
    }
}