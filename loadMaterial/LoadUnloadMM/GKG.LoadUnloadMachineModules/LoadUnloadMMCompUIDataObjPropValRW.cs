using GF_Gereric;
using GKG.LoadUnloadMachineModules.Properties;
using GKG.SubMM;
using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;

namespace GKG.MM
{
    /// <summary>
    /// 上下料总模组的 UIDataObj 属性读写实现。
    /// 当前只对外暴露一个属性：MaterialContainerStatus。
    /// 该属性值不是对象本身，而是 MaterialContainerStatus 的序列化 JSON 字符串，
    /// 与 LoadUnloadMMCmdExecutor.TryGetMaterialContainerStatusJson() 的返回值保持一致。
    /// </summary>
    public class LoadUnloadMMCompUIDataObjPropValRW : ICompUIDataObjPropValRW
    {
        /// <summary>
        /// 统一使用总模组常量中的 PropertyID，避免前后端各自手写字符串。
        /// </summary>
        private const string MaterialContainerStatusPropertyName = LoadUnloadMachineModulesConst.MaterialContainerStatusPropertyID;

        /// <summary>
        /// 由 LoadUnloadMMCmdExecutor 注入的运行时命令执行入口。
        /// UIDataObj 命令需要与 ExecRuntimeCtlCmd 复用同一套逻辑时，通过该委托回调总控。
        /// </summary>
        private readonly Func<string, GFBaseTypeParamValueList, GFBaseTypeParamValueList> runtimeCmdExecutor;

