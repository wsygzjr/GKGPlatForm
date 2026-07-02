using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;

namespace GKG.Map.DataMonitorFuncCtlMapCell
{
    /// <summary>
    /// 水滴状态图片转换器
    /// 根据布尔值状态切换显示水滴或关闭图片
    /// true显示水滴图片，false显示关闭图片
    /// </summary>
    public class WaterToCloseImageConverter : IValueConverter
    {
        public static readonly WaterToCloseImageConverter Instance = new WaterToCloseImageConverter();

        private static Bitmap _waterBitmap;
        private static Bitmap _closeBitmap;
        private static bool _initialized = false;
        private static bool _loadFailed = false;

        private static void Initialize()
        {
            if (_initialized) return;
            
            try
            {
                // 加载水滴图片
                var waterUri = new Uri("avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/Water.png");
                using (var waterStream = AssetLoader.Open(waterUri))
                {
                    _waterBitmap = new Bitmap(waterStream);
                }

                // 加载关闭图片
                var closeUri = new Uri("avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/Close.png");
                using (var closeStream = AssetLoader.Open(closeUri))
                {
                    _closeBitmap = new Bitmap(closeStream);
                }
                
                _initialized = true;
            }
            catch (Exception ex)
            {
                // 如果加载失败，使用默认的空图片
                _waterBitmap = CreateDefaultBitmap(true);
                _closeBitmap = CreateDefaultBitmap(false);
                _initialized = true;
                _loadFailed = true;
            }
        }

        private static Bitmap CreateDefaultBitmap(bool isWater)
        {
            try
            {
                // 创建一个简单的默认图片
                var bitmap = new RenderTargetBitmap(new PixelSize(20, 20), new Vector(96, 96));
                using (var drawingContext = bitmap.CreateDrawingContext())
                {
                    var brush = isWater ? 
                        new SolidColorBrush(Color.FromArgb(255, 0, 120, 215)) :  // 蓝色表示水滴
                        new SolidColorBrush(Color.FromArgb(255, 215, 0, 0));     // 红色表示关闭
                    
                    drawingContext.DrawRectangle(brush, null, new Rect(0, 0, 20, 20));
                }
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Initialize();
            
            if (value is bool status)
            {
                // 确保图片已加载，如果没有则使用默认图片
                if (status && _waterBitmap != null)
                    return _waterBitmap;
                else if (!status && _closeBitmap != null)
                    return _closeBitmap;
                else
                    return CreateDefaultBitmap(true); // 默认水滴图片
            }
            // 如果图片都加载失败，创建默认图片
            if (_waterBitmap == null && _closeBitmap == null)
                return CreateDefaultBitmap(true);
            else
                return _waterBitmap ?? CreateDefaultBitmap(true); // 默认返回水滴图片
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
