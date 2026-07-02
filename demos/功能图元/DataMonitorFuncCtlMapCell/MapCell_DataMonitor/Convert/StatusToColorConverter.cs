using Avalonia.Media;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace GKG.Map.DataMonitorFuncCtlMapCell
{
    /// <summary>
    /// 状态到颜色转换器
    /// 根据布尔值状态返回对应的背景颜色
    /// true返回绿色，false返回灰色
    /// </summary>
    public class StatusToColorConverter : IValueConverter
    {
        public static readonly StatusToColorConverter Instance = new StatusToColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool status)
            {
                // false显示Water图片时返回绿色，true显示Close图片时返回灰色
                // 使用更鲜艳的颜色确保可见
                // 将Color转换为SolidColorBrush以满足IBrush类型要求
                return status ? 
                    new SolidColorBrush(Colors.Gray) :           // Close状态 - 灰色
                    new SolidColorBrush(Colors.LimeGreen);      // Water状态 - 绿色
            }
            return new SolidColorBrush(Colors.LimeGreen); // 默认绿色（Water状态）
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
