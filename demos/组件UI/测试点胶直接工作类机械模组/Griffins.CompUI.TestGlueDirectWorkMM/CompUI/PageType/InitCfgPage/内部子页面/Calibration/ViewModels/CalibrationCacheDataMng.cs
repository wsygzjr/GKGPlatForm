using Griffins.UI.General;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModels
{
    /// <summary>
    /// 标定缓存数据管理
    /// </summary>
    internal class CalibrationCacheDataMng
    {
        static CalibrationCacheDataMng()
        {
            //AxisViewModel = new AxisViewModel();
        }
        //#region 相机中的坐标轴信息

        ///// <summary>
        ///// 相机的坐标轴信息
        ///// 用于在教导时读取相机坐标轴信息
        ///// </summary>
        //public static AxisViewModel AxisViewModel { set; get; }

        //#endregion

        #region 功能头标定缓存信息

        #region 功能头标定缓存

        /// <summary>
        /// 功能头标定信息缓存
        /// </summary>
        private static Dictionary<string, CalibrationCfgInfo> functionHeadCalibrationDic { set; get; } = new Dictionary<string, CalibrationCfgInfo>();
       
        /// <summary>
        /// 设置功能头标定信息
        /// </summary>
        /// <param name="functionHeadCalibrationDic"></param>
        public static void SetCalibration(FunctionHeadCalibrationCfgInfoList functionHeadCalibrationCfgInfoes)
        {
            functionHeadCalibrationDic.Clear();
            foreach (var item in functionHeadCalibrationCfgInfoes)
            {
                functionHeadCalibrationDic.Add(item.FunctionHeadID, item.CalibrationCfgInfo);
            }
        }
        /// <summary>
        /// 获取功能头标定信息
        /// </summary>
        /// <param name="functionHeadCalibrationDic"></param>
        public static FunctionHeadCalibrationCfgInfoList GetCalibration()
        {
            FunctionHeadCalibrationCfgInfoList functionHeadCalibrationCfgInfoes = new FunctionHeadCalibrationCfgInfoList();
            foreach (var item in functionHeadCalibrationDic.Keys)
            {
                functionHeadCalibrationCfgInfoes.Add(new FunctionHeadCalibrationCfgInfo()
                {
                    FunctionHeadID = item,
                    CalibrationCfgInfo = functionHeadCalibrationDic[item]
                });
            }
            return functionHeadCalibrationCfgInfoes;
        }
        #endregion

        #region 偏移标定
        /// <summary>
        /// 设置相机Vs胶阀缓存
        /// </summary>
        /// <param name="functionHeadID"></param>
        /// <param name="cameraVsGluevalve"></param>
        public static void SetCameraVsGluevalve(string functionHeadID,
            CameraVsGluevalve cameraVsGluevalve)
        {
            if (!functionHeadCalibrationDic.ContainsKey(functionHeadID))
            {
                functionHeadCalibrationDic.Add(functionHeadID, new CalibrationCfgInfo());
            }
            functionHeadCalibrationDic[functionHeadID].OffsetCalibrationInfo.CameraVsGluevalve = cameraVsGluevalve;
        }
        /// <summary>
        /// 获取功能头相机Vs胶阀
        /// </summary>
        /// <param name="functionHeadID"></param>
        /// <param name="cameraVsGluevalve"></param>
        public static void GetCameraVsGluevalve(string functionHeadID,
           out CameraVsGluevalve cameraVsGluevalve)
        {
            if (!functionHeadCalibrationDic.ContainsKey(functionHeadID))
            {
                functionHeadCalibrationDic.Add(functionHeadID, new CalibrationCfgInfo());
            }
            cameraVsGluevalve = functionHeadCalibrationDic[functionHeadID].OffsetCalibrationInfo.CameraVsGluevalve;
        }

        /// <summary>
        /// 设置相机Vs激光
        /// </summary>
        /// <param name="cameraVsLaser"></param>
        public static void SetCameraVsLaser(string functionHeadID,
            CameraVsLaser cameraVsLaser)
        {
            if (!functionHeadCalibrationDic.ContainsKey(functionHeadID))
            {
                functionHeadCalibrationDic.Add(functionHeadID, new CalibrationCfgInfo());
            }
            functionHeadCalibrationDic[functionHeadID].OffsetCalibrationInfo.CameraVsLaser = cameraVsLaser;
        }
        /// <summary>
        /// 获取功能头相机Vs激光
        /// </summary>
        /// <param name="cameraVsLaser"></param>
        public static void GetCameraVsLaser(string functionHeadID,
           out CameraVsLaser cameraVsLaser)
        {
            if (!functionHeadCalibrationDic.ContainsKey(functionHeadID))
            {
                functionHeadCalibrationDic.Add(functionHeadID, new CalibrationCfgInfo());
            }
            cameraVsLaser = functionHeadCalibrationDic[functionHeadID].OffsetCalibrationInfo.CameraVsLaser;
        }
        #endregion

        #region 激光测高标定
        /// <summary>
        /// 设置激光测高标定信息
        /// </summary>
        /// <param name="functionHeadID"></param>
        /// <param name="laserAltimetryCalibrationInfo"></param>
        public static void SetLaserAltimetryCalibrationInfo(string functionHeadID,
            LaserAltimetryCalibrationInfo laserAltimetryCalibrationInfo)
        {
            if (!functionHeadCalibrationDic.ContainsKey(functionHeadID))
            {
                functionHeadCalibrationDic.Add(functionHeadID, new CalibrationCfgInfo());
            }
            functionHeadCalibrationDic[functionHeadID].LaserAltimetryCalibrationInfo = laserAltimetryCalibrationInfo;
        }
        /// <summary>
        /// 获取激光测高标定信息
        /// </summary>
        /// <param name="functionHeadID"></param>
        /// <param name="laserAltimetryCalibrationInfo"></param>
        public static void GetLaserAltimetryCalibrationInfo(string functionHeadID,
           out LaserAltimetryCalibrationInfo laserAltimetryCalibrationInfo)
        {
            if (!functionHeadCalibrationDic.ContainsKey(functionHeadID))
            {
                functionHeadCalibrationDic.Add(functionHeadID, new CalibrationCfgInfo());
            }
            laserAltimetryCalibrationInfo = functionHeadCalibrationDic[functionHeadID].LaserAltimetryCalibrationInfo;
        }

        #endregion

        #region 相机比例标定
        /// <summary>
        /// 设置激光测高标定信息
        /// </summary>
        /// <param name="functionHeadID"></param>
        /// <param name="laserAltimetryCalibrationInfo"></param>
        public static void SetCameraRatioCalibrationInfo(string functionHeadID,
            CameraRatioCalibrationInfo cameraRatioCalibrationInfo)
        {
            if (!functionHeadCalibrationDic.ContainsKey(functionHeadID))
            {
                functionHeadCalibrationDic.Add(functionHeadID, new CalibrationCfgInfo());
            }
            functionHeadCalibrationDic[functionHeadID].CameraRatioCalibrationInfo = cameraRatioCalibrationInfo;
        }
        /// <summary>
        /// 获取激光测高标定信息
        /// </summary>
        /// <param name="functionHeadID"></param>
        /// <param name="cameraRatioCalibrationInfo"></param>
        public static void GetCameraRatioCalibrationInfo(string functionHeadID,
           out CameraRatioCalibrationInfo cameraRatioCalibrationInfo)
        {
            if (!functionHeadCalibrationDic.ContainsKey(functionHeadID))
            {
                functionHeadCalibrationDic.Add(functionHeadID, new CalibrationCfgInfo());
            }
            cameraRatioCalibrationInfo = functionHeadCalibrationDic[functionHeadID].CameraRatioCalibrationInfo;
        }

        #endregion

        #region 飞拍标定
        /// <summary>
        /// 设置飞拍标定信息
        /// </summary>
        /// <param name="functionHeadID"></param>
        /// <param name="laserAltimetryCalibrationInfo"></param>
        public static void SetFlyingPhotoCalibrationInfo(string functionHeadID,
            FlyingPhotoCalibrationInfo flyingPhotoCalibrationInfo)
        {
            if (!functionHeadCalibrationDic.ContainsKey(functionHeadID))
            {
                functionHeadCalibrationDic.Add(functionHeadID, new CalibrationCfgInfo());
            }
            functionHeadCalibrationDic[functionHeadID].FlyingPhotoCalibrationInfo = flyingPhotoCalibrationInfo;
        }
        /// <summary>
        /// 获取飞拍标定信息
        /// </summary>
        /// <param name="functionHeadID"></param>
        /// <param name="flyingPhotoCalibrationInfo"></param>
        public static void GetFlyingPhotoCalibrationInfo(string functionHeadID,
           out FlyingPhotoCalibrationInfo flyingPhotoCalibrationInfo)
        {
            if (!functionHeadCalibrationDic.ContainsKey(functionHeadID))
            {
                functionHeadCalibrationDic.Add(functionHeadID, new CalibrationCfgInfo());
            }
            flyingPhotoCalibrationInfo = functionHeadCalibrationDic[functionHeadID].FlyingPhotoCalibrationInfo;
        }

        #endregion


        #endregion
    }
}
