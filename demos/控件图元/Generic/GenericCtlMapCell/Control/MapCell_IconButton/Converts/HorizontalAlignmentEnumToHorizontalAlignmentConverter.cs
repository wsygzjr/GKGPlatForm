#nullable enable
using Avalonia.Layout;
using GKG.Map.MapCell.Generic.IconButton;
using System;
using System.Globalization;

namespace GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts
{
    /// <summary>
    /// 将IconButtonLayoutInfo.HorizontalAlignmentEnum转换为Avalonia.Layout.HorizontalAlignment
    /// </summary>
    public class HorizontalAlignmentEnumToHorizontalAlignmentConverter : Avalonia.Data.Converters.IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IconButtonLayoutInfo.HorizontalAlignmentEnum alignmentEnum)
            {
                return alignmentEnum switch
                {
                    IconButtonLayoutInfo.HorizontalAlignmentEnum.Left => HorizontalAlignment.Left,
                    IconButtonLayoutInfo.HorizontalAlignmentEnum.Center => HorizontalAlignment.Center,
                    IconButtonLayoutInfo.HorizontalAlignmentEnum.Right => HorizontalAlignment.Right,
                    IconButtonLayoutInfo.HorizontalAlignmentEnum.Stretch => HorizontalAlignment.Stretch,
                    _ => HorizontalAlignment.Stretch
                };
            }
            return HorizontalAlignment.Stretch;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is HorizontalAlignment horizontalAlignment)
            {
                return horizontalAlignment switch
                {
                    HorizontalAlignment.Left => IconButtonLayoutInfo.HorizontalAlignmentEnum.Left,
                    HorizontalAlignment.Center => IconButtonLayoutInfo.HorizontalAlignmentEnum.Center,
                    HorizontalAlignment.Right => IconButtonLayoutInfo.HorizontalAlignmentEnum.Right,
                    HorizontalAlignment.Stretch => IconButtonLayoutInfo.HorizontalAlignmentEnum.Stretch,
                    _ => IconButtonLayoutInfo.HorizontalAlignmentEnum.Stretch
                };
            }
            return IconButtonLayoutInfo.HorizontalAlignmentEnum.Stretch;
        }
    }
}