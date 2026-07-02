using Avalonia.Data.Converters;
using Griffins.UI;
using System.Globalization;
using System;
using System.Collections.Generic;

namespace Griffins.CompUI.ElectricalMngObj.Converters
{
    /// <summary>
    /// IO设备型号转名称转换器
    /// </summary>
    public class IODeviceIDToNameConverter : IMultiValueConverter
    {
        /// <summary>
        /// 转换逻辑：从ItemsSource中根据SelectedControlCardType匹配显示名称
        /// </summary>
        /// <param name="values">MultiBinding传入的参数数组：[0] = SelectedControlCardType, [1] = ItemsSource</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">附加参数</param>
        /// <param name="culture">区域信息</param>
        /// <returns>转换后的显示名称</returns> 
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            // 校验参数合法性
            if (values == null || values.Count < 2 || values[0] == null || values[1] == null)
                return string.Empty;

            var  itemsSource = values[0] as List<ComBoxItem> ;
            var selectedValue = values[1].ToString();

            if (itemsSource == null)
                return string.Empty;

            // 示例逻辑：遍历ItemsSource，根据SelectedControlCardType匹配名称
            // 需根据你的实际数据模型调整（比如ItemsSource是List<ControlCardModel>）
            foreach (var item in itemsSource)
            {
                // 假设你的数据模型有 Value 和 Name 属性
                var propValue = item.Value.ToString();
                var propName = item.DisplayName.ToString();

                if (propValue == selectedValue)
                    return propName ?? selectedValue;
            }

            // 无匹配项时返回原始值
            return selectedValue;
        }

        /// <summary>
        /// 反向转换（Avalonia中需实现，若无需反向转换则抛出异常）
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("反向转换未实现");
        }
    }
}