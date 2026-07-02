using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ComboBox.Converts
{
    public class StringToFontFamilyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 直接将 string 类型的字体名称转换为 Avalonia 的 FontFamily
            if (value is string fontName && !string.IsNullOrWhiteSpace(fontName))
            {
                try
                {
                    return new FontFamily(fontName);
                }
                catch
                {
                    // 如果输入的字体系统不支持，回退到默认字体
                    return FontFamily.Default;
                }
            }
            return FontFamily.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FontFamily fontFamily)
            {
                return fontFamily.Name;
            }
            return string.Empty;
        }
    }
}