        /// <summary>
        /// 保护最新状态字符串的并发读写；
        /// Inform 回调、UIDataObj 命令和前端取值可能来自不同线程。
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// 当前缓存的 MaterialContainerStatus JSON。
        /// 这就是 UIDataObj 对外返回的实际属性值。
        /// </summary>
        private MaterialContainerStatusList latestMaterialContainerStatus;
        //private GFBaseTypePropValueList gfBaseTypePropValues = new GFBaseTypePropValueList();
        public LoadUnloadMMCompUIDataObjPropValRW(Func<string, GFBaseTypeParamValueList, GFBaseTypeParamValueList> runtimeCmdExecutor = null)
        {
            this.runtimeCmdExecutor = runtimeCmdExecutor;
            //GFUIPropDefInfoList gFUIPropDefInfos = GFPropObjBase.GetGFPropDefInfoes<MaterialContainerStatusList>(Resources.ResourceManager.GetString, MachineModulesCabilityDef.getValueRangeEnums, MachineModulesCabilityDef.getValueNamePairs);
            //GFBaseTypePropValueList gFBaseTypePropValueList = new GFBaseTypePropValueList();
            //if (gFUIPropDefInfos == null)
            //{
            //    gfBaseTypePropValues = gFBaseTypePropValueList;
            //}
            //IMachineModulesDefSvr machineModulesDefSvr = new MachineModulesDefSvr(null);
            //GFBaseTypeObjPropPathValueList gFBaseTypeObjPropPathValueList = new GFBaseTypeObjPropPathValueList();
            //fillGFBaseTypeObjPropPathValues(null, gFBaseTypeObjPropPathValueList, gFUIPropDefInfos, machineModulesDefSvr.GetSubUIProObjItemNames);
            //gFBaseTypePropValueList.Merge(gFBaseTypeObjPropPathValueList);
            //gfBaseTypePropValues = gFBaseTypePropValueList;
            //gfBaseTypePropValues.GetLeafGFBaseTypeObjPropPathValues();
        }
        private static void fillGFBaseTypeObjPropPathValues(string[] parentPropIDPath, GFBaseTypeObjPropPathValueList gFBaseTypeObjPropPathValues, GFUIPropDefInfoList gFUIPropDefInfos, Func<ObjInstPropPath, Dictionary<string, string>> onGetSubUIProObjItemNames)
        {
            foreach (GFUIPropDefInfo gFUIPropDefInfo in gFUIPropDefInfos)
            {
                List<string> list = new List<string>();
                if (parentPropIDPath != null)
                {
                    list.AddRange(parentPropIDPath);
                }

                if (gFUIPropDefInfo.IsSubGFPropObj(out var isList))
                {
                    if (isList)
                    {
                        List<string> list2 = new List<string>(list);
                        list2.Add(gFUIPropDefInfo.PropertyID.ToString());
                        ObjInstPropPath arg = new ObjInstPropPath(list2.ToArray());
                        Dictionary<string, string> dictionary = onGetSubUIProObjItemNames(arg);
                        if (dictionary == null)
                        {
                            continue;
                        }

                        foreach (string key in dictionary.Keys)
                        {
                            List<string> list3 = new List<string>(list);
                            list3.Add($"{gFUIPropDefInfo.PropertyID}[{key}]");
                            fillGFBaseTypeObjPropPathValues(list3.ToArray(), gFBaseTypeObjPropPathValues, gFUIPropDefInfo.SubPropDefInfoes, onGetSubUIProObjItemNames);
                        }
                    }
                    else
                    {
                        list.Add(gFUIPropDefInfo.PropertyID.ToString());
                        fillGFBaseTypeObjPropPathValues(list.ToArray(), gFBaseTypeObjPropPathValues, gFUIPropDefInfo.SubPropDefInfoes, onGetSubUIProObjItemNames);
                    }
                }
                else
                {
                    object baseTypeDefaultValue = getBaseTypeDefaultValue(gFUIPropDefInfo.DataType);
                    list.Add(gFUIPropDefInfo.PropertyID.ToString());
                    gFBaseTypeObjPropPathValues.Add(new GFBaseTypeObjPropPathValue
                    {
                        ObjInstPropPath = new ObjInstPropPath(list.ToArray()),
                        Value = new GriffinsBaseValue(baseTypeDefaultValue)
                    });
                }
            }
        }
        //
        // 摘要:
        //     获取默认的基础类型值
        //
        // 参数:
        //   dataType:
        //     类型
        //
        //   baseTypDefaultValue:
        //
        // 返回结果:
        //     默认的基础类型值
        private static object getBaseTypeDefaultValue(GriffinsBaseDataType dataType)
        {
            object result = null;
            switch (dataType)
            {
                case GriffinsBaseDataType.Bool:
                    result = false;
                    break;
                case GriffinsBaseDataType.Decimal:
                    result = 0.00m;
                    break;
                case GriffinsBaseDataType.Integer:
                    result = 0;
                    break;
                case GriffinsBaseDataType.String:
                    result = string.Empty;
                    break;
                case GriffinsBaseDataType.DateTime:
                    result = DateTime.Now;
                    break;
                case GriffinsBaseDataType.Guid:
                    result = Guid.NewGuid();
                    break;
            }

            return result;
        }
        /// <summary>
        /// 当 MaterialContainerStatus 发生变化时，向界面层推送属性变更事件。
        /// </summary>
        private event ImePropValChangedEventHandler uIDataObjPropValChangedEvent;
        event ImePropValChangedEventHandler ICompUIDataObjPropValRW.UIDataObjPropValChangedEvent
        {
            add
            {
                uIDataObjPropValChangedEvent += value;
            }

            remove
            {
                uIDataObjPropValChangedEvent -= value;
            }
        }
        /// <summary>
        /// 执行界面数据对象命令。
        /// 这里不单独实现业务逻辑，而是直接复用总控的运行时命令入口；
        /// 如果命令返回里附带了最新的 MaterialContainerStatus，则顺手刷新本地缓存。
        /// </summary>
        GFBaseTypeParamValueList ICompUIDataObjPropValRW.ExecUIDataObjCommand(string cmdID, GFBaseTypeParamValueList cmdParam)
        {
            if (runtimeCmdExecutor == null)
                return CreateErrorResult(Resources.UIDataObjRuntimeCommandExecutorNotInjected);

            GFBaseTypeParamValueList result = runtimeCmdExecutor(cmdID, cmdParam);

            return result;
        }

