using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.Converters
{
    /// 第一行转可见性转换器
    /// /如果是第一行（SerialNumber == 1），返回 false隐藏，否则返回 true展示
    public class FirstRowToVisibilityConverter : IValueConverter
    {

        public object? Convert(object ? value,Type targetType,object? parameter,CultureInfo culture)
        {
            if(value is int SerialNumber)
            {
                return SerialNumber!=1;
            }
            return true;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
