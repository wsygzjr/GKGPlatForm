using Avalonia.Controls;
using GKG.UI.General;
using ReactiveUI;
using System;

namespace Griffins.CompUI.Calibration.ViewModels
{
    /// <summary>
    /// 偏移标定信息 ViewModel
    /// </summary>
    public class OffsetCalibrationViewModel : ReactiveObject
    {
        /// <summary>
        /// 标定结果改变事件
        /// </summary>
        public event EventHandler<CalibrationResultInfoChangedEventArgs>? OnCalibrationValueChanged;
        /// <summary>
        /// 相机Vs胶阀 ViewModel
        /// </summary>
        public CameraVsGluevalveViewModel CameraVsGluevalve { get; }

        /// <summary>
        /// 相机Vs激光 ViewModel
        /// </summary>
        public CameraVsLaserViewModel CameraVsLaser { get; }

        public OffsetCalibrationViewModel()
        {
            CameraVsGluevalve = new CameraVsGluevalveViewModel();
            CameraVsLaser = new CameraVsLaserViewModel();

            CameraVsGluevalve.OnCalibrationValueChanged += OnCalibrationValueChanged;
            CameraVsLaser.OnCalibrationValueChanged += OnCalibrationValueChanged;
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            CameraVsGluevalve.SetViewReference(view);
            CameraVsLaser.SetViewReference(view);
        }
        /// <summary>
        /// 将数据回写到缓存
        /// </summary>
        public void UpdateToCache()
        {
            CameraVsGluevalve.UpdateToCache();
            CameraVsLaser.UpdateToCache();
        }
    }

}