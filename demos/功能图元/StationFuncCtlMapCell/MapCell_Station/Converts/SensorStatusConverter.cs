using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;

namespace GKG.Map.StationFuncCtlMapCell.Convert
{
    /// <summary>
    /// 感应器状态图像转换器 (单例模式)
    /// 将感应器的布尔状态转换为直观的圆形指示灯图像。
    /// </summary>
    public class SensorStatusConverter : IValueConverter
    {
        /// <summary>
        /// 静态全局单例，提高绑定性能
        /// </summary>
        public static readonly SensorStatusConverter Instance = new SensorStatusConverter();

        private static readonly Bitmap? _noBmp = LoadBitmap("ctr-No.png");
        private static readonly Bitmap? _yesBmp = LoadBitmap("ctr-Yes.png");

        private static Bitmap? LoadBitmap(string fileName)
        {
            try
            {
                return new Bitmap(AssetLoader.Open(new Uri("avares://GKG.Map.StationFuncCtlMapCell/Assets/Images/" + fileName)));
            }
            catch
            {
                return null;
            }
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool status)
            {
                return status ? _yesBmp : _noBmp;
            }
            return _noBmp;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("不支持反向转换");
        }
    }
}