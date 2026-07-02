using GKG.SubMM;
using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    namespace MM
    {
        class MachineModulesCabilityDef : IMachineModulesCabilityDef
        {
            CompContainSubMMDataList IMachineModulesCabilityDef.SubMMs => new CompContainSubMMDataList
            {
                new CompContainSubMMData(FixingStructureMachineModulesConst.InnerAliasFixing,FixingStructureMachineModulesConst.SubMMModelFixing,"电机顶升子机械模组")
            };

            ImeCompEventDefInfoList IMachineModulesCabilityDef.Events { get => FixingStructureMachineModulesConst.CompEvents; }

            ImeCompMethodDefInfoList IMachineModulesCabilityDef.Methods { get => FixingStructureMachineModulesConst.CompMethods; }
            ImeCompPropDefInfoList IMachineModulesCabilityDef.UIDataObjProps => new ImeCompPropDefInfoList
            {
                //new ImeCompPropDefInfo(FixingStructureMachineModulesConst.FixingStatePropertyID,"当前顶升状态",Griffins.GfPropReadWrite.ReadOnly,Griffins.GriffinsBaseDataType.Bool)
            };
            ImeCompMethodDefInfoList IMachineModulesCabilityDef.UICommands => new ImeCompMethodDefInfoList
            {
                new ImeCompMethodDefInfo(FixingStructureMachineModulesConst.JackingCmdID,"顶升",new GFParamDefInfoList(),new GFParamDefInfoList(),false)
            };

            IMachineModulesDefSvr IMachineModulesCabilityDef.CreateIMachineModulesDefSvr(MMAlias alias, byte[] factoryCfgInfo)
            {
                return null!;
            }
        }
    }
}
