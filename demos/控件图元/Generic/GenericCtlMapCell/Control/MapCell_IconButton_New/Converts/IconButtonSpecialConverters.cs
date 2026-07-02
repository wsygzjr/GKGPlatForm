using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace GKG.Map.MapCell.Generic.IconButton.Converts
{
    // 1. 圆角转换器：String -> CornerRadius
    public class StringToCornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrWhiteSpace(str))
            {
                try { return CornerRadius.Parse(str); } catch { return new CornerRadius(0); }
            }
            return new CornerRadius(0);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // 2. 图文间距转换器：Double -> Thickness (根据参数决定上下左右)
    public class SpacingToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double spacing && parameter is string direction)
            {
                return direction switch
                {
                    "Left" => new Thickness(0, 0, spacing, 0), // 图标在左，所以给图标的右边加 Margin
                    "Right" => new Thickness(spacing, 0, 0, 0), // 图标在右，左边加 Margin
                    "Top" => new Thickness(0, 0, 0, spacing),   // 图标在上，下边加 Margin
                    "Bottom" => new Thickness(0, spacing, 0, 0),// 图标在下，上边加 Margin
                    _ => new Thickness(0)
                };
            }
            return new Thickness(0);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}