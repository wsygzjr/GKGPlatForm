using GKG.SubMM;
using Griffins;
using Griffins.ImeIOT;

namespace GKG
{
    namespace MM
    {
        public class FixingStructureMachineModulesConst
        {
            public const string MMName = "顶升固定机械模组";

            public const string MMModelStr = "Fixing";

            public readonly static MMNumber MMModel = MMNumber.Parse(MMModelStr);

            #region 能力方法ID
            /// <summary>
            /// 固定
            /// </summary>
            public const string FixingMethodID = "Fixing";
            /// <summary>
            /// 取消固定
            /// </summary>
            public const string ReleaseFixingMethodID = "ReleaseFixing";
            #endregion

            #region 能力事件ID
            public const string FixingFinishedEventID = "FixingFinished";
            public const string FixingFailedEventID = "FixingFailed";

            public const string ReleaseFixingFinishedEventID = "ReleaseFixingFinished";
            public const string ReleaseFixingFailedEventID = "ReleaseFixingFailed";
            #endregion

            #region 界面数据对象属性ID
            public const string FixingStatePropertyID = "FixingState";
            #endregion
            #region 界面数据对象命令ID
            public const string JackingCmdID = "Jacking";
            #endregion
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

            /// <summary>
            /// 轨道运输电机子机械模组
            /// </summary>
			public readonly static SubMMModel SubMMModelFixing = SubMMModel.Parse(FixingStructureSubMachineModulesConst.SubMMModelStr);
            public readonly static InnerAlias InnerAliasFixing = InnerAlias.Parse("FixingStructure1");
        }
    }
}
