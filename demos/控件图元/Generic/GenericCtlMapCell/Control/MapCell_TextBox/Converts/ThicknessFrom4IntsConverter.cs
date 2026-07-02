using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia;
using Avalonia.Markup.Xaml;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox.Converts
{
    /// <summary>
    /// 多值转换：四个数值（左/上/右/下） -> Avalonia Thickness
    /// </summary>
    public class ThicknessFrom4IntsConverter : MarkupExtension, IMultiValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Count < 4)
                return new Thickness(0);

            static double ToDouble(object v)
            {
                try
                {
                    // 兼容属性面板可能产生的 int/double/float/decimal/string 等输入
                    if (v is int i) return i;
                    if (v is double d) return d;
                    if (v is float f) return f;
                    if (v is decimal m) return (double)m;
                    if (v is string s && double.TryParse(s, out var sd)) return sd;
                }
                catch
                {
                }
                return 0;
            }

            var left = Math.Max(0, ToDouble(values[0]));
            var top = Math.Max(0, ToDouble(values[1]));
            var right = Math.Max(0, ToDouble(values[2]));
            var bottom = Math.Max(0, ToDouble(values[3]));
            // 统一对负值做裁剪，避免异常布局
            return new Thickness(left, top, right, bottom);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
