using Avalonia.Controls;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace GKG.Map.StationFuncCtlMapCell.Convert
{
    /// <summary>
    /// 将 ViewModel 暴露的 double 比例安全转换为 Avalonia 的 GridLength
    /// </summary>
    public class RatioToGridLengthConverter : IValueConverter
    {
        /// <summary>
        /// 静态全局单例，实现零内存分配
        /// </summary>
        public static readonly RatioToGridLengthConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double ratio)
            {
                // 如果比例大于 0，说明需要显示，将其转换为 Star (例如 1.2*)
                if (ratio > 0)
                {
                    return new GridLength(ratio, GridUnitType.Star);
                }

                // 如果比例小于等于 0，直接将其宽度设置为 0 像素，实现完美折叠隐藏
                return new GridLength(0, GridUnitType.Pixel);
            }

            // 容错后备方案
            return new GridLength(0, GridUnitType.Pixel);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("单向绑定，不支持反向转换");
        }
    }
}