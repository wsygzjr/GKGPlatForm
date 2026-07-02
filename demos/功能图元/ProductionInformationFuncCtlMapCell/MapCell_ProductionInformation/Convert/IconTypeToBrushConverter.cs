using Avalonia.Data.Converters;
using Avalonia.Media;
using GKG.Map.ProductionInformationFuncCtlMapCell.Models;
using System;
using System.Globalization;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.Convert
{
    /// <summary>
    /// 图标类型 → 画刷颜色 转换器 (纯净单例版)
    /// </summary>
    public class IconTypeToBrushConverter : IValueConverter
    {
        /// <summary>
        /// 静态全局单例，供 XAML 直接静态挂载
        /// </summary>
        public static readonly IconTypeToBrushConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is MessageDialogIconType type)
            {
                // 采用底层具备哈希跳转优化的 switch 表达式，直达目标
                return type switch
                {
                    MessageDialogIconType.Alarm => Brushes.Red,
                    MessageDialogIconType.Tip => Brushes.DodgerBlue,
                    MessageDialogIconType.Question => Brushes.DodgerBlue,
                    _ => Brushes.Gray
                };
            }

            return Brushes.Transparent;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("不支持反向转换");
        }
    }
}