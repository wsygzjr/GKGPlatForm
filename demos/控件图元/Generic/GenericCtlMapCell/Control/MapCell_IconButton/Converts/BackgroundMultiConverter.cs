using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts
{
    public class BackgroundMultiConverter : MarkupExtension, IMultiValueConverter
    {
        /// <summary>
        /// 运行时直接切按钮背景时复用和 XAML MultiBinding 一致的背景拼装规则，避免组选中态与普通绑定态出现两套背景算法。
        /// </summary>
        public static IBrush GetBackgroundBrush(BitmapData? bitmapData, Color color, ImageSizeMode sizeMode)
        {
            if (bitmapData != null && bitmapData.Bitmap != null)
            {
                var imageBrush = new BitmapDataToImageBrushConverter().Convert(bitmapData, typeof(IBrush), null, CultureInfo.CurrentCulture) as ImageBrush;
                if (imageBrush != null)
                {
                    imageBrush.Stretch = (Stretch)new ImageSizeModeToStretchConverter().Convert(sizeMode, typeof(Stretch), null, CultureInfo.CurrentCulture);
                    return imageBrush;
                }
            }

            return new SolidColorBrush(color);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count >= 3 && values[0] is BitmapData bitmapData && bitmapData.Bitmap != null)
            {
                var imageBrush = new BitmapDataToImageBrushConverter().Convert(bitmapData, targetType, parameter, culture) as ImageBrush;
                if (imageBrush != null && values[2] is ImageSizeMode sizeMode)
                {
                    imageBrush.Stretch = (Stretch)new ImageSizeModeToStretchConverter().Convert(sizeMode, targetType, parameter, culture);
                }
                return imageBrush;
            }

            if (values.Count >= 2 && values[1] is Color color)
            {
                return new SolidColorBrush(color);
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public IList<object> ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[targetTypes.Length];
        }
    }
}
