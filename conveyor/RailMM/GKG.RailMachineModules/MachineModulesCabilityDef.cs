using GKG.SubMM;
using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
	namespace MM
	{
		class MachineModulesCabilityDef : IMachineModulesCabilityDef
		{
            CompContainSubMMDataList IMachineModulesCabilityDef.SubMMs => new CompContainSubMMDataList()
            {
                new CompContainSubMMData(RailMachineModulesConst.InnerAliasLeftWorkStation, RailWorkStationSubMachineModulesConst.SubMMModel)
                {
                    Memo = "左工位子机械模组"
                },
                new CompContainSubMMData(RailMachineModulesConst.InnerAliasMiddleWorkStation, RailWorkStationSubMachineModulesConst.SubMMModel)
                {
                    Memo = "中间工位子机械模组"
                },
                new CompContainSubMMData(RailMachineModulesConst.InnerAliasRightWorkStation, RailWorkStationSubMachineModulesConst.SubMMModel)
                {
                    Memo = "右工位子机械模组"
                },
                new CompContainSubMMData(RailMachineModulesConst.InnerAliasRailMotor, RailMotorSubMachineModulesConst.SubMMModel)
                {
                    Memo = "运输电机子机械模组"
                },
            };

            ImeCompEventDefInfoList IMachineModulesCabilityDef.Events { get => RailMachineModulesConst.CompEvents; }

            ImeCompMethodDefInfoList IMachineModulesCabilityDef.Methods { get => RailMachineModulesConst.CompMethods; }
            ImeCompPropDefInfoList IMachineModulesCabilityDef.UIDataObjProps { get => null; }
            ImeCompMethodDefInfoList IMachineModulesCabilityDef.UICommands { get => new ImeCompMethodDefInfoList
            {
				new ImeCompMethodDefInfo(RailMachineModulesConst.InletPanelMethodID, "进板", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(RailMachineModulesConst.OutletPanelMethodID, "出板", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            }; 
            }

            IMachineModulesDefSvr IMachineModulesCabilityDef.CreateIMachineModulesDefSvr(MMAlias alias, byte[] factoryCfgInfo)
            {
                return null!;
            }
        }
	}
}
