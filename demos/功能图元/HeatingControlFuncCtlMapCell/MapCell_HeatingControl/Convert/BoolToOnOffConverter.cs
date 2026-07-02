using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System;
using System.Globalization;

namespace GKG.Map.HeatingControlFuncCtlMapCell.Convert
{
    /// <summary>
    /// 布尔值到开/关文本转换器
    /// 将布尔值转换为"On"或"Off"文本显示
    /// </summary>
    public class BoolToOnOffConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// 提供转换器实例
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns>转换器实例</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <summary>
        /// 将布尔值转换为开/关文本
        /// </summary>
        /// <param name="value">源值（布尔值）</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">转换参数</param>
        /// <param name="culture">文化信息</param>
        /// <returns>转换后的文本（"On"或"Off"）</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? "On" : "Off";
            return "Off";
        }

        /// <summary>
        /// 将开/关文本转换回布尔值
        /// </summary>
        /// <param name="value">源值（文本）</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">转换参数</param>
        /// <param name="culture">文化信息</param>
        /// <returns>转换后的布尔值</returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                if (bool.TryParse(s, out var b))
                    return b;

                if (string.Equals(s, "On", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (string.Equals(s, "Off", StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return false;
        }
    }
}
