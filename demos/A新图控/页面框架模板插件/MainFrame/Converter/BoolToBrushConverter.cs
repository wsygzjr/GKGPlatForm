using System.Globalization;

namespace MainFrame.Converter
{
    /// <summary>
    /// 辅助转换器：布尔值转背景色（选中行高亮）
    /// </summary>
    public class BoolToBrushConverter : Avalonia.Data.Converters.IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // parameter格式："选中色,未选中色"（如"#E8F4FD,#FFFFFF"）
            if (value is bool isSelected && parameter is string brushStr)
            {
                var brushArray = brushStr.Split(',');
                if (brushArray.Length == 2)
                {
                    return isSelected
                        ? Avalonia.Media.Brush.Parse(brushArray[0])
                        : Avalonia.Media.Brush.Parse(brushArray[1]);
                }
            }
            return Avalonia.Media.Brushes.White;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
