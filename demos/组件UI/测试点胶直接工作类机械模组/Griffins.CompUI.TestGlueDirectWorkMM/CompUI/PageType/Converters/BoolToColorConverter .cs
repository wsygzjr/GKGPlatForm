using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Griffins.CompUI.TestGlueDirectWorkMM.Converters
{
    /// <summary>
    ///  Bool 转颜色的转换器
    /// </summary>
    /// <summary>
    /// Bool值转颜色转换器
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        // 自定义 True 对应的颜色（默认绿色）
        public Brush TrueColor { get; set; } = new SolidColorBrush(Colors.Green);

        // 自定义 False 对应的颜色（默认灰色）
        public Brush FalseColor { get; set; } = new SolidColorBrush(Colors.Gray);

        /// <summary>
        /// Bool → 颜色（正向转换）
        /// </summary>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // 若值为 null 或不是 bool 类型，返回默认颜色（可自定义）
            if (value is not bool boolValue)
                return new SolidColorBrush(Colors.Gray); // 空值默认灰色

            // 根据 bool 值返回对应颜色
            return boolValue ? TrueColor : FalseColor;
        }

        /// <summary>
        /// 颜色 → Bool（反向转换，一般用不到，返回 null 即可）
        /// </summary>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}