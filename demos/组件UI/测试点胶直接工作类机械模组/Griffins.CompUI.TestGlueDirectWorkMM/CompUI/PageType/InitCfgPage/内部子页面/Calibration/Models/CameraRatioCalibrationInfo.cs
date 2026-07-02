using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;
using Griffins.UI.General;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Models
{

    /// <summary>
    /// 相机比例标定信息
    /// </summary>
    public class CameraRatioCalibrationInfo : BasePositionInfo
    {
        /// <summary>
        ///运动步距(mm)
        /// </summary>
        public decimal SportsStride { set; get; }

        ///// <summary>
        ///// 标定结果
        ///// </summary>
        //public CameraScaleCalibrationResult CalibrationResult { set; get; }

        public CameraRatioCalibrationInfo()
        {
            //CalibrationResult = new CameraScaleCalibrationResult();
        }
    }

}
