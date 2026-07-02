using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
	namespace SubMM
	{
        /// <summary>
        /// 料盒子机械模组能力定义。
        /// 这里只暴露料盒子机械模组自身的能力方法；储料装置对象和运输机构对象的方法统一作为普通方法处理。
        /// 当前 Griffins 子模组能力定义接口未提供普通方法元数据单独注册入口，
        /// 因此普通方法仅通过执行器的 ExecMethod 承载，不在这里挂出。
        /// </summary>
		class SubMachineModulesCabilityDef : ISubMachineModulesCabilityDef
		{
            ImeCompEventDefInfoList ISubMachineModulesCabilityDef.Events => MaterialBoxSubMachineModulesConst.CompEvents;
            ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.Methods => MaterialBoxSubMachineModulesConst.CompMethods;
            ImeCompPropDefInfoList ISubMachineModulesCabilityDef.UIDataObjProps => null!;
            ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.UICommands => null!;
            DevicePropertyInfoList ISubMachineModulesCabilityDef.DeviceProps => null!;

            ISubMachineModulesDefSvr ISubMachineModulesCabilityDef.CreateISubMachineModulesDefSvr(SubMMAlias alias, byte[] factoryCfgInfo, GFBaseTypePropValueList devicePropValues)
            {
                return null!;
            }
        }
	}
}
