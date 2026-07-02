using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.Map.CtlMapCell.Generic.Container.Converts
{
	/// <summary>
	/// bool转边框厚度转换器：true→指定厚度，false→0厚度（隐藏边框）
	/// </summary>
	public class BooleanToBorderThicknessConverter : IValueConverter
	{
		/// <summary>
		/// 显示边框时的厚度（可通过XAML参数配置）
		/// </summary>
		public Thickness VisibleThickness { get; set; } = new Thickness(1); // 默认1像素边框

		/// <summary>
		/// 隐藏边框时的厚度（固定为0）
		/// </summary>
		public Thickness HiddenThickness { get; set; } = new Thickness(0);

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// 若值为true→显示边框（VisibleThickness），否则隐藏（HiddenThickness）
			return value is bool isShow && isShow ? VisibleThickness : HiddenThickness;
		}

		// 反向转换（无需实现，返回null即可）
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
