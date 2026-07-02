using GKG.UI.General;

namespace Griffins.CompUI.Calibration.Models
{
    /// <summary>
    /// 偏移标定信息
    /// </summary>
    public class OffsetCalibrationInfo
    {
        /// <summary>
        /// 相机Vs胶阀
        /// </summary>
        public CameraVsGluevalve CameraVsGluevalve { set; get; }
        /// <summary>
        /// 相机Vs激光
        /// </summary>
        public CameraVsLaser CameraVsLaser { set; get; }

        public OffsetCalibrationInfo()
        {
            CameraVsGluevalve = new CameraVsGluevalve();
            CameraVsLaser = new CameraVsLaser();
        }
    }

}
