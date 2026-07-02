using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// 图像转换帮助类。
        /// 提供 Bitmap 与 byte[] 之间的常用互转能力。
        /// </summary>
        public static class ImageConvertHelper
        {
            /// <summary>
            /// Bitmap 转指定格式字节数组（PNG/JPEG/BMP）。
            /// </summary>
            public static byte[] BitmapToBytes(Bitmap bitmap, ImageFormat format)
            {
                if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
                if (format == null) throw new ArgumentNullException(nameof(format));

                using var ms = new MemoryStream();
                bitmap.Save(ms, format);
                return ms.ToArray();
            }

            /// <summary>
            /// 字节数组（PNG/JPEG/BMP）转 Bitmap。
            /// </summary>
            public static Bitmap BytesToBitmap(byte[] bytes)
            {
                if (bytes == null || bytes.Length == 0)
                    throw new ArgumentException("bytes 不能为空", nameof(bytes));

                using var ms = new MemoryStream(bytes);
                using var img = Image.FromStream(ms);
                return new Bitmap(img);
            }

            /// <summary>
            /// Bitmap 转原始像素字节数组。
            /// </summary>
            public static byte[] BitmapToRawBytes(Bitmap bitmap)
            {
                if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));

                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

                try
                {
                    int byteCount = Math.Abs(data.Stride) * bitmap.Height;
                    byte[] bytes = new byte[byteCount];
                    Marshal.Copy(data.Scan0, bytes, 0, byteCount);
                    return bytes;
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
            }

            /// <summary>
            /// 原始像素字节数组转 Bitmap。
            /// </summary>
            public static Bitmap RawBytesToBitmap(byte[] bytes, int width, int height, PixelFormat pixelFormat)
            {
                if (bytes == null || bytes.Length == 0)
                    throw new ArgumentException("bytes 不能为空", nameof(bytes));

                Bitmap bitmap = new Bitmap(width, height, pixelFormat);
                Rectangle rect = new Rectangle(0, 0, width, height);
                BitmapData data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, pixelFormat);

                try
                {
                    int byteCount = Math.Abs(data.Stride) * height;
                    Marshal.Copy(bytes, 0, data.Scan0, Math.Min(bytes.Length, byteCount));
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }

                return bitmap;
            }
        }
    }
}
