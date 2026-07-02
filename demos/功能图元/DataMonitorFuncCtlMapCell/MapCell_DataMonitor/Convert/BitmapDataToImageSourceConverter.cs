#nullable enable
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Griffins.UI2;
using System;
using System.Globalization;

namespace GKG.Map.DataMonitorFuncCtlMapCell.Convert
{
    /// <summary>
    /// BitmapData → ImageSource 转换器（ViewModel 是 BitmapData，UI 是 ImageSource）
    /// </summary>
    public class BitmapDataToImageSourceConverter : MarkupExtension, IValueConverter
    {
        // 实现 MarkupExtension 必需的 ProvideValue 方法（返回自身实例）
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this; // 直接返回转换器实例，供 XAML 调用
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
            // 双向绑定反向转换：一般不需要（图片通过文件选择器设置，而非 UI 直接修改 ImageSource）
            return null;
        }
    }
}
