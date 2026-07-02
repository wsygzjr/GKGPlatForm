using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using GKG.Map.StatusInfoFuncCtlMapCell.MapCell_StatusInfo.MapOprtCellParamCfgView;
using GKG.Map.StatusInfoFuncCtlMapCell.View;
using GKG.Map.StatusInfoFuncCtlMapCell.ViewModel;
using Griffins.Map.UI;
using Griffins.UI2;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reactive.Linq;
using System.Text.Json;
using Griffins;
using Griffins.Map;

namespace GKG.Map.StatusInfoFuncCtlMapCell
{
    /// <summary>
    /// 状态信息图元对象
    /// </summary>
    class MapCellStatusInfoCtlObj : FunctionalCellBase
    {
        #region 私有字段
        private StatusInfoView view;
        private StatusInfoViewModel viewModel;
        private readonly ConcurrentDictionary<Guid, MapOprtCellID> _oprtCellIdByInstanceId = new();
        private bool _loadedPropertyEditFromBytes;
        private bool _pendingBackColorOprtExec;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化状态信息图元对象（运行时）
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        public MapCellStatusInfoCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        /// <summary>
        /// 初始化状态信息图元对象
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <param name="designTime">是否为设计时模式</param>
        public MapCellStatusInfoCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();

            StatusInfoPropertyModelEdit.PropertyChanged += StatusInfoPropertyModelEdit_PropertyChanged;

            // 订阅子对象的PropertyChanged事件，实现跨操作的操作原子联动
            StatusInfoPropertyModelEdit.StateInfo.PropertyChanged += (_, e) => ExecuteOprtByPropertyId(nameof(StatusInfoPropertyModelEdit.StateInfo), "PropertyChanged", e?.PropertyName);
            StatusInfoPropertyModelEdit.TimeInfo.PropertyChanged += (_, e) => ExecuteOprtByPropertyId(nameof(StatusInfoPropertyModelEdit.TimeInfo), "PropertyChanged", e?.PropertyName);

            base.SetID(mapCellID);
            base.SetName(mapCellName);

            view = new StatusInfoView();

            RegisterProperty(new MapObjPropertyInfo(nameof(StatusInfoPropertyModelEdit.BackColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Color.FromArgb(33, 0, 0, 0).ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(StatusInfoPropertyModelEdit.TextColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_TextColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Colors.Black.ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(StatusInfoPropertyModelEdit.TextFont), ResourceA.TextFont, GriffinsBaseDataType.Object_Json, FontInfo.Object_ID, typeof(FontInfo), true, true, new GriffinsBaseValue(FontInfo.DefaultFont)));
            RegisterProperty(new MapObjPropertyInfo(nameof(StatusInfoPropertyModelEdit.IsDualValve), "双阀模式", GriffinsBaseDataType.Integer, Guid.Empty, typeof(bool), true, true, new GriffinsBaseValue(0)));

            RegisterProperty(new MapObjPropertyInfo(nameof(StatusInfoPropertyModelEdit.StateInfo), "状态量", GriffinsBaseDataType.Object_Json, StatusInfoStateInfo.Object_ID, typeof(StatusInfoStateInfo), true, true, new GriffinsBaseValue(StatusInfoStateInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(StatusInfoPropertyModelEdit.TimeInfo), "时间", GriffinsBaseDataType.Object_Json, StatusInfoTimeInfo.Object_ID, typeof(StatusInfoTimeInfo), true, true, new GriffinsBaseValue(StatusInfoTimeInfo.Default)));

            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), GriffinsBaseDataType.Object_Bytes, GraphMouseEventParam.Object_ID));

            RegisterOprtCellInfo(new MapOprtCellInfo(StatusInfoMapOprtCellConst.TextColor_MapOprtCellID, ResourceA.TextColor_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(StatusInfoMapOprtCellConst.BackColor_MapOprtCellID, ResourceA.BackColor_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(StatusInfoMapOprtCellConst.TextFont_MapOprtCellID, ResourceA.TextFont_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(StatusInfoMapOprtCellConst.IsDualValve_MapOprtCellID, "双阀模式"));

            RegisterOprtCellInfo(new MapOprtCellInfo(StatusInfoMapOprtCellConst.StateInfo_MapOprtCellID, "状态量", typeof(StatusInfoStateMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(StatusInfoMapOprtCellConst.TimeInfo_MapOprtCellID, "时间", typeof(StatusInfoTimeMapOprtCellParamCfgView)));

            RegisterOprtInfo(new MapOprtInfo(nameof(StatusInfoPropertyModelEdit.TextColor), ResourceA.TextColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = StatusInfoMapOprtCellConst.TextColor_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(StatusInfoPropertyModelEdit.BackColor), ResourceA.BackColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = StatusInfoMapOprtCellConst.BackColor_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(StatusInfoPropertyModelEdit.TextFont), ResourceA.TextFont_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = StatusInfoMapOprtCellConst.TextFont_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(StatusInfoPropertyModelEdit.IsDualValve), "双阀模式", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = StatusInfoMapOprtCellConst.IsDualValve_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(StatusInfoPropertyModelEdit.StateInfo), "状态量", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = StatusInfoMapOprtCellConst.StateInfo_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(StatusInfoPropertyModelEdit.TimeInfo), "时间", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = StatusInfoMapOprtCellConst.TimeInfo_MapOprtCellID, CfgInfo = null }
            }));

            (this as IMapCellTypeBase).Name = ResourceA.StatusInfo;

            viewModel = new StatusInfoViewModel(StatusInfoPropertyModelEdit, clickExec);
            StatusInfoPropertyModelEdit.PropertyChanged += StatusInfoPropertyModelEdit_PropertyChanged;
            SyncModelToViewModel();
            view.DataContext = viewModel;
        }

        #endregion

        /// <summary>
        /// 点击事件执行处理
        /// </summary>
        private void clickExec()
        {
            EventCmdInfo? eventCmdInfo = EventBindEditModel.EventCmdInfos.FirstOrDefault
                (info => info.EventID == MapObjPropEventConst.Event_MouseClick);
            if (eventCmdInfo != null)
            {
                GFBaseTypeParamValueList cmdParam = null;
                if (!string.IsNullOrWhiteSpace(eventCmdInfo.CmdParam))
                {
                    cmdParam = new GFBaseTypeParamValueList();
                    cmdParam.FromJson(eventCmdInfo.CmdParam);
                }
                CallBack?.ExecMapCellEvent(eventCmdInfo.EventCmdKind, eventCmdInfo.CmdID, cmdParam, out _);
            }
        }

        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null!;
        }

        /// <summary>
        /// 获取状态信息属性模型
        /// </summary>
        [Browsable(false)]
        public StatusInfoPropertyModelEdit StatusInfoPropertyModelEdit
        {
            get { return PropertyEditModelBase as StatusInfoPropertyModelEdit; }
        }

        /// <summary>
        /// 属性模型变更事件处理
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">属性变更事件参数</param>
        private void StatusInfoPropertyModelEdit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e == null)
                return;

            if (string.Equals(e.PropertyName, nameof(StatusInfoPropertyModelEdit.TextFont), StringComparison.Ordinal))
                SyncTextFont();
            else
                SyncModelToViewModel();
        }

