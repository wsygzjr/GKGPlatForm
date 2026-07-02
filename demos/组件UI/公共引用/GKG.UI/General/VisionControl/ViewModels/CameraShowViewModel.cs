using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaVisionControl;
using ReactiveUI;
using AvaloniaPoint = Avalonia.Point;

namespace GKG.UI.General
{
    /// <summary>
    /// 相机显示-视图模型
    /// </summary>
    public class CameraShowViewModel : ReactiveObject
    {
        /// <summary>
        /// 页面布局根视图引用（用于UI上下文关联）
        /// </summary>
        private Control? _viewReference;
        private bool _isExpandedLightSource;
        private bool _isDrawMap;
        private string _drawMapType = string.Empty;
        private AvaloniaPoint? _roiDrawStartImagePosition;
        private CtlOnlyShowImage? _drawMapImageControl;
        private EventHandler<ImageClickEventArgs>? _roiDrawMouseDownHandler;
        private EventHandler<ImageClickEventArgs>? _roiDrawMouseUpHandler;

        private const double MinRoiDrawSizePixels = 4.0;

        /// <summary>
        /// 是否展开RGBD光源信息栏
        /// true:展示光源信息栏 否:展示坐标轴信息栏
        /// </summary>
        public bool IsExpandedLightSource
        {
            get => _isExpandedLightSource;
            set => this.RaiseAndSetIfChanged(ref _isExpandedLightSource, value);
        }
        /// <summary>
        /// 光源信息
        /// </summary>
        public LightSourceViewModel LightSourceViewModel { set; get; }
        /// <summary>
        /// 坐标轴配置信息
        /// </summary>
        public AxisViewModel AxisViewModel { set; get; }
        /// <summary>
        /// 图像显示设置配置信息
        /// </summary>
        public ImageDisplayViewModel ImageDisplayViewModel { set; get; }

        /// <summary>
        /// 是否处于 ROI 拖拽绘制模式。
        /// </summary>
        public bool IsDrawMap
        {
            get => _isDrawMap;
            set
            {
                if (_isDrawMap == value)
                {
                    return;
                }

                _isDrawMap = value;
                this.RaisePropertyChanged(nameof(IsDrawMap));
                syncDrawMapHandlers();
            }
        }

        /// <summary>
        /// ROI 绘制类型：Square / Circle / Polygon。
        /// </summary>
        public string DrawMapType
        {
            get => _drawMapType;
            set
            {
                var normalized = value ?? string.Empty;
                if (string.Equals(_drawMapType, normalized, StringComparison.Ordinal))
                {
                    return;
                }

                _drawMapType = normalized;
                this.RaisePropertyChanged(nameof(DrawMapType));
            }
        }

        /// <summary>
        /// 事件（通知外部数据变更）
        /// </summary>
        public event EventHandler? AfterModified;
        /// <summary>
        /// 构造函数
        /// </summary>
        public CameraShowViewModel()
        {
            LightSourceViewModel = new LightSourceViewModel();
            AxisViewModel = new AxisViewModel();
            ImageDisplayViewModel = new ImageDisplayViewModel();
            subscribeChildViewModelEvents();
        }

