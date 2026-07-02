using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Objects;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.MapOprtCellParamCfgView;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.ViewModels;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Views;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Avalonia.Threading;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList
{
    internal class MapCellDisplayListCtlObj : ControlCellBase
    {
        private EventBindEditModel _eventBindEditModel;
        private DisplayListView view;
        private DisplayListViewModel viewModel;
        private bool _loadedPropertyEditFromBytes;
        private HashSet<string>? _loadedSelectedRowKeys;
        private IMapOprtCellExectorCallBack _callBack;

        static MapCellDisplayListCtlObj() { }

        public MapCellDisplayListCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false) { }

        public MapCellDisplayListCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            base.SetID(mapCellID);
            base.SetName(mapCellName);

            view = new DisplayListView();

            // 属性注册：CommonInfo（包含列配置、排序、是否允许全选等）
            RegisterProperty(new MapObjPropertyInfo(nameof(DisplayListPropertyModelEdit.CommonInfo), "公共设置", MapCellPropDataType.Object_Json, DisplayListCommonInfo.Object_ID, typeof(DisplayListCommonInfo), false, true, new MapCellPropValue(DisplayListCommonInfo.Default)));
            // 属性注册：SelectedRowKeys（勾选状态持久化）
            RegisterProperty(new MapObjPropertyInfo(nameof(DisplayListPropertyModelEdit.SelectedRowKeys), "勾选行键", MapCellPropDataType.String, Guid.Empty, typeof(string), false, true, new MapCellPropValue(string.Empty)));

            // 操作原子注册：用于“设置公共”操作时刷新 ViewModel
            RegisterOprtCellInfo(new MapOprtCellInfo(DisplayListMapOprtCellConst.CommonInfo_MapOprtCellID, "公共设置操作原子", typeof(CommonInfoMapOprtCellParamCfgView)));

            // 操作注册：将属性模型与操作原子实例绑定
            RegisterOprtInfo(new MapOprtInfo(nameof(DisplayListPropertyModelEdit.CommonInfo), "设置公共", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DisplayListMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));

            (this as IMapCellTypeBase).Name = "显示列表";

            viewModel = new DisplayListViewModel(DisplayListPropertyModelEdit);
            view.DataContext = viewModel;

            DisplayListPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;

            _callBack = this as IMapOprtCellExectorCallBack;
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(DisplayListPropertyModelEdit.CommonInfo));
        }

        public override void OnDispose()
        {
            DisplayListPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;

            view.DataContext = null;
            viewModel?.Dispose();
            viewModel = null;

            base.OnDispose();
        }
        [Browsable(false)]
        public DisplayListPropertyModelEdit DisplayListPropertyModelEdit => PropertyEditModelBase as DisplayListPropertyModelEdit;

                protected override bool SetPropertyValue(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            if (gFBaseTypePropValues == null)
                return false;

            foreach (var gFBaseTypePropValue in gFBaseTypePropValues)
            {
                var propertyId = gFBaseTypePropValue?.PropertyID.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(propertyId))
                    continue;

                SetPropertyValue(propertyId, gFBaseTypePropValue.Value, true);
            }

            return true;
        }
