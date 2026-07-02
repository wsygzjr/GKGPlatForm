using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ShareMemRPCLite;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace GKG.Vision
{
    /// <summary>
    /// Frontend-facing vision service.
    /// Converts GVisionQt bitmap events to byte[] events for UI/web layers.
    /// </summary>
    public sealed class FrontendVisionService : IFrontendVisionService
    {
        private GVisionRealtimeBitmapClient? _realtimeClient;

        public FrontendVisionService()
        {
            // 创建并启动实时客户端，支持多相机端口订阅并将图像显示到窗口。
            try
            {
                _realtimeClient = new GVisionRealtimeBitmapClient(
                    camId: 0,
                    isInvokeGVision: true,
                    ensureIntervalMs: 2000,
                    subscribeAllCams: true,
                    subscribeMaxCamId: 15);
                _realtimeClient.BitmapReceived += GVision_WhenReceiveBitmap;
                _realtimeClient.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Init realtime stream failed: {ex.Message}");
                DisposeRealtimeStream();
            }
        }

        public event EventHandler<GrabImageSucceededEventArgs>? GrabImageSucceeded;

        public void NotifyGrabImageSucceeded(byte[] imageBytes)
        {
            GrabImageSucceeded?.Invoke(this, new GrabImageSucceededEventArgs(imageBytes));
        }

        /// <summary>
        /// 取消订阅并释放实时客户端。
        /// </summary>
        private void DisposeRealtimeStream()
        {
            if (_realtimeClient != null)
            {
                _realtimeClient.BitmapReceived -= GVision_WhenReceiveBitmap;
                _realtimeClient.Dispose();
                _realtimeClient = null;
            }
        }
        public void Dispose()
        {
            DisposeRealtimeStream();
        }
        #region 前端使用
        /// <summary>
        /// 接收实时帧，转换为 Avalonia 位图并显示。
        /// </summary>
        private void GVision_WhenReceiveBitmap(object? sender, ShareMemRPCLite.ReceiveBitmapEventArgs e)
        {
            if (e.Image == null)
            {
                return;
            }
            NotifyGrabImageSucceeded(ImageConvertHelper.BitmapToBytes(e.Image, ImageFormat.Bmp));
            //Avalonia.Media.Imaging.Bitmap? avaloniaBitmap = null;
            //try
            //{
            //    avaloniaBitmap = ConvertDrawingBitmap(e.Image);
            //    int code = _imageControl?.ShowImage(
            //        new AvaloniaVisionControl.ReceiveBitmapEventArgs(DisplayCamId, avaloniaBitmap)) ?? -1;
            //    if (code == 0)
            //    {
            //        avaloniaBitmap = null;
            //    }
            //    else
            //    {
            //        Console.WriteLine($"ShowImage returned {code}.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Decode/display frame failed: {ex.Message}");
            //}
            //finally
            //{
            //    avaloniaBitmap?.Dispose();
            //    e.Image?.Dispose();
            //}
        }

        /// <summary>
        /// 将 System.Drawing 位图转换为用于渲染的 Avalonia 位图（Bgra8888）。
        /// </summary>
        private static Avalonia.Media.Imaging.Bitmap ConvertDrawingBitmap(System.Drawing.Bitmap sourceBitmap)
        {
            System.Drawing.Bitmap? normalizedBitmap = null;
            System.Drawing.Bitmap bitmapToCopy = sourceBitmap;

            if (sourceBitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                normalizedBitmap = sourceBitmap.Clone(
                    new System.Drawing.Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                bitmapToCopy = normalizedBitmap;
            }

            try
            {
                var avaloniaBitmap = new WriteableBitmap(
                    new PixelSize(bitmapToCopy.Width, bitmapToCopy.Height),
                    new Vector(
                        bitmapToCopy.HorizontalResolution > 0 ? bitmapToCopy.HorizontalResolution : 96,
                        bitmapToCopy.VerticalResolution > 0 ? bitmapToCopy.VerticalResolution : 96),
                    Avalonia.Platform.PixelFormat.Bgra8888,
                    AlphaFormat.Opaque);

                var rect = new System.Drawing.Rectangle(0, 0, bitmapToCopy.Width, bitmapToCopy.Height);
                BitmapData bitmapData = bitmapToCopy.LockBits(
                    rect,
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                try
                {
                    int sourceRowBytes = bitmapToCopy.Width * 3;
                    byte[] sourceRowBuffer = new byte[sourceRowBytes];
                    byte[] targetRowBuffer = new byte[bitmapToCopy.Width * 4];

                    using var framebuffer = avaloniaBitmap.Lock();
                    for (int y = 0; y < bitmapToCopy.Height; y++)
                    {
                        IntPtr sourceRow = IntPtr.Add(bitmapData.Scan0, y * bitmapData.Stride);
                        IntPtr targetRow = IntPtr.Add(framebuffer.Address, y * framebuffer.RowBytes);

                        Marshal.Copy(sourceRow, sourceRowBuffer, 0, sourceRowBytes);

                        for (int x = 0; x < bitmapToCopy.Width; x++)
                        {
                            int sourceIndex = x * 3;
                            int targetIndex = x * 4;
                            targetRowBuffer[targetIndex] = sourceRowBuffer[sourceIndex];
                            targetRowBuffer[targetIndex + 1] = sourceRowBuffer[sourceIndex + 1];
                            targetRowBuffer[targetIndex + 2] = sourceRowBuffer[sourceIndex + 2];
                            targetRowBuffer[targetIndex + 3] = 255;
                        }

                        Marshal.Copy(targetRowBuffer, 0, targetRow, targetRowBuffer.Length);
                    }
                }
                finally
                {
                    bitmapToCopy.UnlockBits(bitmapData);
                }

                return avaloniaBitmap;
            }
            finally
            {
                normalizedBitmap?.Dispose();
            }
        }


        #endregion
    }
}
