using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.Converters
{
    public class MotionControlAxisHomingModeToCnConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is GKG.MotionControlAxisHomingMode mode)
            {
                return mode switch
                {
                    GKG.MotionControlAxisHomingMode.OnceOriginGoHome => "原点回零(一次)",
                    GKG.MotionControlAxisHomingMode.TwiceOriginGoHome => "原点回零(二次)",
                    GKG.MotionControlAxisHomingMode.NegativeGoHome => "负限位回零",
                    GKG.MotionControlAxisHomingMode.EZGoHome => "EZ回零",
                    GKG.MotionControlAxisHomingMode.FindEZStop => "找EZ停止",
                    GKG.MotionControlAxisHomingMode.FindEZLatchBack => "找EZ锁存回找",
                    _ => mode.ToString(),
                };
            }

            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value ?? Avalonia.Data.BindingOperations.DoNothing;
        }
    }
}
