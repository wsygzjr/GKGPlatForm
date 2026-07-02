using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;
using System.IO;

namespace GKG.Map.MapCell.Generic.IconButton.Converts
{
    public class Base64ToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string base64 && !string.IsNullOrWhiteSpace(base64))
            {
                try
                {
                    // 兼容前端常见的 "data:image/png;base64," 前缀
                    var commaIndex = base64.IndexOf(',');
                    if (commaIndex >= 0)
                    {
                        base64 = base64.Substring(commaIndex + 1);
                    }
                    var bytes = System.Convert.FromBase64String(base64);
                    using var ms = new MemoryStream(bytes);
                    return new Bitmap(ms);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}