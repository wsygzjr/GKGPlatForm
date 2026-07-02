using System;
using System.Globalization;
using Avalonia.Layout;
using Avalonia.Data.Converters;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider.Converts
{
    /// <summary>
    /// 方向枚举到Orientation转换器
    /// </summary>
    public class StringToOrientationConverter : IValueConverter
    {
        public static readonly StringToOrientationConverter Instance = new StringToOrientationConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SliderCommonInfo.DirectionEnum direction)
            {
                // 如果参数是Height，返回垂直方向的高度值（等于水平时的宽度，这里使用150作为默认宽度）
                if (parameter?.ToString() == "Height")
                {
                    return direction == SliderCommonInfo.DirectionEnum.垂直 ? 150.0 : double.NaN;
                }
                // 否则返回Orientation
                return direction == SliderCommonInfo.DirectionEnum.垂直 ? Orientation.Vertical : Orientation.Horizontal;
            }
            return parameter?.ToString() == "Height" ? double.NaN : Orientation.Horizontal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Orientation orientation)
            {
                return orientation == Orientation.Vertical ? SliderCommonInfo.DirectionEnum.垂直 : SliderCommonInfo.DirectionEnum.水平;
            }
            return SliderCommonInfo.DirectionEnum.水平;
        }
    }
}