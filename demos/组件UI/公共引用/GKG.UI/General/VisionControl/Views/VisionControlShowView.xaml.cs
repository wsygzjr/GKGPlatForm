using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaVisionControl;

namespace GKG.UI.General
{
    /// <summary>
    /// 相机显示视图
    /// </summary>
    public partial class VisionControlShowView : UserControl
    { 
        /// <summary>
        /// UI线程安全的定时器
        /// </summary>
        private DispatcherTimer? _videoTimer;
        /// <summary>
        /// 视频帧率（默认30帧/秒，间隔33ms）
        /// </summary>
        private const int FrameIntervalMs = 33;

       
        private CtlOnlyShowImage? _cameraPreControl;
        /// <summary>
        /// 相机视频显示控件
        /// </summary>
        public CtlOnlyShowImage CameraPreControl
        {
            get
            {
                if (_cameraPreControl == null)
                    throw new Exception("视觉控件实例不能为空");
                return _cameraPreControl;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public VisionControlShowView()
        {
            InitializeComponent();
            DataContext = GlobalVisionViewModel.CameraShowViewModel;

           
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #region 控件显示/隐藏

        /// <summary>
        /// 控件显示自动注册
        /// </summary>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            //获取图像显示控件实例
            _cameraPreControl = this.FindControl<CtlOnlyShowImage>("ImageControl");
            if (_cameraPreControl != null)
            {
                var mmPerPixel = new Avalonia.Point(0.1, 0.1);
                _cameraPreControl.SetCameraCalib(mmPerPixel, 810, 500);
            }
            //初始化并启动定时器
            initVideoTimer();

            updateVideoFrame();

            if (DataContext == null)
                throw new Exception("未设置数据上下文");
            var globalVM = (CameraShowViewModel)DataContext;
            globalVM.RegisterActiveVisionView(this);
        }

        /// <summary>
        /// 控件隐藏/卸载时，取消注册
        /// </summary>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            if (DataContext == null)
                throw new Exception("未设置数据上下文");
            var globalVM = (CameraShowViewModel)DataContext;
            globalVM.UnregisterActiveVisionView(this);

            if (_videoTimer != null)
            {
                _videoTimer.Stop();
                _videoTimer.Tick -= videoTimer_Tick;
                _videoTimer = null;

            }
        }
        #endregion

        #region 定时更新图片
        /// <summary>
        /// 初始化视频定时器
        /// </summary>
        private void initVideoTimer()
        {
            if (_videoTimer != null)
            {
                _videoTimer.Stop();
                _videoTimer.Tick -= videoTimer_Tick;
                _videoTimer = null;
            }
            _videoTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(FrameIntervalMs)
            };
            _videoTimer.Tick += videoTimer_Tick;
            _videoTimer.Start();
        }

        /// <summary>
        /// 定时器触发：刷新视频帧
        /// </summary>
        private void videoTimer_Tick(object? sender, EventArgs e)
        {
            if (_cameraPreControl == null || !this.IsLoaded)
                return;
            updateVideoFrame();
        }

        /// <summary>
        /// 更新视频帧
        /// </summary>
        private void updateVideoFrame()
        {
            try
            {
                // 方式1：从本地文件加载
                //ImageControlHelper.ShowImageFromFile(_cameraPreControl!, 0, _filePath);

                // 方式2：若为实时相机流
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新视频帧失败：{ex.Message}");
            }
        } 
        #endregion

    }
}
