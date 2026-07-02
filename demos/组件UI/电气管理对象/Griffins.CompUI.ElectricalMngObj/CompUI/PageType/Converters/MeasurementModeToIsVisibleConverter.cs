//// Converters/ListCountToBoolConverter.cs
//using Avalonia.Data.Converters;
//using PageTypeSimple.RecipeParamCfgPage.RecipeParam.Models.TemplateSubPage.Command;
//using System.Globalization;

//namespace Griffins.CompUI.ElectricalMngObj.Converters
//{
//    /// <summary>
//    ///  测高模式选中三点时，为true状态 
//    /// </summary>
//    public class MeasurementModeToIsVisibleConverter : IValueConverter
//    {
//        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
//        {
//            // 1. 校验输入：必须是MarkMode类型（防止绑定错误）
//            if (value is not HeightMeasurementMode selectedMode)
//                return false; // 类型错误时默认禁用

//            // 2. 核心逻辑（按需修改！） 
//            if(selectedMode == HeightMeasurementMode.ThreePoint)
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