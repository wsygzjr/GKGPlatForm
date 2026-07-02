using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace GKG.Map.MapCell.Generic.Control.Lable.Converts
{
    public class StringToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                try
                {
                    // Avalonia 原生的 Parse 方法，支持 "1" 或 "1,2,3,4" 等格式
                    return Thickness.Parse(strValue);
                }
                catch
                {
                    return new Thickness(0);
                }
            }
            return new Thickness(0); // 默认回退值为 0
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                return thickness.ToString();
            }
            return "0";
        }
    }
}