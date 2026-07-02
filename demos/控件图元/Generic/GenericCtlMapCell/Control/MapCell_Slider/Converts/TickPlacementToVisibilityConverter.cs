using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider.Converts
{
    /// <summary>
    /// 根据刻度线位置决定指定一侧的刻度条是否显示。
    /// </summary>
    public class TickPlacementToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not TickPlacement placement || parameter is not string side)
                return false;

            return side switch
            {
                "TopLeft" => placement == TickPlacement.TopLeft || placement == TickPlacement.Outside,
                "BottomRight" => placement == TickPlacement.BottomRight || placement == TickPlacement.Outside,
                _ => false
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
