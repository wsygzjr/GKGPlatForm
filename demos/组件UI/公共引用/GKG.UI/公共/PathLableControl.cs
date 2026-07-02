using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using ReactiveUI;
using System.Reactive.Disposables;

namespace GKG.UI
{
    /// <summary>
    /// 带标题的矢量图控件
    /// </summary>
    public class PathLableControl : BasicControl<PathLableControlViewModel>
    {
        private PathControl? _pathControl;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PathLableControl() : base()
        {
            initializeTextBox();
        }

        private void initializeTextBox()
        { 
            _pathControl = new PathControl
            { 
            }; 

            base.SetContent(_pathControl, Avalonia.Layout.HorizontalAlignment.Stretch);

            this.WhenActivated(disposables =>
            { 
                this.Bind(ViewModel, vm => vm.FillColor, v => v._pathControl!.FillColor)
                    .DisposeWith(disposables);
                 
                this.Bind(ViewModel, vm => vm.IconData , v => v._pathControl!.IconData)
                    .DisposeWith(disposables);
                 
                this.Bind(ViewModel, vm => vm.StrokeColor, v => v._pathControl!.StrokeColor)
                    .DisposeWith(disposables);
                 
                this.Bind(ViewModel, vm => vm.StrokeThickness , v => v._pathControl!.StrokeThickness)
                    .DisposeWith(disposables);
                  
                // 禁用
                this.Bind(ViewModel, vm => vm.IsEnabled, v => v._pathControl!.IsEnabled)
                    .DisposeWith(disposables); 

                // 显示
                this.Bind(ViewModel, vm => vm.IsVisible, v => v._pathControl!.IsVisible)
                    .DisposeWith(disposables);

            });
        }
    }

    /// <summary>
    /// 带标题的时分秒选择器控件-视图模型
    /// </summary>
    public class PathLableControlViewModel : BasicControlViewModel
    {
        private double _strokeThickness = 1;
        /// <summary>
        /// 描边厚度属性
        /// </summary>
        public double StrokeThickness
        {
            get => _strokeThickness;
            set
            {
                if (_strokeThickness != value)
                {
                    var oldValue = _strokeThickness; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _strokeThickness, value); 
                }
            }
        }

        private Brush _strokeColor = new SolidColorBrush(Colors.Gray);
        /// <summary>
        /// 描边色属性
        /// </summary>
        public Brush StrokeColor
        {
            get => _strokeColor;
            set
            {
                if (_strokeColor != value)
                {
                    var oldValue = _strokeColor; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _strokeColor, value); 
                }
            }
        }

        private IBrush _fillColor= new SolidColorBrush(Colors.Gray);
        /// <summary>
        /// 填充颜色
        /// </summary>
        public IBrush FillColor
        {
            get => _fillColor;
            set
            {
                if (_fillColor != value)
                {
                    var oldValue = _fillColor; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _fillColor, value); 
                }
            }
        }

        private Geometry _iconData = new StreamGeometry();
        /// <summary>
        /// 填充图形
        /// </summary>
        public Geometry IconData
        {
            get => _iconData;
            set
            {
                if (_iconData != value)
                {
                    var oldValue = _iconData; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _iconData, value); 
                }
            }
        }

        private bool _isEnabled = true;
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