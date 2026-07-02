using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;
using System;

namespace GKG.SubMM
{
    public class FixingStructureSubMachineModulesConst
    {
        public const string SubMMName = "固定机构";
        public const string CylinderSubMMName = "气缸固定机构";
        public const string AxisSubMMName = "电机固定机构";

        public const string SubMMModelStr = "FixingStructure";

        public static readonly SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

        public static readonly SubMMObjInfoList SubMMObjInfos = new()
        {
            new SubMMObjInfo
            {
                SubMMObjID = Guid.Parse("{3F52C865-78F8-46E0-BD5C-2F2C49F35A2D}"),
                SubMMObjName = CylinderSubMMName,
            },
            new SubMMObjInfo
            {
                SubMMObjID = Guid.Parse("{333B930C-165E-4C5C-B5F2-8FD45D728A95}"),
                SubMMObjName = AxisSubMMName,
            },
        };

        /// <summary>
        /// 固定
        /// </summary>
        public const string FixingMethodID = "Fixing";
        /// <summary>
        /// 取消固定
        /// </summary>
        public const string ReleaseFixingMethodID = "ReleaseFixing";

        public const string RtCmdGetIOOptions = "GetIOOptions";
        public const string RtCmdGetAxisOptions = "GetAxisOptions";

        public const string RtCmdGetFixingState = "GetFixingState";

        public const string RtCmdMoveTo = "MoveTo";

        public const string RtCmdContinueMove = "ContinueMove";

        public const string RtCmdStop = "Stop";

        public const string FixingState = "FixingState";

        public const string FixingFinishedEventID = "Fixing_Finished";
        public const string FixingFailedEventID = "Fixing_Failed";

        public const string ReleaseFixingFinishedEventID = "ReleaseFixing_Finished";
        public const string ReleaseFixingFailedEventID = "ReleaseFixing_Failed";

        public static readonly ImeCompMethodDefInfoList CompMethods = new()
        {
            new ImeCompMethodDefInfo(FixingMethodID, "固定", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(ReleaseFixingMethodID, "解除固定", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
        };

        public static readonly ImeCompEventDefInfoList CompEvents = new()
        {
            new ImeCompEventDefInfo(FixingFinishedEventID, "固定完成", new GFParamDefInfoList()),
            new ImeCompEventDefInfo(FixingFailedEventID, "固定失败", new GFParamDefInfoList()),
            new ImeCompEventDefInfo(ReleaseFixingFinishedEventID, "解除固定完成", new GFParamDefInfoList()),
            new ImeCompEventDefInfo(ReleaseFixingFailedEventID, "解除固定失败", new GFParamDefInfoList()),
        };

        public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new()
        {
            new ImeCompMethodDefInfo(FixingMethodID, "固定", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(ReleaseFixingMethodID, "解除固定", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
        };
    }
}
