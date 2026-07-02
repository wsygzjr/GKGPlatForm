using Griffins;
using Griffins.ImeIOT;
using GKG.SubMM;

namespace GKG
{
    namespace MM
    {
        public class DeviceManagerMachineModulesConst
        {
            /// <summary>总机械模组在宿主中的显示名称。</summary>
            public const string MMName = "设备管理机械模组";

            /// <summary>总机械模组模型编号；用于插件注册和宿主识别。</summary>
            public const string MMModelStr = ImeIOTConst.DevMngMMStr;

            public readonly static MMNumber MMModel = MMNumber.Parse(MMModelStr);
            public const string MachineRunMode = "MachineRunMode";
            public const string CurFormulaNumber = "CurFormulaNumber";
            
            public const string MachineRunModeList = "MachineRunModeList";
            public const string FormulaNumberList = "FormulaNumberList";

            public const string ExecMode = "ExecMode";

            public const string MachineStartWork = "MachineStartWork";
            public const string MachineStopWork = "MachineStopWork";
            public const string SetExecMode = "SetExecMode";
            public const string NextStep = "NextStep";

            /// <summary>总机械模组对外能力方法：当前只暴露上料、下料两个主流程入口。</summary>
            public static readonly ImeCompMethodDefInfoList CompMethods = new ImeCompMethodDefInfoList()
            {
            };

            /// <summary>额外能力事件定义；当前总控主要通过返回值输出事件号，这里暂未单独注册。</summary>
            public static readonly ImeCompEventDefInfoList CompEvents = new ImeCompEventDefInfoList()
            {
            };

            public readonly static SubMMModel SubMMModelEletronicManager = EletronicManagerSubMachineModulesConst.SubMMModel;

            /// <summary>总模块内部使用的子模组别名；用于创建和路由到对应执行器。</summary>
            
            /// <summary>
            /// 电气管理
            /// </summary>
            public readonly static InnerAlias InnerAliasEletronicManager = InnerAlias.Parse("EletronicManager");
        }
    }
}
