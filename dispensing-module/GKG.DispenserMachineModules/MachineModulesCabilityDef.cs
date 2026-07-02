using Griffins.ImeIOT;

namespace GKG
{
    namespace MM
    {
        internal class MachineModulesCabilityDef : IMachineModulesCabilityDef
        {
            /// <summary>
            /// 机械模组的机械模组能力事件列表
            /// </summary>
            ImeCabilityEventDefInfoList IMachineModulesCabilityDef.Events
            {
                get
                {
                    return DispenserMachineModulesConst.MMCabilityEventes;
                }
            }

            /// <summary>
            /// 机械模组的机械模组能力方法列表
            /// </summary>
            ImeCabilityMethodDefInfoList IMachineModulesCabilityDef.Methods
            {
                get
                {
                    return DispenserMachineModulesConst.MMCabilityMethodes;
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
                     new MMCabilitySubMMData(DispenserMachineModulesConst.Robot_Alias,DispenserMachineModulesConst.RobotSubMMModel)
                     {
                        Memo="机械手"
                     },
                     new MMCabilitySubMMData(DispenserMachineModulesConst.Vision_Alias,DispenserMachineModulesConst.VisionSubMMModel)
                     {
                        Memo="视觉分析"
                     },
                     new MMCabilitySubMMData(DispenserMachineModulesConst.MotionCalculate_Alias,DispenserMachineModulesConst.MotionCalculateSubMMModel)
                     {
                        Memo="运动计算"
                     },
                     new MMCabilitySubMMData(DispenserMachineModulesConst.Valve1_Alias,DispenserMachineModulesConst.Valve1SubMMModel)
                     {
                        Memo="阀1"
                     },
                     new MMCabilitySubMMData(DispenserMachineModulesConst.Valve2_Alias,DispenserMachineModulesConst.Valve2SubMMModel)
                     {
                        Memo="阀2"
                     }
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
}