using Avalonia.Data.Converters;

namespace NonMainFrameView.Converter
{
    /// <summary>
    /// 无分隔符转换器
    /// </summary>
    public class IndexToVisibilityConverter : IValueConverter
    {
        public static readonly IndexToVisibilityConverter NotLastItem = new IndexToVisibilityConverter();

        public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int index && parameter is int count)
            {
                return index < count - 1;
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
