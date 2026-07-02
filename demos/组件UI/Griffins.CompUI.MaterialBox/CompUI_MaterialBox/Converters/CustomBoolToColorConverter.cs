using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.Converters
{
    /// <summary>
    /// 将布尔值转换成不同颜色画刷，常用于状态灯或状态文本显示。
    /// </summary>
    public class CustomBoolToColorConverter : IValueConverter
    {
        /// <summary>布尔值为 true 时使用的默认颜色。</summary>
        private readonly Color _defaultTrueColor = Color.Parse("#3498db");
        /// <summary>布尔值为 false 时使用的默认颜色。</summary>
        private readonly Color _defaultFalseColor = Color.Parse("#e67e22");

        /// <summary>
        /// 把布尔状态转换成颜色画刷，可通过参数传入自定义真值色与假值色。
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return new SolidColorBrush(_defaultFalseColor);

            var (trueColor, falseColor) = ParseParameter(parameter);
            return new SolidColorBrush(boolValue ? trueColor : falseColor);
        }

        /// <summary>
        /// 当前场景不需要把颜色反向转换成布尔值，直接返回空。
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        /// <summary>
        /// 解析转换器参数中的颜色配置，格式为“真值色,假值色”。
        /// </summary>
        private (Color TrueColor, Color FalseColor) ParseParameter(object parameter)
        {
            var trueColor = _defaultTrueColor;
            var falseColor = _defaultFalseColor;

            if (parameter == null || string.IsNullOrEmpty(parameter.ToString()))
                return (trueColor, falseColor);

            try
            {
                var colorParams = parameter.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (colorParams.Length >= 1)
                    trueColor = Color.Parse(colorParams[0].Trim());
                if (colorParams.Length >= 2)
                    falseColor = Color.Parse(colorParams[1].Trim());
            }
            catch
            {
            }

            return (trueColor, falseColor);
        }
    }
}
