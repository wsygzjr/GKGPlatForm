using Avalonia.Media;
using System;
using System.Globalization;
using GKG.Map.MapCell.Generic.IconButton;

namespace GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts
{
    /// <summary>
    /// 将 IconMargin 四边值换算为图标绘制偏移量
    /// </summary>
    public class IconMarginToTranslateTransformConverter : Avalonia.Data.Converters.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IconButtonMiscInfo miscInfo)
            {
                // IconMargin 现在只用于绘制偏移，不参与布局测量，因此文字不会因为图标偏移而重新排版。
                return new TranslateTransform(
                    miscInfo.IconMarginLeft - miscInfo.IconMarginRight,
                    miscInfo.IconMarginTop - miscInfo.IconMarginBottom);
            }

            return new TranslateTransform();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
