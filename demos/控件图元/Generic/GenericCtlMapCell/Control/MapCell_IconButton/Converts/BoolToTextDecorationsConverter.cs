using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts
{
    /// <summary>
    /// bool → TextDecorations 转换器（将布尔值转换为下划线文本装饰）
    /// </summary>
    public class BoolToTextDecorationsConverter : MarkupExtension, IValueConverter
    {
        // 实现 MarkupExtension 必需的 ProvideValue 方法（返回自身实例）
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this; // 直接返回转换器实例，供 XAML 调用
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isUnderline && isUnderline)
            {
                return TextDecorations.Underline;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TextDecorationCollection decorations && decorations.Count > 0 && decorations[0].Location == TextDecorationLocation.Underline)
            {
                return true;
            }
            return false;
        }
    }
}