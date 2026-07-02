#nullable enable
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;
using AvaloniaColor = Avalonia.Media.Color;

namespace GKG.Map.MapCell.Generic.Control.MapCell_IconButton.Converts
{
    /// <summary>
    /// BitmapData + 颜色 → 染色后的 ImageSource。
    /// </summary>
    public class BitmapDataAndColorToTintedImageSourceConverter : MarkupExtension, IMultiValueConverter
    {
        private static readonly ConcurrentDictionary<string, AvaloniaBitmap> Cache = new();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 2 || values[0] is not BitmapData bitmapData || bitmapData.Bitmap == null || values[1] is not AvaloniaColor tintColor)
            {
                return null;
            }

            try
            {
                byte[] sourceBytes = bitmapData.ToBytes();
                string cacheKey = $"{System.Convert.ToHexString(SHA256.HashData(sourceBytes))}_{tintColor.A:X2}{tintColor.R:X2}{tintColor.G:X2}{tintColor.B:X2}";
                return Cache.GetOrAdd(cacheKey, _ => CreateTintedBitmap(sourceBytes, tintColor));
            }
            catch
            {
                return bitmapData.Bitmap as AvaloniaBitmap;
            }
        }

        public object? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            return null;
        }

        private static AvaloniaBitmap CreateTintedBitmap(byte[] sourceBytes, AvaloniaColor tintColor)
        {
            using MemoryStream inputStream = new(sourceBytes);
            using AvaloniaBitmap sourceBitmap = new(inputStream);
            WriteableBitmap tintedBitmap = new(sourceBitmap.PixelSize, sourceBitmap.Dpi, PixelFormat.Bgra8888, AlphaFormat.Premul);

            using (ILockedFramebuffer framebuffer = tintedBitmap.Lock())
            {
                sourceBitmap.CopyPixels(framebuffer, AlphaFormat.Premul);

                int bufferLength = framebuffer.RowBytes * framebuffer.Size.Height;
                byte[] pixels = new byte[bufferLength];
                Marshal.Copy(framebuffer.Address, pixels, 0, bufferLength);

                // 1. 快速扫描检测：只要原图存在透明通道（有非全不透明的像素），就判定为抠好底的PNG图标
                bool hasAlphaChannel = false;
                for (int i = 3; i < bufferLength; i += 4)
                {
                    if (pixels[i] < 255)
                    {
                        hasAlphaChannel = true;
                        break;
                    }
                }

                // 2. 染色逻辑
                for (int y = 0; y < framebuffer.Size.Height; y++)
                {
                    int rowOffset = y * framebuffer.RowBytes;
                    for (int x = 0; x < framebuffer.Size.Width; x++)
                    {
                        int pixelOffset = rowOffset + (x * 4);
                        byte blue = pixels[pixelOffset];
                        byte green = pixels[pixelOffset + 1];
                        byte red = pixels[pixelOffset + 2];
                        byte alpha = pixels[pixelOffset + 3];

                        byte maskAlpha;
                        if (hasAlphaChannel)
                        {
                            // 带透明通道的图标：直接使用原图自身的透明度（纯白主体不再会被误当做白底删除了）
                            maskAlpha = alpha;
                        }
                        else
                        {
                            // 无透明通道的白底图片（如JPG）：使用亮度反相作为透明度
                            int darkness = 255 - Math.Min(red, Math.Min(green, blue));
                            maskAlpha = darkness <= 8 ? (byte)0 : (byte)Math.Clamp(darkness, 0, 255);
                        }

                        // 使用预乘Alpha (Premultiplied Alpha) 写入新颜色
                        pixels[pixelOffset] = (byte)((tintColor.B * maskAlpha) / 255);
                        pixels[pixelOffset + 1] = (byte)((tintColor.G * maskAlpha) / 255);
                        pixels[pixelOffset + 2] = (byte)((tintColor.R * maskAlpha) / 255);
                        pixels[pixelOffset + 3] = maskAlpha;
                    }
                }
                Marshal.Copy(pixels, 0, framebuffer.Address, bufferLength);
            }
            return tintedBitmap;
        }

        private static byte CalculateMaskAlpha(byte red, byte green, byte blue, byte alpha)
        {
            if (alpha <= 4)
            {
                return 0;
            }

            if (alpha < 250)
            {
                return alpha;
            }

            int darkness = 255 - Math.Min(red, Math.Min(green, blue));
            if (darkness <= 8)
            {
                return 0;
            }

            return (byte)Math.Clamp(darkness, 0, 255);
        }
    }
}
