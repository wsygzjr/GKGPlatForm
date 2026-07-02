// Converters/ListCountToBoolConverter.cs
using Avalonia.Data.Converters;
using System.Globalization;
using System;

namespace Griffins.CompUI.TestGlueDirectWorkMM.Converters
{
    /// <summary>
    ///  （暂无用）为序号列准备值：Point+列表序号
    /// </summary>
    public class IndexOffsetConverter : IValueConverter
    {
        /// <summary>
        /// 偏移量（默认0，这里设为1即可实现0→1、1→2...）
        /// </summary>
        public int Offset { get; set; } = 0;

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // 校验输入：必须是int类型的索引
            if (value is int index)
            {
                // 返回 "Point+Offset"（Point=0→1，拼接为"Point1"）
                return $"Point{index + Offset}";
            }
            // 异常情况返回空字符串
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value;
        }
    }
} 