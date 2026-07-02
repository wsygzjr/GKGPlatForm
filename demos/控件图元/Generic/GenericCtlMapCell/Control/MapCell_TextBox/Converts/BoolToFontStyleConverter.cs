using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox.Converts
{
    /// <summary>
    /// 是否斜体（bool） -> Avalonia FontStyle
    /// </summary>
    public class BoolToFontStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // true: Italic, false: Normal
            return value is bool b && b ? FontStyle.Italic : FontStyle.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
