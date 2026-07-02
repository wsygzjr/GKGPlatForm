using Griffins;
using Griffins.ImeIOT;

namespace GKG.SubMM
{
    public class CalibrationSubMachineModulesConst
    {
        /// <summary>
        /// 标定
        /// </summary>
        public const string SubMMName = "标定";

        public const string SubMMModelStr = "Calibration";

        public static readonly SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

        /// <summary>
        /// 获取标定结果
        /// </summary>
        public const string GetCalibrationResultMethodID = "GetCalibrationResult";

        /// <summary>
        /// 获取标定对象列表
        /// </summary>
        public const string GetCalibrationObjectsMethodID = "GetCalibrationObjects";

        /// <summary>
        /// 获取功能头列表
        /// </summary>
        public const string GetFunctionHeadsMethodID = "GetFunctionHeads";

        /// <summary>
        /// 针头移动到指定位置
        /// </summary>
        public const string RunTimeCtlCmdNeedleMoveTo = "NeedleMoveTo";

        /// <summary>
        /// 相机移动到指定位置
        /// </summary>
        public const string RunTimeCtlCmdCamreaMoveTo = "CamreaMoveTo";

        /// <summary>
        /// 激光移动到指定位置
        /// </summary>
        public const string RunTimeCtlCmdLaserMoveTo = "LaserMoveTo";

        /// <summary>
        /// 出胶
        /// </summary>
        public const string RunTimeCtlCmdOutGlue = "OutGlue";

        /// <summary>
        /// 创建视觉模板
        /// </summary>
        public const string RunTimeCtlCmdCreateModel = "CreateModel";

        /// <summary>
        /// 查找Mark
        /// </summary>
        public const string RunTimeCtlCmdSearchMark = "SearchMark";

        /// <summary>
        /// 执行标定
        /// </summary>
        public const string RunTimeCtlCmdCalibrate = "Calibrate";

        /// <summary>
        /// 获取标定结果
        /// </summary>
        public const string RunTimeCtlCmdGetCalibrationResult = "GetCalibrationResult";

        /// <summary>
        /// 子机械模组能力方法定义信息列表
        /// 约定能力方法产生的能力事件，用 ImeCompMethodDefInfo.GetFinishedEventID() 产生
        /// </summary>
        public static readonly ImeCompMethodDefInfoList Methods = new ImeCompMethodDefInfoList()
        {
        };

        /// <summary>
        /// 子机械模组能力事件定义信息列表
        /// 不是和能力方法匹配的其他能力事件
        /// </summary>
        public static readonly ImeCompEventDefInfoList Events = new ImeCompEventDefInfoList()
        {
        };

        public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new ImeCompMethodDefInfoList()
        {
            new ImeCompMethodDefInfo(GetCalibrationResultMethodID, "获取标定结果", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(GetCalibrationObjectsMethodID, "获取标定对象列表", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(GetFunctionHeadsMethodID, "获取功能头列表", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
        };
        public static SubMMObjInfoList SubMMObjInfos = new SubMMObjInfoList
        {
            new SubMMObjInfo()
            {
                    SubMMObjID = Guid.Parse("{29904FBC-8B6F-4A30-9EDE-3BE4A3ABD5A0}"),
                    SubMMObjName = "通用标定对象",
            }
        };
    }
}