using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace NonMainFrameView.Converter;

/// <summary>
/// 将集合的数量(int)转换为可见性(bool)
/// 当 Count > 0 时返回 true（显示），否则返回 false（隐藏）
/// </summary>
public class CountToIsVisibleConverter : IValueConverter
{
    // 提供一个静态单例，方便在 XAML 中直接引用（可选，但推荐）
    public static readonly CountToIsVisibleConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // 判断传入的值是否为 int 类型（即 Count）
        if (value is int count)
        {
            return count > 0;
        }
        return false; // 默认隐藏
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}