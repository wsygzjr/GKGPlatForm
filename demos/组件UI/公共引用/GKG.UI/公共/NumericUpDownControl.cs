using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections;
using System.Reactive.Disposables;

namespace GKG.UI
{
    /// <summary>
    /// 数字输入框上下加减按钮布局控件
    /// </summary>
    public class NumericUpDownControl : BasicControl<NumericUpDownViewModel>
    { 
        private NumericUpDown _numericUpDown;

        /// <summary>
        /// 
        /// </summary>
        public NumericUpDownControl() : base()
        {
            _numericUpDown = new NumericUpDown
            {
                Name = "NumericUpDown1",
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                Increment = 1,
                Minimum = 0,
                Maximum = 6535,
                FormatString = "F0",
                Value = 0,
            };

            initializeControl();
        }

        private void initializeControl()
        {
            base.SetContent(_numericUpDown , Avalonia.Layout.HorizontalAlignment.Stretch);

            this.WhenActivated(disposables =>
            {
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
    public class NumericUpDownViewModel : BasicControlViewModel
    {
        private decimal _value;
        private decimal _minimum = 0;
        private decimal _maximum = 6535;
        private decimal _increment =1;  
        private int _decimalPlaces = 0;
        private bool _isEnabled = true;  //缺省不禁用

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