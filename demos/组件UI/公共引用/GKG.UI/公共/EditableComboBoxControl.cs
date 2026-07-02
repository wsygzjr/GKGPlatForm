using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Newtonsoft.JsonG.Linq;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.UI
{
    /// <summary>
    /// 带标题的可编辑的下拉框控件
    /// </summary>
    public class EditableComboBoxControl : BasicControl<EditableComboBoxViewModel>
    { 
        TextBox _inputTextBox; 
        // 私有字段：存储数据源（与内部 ComboBox 同步）
        //private IEnumerable _itemsSource;
        // 内部可编辑的 ComboBox 控件引用
        private ComboBox _innerComboBox;
        private Canvas canvas;

        /// <summary>
        /// 
        /// </summary>
        public EditableComboBoxControl() : base()
        {
            // 1. 创建  Canvas, 用于将重叠的控件置顶
            canvas = new Canvas()
            {
                Name = "canvas1",
                Height = 35,
            };
            // 3. 创建 TextBox
            _inputTextBox = new TextBox
            {
                Name = "InputTextBox",
                Watermark = "请选择或输入...",
                Focusable = true,
            };
            // 4. 创建 ComboBox，将下拉框置于画布底部
            _innerComboBox = new ComboBox
            {
                Name = "InnerComboBox",
                IsDropDownOpen = false,
                PlaceholderText = "请选择下拉项",
                IsEnabled = true,
                AutoScrollToSelectedItem = true,
                MaxDropDownHeight = 200,
                Focusable = false,
            };

            initializeControl();
        }

        private void initializeControl()
        {
            // 1. 创建外层 StackPanel
            var stackPanel = new StackPanel
            {
                Name = "stackPanel1",
            };

            _inputTextBox.TextChanged += InputTextBox_TextChanged;
            //_inputTextBox.KeyDown += InputTextBox_KeyDown;

            // 直接通过附加属性设置，无需依赖 SetZIndex 方法
            _inputTextBox.SetValue(Panel.ZIndexProperty, 10);  //  将文本框在画布做的 ZIndex设置为最大，使其置顶
            _innerComboBox.SetValue(Panel.ZIndexProperty, 0); // 将下拉框置于画布底部
            canvas.Children.Add(_inputTextBox);
            canvas.Children.Add(_innerComboBox);
            stackPanel.Children.Add(canvas);
            //// 7. 设置为当前容器的内容  
            this.SetContent(stackPanel, Avalonia.Layout.HorizontalAlignment.Stretch);

            BindPropertyMethod(); //绑定数据对象属性
        }

        // 根据条件构建组合控件 
        private void CreateDynamicLayout(bool isEdit)
        {
            // 仅允许选择列表项(禁止编辑) 
            if (!isEdit)
            {
                //canvas.Children.Remove(_inputTextBox); // 移除掉该控件
                _inputTextBox.IsVisible = false;  // 只设置隐藏
                //// 场景4：清空所有类名（仅保留基础样式）
                //_innerComboBox.Classes.Clear();
                //// 场景3：替换类名  
                //_innerComboBox.Classes.Add("InnerComboBoxNoEdit");
            }
        }
        /// <summary>
        ///  下拉框绑定相关属性
        /// </summary>
        private void BindPropertyMethod()
        {
            this.WhenActivated(disposables =>
            {
                try
                {
                    if (_innerComboBox!.ItemsSource == null)
                    {
                        this.WhenAnyValue(x => x.ViewModel!.ItemsSource)
                        .Subscribe(itemsSource =>
                        {
                            _innerComboBox.ItemsSource = ViewModel!.ItemsSource;
                        });
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

                    //选中值 
                    //用于创建一个可观察序列（Observable），当属性发生变化时，会发射新的值。
                    this.WhenAnyValue(x => x.ViewModel!.SelectedItem)
                    //对序列进行过滤，只保留非null的值，避免后续处理null的情况。
                    .Where(selectedItem => selectedItem != null) // 过滤null值
                    //订阅这个可观察序列，当有新的非null值发射时，会执行Subscribe的回调中处理
                    .Subscribe(selectedItem =>
                    {
                        if (_innerComboBox != null && selectedItem != null)
                        {
                            string strNewContent = ((ComBoxItem)selectedItem).DisplayName.ToString();
                            if (_inputTextBox.Text == null)
                                _inputTextBox.Text = strNewContent; // 手动更新文本框 
                            else if (!_inputTextBox.Text.Equals(strNewContent))
                                _inputTextBox.Text = strNewContent; // 手动更新文本框 
                        }
                    })
                    .DisposeWith(disposables);

                    // 根据控件可编辑状态去构建组合控件
                    this.WhenAnyValue(x => x.ViewModel!.IsEdit)
                    .Subscribe(isEdit =>
                    {
                        CreateDynamicLayout(isEdit);
                    })
                    .DisposeWith(disposables);

                    // 设置控件宽度
                    this.WhenAnyValue(x => x.ViewModel!.RightContentWidth)
                   .Subscribe(rightContentwidthComboxViewModel =>
                   {
                       double contentWidth = (double)rightContentwidthComboxViewModel!;
                       double comboBoxWidth = contentWidth - 16; //下拉框宽度（下拉框占画布的全部宽度） ，16是stackPanel1外边距 ，16是NoEditInnerComboBox内边距
                       if (comboBoxWidth > 0)
                       {
                           if (_inputTextBox != null)
                           {
                               double textWidth = comboBoxWidth - 38;  //38为下拉框的下拉箭头部分的宽度 
                               _inputTextBox.Width = textWidth;  //标签宽度   
                           }
                           _innerComboBox.Width = comboBoxWidth;
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
                        //_innerComboBox.Width=  ViewModel.RightContentWidth == null ? 100: (double) ViewModel.RightContentWidth;                        
                    }
                    //_innerComboBox.Width = 148;  //zgl 作为调试代码用：下拉框实例无数据源时，必须要有宽度才能显示。

                }
                catch
                {
                    throw;
                }
            });
        }

        /// <summary>
        /// 文本框改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTextBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            bool flag = false;
            // 获取当前文本
            var textBox = sender as TextBox;
            string newText = textBox?.Text ?? string.Empty;
            ComBoxItem selectedOption;
            // 选择下拉框项时，将SelectedItem值直接赋予 SelectedItem新实例中，这样就能统一取SelectedItem中的DisplayName
            if (this.ViewModel!.SelectedItem != null)
            {
                selectedOption = (ComBoxItem)this.ViewModel.SelectedItem;
                if (!selectedOption.DisplayName.Equals(newText))
                {
                    flag = true;
                }
            }
            else
            {
                flag = true;
            }

            if (flag)
            {
                // 输入的文本值时，将输入的文本值放入 SelectedItem新实例中，这样就能统一取SelectedItem中的DisplayName
                selectedOption = new ComBoxItem()
                {
                    Value = newText,
                    DisplayName = newText,
                };
                this.ViewModel.SelectedItem = null;
                this.ViewModel.SelectedItem = selectedOption;
            }
        }

        // 文本框按键事件：支持按 Enter 确认或 Down 键打开下拉
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 确认输入，可在此处添加验证逻辑
                _innerComboBox.IsDropDownOpen = false;
            }
            else if (e.Key == Key.Down)
            {
                // 按 Down 键打开下拉框
                _innerComboBox.IsDropDownOpen = true;
                _innerComboBox.Focus(); // 焦点移到下拉框，支持键盘选择
            }
        }
    }

    /// <summary>
    /// 下拉框控件-视图模型
    /// </summary>
    public class EditableComboBoxViewModel : BasicControlViewModel
    {
        // 存储 DisplayMemberBinding 对应的字段名（如 "Name"）
        private string _displayMemberPath = string.Empty;
        private bool _isDropDownOpen = false;
        private string _placeholderText = string.Empty;
        private bool _isEnabled = true;
        private int _maxDropDownHeight = 200;
        private bool _autoScrollToSelectedItem = false;
        private bool _isEdit = false;

        /// <summary>
        /// 暴露 是否启用控件 属性（绑定到内部ComboBox）
        /// </summary>
        public bool IsEdit
        {
            get => _isEdit;
            set => this.RaiseAndSetIfChanged(ref _isEdit, value);
        }
        private IEnumerable? _itemsSource; 
        /// <summary>
        /// 数据源（下拉框选项集合）
        /// </summary>
        public IEnumerable? ItemsSource
        {
            get => _itemsSource;
            set => this.RaiseAndSetIfChanged(ref _itemsSource, value);
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
                if (value != null)
                {
                    if (_selectedItem != value)
                    {
                        var oldValue = _selectedItem; // 记录旧值
                        this.RaiseAndSetIfChanged(ref _selectedItem, value);
                        OnValueChanged(oldValue, value); // 触发基类事件
                    }
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