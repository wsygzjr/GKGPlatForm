using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider.Converts
{
    /// <summary>
    /// 颜色到画笔转换器
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 处理Color类型
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }
            // 处理string类型
            else if (value is string colorString && !string.IsNullOrEmpty(colorString))
            {
                try
                {
                    if (Color.TryParse(colorString, out Color parsedColor))
                    {
                        return new SolidColorBrush(parsedColor);
                    }
                }
                catch (Exception)
                {
                    // 忽略颜色解析错误
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}