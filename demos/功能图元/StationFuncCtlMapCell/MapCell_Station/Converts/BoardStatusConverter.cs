using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;

namespace GKG.Map.StationFuncCtlMapCell.Convert
{
    /// <summary>
    /// 料板状态图像转换器 (单例模式)
    /// 根据料板是否在位的布尔状态，返回对应的高亮或默认工位图片。
    /// </summary>
    public class BoardStatusConverter : IValueConverter
    {
        /// <summary>
        /// 静态全局单例，避免重复创建实例，降低垃圾回收 (GC) 压力
        /// </summary>
        public static readonly BoardStatusConverter Instance = new();

        /// <summary>
        /// 默认状态位图（无料板）
        /// </summary>
        private static readonly Bitmap? _stationBmp = LoadBitmap("ctr-Station.png");

        /// <summary>
        /// 激活状态位图（有料板）
        /// </summary>
        private static readonly Bitmap? _stationActiveBmp = LoadBitmap("ctr-Station-active.png");

        /// <summary>
        /// 从程序集中加载本地图片资源
        /// </summary>
        /// <param name="fileName">图片文件名</param>
        /// <returns>成功返回 Bitmap 对象，失败返回 null</returns>
        private static Bitmap? LoadBitmap(string fileName)
        {
            try
            {
                // 使用字符串插值优化路径拼接
                return new Bitmap(AssetLoader.Open(new Uri($"avares://GKG.Map.StationFuncCtlMapCell/Assets/Images/{fileName}")));
            }
            catch
            {
                // 忽略资源加载异常，UI 引擎会自动进行优雅降级（显示为空白而不会引发程序崩溃）
                return null;
            }
        }

        /// <summary>
        /// 正向转换：bool 状态 -> 对应的 Bitmap 图片
        /// </summary>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool status)
            {
                return status ? _stationActiveBmp : _stationBmp;
            }

            // 容错处理：如果传入的不是 bool 类型，默认返回无料状态
            return _stationBmp;
        }

        /// <summary>
        /// 反向转换（不支持）
        /// </summary>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // 明确告知 UI 框架这是一个纯单向的转换器
            throw new NotSupportedException("不支持反向转换");
        }
    }
}