public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal, bool isRuning)
        {
            // 运行态标记：由宿主传入，用于属性模型内部区分设计/运行逻辑
            DisplayListPropertyModelEdit.IsRuning = isRuning;
            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoadedDisplayList(propertyID, propertyVal))
            {
                return true;
            }
            var ok = DisplayListPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
            if (ok)
                ExecuteOprtByPropertyId(propertyID);
            return ok;
        }
        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            if (DisplayListPropertyModelEdit.IsRuning)
                return;
            if (CallBack == null || string.IsNullOrWhiteSpace(propertyID) || propertyValue == null)
                return;

           
            CallBack.UpdatePropertyValue(new GFBaseTypePropValueList()
            {
                    new GFBaseTypePropValue(MPPropertyID.Parse(propertyID), propertyValue)
            });
            
            
        }

        private bool IsDefaultOverwriteForLoadedDisplayList(string propertyID, MapCellPropValue propertyVal)
        {
            try
            {
                if (string.Compare(propertyID, nameof(DisplayListPropertyModelEdit.CommonInfo)) == 0)
                {
                    var incoming = propertyVal != null
                        ? DisplayListPropertyModelEditCommonInfoFromValue(propertyVal)
                        : null;
                    return IsDefault(incoming) && !IsDefault(DisplayListPropertyModelEdit.CommonInfo);
                }
                return false;
            }
            catch { return false; }
        }

        private static DisplayListCommonInfo? DisplayListPropertyModelEditCommonInfoFromValue(MapCellPropValue val)
        {
            try
            {
                var m = new DisplayListCommonInfo();
                var tmp = new DisplayListPropertyModelEdit();
                var ok = tmp.SetPropertyValue(nameof(DisplayListPropertyModelEdit.CommonInfo), val);
                if (!ok) return null;
                return tmp.CommonInfo;
            }
            catch
            {
                return null;
            }
        }

        private static bool IsDefault(DisplayListCommonInfo? info)
        {
            if (info == null) return true;

            var defaultInfo = DisplayListCommonInfo.Default;

            var cols = info.Columns;
            var defaultCols = defaultInfo.Columns;
            var colsDefault = cols == null || defaultCols == null
                ? true
                : (cols.Count == defaultCols.Count && !cols.Where((c, i) =>
                        c == null || defaultCols[i] == null
                            ? !(c == null && defaultCols[i] == null)
                            : !(string.Equals(c.FieldID ?? string.Empty, defaultCols[i].FieldID ?? string.Empty, StringComparison.Ordinal)
                                && string.Equals(c.DisplayName ?? string.Empty, defaultCols[i].DisplayName ?? string.Empty, StringComparison.Ordinal)))
                    .Any());

            return colsDefault
                && info.EnableSelectAll == defaultInfo.EnableSelectAll
                && string.Equals(info.SortField ?? string.Empty, defaultInfo.SortField ?? string.Empty, StringComparison.Ordinal)
                && info.SortDirection == defaultInfo.SortDirection;
        }

        private void ExecuteOprtByPropertyId(string propertyID)
        {
            if (string.IsNullOrWhiteSpace(propertyID)) return;

            var normalized = propertyID;
            var dot = normalized.IndexOf('.');
            if (dot > 0) normalized = normalized.Substring(0, dot);

            if (!string.Equals(normalized, nameof(DisplayListPropertyModelEdit.CommonInfo), StringComparison.Ordinal))
                return;

            TryExecuteOprtInfoById(normalized);
        }

        private bool TryExecuteOprtInfoById(string oprtId)
        {
            try
            {
                foreach (var oprtInfo in EnumerateOprtInfos())
                {
                    var id = GetOprtInfoId(oprtInfo);
                    if (!string.Equals(id, oprtId, StringComparison.Ordinal))
                        continue;

                    var instList = GetOprtInfoInstList(oprtInfo);
                    if (instList == null)
                        return true;

                    foreach (var instObj in instList)
                    {
                        if (instObj is not MapOprtCellInstInfo inst)
                            continue;

                        if (Dispatcher.UIThread.CheckAccess())
                            ExecOprtCell(inst);
                        else
                            Dispatcher.UIThread.Post(() => ExecOprtCell(inst));
                    }
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        private IEnumerable<object> EnumerateOprtInfos()
        {
            foreach (var member in EnumerateInstanceMembers(GetType()))
            {
                var val = GetMemberValue(member, this);
                if (val == null) continue;

                if (val is IDictionary dict)
                {
                    foreach (DictionaryEntry entry in dict)
                        if (entry.Value is MapOprtInfo)
                            yield return entry.Value;
                    continue;
                }

                if (val is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                        if (item is MapOprtInfo)
                            yield return item;
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
                    if (p == null) continue;
                    var v = p.GetValue(oprtInfo);
                    if (v is string s && !string.IsNullOrWhiteSpace(s))
                        return s;
                }
            }
            catch
            {
            }
            return string.Empty;
        }

        private static IEnumerable GetOprtInfoInstList(object oprtInfo)
        {
            try
            {
                var t = oprtInfo.GetType();
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                foreach (var p in t.GetProperties(flags))
                {
                    if (p.GetIndexParameters().Length != 0 || !p.CanRead) continue;
                    if (!typeof(IEnumerable).IsAssignableFrom(p.PropertyType)) continue;
                    if (p.PropertyType == typeof(string)) continue;

                    var val = p.GetValue(oprtInfo) as IEnumerable;
                    if (val == null) continue;

                    // 优先使用常见命名
                    if (string.Equals(p.Name, "OprtCellInstInfoList", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(p.Name, "MapOprtCellInstInfoList", StringComparison.OrdinalIgnoreCase))
                        return val;
                }

                foreach (var f in t.GetFields(flags))
                {
                    if (f.IsStatic) continue;
                    if (!typeof(IEnumerable).IsAssignableFrom(f.FieldType)) continue;
                    if (f.FieldType == typeof(string)) continue;

                    var val = f.GetValue(oprtInfo) as IEnumerable;
                    if (val == null) continue;

                    if (string.Equals(f.Name, "OprtCellInstInfoList", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(f.Name, "MapOprtCellInstInfoList", StringComparison.OrdinalIgnoreCase))
                        return val;
                }
            }
            catch
            {
            }
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
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);

            // 反序列化：属性模型
            string propertyEditJson = br.ReadString("PropertyEditModelBase");
            if (!string.IsNullOrEmpty(propertyEditJson))
            {
                try
                {
                    var propertyEditModelBase = JsonObjConvert.FromJSon<DisplayListPropertyModelEdit>(propertyEditJson);
                    if (propertyEditModelBase != null)
                    {
                        (PropertyEditModelBase as DisplayListPropertyModelEdit).CopyFrom(propertyEditModelBase);
                        _loadedPropertyEditFromBytes = true;
                    }
                }
                catch { }
            }

            try
            {
                var selectedRowKeysJson = br.ReadString("SelectedRowKeys");
                if (!string.IsNullOrWhiteSpace(selectedRowKeysJson))
                {
                    var keys = System.Text.Json.JsonSerializer.Deserialize<List<string>>(selectedRowKeysJson);
                    if (keys != null)
                        _loadedSelectedRowKeys = new HashSet<string>(keys.Where(x => !string.IsNullOrWhiteSpace(x)), StringComparer.Ordinal);
                }
            }
            catch { }

            // 反序列化：属性绑定模型
            string propertyBindJson = br.ReadString("PropertyBindEditModelBase");
            if (!string.IsNullOrEmpty(propertyBindJson))
            {
                try
                {
                    var propertyBindEditModelBase = JsonObjConvert.FromJSon<DisplayListPropertyBindEditModel>(propertyBindJson);
                    if (propertyBindEditModelBase != null)
                        (PropertyBindEditModelBase as DisplayListPropertyBindEditModel)?.CopyFrom(propertyBindEditModelBase);
                }
                catch { }
            }

            // 反序列化：事件绑定模型
            string eventBindJson = br.ReadString("EventBindEditModel");
            if (!string.IsNullOrEmpty(eventBindJson))
            {
                try
                {
                    var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(eventBindJson);
                    if (eventBindEditModel != null)
                        EventBindEditModel.CopyFrom(eventBindEditModel);
                }
                catch { }
            }

            if (_loadedSelectedRowKeys != null && _loadedSelectedRowKeys.Count > 0)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    try
                    {
                        viewModel?.EnsureTestRows();
                        foreach (var r in viewModel.Rows)
                            r.IsSelected = _loadedSelectedRowKeys.Contains(BuildRowKey(r));
                        viewModel?.ReloadFromModel();
                    }
                    catch { }
                });
            }
            else if (_loadedPropertyEditFromBytes)
            {
                Dispatcher.UIThread.Post(() => viewModel?.ReloadFromModel());
            }
        }

        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            // 序列化：按宿主约定写入 3 块信息（属性/绑定/事件）
            // 在写入前把当前勾选状态同步到 PropertyModelEdit，确保宿主能持久化
            try
            {
                var selected = viewModel?.Rows?.Where(r => r.IsSelected).Select(BuildRowKey).ToList() ?? new List<string>();
                DisplayListPropertyModelEdit.SelectedRowKeys = System.Text.Json.JsonSerializer.Serialize(selected);
            }
            catch { }

            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));

            // 将 SelectedRowKeys 写入字节流（兼容旧路径）
            try
            {
                var selected = viewModel?.Rows?.Where(r => r.IsSelected).Select(BuildRowKey).ToList() ?? new List<string>();
                bw.Write("SelectedRowKeys", System.Text.Json.JsonSerializer.Serialize(selected));
            }
            catch { }
        }

        private static string BuildRowKey(DisplayListTestRow row)
        {
            try
            {
                if (row?.Values == null || row.Values.Count == 0)
                    return string.Empty;
                var parts = row.Values
                    .OrderBy(kv => kv.Key, StringComparer.Ordinal)
                    .Select(kv => $"{kv.Key}={kv.Value}")
                    .ToArray();
                return string.Join(";", parts);
            }
            catch
            {
                return string.Empty;
            }
        }

        protected override void OnCopyFrom(ControlCellBase source)
        {
            MapCellDisplayListCtlObj obj = source as MapCellDisplayListCtlObj;
            base._CopyFrom(obj);

            // 拷贝属性模型
            (PropertyEditModelBase as DisplayListPropertyModelEdit).CopyFrom(source.PropertyEditModelBase as DisplayListPropertyModelEdit);

            if (PropertyBindEditModelBase is DisplayListPropertyBindEditModel dest &&
                source.PropertyBindEditModelBase is DisplayListPropertyBindEditModel src)
            {
                dest.CopyFrom(src);
            }
            else
            {
                (PropertyBindEditModelBase as DisplayListPropertyBindEditModel)?.CopyFrom(source.PropertyBindEditModelBase as DisplayListPropertyBindEditModel);
            }
            // 拷贝事件绑定模型
            EventBindEditModel.CopyFrom(source.EventBindEditModel);

            _loadedPropertyEditFromBytes = true;
            Dispatcher.UIThread.Post(() => viewModel?.ReloadFromModel());
        }

        protected override void OnInit()
        {
            base.OnInit();
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    // 从宿主回拉已保存的 SelectedRowKeys（如果宿主有值）
                    var val = _callBack?.GetMapCellPropValue(nameof(DisplayListPropertyModelEdit.SelectedRowKeys));
                    if (val != null)
                    {
                        var json = val.ToString();
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            try
                            {
                                var keys = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json);
                                if (keys != null)
                                {
                                    viewModel?.EnsureTestRows();
                                    var keySet = new HashSet<string>(keys.Where(x => !string.IsNullOrWhiteSpace(x)), StringComparer.Ordinal);
                                    foreach (var r in viewModel.Rows)
                                        r.IsSelected = keySet.Contains(BuildRowKey(r));
                                }
                            }
                            catch { }
                        }
                    }
                    ExecuteOprtByPropertyId(nameof(DisplayListPropertyModelEdit.CommonInfo));
                    viewModel?.ReloadFromModel();
                }
                catch { }
            });
        }

        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => viewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new DisplayListPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new DisplayListPropertyBindEditModel();

        public override EventBindEditModel CreateEventBindEditModel()
        {
            _eventBindEditModel ??= new EventBindEditModel() { EventCmdInfos = new BindingList<EventCmdInfo>() };
            return _eventBindEditModel;
        }

        public override void OnZoomChanged() { }

        public override string ToString() => "显示列表";

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == DisplayListMapOprtCellConst.CommonInfo_MapOprtCellID)
                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);
            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        private bool ExecuteOprtCell<T>(MapOprtCellInstInfo mapOprtCellInstInfo) where T : IMapOprtCellExector, new()
        {
            if (!MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
            {
                mapOprtCellExector = new T();
                mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
            }
            mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
            return true;
        }

        public override MapCellPropValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null;
        }

        private class CommonInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is DisplayListViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<CommonInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                var cols = new System.Collections.ObjectModel.ObservableCollection<DisplayListColumnInfo>();
                                if (param.Columns != null)
                                {
                                    foreach (var c in param.Columns)
                                    {
                                        if (c == null) continue;
                                        cols.Add(new DisplayListColumnInfo { FieldID = c.FieldID, DisplayName = c.DisplayName });
                                    }
                                }

                                vm.CommonInfo.Columns = cols;
                                vm.CommonInfo.EnableSelectAll = param.EnableSelectAll;
                                vm.CommonInfo.SortField = param.SortField ?? string.Empty;
                                vm.CommonInfo.SortDirection = param.SortDirection;

                                Dispatcher.UIThread.Post(() =>
                                {
                                    vm.RaisePropertyChanged(nameof(vm.CommonInfo));
                                    try
                                    {
                                        vm.EnsureTestRows();
                                        vm.ApplySort();
                                    }
                                    catch { }
                                });
                                return;
                            }
                        }
                        catch
                        {
                        }
                    }

                    Dispatcher.UIThread.Post(() =>
                    {
                        vm.RaisePropertyChanged(nameof(vm.CommonInfo));
                        try
                        {
                            vm.EnsureTestRows();
                            vm.ApplySort();
                        }
                        catch { }
                    });
                }
            }
        }
    }

    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    [CategoryPriority("列绑定信息", 3)]
    public class DisplayListPropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _column1 = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("第一列")]
        [Category("列绑定信息")]
        [PropertySortOrder(1)]
        [BindMPPropertyID]
        public PropertyBindInfo Column1
        {
            get => _column1;
            set => SetProperty(ref _column1, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _column2 = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("第二列")]
        [Category("列绑定信息")]
        [PropertySortOrder(2)]
        [BindMPPropertyID]
        public PropertyBindInfo Column2
        {
            get => _column2;
            set => SetProperty(ref _column2, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _column3 = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("第三列")]
        [Category("列绑定信息")]
        [PropertySortOrder(3)]
        [BindMPPropertyID]
        public PropertyBindInfo Column3
        {
            get => _column3;
            set => SetProperty(ref _column3, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _column4 = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("第四列")]
        [Category("列绑定信息")]
        [PropertySortOrder(4)]
        [BindMPPropertyID]
        public PropertyBindInfo Column4
        {
            get => _column4;
            set => SetProperty(ref _column4, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _column5 = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("第五列")]
        [Category("列绑定信息")]
        [PropertySortOrder(5)]
        [BindMPPropertyID]
        public PropertyBindInfo Column5
        {
            get => _column5;
            set => SetProperty(ref _column5, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _column6 = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("第六列")]
        [Category("列绑定信息")]
        [PropertySortOrder(6)]
        [BindMPPropertyID]
        public PropertyBindInfo Column6
        {
            get => _column6;
            set => SetProperty(ref _column6, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _column7 = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("第七列")]
        [Category("列绑定信息")]
        [PropertySortOrder(7)]
        [BindMPPropertyID]
        public PropertyBindInfo Column7
        {
            get => _column7;
            set => SetProperty(ref _column7, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _column8 = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("第八列")]
        [Category("列绑定信息")]
        [PropertySortOrder(8)]
        [BindMPPropertyID]
        public PropertyBindInfo Column8
        {
            get => _column8;
            set => SetProperty(ref _column8, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _column9 = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("第九列")]
        [Category("列绑定信息")]
        [PropertySortOrder(9)]
        [BindMPPropertyID]
        public PropertyBindInfo Column9
        {
            get => _column9;
            set => SetProperty(ref _column9, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _column10 = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("第十列")]
        [Category("列绑定信息")]
        [PropertySortOrder(10)]
        [BindMPPropertyID]
        public PropertyBindInfo Column10
        {
            get => _column10;
            set => SetProperty(ref _column10, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        public void CopyFrom(DisplayListPropertyBindEditModel source)
        {
			// 继承自基类的绑定信息（例如点位信息、通用绑定信息等）
            base.CopyFrom(source);
			// 当前类的列绑定字段
            Column1 = source.Column1;
            Column2 = source.Column2;
            Column3 = source.Column3;
            Column4 = source.Column4;
            Column5 = source.Column5;
            Column6 = source.Column6;
            Column7 = source.Column7;
            Column8 = source.Column8;
            Column9 = source.Column9;
            Column10 = source.Column10;
        }
    }
}

