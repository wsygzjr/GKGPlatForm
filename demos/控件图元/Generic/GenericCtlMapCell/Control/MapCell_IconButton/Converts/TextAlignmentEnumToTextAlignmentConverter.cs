using Avalonia.Media;

using System;
using System.Globalization;
using GKG.Map.MapCell.Generic.IconButton;

namespace GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts
{
    /// <summary>
    /// 将IconButtonParagraphInfo.TextAlignmentEnum转换为Avalonia.Media.TextAlignment
    /// </summary>
    public class TextAlignmentEnumToTextAlignmentConverter : Avalonia.Data.Converters.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IconButtonParagraphInfo.TextAlignmentEnum textAlignmentEnum)
            {
                return textAlignmentEnum switch
                {
                    IconButtonParagraphInfo.TextAlignmentEnum.Left => TextAlignment.Left,
                    IconButtonParagraphInfo.TextAlignmentEnum.Center => TextAlignment.Center,
                    IconButtonParagraphInfo.TextAlignmentEnum.Right => TextAlignment.Right,
                    _ => TextAlignment.Center
                };
            }
            return TextAlignment.Center;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TextAlignment textAlignment)
            {
                return textAlignment switch
                {
                    TextAlignment.Left => IconButtonParagraphInfo.TextAlignmentEnum.Left,
                    TextAlignment.Center => IconButtonParagraphInfo.TextAlignmentEnum.Center,
                    TextAlignment.Right => IconButtonParagraphInfo.TextAlignmentEnum.Right,
                    _ => IconButtonParagraphInfo.TextAlignmentEnum.Center
                };
            }
            return IconButtonParagraphInfo.TextAlignmentEnum.Center;
        }
    }
}