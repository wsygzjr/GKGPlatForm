using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.Converters
{
    public class CustomBoolToColorConverter : IValueConverter
    {
        private readonly Color _defaultTrueColor = Color.Parse("#3498db");
        private readonly Color _defaultFalseColor = Color.Parse("#e67e22");

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
            {
                return new SolidColorBrush(_defaultFalseColor);
            }

            var (trueColor, falseColor) = ParseParameter(parameter);
            return new SolidColorBrush(boolValue ? trueColor : falseColor);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }

        private (Color TrueColor, Color FalseColor) ParseParameter(object? parameter)
        {
            Color trueColor = _defaultTrueColor;
            Color falseColor = _defaultFalseColor;

            if (parameter == null || string.IsNullOrEmpty(parameter.ToString()))
            {
                return (trueColor, falseColor);
            }

            try
            {
                string[] colorParams = parameter.ToString()!.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (colorParams.Length >= 1)
                {
                    trueColor = Color.Parse(colorParams[0].Trim());
                }
                if (colorParams.Length >= 2)
                {
                    falseColor = Color.Parse(colorParams[1].Trim());
                }
            }
            catch
            {
            }

            return (trueColor, falseColor);
        }
    }
}
