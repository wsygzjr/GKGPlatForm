using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.Converters
{
    public class ListCountToBoolConverter : IMultiValueConverter
    {
        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 1 || values[0] == null)
                return false;

            if (int.TryParse(values[0]!.ToString(), out int listCount))
            {
                return listCount > 1;
            }

            return false;
        }

        public IList<object?> ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ListCountToBoolConverter 不支持反向转换");
        }
    }
}
