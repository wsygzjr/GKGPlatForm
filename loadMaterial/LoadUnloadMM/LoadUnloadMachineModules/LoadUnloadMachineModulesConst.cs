using Griffins;
using Griffins.ImeIOT;
using GKG.SubMM;

namespace GKG
{
    namespace MM
    {
        public class LoadUnloadMachineModulesConst
        {
            /// <summary>总机械模组在宿主中的显示名称。</summary>
            public const string MMName = "上下料机械模组";

            /// <summary>总机械模组模型编号；用于插件注册和宿主识别。</summary>
            public const string MMModelStr = "LoadUnload";

            public readonly static MMNumber MMModel = MMNumber.Parse(MMModelStr);

            public readonly static Guid ContainerNameID = new Guid("{B096C228-E63F-4A1A-B242-AF06F1728396}");
            public readonly static Guid MagNameID = new Guid("{38A7E86E-AAB6-4642-8EF4-1AC271635E70}");

            public const string MemoMaterialBoxSubMM = "料盒子机械模组";
            public const string MemoMotorPushRodSubMM = "电机型推杆子机械模组";
            public const string MemoCylinderPushRodSubMM = "气缸型推杆子机械模组";

            public const string UIPropNameMaterialStatus = "料盒状态";
            public const string UIPropMemoMaterialStatus = "料盒子数据对象属性";

            public const string UICommandNameStorageOpen = "料盒张开";
            public const string UICommandNameStorageClose = "料盒夹紧";

            public const string ParamIDContainerName = "ContainerName";
            public const string ParamNameContainerName = "料盒容器名称";
            public const string ParamIDMagName = "MagName";
            public const string ParamNameMagName = "料盒名称";

            public const string SlotName = "料槽";

