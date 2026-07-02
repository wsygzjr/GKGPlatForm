using GKG;
using GKG.UI.General;
using System.Collections.Generic;

namespace Griffins.CompUI.Calibration.Models
{
    /// <summary>
    ///相机Vs激光
    /// </summary>
    public class CameraVsLaser
    {
        /// <summary>
        ///激光位置
        /// </summary>
        public LaserPosition LaserPosition { set; get; }

        /// <summary>
        ///相机位置
        /// </summary>
        public BasePositionInfo CameraPosition { set; get; }

        /// <summary>
        /// 标定结果：偏移值
        /// key：相机ID
        /// </summary>
        public Dictionary<string, Point2D> CalibrationResult { get; set; }
        public CameraVsLaser()
        {
            LaserPosition = new LaserPosition();
            CameraPosition = new LaserPosition();
            CalibrationResult = new Dictionary<string, Point2D>();
        }
    }



}
