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
		class SubMachineModulesCabilityDef : ISubMachineModulesCabilityDef
		{
            ImeCompEventDefInfoList ISubMachineModulesCabilityDef.Events => EletronicManagerSubMachineModulesConst.CompEvents;
            ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.Methods => EletronicManagerSubMachineModulesConst.CompMethods;
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
