using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace GKG.Map.CommunicationFuncCtlMapCell.Convert;

public sealed class DispenseModeIntToBoolConverter : IValueConverter
{
	public static readonly DispenseModeIntToBoolConverter Instance = new();

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is null || parameter is null)
			return false;

		if (!TryGetInt(value, out var v) || !TryGetInt(parameter, out var p))
			return false;

		return v == p;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is true && parameter is not null && TryGetInt(parameter, out var p))
			return p;

		return BindingOperations.DoNothing;
	}

	private static bool TryGetInt(object value, out int result)
	{
		switch (value)
		{
			case int i:
				result = i;
				return true;
			case string s when int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var si):
				result = si;
				return true;
			default:
				result = default;
				return false;
		}
	}
}
