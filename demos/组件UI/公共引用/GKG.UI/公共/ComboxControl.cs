using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using GKG.UI.General;
using Newtonsoft.JsonG.Linq;
using ReactiveUI;
using System;
using System.Collections;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq; 
using System.Threading;

namespace GKG.UI
{
    /// <summary>
    /// 带标题的不可编辑的下拉框控件
    /// </summary>
    public class ComboxControl : BasicControl<ComboxViewModel>
    {
        private ComboBox? _innerComboBox;

        /// <summary>
        /// 视图引用（用于显示对话框）
        /// </summary>
        private Control? _viewReference;

        /// <summary>
        /// 
        /// </summary>
        public ComboxControl() : base()
        {
            initializeControl();
        }
        private void initializeControl()
        {
            _innerComboBox = new ComboBox
            {
                Name = "ComboBox1",  
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, 
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                IsDropDownOpen =false, 
                PlaceholderText = "请选择下拉项",
                IsEnabled = true,
                AutoScrollToSelectedItem = true ,
                MaxDropDownHeight = 200,  
            };

            //  设置为当前容器的内容  
            this.SetContent(_innerComboBox, Avalonia.Layout.HorizontalAlignment.Stretch);

            this.WhenActivated(disposables =>
            {
                try
                {
                    if(_innerComboBox!.ItemsSource==null)
                    {
                        this.WhenAnyValue(x => x.ViewModel!.ItemsSource)
                    .Subscribe(itemsSource =>
                    {
                        _innerComboBox.ItemsSource = ViewModel!.ItemsSource;
                    });
                      //  this.Bind(ViewModel, vm => vm.ItemsSource, v => v._innerComboBox!.ItemsSource)
                      //.DisposeWith(disposables);
                    }

                    this.Bind(ViewModel, vm => vm.SelectedItem, v => v._innerComboBox!.SelectedItem)
                        .DisposeWith(disposables);

                    // 显示值字段
                    this.WhenAnyValue(x => x.ViewModel!.DisplayMemberPath)
                    .Subscribe(displayPath =>
                    {
                        if (_innerComboBox != null && !string.IsNullOrEmpty(displayPath))
                        {
                            _innerComboBox.ItemTemplate = null;
                            _innerComboBox.DisplayMemberBinding = new Binding(displayPath); // 这里才需要创建 Binding
                        }
                    })
                    .DisposeWith(disposables);


                    this.Bind(ViewModel, vm => vm.IsDropDownOpen, v => v._innerComboBox!.IsDropDownOpen)
                        .DisposeWith(disposables);

                    this.Bind(ViewModel, vm => vm.PlaceholderText, v => v._innerComboBox!.PlaceholderText)
                        .DisposeWith(disposables);

                    this.Bind(ViewModel, vm => vm.IsEnabled, v => v._innerComboBox!.IsEnabled)
                        .DisposeWith(disposables);

                    // 显示
                    this.Bind(ViewModel, vm => vm.IsVisible, v => v._innerComboBox!.IsVisible)
                        .DisposeWith(disposables);

                    this.Bind(ViewModel, vm => vm.MaxDropDownHeight, v => v._innerComboBox!.MaxDropDownHeight)
                        .DisposeWith(disposables);

                    this.Bind(ViewModel, vm => vm.AutoScrollToSelectedItem, v => v._innerComboBox!.AutoScrollToSelectedItem)
                        .DisposeWith(disposables);

                    // 设置控件宽度（避免右侧区域宽度变化时文字被挤压）
                    this.WhenAnyValue(x => x.ViewModel!.RightContentWidth)
                        .Subscribe(rightContentWidth =>
                        {
                            if (_innerComboBox == null)
                                return;

                            if (rightContentWidth.HasValue)
                            {
                                var comboBoxWidth = rightContentWidth.Value;
                                if (comboBoxWidth > 0)
                                {
                                    _innerComboBox.Width = comboBoxWidth;
                                }
                            }
                            else
                            {
                                _innerComboBox.ClearValue(WidthProperty);
                            }
                        })
                        .DisposeWith(disposables);

                    if (ViewModel != null)
                    {
                        var dataSource = ViewModel.ItemsSource;
                        if (dataSource != null && ViewModel.SelectedItem != null)
                        {
                            //int index = GetSelectedItemIndex(dataSource, ViewModel.SelectedItem);
                            //_innerComboBox.SelectedIndex = index;
                            _innerComboBox.SelectedItem = null;
                            _innerComboBox.SelectedItem = ViewModel.SelectedItem;
                        }
                    }
                    //_innerComboBox.Width = 148;  //zgl 作为调试代码用：下拉框实例无数据源时，必须要有宽度才能显示。
                }
                catch 
                {
                    throw  ; 
                }
            });
             
        }


