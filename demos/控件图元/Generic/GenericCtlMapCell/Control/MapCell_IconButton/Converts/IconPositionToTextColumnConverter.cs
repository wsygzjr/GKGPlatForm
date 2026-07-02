using System;
using System.Globalization;
using GKG.Map.MapCell.Generic.IconButton;

namespace GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts
{
    /// <summary>
    /// 将IconPositionEnum转换为文本的Grid列索引
    /// </summary>
    public class IconPositionToTextColumnConverter : Avalonia.Data.Converters.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IconButtonMiscInfo.IconPositionEnum iconPosition)
            {
                return iconPosition switch
                {
                    IconButtonMiscInfo.IconPositionEnum.Left => 1,
                    IconButtonMiscInfo.IconPositionEnum.Right => 0,
                    IconButtonMiscInfo.IconPositionEnum.Top => 0,
                    IconButtonMiscInfo.IconPositionEnum.Bottom => 0,
                    _ => 1
                };
            }
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IconButtonMiscInfo.IconPositionEnum.Left;
        }
    }
}