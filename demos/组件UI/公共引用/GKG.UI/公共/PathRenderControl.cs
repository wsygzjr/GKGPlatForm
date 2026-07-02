using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Skia;
using System;
using System.Globalization;

namespace GKG.UI
{
    /// <summary>
    ///不带标题的 Path手动渲染图形控件（ICon在控件中绝对居中，可传入缩放尺寸），可根据状态来改变状态颜色和形状
    /// </summary>
    public class PathRenderControl : Control
    {
        #region 依赖属性
        /// <summary>
        /// 填充颜色依赖属性
        /// </summary>
        public static readonly StyledProperty<IBrush> FillColorProperty =
            AvaloniaProperty.Register<PathControl, IBrush>(
                nameof(FillColor),
                defaultValue: new SolidColorBrush(Colors.Gray),
                defaultBindingMode: BindingMode.TwoWay
            );
        /// <summary>
        /// 填充颜色属性
        /// </summary>
        public IBrush FillColor
        {
            get => GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }
         
        /// <summary>
        /// 描边色依赖属性
        /// </summary>
        public static readonly StyledProperty<Brush> StrokeColorProperty =
            AvaloniaProperty.Register<PathControl, Brush>(nameof(StrokeColor), new SolidColorBrush(Colors.Gray));
        /// <summary>
        /// 描边色属性
        /// </summary>
        public Brush StrokeColor
        {
            get => GetValue(StrokeColorProperty);
            set => SetValue(StrokeColorProperty, value);
        }
        /// <summary>
        /// 描边厚度依赖属性
        /// </summary>
        public static readonly StyledProperty<double> StrokeThicknessProperty =
            AvaloniaProperty.Register<PathRenderControl, double>(nameof(StrokeThickness), 1);
        /// <summary>
        /// 描边厚度属性
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
            AvaloniaProperty.Register<PathRenderControl, Size>(nameof(IconSize), new Size(220, 220));
        /// <summary>
        /// Icon尺寸属性
        /// </summary>
        public Size IconSize
        {
            get => GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
         
        /// <summary>
        /// 矢量图路径字符串依赖属性
        /// </summary>
        public static readonly StyledProperty<string> IconDataPathProperty =
            AvaloniaProperty.Register<PathRenderControl, string>(nameof(IconDataPath)
                , "M25,7.5 A17.5,17.5 0 1 1 25,42.5 A17.5,17.5 0 1 1 25,7.5 Z");
        /// <summary>
        /// 矢量图路径字符串属性
        /// </summary>
        public string IconDataPath
        {
            get => GetValue(IconDataPathProperty);
            set => SetValue(IconDataPathProperty, value);
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public PathRenderControl()
        {
            // 绑定宽高到TargetSize，确保控件填满Grid
            this.Bind(WidthProperty, new Binding(nameof(IconSize)) { Source = this, Converter = new SizeToWidthConverter() });
            this.Bind(HeightProperty, new Binding(nameof(IconSize)) { Source = this, Converter = new SizeToHeightConverter() }); 
        }

        /// <summary>
        /// 矢量图重绘
        /// </summary>
        /// <param name="context"></param>
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            try
            {
                // 原始圆形路径（半径17.5，直径35）
                //string IconDataPath = "M25,7.5 A17.5,17.5 0 1 1 25,42.5 A17.5,17.5 0 1 1 25,7.5 Z"; //测试
                if (string.IsNullOrEmpty(IconDataPath)) return;

                var geometry = StreamGeometry.Parse(IconDataPath);
                var pathBounds = geometry.Bounds;
                if (pathBounds.Width <= 0 || pathBounds.Height <= 0) return;

                // ========== 核心修正：精准计算路径尺寸（让描边完全在内） ==========
                // 最终路径直径 = TargetSize - StrokeThickness（描边完全在路径内侧）
                double finalPathWidth = IconSize.Width - (StrokeThickness * 6) ;
                double finalPathHeight = IconSize.Height - (StrokeThickness * 6);

                if (finalPathWidth <= 0 || finalPathHeight <= 0) return;

                // 等比缩放比例（无安全余量，避免过度压缩）
                double scaleRatio = Math.Min(finalPathWidth / pathBounds.Width, finalPathHeight / pathBounds.Height);

                // ========== 修正中心坐标（消除浮点误差） ==========
                // 路径原始中心（精确到小数点后2位）
                double pathCenterX = Math.Round(pathBounds.X + pathBounds.Width / 2, 2);
                double pathCenterY = Math.Round(pathBounds.Y + pathBounds.Height / 2, 2);
                // Grid中心（整数，无浮点误差）
                double controlCenterX = Math.Round(IconSize.Width / 2, 2);
                double controlCenterY = Math.Round(IconSize.Height / 2, 2);

                // ========== 变换矩阵：让路径+描边完全居中且在内 ==========
                // 逻辑：路径归位原点 → 缩放 → 平移到Grid中心 → 向内偏移描边的一半（关键！）
                var transformMatrix = Matrix.CreateTranslation(-pathCenterX, -pathCenterY)
                                     * Matrix.CreateScale(scaleRatio, scaleRatio)
                                     * Matrix.CreateTranslation(controlCenterX, controlCenterY)
                                     * Matrix.CreateTranslation(-StrokeThickness / 2, -StrokeThickness / 2); // 向内偏移描边宽度的一半

                // 推送变换（自动释放）
                using (context.PushTransform(transformMatrix))
                {
                    var pen = new Pen(StrokeColor, StrokeThickness)
                    {
                        LineJoin = PenLineJoin.Round,
                        LineCap = PenLineCap.Round, 
                    };

                    // 裁剪区域：匹配Grid尺寸（仅裁剪极端情况）
                    var clipRect = new Rect(StrokeThickness / 2, StrokeThickness / 2,
                                           IconSize.Width - StrokeThickness,
                                           IconSize.Height - StrokeThickness);
                    var clipGeometry = new RoundedRect(clipRect);
                    using (context.PushClip(clipGeometry))
                    {
                        // 绘制路径：此时描边完全在clipRect内，不会被Grid裁切
                        context.DrawGeometry(FillColor, pen, geometry);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"路径绘制失败：{ex.Message}");
            }
        }

        #region 转换器
        private class SizeToWidthConverter : IValueConverter
        {
            public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            {
                return value is Size size ? size.Width : 220;
            }

            public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private class SizeToHeightConverter : IValueConverter
        {
            public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            {
                return value is Size size ? size.Height : 220;
            }

            public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}