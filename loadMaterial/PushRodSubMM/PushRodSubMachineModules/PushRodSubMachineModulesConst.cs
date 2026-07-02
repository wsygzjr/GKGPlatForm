using Griffins;
using Griffins.ImeIOT;

namespace GKG
{
    namespace SubMM
    {
        public class PushRodSubMachineModulesConst
        {
            public const string SubMMName = "推杆子机械模块";
            public const string SubMMModelStr = "PushRod";
            public static readonly SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);

            public static readonly Guid MotorSubMMObjID = Guid.Parse("34A3A81E-A741-458C-AECE-4706811DFECA");
            public static readonly Guid CylinderSubMMObjID = Guid.Parse("4CCC3359-F92C-4372-B35D-EE57E26ADCE6");

            public const string MotorSubMMObjName = "电机推杆1";
            public const string CylinderSubMMObjName = "气缸推杆1";

            public static readonly ImeCompMethodDefInfoList CompMethods = new ImeCompMethodDefInfoList()
            {
                new ImeCompMethodDefInfo(ExtendMethodID, "伸出", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(RetractMethodID, "缩回", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(CheckHasMaterialMethodID, "检测是否有料", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(SetLoadFinishedMethodID, "设置上料完成", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(SetUnloadFinishedMethodID, "设置下料完成", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            };

            public static readonly ImeCompEventDefInfoList CompEvents = new ImeCompEventDefInfoList()
            {
            };

            public const string ExtendMethodID = "Extend";
            public const string RetractMethodID = "Retract";
            public const string CheckHasMaterialMethodID = "CheckHasMaterial";
            public const string GetInitParametersMethodID = "GetInitParameters";
            public const string GetRecipeParametersMethodID = "GetRecipeParameters";

            public const string LoadMaterialMethodID = ExtendMethodID;
            public const string UnloadMaterialMethodID = RetractMethodID;

            public const string SetLoadFinishedMethodID = "SetLoadFinished";
            public const string SetUnloadFinishedMethodID = "SetUnloadFinished";

            public const string EventPusherForwardCompleted = "PusherForwardCompleted";
            public const string EventPusherBackwardCompleted = "PusherBackwardCompleted";
            public const string EventPusherExtendFailed = "PusherExtendFailed";
            public const string EventPusherRetractFailed = "PusherRetractFailed";
            public const string EventPushJam = "PushJam";

            public const string RtCmdPusherForward = "PusherForward";
            public const string RtCmdPusherBackward = "PusherBackward";
            public const string RtCmdPushOnce = "PushOnce";
            public const string RtCmdGetStatus = "GetStatus";
            public const string RtCmdGetCylinderIOChannelOptions = "GetIOInfos";
            public const string RtCmdGetAxisOptions = "GetAxisInfos";

            public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = CompMethods;
        }
    }
}
