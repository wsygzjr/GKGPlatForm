using Avalonia.Data;
using Avalonia.Data.Converters;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.Map.UIDataObjProcessDesignTime.ProductionInfo.Converts
{
	internal class MMAliasConverter : IValueConverter
	{
		public static MMAliasConverter Instance { get; } = new();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is MMAlias alias)
			{
				return alias.ToString();
			}
			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				if (value is string str && !string.IsNullOrWhiteSpace(str))
				{
					return MMAlias.Parse(str);
				}
				return default(MMAlias);
			}
			catch
			{
				return BindingOperations.DoNothing;
			}
		}
	}
}
