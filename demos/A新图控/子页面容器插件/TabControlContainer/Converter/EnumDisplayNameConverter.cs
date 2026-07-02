using Avalonia.Data;
using Avalonia.Data.Converters;
using PropertyModels.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace GKG.Map.Page.UIContainer.TabControlContainer.Converter
{
    /// <summary>
    /// 用于将枚举值转换为其EnumDisplayName特性值的转换器
    /// </summary>
    public class EnumDisplayNameConverter : IValueConverter
    {
        /// <summary>
        /// 将枚举值转换为EnumDisplayName特性的值
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">文化信息</param>
        /// <returns>转换后的显示名称，若无法转换则返回原始值的字符串表示</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            // 检查值是否为枚举类型
            if (value is not Enum enumValue)
                return value.ToString();

            // 获取枚举成员信息
            MemberInfo[] memberInfo = enumValue.GetType().GetMember(enumValue.ToString());
            if (memberInfo.Length == 0)
                return enumValue.ToString();

            // 获取EnumDisplayName特性
            EnumDisplayNameAttribute? attribute = memberInfo[0]
                .GetCustomAttributes(typeof(EnumDisplayNameAttribute), false)
                .FirstOrDefault() as EnumDisplayNameAttribute;

            // 返回特性值或枚举名称
            return attribute?.DisplayName ?? enumValue.ToString();
        }

        /// <summary>
        /// 转换方法的逆过程（从显示名称转换回枚举值）
        /// </summary>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || !targetType.IsEnum)
                return BindingOperations.DoNothing;

            // 遍历枚举所有值查找匹配的显示名称
            foreach (Enum enumValue in Enum.GetValues(targetType))
            {
                MemberInfo[] memberInfo = targetType.GetMember(enumValue.ToString());
                if (memberInfo.Length == 0)
                    continue;

                EnumDisplayNameAttribute? attribute = memberInfo[0]
                    .GetCustomAttributes(typeof(EnumDisplayNameAttribute), false)
                    .FirstOrDefault() as EnumDisplayNameAttribute;

                // 找到匹配的显示名称
                if (attribute?.DisplayName == value.ToString())
                {
                    return enumValue;
                }
            }

            // 尝试直接通过名称匹配
            if (Enum.TryParse(targetType, value.ToString(), out object? result))
            {
                return result;
            }

            return BindingOperations.DoNothing;
        }
    }
}
