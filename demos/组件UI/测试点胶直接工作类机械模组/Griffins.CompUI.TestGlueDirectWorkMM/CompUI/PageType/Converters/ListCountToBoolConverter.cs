// Converters/ListCountToBoolConverter.cs
using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Griffins.CompUI.TestGlueDirectWorkMM.Converters
{
    /// <summary>
    /// 列表数量转布尔值转换器：用于控制删除按钮禁用状态
    /// 逻辑：当列表总数 > 1 时返回true（启用），否则返回false（禁用）
    /// </summary>
    public class ListCountToBoolConverter : IMultiValueConverter
    {
        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            // 校验输入：第一个值必须是列表总数（int类型）
            if (values.Count < 1 || values[0] == null )
                return false;

            if (int.TryParse(values[0]!.ToString(), out int listCount))
            {
                // 核心逻辑：列表数量 > 1 时启用删除按钮
                return listCount > 1;
            }

            return false;
        }

        public IList<object?> ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ListCountToBoolConverter不支持反向转换");
        }
    }
}