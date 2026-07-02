using Griffins;
using Griffins.ImeIOT;
using GKG.SubMM;
using System;
using System.Collections.Generic;
using GKG.LoadUnloadMachineModules.Properties;

namespace GKG
{
    namespace MM
    {
        public class MachineModulesCabilityDef : IMachineModulesCabilityDef
        {
            CompContainSubMMDataList IMachineModulesCabilityDef.SubMMs => new CompContainSubMMDataList()
            {
                new CompContainSubMMData(LoadUnloadMachineModulesConst.InnerAliasMaterialBox, LoadUnloadMachineModulesConst.SubMMModelMaterialBox)
                {
                    Memo = LoadUnloadMachineModulesConst.MemoMaterialBoxSubMM
                },
                new CompContainSubMMData(LoadUnloadMachineModulesConst.InnerAliasLoadPushRod, LoadUnloadMachineModulesConst.SubMMModelMotorPushRod)
                {
                    Memo = LoadUnloadMachineModulesConst.MemoMotorPushRodSubMM
                },
                new CompContainSubMMData(LoadUnloadMachineModulesConst.InnerAliasUnLoadPushRod, LoadUnloadMachineModulesConst.SubMMModelCylinderPushRod)
                {
                    Memo = LoadUnloadMachineModulesConst.MemoCylinderPushRodSubMM
                },
            };

            ImeCompEventDefInfoList IMachineModulesCabilityDef.Events => LoadUnloadMachineModulesConst.CompEvents;

            ImeCompMethodDefInfoList IMachineModulesCabilityDef.Methods => LoadUnloadMachineModulesConst.CompMethods;

