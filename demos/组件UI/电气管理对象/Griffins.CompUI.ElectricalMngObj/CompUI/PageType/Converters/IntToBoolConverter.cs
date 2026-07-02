using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;
using System;

namespace Griffins.CompUI.ElectricalMngObj.Converters
{
    /// <summary>
    /// 整数转布尔值：Count > 0 → True，否则 False（含空值）
    /// </summary>
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 处理空值（ItemsSource=null 时 Count 为 null）
            if (value == null) return false;

            // 转换为整数，判断是否 > 0
            if (int.TryParse(value.ToString(), out int count))
            {
                return count > 0; // 核心逻辑：Count > 0 才返回 True
            }

            // 非整数类型（异常情况）返回 False
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("不支持反向转换");
        }
    }
}