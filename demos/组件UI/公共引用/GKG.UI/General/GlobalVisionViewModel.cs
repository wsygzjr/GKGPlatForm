using Avalonia.Controls;
using AvaloniaVisionControl;
using GKG.UI.General;


namespace GKG.UI.General
{
    /// <summary>
    /// 全局静态视觉对象管理
    /// </summary>
    public static class GlobalVisionViewModel
    {
         static GlobalVisionViewModel()
        {
            //全局只有一个视觉vm实例,但有多个视觉view
            CameraShowViewModel = new CameraShowViewModel();
            CameraOperationViewModel = new CameraOperationViewModel(CameraShowViewModel);
            AxisViewModel=new AxisViewModel();
        }

        /// <summary>
        /// 图像显示设置配置信息
        /// </summary>
        public static CameraShowViewModel CameraShowViewModel { get; }

        /// <summary>
        /// 相机操作-视图模型
        /// </summary>
        public static CameraOperationViewModel CameraOperationViewModel { get; }

        /// <summary>
        /// 相机的坐标轴信息
        /// 用于在教导时读取相机坐标轴信息
        /// </summary>
        public static AxisViewModel AxisViewModel { set; get; }
        /// <summary>
        /// 当前活动页面的相机视频显示控件
        /// </summary>
        public static CtlOnlyShowImage? CurActiveViewImageControl { set; get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cameraShowCfgInfo">相机显示配置信息</param>
        /// <param name="cameraOperationCfgInfo">相机显示配置信息</param>
        public static void Init(CameraShowCfgInfo cameraShowCfgInfo,CameraOperationCfgInfo cameraOperationCfgInfo)
        {
            CameraShowViewModel.Init(cameraShowCfgInfo);
            CameraOperationViewModel.Init(cameraOperationCfgInfo);
            AxisViewModel = CameraShowViewModel.AxisViewModel;
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public static void SetViewReference(Control view)
        {
            CameraOperationViewModel.SetViewReference(view);
        }
    }
}
