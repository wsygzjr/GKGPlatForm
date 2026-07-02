using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Globalization;

namespace Griffins.CompUI.GlueDirectWork.Converters
{
    public class CustomBoolToGeometryConverter : IValueConverter
    {
        private readonly string _defaultNormalKey = "CircleData_Normal";
        private readonly string _defaultErrorKey = "CircleData_Error";

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // 1. 校验输入值为 bool
            if (value is not bool isNormal)
            {
                Console.WriteLine($"输入值类型错误：{value?.GetType().Name ?? "null"}，预期 bool");
                return StreamGeometry.Parse("M0,0");
            }

            // 2. 解析转换器参数（自定义资源Key）
            var (normalKey, errorKey) = ParseParameter(parameter);

            // 3. 正确调用 TryGetResource（核心修复：补全 out 参数）
            object? pathDataObj;
            bool isResourceFound = Application.Current!.Resources.TryGetResource(
                isNormal ? normalKey : errorKey,  // 参数1：资源Key
                Application.Current.ActualThemeVariant, // 参数2：当前主题（也可传null）
                out pathDataObj                     // 参数3：out输出参数（必须加out）
            );

            // 4. 校验资源有效性
            if (isResourceFound && pathDataObj is string pathData)
            {
                return pathData; // 返回有效的 Path Data 字符串
            }
            else
            {
                string str = isNormal ? normalKey : errorKey;
                Console.WriteLine($"未找到资源 Key：{str}，或资源类型不是 string");
                // 兜底：返回默认圆形路径
                return StreamGeometry.Parse(_defaultNormalKey);
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("不支持反向转换");
        }

        private (string NormalKey, string ErrorKey) ParseParameter(object? parameter)
        {
            string normalKey = _defaultNormalKey;
            string errorKey = _defaultErrorKey;

            if (parameter is string param && !string.IsNullOrEmpty(param))
            {
                try
                {
                    var parts = param.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 1) normalKey = parts[0].Trim();
                    if (parts.Length >= 2) errorKey = parts[1].Trim();
                }
                catch
                {
                    // 参数解析失败，使用默认Key
                }
            }

            return (normalKey, errorKey);
        }
    }
}