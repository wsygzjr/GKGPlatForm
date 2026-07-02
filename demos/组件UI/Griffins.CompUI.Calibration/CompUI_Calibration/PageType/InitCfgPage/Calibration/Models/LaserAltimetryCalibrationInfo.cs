using GKG;
using GKG.UI.General;

namespace Griffins.CompUI.Calibration.Models
{
    /// <summary>
    /// 激光测高标定信息
    /// </summary>
    public class LaserAltimetryCalibrationInfo : BasePositionInfo
    {
        /// <summary>
        ///蘑菇头限制高度(mm)
        /// </summary>
        public decimal MushroomHeadLimitHight { set; get; }
        /// <summary>
        ///阀到平面距离偏差报警（mm）
        /// </summary>
        public decimal ValveToPlaneDistanceDeviationAlarm { set; get; }
        /// <summary>
        ///蘑菇头标定补偿值(mm)
        /// </summary>
        public decimal MushroomHeadCalibrationCompensationValue { set; get; }
        /// <summary>
        ///激光测高值(mm)显示
        /// </summary>
        public decimal LaserHight { set; get; }
        /// <summary>
        ///测高状态 显示
        /// </summary>
        public bool HightState { set; get; }
        /// <summary>
        /// 标定结果
        /// </summary>
        public LaserAndFunctionHeadCalibrationResult CalibrationResult { set; get; }

        public LaserAltimetryCalibrationInfo()
        {
            CalibrationResult = new LaserAndFunctionHeadCalibrationResult();
        }

    }

}
