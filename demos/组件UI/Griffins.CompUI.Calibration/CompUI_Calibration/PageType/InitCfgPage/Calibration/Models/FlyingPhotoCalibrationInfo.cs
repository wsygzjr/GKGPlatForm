using GKG;
using GKG.UI.General;
using System.Collections.Generic;

namespace Griffins.CompUI.Calibration.Models
{

    /// <summary>
    /// 飞拍标定信息
    /// </summary>
    public class FlyingPhotoCalibrationInfo : BasePositionInfo
    {
        /// <summary>
        ///飞拍速度（mm/s）
        /// </summary>
        public decimal FlyingSpeed { set; get; }
        /// <summary>
        ///提前加速度距离（mm）
        /// </summary>
        public decimal AdvanceaccelerationDistance { set; get; }
        /// <summary>
        ///飞拍首点整定时间（ms）
        /// </summary>
        public decimal FlyingFirsSettingTime { set; get; }
        /// <summary>
        ///方向
        /// </summary>
        public FlyingDirection Direction { set; get; }
        /// <summary>
        ///已标定速度
        /// </summary>
        public List<decimal> CalibratedSpeeds { set; get; }


        public FlyingPhotoCalibrationInfo()
        {
            CalibratedSpeeds = new List<decimal>();
        }
    }

}
