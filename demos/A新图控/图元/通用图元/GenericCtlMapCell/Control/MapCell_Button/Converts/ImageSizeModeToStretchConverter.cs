using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.Map.CtlMapCell.Generic.Convert
{
	/// <summary>
	/// ImageSizeMode → Stretch 转换器（ViewModel 枚举映射到 UI 拉伸模式）
	/// </summary>
	public class ImageSizeModeToStretchConverter : MarkupExtension, IValueConverter
	{
		// 实现 MarkupExtension 必需的 ProvideValue 方法（返回自身实例）
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this; // 直接返回转换器实例，供 XAML 调用
		}

		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return value switch
			{
				ImageSizeMode.Fill => Stretch.Fill,
				ImageSizeMode.Uniform => Stretch.Uniform,
				ImageSizeMode.UniformToFill => Stretch.UniformToFill,
				ImageSizeMode.None => Stretch.None,
				_ => Stretch.None
			};
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return value switch
			{
				Stretch.Fill => ImageSizeMode.Fill,
				Stretch.Uniform => ImageSizeMode.Uniform,
				Stretch.UniformToFill => ImageSizeMode.UniformToFill,
				Stretch.None => ImageSizeMode.None,
				_ => ImageSizeMode.None
			};
		}
	}
}
