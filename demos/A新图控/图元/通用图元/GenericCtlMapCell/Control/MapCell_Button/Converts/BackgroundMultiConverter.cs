using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Griffins.Map.UI;
using Griffins.UI2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.Map.CtlMapCell.Generic.Convert
{
	/// <summary>
	/// 背景混合转换器（优先图片，无图片则显示颜色）
	/// </summary>
	public class BackgroundMultiConverter : MarkupExtension, IMultiValueConverter
	{
		// 实现 MarkupExtension 必需的 ProvideValue 方法（返回自身实例）
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this; // 直接返回转换器实例，供 XAML 调用
		}

		public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
		{
			if (values[0] is BitmapData bitmapData && bitmapData.Bitmap != null)
			{
				// 有图片：创建 ImageBrush 并应用拉伸模式
				var imageBrush = new BitmapDataToImageBrushConverter().Convert(bitmapData, targetType, parameter, culture) as ImageBrush;
				if (imageBrush != null && values[2] is ImageSizeMode sizeMode)
				{
					imageBrush.Stretch = (Stretch)new ImageSizeModeToStretchConverter().Convert(sizeMode, targetType, parameter, culture)!;
				}
				return imageBrush;
			}
			else
			{
				// 无图片：显示背景色
				if (values[1] is Color color)
				{
					return new SolidColorBrush(color);
				}
			}

			return new SolidColorBrush(Colors.White); // 默认白色背景
		}

		public IList<object?> ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
		{
			// 反向转换不需要（背景通过 ViewModel 设置，而非 UI 直接修改）
			return new object?[3];
		}
	}

	/// <summary>
	/// ImageSizeMode 枚举扩展（供 ComboBox 绑定数据源）
	/// </summary>
	public static class ImageSizeModeExtensions
	{
		public static List<ImageSizeMode> AllValues { get; } = Enum.GetValues<ImageSizeMode>().ToList();
	}
}
