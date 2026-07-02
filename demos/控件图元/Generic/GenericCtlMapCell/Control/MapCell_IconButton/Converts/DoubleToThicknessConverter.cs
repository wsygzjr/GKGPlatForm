using Avalonia;
using System;
using System.Globalization;


namespace GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts
{
    /// <summary>
    /// 将double值转换为Thickness，支持参数指定方向
    /// </summary>
    public class DoubleToThicknessConverter : Avalonia.Data.Converters.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = 0;
            if (value is int intValue)
            {
                doubleValue = intValue;
            }
            else if (value is double dValue)
            {
                doubleValue = dValue;
            }
            
            if (parameter is string direction)
            {
                return direction switch
                {
                    "Left" => new Thickness(doubleValue, 0, 0, 0),
                    "Top" => new Thickness(0, doubleValue, 0, 0),
                    "Right" => new Thickness(0, 0, doubleValue, 0),
                    "Bottom" => new Thickness(0, 0, 0, doubleValue),
                    _ => new Thickness(doubleValue)
                };
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness thickness && parameter is string direction)
            {
                return direction switch
                {
                    "Left" => thickness.Left,
                    "Top" => thickness.Top,
                    "Right" => thickness.Right,
                    "Bottom" => thickness.Bottom,
                    _ => thickness.Left
                };
            }
            return 0.0;
        }
    }
}