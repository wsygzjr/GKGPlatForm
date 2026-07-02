using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GKG
{
    namespace MM
    {
        class MachineModulesCabilityDef : IMachineModulesCabilityDef
        {
            CompContainSubMMDataList IMachineModulesCabilityDef.SubMMs => new CompContainSubMMDataList()
            {
                new CompContainSubMMData(DeviceManagerMachineModulesConst.InnerAliasEletronicManager, DeviceManagerMachineModulesConst.SubMMModelEletronicManager)
                {
                    Memo = "电气管理子机械模组"
                },
            };

            ImeCompEventDefInfoList IMachineModulesCabilityDef.Events => DeviceManagerMachineModulesConst.CompEvents;

            ImeCompMethodDefInfoList IMachineModulesCabilityDef.Methods => DeviceManagerMachineModulesConst.CompMethods;

            ImeCompPropDefInfoList IMachineModulesCabilityDef.UIDataObjProps
            {
                get
                {
                    GFUIPropDefInfoList gFUIPropDefInfos = GFPropObjBase.GetGFPropDefInfoes<DeviceManagerStatus>(Resources.ResourceManager.GetString, getValueRangeEnums, getValueNamePairs);
                    ImeCompPropDefInfoList imeCompPropDefInfos = new ImeCompPropDefInfoList();
                    if (gFUIPropDefInfos != null)
                    {
                        foreach (var item in gFUIPropDefInfos)
                        {
                            ImeCompPropDefInfo imeCompPropDefInfo = new ImeCompPropDefInfo();
                            imeCompPropDefInfo.CopyFrom(item);
                            imeCompPropDefInfo.ReadWrite = item.ReadWrite;
                            imeCompPropDefInfos.Add(imeCompPropDefInfo);
                        }
                    }
                    return imeCompPropDefInfos;
                }
            }
            private static List<GriffinsBaseValue> getValueRangeEnums(string x)
            {
                switch (x)
                {
                    case nameof(DeviceManagerStatus.ImeRunMode):
                        {
                            List<GriffinsBaseValue> materialStatusEnums = new List<GriffinsBaseValue>();
                            foreach (ImeRunMode status in Enum.GetValues(typeof(ImeRunMode)))
                            {
                                materialStatusEnums.Add(new GriffinsBaseValue(status));
                                //取值范围值对应的显示名称或值含义名称，需在资源文件中定义
                            }
                            return materialStatusEnums;
                        }
                    case nameof(DeviceManagerStatus.ExecMode):
                        {
                            List<GriffinsBaseValue> materialStatusEnums = new List<GriffinsBaseValue>();
                            foreach (ImeExecMode status in Enum.GetValues(typeof(ImeExecMode)))
                            {
                                materialStatusEnums.Add(new GriffinsBaseValue(status));
                                //取值范围值对应的显示名称或值含义名称，需在资源文件中定义
                            }
                            return materialStatusEnums;
                        }
                    default:
                        return new List<GriffinsBaseValue>();
                }
            }

            private static GriffinsValueNamePairList getValueNamePairs(string x)
            {
                switch (x)
                {
                    case nameof(DeviceManagerStatus.ImeRunMode):
                        {
                            GriffinsValueNamePairList materialStatusValueNamePairs = new GriffinsValueNamePairList();
                            foreach (ImeRunMode status in Enum.GetValues(typeof(ImeRunMode)))
                            {
                                //取值范围值对应的显示名称或值含义名称，需在资源文件中定义
                                string cnName = Resources.ResourceManager.GetString(status.ToString());
                                materialStatusValueNamePairs.Add(new GriffinsValueNamePair(new GriffinsBaseValue(status), cnName));
                            }
                            return materialStatusValueNamePairs;
                        }
                    case nameof(DeviceManagerStatus.ExecMode):
                        {
                            GriffinsValueNamePairList materialStatusValueNamePairs = new GriffinsValueNamePairList();
                            foreach (ImeExecMode status in Enum.GetValues(typeof(ImeExecMode)))
                            {
                                //取值范围值对应的显示名称或值含义名称，需在资源文件中定义
                                string cnName = Resources.ResourceManager.GetString(status.ToString());
                                materialStatusValueNamePairs.Add(new GriffinsValueNamePair(new GriffinsBaseValue(status), cnName));
                            }
                            return materialStatusValueNamePairs;
                        }
                    default:
                        return new GriffinsValueNamePairList();
                }
            }
            ImeCompMethodDefInfoList IMachineModulesCabilityDef.UICommands => new ImeCompMethodDefInfoList
            {
                new ImeCompMethodDefInfo(DeviceManagerMachineModulesConst.MachineStartWork, "开始工作", new GFParamDefInfoList(), new GFParamDefInfoList(), false)
                {

                },
                new ImeCompMethodDefInfo(DeviceManagerMachineModulesConst.MachineStopWork, "停止工作", new GFParamDefInfoList(), new GFParamDefInfoList(), false)
                {

                }
            };

            IMachineModulesDefSvr IMachineModulesCabilityDef.CreateIMachineModulesDefSvr(MMAlias alias, byte[] factoryCfgInfo)
            {
                return null!;
            }
        }
        public class MachineModulesDefSvr : IMachineModulesDefSvr
        {
            Dictionary<string, string> IMachineModulesDefSvr.GetSubUIProObjItemNames(ObjInstPropPath objInstPropPath)
            {
                if(objInstPropPath.PropIDPath.Length == 1 && objInstPropPath.PropIDPath[0] == nameof(DeviceManagerStatus.RunModeList))
                {
                    return new Dictionary<string, string>
                    {
                        //{ "工作模式", "工作模式" },
                        //{ "配置模式", "配置模式" },
                        //{ "老化模式", "老化模式" },
                        { "RunModeList", "运行模式列表" },
                    };
                }
                else if (objInstPropPath.PropIDPath.Length == 1 && objInstPropPath.PropIDPath[0] == nameof(DeviceManagerStatus.FormulaNumberList))
                {
                    return new Dictionary<string, string>
                    {
                        //{ "工作模式", "工作模式" },
                        //{ "配置模式", "配置模式" },
                        //{ "老化模式", "老化模式" },
                        { "FormulaNumberList", "配方列表" },
                    };
                }
                return new Dictionary<string, string>();
            }

            void IMachineModulesDefSvr.Init(IMachineModulesDefSvrCallBack callBack)
            {
                throw new NotImplementedException();
            }
        }
    }
}
