using GF_Gereric;
using Griffins.ImeIOT;
using System;

[assembly: Plugin(Griffins.ImeIOT.SubMachineModulesMngAttribute.PLUGINKIND_Str, "{C480B657-B7DF-43B5-9DEC-D61A6B7D24D3}", "GKG.PushRodSubMachineModules")]

namespace GKG
{
    namespace SubMM
    {
        [SubMachineModulesMngAttribute(PushRodSubMachineModulesConst.SubMMModelStr, "LoadUnload")]
        class PushRodSubMachineModulesMain : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
        {
            void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
            {
            }

            string ISubMachineModulesMng.SubMMName => PushRodSubMachineModulesConst.SubMMName;

            SubMMObjInfoList ISubMachineModulesMng.SubMMObjInfos => new SubMMObjInfoList
            {
                new SubMMObjInfo
                {
                    SubMMObjID = PushRodSubMachineModulesConst.MotorSubMMObjID,
                    SubMMObjName = PushRodSubMachineModulesConst.MotorSubMMObjName,
                },
                new SubMMObjInfo
                {
                    SubMMObjID = PushRodSubMachineModulesConst.CylinderSubMMObjID,
                    SubMMObjName = PushRodSubMachineModulesConst.CylinderSubMMObjName,
                }
            };

            public SubMMObjInfoList SubMMObjInfos()
            {
                return new SubMMObjInfoList();
            }

            public ISubMachineModulesCabilityDef CretaeCabilityDef()
            {
                return new PushRodSubMachineModulesCabilityDef();
            }

            public ISubMMCmdExecutor CreateSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
            {
                if (subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID)
                {
                    return new MotorPushRodSubMMCmdExecutor(alias, subMMObjID, factoryCfgInfo);
                }

                if (subMMObjID == PushRodSubMachineModulesConst.CylinderSubMMObjID)
                {
                    return new CylinderPushRodSubMMCmdExecutor(alias, subMMObjID, factoryCfgInfo);
                }

                throw new ArgumentException($"Unknown PushRod subMMObjID: {subMMObjID}");
            }
        }
    }
}
