using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Input;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox.Converts
{
    /// <summary>
    /// 属性面板的 CommonCursorType -> Avalonia Cursor
    /// </summary>
    public class CursorTypeToCursorConverter : IValueConverter
    {
        public static readonly CursorTypeToCursorConverter Instance = new CursorTypeToCursorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 将业务枚举映射到 Avalonia 标准光标类型
            if (value is CommonCursorType cursorType)
            {
                return new Cursor(cursorType.ToStandard());
            }
            return new Cursor(StandardCursorType.Arrow);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class CommonCursorTypeExtensions
    {
        public static StandardCursorType ToStandard(this CommonCursorType type)
        {
            // 统一处理业务枚举到 Avalonia 标准枚举的映射
            return type switch
            {
                CommonCursorType.默认箭头 => StandardCursorType.Arrow,
                CommonCursorType.文本输入 => StandardCursorType.Ibeam,
                CommonCursorType.等待 => StandardCursorType.Wait,
                CommonCursorType.十字 => StandardCursorType.Cross,
                CommonCursorType.向上箭头 => StandardCursorType.UpArrow,
                CommonCursorType.左右调整 => StandardCursorType.SizeWestEast,
                CommonCursorType.上下调整 => StandardCursorType.SizeNorthSouth,
                CommonCursorType.移动 => StandardCursorType.SizeAll,
                CommonCursorType.禁止 => StandardCursorType.No,
                CommonCursorType.手型 => StandardCursorType.Hand,
                CommonCursorType.启动中 => StandardCursorType.AppStarting,
                CommonCursorType.帮助 => StandardCursorType.Help,
                _ => StandardCursorType.Arrow
            };
        }
    }
}
