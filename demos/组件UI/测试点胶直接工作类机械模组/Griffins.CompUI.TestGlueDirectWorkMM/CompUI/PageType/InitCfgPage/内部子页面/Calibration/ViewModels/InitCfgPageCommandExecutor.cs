using GF_Gereric;
using Griffins.Map.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModels
{
    /// <summary>
    /// 初始化页面的命令执行器
    /// </summary>
    public partial class InitCfgPageCommandExecutor
    {
        /// <summary>
        /// 页面类型运行时回调
        /// </summary>
        private IPageTypeRunTimeCallBack? _callBack;
        /// <summary>
        /// 标定模组别名
        /// </summary>
        private string _calibrationTechnicalModulesAlias="";
        private static InitCfgPageCommandExecutor? _controCommandExecutor;
        public static InitCfgPageCommandExecutor Instance
        {
            get
            {
                if (_controCommandExecutor == null)
                    _controCommandExecutor = new InitCfgPageCommandExecutor();
                return _controCommandExecutor;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="callBack"></param>
        public void Init(IPageTypeRunTimeCallBack? callBack)
        {
            _callBack = callBack;
        }

        #region 标定模组执行命令
        /// <summary>
        /// 设置标定模组别名
        /// </summary>
        /// <param name="calibrationTechnicalModulesAlias">标定模组别名</param>
        public void SetCalibrationTechnicalModulesAlias(string calibrationTechnicalModulesAlias)
        {
            this._calibrationTechnicalModulesAlias = calibrationTechnicalModulesAlias;
        }

        ///// <summary>
        ///// 针头到命令
        ///// </summary>
        ///// <param name="functionHeadID">功能头ID</param>
        ///// <param name="calibrationType">标定类型</param>
        ///// <param name="coordinates">移动到的坐标</param>
        ///// <returns></returns>
        //public async Task NeedleMoveTo(string functionHeadID, CalibrationType calibrationType, Point3D coordinates)
        //{
        //    var param = new CalibrationCmdExecutor.NeedleMoveTo_Param(functionHeadID, calibrationType, coordinates);
        //    string paramStr = JsonObjConvert.ToJSon(param);

        //    string respStr = exeCmd("NeedleMoveTo",paramStr);
        //    var respData = JsonObjConvert.FromJSon<CalibrationCmdExecutor.NeedleMoveTo_Response>(respStr);
        //    if (!respData.Success)
        //        throw new Exception(respData.ErrorMessage);
        //}

        ///// <summary>
        ///// 相机到
        ///// </summary>
        ///// <param name="functionHeadID">功能头ID</param>
        ///// <param name="calibrationType">标定类型</param>
        ///// <param name="coordinates">移动到的坐标</param>
        ///// <returns></returns>
        //public async Task CamreaMoveTo(string functionHeadID, CalibrationType calibrationType, Point3D coordinates)
        //{
        //    var param = new CalibrationCmdExecutor.CamreaMoveTo_Param(functionHeadID, calibrationType, coordinates);
        //    string paramStr = JsonObjConvert.ToJSon(param);

        //    string respStr = exeCmd("CamreaMoveTo", paramStr);
        //    var respData = JsonObjConvert.FromJSon<CalibrationCmdExecutor.CamreaMoveTo_Response>(respStr);
        //    if (!respData.Success)
        //        throw new Exception(respData.ErrorMessage);
        //}

        ///// <summary>
        ///// 激光到
        ///// </summary>
        ///// <param name="functionHeadID">功能头ID</param>
        ///// <param name="calibrationType">标定类型</param>
        ///// <param name="coordinates">移动到的坐标</param>
        ///// <returns></returns>
        //public async Task LaserMoveTo(string functionHeadID, CalibrationType calibrationType, Point3D coordinates)
        //{
        //    var param = new CalibrationCmdExecutor.LaserMoveTo_Param(functionHeadID, calibrationType, coordinates);
        //    string paramStr = JsonObjConvert.ToJSon(param);

        //    string respStr = exeCmd("LaserMoveTo", paramStr);
        //    var respData = JsonObjConvert.FromJSon<CalibrationCmdExecutor.LaserMoveTo_Response>(respStr);
        //    if (!respData.Success)
        //        throw new Exception(respData.ErrorMessage);
        //}

        ///// <summary>
        ///// 出胶
        ///// </summary>
        ///// <param name="functionHeadID">功能头ID</param>
        ///// <param name="calibrationType">标定类型</param>
        ///// <param name="pointCount">出胶点数</param>
        ///// <returns></returns>
        //public async Task OutGlue(string functionHeadID, CalibrationType calibrationType, int pointCount)
        //{
        //    var param = new CalibrationCmdExecutor.OutGlue_Param(functionHeadID, calibrationType, pointCount);
        //    string paramStr = JsonObjConvert.ToJSon(param);

        //    string respStr = exeCmd("OutGlue", paramStr);
        //    var respData = JsonObjConvert.FromJSon<CalibrationCmdExecutor.OutGlue_Response>(respStr);
        //    if (!respData.Success)
        //        throw new Exception(respData.ErrorMessage);
        //}

        ///// <summary>
        ///// 标定
        ///// </summary>
        ///// <param name="functionHeadID">功能头ID</param>
        ///// <param name="calibrationType">标定类型</param>
        ///// <param name="calibrationParams"></param>
        ///// <returns></returns>
        //public async Task<CalibrationResultBase> Calibrate(string functionHeadID, CalibrationType calibrationType, CalibrationParameters calibrationParams)
        //{
        //    var param = new CalibrationCmdExecutor.Calibrate_Param(functionHeadID, calibrationType, calibrationParams);
        //    string paramStr = param.ToJson();

        //    string respStr = exeCmd("Calibrate", paramStr);
        //    var response = new Calibrate_Response();
        //    response.FromJson(respStr);
        //    if (!response.Success)
        //        throw new Exception(response.ErrorMessage);
        //    return response.CalibrationResults;
        //}

        ///// <summary>
        ///// 获取标定结果
        ///// </summary>
        ///// <param name="functionHeadID">功能头ID</param>
        ///// <param name="calibrationType">标定类型</param>
        ///// <returns></returns>
        //public async Task<CalibrationResultBase> GetCalibrationResult(string functionHeadID, CalibrationType calibrationType)
        //{
        //    var param = new CalibrationCmdExecutor.GetCalibrationResult_Param(functionHeadID, calibrationType);
        //    string paramStr = param.ToJson();

        //    string respStr = exeCmd("GetCalibrationResult", paramStr);
        //    var response = new GetCalibrationResult_Response();
        //    response.FromJson(respStr);
        //    if (!response.Success)
        //        throw new Exception(response.ErrorMessage);
        //    return response.CalibrationResults;
        //}

        /// <summary>
        /// 旋转90度
        /// </summary>
        /// <returns></returns>
        public async Task RotateNinetyDegreesCommand()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 获取标定对象列表??
        /// </summary>
        /// <returns></returns>
        public  List<object> GetCalibrationObjects()
        {
            throw new NotImplementedException ();  
        }
        /// <summary>
        /// 获取功能头列表??
        /// </summary>
        /// <returns></returns>
        public  List<object> GetFunctionHeadsMethodID()
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="paramStr"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string exeCmd(string cmdID, string paramStr)
        {
            if (_callBack == null)
                throw new Exception("未设置执行命令回调实例");
            throw new NotImplementedException();
            //return _callBack.ExecCmd(_calibrationTechnicalModulesAlias, cmdID, paramStr);
        }
    }
}
