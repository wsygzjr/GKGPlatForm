using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using GKG.Map.MapCell.Generic.Control.MapCell_TextBox;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox.Converts
{
    /// <summary>
    /// 字体族名称字符串 -> Avalonia FontFamily
    /// </summary>
    public class StringToFontFamilyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TextBoxFontFamilyType ft)
            {
                var name = ft switch
                {
                    TextBoxFontFamilyType.微软雅黑 => "Microsoft YaHei",
                    TextBoxFontFamilyType.宋体 => "SimSun",
                    TextBoxFontFamilyType.黑体 => "SimHei",
                    TextBoxFontFamilyType.英文_SegoeUI => "Segoe UI",
                    TextBoxFontFamilyType.英文_Arial => "Arial",
                    TextBoxFontFamilyType.等宽_Consolas => "Consolas",
                    TextBoxFontFamilyType.英文_TimesNewRoman => "Times New Roman",
                    _ => string.Empty
                };

                if (!string.IsNullOrWhiteSpace(name))
                    return new FontFamily(name);
                return FontFamily.Default;
            }
            // 空值回退到默认字体，避免 FontFamily 构造异常
            if (value is string s && !string.IsNullOrWhiteSpace(s))
                return new FontFamily(s);
            return FontFamily.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
