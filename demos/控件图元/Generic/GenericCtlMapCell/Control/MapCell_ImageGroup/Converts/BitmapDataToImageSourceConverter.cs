#nullable enable
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.Converts
{
    /// <summary>
    /// BitmapData → ImageSource 转换器
    /// </summary>
    public class BitmapDataToImageSourceConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is BitmapData bitmapData && bitmapData.Bitmap != null)
            {
                try
                {
                    return (Bitmap)bitmapData.Bitmap;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
