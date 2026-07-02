#nullable enable
using Avalonia.Layout;
using GKG.Map.MapCell.Generic.IconButton;
using System;
using System.Globalization;

namespace GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts
{
    /// <summary>
    /// 将IconButtonLayoutInfo.VerticalAlignmentEnum转换为Avalonia.Layout.VerticalAlignment
    /// </summary>
    public class VerticalAlignmentEnumToVerticalAlignmentConverter : Avalonia.Data.Converters.IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IconButtonLayoutInfo.VerticalAlignmentEnum alignmentEnum)
            {
                return alignmentEnum switch
                {
                    IconButtonLayoutInfo.VerticalAlignmentEnum.Top => VerticalAlignment.Top,
                    IconButtonLayoutInfo.VerticalAlignmentEnum.Center => VerticalAlignment.Center,
                    IconButtonLayoutInfo.VerticalAlignmentEnum.Bottom => VerticalAlignment.Bottom,
                    IconButtonLayoutInfo.VerticalAlignmentEnum.Stretch => VerticalAlignment.Stretch,
                    _ => VerticalAlignment.Stretch
                };
            }
            return VerticalAlignment.Stretch;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is VerticalAlignment verticalAlignment)
            {
                return verticalAlignment switch
                {
                    VerticalAlignment.Top => IconButtonLayoutInfo.VerticalAlignmentEnum.Top,
                    VerticalAlignment.Center => IconButtonLayoutInfo.VerticalAlignmentEnum.Center,
                    VerticalAlignment.Bottom => IconButtonLayoutInfo.VerticalAlignmentEnum.Bottom,
                    VerticalAlignment.Stretch => IconButtonLayoutInfo.VerticalAlignmentEnum.Stretch,
                    _ => IconButtonLayoutInfo.VerticalAlignmentEnum.Stretch
                };
            }
            return IconButtonLayoutInfo.VerticalAlignmentEnum.Stretch;
        }
    }
}