        /// <summary>
        ///注册当前激活的视觉控件
        ///vm全局只有一个实例，视觉控件有多个，固要设置当前使用的视觉控件
        /// </summary>
        public void RegisterActiveVisionView(VisionControlShowView view)
        {
            detachDrawMapHandlers();
            GlobalVisionViewModel.CurActiveViewImageControl = view.CameraPreControl;
            view.CameraPreControl.ImageClick += _imageControl_ImageClick;
            syncDrawMapHandlers();
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        public void UnregisterActiveVisionView(VisionControlShowView view)
        {
            view.CameraPreControl.ImageClick -= _imageControl_ImageClick;
            if (ReferenceEquals(_drawMapImageControl, view.CameraPreControl))
            {
                detachDrawMapHandlers();
            }
        }
        /// <summary>
        /// 鼠标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _imageControl_ImageClick(object? sender, ImageClickEventArgs e)
        {
            //控制机械手运动
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cameraShowCfgInfo">相机显示配置信息</param>
        public void Init(CameraShowCfgInfo cameraShowCfgInfo)
        {
            LightSourceViewModel.Init(cameraShowCfgInfo.LightSourceCfgInfo);
            AxisViewModel.Init(cameraShowCfgInfo.AxisCfgInfo);
            ImageDisplayViewModel.Init(cameraShowCfgInfo.ImageDisplayCfgInfo);

        }
        /// <summary>
        /// 复制到指定相机显示配置信息
        /// </summary>
        /// <param name="cameraShowCfgInfo">相机显示配置信息</param>
        public void CopyTo(CameraShowCfgInfo cameraShowCfgInfo)
        {
            LightSourceViewModel.Extract(cameraShowCfgInfo.LightSourceCfgInfo);
            AxisViewModel.Extract(cameraShowCfgInfo.AxisCfgInfo);
            ImageDisplayViewModel.Extract(cameraShowCfgInfo.ImageDisplayCfgInfo);
        }
        /// <summary>
        /// 订阅子ViewModel的事件
        /// </summary>
        private void subscribeChildViewModelEvents()
        {
            LightSourceViewModel.AfterModified += viewModel_ValueChanged;
            ImageDisplayViewModel.AfterModified += viewModel_ValueChanged;
        }

        /// <summary>
        /// 界面配置信息改变事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void viewModel_ValueChanged(object? sender, EventArgs e)
        {
            onAfterModified();
        }

        /// <summary>
        /// 触发配置修改事件
        /// </summary>
        private void onAfterModified()
        {
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void syncDrawMapHandlers()
        {
            detachDrawMapHandlers();
            if (!_isDrawMap)
            {
                return;
            }

            var control = GlobalVisionViewModel.CurActiveViewImageControl;
            if (control == null)
            {
                return;
            }

            control.IsElementEditingEnabled = true;
            if (control.CtlShowPaintStatus == ImageElementCtlStatus.None)
            {
                control.CtlShowPaintStatus = ImageElementCtlStatus.ShowAll;
            }

            _drawMapImageControl = control;
            _roiDrawMouseDownHandler = onRoiDrawMouseDown;
            _roiDrawMouseUpHandler = onRoiDrawMouseUp;
            control.ImageMouseDown += _roiDrawMouseDownHandler;
            control.ImageMouseUp += _roiDrawMouseUpHandler;
        }

        private void detachDrawMapHandlers()
        {
            if (_drawMapImageControl != null)
            {
                if (_roiDrawMouseDownHandler != null)
                {
                    _drawMapImageControl.ImageMouseDown -= _roiDrawMouseDownHandler;
                }

                if (_roiDrawMouseUpHandler != null)
                {
                    _drawMapImageControl.ImageMouseUp -= _roiDrawMouseUpHandler;
                }
            }

            _drawMapImageControl = null;
            _roiDrawMouseDownHandler = null;
            _roiDrawMouseUpHandler = null;
            _roiDrawStartImagePosition = null;
        }

        private void onRoiDrawMouseDown(object? sender, ImageClickEventArgs e)
        {
            _roiDrawStartImagePosition = e.ImagePosition;
        }

        private void onRoiDrawMouseUp(object? sender, ImageClickEventArgs e)
        {
            if (!_isDrawMap || _roiDrawStartImagePosition == null || sender is not CtlOnlyShowImage control)
            {
                return;
            }

            var start = _roiDrawStartImagePosition.Value;
            var end = e.ImagePosition;
            _roiDrawStartImagePosition = null;

            if (!tryCreateRoiElement(start, end, out var element))
            {
                return;
            }

            control.AddPaintElement(element);
            control.ReFresh();
            onAfterModified();
        }

        private bool tryCreateRoiElement(AvaloniaPoint start, AvaloniaPoint end, out PaintElement element)
        {
            element = null!;
            if (Math.Abs(end.X - start.X) < MinRoiDrawSizePixels &&
                Math.Abs(end.Y - start.Y) < MinRoiDrawSizePixels)
            {
                return false;
            }

            var drawType = _drawMapType?.Trim() ?? string.Empty;
            element = new PaintElement
            {
                Color = Colors.Cyan,
                LineWidth = 2.0,
                IsFill = false,
                Visible = true,
            };

            switch (drawType)
            {
                case "Square":
                    element.Type = PaintElementType.Rectangle;
                    element.Pts = new List<double> { start.X, start.Y, end.X, end.Y };
                    return true;

                case "Circle":
                    element.Type = PaintElementType.Circle;
                    element.Pts = new List<double> { start.X, start.Y, end.X, end.Y };
                    return true;

                case "Polygon":
                    var x1 = Math.Min(start.X, end.X);
                    var y1 = Math.Min(start.Y, end.Y);
                    var x2 = Math.Max(start.X, end.X);
                    var y2 = Math.Max(start.Y, end.Y);
                    element.Type = PaintElementType.Polygon;
                    element.Pts = new List<double> { x1, y1, x2, y1, x2, y2, x1, y2 };
                    return true;

                default:
                    return false;
            }
        }
    }
}