        /// <summary>
        /// 用最新的 MaterialContainerStatus JSON 刷新缓存，
        /// 并在值变化时触发 UIDataObjPropValChangedEvent。
        /// </summary>
        public void UpdateMaterialContainerStatus(GriffinsBaseValue materialContainerStatusJson)
        {
            GriffinsBaseValue newValue = materialContainerStatusJson ?? new GriffinsBaseValue();
            MaterialContainerStatusList materialContainerStatusList = JsonObjConvert.FromJSon<MaterialContainerStatusList>(materialContainerStatusJson.Val.ToString());
            foreach (var materialContainer in materialContainerStatusList.MaterialContainers)
            {
                if(latestMaterialContainerStatus == null || materialContainer.Value.IsFeeding != latestMaterialContainerStatus.MaterialContainers[materialContainer.Key].IsFeeding)
                {
                    InvokeUIDataObjPropValChangedEvent(new ObjInstPropPath(
                        new string[] {
                        combinePath(nameof(MaterialContainerStatusList.MaterialContainers),
                        materialContainer.Value.Name),
                        nameof(MaterialContainerStatus.IsFeeding)}
                        ),
                        new GriffinsBaseValue(materialContainer.Value.IsFeeding));
                }
                if (latestMaterialContainerStatus == null || materialContainer.Value.Name != latestMaterialContainerStatus.MaterialContainers[materialContainer.Key].Name)
                {
                    InvokeUIDataObjPropValChangedEvent(new ObjInstPropPath(
                        new string[] {
                        combinePath(nameof(MaterialContainerStatusList.MaterialContainers),
                        materialContainer.Value.Name),
                        nameof(MaterialContainerStatus.Name) }
                        ),
                        new GriffinsBaseValue(materialContainer.Value.Name));
                }
                foreach (var MaterialBox in materialContainer.Value.MaterialBoxes)
                {
                    if(latestMaterialContainerStatus == null || MaterialBox.Value.IsFeeding != latestMaterialContainerStatus.MaterialContainers[materialContainer.Key].MaterialBoxes[MaterialBox.Key].IsFeeding)
                    {
                        InvokeUIDataObjPropValChangedEvent(new ObjInstPropPath(
                            new string[] {
                            combinePath(nameof(MaterialContainerStatusList.MaterialContainers),materialContainer.Value.Name),
                            combinePath(nameof(MaterialContainerStatus.MaterialBoxes),MaterialBox.Value.Name),
                            nameof(MaterialBoxStatus.IsFeeding) }
                            ),
                            new GriffinsBaseValue(MaterialBox.Value.IsFeeding));
                    }
                    if (latestMaterialContainerStatus == null || MaterialBox.Value.Name != latestMaterialContainerStatus.MaterialContainers[materialContainer.Key].MaterialBoxes[MaterialBox.Key].Name)
                    {
                        InvokeUIDataObjPropValChangedEvent(new ObjInstPropPath(
                            new string[] {
                            combinePath(nameof(MaterialContainerStatusList.MaterialContainers),
                            materialContainer.Value.Name),
                            combinePath(nameof(MaterialContainerStatus.MaterialBoxes),
                            MaterialBox.Value.Name),
                            nameof(MaterialBoxStatus.Name) }
                            ),
                            new GriffinsBaseValue(MaterialBox.Value.Name));
                    }
                    if (latestMaterialContainerStatus == null || MaterialBox.Value.IsEmpty != latestMaterialContainerStatus.MaterialContainers[materialContainer.Key].MaterialBoxes[MaterialBox.Key].IsEmpty)
                    {
                        InvokeUIDataObjPropValChangedEvent(new ObjInstPropPath(
                            new string[] {
                            combinePath(nameof(MaterialContainerStatusList.MaterialContainers),
                            materialContainer.Value.Name),
                            combinePath(nameof(MaterialContainerStatus.MaterialBoxes),
                            MaterialBox.Value.Name),
                            nameof(MaterialBoxStatus.IsEmpty) }
                            ),
                            new GriffinsBaseValue(MaterialBox.Value.IsEmpty));
                    }
                    if (latestMaterialContainerStatus == null || MaterialBox.Value.MaterialBoxCylinderStatus != latestMaterialContainerStatus.MaterialContainers[materialContainer.Key].MaterialBoxes[MaterialBox.Key].MaterialBoxCylinderStatus)
                    {
                        InvokeUIDataObjPropValChangedEvent(new ObjInstPropPath(
                            new string[] {
                            combinePath(nameof(MaterialContainerStatusList.MaterialContainers),
                            materialContainer.Value.Name),
                            combinePath(nameof(MaterialContainerStatus.MaterialBoxes),
                            MaterialBox.Value.Name),
                            nameof(MaterialBoxStatus.MaterialBoxCylinderStatus) }
                            ),
                            new GriffinsBaseValue(MaterialBox.Value.MaterialBoxCylinderStatus));
                    }
                    if (latestMaterialContainerStatus == null || MaterialBox.Value.SlotStatusList["SlotStatusList"] != latestMaterialContainerStatus.MaterialContainers[materialContainer.Key].MaterialBoxes[MaterialBox.Key].SlotStatusList["SlotStatusList"])
                    {
                        InvokeUIDataObjPropValChangedEvent(new ObjInstPropPath(
                            new string[] {
                            combinePath(nameof(MaterialContainerStatusList.MaterialContainers),
                            materialContainer.Value.Name),
                            combinePath(nameof(MaterialContainerStatus.MaterialBoxes),
                            MaterialBox.Value.Name),
                            nameof(MaterialBoxStatus.SlotStatusList) }
                            ),
                            ((IGriffinsBaseValue)MaterialBox.Value.SlotStatusList["SlotStatusList"]).ToBaseValue());
                    }
                }

            }
            //GFBaseTypeObjPropPathValueList gFBaseTypeObjPropPathValues = gfBaseTypePropValues.GetLeafGFBaseTypeObjPropPathValues();
            //UpdateUIDataObjProp(gFBaseTypeObjPropPathValues);
            latestMaterialContainerStatus = materialContainerStatusList;
        }

