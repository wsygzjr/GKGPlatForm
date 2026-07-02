using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using GKG.UI.General;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace GKG.Map.CameraViewerFuncCtlMapCell.ViewModel
{
    public class CameraViewerViewModel : ReactiveObject, IDisposable
    {
        public ObservableCollection<Bitmap> ImageGroup { get; } = new ObservableCollection<Bitmap>();
        private readonly int _maxGroupSize = 10;

        public CameraViewerViewModel()
        {
            
        }  

        public void OnFrameReceivedFromCamera(int camId, byte[] imageData)
        {
            var activeControl = GlobalVisionViewModel.CurActiveViewImageControl;
            if (activeControl != null)
            {
                var stream = new MemoryStream(imageData);
                activeControl.ShowImageFromStream(camId, stream);
            }

            if (CheckIfNeedsToSaveToGroup())
            {
                Dispatcher.UIThread.Post(() =>
                {
                    var thumbStream = new MemoryStream(imageData);
                    var thumbBitmap = new Bitmap(thumbStream);

                    ImageGroup.Add(thumbBitmap);

                    if (ImageGroup.Count > _maxGroupSize)
                    {
                        var oldestImage = ImageGroup[0];
                        ImageGroup.RemoveAt(0);
                        oldestImage?.Dispose();
                    }
                });
            }
        }

        private bool CheckIfNeedsToSaveToGroup()
        {
            // TODO: 实现是否需要保存到图片组的逻辑
            return false;
        }

        public void Dispose()
        {
            foreach (var img in ImageGroup)
            {
                img?.Dispose();
            }
            ImageGroup.Clear();
        }
    }
}