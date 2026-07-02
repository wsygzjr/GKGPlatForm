using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using System;

namespace GKG.UI
{
    /// <summary>
    /// 矢量图控件
    /// </summary>
    public class PathControl : UserControl
    {
        private Avalonia.Controls.Shapes.Path? _innerPath;
         
        /// <summary>
        /// 填充颜色依赖属性
        /// </summary>
        public static readonly StyledProperty<IBrush> FillColorProperty =
            AvaloniaProperty.Register<PathControl, IBrush>(
                nameof(FillColor),
                defaultValue: new SolidColorBrush(Colors.Gray),
                defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// 填充颜色属性
        /// </summary>
        public IBrush FillColor
        {
            get => GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }
         
        /// <summary>
        ///  Path形状依赖属性
        /// </summary>
        public static readonly StyledProperty<Geometry> IconDataProperty =
            AvaloniaProperty.Register<PathControl, Geometry>(
                nameof(IconData),
                defaultValue: new StreamGeometry(), // 兼容低版本空Geometry
                defaultBindingMode: BindingMode.TwoWay);
        /// <summary>
        ///  Path形状属性
        /// </summary>
        public Geometry IconData
        {
            get => GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }
         
        /// <summary>
        ///  描边色依赖属性
        /// </summary>
        public static readonly StyledProperty<Brush> StrokeColorProperty =
            AvaloniaProperty.Register<PathControl, Brush>(nameof(StrokeColor), new SolidColorBrush(Colors.Gray));

        /// <summary>
        ///  描边色属性
        /// </summary>
        public Brush StrokeColor
        {
            get => GetValue(StrokeColorProperty);
            set => SetValue(StrokeColorProperty, value);
        }

        /// <summary>
        ///  描边厚度依赖属性
        /// </summary>
        public static readonly StyledProperty<double> StrokeThicknessProperty =
            AvaloniaProperty.Register<PathControl, double>(nameof(StrokeThickness), 1);
        /// <summary>
        ///  描边厚度属性
        /// </summary>
        public double StrokeThickness
        {
            get => GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }
         
        /// <summary>
        /// Icon尺寸依赖属性
        /// </summary>
        public static readonly StyledProperty<Size> IconSizeProperty =
            AvaloniaProperty.Register<PathControl, Size>(
                nameof(IconSize),
                defaultValue: new Size(41, 41),
                defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Icon尺寸属性
        /// </summary>
        public Size IconSize
        {
            get => GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PathControl() : base()
        {
            initializeControl();
            InitializeBindings();
            // 监听属性变化（无ReactiveUI的兼容写法）
            this.PropertyChanged += PathControl_PropertyChanged;
        }

        private void PathControl_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == IconDataProperty || e.Property == IconSizeProperty)
            {
                UpdatePathTransform();
            }
        }

        private void initializeControl()
        {
            // 让内部Path占满整个控件
            this.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            this.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            this.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            this.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;

            _innerPath = new Avalonia.Controls.Shapes.Path
            {
                Stretch = Stretch.Uniform, 
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                StrokeThickness = 0,
                UseLayoutRounding = true
            };
            Content = _innerPath;
        }

        private void InitializeBindings()
        {
            if (_innerPath == null) return;

            // 绑定核心属性
            _innerPath.Bind(
                Avalonia.Controls.Shapes.Path.FillProperty,
                new Binding(nameof(FillColor)) { Source = this, Mode = BindingMode.TwoWay });

            _innerPath.Bind(
                Avalonia.Controls.Shapes.Path.DataProperty,
                new Binding(nameof(IconData)) { Source = this, Mode = BindingMode.TwoWay });

            _innerPath.Bind(
                Avalonia.Controls.Shapes.Path.StrokeProperty,
                new Binding(nameof(StrokeColor)) { Source = this, Mode = BindingMode.TwoWay });

            _innerPath.Bind(
                Avalonia.Controls.Shapes.Path.StrokeThicknessProperty,
                new Binding(nameof(StrokeThickness)) { Source = this, Mode = BindingMode.TwoWay });
        }

        private void UpdatePathTransform()
        {
            // 兜底：空控件/空Geometry直接重置变换
            if (_innerPath == null || IconData == null)
            {
                _innerPath!.RenderTransform = new TransformGroup();
                return;
            }

            // 核心：通过Bounds判断是否为空Geometry（替代IsEmpty）
            var pathBounds = IconData.Bounds;
            if (pathBounds.Width <= 0 || pathBounds.Height <= 0)
            {
                _innerPath.RenderTransform = new TransformGroup();
                return;
            }

            // 尺寸合法性校验
            if (IconSize.Width <= 0 || IconSize.Height <= 0)
            {
                _innerPath.RenderTransform = new TransformGroup();
                return;
            }

            // 计算等比缩放+居中平移 
            //var scaleX = IconSize.Width / pathBounds.Width;
            //var scaleY = IconSize.Height / pathBounds.Height;
            var scaleX = IconSize.Width / 4 / pathBounds.Width;
            var scaleY = IconSize.Height / 4 / pathBounds.Height;

            var scale = Math.Min(scaleX, scaleY); // 等比缩放避免溢出

            double scaledWidth = pathBounds.Width * scale;
            double scaledHeight = pathBounds.Height * scale;

            // 组合变换（先缩放后平移）
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(scale, scale));
            //// 平移：让图形中心对齐IconSize的中心
            //double translateX = (IconSize.Width - scaledWidth) / 2 - pathBounds.X * scale;
            //double translateY = (IconSize.Height - scaledHeight) / 2 - pathBounds.Y * scale;
            //transformGroup.Children.Add(new TranslateTransform(translateX, translateY));

            _innerPath.RenderTransform = transformGroup;
        }
    }
}