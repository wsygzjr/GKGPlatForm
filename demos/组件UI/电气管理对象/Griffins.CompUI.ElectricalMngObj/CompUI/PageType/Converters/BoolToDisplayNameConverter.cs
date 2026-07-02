using Avalonia.Data.Converters;
using System.Globalization;
using System;

namespace Griffins.CompUI.ElectricalMngObj.Converters
{
    /// <summary>
    /// Bool值转显示名称转换器
    /// </summary>
    public class BoolToDisplayNameConverter : IValueConverter
    { 
        /// <summary>
        /// Bool → 显示名称（正向转换）
        /// </summary>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // 若值为 null 或不是 bool 类型，返回默认否
            if (value is not bool boolValue)
                return "否"; 

            // 根据 bool 值返回对应显示名称
            return boolValue ? "是" : "否";
        }

        /// <summary>
        /// 显示名称 → Bool（反向转换，一般用不到，返回 null 即可）
        /// </summary>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}