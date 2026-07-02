using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.Convert
{
    /// <summary>
    /// Color → SolidColorBrush 转换器
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        /// <summary>
        /// 静态全局单例，配合 XAML 中的 {x:Static} 实现零 GC 绑定
        /// </summary>
        public static readonly ColorToBrushConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color;
            }
            return Colors.Transparent;
        }
    }
}