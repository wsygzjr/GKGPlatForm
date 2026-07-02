using Avalonia.Data.Converters;
using Avalonia.Platform;
using System;
using System.Globalization;

namespace Griffins.CompUI.ElectricalMngObj.Converters
{
    /// <summary>
    ///  Bool 转颜色的转换器
    /// </summary>
    /// <summary>
    /// Bool值转颜色转换器
    /// </summary>
    public class StringPathToBitmapConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string path || string.IsNullOrEmpty(path))
            {
                using var stream = AssetLoader.Open(new Uri(""));
                return new Avalonia.Media.Imaging.Bitmap(stream); // 路径无效时兜底
            }

            try
            {
                // 嵌入式资源加载
                using var stream = AssetLoader.Open(new Uri(path));  
                return new Avalonia.Media.Imaging.Bitmap(stream);
            }
            catch
            {
                using var stream = AssetLoader.Open(new Uri("")); 
                return new Avalonia.Media.Imaging.Bitmap(stream); // 路径无效时兜底
            }
        }

        /// <summary>
        /// 获取空的 BitmapImage（1x1 透明像素）
        /// </summary>
        /// <returns>空图片实例</returns>
        //public static Bitmap GetEmptyBitmapImage()
        //{
        //    // 1. 创建 1x1 透明位图的内存流
        //    using var ms = new MemoryStream();
        //    var emptyBitmap = new Bitmap(
        //        PixelFormat.Bgra8888,
        //        AlphaFormat.Premul,
        //        new Vector(96, 96), // DPI
        //        new PixelSize(1, 1), // 1x1 像素
        //        new[] { 0x00, 0x00, 0x00, 0x00 } // 透明像素数据（BGRA：0=透明）
        //    );
        //    emptyBitmap.Save(ms);
        //    ms.Seek(0, SeekOrigin.Begin);

        //    // 2. 封装为 BitmapImage 返回
        //    var emptyImage = new BitmapImage();
        //    emptyImage.SetSource(ms); // 同步加载（Avalonia 11.x 也可改用 SetSourceAsync）
        //    return emptyImage;
        //}

        /// <summary>
        /// 获取空 Bitmap（1x1 透明）
        /// </summary>
        //public static Bitmap GetEmptyBitmap()
        //{
        //    return new Bitmap(
        //        PixelFormat.Bgra8888,
        //        AlphaFormat.Premul,
        //        new Vector(96, 96),
        //        new PixelSize(1, 1),
        //        new[] { 0x00, 0x00, 0x00, 0x00 }
        //    );
        //}



        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}