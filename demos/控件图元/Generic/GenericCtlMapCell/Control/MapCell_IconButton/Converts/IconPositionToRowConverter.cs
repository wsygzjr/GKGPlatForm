using System;
using System.Globalization;
using GKG.Map.MapCell.Generic.IconButton;

namespace GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts
{
    /// <summary>
    /// 将IconPositionEnum转换为Grid行索引
    /// </summary>
    public class IconPositionToRowConverter : Avalonia.Data.Converters.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IconButtonMiscInfo.IconPositionEnum iconPosition)
            {
                return iconPosition switch
                {
                    IconButtonMiscInfo.IconPositionEnum.Left => 0,
                    IconButtonMiscInfo.IconPositionEnum.Right => 0,
                    IconButtonMiscInfo.IconPositionEnum.Top => 0,
                    IconButtonMiscInfo.IconPositionEnum.Bottom => 1,
                    _ => 0
                };
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IconButtonMiscInfo.IconPositionEnum.Left;
        }
    }
}