using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox.Converts
{
    /// <summary>
    /// 颜色值转换为 Avalonia Brush
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 支持 Color / 字符串颜色（#AARRGGBB 等）两种输入
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }
            if (value is string colorString && !string.IsNullOrEmpty(colorString))
            {
                try
                {
                    if (Color.TryParse(colorString, out var parsedColor))
                        return new SolidColorBrush(parsedColor);
                }
                catch
                {
                }
            }
            // 解析失败时返回透明，避免绑定异常导致界面崩溃
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
