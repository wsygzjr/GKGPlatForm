using Avalonia.Data.Converters;
using GKG.UI;
using GKG.UI.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.Converters
{
    public class IODeviceIDToNameConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null || values.Count < 2 || values[0] == null || values[1] == null)
                return string.Empty;

            var itemsSource = values[0] as IEnumerable;
            var selectedValue = values[1]?.ToString();

            if (itemsSource == null || string.IsNullOrEmpty(selectedValue))
                return string.Empty;

            foreach (var item in itemsSource)
            {
                if (item is not ComBoxItem cb)
                    continue;

                var propValue = cb.Value?.ToString();
                var propName = cb.DisplayName;

                if (propValue == selectedValue)
                    return propName ?? selectedValue;
            }

            return selectedValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("反向转换未实现");
        }
    }
}
