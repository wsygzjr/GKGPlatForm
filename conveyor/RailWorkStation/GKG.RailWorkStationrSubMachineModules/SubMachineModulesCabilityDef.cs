using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;

namespace GKG
{
    namespace SubMM
    {
        class SubMachineModulesCabilityDef : ISubMachineModulesCabilityDef
        {
            ImeCompEventDefInfoList ISubMachineModulesCabilityDef.Events => RailWorkStationSubMachineModulesConst.CompEvents;

            ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.Methods => RailWorkStationSubMachineModulesConst.CompMethods;

            ImeCompPropDefInfoList ISubMachineModulesCabilityDef.UIDataObjProps
            {
                get
                {
                    GFUIPropDefInfoList gFUIPropDefInfos = GFPropObjBase.GetGFPropDefInfoes<RailWorkStationStatus>(Resources.ResourceManager.GetString, getValueRangeEnums, getValueNamePairs);
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
                    case nameof(RailWorkStationStatus.LeftCylinderState):
                    case nameof(RailWorkStationStatus.RightCylinderState):
                        {
                            List<GriffinsBaseValue> materialStatusEnums = new List<GriffinsBaseValue>();
                            foreach (ECylinderPosType status in Enum.GetValues(typeof(ECylinderPosType)))
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
                    case nameof(RailWorkStationStatus.LeftCylinderState):
                    case nameof(RailWorkStationStatus.RightCylinderState):
                        {
                            GriffinsValueNamePairList materialStatusValueNamePairs = new GriffinsValueNamePairList();
                            foreach (ECylinderPosType status in Enum.GetValues(typeof(ECylinderPosType)))
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
            ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.UICommands => new ImeCompMethodDefInfoList
            {
                new ImeCompMethodDefInfo
                {
                    MethodID = RailWorkStationSubMachineModulesConst.ControlLeftCylinderMethodID,
                    MethodName=RailWorkStationSubMachineModulesConst.UICommandMemoControlLeftCylinder,
                    Memo = RailWorkStationSubMachineModulesConst.UICommandMemoControlLeftCylinder,
                    ParamDefInfoes = new GFParamDefInfoList
                    {
                        new GFParamDefInfo(
                            RailWorkStationSubMachineModulesConst.LeftCylinderStretch,
                            RailWorkStationSubMachineModulesConst.ParamNameLeftCylinderStretch,
                            GriffinsBaseDataType.Bool),
                        new GFParamDefInfo(
                            RailWorkStationSubMachineModulesConst.LeftCylinderRetract,
                            RailWorkStationSubMachineModulesConst.ParamNameLeftCylinderRetract,
                            GriffinsBaseDataType.Bool),
                    },
                    RetValDefInfoes = new GFParamDefInfoList(),
                    IsAsyn = false
                },
                new ImeCompMethodDefInfo
                {
                    MethodID = RailWorkStationSubMachineModulesConst.ControlRightCylinderMethodID,
                    MethodName=RailWorkStationSubMachineModulesConst.UICommandMemoControlRightCylinder,
                    Memo = RailWorkStationSubMachineModulesConst.UICommandMemoControlRightCylinder,
                    ParamDefInfoes = new GFParamDefInfoList
                    {
                        new GFParamDefInfo(
                            RailWorkStationSubMachineModulesConst.RightCylinderStretch,
                            RailWorkStationSubMachineModulesConst.ParamNameRightCylinderStretch,
                            GriffinsBaseDataType.Bool),
                        new GFParamDefInfo(
                            RailWorkStationSubMachineModulesConst.RightCylinderRetract,
                            RailWorkStationSubMachineModulesConst.ParamNameRightCylinderRetract,
                            GriffinsBaseDataType.Bool),
                    },
                    RetValDefInfoes = new GFParamDefInfoList(),
                    IsAsyn = false
                },
            };

            DevicePropertyInfoList ISubMachineModulesCabilityDef.DeviceProps => null!;

            ISubMachineModulesDefSvr ISubMachineModulesCabilityDef.CreateISubMachineModulesDefSvr(SubMMAlias alias, byte[] factoryCfgInfo, GFBaseTypePropValueList devicePropValues)
            {
                return new SubMachineModulesDefSvr(factoryCfgInfo);
            }
        }

        /// <summary>
        /// 子机械模组定义信息服务对象
        /// </summary>
        class SubMachineModulesDefSvr : ISubMachineModulesDefSvr
        {
            private ISubMachineModulesDefSvrCallBack callBack;
            private readonly RailWorkStationSubMachineModulesFactoryCfg factoryCfg;

            public SubMachineModulesDefSvr(byte[] factoryCfgInfo)
            {
                factoryCfg = new RailWorkStationSubMachineModulesFactoryCfg();
                if (factoryCfgInfo != null && factoryCfgInfo.Length > 0)
                    factoryCfg.FromBytes(factoryCfgInfo);
            }

            /// <summary>
            /// 初始化
            /// </summary>
            void ISubMachineModulesDefSvr.Init(ISubMachineModulesDefSvrCallBack callBack)
            {
                this.callBack = callBack;
            }

            /// <summary>
            /// 获取子界面数据对象项名称字典
            /// </summary>
            Dictionary<string, string> ISubMachineModulesDefSvr.GetSubUIProObjItemNames(ObjInstPropPath objInstPropPath)
            {
                return getSubUIProObjItemNames(objInstPropPath);
            }

            /// <summary>
            /// 当前工位状态对象仅包含标量属性，无字典/列表子项。
            /// </summary>
            private Dictionary<string, string> getSubUIProObjItemNames(ObjInstPropPath objInstPropPath)
            {
                _ = factoryCfg;
                _ = callBack;
                _ = objInstPropPath;
                return null;
            }
        }
    }
}
