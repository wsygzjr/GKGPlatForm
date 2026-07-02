using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Globalization;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.Convert
{
	/// <summary>
	/// Color → SolidColorBrush 转换器（ViewModel 是 Color，UI 是 Brush）
	/// </summary>
	public class ColorToBrushConverter : MarkupExtension, IValueConverter
	{
		// 实现 MarkupExtension 必需的 ProvideValue 方法（返回自身实例）
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this; // 直接返回转换器实例，供 XAML 调用
		}

		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is Color color)
			{
				return new SolidColorBrush(color);
			}
			return new SolidColorBrush(Colors.Transparent); // 默认透明
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is SolidColorBrush brush)
			{
				return brush.Color;
			}
			return Colors.Transparent;
		}
	}
}
