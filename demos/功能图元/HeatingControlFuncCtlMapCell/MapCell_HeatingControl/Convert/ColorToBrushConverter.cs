using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Globalization;

namespace GKG.Map.HeatingControlFuncCtlMapCell.Convert
{
    /// <summary>
    /// 颜色到画刷转换器
    /// 将Color对象转换为SolidColorBrush对象用于UI显示
    /// </summary>
    public class ColorToBrushConverter : MarkupExtension, IValueConverter
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
        /// 将颜色转换为画刷
        /// </summary>
        /// <param name="value">源值（Color对象）</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">转换参数</param>
        /// <param name="culture">文化信息</param>
        /// <returns>转换后的画刷对象</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// 将画刷转换回颜色
        /// </summary>
        /// <param name="value">源值（画刷对象）</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">转换参数</param>
        /// <param name="culture">文化信息</param>
        /// <returns>转换后的颜色对象</returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color;
            }
            return Colors.Transparent;
        }
    }
}