        private string combinePath(string parent, string child)
        {
            return $"{parent}[{child}]";
        }
        public void UpdateUIDataObjProp(GFBaseTypeObjPropPathValueList propVals)
        {
            foreach (var propVal in propVals)
            {
                uIDataObjPropValChangedEvent?.Invoke(this, new ImePropValChangedEventArgs(propVal.ObjInstPropPath, propVal.Value, DateTime.Now));
            }
        }

        /// <summary>
        /// 返回当前 UIDataObj 的全部属性。
        /// 当前仅维护一个属性：MaterialContainerStatus。
        /// </summary>
        public GFBaseTypePropValueList GetAllUIDataObjPropValues()
        {
            //GFBaseTypePropValueList result = new GFBaseTypePropValueList();
            //lock (syncRoot)
            //{
            //    result.Add(new GFBaseTypePropValue(
            //        new MPPropertyID(MaterialContainerStatusPropertyName),
            //        new GriffinsBaseValue(JsonObjConvert.ToJSon(latestMaterialContainerStatus))));
            //}
            //return result;
            return latestMaterialContainerStatus.ToGFBaseTypePropValues();
        }

        /// <summary>
        /// 允许外部直接写回 MaterialContainerStatus。
        /// 写入后仍会走统一的缓存更新与事件通知逻辑。
        /// </summary>
        public void SetUIDataObjPropValue(MPPropertyID propertyID, GriffinsBaseValue value)
        {
            if (!string.Equals(propertyID.ToString(), MaterialContainerStatusPropertyName, StringComparison.Ordinal))
                return;

            UpdateMaterialContainerStatus(value);
        }

        /// <summary>
        /// 构造 UIDataObj 命令执行失败时的标准返回结构。
        /// </summary>
        private static GFBaseTypeParamValueList CreateErrorResult(string errorMsg)
        {
            GFBaseTypeParamValueList result = new GFBaseTypeParamValueList();
            result.Add(new GFBaseTypeParamValue("Result", new GriffinsBaseValue("-1")));
            result.Add(new GFBaseTypeParamValue("errorMsg", new GriffinsBaseValue(errorMsg ?? string.Empty)));
            result.Add(new GFBaseTypeParamValue("data", new GriffinsBaseValue(string.Empty)));
            return result;
        }

