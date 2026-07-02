using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.Converter
{
    public class ScaleToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double scale)
            {
                // 根据进度比例返回不同的颜色画刷
                if (scale < 0.5)
                    return Brushes.LimeGreen; // 安全：绿色
                else if (scale < 0.8)
                    return Brushes.Orange;    // 警告：橙色
                else
                    return Brushes.Red;       // 危险/过期：红色
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}