using System;
using System.Globalization;
using Avalonia.Input;
using Avalonia.Data.Converters;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider.Converts
{
    /// <summary>
    /// 布尔值到Cursor转换器
    /// </summary>
    public class BooleanToCursorConverter : IValueConverter
    {
        public static readonly BooleanToCursorConverter Instance = new BooleanToCursorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool enabledCursor)
            {
                return enabledCursor ? new Cursor(StandardCursorType.Hand) : new Cursor(StandardCursorType.Arrow);
            }
            return new Cursor(StandardCursorType.Arrow);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}