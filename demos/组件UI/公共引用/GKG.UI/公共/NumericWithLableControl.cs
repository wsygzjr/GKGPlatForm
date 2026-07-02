using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections;
using System.Reactive.Disposables;
using System.Runtime.ConstrainedExecution;

namespace GKG.UI
{
    /// <summary>
    /// 带标题的数字输入框+标签控件
    /// </summary>
    public class NumericWithLableControl : BasicControl<NumericWithLableViewModel>
    { 
        private NumericUpDown _numericUpDown;
        private TextBlock _textBlock1;

        /// <summary>
        /// 
        /// </summary>
        public NumericWithLableControl() : base()
        { 
            // 2. 创建 NumericUpDown 数字输入框
            _numericUpDown = new NumericUpDown
            {
                Name = "NumericWithLableControlInnerNumericUpDown1",
                Increment = 1,
                Minimum = 0,
                Maximum = 6535,
                FormatString = "F0",
                Value = 0,
            };
            // 3.创建第一个 TextBlock
            _textBlock1 = new TextBlock
            {
                Name = "InnerTextBlock1",
                Text = "ms",
            };
            initializeControl();
        }

        private void initializeControl()
        {   
            // 1. 创建 WrapPanel，默认 Orientation 为 Horizontal（从左到右）
            var stackPanel = new StackPanel
            {
                Name = "stackPanel1",
                Orientation = Orientation.Horizontal, // 显式指定水平方向（默认值，可省略）   
            };
            //添加_numericUpDown 到stackPanel
            stackPanel.Children.Add(_numericUpDown);
            //添加 _textBlock1 到stackPanel
            stackPanel.Children.Add(_textBlock1);

            base.SetContent(stackPanel , Avalonia.Layout.HorizontalAlignment.Stretch);
              

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.ViewModel!.RightContentWidth)
               .Subscribe(rightContentwidthComboxViewModel =>
               {
                   if (rightContentwidthComboxViewModel != null && rightContentwidthComboxViewModel > 0)
                   {
                       double contentWidth = (double)rightContentwidthComboxViewModel;
                       double averagePerserving = contentWidth / 10;
                       double lblWidth = averagePerserving * 2 ;  //60为单位部分的宽度
                       _textBlock1.Width = lblWidth;  //标签宽度 
                       _textBlock1.MinWidth = 30;  //标签最小宽度   
                       double numericUpDownWidth = contentWidth  - lblWidth   - 22   ;    
                       _numericUpDown.Width = numericUpDownWidth;
                       _numericUpDown.MinWidth = 110;
                   } 

               })
               .DisposeWith(disposables);

                // 当前值
                this.Bind(ViewModel, vm => vm.LableText, v => v._textBlock1!.Text)
                    .DisposeWith(disposables);
                 
                // 禁用
                this.Bind(ViewModel, vm => vm.IsEnabled, v => v._numericUpDown!.IsEnabled)
                    .DisposeWith(disposables);

                // 显示
                this.Bind(ViewModel, vm => vm.IsVisible, v => v._numericUpDown!.IsVisible)
                    .DisposeWith(disposables);

                // 当前值
                this.Bind(ViewModel, vm => vm.Value, v => v._numericUpDown!.Value)
                    .DisposeWith(disposables);

                // 最小值
                this.Bind(ViewModel, vm => vm.Minimum, v => v._numericUpDown!.Minimum)
                   .DisposeWith(disposables);

                // 最大值Maximum
                this.Bind(ViewModel, vm => vm.Maximum, v => v._numericUpDown!.Maximum)
                   .DisposeWith(disposables);

                // 步长
                this.Bind(ViewModel, vm => vm.Increment, v => v._numericUpDown!.Increment)
                   .DisposeWith(disposables);
                 
                // 保留几位小数
                //用于创建一个可观察序列（Observable），当属性发生变化时，会发射新的值。
                this.WhenAnyValue(x => x.ViewModel!.DecimalPlaces) 
                //订阅这个可观察序列，当有新的非null值发射时，会执行Subscribe的回调中处理
                .Subscribe(decimalPlaces =>
                {
                    if (_numericUpDown != null && decimalPlaces >= 0 )
                    { 
                        switch (decimalPlaces)
                        {
                            case 0:
                                _numericUpDown!.FormatString = "F0";
                                break; 
                            case 1:
                                _numericUpDown!.FormatString = "F1";
                                break;
                            case 2:
                                _numericUpDown!.FormatString = "F2";
                                break;
                            case 3:
                                _numericUpDown!.FormatString = "F3";
                                break;
                            case 4:
                                _numericUpDown!.FormatString = "F4";
                                break;
                        }
                    }
                })
                .DisposeWith(disposables); 

            });
             
        }
    }

    /// <summary>
    /// 数字输入框控件-视图模型
    /// </summary>
    public class NumericWithLableViewModel  : BasicControlViewModel
    {
        private decimal _value;
        private decimal _minimum = 0;
        private decimal _maximum = 6535;
        private decimal _increment = 1;
        private int _decimalPlaces = 0; 
        private string _lableText = "ms";
        private bool _isEnabled = true; 

        /// <summary>
        /// 当前值（双向绑定）
        /// </summary>
        public decimal Value
        {
            get => _value; 
            set
            {
                if (_value != value)
                {
                    var oldValue = _value; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _value , value);
                    OnValueChanged(oldValue, value); // 触发基类事件
                }
            }
        }
        /// <summary>
        /// 最小值Minimum 
        /// </summary> 
        public decimal Minimum
        {
            get => _minimum;
            set => this.RaiseAndSetIfChanged(ref _minimum, value);
        }
        /// <summary>
        /// 最小值Minimum 
        /// </summary> 
        public decimal Maximum
        {
            get => _maximum;
            set => this.RaiseAndSetIfChanged(ref _maximum, value);
        }

        /// <summary>
        /// 步长
        /// </summary> 
        public decimal Increment
        {
            get => _increment;
            set => this.RaiseAndSetIfChanged(ref _increment, value);
        }
        /// <summary>
        /// 保留几位小数 
        /// </summary>    
        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set => this.RaiseAndSetIfChanged(ref _decimalPlaces, value); 
        } 
        /// <summary>
        /// Lable显示的文本
        /// </summary>
        public string LableText
        {
            get => _lableText;
            set => this.RaiseAndSetIfChanged(ref _lableText, value); 
        }
        /// <summary>
        /// 是否禁用，缺省不禁用true
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

    }

}