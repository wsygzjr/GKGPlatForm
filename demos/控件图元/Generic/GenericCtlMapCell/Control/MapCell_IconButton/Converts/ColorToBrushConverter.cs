using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts
{
    /// <summary>
    /// 颜色到画笔转换器
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}