using GKG;
using GKG.UI.General;
using System.Collections.Generic;

namespace Griffins.CompUI.Calibration.Models
{
    /// <summary>
    ///相机Vs胶阀
    /// </summary>
    public class CameraVsGluevalve
    {
        /// <summary>
        ///0度阀位置
        /// </summary>
        public DegreevalvePosition ZeroDegreevalvePosition { set; get; }
        /// <summary>
        ///0相机位置
        /// </summary>
        public ZeroCameraPosition ZeroCameraPosition { set; get; }
        /// <summary>
        ///90度阀位置
        /// </summary>
        public DegreevalvePosition NinetyDegreevalvePosition { set; get; }
        /// <summary>
        ///90相机位置
        /// </summary>
        public BasePositionInfo NinetyCameraPosition { set; get; }
        /// <summary>
        /// 相机Vs胶阀公共参数配置
        /// </summary>
        public CameraVsGluevalveComInfo CameraVsGluevalveComInfo { set; get; }

        /// <summary>
        /// 0度阀标定结果
        /// key：相机ID
        /// </summary>
        public Dictionary<string, Point2D> ZeroCalibrationResult { get; set; }
        public CameraVsGluevalve()
        {
            ZeroDegreevalvePosition = new DegreevalvePosition();
            ZeroCameraPosition = new ZeroCameraPosition();
            NinetyDegreevalvePosition = new DegreevalvePosition();
            NinetyCameraPosition = new BasePositionInfo();
            CameraVsGluevalveComInfo = new CameraVsGluevalveComInfo();
            ZeroCalibrationResult = new Dictionary<string, Point2D>();
        }
    }


}
