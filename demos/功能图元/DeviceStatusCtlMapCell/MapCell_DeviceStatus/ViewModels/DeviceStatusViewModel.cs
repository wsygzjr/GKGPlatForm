using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using GF_Gereric;
using Griffins;
using Griffins.Map;
using Griffins.UI2;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.ViewModels
{
    public class DeviceStatusViewModel : ReactiveObject, IDisposable
    {
        private bool _designTime;
        private DeviceStatusPropertyModelEdit _propertyModelEdit;
        private readonly object _imageLock = new object();
        private CancellationTokenSource _imageCts;

        public DeviceStatusViewModel() { }

        public DeviceStatusViewModel(bool designTime, DeviceStatusPropertyModelEdit propertyModelEdit)
        {
            _designTime = designTime;
            _propertyModelEdit = propertyModelEdit;
            InitializeFromModel(propertyModelEdit);
            ScheduleUpdateCurrentImage();
        }

        private void InitializeFromModel(DeviceStatusPropertyModelEdit model)
        {
            if (model == null) return;
            // Removed AppearanceInfo and LayoutInfo initialization as requested
            if (model.CommonInfo != null)
            {
                _imageSources = model.CommonInfo.ImageSources ?? new List<BitmapData>();
                _currentIndex = model.CommonInfo.CurrentIndex;
                _statusName = model.CommonInfo.StatusName;
                _deviceStatusValue = model.CommonInfo.DeviceStatusValue;
                _deviceStatusUnit = model.CommonInfo.DeviceStatusUnit;
            }
        }

        private double _opacity = 1.0;
        public double Opacity { get => _opacity; set => this.RaiseAndSetIfChanged(ref _opacity, value); }

        private bool _isVisible = true;
        public bool IsVisible { get => _isVisible; set => this.RaiseAndSetIfChanged(ref _isVisible, value); }

        private List<BitmapData> _imageSources = new List<BitmapData>();
        public List<BitmapData> ImageSources
        {
            get => _imageSources;
            set
            {
                if (ReferenceEquals(_imageSources, value)) return;
                _imageSources = value ?? new List<BitmapData>();
                this.RaisePropertyChanged(nameof(ImageSources));
                ScheduleUpdateCurrentImage();
            }
        }

        private int _currentIndex;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex == value) return;
                _currentIndex = value;
                this.RaisePropertyChanged(nameof(CurrentIndex));
                ScheduleUpdateCurrentImage();
            }
        }

        private IImage _currentImage;
        public IImage CurrentImage { get => _currentImage; private set => this.RaiseAndSetIfChanged(ref _currentImage, value); }

        public BitmapData CurrentImageSource
        {
            get
            {
                if (_imageSources != null && _imageSources.Count > 0)
                {
                    int validIndex = Math.Max(0, Math.Min(_currentIndex, _imageSources.Count - 1));
                    return _imageSources[validIndex];
                }
                return null;
            }
        }

        private void ScheduleUpdateCurrentImage()
        {
            CancellationTokenSource newCts;
            lock (_imageLock)
            {
                _imageCts?.Cancel();
                _imageCts?.Dispose();
                _imageCts = new CancellationTokenSource();
                newCts = _imageCts;
            }
            var token = newCts.Token;
            var src = CurrentImageSource;
            if (src == null) { CurrentImage = null; return; }
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(80, token);
                    var bmp = TryGetBitmapFromBitmapData(src);
                    if (bmp == null || token.IsCancellationRequested) return;
                    Dispatcher.UIThread.Post(() => { if (!token.IsCancellationRequested) CurrentImage = bmp; }, DispatcherPriority.Background);
                }
                catch { }
            }, token);
        }

        private static Bitmap TryGetBitmapFromBitmapData(BitmapData bitmapData)
        {
            try
            {
                if (bitmapData == null) return null;
                var bytes = bitmapData.ToBytes();
                if (bytes == null || bytes.Length == 0) return null;
                using var ms = new MemoryStream(bytes);
                return new Bitmap(ms);
            }
            catch { return null; }
        }

        private string _statusName;
        public string StatusName { get => _statusName; set => this.RaiseAndSetIfChanged(ref _statusName, value); }

        private string _deviceStatusValue;
        public string DeviceStatusValue { get => _deviceStatusValue; set => this.RaiseAndSetIfChanged(ref _deviceStatusValue, value); }

        private string _deviceStatusUnit;
        public string DeviceStatusUnit { get => _deviceStatusUnit; set => this.RaiseAndSetIfChanged(ref _deviceStatusUnit, value); }

        public void Dispose()
        {
            lock (_imageLock) { _imageCts?.Cancel(); _imageCts?.Dispose(); _imageCts = null; }
        }
    }
}
