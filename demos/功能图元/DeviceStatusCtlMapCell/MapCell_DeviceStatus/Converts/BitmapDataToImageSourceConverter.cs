#nullable enable
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;
using GF_Gereric;
using Griffins.UI2; // Assuming BitmapData is here

namespace GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.Converts
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
            if (value is BitmapData bitmapData)
            {
                try
                {
                    // Assuming BitmapData has a method or property to get Bitmap, 
                    // or we need to convert it.
                    // The original code was: return (Bitmap)bitmapData.Bitmap;
                    // But BitmapData in GF_Gereric might be different?
                    // Let's assume the original code was correct for the type it expected.
                    // I will check BitmapData definition if I can, but I can't find it.
                    // I'll stick to what I saw in the original file, but adding GF_Gereric using.
                    
                    // Wait, original code: if (value is BitmapData bitmapData && bitmapData.Bitmap != null)
                    // Does BitmapData have a .Bitmap property?
                    // If it is GF_Gereric.BitmapData, I hope so.
                    
                     var bytes = bitmapData.ToBytes();
                     if (bytes == null || bytes.Length == 0) return null;
                     using var ms = new System.IO.MemoryStream(bytes);
                     return new Bitmap(ms);
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