        #region 公共方法

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="propertyID">属性ID</param>
        /// <param name="propertyVal">属性值</param>
        /// <param name="isRuning">是否为运行时</param>
        /// <returns>设置是否成功</returns>
        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            // 如果已经从字节加载了属性，且不是运行时，且是默认值覆盖，则跳过
            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoadedStatusInfo(propertyID, propertyVal))
            {
                return true;
            }

            StatusInfoPropertyModelEdit.IsRuning = isRuning;
            var result = StatusInfoPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
            if (result && !isRuning && string.Equals(propertyID, nameof(StatusInfoPropertyModelEdit.BackColor), StringComparison.Ordinal))
                _pendingBackColorOprtExec = true;
            if (result)
                SyncModelToViewModel();
            return result;
        }

        /// <summary>
        /// 检查是否是默认值覆盖已加载的属性
        /// </summary>
        private bool IsDefaultOverwriteForLoadedStatusInfo(string propertyID, GriffinsBaseValue propertyVal)
        {
            try
            {
                // 检查StateInfo属性
                if (string.Equals(propertyID, nameof(StatusInfoPropertyModelEdit.StateInfo), StringComparison.Ordinal))
                {
                    if (propertyVal?.ToObjectValue_Json() != null)
                    {
                        var defaultStateInfo = StatusInfoStateInfo.Default;
                        var loadedStateInfo = StatusInfoPropertyModelEdit.StateInfo;

                        // 如果当前值不是默认值，而要设置的值是默认值，则跳过
                        if (!IsStateInfoEqual(loadedStateInfo, defaultStateInfo) && IsStateInfoEqual(propertyVal.ToObjectValue_Json(), defaultStateInfo))
                        {
                            return true;
                        }
                    }
                }

                // 检查TimeInfo属性
                if (string.Equals(propertyID, nameof(StatusInfoPropertyModelEdit.TimeInfo), StringComparison.Ordinal))
                {
                    if (propertyVal?.ToObjectValue_Json() != null)
                    {
                        var defaultTimeInfo = StatusInfoTimeInfo.Default;
                        var loadedTimeInfo = StatusInfoPropertyModelEdit.TimeInfo;

                        // 如果当前值不是默认值，而要设置的值是默认值，则跳过
                        if (!IsTimeInfoEqual(loadedTimeInfo, defaultTimeInfo) && IsTimeInfoEqual(propertyVal.ToObjectValue_Json(), defaultTimeInfo))
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
                // 出现异常时，不跳过，允许设置
            }

            return false;
        }

        /// <summary>
        /// 比较两个StateInfo是否相等
        /// </summary>
        private bool IsStateInfoEqual(object obj1, object obj2)
        {
            if (obj1 is StatusInfoStateInfo state1 && obj2 is StatusInfoStateInfo state2)
            {
                return state1.LeftValveGlueMonitorState == state2.LeftValveGlueMonitorState &&
                       state1.LeftValveQuantitativeGlueMonitorState == state2.LeftValveQuantitativeGlueMonitorState &&
                       state1.LeftValveRemainingMonitorState == state2.LeftValveRemainingMonitorState &&
                       state1.LeftPressureCyclesAlarmState == state2.LeftPressureCyclesAlarmState &&
                       state1.RightValveGlueMonitorState == state2.RightValveGlueMonitorState &&
                       state1.RightValveQuantitativeGlueMonitorState == state2.RightValveQuantitativeGlueMonitorState &&
                       state1.RightValveRemainingMonitorState == state2.RightValveRemainingMonitorState &&
                       state1.RightPressureCyclesAlarmState == state2.RightPressureCyclesAlarmState;
            }
            return false;
        }

        /// <summary>
        /// 比较两个TimeInfo是否相等
        /// </summary>
        private bool IsTimeInfoEqual(object obj1, object obj2)
        {
            if (obj1 is StatusInfoTimeInfo time1 && obj2 is StatusInfoTimeInfo time2)
            {
                return string.Equals(time1.AWaitingAddGlueTime, time2.AWaitingAddGlueTime, StringComparison.Ordinal) &&
                       string.Equals(time1.BWaitingAddGlueTime, time2.BWaitingAddGlueTime, StringComparison.Ordinal);
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 从字节流读取绘制信息
        /// </summary>
        /// <param name="br">XML读取器</param>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            var propertyEditModelBase = JsonObjConvert.FromJSon<StatusInfoPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            (PropertyEditModelBase as StatusInfoPropertyModelEdit).CopyFrom(propertyEditModelBase);
            var propertyBindEditModelBase = JsonObjConvert.FromJSon<StatusInfoPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            (PropertyBindEditModelBase as StatusInfoPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
            var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(br.ReadString("EventBindEditModel"));
            EventBindEditModel.CopyFrom(eventBindEditModel);
        }

        /// <summary>
        /// 将绘制信息写入字节流
        /// </summary>
        /// <param name="bw">XML写入器</param>
        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
        }

        /// <summary>
        /// 从其他图元对象复制属性
        /// </summary>
        /// <param name="source">源图元对象</param>
        protected override void OnCopyFrom(FunctionalCellBase source)
        {
            MapCellStatusInfoCtlObj mapCellStatusInfoCtlObj = (source as MapCellStatusInfoCtlObj);
            base._CopyFrom(mapCellStatusInfoCtlObj);
            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
            (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel.CopyFrom(source.EventBindEditModel);
        }

        #region 动态操作原子管理

        /// <summary>
        /// 动态添加操作原子到指定操作下
        /// </summary>
        /// <param name="propertyId">属性ID（操作ID）</param>
        /// <param name="oprtCellId">操作原子ID</param>
        /// <param name="cfgInfo">配置信息</param>
        /// <returns>是否添加成功</returns>
        public bool AddOprtCellToProperty(string propertyId, MapOprtCellID oprtCellId, byte[] cfgInfo = null)
        {
            try
            {
                // 查找对应的操作信息
                var oprtInfo = FindOprtInfoByPropertyId(propertyId);
                if (oprtInfo == null)
                {
                    // 如果操作不存在，创建新的操作
                    var newOprtInfo = new MapOprtInfo(propertyId, $"动态操作-{propertyId}", OprtExecKind.Normal, "", new MapOprtCellInstInfoList());
                    RegisterOprtInfo(newOprtInfo);
                    oprtInfo = newOprtInfo;
                }

                // 创建新的操作原子实例
                var newInst = new MapOprtCellInstInfo()
                {
                    InstanceID = Guid.NewGuid(),
                    OprtCellID = oprtCellId,
                    CfgInfo = cfgInfo
                };

                // 添加到操作的实例列表中
                var instList = GetOprtInfoInstList(oprtInfo);
                if (instList is MapOprtCellInstInfoList list)
                {
                    list.Add(newInst);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"添加操作原子失败: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// 从指定操作下移除操作原子
        /// </summary>
        /// <param name="propertyId">属性ID（操作ID）</param>
        /// <param name="instanceId">操作原子实例ID</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveOprtCellFromProperty(string propertyId, Guid instanceId)
        {
            try
            {
                var oprtInfo = FindOprtInfoByPropertyId(propertyId);
                if (oprtInfo == null) return false;

                var instList = GetOprtInfoInstList(oprtInfo);
                if (instList is MapOprtCellInstInfoList list)
                {
                    var instToRemove = list.FirstOrDefault(x => x.InstanceID == instanceId);
                    if (instToRemove != null)
                    {
                        list.Remove(instToRemove);
                        // 清理执行器字典中的对应项
                        MapOprtCellExectorDict.TryRemove(instanceId, out _);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"移除操作原子失败: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// 获取指定操作下的所有操作原子
        /// </summary>
        /// <param name="propertyId">属性ID（操作ID）</param>
        /// <returns>操作原子实例列表</returns>
        public IEnumerable<MapOprtCellInstInfo> GetOprtCellsByProperty(string propertyId)
        {
            var oprtInfo = FindOprtInfoByPropertyId(propertyId);
            if (oprtInfo == null) return Enumerable.Empty<MapOprtCellInstInfo>();

            var instList = GetOprtInfoInstList(oprtInfo);
            if (instList is MapOprtCellInstInfoList list)
                return list.ToList();

            return Enumerable.Empty<MapOprtCellInstInfo>();
        }

        /// <summary>
        /// 根据属性ID查找操作信息
        /// </summary>
        /// <param name="propertyId">属性ID</param>
        /// <returns>操作信息</returns>
        private MapOprtInfo FindOprtInfoByPropertyId(string propertyId)
        {
            foreach (var oprtInfo in EnumerateOprtInfos())
            {
                var id = GetOprtInfoId(oprtInfo);
                if (string.Equals(id, propertyId, StringComparison.Ordinal))
                    return oprtInfo as MapOprtInfo;
            }
            return null;
        }

        #endregion

        #region 动态操作原子序列化

        /// <summary>
        /// 序列化动态操作原子信息
        /// </summary>
        /// <returns>序列化后的JSON字符串</returns>
        private string SerializeDynamicOprtCells()
        {
            try
            {
                var dynamicOprtCells = new List<DynamicOprtCellData>();

                // 收集所有操作的操作原子信息
                foreach (var oprtInfo in EnumerateOprtInfos())
                {
                    var id = GetOprtInfoId(oprtInfo);
                    if (string.IsNullOrEmpty(id)) continue;

                    var instList = GetOprtInfoInstList(oprtInfo);
                    if (instList is MapOprtCellInstInfoList list)
                    {
                        foreach (var inst in list)
                        {
                            // 只序列化跨操作添加的操作原子
                            if (IsCrossOperationOprtCell(inst, id))
                            {
                                dynamicOprtCells.Add(new DynamicOprtCellData
                                {
                                    PropertyId = id,
                                    InstanceID = inst.InstanceID,
                                    OprtCellID = inst.OprtCellID,
                                    CfgInfo = inst.CfgInfo
                                });
                            }
                        }
                    }
                }

                return System.Text.Json.JsonSerializer.Serialize(dynamicOprtCells);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"序列化动态操作原子失败: {ex.Message}");
                return "[]";
            }
        }

        /// <summary>
        /// 反序列化动态操作原子信息
        /// </summary>
        /// <param name="json">JSON字符串</param>
        private void DeserializeDynamicOprtCells(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return;

                var dynamicOprtCells = System.Text.Json.JsonSerializer.Deserialize<List<DynamicOprtCellData>>(json);
                if (dynamicOprtCells == null) return;

                foreach (var data in dynamicOprtCells)
                {
                    // 重新添加动态操作原子
                    AddOprtCellToProperty(data.PropertyId, data.OprtCellID, data.CfgInfo);
                }

                System.Diagnostics.Debug.WriteLine($"成功反序列化 {dynamicOprtCells.Count} 个动态操作原子");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"反序列化动态操作原子失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 判断是否为跨操作添加的操作原子
        /// </summary>
        /// <param name="inst">操作原子实例</param>
        /// <param name="propertyId">属性ID</param>
        /// <returns>是否为跨操作操作原子</returns>
        private bool IsCrossOperationOprtCell(MapOprtCellInstInfo inst, string propertyId)
        {
            // 如果是时间操作原子但在状态量操作中，或者是状态量操作原子在时间操作中
            return (inst.OprtCellID == StatusInfoMapOprtCellConst.TimeInfo_MapOprtCellID &&
                    string.Equals(propertyId, nameof(StatusInfoPropertyModelEdit.StateInfo))) ||
                   (inst.OprtCellID == StatusInfoMapOprtCellConst.StateInfo_MapOprtCellID &&
                    string.Equals(propertyId, nameof(StatusInfoPropertyModelEdit.TimeInfo)));
        }

        #endregion

        private void ExecuteOprtByPropertyId(string oprtId)
        {
            ExecuteOprtByPropertyId(oprtId, null, null);
        }

        /// <summary>
        /// 执行指定操作的操作原子，支持跨操作联动
        /// </summary>
        /// <param name="oprtId">操作ID</param>
        /// <param name="trigger">触发源</param>
        /// <param name="changedProp">变化的属性名</param>
        private void ExecuteOprtByPropertyId(string oprtId, string trigger, string? changedProp)
        {
            if (string.IsNullOrWhiteSpace(oprtId))
                return;

            try
            {
                // 执行当前操作的所有操作原子
                foreach (var oprtInfo in EnumerateOprtInfos())
                {
                    var id = GetOprtInfoId(oprtInfo);
                    if (!string.Equals(id, oprtId, StringComparison.Ordinal))
                        continue;

                    var instList = GetOprtInfoInstList(oprtInfo);
                    if (instList == null)
                        return;

                    foreach (var instObj in instList)
                    {
                        if (instObj is not MapOprtCellInstInfo inst)
                            continue;

                        if (Dispatcher.UIThread.CheckAccess())
                            ExecOprtCell(inst);
                        else
                            Dispatcher.UIThread.Post(() => ExecOprtCell(inst));
                    }

                    // 执行完操作原子后，同步属性模型到ViewModel
                    SyncModelToViewModel();
                    return;
                }

                // 跨操作联动：当状态量变化时，也执行时间操作原子
                if (string.Equals(oprtId, nameof(StatusInfoPropertyModelEdit.StateInfo), StringComparison.Ordinal) &&
                    string.Equals(trigger, "PropertyChanged", StringComparison.Ordinal))
                {
                    ExecuteOprtByPropertyId(nameof(StatusInfoPropertyModelEdit.TimeInfo), "CrossOperation", changedProp);
                }
                // 反向联动：当时间变化时，也可以执行状态量操作原子（如果需要）
                else if (string.Equals(oprtId, nameof(StatusInfoPropertyModelEdit.TimeInfo), StringComparison.Ordinal) &&
                         string.Equals(trigger, "PropertyChanged", StringComparison.Ordinal))
                {
                    // 可选：时间变化时也执行状态量操作原子
                    // ExecuteOprtByPropertyId(nameof(StatusInfoPropertyModelEdit.StateInfo), "CrossOperation", changedProp);
                }
            }
            catch
            {
            }
        }

        private IEnumerable<object> EnumerateOprtInfos()
        {
            foreach (var member in EnumerateInstanceMembers(GetType()))
            {
                var val = GetMemberValue(member, this);
                if (val == null)
                    continue;
                if (val is MapOprtInfo oprtInfo)
                    yield return oprtInfo;
                if (val is IEnumerable enumerable && val is not string)
                {
                    foreach (var item in enumerable)
                        if (item is MapOprtInfo info)
                            yield return info;
                }
            }
        }

        private static string GetOprtInfoId(object oprtInfo)
        {
            try
            {
                var t = oprtInfo.GetType();
                foreach (var name in new[] { "OprtID", "OprtId", "ID", "Id", "PropertyID", "PropertyId" })
                {
                    var p = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (p == null)
                        continue;
                    var v = p.GetValue(oprtInfo);
                    if (v is string s && !string.IsNullOrWhiteSpace(s))
                        return s;
                }
            }
            catch
            {
            }
            return null;
        }

        private static IEnumerable GetOprtInfoInstList(object oprtInfo)
        {
            if (oprtInfo is MapOprtInfo info)
                return info.MapOprtCellInstInfos;
            return null;
        }

        private static IEnumerable<MemberInfo> EnumerateInstanceMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            for (var t = type; t != null; t = t.BaseType)
            {
                foreach (var f in t.GetFields(flags))
                    if (!f.IsStatic)
                        yield return f;
                foreach (var p in t.GetProperties(flags))
                    if (p.GetIndexParameters().Length == 0 && p.CanRead)
                        yield return p;
            }
        }

        private static object GetMemberValue(MemberInfo member, object instance)
        {
            try
            {
                return member switch
                {
                    FieldInfo f => f.GetValue(instance),
                    PropertyInfo p => p.GetValue(instance),
                    _ => null,
                };
            }
            catch
            {
                return null;
            }
        }

        protected override object OnGetView()
        {
            return view;
        }

        protected override object OnGetViewModel()
        {
            return viewModel;
        }

        public override PropertyEditModelBase CreatePropertyModelEditBase()
        {
            return new StatusInfoPropertyModelEdit();
        }

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
        {
            var m = new StatusInfoPropertyBindEditModel();
            return m;
        }

        public override EventBindEditModel CreateEventBindEditModel()
        {
            return new EventBindEditModel()
            {
                EventCmdInfos = new BindingList<EventCmdInfo>()
                {
                    new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = MapObjPropEventConst.Event_MouseClick },
                }
            };
        }

        public override void OnZoomChanged()
        {
            SyncTextFont();
        }

        internal void SetButtonTextFont()
        {
            SyncTextFont();
        }

        private void SyncTextFont()
        {
            if (viewModel == null || StatusInfoPropertyModelEdit == null)
                return;

            double size = base.CallBack?.Calc?.CalcZoomVal((decimal)this.StatusInfoPropertyModelEdit.TextFont.FontSize) ?? this.StatusInfoPropertyModelEdit.TextFont.FontSize;
            if (size < 2)
                size = 2;
            FontInfo font = new FontInfo(this.StatusInfoPropertyModelEdit.TextFont.FontFamily, size, this.StatusInfoPropertyModelEdit.TextFont.FontWeight, this.StatusInfoPropertyModelEdit.TextFont.FontStyle);
            viewModel.TextFont = font;
        }

        private void SyncModelToViewModel()
        {
            if (viewModel == null || StatusInfoPropertyModelEdit == null)
                return;

            SyncTextFont();
            viewModel.TextColor = StatusInfoPropertyModelEdit.TextColor;
            viewModel.BackColor = StatusInfoPropertyModelEdit.BackColor;
            viewModel.IsDualValve = StatusInfoPropertyModelEdit.IsDualValve;

            viewModel.LeftValveGlueMonitorState = StatusInfoPropertyModelEdit.LeftValveGlueMonitorState;
            viewModel.LeftValveQuantitativeGlueMonitorState = StatusInfoPropertyModelEdit.LeftValveQuantitativeGlueMonitorState;
            viewModel.LeftValveRemainingMonitorState = StatusInfoPropertyModelEdit.LeftValveRemainingMonitorState;
            viewModel.LeftPressureCyclesAlarmState = StatusInfoPropertyModelEdit.LeftPressureCyclesAlarmState;
            viewModel.RightValveGlueMonitorState = StatusInfoPropertyModelEdit.RightValveGlueMonitorState;
            viewModel.RightValveQuantitativeGlueMonitorState = StatusInfoPropertyModelEdit.RightValveQuantitativeGlueMonitorState;
            viewModel.RightValveRemainingMonitorState = StatusInfoPropertyModelEdit.RightValveRemainingMonitorState;
            viewModel.RightPressureCyclesAlarmState = StatusInfoPropertyModelEdit.RightPressureCyclesAlarmState;

            viewModel.AWaitingAddGlueTime = string.IsNullOrWhiteSpace(StatusInfoPropertyModelEdit.AWaitingAddGlueTime) ? "00:00" : StatusInfoPropertyModelEdit.AWaitingAddGlueTime;
            viewModel.BWaitingAddGlueTime = string.IsNullOrWhiteSpace(StatusInfoPropertyModelEdit.BWaitingAddGlueTime) ? "00:00" : StatusInfoPropertyModelEdit.BWaitingAddGlueTime;
        }

        #region 操作原子执行对象

        private bool ExecBoolProperty(MapOprtCellInstInfo info, string propName, Action<bool> setAction)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new BoolPropertyMapOprtCellExector(propName, setAction);
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }

        private bool ExecColorProperty(MapOprtCellInstInfo info, string propName, Action<Color> setAction)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new ColorPropertyMapOprtCellExector(propName, setAction);
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }

        private bool ExecTextFontProperty(MapOprtCellInstInfo info)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new TextFontMapOprtCellExector();
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }

        private bool ExecStateInfoProperty(MapOprtCellInstInfo info)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new StateInfoMapOprtCellExector();
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }

        private bool ExecTimeInfoProperty(MapOprtCellInstInfo info)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new TimeInfoMapOprtCellExector();
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }

        private class StateInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is StatusInfoViewModel vm)
                {
                    StatusInfoStateInfo infoObj = null;
                    try
                    {
                        var cfgVm = StatusInfoOprtCellCfgSerializer.FromBytes<StatusInfoStateMapOprtCellParamViewModel>(cfg);
                        if (cfgVm != null)
                        {
                            infoObj = new StatusInfoStateInfo
                            {
                                LeftValveGlueMonitorState = cfgVm.LeftValveGlueMonitorState,
                                RightValveGlueMonitorState = cfgVm.RightValveGlueMonitorState,
                                RightPressureCyclesAlarmState = cfgVm.RightPressureCyclesAlarmState,
                            };
                        }
                    }
                    catch
                    {
                        infoObj = null;
                    }

                    if (infoObj == null)
                    {
                        GriffinsBaseValue val = callBack.GetMapCellPropValue(nameof(StatusInfoPropertyModelEdit.StateInfo));
                        if (val != null)
                        {
                            ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
                            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                            IGriffinsBaseValue iMPPropObjectValue = new StatusInfoStateInfo();
                            iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                            infoObj = (StatusInfoStateInfo)iMPPropObjectValue;
                        }
                    }

                    if (infoObj == null)
                        return;

                    // 同时更新ViewModel和PropertyModelEdit
                    Dispatcher.UIThread.Post(() =>
                    {
                        vm.LeftValveGlueMonitorState = infoObj.LeftValveGlueMonitorState;
                        vm.RightValveGlueMonitorState = infoObj.RightValveGlueMonitorState;
                        vm.RightPressureCyclesAlarmState = infoObj.RightPressureCyclesAlarmState;
                    });
                }
            }
        }

        private class TimeInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is StatusInfoViewModel vm)
                {
                    StatusInfoTimeInfo infoObj = null;
                    try
                    {
                        var cfgVm = StatusInfoOprtCellCfgSerializer.FromBytes<StatusInfoTimeMapOprtCellParamViewModel>(cfg);
                        if (cfgVm != null)
                        {
                            infoObj = new StatusInfoTimeInfo
                            {
                                AWaitingAddGlueTime = cfgVm.AWaitingAddGlueTime,
                                BWaitingAddGlueTime = cfgVm.BWaitingAddGlueTime,
                            };
                        }
                    }
                    catch
                    {
                        infoObj = null;
                    }

                    if (infoObj == null)
                    {
                        GriffinsBaseValue val = callBack.GetMapCellPropValue(nameof(StatusInfoPropertyModelEdit.TimeInfo));
                        if (val != null)
                        {
                            ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
                            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                            IGriffinsBaseValue iMPPropObjectValue = new StatusInfoTimeInfo();
                            iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                            infoObj = (StatusInfoTimeInfo)iMPPropObjectValue;
                        }
                    }

                    if (infoObj == null)
                        return;

                    // 同时更新ViewModel和PropertyModelEdit
                    Dispatcher.UIThread.Post(() =>
                    {
                        vm.AWaitingAddGlueTime = string.IsNullOrWhiteSpace(infoObj.AWaitingAddGlueTime) ? "00:00" : infoObj.AWaitingAddGlueTime;
                        vm.BWaitingAddGlueTime = string.IsNullOrWhiteSpace(infoObj.BWaitingAddGlueTime) ? "00:00" : infoObj.BWaitingAddGlueTime;
                    });
                }
            }
        }

        private class StringPropertyMapOprtCellExector : IMapOprtCellExector
        {
            private readonly string propName;
            private readonly Action<string> setAction;
            private IMapOprtCellExectorCallBack callBack;

            public StringPropertyMapOprtCellExector(string propName, Action<string> setAction)
            {
                this.propName = propName;
                this.setAction = setAction;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                GriffinsBaseValue val = callBack.GetMapCellPropValue(propName);
                setAction?.Invoke(val?.ToPrimitiveValue<string>());
            }
        }

        private class BoolPropertyMapOprtCellExector : IMapOprtCellExector
        {
            private readonly string propName;
            private readonly Action<bool> setAction;
            private IMapOprtCellExectorCallBack callBack;

            public BoolPropertyMapOprtCellExector(string propName, Action<bool> setAction)
            {
                this.propName = propName;
                this.setAction = setAction;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                GriffinsBaseValue val = callBack.GetMapCellPropValue(propName);
                setAction?.Invoke(ParseBool(val));
            }

            private static bool ParseBool(GriffinsBaseValue v)
            {
                if (v == null)
                    return false;

                try
                {
                    return v.ToPrimitiveValue<bool>();
                }
                catch
                {
                }

                try
                {
                    var i = v.ToPrimitiveValue<int>();
                    return i != 0;
                }
                catch
                {
                }

                string str = null;
                try
                {
                    str = v.ToPrimitiveValue<string>();
                }
                catch
                {
                    str = null;
                }

                if (string.IsNullOrWhiteSpace(str))
                    return false;

                if (bool.TryParse(str, out var b))
                    return b;

                if (int.TryParse(str, out var i2))
                    return i2 != 0;

                return string.Equals(str.Trim(), "Y", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(str.Trim(), "Yes", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(str.Trim(), "On", StringComparison.OrdinalIgnoreCase);
            }
        }

        private class ColorPropertyMapOprtCellExector : IMapOprtCellExector
        {
            private readonly string propName;
            private readonly Action<Color> setAction;
            private IMapOprtCellExectorCallBack callBack;

            public ColorPropertyMapOprtCellExector(string propName, Action<Color> setAction)
            {
                this.propName = propName;
                this.setAction = setAction;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                GriffinsBaseValue val = callBack.GetMapCellPropValue(propName);
                if (val == null)
                    return;

                var colorStr = val.ToPrimitiveValue<string>();
                if (string.IsNullOrWhiteSpace(colorStr))
                    return;

                setAction?.Invoke(Color.Parse(colorStr));
            }
        }

        private class TextFontMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is StatusInfoViewModel vm)
                {
                    GriffinsBaseValue val = callBack.GetMapCellPropValue(nameof(StatusInfoPropertyModelEdit.TextFont));
                    if (val != null)
                    {
                        ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
                        GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                        IGriffinsBaseValue iMPPropObjectValue = new FontInfo();
                        iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                        vm.TextFont = (FontInfo)iMPPropObjectValue;
                    }
                }
            }
        }

        #endregion

        #region 操作原子执行重写

        /// <summary>
        /// 重写操作原子执行方法，支持动态操作原子
        /// </summary>
        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            try
            {
                // 处理原有的操作原子
            if (mapOprtCellInstInfo.OprtCellID == StatusInfoMapOprtCellConst.TextColor_MapOprtCellID)
                return ExecColorProperty(mapOprtCellInstInfo, nameof(StatusInfoPropertyModelEdit.TextColor), (value) => {
                    viewModel.TextColor = value;
                    StatusInfoPropertyModelEdit.TextColor = value;
                });
            if (mapOprtCellInstInfo.OprtCellID == StatusInfoMapOprtCellConst.BackColor_MapOprtCellID)
                return ExecColorProperty(mapOprtCellInstInfo, nameof(StatusInfoPropertyModelEdit.BackColor), (value) => {
                    viewModel.BackColor = value;
                    StatusInfoPropertyModelEdit.BackColor = value;
                });
            if (mapOprtCellInstInfo.OprtCellID == StatusInfoMapOprtCellConst.TextFont_MapOprtCellID)
                return ExecTextFontProperty(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == StatusInfoMapOprtCellConst.IsDualValve_MapOprtCellID)
                return ExecBoolProperty(mapOprtCellInstInfo, nameof(StatusInfoPropertyModelEdit.IsDualValve), (value) => {
                    viewModel.IsDualValve = value;
                    StatusInfoPropertyModelEdit.IsDualValve = value;
                });
            if (mapOprtCellInstInfo.OprtCellID == StatusInfoMapOprtCellConst.StateInfo_MapOprtCellID)
                return ExecStateInfoProperty(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == StatusInfoMapOprtCellConst.TimeInfo_MapOprtCellID)
                return ExecTimeInfoProperty(mapOprtCellInstInfo);

                // 处理动态操作原子
                if (mapOprtCellInstInfo.OprtCellID == StatusInfoMapOprtCellConst.DynamicStatusUpdate_MapOprtCellID)
                    return ExecDynamicStatusUpdateProperty(mapOprtCellInstInfo);
                if (mapOprtCellInstInfo.OprtCellID == StatusInfoMapOprtCellConst.DynamicTimeUpdate_MapOprtCellID)
                    return ExecDynamicTimeUpdateProperty(mapOprtCellInstInfo);
                if (mapOprtCellInstInfo.OprtCellID == StatusInfoMapOprtCellConst.DynamicColorChange_MapOprtCellID)
                    return ExecDynamicColorChangeProperty(mapOprtCellInstInfo);

                return base.ExecOprtCell(mapOprtCellInstInfo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"执行操作原子时发生错误: {ex.Message}");
                return false;
            }
        }

        protected override bool SetUIDataObjPropValues(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            bool needExecStateInfo = false;
            bool needExecTimeInfo = false;

            foreach (GFBaseTypePropValue gFBaseTypePropValue in gFBaseTypePropValues)
            {
                if (gFBaseTypePropValue == null)
                    continue;

                string propId = gFBaseTypePropValue.PropertyID.ToString();
                if (string.IsNullOrWhiteSpace(propId))
                    continue;

                if (string.Compare(propId, nameof(StatusInfoPropertyModelEdit.IsDualValve)) == 0)
                {
                    SetPropertyValue(nameof(StatusInfoPropertyModelEdit.IsDualValve), gFBaseTypePropValue.Value, true);
                    continue;
                }

                if (string.Compare(propId, nameof(StatusInfoPropertyModelEdit.LeftValveGlueMonitorState)) == 0)
                {
                    SetPropertyValue(nameof(StatusInfoPropertyModelEdit.LeftValveGlueMonitorState), gFBaseTypePropValue.Value, true);
                    needExecStateInfo = true;
                    continue;
                }
                if (string.Compare(propId, nameof(StatusInfoPropertyModelEdit.LeftValveQuantitativeGlueMonitorState)) == 0)
                {
                    SetPropertyValue(nameof(StatusInfoPropertyModelEdit.LeftValveQuantitativeGlueMonitorState), gFBaseTypePropValue.Value, true);
                    needExecStateInfo = true;
                    continue;
                }
                if (string.Compare(propId, nameof(StatusInfoPropertyModelEdit.LeftValveRemainingMonitorState)) == 0)
                {
                    SetPropertyValue(nameof(StatusInfoPropertyModelEdit.LeftValveRemainingMonitorState), gFBaseTypePropValue.Value, true);
                    needExecStateInfo = true;
                    continue;
                }
                if (string.Compare(propId, nameof(StatusInfoPropertyModelEdit.LeftPressureCyclesAlarmState)) == 0)
                {
                    SetPropertyValue(nameof(StatusInfoPropertyModelEdit.LeftPressureCyclesAlarmState), gFBaseTypePropValue.Value, true);
                    needExecStateInfo = true;
                    continue;
                }

                if (string.Compare(propId, nameof(StatusInfoPropertyModelEdit.RightValveGlueMonitorState)) == 0)
                {
                    SetPropertyValue(nameof(StatusInfoPropertyModelEdit.RightValveGlueMonitorState), gFBaseTypePropValue.Value, true);
                    needExecStateInfo = true;
                    continue;
                }
                if (string.Compare(propId, nameof(StatusInfoPropertyModelEdit.RightValveQuantitativeGlueMonitorState)) == 0)
                {
                    SetPropertyValue(nameof(StatusInfoPropertyModelEdit.RightValveQuantitativeGlueMonitorState), gFBaseTypePropValue.Value, true);
                    needExecStateInfo = true;
                    continue;
                }
                if (string.Compare(propId, nameof(StatusInfoPropertyModelEdit.RightValveRemainingMonitorState)) == 0)
                {
                    SetPropertyValue(nameof(StatusInfoPropertyModelEdit.RightValveRemainingMonitorState), gFBaseTypePropValue.Value, true);
                    needExecStateInfo = true;
                    continue;
                }
                if (string.Compare(propId, nameof(StatusInfoPropertyModelEdit.RightPressureCyclesAlarmState)) == 0)
                {
                    SetPropertyValue(nameof(StatusInfoPropertyModelEdit.RightPressureCyclesAlarmState), gFBaseTypePropValue.Value, true);
                    needExecStateInfo = true;
                    continue;
                }

                if (string.Compare(propId, nameof(StatusInfoPropertyModelEdit.AWaitingAddGlueTime)) == 0)
                {
                    SetPropertyValue(nameof(StatusInfoPropertyModelEdit.AWaitingAddGlueTime), gFBaseTypePropValue.Value, true);
                    needExecTimeInfo = true;
                    continue;
                }
                if (string.Compare(propId, nameof(StatusInfoPropertyModelEdit.BWaitingAddGlueTime)) == 0)
                {
                    SetPropertyValue(nameof(StatusInfoPropertyModelEdit.BWaitingAddGlueTime), gFBaseTypePropValue.Value, true);
                    needExecTimeInfo = true;
                    continue;
                }
            }

            if (needExecStateInfo)
            {
                CallBack?.ExecOprt(nameof(StatusInfoPropertyModelEdit.StateInfo));
            }

            if (needExecTimeInfo)
            {
                CallBack?.ExecOprt(nameof(StatusInfoPropertyModelEdit.TimeInfo));
            }

            return true;
        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            if (string.Compare(propertyID, nameof(StatusInfoPropertyModelEdit.TextColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(StatusInfoPropertyModelEdit.TextColor));
            }
            if (string.Compare(propertyID, nameof(StatusInfoPropertyModelEdit.BackColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(StatusInfoPropertyModelEdit.BackColor));
            }
            if (string.Compare(propertyID, nameof(StatusInfoPropertyModelEdit.TextFont)) == 0)
            {
                CallBack?.ExecOprt(nameof(StatusInfoPropertyModelEdit.TextFont));
            }
            if (string.Compare(propertyID, nameof(StatusInfoPropertyModelEdit.IsDualValve)) == 0)
            {
                CallBack?.ExecOprt(nameof(StatusInfoPropertyModelEdit.IsDualValve));
            }
            if (string.Compare(propertyID, nameof(StatusInfoPropertyModelEdit.StateInfo)) == 0)
            {
                CallBack?.ExecOprt(nameof(StatusInfoPropertyModelEdit.StateInfo));
            }
            if (string.Compare(propertyID, nameof(StatusInfoPropertyModelEdit.TimeInfo)) == 0)
            {
                CallBack?.ExecOprt(nameof(StatusInfoPropertyModelEdit.TimeInfo));
            }

            if (!StatusInfoPropertyModelEdit.IsRuning)
            {
                GFBaseTypePropValueList gFBaseTypePropValues = new GFBaseTypePropValueList();

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(StatusInfoPropertyModelEdit.IsDualValve)), new GriffinsBaseValue(StatusInfoPropertyModelEdit.IsDualValve)));

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(StatusInfoPropertyModelEdit.LeftValveGlueMonitorState)), new GriffinsBaseValue(StatusInfoPropertyModelEdit.LeftValveGlueMonitorState)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(StatusInfoPropertyModelEdit.LeftValveQuantitativeGlueMonitorState)), new GriffinsBaseValue(StatusInfoPropertyModelEdit.LeftValveQuantitativeGlueMonitorState)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(StatusInfoPropertyModelEdit.LeftValveRemainingMonitorState)), new GriffinsBaseValue(StatusInfoPropertyModelEdit.LeftValveRemainingMonitorState)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(StatusInfoPropertyModelEdit.LeftPressureCyclesAlarmState)), new GriffinsBaseValue(StatusInfoPropertyModelEdit.LeftPressureCyclesAlarmState)));

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(StatusInfoPropertyModelEdit.RightValveGlueMonitorState)), new GriffinsBaseValue(StatusInfoPropertyModelEdit.RightValveGlueMonitorState)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(StatusInfoPropertyModelEdit.RightValveQuantitativeGlueMonitorState)), new GriffinsBaseValue(StatusInfoPropertyModelEdit.RightValveQuantitativeGlueMonitorState)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(StatusInfoPropertyModelEdit.RightValveRemainingMonitorState)), new GriffinsBaseValue(StatusInfoPropertyModelEdit.RightValveRemainingMonitorState)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(StatusInfoPropertyModelEdit.RightPressureCyclesAlarmState)), new GriffinsBaseValue(StatusInfoPropertyModelEdit.RightPressureCyclesAlarmState)));

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(StatusInfoPropertyModelEdit.AWaitingAddGlueTime)), new GriffinsBaseValue(StatusInfoPropertyModelEdit.AWaitingAddGlueTime)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(StatusInfoPropertyModelEdit.BWaitingAddGlueTime)), new GriffinsBaseValue(StatusInfoPropertyModelEdit.BWaitingAddGlueTime)));

                CallBack?.UpdateUIDataObjPropValues(gFBaseTypePropValues);
            }
        }

        /// <summary>
        /// 执行动态状态更新操作原子
        /// </summary>
        private bool ExecDynamicStatusUpdateProperty(MapOprtCellInstInfo info)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new DynamicStatusUpdateMapOprtCellExector();
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }

        /// <summary>
        /// 执行动态时间更新操作原子
        /// </summary>
        private bool ExecDynamicTimeUpdateProperty(MapOprtCellInstInfo info)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new DynamicTimeUpdateMapOprtCellExector();
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }

        /// <summary>
        /// 执行动态颜色变化操作原子
        /// </summary>
        private bool ExecDynamicColorChangeProperty(MapOprtCellInstInfo info)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new DynamicColorChangeMapOprtCellExector();
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }

        #endregion

        #region 动态操作原子执行器

        /// <summary>
        /// 动态状态更新操作原子执行器
        /// </summary>
        private class DynamicStatusUpdateMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is StatusInfoViewModel vm)
                {
                    try
                    {
                        if (cfg != null && cfg.Length > 0)
                        {
                            // 从配置中解析状态信息
                            var configStr = System.Text.Encoding.UTF8.GetString(cfg);
                            var config = System.Text.Json.JsonSerializer.Deserialize<DynamicStatusConfig>(configStr);

                            if (config != null)
                            {
                                // 更新UI状态
                                Dispatcher.UIThread.Post(() =>
                                {
                                    vm.LeftValveGlueMonitorState = config.LeftValveGlueMonitorState;
                                    vm.RightValveGlueMonitorState = config.RightValveGlueMonitorState;
                                    vm.RightPressureCyclesAlarmState = config.RightPressureCyclesAlarmState;
                                });
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"动态状态更新执行器错误: {ex.Message}");
                    }

                    // 如果配置解析失败，从属性中获取默认值
                    var val = callBack.GetMapCellPropValue(nameof(StatusInfoPropertyModelEdit.StateInfo));
                    if (val != null)
                    {
                        ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
                        GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                        IGriffinsBaseValue iMPPropObjectValue = new StatusInfoStateInfo();
                        iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                        var infoObj = (StatusInfoStateInfo)iMPPropObjectValue;

                        if (infoObj != null)
                        {
                            Dispatcher.UIThread.Post(() =>
                            {
                                vm.LeftValveGlueMonitorState = infoObj.LeftValveGlueMonitorState;
                                vm.RightValveGlueMonitorState = infoObj.RightValveGlueMonitorState;
                                vm.RightPressureCyclesAlarmState = infoObj.RightPressureCyclesAlarmState;
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 动态时间更新操作原子执行器
        /// </summary>
        private class DynamicTimeUpdateMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is StatusInfoViewModel vm)
                {
                    try
                    {
                        if (cfg != null && cfg.Length > 0)
                        {
                            var configStr = System.Text.Encoding.UTF8.GetString(cfg);
                            var config = System.Text.Json.JsonSerializer.Deserialize<DynamicTimeConfig>(configStr);

                            if (config != null)
                            {
                                Dispatcher.UIThread.Post(() =>
                                {
                                    vm.AWaitingAddGlueTime = string.IsNullOrWhiteSpace(config.AWaitingAddGlueTime) ? "00:00" : config.AWaitingAddGlueTime;
                                    vm.BWaitingAddGlueTime = string.IsNullOrWhiteSpace(config.BWaitingAddGlueTime) ? "00:00" : config.BWaitingAddGlueTime;
                                });
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"动态时间更新执行器错误: {ex.Message}");
                    }

                    // 从属性中获取默认值
                    var val = callBack.GetMapCellPropValue(nameof(StatusInfoPropertyModelEdit.TimeInfo));
                    if (val != null)
                    {
                        ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
                        GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                        IGriffinsBaseValue iMPPropObjectValue = new StatusInfoTimeInfo();
                        iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                        var infoObj = (StatusInfoTimeInfo)iMPPropObjectValue;

                        if (infoObj != null)
                        {
                            Dispatcher.UIThread.Post(() =>
                            {
                                vm.AWaitingAddGlueTime = string.IsNullOrWhiteSpace(infoObj.AWaitingAddGlueTime) ? "00:00" : infoObj.AWaitingAddGlueTime;
                                vm.BWaitingAddGlueTime = string.IsNullOrWhiteSpace(infoObj.BWaitingAddGlueTime) ? "00:00" : infoObj.BWaitingAddGlueTime;
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 动态颜色变化操作原子执行器
        /// </summary>
        private class DynamicColorChangeMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is StatusInfoViewModel vm)
                {
                    try
                    {
                        if (cfg != null && cfg.Length > 0)
                        {
                            var configStr = System.Text.Encoding.UTF8.GetString(cfg);
                            var config = System.Text.Json.JsonSerializer.Deserialize<DynamicColorConfig>(configStr);

                            if (config != null && !string.IsNullOrWhiteSpace(config.BackColor))
                            {
                                Dispatcher.UIThread.Post(() =>
                                {
                                    try
                                    {
                                        var color = Color.Parse(config.BackColor);
                                        vm.BackColor = color;

                                        // 如果有动画持续时间，可以在这里添加动画效果
                                        if (config.AnimationDuration > 0)
                                        {
                                            // 简单的颜色变化动画逻辑
                                            System.Diagnostics.Debug.WriteLine($"颜色动画持续时间: {config.AnimationDuration}ms");
                                        }
                                    }
                                    catch (Exception colorEx)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"颜色解析错误: {colorEx.Message}");
                                    }
                                });
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"动态颜色变化执行器错误: {ex.Message}");
                    }

                    // 从属性中获取默认值
                    var val = callBack.GetMapCellPropValue(nameof(StatusInfoPropertyModelEdit.BackColor));
                    if (val != null)
                    {
                        var colorStr = val.ToPrimitiveValue<string>();
                        if (!string.IsNullOrWhiteSpace(colorStr))
                        {
                            Dispatcher.UIThread.Post(() =>
                            {
                                try
                                {
                                    var color = Color.Parse(colorStr);
                                    vm.BackColor = color;
                                }
                                catch (Exception colorEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"默认颜色解析错误: {colorEx.Message}");
                                }
                            });
                        }
                    }
                }
            }
        }

        #endregion

        #region 动态操作原子配置类

        /// <summary>
        /// 动态状态配置
        /// </summary>
        private class DynamicStatusConfig
        {
            public bool LeftValveGlueMonitorState { get; set; }
            public bool RightValveGlueMonitorState { get; set; }
            public bool RightPressureCyclesAlarmState { get; set; }
        }

        /// <summary>
        /// 动态时间配置
        /// </summary>
        private class DynamicTimeConfig
        {
            public string AWaitingAddGlueTime { get; set; }
            public string BWaitingAddGlueTime { get; set; }
        }

        /// <summary>
        /// 动态颜色配置
        /// </summary>
        private class DynamicColorConfig
        {
            public string BackColor { get; set; }
            public int AnimationDuration { get; set; }
        }

        #endregion

        #region 动态操作原子数据类

        /// <summary>
        /// 动态操作原子数据
        /// </summary>
        private class DynamicOprtCellData
        {
            public string PropertyId { get; set; }
            public Guid InstanceID { get; set; }
            public MapOprtCellID OprtCellID { get; set; }
            public byte[] CfgInfo { get; set; }
        }

        #endregion

        #region 动态操作原子示例

        /// <summary>
        /// 将时间操作原子添加到状态量操作中，实现状态量变化时时间UI同步更新
        /// </summary>
        public void AddTimeOprtCellToStateInfo()
        {
            try
            {
                // 为StateInfo操作添加TimeInfo操作原子
                var success = AddOprtCellToProperty(
                    nameof(StatusInfoPropertyModelEdit.StateInfo),
                    StatusInfoMapOprtCellConst.TimeInfo_MapOprtCellID,
                    null);

                if (success)
                {
                    System.Diagnostics.Debug.WriteLine("成功将时间操作原子添加到状态量操作中");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("添加时间操作原子到状态量操作失败");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"添加时间操作原子时发生错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 移除状态量操作中的时间操作原子
        /// </summary>
        public void RemoveTimeOprtCellFromStateInfo()
        {
            try
            {
                var oprtCells = GetOprtCellsByProperty(nameof(StatusInfoPropertyModelEdit.StateInfo));
                foreach (var oprtCell in oprtCells)
                {
                    if (oprtCell.OprtCellID == StatusInfoMapOprtCellConst.TimeInfo_MapOprtCellID)
                    {
                        var success = RemoveOprtCellFromProperty(nameof(StatusInfoPropertyModelEdit.StateInfo), oprtCell.InstanceID);
                        System.Diagnostics.Debug.WriteLine($"移除状态量操作中的时间操作原子: {(success ? "成功" : "失败")}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"移除时间操作原子时发生错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 演示如何动态添加操作原子
        /// </summary>
        public void DemonstrateDynamicOprtCellAddition()
        {
            try
            {
                // 示例1: 为StateInfo操作添加一个动态状态更新操作原子
                var statusConfig = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(
                    new
                    {
                        LeftValveGlueMonitorState = true,
                        RightValveGlueMonitorState = false,
                        RightPressureCyclesAlarmState = false
                    });

                var success1 = AddOprtCellToProperty(
                    nameof(StatusInfoPropertyModelEdit.StateInfo),
                    StatusInfoMapOprtCellConst.DynamicStatusUpdate_MapOprtCellID,
                    statusConfig);

                System.Diagnostics.Debug.WriteLine($"动态状态更新操作原子添加: {(success1 ? "成功" : "失败")}");

                // 示例2: 为TimeInfo操作添加一个动态时间更新操作原子
                var timeConfig = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(
                    new
                    {
                        AWaitingAddGlueTime = "01:30",
                        BWaitingAddGlueTime = "02:45"
                    });

                var success2 = AddOprtCellToProperty(
                    nameof(StatusInfoPropertyModelEdit.TimeInfo),
                    StatusInfoMapOprtCellConst.DynamicTimeUpdate_MapOprtCellID,
                    timeConfig);

                System.Diagnostics.Debug.WriteLine($"动态时间更新操作原子添加: {(success2 ? "成功" : "失败")}");

                // 示例3: 为BackColor操作添加一个动态颜色变化操作原子
                var colorConfig = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(
                    new
                    {
                        BackColor = "#FF0000",  // 红色
                        AnimationDuration = 1000  // 动画持续时间（毫秒）
                    });

                var success3 = AddOprtCellToProperty(
                    nameof(StatusInfoPropertyModelEdit.BackColor),
                    StatusInfoMapOprtCellConst.DynamicColorChange_MapOprtCellID,
                    colorConfig);

                System.Diagnostics.Debug.WriteLine($"动态颜色变化操作原子添加: {(success3 ? "成功" : "失败")}");

                // 显示当前操作下的所有操作原子
                var stateInfoOprtCells = GetOprtCellsByProperty(nameof(StatusInfoPropertyModelEdit.StateInfo));
                System.Diagnostics.Debug.WriteLine($"StateInfo操作下的操作原子数量: {stateInfoOprtCells.Count()}");

                foreach (var oprtCell in stateInfoOprtCells)
                {
                    System.Diagnostics.Debug.WriteLine($"  - 操作原子ID: {oprtCell.OprtCellID}, 实例ID: {oprtCell.InstanceID}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"演示动态操作原子添加时发生错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 执行指定操作下的所有操作原子（包括动态添加的）
        /// </summary>
        /// <param name="propertyId">属性ID</param>
        public void ExecuteAllOprtCellsForProperty(string propertyId)
        {
            ExecuteOprtByPropertyId(propertyId);
        }

        #endregion
    }

    #region 属性编辑模型
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("状态信息", 2)]
    public class StatusInfoPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        #region 构造函数
        public StatusInfoPropertyModelEdit()
        {
            TextFont.PropertyChanged += textFont_PropertyChanged;
            StateInfo.PropertyChanged += stateInfo_PropertyChanged;
            TimeInfo.PropertyChanged += timeInfo_PropertyChanged;
        }

        #endregion

        #region 私有方法

        private bool _isSettingStateInfo;
        private bool _isSettingTimeInfo;

        private void textFont_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(TextFont));
        }

        private void stateInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isSettingStateInfo)
                return;

            if (!string.IsNullOrWhiteSpace(e?.PropertyName))
                RaisePropertyChanged(e.PropertyName);
        }

        private void timeInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isSettingTimeInfo)
                return;

            if (!string.IsNullOrWhiteSpace(e?.PropertyName))
                RaisePropertyChanged(e.PropertyName);
        }

        #endregion

        #region 属性

        private FontInfo _textFont = new FontInfo(FontInfo.DefaultFont.FontFamily, 16, FontInfo.DefaultFont.FontWeight, FontInfo.DefaultFont.FontStyle);
        [DisplayName("文字字体")]
        [Category("图元信息")]
        [PropertySortOrder(8)]
        public FontInfo TextFont
        {
            get { return _textFont; }
            set { SetProperty(ref _textFont, value); }
        }

        private Color _textColor = Colors.Black;
        [DisplayName("文本颜色")]
        [Category("图元信息")]
        [PropertySortOrder(9)]
        [JsonConverter(typeof(ColorConvert))]
        public Color TextColor
        {
            get { return _textColor; }
            set { SetProperty(ref _textColor, value); }
        }

        private Color _backColor = Colors.White;
        [DisplayName("背景颜色")]
        [Category("图元信息")]
        [PropertySortOrder(10)]
        [JsonConverter(typeof(ColorConvert))]
        public Color BackColor
        {
            get { return _backColor; }
            set { SetProperty(ref _backColor, value); }
        }

        private StatusInfoStateInfo _stateInfo = new StatusInfoStateInfo();
        [DisplayName("状态量")]
        [Category("状态信息")]
        [PropertySortOrder(11)]
        public StatusInfoStateInfo StateInfo
        {
            get { return _stateInfo; }
            set
            {
                if (value == null)
                    value = new StatusInfoStateInfo();

                if (ReferenceEquals(_stateInfo, value))
                    return;

                if (_stateInfo != null)
                    _stateInfo.PropertyChanged -= stateInfo_PropertyChanged;

                _stateInfo = value;
                _stateInfo.PropertyChanged += stateInfo_PropertyChanged;
                RaisePropertyChanged(nameof(StateInfo));
            }
        }

        private StatusInfoTimeInfo _timeInfo = new StatusInfoTimeInfo();
        [DisplayName("时间")]
        [Category("状态信息")]
        [PropertySortOrder(12)]
        public StatusInfoTimeInfo TimeInfo
        {
            get { return _timeInfo; }
            set
            {
                if (value == null)
                    value = new StatusInfoTimeInfo();

                if (ReferenceEquals(_timeInfo, value))
                    return;

                if (_timeInfo != null)
                    _timeInfo.PropertyChanged -= timeInfo_PropertyChanged;

                _timeInfo = value;
                _timeInfo.PropertyChanged += timeInfo_PropertyChanged;
                RaisePropertyChanged(nameof(TimeInfo));
            }
        }

        private bool _isDualValve = false;
        [DisplayName("双阀模式")]
        [Category("状态信息")]
        [PropertySortOrder(10)]
        public bool IsDualValve
        {
            get { return _isDualValve; }
            set { SetProperty(ref _isDualValve, value); }
        }

        [Browsable(false)]
        public bool LeftValveGlueMonitorState
        {
            get { return StateInfo.LeftValveGlueMonitorState; }
            set { StateInfo.LeftValveGlueMonitorState = value; }
        }

        [Browsable(false)]
        public bool LeftValveQuantitativeGlueMonitorState
        {
            get { return StateInfo.LeftValveQuantitativeGlueMonitorState; }
            set { StateInfo.LeftValveQuantitativeGlueMonitorState = value; }
        }

        [Browsable(false)]
        public bool LeftValveRemainingMonitorState
        {
            get { return StateInfo.LeftValveRemainingMonitorState; }
            set { StateInfo.LeftValveRemainingMonitorState = value; }
        }

        [Browsable(false)]
        public bool LeftPressureCyclesAlarmState
        {
            get { return StateInfo.LeftPressureCyclesAlarmState; }
            set { StateInfo.LeftPressureCyclesAlarmState = value; }
        }

        [Browsable(false)]
        public bool RightValveGlueMonitorState
        {
            get { return StateInfo.RightValveGlueMonitorState; }
            set { StateInfo.RightValveGlueMonitorState = value; }
        }

        [Browsable(false)]
        public bool RightValveQuantitativeGlueMonitorState
        {
            get { return StateInfo.RightValveQuantitativeGlueMonitorState; }
            set { StateInfo.RightValveQuantitativeGlueMonitorState = value; }
        }

        [Browsable(false)]
        public bool RightValveRemainingMonitorState
        {
            get { return StateInfo.RightValveRemainingMonitorState; }
            set { StateInfo.RightValveRemainingMonitorState = value; }
        }

        [Browsable(false)]
        public bool RightPressureCyclesAlarmState
        {
            get { return StateInfo.RightPressureCyclesAlarmState; }
            set { StateInfo.RightPressureCyclesAlarmState = value; }
        }

        [Browsable(false)]
        public string AWaitingAddGlueTime
        {
            get { return TimeInfo.AWaitingAddGlueTime; }
            set { TimeInfo.AWaitingAddGlueTime = value; }
        }

        [Browsable(false)]
        public string BWaitingAddGlueTime
        {
            get { return TimeInfo.BWaitingAddGlueTime; }
            set { TimeInfo.BWaitingAddGlueTime = value; }
        }

        #endregion

        #region 重写方法

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(BackColor)) == 0)
            {
                if (propertyVal == null)
                    BackColor = Color.FromArgb(33, 0, 0, 0);
                else
                    BackColor = Color.Parse(propertyVal.ToPrimitiveValue<string>());
                return true;
            }
            if (string.Compare(propertyID, nameof(StateInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<StatusInfoStateInfo>(propertyVal) : new StatusInfoStateInfo();
                StateInfo ??= new StatusInfoStateInfo();

                _isSettingStateInfo = true;
                try
                {
                    StateInfo.LeftValveGlueMonitorState = src.LeftValveGlueMonitorState;
                    StateInfo.LeftValveQuantitativeGlueMonitorState = src.LeftValveQuantitativeGlueMonitorState;
                    StateInfo.LeftValveRemainingMonitorState = src.LeftValveRemainingMonitorState;
                    StateInfo.LeftPressureCyclesAlarmState = src.LeftPressureCyclesAlarmState;
                    StateInfo.RightValveGlueMonitorState = src.RightValveGlueMonitorState;
                    StateInfo.RightValveQuantitativeGlueMonitorState = src.RightValveQuantitativeGlueMonitorState;
                    StateInfo.RightValveRemainingMonitorState = src.RightValveRemainingMonitorState;
                    StateInfo.RightPressureCyclesAlarmState = src.RightPressureCyclesAlarmState;
                }
                finally
                {
                    _isSettingStateInfo = false;
                }

                RaisePropertyChanged(nameof(StateInfo));
                return true;
            }

            if (string.Compare(propertyID, nameof(TimeInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<StatusInfoTimeInfo>(propertyVal) : new StatusInfoTimeInfo();
                TimeInfo ??= new StatusInfoTimeInfo();

                _isSettingTimeInfo = true;
                try
                {
                    TimeInfo.AWaitingAddGlueTime = src.AWaitingAddGlueTime;
                    TimeInfo.BWaitingAddGlueTime = src.BWaitingAddGlueTime;
                }
                finally
                {
                    _isSettingTimeInfo = false;
                }

                RaisePropertyChanged(nameof(TimeInfo));
                return true;
            }

            if (string.Compare(propertyID, nameof(TextColor)) == 0)
            {
                if (propertyVal == null)
                    TextColor = Colors.Black;
                else
                    TextColor = Color.Parse(propertyVal.ToPrimitiveValue<string>());
                return true;
            }

            if (string.Compare(propertyID, nameof(TextFont)) == 0)
            {
                if (propertyVal == null)
                    TextFont = FontInfo.DefaultFont;
                else
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new FontInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    TextFont = (FontInfo)iMPPropObjectValue;
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(LeftValveGlueMonitorState)) == 0)
            {
                LeftValveGlueMonitorState = ParseBool(propertyVal);
                return true;
            }

            if (string.Compare(propertyID, nameof(LeftValveQuantitativeGlueMonitorState)) == 0)
            {
                LeftValveQuantitativeGlueMonitorState = ParseBool(propertyVal);
                return true;
            }

            if (string.Compare(propertyID, nameof(LeftValveRemainingMonitorState)) == 0)
            {
                LeftValveRemainingMonitorState = ParseBool(propertyVal);
                return true;
            }

            if (string.Compare(propertyID, nameof(LeftPressureCyclesAlarmState)) == 0)
            {
                LeftPressureCyclesAlarmState = ParseBool(propertyVal);
                return true;
            }

            if (string.Compare(propertyID, nameof(RightValveGlueMonitorState)) == 0)
            {
                RightValveGlueMonitorState = ParseBool(propertyVal);
                return true;
            }

            if (string.Compare(propertyID, nameof(RightValveQuantitativeGlueMonitorState)) == 0)
            {
                RightValveQuantitativeGlueMonitorState = ParseBool(propertyVal);
                return true;
            }

            if (string.Compare(propertyID, nameof(RightValveRemainingMonitorState)) == 0)
            {
                RightValveRemainingMonitorState = ParseBool(propertyVal);
                return true;
            }

            if (string.Compare(propertyID, nameof(RightPressureCyclesAlarmState)) == 0)
            {
                RightPressureCyclesAlarmState = ParseBool(propertyVal);
                return true;
            }

            if (string.Compare(propertyID, nameof(IsDualValve)) == 0)
            {
                IsDualValve = ParseBool(propertyVal);
                return true;
            }

            if (string.Compare(propertyID, nameof(AWaitingAddGlueTime)) == 0)
            {
                if (propertyVal == null)
                {
                    AWaitingAddGlueTime = "00:00";
                }
                else
                {
                    var str = propertyVal.ToPrimitiveValue<string>();
                    AWaitingAddGlueTime = string.IsNullOrWhiteSpace(str) ? "00:00" : str;
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(BWaitingAddGlueTime)) == 0)
            {
                if (propertyVal == null)
                {
                    BWaitingAddGlueTime = "00:00";
                }
                else
                {
                    var str = propertyVal.ToPrimitiveValue<string>();
                    BWaitingAddGlueTime = string.IsNullOrWhiteSpace(str) ? "00:00" : str;
                }
                return true;
            }
            return base.SetPropertyValue(propertyID, propertyVal);
        }

        private static T DeserializeObject<T>(GriffinsBaseValue val) where T : IGriffinsBaseValue, new()
        {
            ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
            IGriffinsBaseValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }

        #endregion

        #region 私有方法

        private static bool ParseBool(GriffinsBaseValue v)
        {
            if (v == null)
                return false;

            try
            {
                return v.ToPrimitiveValue<bool>();
            }
            catch
            {
            }

            try
            {
                var i = v.ToPrimitiveValue<int>();
                return i != 0;
            }
            catch
            {
            }

            string str = null;
            try
            {
                str = v.ToPrimitiveValue<string>();
            }
            catch
            {
                str = null;
            }

            if (string.IsNullOrWhiteSpace(str))
                return false;

            if (bool.TryParse(str, out var b))
                return b;

            if (int.TryParse(str, out var i2))
                return i2 != 0;

            return string.Equals(str.Trim(), "Y", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(str.Trim(), "Yes", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(str.Trim(), "On", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region 公共方法

        public void CopyFrom(StatusInfoPropertyModelEdit source)
        {
            base.CopyFrom(source);

            if (source.TextFont != null)
                this.TextFont = new FontInfo(source.TextFont.FontFamily, source.TextFont.FontSize, source.TextFont.FontWeight, source.TextFont.FontStyle);

            this.TextColor = source.TextColor;
            this.BackColor = source.BackColor;
            this.IsDualValve = source.IsDualValve;

            if (source.StateInfo != null)
            {
                StateInfo.LeftValveGlueMonitorState = source.StateInfo.LeftValveGlueMonitorState;
                StateInfo.LeftValveQuantitativeGlueMonitorState = source.StateInfo.LeftValveQuantitativeGlueMonitorState;
                StateInfo.LeftValveRemainingMonitorState = source.StateInfo.LeftValveRemainingMonitorState;
                StateInfo.LeftPressureCyclesAlarmState = source.StateInfo.LeftPressureCyclesAlarmState;
                StateInfo.RightValveGlueMonitorState = source.StateInfo.RightValveGlueMonitorState;
                StateInfo.RightValveQuantitativeGlueMonitorState = source.StateInfo.RightValveQuantitativeGlueMonitorState;
                StateInfo.RightValveRemainingMonitorState = source.StateInfo.RightValveRemainingMonitorState;
                StateInfo.RightPressureCyclesAlarmState = source.StateInfo.RightPressureCyclesAlarmState;
            }

            if (source.TimeInfo != null)
            {
                TimeInfo.AWaitingAddGlueTime = string.IsNullOrWhiteSpace(source.TimeInfo.AWaitingAddGlueTime) ? "00:00" : source.TimeInfo.AWaitingAddGlueTime;
                TimeInfo.BWaitingAddGlueTime = string.IsNullOrWhiteSpace(source.TimeInfo.BWaitingAddGlueTime) ? "00:00" : source.TimeInfo.BWaitingAddGlueTime;
            }
        }

        #endregion
    }
    #endregion

    #region 属性绑定编辑模型
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class StatusInfoPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        #region 绑定属性
        private string _textFont = "TextFont";
        [DisplayName("文本字体")]
        [Category("绑定信息")]
        [PropertySortOrder(7)]
        [Browsable(false)]
        public string TextFont
        {
            get { return _textFont; }
            set { SetProperty(ref _textFont, value); }
        }

        private string _textColor = "TextColor";
        [DisplayName("文本颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(8)]
        [Browsable(false)]
        public string TextColor
        {
            get { return _textColor; }
            set { SetProperty(ref _textColor, value); }
        }

        private string _backColor = "BackColor";
        [DisplayName("背景颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(9)]
        [Browsable(false)]
        public string BackColor
        {
            get { return _backColor; }
            set { SetProperty(ref _backColor, value); }
        }

        private string _stateInfo = nameof(StatusInfoPropertyModelEdit.StateInfo);
        [DisplayName("状态量")]
        [Category("绑定信息")]
        [PropertySortOrder(10)]
        [Browsable(false)]
        public string StateInfo
        {
            get { return _stateInfo; }
            set { SetProperty(ref _stateInfo, value); }
        }

        private string _timeInfo = nameof(StatusInfoPropertyModelEdit.TimeInfo);
        [DisplayName("时间")]
        [Category("绑定信息")]
        [PropertySortOrder(11)]
        [Browsable(false)]
        public string TimeInfo
        {
            get { return _timeInfo; }
            set { SetProperty(ref _timeInfo, value); }
        }

        private string _aWaitingAddGlueTime = nameof(StatusInfoPropertyModelEdit.AWaitingAddGlueTime);
        [DisplayName("A轨待补加热时间(m:s)")]
        [Category("绑定信息")]
        [PropertySortOrder(12)]
        [Browsable(false)]
        public string AWaitingAddGlueTime
        {
            get { return _aWaitingAddGlueTime; }
            set { SetProperty(ref _aWaitingAddGlueTime, value); }
        }

        private string _bWaitingAddGlueTime = nameof(StatusInfoPropertyModelEdit.BWaitingAddGlueTime);
        [DisplayName("B轨待补加热时间(m:s)")]
        [Category("绑定信息")]
        [PropertySortOrder(13)]
        [Browsable(false)]
        public string BWaitingAddGlueTime
        {
            get { return _bWaitingAddGlueTime; }
            set { SetProperty(ref _bWaitingAddGlueTime, value); }
        }

        private string _leftValveGlueMonitorState = nameof(StatusInfoPropertyModelEdit.LeftValveGlueMonitorState);
        [DisplayName("左阀定时称重")]
        [Category("绑定信息")]
        [PropertySortOrder(14)]
        [Browsable(false)]
        public string LeftValveGlueMonitorState
        {
            get { return _leftValveGlueMonitorState; }
            set { SetProperty(ref _leftValveGlueMonitorState, value); }
        }

        private string _leftValveQuantitativeGlueMonitorState = nameof(StatusInfoPropertyModelEdit.LeftValveQuantitativeGlueMonitorState);
        [DisplayName("左阀定时定量称重")]
        [Category("绑定信息")]
        [PropertySortOrder(15)]
        [Browsable(false)]
        public string LeftValveQuantitativeGlueMonitorState
        {
            get { return _leftValveQuantitativeGlueMonitorState; }
            set { SetProperty(ref _leftValveQuantitativeGlueMonitorState, value); }
        }

        private string _leftValveRemainingMonitorState = nameof(StatusInfoPropertyModelEdit.LeftValveRemainingMonitorState);
        [DisplayName("左阀余量监控")]
        [Category("绑定信息")]
        [PropertySortOrder(16)]
        [Browsable(false)]
        public string LeftValveRemainingMonitorState
        {
            get { return _leftValveRemainingMonitorState; }
            set { SetProperty(ref _leftValveRemainingMonitorState, value); }
        }

        private string _leftPressureCyclesAlarmState = nameof(StatusInfoPropertyModelEdit.LeftPressureCyclesAlarmState);
        [DisplayName("左压电阀总次数报警")]
        [Category("绑定信息")]
        [PropertySortOrder(17)]
        [Browsable(false)]
        public string LeftPressureCyclesAlarmState
        {
            get { return _leftPressureCyclesAlarmState; }
            set { SetProperty(ref _leftPressureCyclesAlarmState, value); }
        }

        private string _rightValveGlueMonitorState = nameof(StatusInfoPropertyModelEdit.RightValveGlueMonitorState);
        [DisplayName("右阀定时称重")]
        [Category("绑定信息")]
        [PropertySortOrder(18)]
        [Browsable(false)]
        public string RightValveGlueMonitorState
        {
            get { return _rightValveGlueMonitorState; }
            set { SetProperty(ref _rightValveGlueMonitorState, value); }
        }

        private string _rightValveQuantitativeGlueMonitorState = nameof(StatusInfoPropertyModelEdit.RightValveQuantitativeGlueMonitorState);
        [DisplayName("右阀定时定量称重")]
        [Category("绑定信息")]
        [PropertySortOrder(19)]
        [Browsable(false)]
        public string RightValveQuantitativeGlueMonitorState
        {
            get { return _rightValveQuantitativeGlueMonitorState; }
            set { SetProperty(ref _rightValveQuantitativeGlueMonitorState, value); }
        }

        private string _rightValveRemainingMonitorState = nameof(StatusInfoPropertyModelEdit.RightValveRemainingMonitorState);
        [DisplayName("右阀余量监控")]
        [Category("绑定信息")]
        [PropertySortOrder(20)]
        [Browsable(false)]
        public string RightValveRemainingMonitorState
        {
            get { return _rightValveRemainingMonitorState; }
            set { SetProperty(ref _rightValveRemainingMonitorState, value); }
        }

        private string _rightPressureCyclesAlarmState = nameof(StatusInfoPropertyModelEdit.RightPressureCyclesAlarmState);
        [DisplayName("右压电阀总次数报警")]
        [Category("绑定信息")]
        [PropertySortOrder(21)]
        [Browsable(false)]
        public string RightPressureCyclesAlarmState
        {
            get { return _rightPressureCyclesAlarmState; }
            set { SetProperty(ref _rightPressureCyclesAlarmState, value); }
        }

        private string _isDualValve = nameof(StatusInfoPropertyModelEdit.IsDualValve);
        [DisplayName("双阀模式")]
        [Category("绑定信息")]
        [PropertySortOrder(22)]
        [Browsable(false)]
        public string IsDualValve
        {
            get { return _isDualValve; }
            set { SetProperty(ref _isDualValve, value); }
        }

        #endregion

        #region 公共方法

        public void CopyFrom(StatusInfoPropertyBindEditModel source)
        {
            base.CopyFrom(source);
            this.TextFont = source.TextFont;
            this.TextColor = source.TextColor;
            this.BackColor = source.BackColor;
            this.StateInfo = source.StateInfo;
            this.TimeInfo = source.TimeInfo;
            this.AWaitingAddGlueTime = source.AWaitingAddGlueTime;
            this.BWaitingAddGlueTime = source.BWaitingAddGlueTime;
            this.LeftValveGlueMonitorState = source.LeftValveGlueMonitorState;
            this.LeftValveQuantitativeGlueMonitorState = source.LeftValveQuantitativeGlueMonitorState;
            this.LeftValveRemainingMonitorState = source.LeftValveRemainingMonitorState;
            this.LeftPressureCyclesAlarmState = source.LeftPressureCyclesAlarmState;
            this.RightValveGlueMonitorState = source.RightValveGlueMonitorState;
            this.RightValveQuantitativeGlueMonitorState = source.RightValveQuantitativeGlueMonitorState;
            this.RightValveRemainingMonitorState = source.RightValveRemainingMonitorState;
            this.RightPressureCyclesAlarmState = source.RightPressureCyclesAlarmState;
            this.IsDualValve = source.IsDualValve;
        }

        #endregion
    }
    #endregion

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class StatusInfoStateInfo : MiniReactiveObject, IJsonValueConvert, IGriffinsBaseValue
    {
        public static readonly StatusInfoStateInfo Default = new StatusInfoStateInfo(false, false, false, false, false, false, false, false);
        public static readonly Guid Object_ID = new Guid("{9D3C0C25-17A6-4C5F-A40A-41A7A3B6B1F4}");

        private bool _leftValveGlueMonitorState;
        private bool _leftValveQuantitativeGlueMonitorState;
        private bool _leftValveRemainingMonitorState;
        private bool _leftPressureCyclesAlarmState;
        private bool _rightValveGlueMonitorState;
        private bool _rightValveQuantitativeGlueMonitorState;
        private bool _rightValveRemainingMonitorState;
        private bool _rightPressureCyclesAlarmState;

        public StatusInfoStateInfo() : this(false, false, false, false, false, false, false, false) { }

        public StatusInfoStateInfo(bool leftValveGlueMonitorState, bool leftValveQuantitativeGlueMonitorState, bool leftValveRemainingMonitorState, bool leftPressureCyclesAlarmState, bool rightValveGlueMonitorState, bool rightValveQuantitativeGlueMonitorState, bool rightValveRemainingMonitorState, bool rightPressureCyclesAlarmState)
        {
            LeftValveGlueMonitorState = leftValveGlueMonitorState;
            LeftValveQuantitativeGlueMonitorState = leftValveQuantitativeGlueMonitorState;
            LeftValveRemainingMonitorState = leftValveRemainingMonitorState;
            LeftPressureCyclesAlarmState = leftPressureCyclesAlarmState;
            RightValveGlueMonitorState = rightValveGlueMonitorState;
            RightValveQuantitativeGlueMonitorState = rightValveQuantitativeGlueMonitorState;
            RightValveRemainingMonitorState = rightValveRemainingMonitorState;
            RightPressureCyclesAlarmState = rightPressureCyclesAlarmState;
        }

        [DisplayName("左阀定时称重")]
        public bool LeftValveGlueMonitorState
        {
            get { return _leftValveGlueMonitorState; }
            set { SetProperty(ref _leftValveGlueMonitorState, value, nameof(LeftValveGlueMonitorState)); }
        }

        [DisplayName("左阀定时定量称重")]
        public bool LeftValveQuantitativeGlueMonitorState
        {
            get { return _leftValveQuantitativeGlueMonitorState; }
            set { SetProperty(ref _leftValveQuantitativeGlueMonitorState, value, nameof(LeftValveQuantitativeGlueMonitorState)); }
        }

        [DisplayName("左阀余量监控")]
        public bool LeftValveRemainingMonitorState
        {
            get { return _leftValveRemainingMonitorState; }
            set { SetProperty(ref _leftValveRemainingMonitorState, value, nameof(LeftValveRemainingMonitorState)); }
        }

        [DisplayName("左压电阀总次数报警")]
        public bool LeftPressureCyclesAlarmState
        {
            get { return _leftPressureCyclesAlarmState; }
            set { SetProperty(ref _leftPressureCyclesAlarmState, value, nameof(LeftPressureCyclesAlarmState)); }
        }

        [DisplayName("右阀定时称重")]
        public bool RightValveGlueMonitorState
        {
            get { return _rightValveGlueMonitorState; }
            set { SetProperty(ref _rightValveGlueMonitorState, value, nameof(RightValveGlueMonitorState)); }
        }

        [DisplayName("右阀定时定量称重")]
        public bool RightValveQuantitativeGlueMonitorState
        {
            get { return _rightValveQuantitativeGlueMonitorState; }
            set { SetProperty(ref _rightValveQuantitativeGlueMonitorState, value, nameof(RightValveQuantitativeGlueMonitorState)); }
        }

        [DisplayName("右阀余量监控")]
        public bool RightValveRemainingMonitorState
        {
            get { return _rightValveRemainingMonitorState; }
            set { SetProperty(ref _rightValveRemainingMonitorState, value, nameof(RightValveRemainingMonitorState)); }
        }

        [DisplayName("右压电阀总次数报警")]
        public bool RightPressureCyclesAlarmState
        {
            get { return _rightPressureCyclesAlarmState; }
            set { SetProperty(ref _rightPressureCyclesAlarmState, value, nameof(RightPressureCyclesAlarmState)); }
        }

        bool IGriffinsBaseValue.IsObject_Byte => false;

        Guid IGriffinsBaseValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID);
            objectValue_Json.JsonVal = ((IJsonValueConvert)this).ToJsonDataObject();
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue != null && baseValue.Val != null)
            {
                if (!(baseValue.Val is ObjectValue_Json))
                {
                    throw new Exception("对象值不是StatusInfoStateInfo转换的");
                }

                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                {
                    throw new Exception("对象值不是StatusInfoStateInfo转换的");
                }

                ((IJsonValueConvert)this).FromJsonDataObject((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
            {
                throw new ArgumentNullException(nameof(jsonDataObject));
            }

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement rootElement = jsonDocument.RootElement;
            JsonElement value;
            LeftValveGlueMonitorState = rootElement.TryGetProperty("LeftValveGlueMonitorState", out value) && value.GetBoolean();
            LeftValveQuantitativeGlueMonitorState = rootElement.TryGetProperty("LeftValveQuantitativeGlueMonitorState", out value) && value.GetBoolean();
            LeftValveRemainingMonitorState = rootElement.TryGetProperty("LeftValveRemainingMonitorState", out value) && value.GetBoolean();
            LeftPressureCyclesAlarmState = rootElement.TryGetProperty("LeftPressureCyclesAlarmState", out value) && value.GetBoolean();
            RightValveGlueMonitorState = rootElement.TryGetProperty("RightValveGlueMonitorState", out value) && value.GetBoolean();
            RightValveQuantitativeGlueMonitorState = rootElement.TryGetProperty("RightValveQuantitativeGlueMonitorState", out value) && value.GetBoolean();
            RightValveRemainingMonitorState = rootElement.TryGetProperty("RightValveRemainingMonitorState", out value) && value.GetBoolean();
            RightPressureCyclesAlarmState = rootElement.TryGetProperty("RightPressureCyclesAlarmState", out value) && value.GetBoolean();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                LeftValveGlueMonitorState = LeftValveGlueMonitorState,
                LeftValveQuantitativeGlueMonitorState = LeftValveQuantitativeGlueMonitorState,
                LeftValveRemainingMonitorState = LeftValveRemainingMonitorState,
                LeftPressureCyclesAlarmState = LeftPressureCyclesAlarmState,
                RightValveGlueMonitorState = RightValveGlueMonitorState,
                RightValveQuantitativeGlueMonitorState = RightValveQuantitativeGlueMonitorState,
                RightValveRemainingMonitorState = RightValveRemainingMonitorState,
                RightPressureCyclesAlarmState = RightPressureCyclesAlarmState,
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        public override string ToString() => "状态量";
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class StatusInfoTimeInfo : MiniReactiveObject, IJsonValueConvert, IGriffinsBaseValue
    {
        public static readonly StatusInfoTimeInfo Default = new StatusInfoTimeInfo("00:00", "00:00");
        public static readonly Guid Object_ID = new Guid("{E10F70B5-7A9E-49E6-BE64-CB3E3E1CC7B4}");

        private string _aWaitingAddGlueTime = "00:00";
        private string _bWaitingAddGlueTime = "00:00";

        public StatusInfoTimeInfo() : this("00:00", "00:00") { }

        public StatusInfoTimeInfo(string aWaitingAddGlueTime, string bWaitingAddGlueTime)
        {
            AWaitingAddGlueTime = aWaitingAddGlueTime;
            BWaitingAddGlueTime = bWaitingAddGlueTime;
        }

        [DisplayName("A轨待补加热时间(m:s)")]
        public string AWaitingAddGlueTime
        {
            get { return _aWaitingAddGlueTime; }
            set { SetProperty(ref _aWaitingAddGlueTime, string.IsNullOrWhiteSpace(value) ? "00:00" : value, nameof(AWaitingAddGlueTime)); }
        }

        [DisplayName("B轨待补加热时间(m:s)")]
        public string BWaitingAddGlueTime
        {
            get { return _bWaitingAddGlueTime; }
            set { SetProperty(ref _bWaitingAddGlueTime, string.IsNullOrWhiteSpace(value) ? "00:00" : value, nameof(BWaitingAddGlueTime)); }
        }

        bool IGriffinsBaseValue.IsObject_Byte => false;

        Guid IGriffinsBaseValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID);
            objectValue_Json.JsonVal = ((IJsonValueConvert)this).ToJsonDataObject();
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue != null && baseValue.Val != null)
            {
                if (!(baseValue.Val is ObjectValue_Json))
                {
                    throw new Exception("对象值不是StatusInfoTimeInfo转换的");
                }

                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                {
                    throw new Exception("对象值不是StatusInfoTimeInfo转换的");
                }

                ((IJsonValueConvert)this).FromJsonDataObject((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
            {
                throw new ArgumentNullException(nameof(jsonDataObject));
            }

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement rootElement = jsonDocument.RootElement;
            JsonElement value;
            AWaitingAddGlueTime = rootElement.TryGetProperty("AWaitingAddGlueTime", out value) ? value.GetString() : "00:00";
            BWaitingAddGlueTime = rootElement.TryGetProperty("BWaitingAddGlueTime", out value) ? value.GetString() : "00:00";
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                AWaitingAddGlueTime = AWaitingAddGlueTime ?? "00:00",
                BWaitingAddGlueTime = BWaitingAddGlueTime ?? "00:00",
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        public override string ToString() => "时间";
    }
}
