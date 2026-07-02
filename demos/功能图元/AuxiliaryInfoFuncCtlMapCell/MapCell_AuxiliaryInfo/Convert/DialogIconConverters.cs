using Avalonia.Data.Converters;
using Avalonia.Media;
using GKG.Map.AuxiliaryInfoFuncCtlMapCell.ViewModel;
using System;
using System.Globalization;

namespace GKG.Map.AuxiliaryInfoFuncCtlMapCell.Convert
{
    /// <summary>
    /// 将 IconType 枚举转换为对应的 SVG 路径
    /// </summary>
    public class DialogIconGeometryConverter : IValueConverter
    {
        // 准备几个标准的 SVG 路径 (提取自你原来的代码或标准化图标)
        private static readonly Geometry CheckIcon = Geometry.Parse("M12,2A10,10 0 0,1 22,12A10,10 0 0,1 12,22A10,10 0 0,1 2,12A10,10 0 0,1 12,2M11,16.5L18,9.5L16.59,8.09L11,13.67L7.91,10.59L6.5,12L11,16.5Z");
        private static readonly Geometry WarningIcon = Geometry.Parse("M12,2L1,21H23M12,6L19.53,19H4.47M11,10V14H13V10M11,16V18H13V16");
        private static readonly Geometry AlarmIcon = Geometry.Parse("M12,2C6.47,2 2,6.47 2,12C2,17.53 6.47,22 12,22C17.53,22 22,17.53 22,12C22,6.47 17.53,2 12,2M11,7H13V13H11V7M11,15H13V17H11V15Z");
        private static readonly Geometry QuestionIcon = Geometry.Parse("M12,2A10,10 0 0,1 22,12A10,10 0 0,1 12,22A10,10 0 0,1 2,12A10,10 0 0,1 12,2M12,4A8,8 0 0,0 4,12A8,8 0 0,0 12,20A8,8 0 0,0 20,12A8,8 0 0,0 12,4M11,16H13V18H11V16M12,6C14.21,6 16,7.79 16,10C16,11.5 14.7,12.26 13.68,12.91C12.82,13.46 12.5,14 12.5,14.5H10.5C10.5,13.2 11.66,12.54 12.58,11.95C13.5,11.36 14,10.9 14,10C14,8.9 13.1,8 12,8C10.9,8 10,8.9 10,10H8C8,7.79 9.79,6 12,6Z");
        private static readonly Geometry HelpIcon = Geometry.Parse("M11,18H13V16H11V18M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,20C7.59,20 4,16.41 4,12C4,7.59 7.59,4 12,4C16.41,4 20,7.59 20,12C20,16.41 16.41,20 12,20M12,6A4,4 0 0,0 8,10H10A2,2 0 0,1 12,8A2,2 0 0,1 14,10C14,12 11,11.75 11,15H13C13,12.75 16,12.5 16,10A4,4 0 0,0 12,6Z");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DialogIconType iconType)
            {
                return iconType switch
                {
                    DialogIconType.Tip => CheckIcon,
                    DialogIconType.Warning => WarningIcon,
                    DialogIconType.Alarm => AlarmIcon,
                    DialogIconType.Question => QuestionIcon,
                    DialogIconType.Help => HelpIcon,
                    _ => CheckIcon
                };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    /// <summary>
    /// 将 IconType 枚举转换为对应的画刷颜色
    /// </summary>
    public class DialogIconColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DialogIconType iconType)
            {
                return iconType switch
                {
                    DialogIconType.Tip => Brush.Parse("#3B82F6"),       // 蓝色
                    DialogIconType.Warning => Brush.Parse("#F59E0B"),   // 警告黄
                    DialogIconType.Alarm => Brush.Parse("#EF4444"),     // 报警红
                    DialogIconType.Question => Brush.Parse("#10B981"),  // 疑问绿
                    DialogIconType.Help => Brush.Parse("#666666"),      // 帮助紫
                    _ => Brushes.Black
                };
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}