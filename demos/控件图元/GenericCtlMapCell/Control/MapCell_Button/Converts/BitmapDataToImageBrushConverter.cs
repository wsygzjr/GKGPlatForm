using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Griffins.UI2;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.Map.CtlMapCell.Generic.Convert
{
	/// <summary>
	/// BitmapData → ImageBrush 转换器（ViewModel 是 BitmapData，UI 是 ImageBrush）
	/// </summary>
	public class BitmapDataToImageBrushConverter :MarkupExtension, IValueConverter
	{
		// 实现 MarkupExtension 必需的 ProvideValue 方法（返回自身实例）
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this; // 直接返回转换器实例，供 XAML 调用
		}

		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is BitmapData bitmapData && bitmapData.Bitmap != null)
			{
				try
				{
					return new ImageBrush()
					{
						Source= (Bitmap)bitmapData.Bitmap,
						Stretch = Stretch.None, // 默认不拉伸，后续通过 ImageSizeMode 调整
						AlignmentX = AlignmentX.Center,
						AlignmentY = AlignmentY.Center
					};
				}
				catch
				{
					return null;
				}
			}
			return null;
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			// 双向绑定反向转换：一般不需要（图片通过文件选择器设置，而非 UI 直接修改 ImageBrush）
			return null;
		}
	}
}