            /// <summary>总机械模组对外能力方法：当前只暴露上料、下料两个主流程入口。</summary>
            public static readonly ImeCompMethodDefInfoList CompMethods = new ImeCompMethodDefInfoList()
            {
                new ImeCompMethodDefInfo(UpfeedMethodID, "上料", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(DownfeedMethodID, "下料", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            };

            /// <summary>额外能力事件定义；当前总控主要通过返回值输出事件号，这里暂未单独注册。</summary>
            public static readonly ImeCompEventDefInfoList CompEvents = new ImeCompEventDefInfoList()
            {
            };

            /// <summary>发起完整上料主流程。</summary>
            public const string UpfeedMethodID = "Upfeed";
            /// <summary>发起完整下料主流程。</summary>
            public const string DownfeedMethodID = "Downfeed";

            /// <summary>普通方法：执行指定储料位料盒张开。</summary>
            public const string StorageOpenMethodID = "StorageOpen";
            /// <summary>普通方法：执行指定储料位料盒夹紧。</summary>
            public const string StorageCloseMethodID = "StorageClose";
            public const string GetMaterialStatus = "GetMaterialStatus";

            /// <summary>UIDataObj 中料盒容器状态属性的统一 PropertyID；值为序列化后的 MaterialContainerStatus JSON。</summary>
            public const string MaterialContainerStatusPropertyID = "MaterialContainerStatus";

            /// <summary>上料主流程整体执行成功。</summary>
            public const string UpfeedSuccessEventID = "UpfeedSuccess";
            /// <summary>上料主流程整体执行失败。</summary>
            public const string UpfeedFailEventID = "UpfeedFail";
            /// <summary>下料主流程整体执行成功。</summary>
            public const string DownfeedSuccessEventID = "DownfeedSuccess";
            /// <summary>下料主流程整体执行失败。</summary>
            public const string DownfeedFailEventID = "DownfeedFail";

            /// <summary>推杆动作过程中检测到卡料。</summary>
            public const string ReturnValuePushJam = "PushJam";
            /// <summary>推杆缩回动作失败。</summary>
            public const string ReturnValuePusherRetractFailed = "PusherRetractFailed";
            /// <summary>连续空推次数超过配方允许上限。</summary>
            public const string ReturnValueContinuousEmptyPushLimitReached = "ContinuousEmptyPushLimitReached";
            /// <summary>上料料盒已成功切换到下一槽位。</summary>
            public const string ReturnValueUpfeedMoveCompleted = "UpfeedMoveCompleted";
            /// <summary>上料料盒切换下一槽位失败。</summary>
            public const string ReturnValueUpfeedMoveFailed = "UpfeedMoveFailed";
            /// <summary>上料料盒已无可用槽位。</summary>
            public const string ReturnValueUpfeedCassetteEmpty = "UpfeedCassetteEmpty";
            /// <summary>上料位未检测到有效料盒。</summary>
            public const string ReturnValueUpfeedNoCassette = "UpfeedNoCassette";
            /// <summary>上料剩余物料不足，触发预警。</summary>
            public const string ReturnValueUpfeedMaterialInsufficientWarning = "UpfeedMaterialInsufficientWarning";
            /// <summary>下料料盒已成功切换到下一槽位。</summary>
            public const string ReturnValueDownfeedMoveCompleted = "DownfeedMoveCompleted";
            /// <summary>下料料盒切换下一槽位失败。</summary>
            public const string ReturnValueDownfeedMoveFailed = "DownfeedMoveFailed";
            /// <summary>下料料盒已满，无可继续放料槽位。</summary>
            public const string ReturnValueDownfeedCassetteFull = "DownfeedCassetteFull";
            /// <summary>下料位未检测到有效料盒。</summary>
            public const string ReturnValueDownfeedNoCassette = "DownfeedNoCassette";
            /// <summary>下料剩余可用槽位不足，触发预警。</summary>
            public const string ReturnValueDownfeedSlotInsufficientWarning = "DownfeedSlotInsufficientWarning";

            /// <summary>移动到首槽位置。</summary>
            public const string RtCmdMoveToFirstSlot = "MoveToFirstSlot";
            /// <summary>移动到末槽位置。</summary>
            public const string RtCmdMoveToLastSlot = "MoveToLastSlot";
            /// <summary>执行 Z 轴回零。</summary>
            public const string RtCmdZHomed = "ZHomed";
            /// <summary>Z 轴相对向上点动。</summary>
            public const string RtCmdZMoveUp = "ZMoveUp";
            /// <summary>Z 轴相对向下点动。</summary>
            public const string RtCmdZMoveDown = "ZMoveDown";
            /// <summary>立即停止当前 Z 轴运动。</summary>
            public const string RtCmdZAxisStop = "ZAxisStop";
            /// <summary>夹紧指定料盒位置的气缸。</summary>
            public const string RtCmdMagazineClamp = "MagazineClamp";
            /// <summary>松开指定料盒位置的气缸。</summary>
            public const string RtCmdMagazineUnclamp = "MagazineUnclamp";
            /// <summary>读取当前 Z 轴位置。</summary>
            public const string RtCmdGetAxisPos = "GetAxisPos";
            /// <summary>强制执行电机推杆前推动作。</summary>
            public const string RtCmdMotorPusherForward = "MotorPusherForward";
            /// <summary>强制执行电机推杆回退动作。</summary>
            public const string RtCmdMotorPusherBackward = "MotorPusherBackward";
            /// <summary>读取电机推杆当前状态。</summary>
            public const string RtCmdMotorPusherGetStatus = "MotorPusherGetStatus";
            /// <summary>强制执行气缸推杆前推动作。</summary>
            public const string RtCmdCylinderPusherForward = "CylinderPusherForward";
            /// <summary>强制执行气缸推杆回退动作。</summary>
            public const string RtCmdCylinderPusherBackward = "CylinderPusherBackward";
            /// <summary>读取气缸推杆当前状态。</summary>
            public const string RtCmdCylinderPusherGetStatus = "CylinderPusherGetStatus";
            /// <summary>
            /// 上料一次
            /// </summary>
            public const string RtCmdLoadOnce = "LoadOnce";
            /// <summary>
            /// 下料一次
            /// </summary>
            public const string RtCmdUnloadOnce = "UnloadOnce";
            public const string UpdateMaterialBoxStateMethodID = "UpdateMaterialBoxState";


            /// <summary>总模块内部依赖的子模组模型定义。</summary>
            public readonly static SubMMModel SubMMModelMaterialBox = MaterialBoxSubMachineModulesConst.SubMMModel;
            public readonly static SubMMModel SubMMModelMotorPushRod = PushRodSubMachineModulesConst.SubMMModel;
            public readonly static SubMMModel SubMMModelCylinderPushRod = PushRodSubMachineModulesConst.SubMMModel;

            /// <summary>总模块内部使用的子模组别名；用于创建和路由到对应执行器。</summary>
            
            /// <summary>
            /// 料盒容器
            /// </summary>
            public readonly static InnerAlias InnerAliasMaterialBox = InnerAlias.Parse("MaterialBox");
            /// <summary>
            /// 上料推杆
            /// </summary>
            public readonly static InnerAlias InnerAliasLoadPushRod = InnerAlias.Parse("MotorPushRod");
            /// <summary>
            /// 下料推杆
            /// </summary>
            public readonly static InnerAlias InnerAliasUnLoadPushRod = InnerAlias.Parse("CylinderPushRod");
        }
    }
}
