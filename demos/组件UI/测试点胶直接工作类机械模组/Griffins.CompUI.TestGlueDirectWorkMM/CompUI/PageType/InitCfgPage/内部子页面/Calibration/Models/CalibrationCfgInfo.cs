using System.Collections.Generic;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Models
{
    /// <summary>
    /// 标定配置信息
    /// </summary>
    public class CalibrationCfgInfo
    {
        /// <summary>
        /// 偏移标定信息
        /// </summary>
        public OffsetCalibrationInfo OffsetCalibrationInfo { set; get; }
        /// <summary>
        /// 激光测高标定信息
        /// </summary>
        public LaserAltimetryCalibrationInfo LaserAltimetryCalibrationInfo { set; get; }
        /// <summary>
        /// 相机比例标定信息
        /// </summary>
        public CameraRatioCalibrationInfo CameraRatioCalibrationInfo { set; get; }
        /// <summary>
        /// 飞拍标定信息
        /// </summary>
        public FlyingPhotoCalibrationInfo FlyingPhotoCalibrationInfo { set; get; }

        ///// <summary>
        ///// 标定结果信息
        ///// </summary>
        //public CalibrationResultInfo CalibrationResultInfo { set; get; }
        public CalibrationCfgInfo()
        {
            OffsetCalibrationInfo = new OffsetCalibrationInfo();
            LaserAltimetryCalibrationInfo = new LaserAltimetryCalibrationInfo();
            CameraRatioCalibrationInfo = new CameraRatioCalibrationInfo();
            FlyingPhotoCalibrationInfo = new FlyingPhotoCalibrationInfo();
            //CalibrationResultInfo = new CalibrationResultInfo();
        }
    }

    /// <summary>
    /// 功能头标定配置信息
    /// </summary>
    public class FunctionHeadCalibrationCfgInfo
    {
        /// <summary>
        /// 功能头
        /// </summary>
        public string FunctionHeadID { set; get; } = "";
        /// <summary>
        /// 标定配置信息
        /// </summary>
        public CalibrationCfgInfo CalibrationCfgInfo { set; get; }
       
        public FunctionHeadCalibrationCfgInfo()
        {
            CalibrationCfgInfo = new CalibrationCfgInfo();
        }
    }
    /// <summary>
    /// 功能头标定配置信息列表
    /// </summary>
    public class FunctionHeadCalibrationCfgInfoList:List<FunctionHeadCalibrationCfgInfo>
    {
       
    }

}
