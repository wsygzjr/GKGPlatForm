//// Converters/ListCountToBoolConverter.cs
//using Avalonia.Data.Converters;
//using System.Globalization;
//using System;

//namespace Griffins.CompUI.ElectricalMngObj.Converters
//{
//    /// <summary>
//    /// Mark方式选中定位补偿时，为true状态
//    /// </summary>
//    public class MarkModeToIsVisibleConverter : IValueConverter
//    {
//        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
//        {
//            // 1. 校验输入：必须是MarkMode类型（防止绑定错误）
//            if (value is not MarkMode selectedMarkMode)
//                return false; // 类型错误时默认禁用

//            // 2. 核心逻辑（按需修改！） 
//            if(selectedMarkMode == MarkMode.PositionCompensation)
//                return true;
//            else
//                return false;
//        }

//        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
//        {
//            return value;
//        }
//    }
//} 