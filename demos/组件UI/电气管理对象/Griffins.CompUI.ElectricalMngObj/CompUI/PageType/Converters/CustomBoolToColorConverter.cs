using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Griffins.CompUI.ElectricalMngObj.Converters
{
    /// <summary>
    /// 布尔值转颜色转换器（支持自定义True/False颜色参数）
    /// 参数格式："True颜色值,False颜色值"（如 "#00FF00,#FF0000" 或 "Green,Red"）
    /// </summary>
    public class CustomBoolToColorConverter : IValueConverter
    {
        // 默认颜色（参数未传递/解析失败时使用）
        private readonly Color _defaultTrueColor = Color.Parse("#3498db"); // 蓝色
        private readonly Color _defaultFalseColor = Color.Parse("#e67e22"); // 橙色

        /// <summary>
        /// 转换逻辑（核心：接收value(布尔值) + parameter(自定义颜色参数)）
        /// </summary>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // 1. 校验输入值是否为布尔类型
            if (value is not bool boolValue)
            {
                // 非布尔值返回默认色（或抛出异常，根据需求调整）
                return new SolidColorBrush(_defaultFalseColor);
            }

            // 2. 解析自定义参数（格式："TrueColor,FalseColor"）
            (Color trueColor, Color falseColor) = ParseParameter(parameter);

            // 3. 根据布尔值返回对应颜色的画刷（FillColor需Brush类型）
            return new SolidColorBrush(boolValue ? trueColor : falseColor);
        }

        /// <summary>
        /// 反向转换（一般无需实现，返回未实现异常）
        /// </summary> 
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }

        /// <summary>
        /// 解析转换器参数，提取True/False对应的颜色
        /// </summary>
        private (Color TrueColor, Color FalseColor) ParseParameter(object? parameter)
        {
            // 默认使用内置颜色
            Color trueColor = _defaultTrueColor;
            Color falseColor = _defaultFalseColor;

            // 无参数时直接返回默认值
            if (parameter == null || string.IsNullOrEmpty(parameter.ToString()))
            {
                return (trueColor, falseColor);
            }

            try
            {
                // 拆分参数（按逗号分割）
                string[] colorParams = parameter.ToString()!.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (colorParams.Length >= 1)
                {
                    // 解析True颜色（支持十六进制/颜色名，如 "#00FF00" 或 "Green"）
                    trueColor = Color.Parse(colorParams[0].Trim());
                }
                if (colorParams.Length >= 2)
                {
                    // 解析False颜色
                    falseColor = Color.Parse(colorParams[1].Trim());
                }
            }
            catch (Exception ex)
            {
                // 参数解析失败（如颜色格式错误），输出日志并使用默认色
                Console.WriteLine($"CustomBoolToColorConverter 参数解析失败：{ex.Message}");
            }

            return (trueColor, falseColor);
        }
    }
}