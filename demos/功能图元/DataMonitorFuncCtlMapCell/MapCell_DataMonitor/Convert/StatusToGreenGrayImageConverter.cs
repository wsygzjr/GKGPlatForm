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
    /// 状态图标转换器
    /// 根据布尔值状态切换显示Green或Gray图标
    /// true显示Green图标，false显示Gray图标
    /// </summary>
    public class StatusToGreenGrayImageConverter : IValueConverter
    {
        public static readonly StatusToGreenGrayImageConverter Instance = new StatusToGreenGrayImageConverter();

        private static Bitmap _greenBitmap;
        private static Bitmap _grayBitmap;
        private static bool _initialized = false;
        private static bool _loadFailed = false;

        private static void Initialize()
        {
            if (_initialized) return;
            
            try
            {
                // 加载Green图标
                var greenUri = new Uri("avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/Green.png");
                using (var greenStream = AssetLoader.Open(greenUri))
                {
                    _greenBitmap = new Bitmap(greenStream);
                }

                // 加载Gray图标
                var grayUri = new Uri("avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/Gray.png");
                using (var grayStream = AssetLoader.Open(grayUri))
                {
                    _grayBitmap = new Bitmap(grayStream);
                }
                
                _initialized = true;
            }
            catch (Exception ex)
            {
                // 如果加载失败，使用默认的空图片
                _greenBitmap = CreateDefaultBitmap(true);
                _grayBitmap = CreateDefaultBitmap(false);
                _initialized = true;
                _loadFailed = true;
            }
        }

        private static Bitmap CreateDefaultBitmap(bool isGreen)
        {
            try
            {
                // 创建一个简单的默认图片
                var bitmap = new RenderTargetBitmap(new PixelSize(20, 20), new Vector(96, 96));
                using (var drawingContext = bitmap.CreateDrawingContext())
                {
                    var brush = isGreen ? 
                        new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)) :   // 绿色表示Green
                        new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));   // 灰色表示Gray
                    
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
                if (status && _greenBitmap != null)
                    return _greenBitmap;
                else if (!status && _grayBitmap != null)
                    return _grayBitmap;
                else
                    return CreateDefaultBitmap(true); // 默认Green图标
            }
            // 如果图片都加载失败，创建默认图片
            if (_greenBitmap == null && _grayBitmap == null)
                return CreateDefaultBitmap(true);
            else
                return _greenBitmap ?? CreateDefaultBitmap(true); // 默认返回Green图标
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
