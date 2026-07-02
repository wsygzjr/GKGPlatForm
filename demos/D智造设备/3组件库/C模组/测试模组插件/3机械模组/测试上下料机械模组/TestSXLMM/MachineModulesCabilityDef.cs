using Griffins.ImeIOT;
using StandardSXLMachineModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardSXLMachineModules
{
	public class MachineModulesCabilityDef: IMachineModulesCabilityDef
	{
        /// <summary>
        /// 机械模组的机械模组能力事件列表
        /// </summary>
        ImeCabilityEventDefInfoList IMachineModulesCabilityDef.Events
        {
            get 
            {
                return StandardSXLMachineModulesConst.ImeCabilityEventes;
            }
        }
        /// <summary>
        /// 机械模组的机械模组能力方法列表
        /// </summary>
        ImeCabilityMethodDefInfoList IMachineModulesCabilityDef.Methods
		{
            get 
            {
                return StandardSXLMachineModulesConst.MMCabilityMethodes;
            }
        }

        /// <summary>
        /// 机械模组包含的子机械模组能力数据列表
        /// </summary>
		MMCabilitySubMMDataList IMachineModulesCabilityDef.SubMMs 
        {
            get 
            {
                return new MMCabilitySubMMDataList()
                {
                   new MMCabilitySubMMData(StandardSXLMachineModulesConst.SXL_SL_TL001Alias,StandardSXLMachineModulesConst.SXL_SL_TL001),
                   new MMCabilitySubMMData(StandardSXLMachineModulesConst.SXL_XL_TL001Alias,StandardSXLMachineModulesConst.SXL_XL_TL001),
                };
            }
        }

        /// <summary>
        /// 机械模组包含的基础软件组件能力数据列表
        /// </summary>
		MMCabilityBscDataList IMachineModulesCabilityDef.Bscs 
        {
            get 
            {
                return new MMCabilityBscDataList();
            }
        }
	}
}