        /// <summary>
        /// 设置视图引用（供对话框使用）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view ?? throw new ArgumentNullException(nameof(view), "视图引用不能为空");
        }

        private int GetSelectedItemIndex(IEnumerable dataSource, object selectedItem)
        {
            if (dataSource == null || selectedItem == null)
                return -1;

            int index = 0;
            foreach (var item in dataSource)
            { 
                // 注意：此处需确保对象相等性判断正确（可重写 Equals 或使用自定义比较逻辑）
                if (item.Equals(selectedItem))
                {
                    return index;
                }
                index++;
            }
            return -1; // 未找到选中项
        } 
    }

    /// <summary>
    /// 下拉框控件-视图模型
    /// </summary>
    public class ComboxViewModel : BasicControlViewModel
    {
        // 存储 DisplayMemberBinding 对应的字段名（如 "Name"）
        private string _displayMemberPath = string.Empty;
        private bool _isDropDownOpen = false;
        private string _placeholderText = string.Empty;
        private bool _isEnabled = true;
        private int _maxDropDownHeight = 200;
        private bool _autoScrollToSelectedItem = false;

        private IEnumerable? _itemsSource;
        /// <summary>
        /// 数据源（下拉框选项集合）
        /// </summary>
        public IEnumerable? ItemsSource
        {
            get => _itemsSource;
            set
            {
                this.RaiseAndSetIfChanged(ref _itemsSource, value);
            }
        }
        private object? _selectedItem;
        /// <summary>
        /// 选中项（双向绑定）
        /// </summary>
        public object? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == null)
                    return;
                if (_selectedItem != value)
                {
                    var oldValue = _selectedItem; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _selectedItem, value);
                    OnValueChanged(oldValue, value); // 触发基类事件
                }
            }
        }

        /// <summary>
        /// 显示字段名（如 "Name"，告诉视图要显示数据源中的哪个字段）
        /// </summary>
        public string DisplayMemberPath
        {
            get => _displayMemberPath;
            set => this.RaiseAndSetIfChanged(ref _displayMemberPath, value);
        }
        /// <summary>
        /// 暴露 下拉框是否展开 属性（绑定到内部ComboBox）
        /// </summary>
        public bool IsDropDownOpen
        {
            get => _isDropDownOpen;
            set => this.RaiseAndSetIfChanged(ref _isDropDownOpen, value);
        }
        /// <summary>
        /// 暴露 未选中时的占位文本 属性（绑定到内部ComboBox）
        /// </summary>
        public string PlaceholderText
        {
            get => _placeholderText;
            set => this.RaiseAndSetIfChanged(ref _placeholderText, value);
        }
        /// <summary>
        /// 暴露 是否启用控件 属性（绑定到内部ComboBox）
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }

        private bool _isVisible = true;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }

        /// <summary>
        /// 暴露 下拉列表的最大高度。这是列表部分的实际高度，而不是显示的元素数量 属性（绑定到内部ComboBox）
        /// </summary>
        public int MaxDropDownHeight
        {
            get => _maxDropDownHeight;
            set => this.RaiseAndSetIfChanged(ref _maxDropDownHeight, value);
        }
        /// <summary>
        /// 暴露 表示是否自动滚动到新选定的元素 属性（绑定到内部ComboBox）
        /// </summary>
        public bool AutoScrollToSelectedItem
        {
            get => _autoScrollToSelectedItem;
            set => this.RaiseAndSetIfChanged(ref _autoScrollToSelectedItem, value);
        }
    }
}