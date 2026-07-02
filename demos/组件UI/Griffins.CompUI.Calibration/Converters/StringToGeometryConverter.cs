using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using System;
using System.Globalization;



namespace Griffins.CompUI.Calibration.Converters
{
    /// <summary>
    /// 适配 Avalonia 11+ 的字符串转 Geometry 转换器
    /// </summary>
    public class StringToGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 1. 空值/非字符串处理：用 StreamGeometry.Empty 替代 Geometry.Empty
            if (value is not string pathStr || string.IsNullOrEmpty(pathStr))
                return StreamGeometry.Parse("");

            try
            {
                return StreamGeometry.Parse(pathStr);
            }
            catch (Exception)
            {
                // 解析失败返回空几何图形
                return StreamGeometry.Parse("");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotSupportedException("不支持反向转换");
            return null;
        }
    }
}