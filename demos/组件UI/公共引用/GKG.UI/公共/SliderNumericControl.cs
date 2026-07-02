using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using ReactiveUI;
using System;
using System.Collections;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.UI
{
    /// <summary>
    ///  带标题的滑块+数字输入框
    /// </summary>
    public class SliderNumericControl : BasicControl<SliderNumericViewModel>
    {
        private NumericUpDown _numericUpDown; 
        private Slider _innerSlider; 

        /// <summary>
        /// 
        /// </summary>
        public SliderNumericControl() : base()
        {
            //滑块
            _innerSlider = new Slider
            {
                Name = "InnerSlider",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Value = 0,
                SmallChange = 1,
                TickFrequency = 1,
                IsSnapToTickEnabled = true,
                Minimum = 0,
                Maximum = 6535,
            };

            //DisableTrackClick(_innerSlider);

            //4. 数字输入框
            _numericUpDown = new NumericUpDown
            {
                Name = "SliderNumericControlInnerNumericUpDown",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
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
            var stackPanel = new StackPanel
            {
                Name = "stackPanel1",
                Orientation = Orientation.Horizontal,  
            };
            // 添加_innerSlider到stackPanel 
            stackPanel.Children.Add(_innerSlider);
            // 添加_numericUpDown到stackPanel 
            stackPanel.Children.Add(_numericUpDown);
             
            base.SetContent(stackPanel, Avalonia.Layout.HorizontalAlignment.Stretch);

            this.WhenActivated(disposables =>
            { 
                this.WhenAnyValue(x => x.ViewModel!.RightContentWidth)
                .Subscribe(rightContentwidthComboxViewModel =>
                {
                    if (rightContentwidthComboxViewModel != null)
                    {
                        double contentWidth = (double)rightContentwidthComboxViewModel;
                        double averagePerserving = contentWidth / 10;
                        double numericUpDownWidth = averagePerserving * 5.5;  //当前值
                        _numericUpDown.Width = numericUpDownWidth;
                        //_numericUpDown.MinWidth = 80; 
                        double sliderWidth = contentWidth - 16 - numericUpDownWidth - 6; // 滑块宽度 16是stackPanel1外边距，6是InnerTextBlock1外边距  
                        _innerSlider.Width = sliderWidth;
                        //_innerSlider.MinWidth = 100;
                    }
                })
                .DisposeWith(disposables);

                //滑块
                //最小值
                this.WhenAnyValue(x => x.ViewModel!.Minimum)
                    .Select(min => (double)min)   
                    .BindTo(this, v => v._innerSlider!.Minimum)
                    .DisposeWith(disposables); 
                //最大值
                this.WhenAnyValue(x => x.ViewModel!.Maximum)
                    .Select(max => (double)max)  
                    .BindTo(this, v => v._innerSlider!.Maximum)
                    .DisposeWith(disposables); 
                this.WhenAnyValue(x => x.ViewModel!.Value)
                    .Select(value => (double)value)   
                    .BindTo(this, v => v._innerSlider!.Value)
                    .DisposeWith(disposables);
                 
                // 禁用
                this.Bind(ViewModel, vm => vm.IsEnabled, v => v._innerSlider!.IsEnabled)
                    .DisposeWith(disposables);

                // 显示
                this.Bind(ViewModel, vm => vm.IsVisible, v => v._innerSlider!.IsVisible)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel!.Value) 
                .Subscribe(value =>
                {
                    if (_numericUpDown != null)
                    { 
                        _numericUpDown.Value = value;  
                    }
                })
                .DisposeWith(disposables);


                // 禁用
                this.Bind(ViewModel, vm => vm.IsEnabled, v => v._numericUpDown!.IsEnabled)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.Value, v => v._numericUpDown!.Value)
                    .DisposeWith(disposables);
                 
                this.Bind(ViewModel, vm => vm.Minimum, v => v._numericUpDown!.Minimum)
                   .DisposeWith(disposables);
 
                this.Bind(ViewModel, vm => vm.Maximum, v => v._numericUpDown!.Maximum)
                   .DisposeWith(disposables);

                // 步长
                this.Bind(ViewModel, vm => vm.Increment, v => v._numericUpDown!.Increment)
                   .DisposeWith(disposables);

                //  小数位数 
                this.WhenAnyValue(x => x.ViewModel!.DecimalPlaces)  
                .Subscribe(decimalPlaces =>
                {
                    if (_numericUpDown != null && decimalPlaces >= 0 )
                    { 
                        switch (decimalPlaces)
                        {
                            case 0:
                                _numericUpDown!.FormatString = "F0";
                                _numericUpDown.Increment = 1; 
                                _innerSlider.TickFrequency = 1;     
                                _innerSlider.SmallChange = 1;     
                                _innerSlider.LargeChange = 10;      
                                break; 
                            case 1:
                                _numericUpDown!.FormatString = "F1";
                                _numericUpDown.Increment = 0.1M; 
                                _innerSlider.SmallChange = 0.1;     
                                _innerSlider.TickFrequency = 0.1;   
                                _innerSlider.LargeChange = 1.0;      
                                break;
                            case 2:
                                _numericUpDown!.FormatString = "F2";
                                _numericUpDown.Increment = 0.01M; 
                                _innerSlider.SmallChange = 0.01;     
                                _innerSlider.TickFrequency = 0.01;  
                                _innerSlider.LargeChange = 0.1;     
                                break;
                            case 3:
                                _numericUpDown!.FormatString = "F3";
                                _numericUpDown.Increment = 0.001M; 
                                _innerSlider.SmallChange = 0.001;     
                                _innerSlider.TickFrequency = 0.001;   
                                _innerSlider.LargeChange = 0.01;      
                                break;
                            case 4:
                                _numericUpDown!.FormatString = "F4";
                                _numericUpDown.Increment = 0.0001M; 
                                _innerSlider.SmallChange = 0.0001;     
                                _innerSlider.TickFrequency = 0.0001;   
                                _innerSlider.LargeChange = 0.001;     
                                break;
                            default:
                                _numericUpDown.FormatString = "F0";
                                _numericUpDown.Increment = 1; 
                                _innerSlider.TickFrequency = 1;     
                                _innerSlider.SmallChange = 1;     
                                _innerSlider.LargeChange = 10;      
                                break;
                        }
                    }
                })
                .DisposeWith(disposables); 
            }); 
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="slider">  </param>
        private static void DisableTrackClick(Slider slider)
        { 
            if (!slider.IsLoaded)
            { 
                slider.Loaded += (s, e) => DisableTrackClick(slider);
                return;
            }
             
            var track = slider.FindDescendantOfType<Track>();
            if (track == null) return;
             
            if (track.DecreaseButton is RepeatButton decreaseBtn)
            {
                decreaseBtn.Command = null;  
                decreaseBtn.IsHitTestVisible = false; 
            } 
            if (track.IncreaseButton is RepeatButton increaseBtn)
            {
                increaseBtn.Command = null;  
                increaseBtn.IsHitTestVisible = false;  
            }
        } 
    }
    
    /// <summary>
    ///  
    /// </summary>
    public class SliderNumericViewModel : BasicControlViewModel
    {
        private decimal _value = 1;
        private decimal _minimum = 0; 
        private decimal _maximum = 6535;
        private decimal _increment = 1;  
        private int _decimalPlaces = 0;
        private string _lableText = "";
        private bool _isEnabled = true;

        /// <summary>
        /// 当前值
        /// </summary>
        public decimal Value
        {
            get => _value; 
            set
            {
                if (_value != value)
                {
                    var oldValue = _value;  
                    this.RaiseAndSetIfChanged(ref _value , value);
                    OnValueChanged(oldValue, value); 
                }
            }
        }
        /// <summary>
        /// 最小值 
        /// </summary> 
        public decimal Minimum
        {
            get => _minimum;
            set => this.RaiseAndSetIfChanged(ref _minimum, value);
        }
        /// <summary>
        /// 最大值 
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
        /// 小数位数 
        /// </summary>    
        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set => this.RaiseAndSetIfChanged(ref _decimalPlaces, value); 
        } 
        /// <summary>
        /// Lable 
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