using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Globalization;

namespace GKG.Map.MapCell.Generic.Control.Lable.Converts
{
    // 1. 鼠标样式转换器
    public class CursorTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CursorType ct)
                return new Cursor((StandardCursorType)(int)ct); // 枚举值已经完美对齐了 Avalonia 标准游标
            return Cursor.Default;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // 2. 水平对齐转换器
    public class HorizontalAlignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is HorizontalAlignType ha)
            {
                return ha switch
                {
                    HorizontalAlignType.Left => HorizontalAlignment.Left,
                    HorizontalAlignType.Center => HorizontalAlignment.Center,
                    HorizontalAlignType.Right => HorizontalAlignment.Right,
                    _ => HorizontalAlignment.Stretch,
                };
            }
            return HorizontalAlignment.Stretch;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // 3. 垂直对齐转换器
    public class VerticalAlignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VerticalAlignType va)
            {
                return va switch
                {
                    VerticalAlignType.Top => VerticalAlignment.Top,
                    VerticalAlignType.Center => VerticalAlignment.Center,
                    VerticalAlignType.Bottom => VerticalAlignment.Bottom,
                    _ => VerticalAlignment.Stretch,
                };
            }
            return VerticalAlignment.Stretch;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // 4. 文本对齐转换器
    public class TextAlignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TextAlignType ta)
            {
                return ta switch
                {
                    TextAlignType.Left => TextAlignment.Left,
                    TextAlignType.Center => TextAlignment.Center,
                    TextAlignType.Right => TextAlignment.Right,
                    TextAlignType.Justify => TextAlignment.Justify, // 两端对齐
                    _ => TextAlignment.Left,
                };
            }
            return TextAlignment.Left;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // 5. 下划线转换器 (顺手帮你补上的)
    public class BoolToTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b ? TextDecorations.Underline : null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}