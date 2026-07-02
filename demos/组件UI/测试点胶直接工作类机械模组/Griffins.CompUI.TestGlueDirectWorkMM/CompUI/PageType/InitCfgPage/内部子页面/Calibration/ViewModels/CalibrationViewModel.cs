using Avalonia.Controls;
using ReactiveUI;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModels.OffsetCalibration;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModels.CameraRatioCalibration;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModels.FlyingPhotoCalibration;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModels.LaserAltimetryCalibration;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModels
{
    /// <summary>
    /// 标定-视图模型
    /// </summary>
    public class CalibrationViewModel : ReactiveObject
    {
        ///// <summary>
        ///// 标定结果改变事件
        ///// </summary>
        //public event EventHandler<CalibrationResultInfoChangedEventArgs>? OnCalibrationValueChanged;
        /// <summary>
        /// 偏移标定信息-视图模型
        /// </summary>
        public OffsetCalibrationViewModel OffsetCalibrationViewModel { get; }
        /// <summary>
        /// 相机比例标定信息-视图模型
        /// </summary>
        public CameraRatioCalibrationViewModel CameraRatioCalibrationViewModel { get; }
        /// <summary>
        /// 激光测高标定信息-视图模型
        /// </summary>
        public LaserAltimetryCalibrationViewModel LaserAltimetryCalibrationViewModel { get; }
        /// <summary>
        /// 飞拍标定信息-视图模型
        /// </summary>
        public FlyingPhotoCalibrationViewModel FlyingPhotoCalibrationViewModel { get; }
        public CalibrationViewModel()
        {
            OffsetCalibrationViewModel=new OffsetCalibrationViewModel();
            LaserAltimetryCalibrationViewModel = new LaserAltimetryCalibrationViewModel();
            CameraRatioCalibrationViewModel = new CameraRatioCalibrationViewModel();
            FlyingPhotoCalibrationViewModel = new FlyingPhotoCalibrationViewModel();

            subscribeChildViewModelEvents();
        }


        /// <summary>
        /// 设置视图引用
        /// </summary>
        public  void SetViewReference(Control view)
        {
            OffsetCalibrationViewModel.SetViewReference(view);
            LaserAltimetryCalibrationViewModel.SetViewReference(view);
            CameraRatioCalibrationViewModel.SetViewReference(view);
            FlyingPhotoCalibrationViewModel.SetViewReference(view);
        }

        /// <summary>
        /// 将数据回写到缓存
        /// </summary>
        public void UpdateToCache()
        {
            OffsetCalibrationViewModel.UpdateToCache();
            LaserAltimetryCalibrationViewModel.UpdateToCache();
            CameraRatioCalibrationViewModel.UpdateToCache();
            FlyingPhotoCalibrationViewModel.UpdateToCache();
        }
        /// <summary>
        /// 订阅子ViewModel的事件
        /// </summary>
        private void subscribeChildViewModelEvents()
        {
            //CameraRatioCalibrationViewModel.OnCalibrationValueChanged+= OnCalibrationValueChanged;
            //OffsetCalibrationViewModel.OnCalibrationValueChanged += OnCalibrationValueChanged;
            //LaserAltimetryCalibrationViewModel.OnCalibrationValueChanged += OnCalibrationValueChanged;
            //FlyingPhotoCalibrationViewModel.OnCalibrationValueChanged += OnCalibrationValueChanged;
        }
    }
}