        void ICompUIDataObjPropValRW.SetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath, GriffinsBaseValue value)
        {
            GFBaseTypePropValueList allValues = GetAllUIDataObjPropValues();
            GFBaseTypePropValue target = getUIDataObjPropPathValue(allValues, objInstPropPath, value);
            if (target != null)
            {
                SetUIDataObjPropValue(target.PropertyID, target.Value);
            }
        }

        void ICompUIDataObjPropValRW.SetUIDataObjPropPathValues(GFBaseTypeObjPropPathValueList values)
        {
            if (values == null || values.Count == 0)
                return;

            GFBaseTypePropValueList allValues = GetAllUIDataObjPropValues();
            allValues.Merge(values);
            foreach (GFBaseTypePropValue item in allValues)
            {
                SetUIDataObjPropValue(item.PropertyID, item.Value);
            }
        }

        GriffinsBaseValue ICompUIDataObjPropValRW.GetUIDataObjPropPathValue(ObjInstPropPath objInstPropPath)
        {
            GFBaseTypePropValueList allValues = GetAllUIDataObjPropValues();
            foreach (GFBaseTypePropValue item in allValues)
            {
                GFBaseTypeObjPropPathValue target = item.GetLeafGFBaseTypeObjPropPathValues().Find(objInstPropPath);
                if (target != null)
                    return target.Value;
            }

            return null;
        }

        GFBaseTypeObjPropPathValueList ICompUIDataObjPropValRW.GetUIDataObjPropPathValues(ObjInstPropPath[] objInstPropPaths)
        {
            GFBaseTypeObjPropPathValueList result = new GFBaseTypeObjPropPathValueList();
            if (objInstPropPaths == null || objInstPropPaths.Length == 0)
                return result;

            GFBaseTypePropValueList allValues = GetAllUIDataObjPropValues();
            foreach (GFBaseTypePropValue item in allValues)
            {
                GFBaseTypeObjPropPathValueList leaves = item.GetLeafGFBaseTypeObjPropPathValues();
                foreach (ObjInstPropPath path in objInstPropPaths)
                {
                    GFBaseTypeObjPropPathValue found = leaves.Find(path);
                    if (found != null)
                        result.Add(found);
                }
            }

            return result;
        }

        GFBaseTypeObjPropPathValueList ICompUIDataObjPropValRW.GetAllUIDataObjPropPathValues()
        {
            var value = GetAllUIDataObjPropValues().GetLeafGFBaseTypeObjPropPathValues();
            return value;
        }

        /// <summary>
        /// 把路径值合并回属性值列表，并返回其所属的顶层属性。
        /// </summary>
        private static GFBaseTypePropValue getUIDataObjPropPathValue(
            GFBaseTypePropValueList gfBaseTypePropValues,
            ObjInstPropPath objInstPropPath,
            GriffinsBaseValue value)
        {
            GFBaseTypeObjPropPathValueList pathValues = new GFBaseTypeObjPropPathValueList
            {
                new GFBaseTypeObjPropPathValue
                {
                    ObjInstPropPath = objInstPropPath,
                    Value = value
                }
            };

            gfBaseTypePropValues.Merge(pathValues);
            foreach (GFBaseTypePropValue item in gfBaseTypePropValues)
            {
                GFBaseTypeObjPropPathValue target = item.GetLeafGFBaseTypeObjPropPathValues().Find(objInstPropPath);
                if (target != null)
                    return item;
            }

            return null;
        }

        private void InvokeUIDataObjPropValChangedEvent(ObjInstPropPath objInstPropPath, GriffinsBaseValue value)
        {
            uIDataObjPropValChangedEvent?.Invoke(this, new ImePropValChangedEventArgs(objInstPropPath, value, DateTime.Now));
        }
    }
}