            ImeCompPropDefInfoList IMachineModulesCabilityDef.UIDataObjProps
            {
                get
                {
                    GFUIPropDefInfoList gFUIPropDefInfos = GFPropObjBase.GetGFPropDefInfoes<MaterialContainerStatusList>(Resources.ResourceManager.GetString, getValueRangeEnums, getValueNamePairs);
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

            public static List<GriffinsBaseValue>  getValueRangeEnums(string x)
            {
                switch (x)
                {
                    case nameof(SlotStatus.MaterialStatus):
                        {
                            List<GriffinsBaseValue> materialStatusEnums = new List<GriffinsBaseValue>();
                            foreach (MaterialStatus status in Enum.GetValues(typeof(MaterialStatus)))
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

            public static GriffinsValueNamePairList getValueNamePairs(string x)
            {
                switch (x)
                {
                    case nameof(SlotStatus.MaterialStatus):
                        {
                            GriffinsValueNamePairList materialStatusValueNamePairs = new GriffinsValueNamePairList();
                            foreach (MaterialStatus status in Enum.GetValues(typeof(MaterialStatus)))
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

            ImeCompMethodDefInfoList IMachineModulesCabilityDef.UICommands => new ImeCompMethodDefInfoList()
            {
                new ImeCompMethodDefInfo(
                    LoadUnloadMachineModulesConst.StorageOpenMethodID, LoadUnloadMachineModulesConst.UICommandNameStorageOpen,
                    new Griffins.GFParamDefInfoList(){
                        new GFParamDefInfo {
                            DataType = GriffinsBaseDataType.String,
                            ObjectID = LoadUnloadMachineModulesConst.ContainerNameID,
                            ParamID = LoadUnloadMachineModulesConst.ParamIDContainerName ,
                            ParamName = LoadUnloadMachineModulesConst.ParamNameContainerName
                        },
                        new GFParamDefInfo {
                            DataType = GriffinsBaseDataType.String,
                            ObjectID = LoadUnloadMachineModulesConst.MagNameID,
                            ParamID = LoadUnloadMachineModulesConst.ParamIDMagName ,
                            ParamName = LoadUnloadMachineModulesConst.ParamNameMagName
                        }
                    },new Griffins.GFParamDefInfoList(),false),
                new ImeCompMethodDefInfo(
                    LoadUnloadMachineModulesConst.StorageCloseMethodID, LoadUnloadMachineModulesConst.UICommandNameStorageClose,
                    new Griffins.GFParamDefInfoList(){
                         new GFParamDefInfo {
                            DataType = GriffinsBaseDataType.String,
                            ObjectID = LoadUnloadMachineModulesConst.ContainerNameID,
                            ParamID = LoadUnloadMachineModulesConst.ParamIDContainerName ,
                            ParamName = LoadUnloadMachineModulesConst.ParamNameContainerName
                        },
                        new GFParamDefInfo {
                            DataType = GriffinsBaseDataType.String,
                            ObjectID = LoadUnloadMachineModulesConst.MagNameID,
                            ParamID = LoadUnloadMachineModulesConst.ParamIDMagName ,
                            ParamName = LoadUnloadMachineModulesConst.ParamNameMagName
                        }
                    },new Griffins.GFParamDefInfoList(),false)
            };

            IMachineModulesDefSvr IMachineModulesCabilityDef.CreateIMachineModulesDefSvr(MMAlias alias, byte[] factoryCfgInfo)
            {
                return new MachineModulesDefSvr(factoryCfgInfo);
            }
        }

        /// <summary>
        /// 机械模组定义信息服务对象
        /// </summary>
        public class MachineModulesDefSvr : IMachineModulesDefSvr
        {
            private IMachineModulesDefSvrCallBack _callBack;
            private readonly LoadUnloadMachineModulesFactoryCfg factoryCfg;
            private Dictionary<string, string> containersNameDic;
            private Dictionary<string, string> materialBoxNameDic;
            private Dictionary<string, string> slotObjNameDic;

            public MachineModulesDefSvr(byte[] factoryCfgInfo)
            {
                factoryCfg = new LoadUnloadMachineModulesFactoryCfg();
                if (factoryCfgInfo != null && factoryCfgInfo.Length > 0)
                    factoryCfg.FromBytes(factoryCfgInfo);
            }

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="callBack">机械模组运行时回调接口</param>
            void IMachineModulesDefSvr.Init(IMachineModulesDefSvrCallBack callBack)
            {
                _callBack = callBack;
            }

            /// <summary>
            /// 获取子界面数据对象项名称字典
            /// </summary>
            /// <param name="objInstPropPath">界面数据对象属性路径</param>
            /// <returns>子界面数据对象项名称字典</returns>
            Dictionary<string, string> IMachineModulesDefSvr.GetSubUIProObjItemNames(ObjInstPropPath objInstPropPath)
            {
                return getSubUIProObjItemNames(objInstPropPath);
            }

            /// <summary>
            /// 参考测试插件 getSubUIProObjItemNames 的路径处理方式，
            /// 按 PropIDPath 不同层级返回对应字典。
            /// </summary>
            private Dictionary<string, string> getSubUIProObjItemNames(ObjInstPropPath objInstPropPath)
            {
                _ = factoryCfg;
                var proIDPath = objInstPropPath.PropIDPath;
                if (proIDPath == null || proIDPath.Length == 0)
                    return null;

                // MaterialContainerStatusList.MaterialContainers
                if (proIDPath.Length == 1 && nameof(MaterialContainerStatusList.MaterialContainers) == proIDPath[0])
                {
                    if (containersNameDic == null)
                        containersNameDic = createContainersNameDic();
                    return containersNameDic;
                }

                // MaterialContainerStatus.MaterialBoxes
                if (proIDPath.Length == 2 && nameof(MaterialContainerStatus.MaterialBoxes) == proIDPath[1])
                {
                    if (materialBoxNameDic == null)
                        materialBoxNameDic = createMaterialBoxNameDic();
                    return materialBoxNameDic;
                }

                // MaterialBoxStatus.SlotStatusList
                if (proIDPath.Length == 3 && nameof(MaterialBoxStatus.SlotStatusList) == proIDPath[2])
                {
                    if (slotObjNameDic == null)
                        slotObjNameDic = createSlotObjNameDic();
                    return slotObjNameDic;
                }

                return null;
            }

            /// <summary>
            /// 容器字典：key 为容器对象键，value 为容器显示名。
            /// </summary>
            private static Dictionary<string, string> createContainersNameDic()
            {
                return new Dictionary<string, string>
                {
                    { MaterialBoxSubMachineModulesConst.LoadStorageDeviceName, MaterialBoxSubMachineModulesConst.LoadStorageDeviceName },
                    { MaterialBoxSubMachineModulesConst.UnloadStorageDeviceName, MaterialBoxSubMachineModulesConst.UnloadStorageDeviceName }
                };
            }

            /// <summary>
            /// 料盒字典：value 以 UpperRack/LowerRack 结尾，兼容总控中的名称解析逻辑。
            /// </summary>
            private Dictionary<string, string> createMaterialBoxNameDic()
            {
                return new Dictionary<string, string>
                {
                    { MaterialBoxSubMachineModulesConst.UpperRackName, $"{MaterialBoxSubMachineModulesConst.UpperRackName}" },
                    { MaterialBoxSubMachineModulesConst.LowerRackName, $"{MaterialBoxSubMachineModulesConst.LowerRackName}" }
                };
            }

            /// <summary>
            /// 默认给两个槽位名称；后续若有配置信息可在此扩展。
            /// </summary>
            private static Dictionary<string, string> createSlotObjNameDic()
            {
                //Dictionary<string, string> pairs = new Dictionary<string, string>();
                //for (int i = 0; i < 5; i++)
                //{
                //    pairs.Add($"{i}", $"{LoadUnloadMachineModulesConst.SlotName}{i}");
                //}
                //return pairs;
                return new Dictionary<string, string>{
                    { "SlotStatusList" ,"SlotStatusList"}
                };
            }
        }
    }
}
