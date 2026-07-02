using Avalonia.Media;
using GKG.Map.MapCell.Generic.Control.MapCell_ComboBox.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ComboBox.ViewModels
{
    public class ComboBoxViewModel : PropertyModels.ComponentModel.ReactiveObject, IDisposable
    {
        private readonly ComboBoxPropertyModelEdit _propertyModel;
        private bool _disposed;

        private readonly Action _selectionChangedAction;
        private readonly Action _dropDownOpenedAction;
        private readonly Action _dropDownClosedAction;

        #region UI 独立控制状态 (UI State)

        private ObservableCollection<string> _items = new ObservableCollection<string>();
        public ObservableCollection<string> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private string _selectedItem = string.Empty;
        public string SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_propertyModel != null && _propertyModel.SelectedItem != value)
                {
                    _propertyModel.SelectedItem = value ?? string.Empty;
                }
            }
        }

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        private bool _isDropDownOpen;
        public bool IsDropDownOpen
        {
            get => _isDropDownOpen;
            set => SetProperty(ref _isDropDownOpen, value);
        }

        #endregion

        #region Model 扁平化属性映射 (Flattened Properties)
        public Color BackgroundColor => _propertyModel.BackgroundColor;
        public Color ForegroundColor => _propertyModel.ForegroundColor;
        public Color BorderBrush => _propertyModel.BorderBrush;
        public string BorderThickness => _propertyModel.BorderThickness;
        public int FontSize => _propertyModel.FontSize;
        public string FontFamily => _propertyModel.FontFamily;
        public bool IsBold => _propertyModel.IsBold;
        public bool IsItalic => _propertyModel.IsItalic;
        public double Opacity => _propertyModel.Opacity;
        public string Margin => _propertyModel.Margin;
        public string Padding => _propertyModel.Padding;
        public double MaxDropDownHeight => _propertyModel.MaxDropDownHeight;
        public string PlaceholderText => _propertyModel.PlaceholderText;
        public bool IsEditable => _propertyModel.IsEditable;

        public ComboBoxPropertyModelEdit Model => _propertyModel;
        #endregion

        public ComboBoxViewModel(
            ComboBoxPropertyModelEdit propertyModel,
            Action selectionChangedAction,
            Action dropDownOpenedAction,
            Action dropDownClosedAction)
        {
            _propertyModel = propertyModel;
            _selectionChangedAction = selectionChangedAction;
            _dropDownOpenedAction = dropDownOpenedAction;
            _dropDownClosedAction = dropDownClosedAction;

        }

        public void ReloadFromModel()
        {
            RefreshData();
            RefreshStyle();
            RefreshFont();
            RefreshLayout();
        }

        public void RefreshData(RunModeList extractedItems = null)
        {
            // 如果平台执行器提取了集合则使用提取的，否则回退读取 Model
            var targetItems = extractedItems ?? _propertyModel.Items ?? new RunModeList();
            Items = new ObservableCollection<string>(targetItems);

            _selectedItem = _propertyModel.SelectedItem ?? string.Empty;
            SelectedIndex = Items.IndexOf(_selectedItem);

            RaisePropertyChanged(nameof(SelectedItem));
            RaisePropertyChanged(nameof(PlaceholderText));
            RaisePropertyChanged(nameof(IsEditable));
        }

        public void RefreshStyle()
        {
            RaisePropertyChanged(nameof(BackgroundColor));
            RaisePropertyChanged(nameof(ForegroundColor));
            RaisePropertyChanged(nameof(BorderBrush));
            RaisePropertyChanged(nameof(BorderThickness));
            RaisePropertyChanged(nameof(Opacity));
        }

        public void RefreshFont()
        {
            RaisePropertyChanged(nameof(FontSize));
            RaisePropertyChanged(nameof(FontFamily));
            RaisePropertyChanged(nameof(IsBold));
            RaisePropertyChanged(nameof(IsItalic));
        }

        public void RefreshLayout()
        {
            RaisePropertyChanged(nameof(Margin));
            RaisePropertyChanged(nameof(Padding));
            RaisePropertyChanged(nameof(MaxDropDownHeight));
        }

        // =======================================================

        #region 事件转发
        public void NotifySelectionChanged() => _selectionChangedAction?.Invoke();
        public void NotifyDropDownOpened() => _dropDownOpenedAction?.Invoke();
        public void NotifyDropDownClosed() => _dropDownClosedAction?.Invoke();
        #endregion

        public void Dispose()
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}