using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using Griffins;
using GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.Views;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Griffins.Map.UI;
using Griffins.UI2;
using GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.ViewModels;
using Griffins.Map;
using GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.MapOprtCellParamCfgView;
using GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.Objects;

namespace GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus
{
    class MapCellDeviceStatusCtlObj : FunctionalCellBase
    {
        private DeviceStatusView view;
        private DeviceStatusViewModel viewModel;
        private bool _loadedPropertyEditFromBytes;
        private readonly ConcurrentDictionary<Guid, MapOprtCellID> _oprtCellIdByInstanceId = new();

        public MapCellDeviceStatusCtlObj(MapObjID mapCellID, string mapCellName) : this(mapCellID, mapCellName, false) { }

        public MapCellDeviceStatusCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            base.SetID(mapCellID);
            base.SetName(mapCellName);
            view = new DeviceStatusView();

            RegisterProperty(new MapObjPropertyInfo(nameof(DeviceStatusPropertyModelEdit.CommonInfo), ResourceA.CommonInfo, GriffinsBaseDataType.Object_Bytes, DeviceStatusCommonInfo.Object_ID, typeof(DeviceStatusCommonInfo), false, true, new GriffinsBaseValue(DeviceStatusCommonInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(DeviceStatusPropertyModelEdit.StatusName), ResourceA.StatusName, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("状态名称")));
            RegisterProperty(new MapObjPropertyInfo(nameof(DeviceStatusPropertyModelEdit.DeviceStatusValue), ResourceA.DeviceStatusValue, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("设备状态")));
            RegisterProperty(new MapObjPropertyInfo(nameof(DeviceStatusPropertyModelEdit.DeviceStatusUnit), "单位", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("")));
            RegisterProperty(new MapObjPropertyInfo(nameof(DeviceStatusPropertyModelEdit.CurrentIndex), ResourceA.CurrentIndex, GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), true, true, new GriffinsBaseValue(0)));

            RegisterOprtCellInfo(new MapOprtCellInfo(DeviceStatusMapOprtCellConst.CommonInfo_MapOprtCellID, ResourceA.CommonSetting, typeof(CommonInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(DeviceStatusMapOprtCellConst.StatusName_MapOprtCellID, ResourceA.StatusName));
            RegisterOprtCellInfo(new MapOprtCellInfo(DeviceStatusMapOprtCellConst.DeviceStatusValue_MapOprtCellID, ResourceA.DeviceStatusValue));
            RegisterOprtCellInfo(new MapOprtCellInfo(DeviceStatusMapOprtCellConst.DeviceStatusUnit_MapOprtCellID, "单位"));
            RegisterOprtCellInfo(new MapOprtCellInfo(DeviceStatusMapOprtCellConst.CurrentIndex_MapOprtCellID, ResourceA.CurrentIndex));

            RegisterOprtInfo(new MapOprtInfo(nameof(DeviceStatusPropertyModelEdit.CommonInfo), ResourceA.CommonSetting, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DeviceStatusMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(DeviceStatusPropertyModelEdit.StatusName), ResourceA.StatusName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DeviceStatusMapOprtCellConst.StatusName_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(DeviceStatusPropertyModelEdit.DeviceStatusValue), ResourceA.DeviceStatusValue, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DeviceStatusMapOprtCellConst.DeviceStatusValue_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(DeviceStatusPropertyModelEdit.DeviceStatusUnit), "单位", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DeviceStatusMapOprtCellConst.DeviceStatusUnit_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(DeviceStatusPropertyModelEdit.CurrentIndex), ResourceA.CurrentIndex, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DeviceStatusMapOprtCellConst.CurrentIndex_MapOprtCellID, CfgInfo = null } }));

            (this as IMapCellTypeBase).Name = ResourceA.DeviceStatusDisplay;
            viewModel = new DeviceStatusViewModel(designTime, DeviceStatusPropertyModelEdit);
            view.DataContext = viewModel;

            DeviceStatusPropertyModelEdit.CommonInfo.PropertyChanged += (_, e) => ExecuteOprtByPropertyId(nameof(DeviceStatusPropertyModelEdit.CommonInfo));
        }

        public DeviceStatusPropertyModelEdit DeviceStatusPropertyModelEdit => PropertyEditModelBase as DeviceStatusPropertyModelEdit;

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoaded(propertyID, propertyVal)) return true;
            DeviceStatusPropertyModelEdit.IsRuning = isRuning;
            var result = DeviceStatusPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
            if (result)
            {
                // Sync to ViewModel if needed, though properties are bound or updated via Oprt usually
                if (propertyID == nameof(DeviceStatusPropertyModelEdit.StatusName)) viewModel.StatusName = DeviceStatusPropertyModelEdit.StatusName;
                if (propertyID == nameof(DeviceStatusPropertyModelEdit.DeviceStatusValue)) viewModel.DeviceStatusValue = DeviceStatusPropertyModelEdit.DeviceStatusValue;
                if (propertyID == nameof(DeviceStatusPropertyModelEdit.DeviceStatusUnit)) viewModel.DeviceStatusUnit = DeviceStatusPropertyModelEdit.DeviceStatusUnit;
                if (propertyID == nameof(DeviceStatusPropertyModelEdit.CurrentIndex)) viewModel.CurrentIndex = DeviceStatusPropertyModelEdit.CurrentIndex;
            }
            return result;
        }

        private bool IsDefaultOverwriteForLoaded(string propertyID, GriffinsBaseValue propertyVal)
        {
            try
            {
                if (string.Compare(propertyID, nameof(DeviceStatusPropertyModelEdit.CommonInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<DeviceStatusCommonInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(DeviceStatusPropertyModelEdit.CommonInfo);
                }
                return false;
            }
            catch { return false; }
        }

        private static bool IsDefault(DeviceStatusCommonInfo info)
        {
            if (info == null) return true;
            return (info.ImageSources == null || info.ImageSources.Count == 0)
                && info.CurrentIndex == DeviceStatusCommonInfo.Default.CurrentIndex
                && string.Equals(info.StatusName ?? "", DeviceStatusCommonInfo.Default.StatusName ?? "", StringComparison.Ordinal)
                && string.Equals(info.DeviceStatusValue ?? "", DeviceStatusCommonInfo.Default.DeviceStatusValue ?? "", StringComparison.Ordinal)
                && string.Equals(info.DeviceStatusUnit ?? "", DeviceStatusCommonInfo.Default.DeviceStatusUnit ?? "", StringComparison.Ordinal);
        }

        private static T DeserializeObject<T>(GriffinsBaseValue val) where T : IGriffinsBaseValue, new()
        {
            if (val == null) return default;
            try
            {
                ObjectValue_Bytes objectValue_Bytes = val.ToObjectValue_Bytes();
                if (objectValue_Bytes != null)
                {
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Bytes);
                    IGriffinsBaseValue obj = new T();
                    obj.PopulateFromBaseValue(griffinsBaseValue);
                    return (T)obj;
                }
            }
            catch { }
            try
            {
                ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
                if (objectValue_Json != null)
                {
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue obj = new T();
                    obj.PopulateFromBaseValue(griffinsBaseValue);
                    return (T)obj;
                }
            }
            catch { }
            return default;
        }

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            string propertyEditJson = br.ReadString("PropertyEditModelBase");
            if (!string.IsNullOrEmpty(propertyEditJson))
            {
                try
                {
                    var propertyEditModelBase = JsonObjConvert.FromJSon<DeviceStatusPropertyModelEdit>(propertyEditJson);
                    if (propertyEditModelBase != null)
                    {
                        (PropertyEditModelBase as DeviceStatusPropertyModelEdit).CopyFrom(propertyEditModelBase);
                        _loadedPropertyEditFromBytes = true;
                    }
                }
                catch { }
            }
        }

        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
        }

        protected override void OnCopyFrom(FunctionalCellBase source)
        {
            MapCellDeviceStatusCtlObj sourceObj = source as MapCellDeviceStatusCtlObj;
            base._CopyFrom(sourceObj);
            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
        }

        protected override void OnInit() { }
        protected override object OnGetView() => view;
        protected override object OnGetViewModel() => viewModel;
        public override PropertyEditModelBase CreatePropertyModelEditBase() => new DeviceStatusPropertyModelEdit();
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new DeviceStatusPropertyBindEditModel();
        public override EventBindEditModel CreateEventBindEditModel() => null;
        public override void OnZoomChanged() { }
        public override string ToString() => "设备状态显示";

        private void ExecuteOprtByPropertyId(string propertyId)
        {
            if (string.IsNullOrWhiteSpace(propertyId)) return;
            if (!TryGetPrimaryOprtCellId(propertyId, out var primaryOprtCellId)) return;
            TryExecuteOprtInfoById(propertyId, primaryOprtCellId);
        }

        private bool TryGetPrimaryOprtCellId(string propertyId, out MapOprtCellID oprtCellId)
        {
            oprtCellId = default;
            switch (propertyId)
            {
                case nameof(DeviceStatusPropertyModelEdit.CommonInfo): oprtCellId = DeviceStatusMapOprtCellConst.CommonInfo_MapOprtCellID; return true;
                case nameof(DeviceStatusPropertyModelEdit.StatusName): oprtCellId = DeviceStatusMapOprtCellConst.StatusName_MapOprtCellID; return true;
                case nameof(DeviceStatusPropertyModelEdit.DeviceStatusValue): oprtCellId = DeviceStatusMapOprtCellConst.DeviceStatusValue_MapOprtCellID; return true;
                case nameof(DeviceStatusPropertyModelEdit.DeviceStatusUnit): oprtCellId = DeviceStatusMapOprtCellConst.DeviceStatusUnit_MapOprtCellID; return true;
                case nameof(DeviceStatusPropertyModelEdit.CurrentIndex): oprtCellId = DeviceStatusMapOprtCellConst.CurrentIndex_MapOprtCellID; return true;
                default: return false;
            }
        }

        private bool TryExecuteOprtInfoById(string oprtId, MapOprtCellID primaryOprtCellId)
        {
            try
            {
                foreach (var oprtInfo in EnumerateOprtInfos())
                {
                    var id = GetOprtInfoId(oprtInfo);
                    if (!string.Equals(id, oprtId, StringComparison.Ordinal)) continue;
                    var instList = GetOprtInfoInstList(oprtInfo);
                    if (instList == null) return true;
                    foreach (var instObj in instList)
                    {
                        if (instObj is not MapOprtCellInstInfo inst) continue;
                        if (Dispatcher.UIThread.CheckAccess()) ExecOprtCell(inst);
                        else Dispatcher.UIThread.Post(() => ExecOprtCell(inst));
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }

        private IEnumerable<object> EnumerateOprtInfos()
        {
            foreach (var member in EnumerateInstanceMembers(GetType()))
            {
                var val = GetMemberValue(member, this);
                if (val == null) continue;
                if (val is MapOprtInfo oprtInfo) yield return oprtInfo;
                if (val is IEnumerable enumerable && val is not string)
                {
                    foreach (var item in enumerable) if (item is MapOprtInfo info) yield return info;
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
                    if (v is string s && !string.IsNullOrWhiteSpace(s)) return s;
                }
            }
            catch { }
            return null;
        }

        private static IEnumerable GetOprtInfoInstList(object oprtInfo)
        {
            if (oprtInfo is MapOprtInfo info) return info.MapOprtCellInstInfos;
            return null;
        }

        private static IEnumerable<MemberInfo> EnumerateInstanceMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            for (var t = type; t != null; t = t.BaseType)
            {
                foreach (var f in t.GetFields(flags)) if (!f.IsStatic) yield return f;
                foreach (var p in t.GetProperties(flags)) if (p.GetIndexParameters().Length == 0 && p.CanRead) yield return p;
            }
        }

        private static object GetMemberValue(MemberInfo member, object instance)
        {
            try { return member switch { FieldInfo f => f.GetValue(instance), PropertyInfo p => p.GetValue(instance), _ => null }; } catch { return null; }
        }

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == DeviceStatusMapOprtCellConst.CommonInfo_MapOprtCellID)
                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == DeviceStatusMapOprtCellConst.StatusName_MapOprtCellID)
                return ExecStringProperty(mapOprtCellInstInfo, nameof(DeviceStatusPropertyModelEdit.StatusName), v => viewModel.StatusName = v);

            if (mapOprtCellInstInfo.OprtCellID == DeviceStatusMapOprtCellConst.DeviceStatusValue_MapOprtCellID)
                return ExecStringProperty(mapOprtCellInstInfo, nameof(DeviceStatusPropertyModelEdit.DeviceStatusValue), v => viewModel.DeviceStatusValue = v);

            if (mapOprtCellInstInfo.OprtCellID == DeviceStatusMapOprtCellConst.DeviceStatusUnit_MapOprtCellID)
                return ExecStringProperty(mapOprtCellInstInfo, nameof(DeviceStatusPropertyModelEdit.DeviceStatusUnit), v => viewModel.DeviceStatusUnit = v);

            if (mapOprtCellInstInfo.OprtCellID == DeviceStatusMapOprtCellConst.CurrentIndex_MapOprtCellID)
                return ExecIntProperty(mapOprtCellInstInfo, nameof(DeviceStatusPropertyModelEdit.CurrentIndex), v => viewModel.CurrentIndex = v);

            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        protected override bool SetUIDataObjPropValues(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            foreach (GFBaseTypePropValue gFBaseTypePropValue in gFBaseTypePropValues)
            {
                if (string.Compare(gFBaseTypePropValue.PropertyID.ToString(), "AAA") == 0)
                {
                }
            }
            return true;
        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);
            if (string.Compare(propertyID, nameof(DeviceStatusPropertyModelEdit.CommonInfo)) == 0)
            {
                CallBack?.ExecOprt(nameof(DeviceStatusPropertyModelEdit.CommonInfo));
            }
            if (string.Compare(propertyID, nameof(DeviceStatusPropertyModelEdit.StatusName)) == 0)
            {
                CallBack?.ExecOprt(nameof(DeviceStatusPropertyModelEdit.StatusName));
            }
            if (string.Compare(propertyID, nameof(DeviceStatusPropertyModelEdit.DeviceStatusValue)) == 0)
            {
                CallBack?.ExecOprt(nameof(DeviceStatusPropertyModelEdit.DeviceStatusValue));
            }
            if (string.Compare(propertyID, nameof(DeviceStatusPropertyModelEdit.DeviceStatusUnit)) == 0)
            {
                CallBack?.ExecOprt(nameof(DeviceStatusPropertyModelEdit.DeviceStatusUnit));
            }
            if (string.Compare(propertyID, nameof(DeviceStatusPropertyModelEdit.CurrentIndex)) == 0)
            {
                CallBack?.ExecOprt(nameof(DeviceStatusPropertyModelEdit.CurrentIndex));
            }

            if (!DeviceStatusPropertyModelEdit.IsRuning)
            {
                CallBack?.UpdateUIDataObjPropValues(new GFBaseTypePropValueList());
            }
        }

        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null!;
        }

        private bool ExecStringProperty(MapOprtCellInstInfo info, string propName, Action<string> setAction)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new StringPropertyMapOprtCellExector(propName, setAction);
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }

        private bool ExecIntProperty(MapOprtCellInstInfo info, string propName, Action<int> setAction)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new IntPropertyMapOprtCellExector(propName, setAction);
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }

        private bool ExecuteOprtCell<T>(MapOprtCellInstInfo mapOprtCellInstInfo) where T : IMapOprtCellExector, new()
        {
            if (_oprtCellIdByInstanceId.TryGetValue(mapOprtCellInstInfo.InstanceID, out var oldOprtCellId) && oldOprtCellId != mapOprtCellInstInfo.OprtCellID)
                MapOprtCellExectorDict.TryRemove(mapOprtCellInstInfo.InstanceID, out _);

            if (!MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector) || mapOprtCellExector == null || mapOprtCellExector.GetType() != typeof(T))
            {
                mapOprtCellExector = new T();
                mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryRemove(mapOprtCellInstInfo.InstanceID, out _);
                MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
            }
            _oprtCellIdByInstanceId.AddOrUpdate(mapOprtCellInstInfo.InstanceID, mapOprtCellInstInfo.OprtCellID, (_, __) => mapOprtCellInstInfo.OprtCellID);
            mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
            return true;
        }

        private static void PostToUI(Action action) { if (Dispatcher.UIThread.CheckAccess()) action(); else Dispatcher.UIThread.Post(action); }

        // Executors
        private class CommonInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is DeviceStatusViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<CommonInfoCfg>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                PostToUI(() =>
                                {
                                    if (param.CurrentIndex.HasValue) vm.CurrentIndex = param.CurrentIndex.Value;
                                    if (param.StatusName != null) vm.StatusName = param.StatusName;
                                    if (param.DeviceStatusValue != null) vm.DeviceStatusValue = param.DeviceStatusValue;
                                    if (param.DeviceStatusUnit != null) vm.DeviceStatusUnit = param.DeviceStatusUnit;
                                });
                                return;
                            }
                        }
                        catch { }
                    }
                    var val = callBack.GetMapCellPropValue(nameof(DeviceStatusPropertyModelEdit.CommonInfo));
                    if (val != null)
                    {
                        var info = DeserializeObject<DeviceStatusCommonInfo>(val);
                        PostToUI(() =>
                        {
                            vm.ImageSources = info.ImageSources ?? new List<BitmapData>();
                            vm.CurrentIndex = info.CurrentIndex;
                            vm.StatusName = info.StatusName;
                            vm.DeviceStatusValue = info.DeviceStatusValue;
                            vm.DeviceStatusUnit = info.DeviceStatusUnit;
                        });
                    }
                }
            }
        }

        private class StringPropertyMapOprtCellExector : IMapOprtCellExector
        {
            private readonly string propName;
            private readonly Action<string> setAction;
            private IMapOprtCellExectorCallBack callBack;
            public StringPropertyMapOprtCellExector(string propName, Action<string> setAction) { this.propName = propName; this.setAction = setAction; }
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                GriffinsBaseValue val = callBack.GetMapCellPropValue(propName);
                if (val != null)
                {
                    string str = val.ToPrimitiveValue<string>();
                    PostToUI(() => setAction?.Invoke(str));
                }
            }
        }

        private class IntPropertyMapOprtCellExector : IMapOprtCellExector
        {
            private readonly string propName;
            private readonly Action<int> setAction;
            private IMapOprtCellExectorCallBack callBack;
            public IntPropertyMapOprtCellExector(string propName, Action<int> setAction) { this.propName = propName; this.setAction = setAction; }
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                GriffinsBaseValue val = callBack.GetMapCellPropValue(propName);
                if (val != null)
                {
                    try
                    {
                        int i = val.ToPrimitiveValue<int>();
                        PostToUI(() => setAction?.Invoke(i));
                    }
                    catch { }
                }
            }
        }

        private class CommonInfoCfg { public int? CurrentIndex { get; set; } public string StatusName { get; set; } public string DeviceStatusValue { get; set; } public string DeviceStatusUnit { get; set; } }
    }

    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("公共", 1)]
    public class DeviceStatusPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        public DeviceStatusPropertyModelEdit()
        {
            CommonInfo = new DeviceStatusCommonInfo();
        }

        private DeviceStatusCommonInfo _commonInfo;
        [DisplayName("公共")] [Category("公共")] [PropertySortOrder(1)]
        public DeviceStatusCommonInfo CommonInfo { get => _commonInfo; set { if (value == null) value = new DeviceStatusCommonInfo(); if (ReferenceEquals(_commonInfo, value)) return; _commonInfo = value; RaisePropertyChanged(nameof(CommonInfo)); } }

        private string _statusName;
        [Browsable(false)]
        public string StatusName { get => _statusName; set { if (_statusName == value) return; _statusName = value; RaisePropertyChanged(nameof(StatusName)); } }

        private string _deviceStatusValue;
        [Browsable(false)]
        public string DeviceStatusValue { get => _deviceStatusValue; set { if (_deviceStatusValue == value) return; _deviceStatusValue = value; RaisePropertyChanged(nameof(DeviceStatusValue)); } }

        private string _deviceStatusUnit;
        [Browsable(false)]
        public string DeviceStatusUnit { get => _deviceStatusUnit; set { if (_deviceStatusUnit == value) return; _deviceStatusUnit = value; RaisePropertyChanged(nameof(DeviceStatusUnit)); } }

        private int _currentIndex;
        [Browsable(false)]
        public int CurrentIndex { get => _currentIndex; set { if (_currentIndex == value) return; _currentIndex = value; RaisePropertyChanged(nameof(CurrentIndex)); } }

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(CommonInfo)) == 0) { 
                var src = propertyVal != null ? DeserializeObject<DeviceStatusCommonInfo>(propertyVal) : new DeviceStatusCommonInfo(); 
                _commonInfo ??= new DeviceStatusCommonInfo(); 
                _commonInfo.CopyFrom(src); 
                // Sync proxy properties
                StatusName = _commonInfo.StatusName;
                DeviceStatusValue = _commonInfo.DeviceStatusValue;
                DeviceStatusUnit = _commonInfo.DeviceStatusUnit;
                CurrentIndex = _commonInfo.CurrentIndex;
                RaisePropertyChanged(nameof(CommonInfo)); 
                return true; 
            }
            if (string.Compare(propertyID, nameof(StatusName)) == 0) { StatusName = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(DeviceStatusValue)) == 0) { DeviceStatusValue = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(DeviceStatusUnit)) == 0) { DeviceStatusUnit = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(CurrentIndex)) == 0) { CurrentIndex = propertyVal?.ToPrimitiveValue<int>() ?? 0; return true; }

            return base.SetPropertyValue(propertyID, propertyVal);
        }

        private static T DeserializeObject<T>(GriffinsBaseValue val) where T : IGriffinsBaseValue, new()
        {
            if (val == null) return default;
            try { ObjectValue_Bytes objectValue_Bytes = val.ToObjectValue_Bytes(); if (objectValue_Bytes != null) { GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Bytes); IGriffinsBaseValue obj = new T(); obj.PopulateFromBaseValue(griffinsBaseValue); return (T)obj; } } catch { }
            try { ObjectValue_Json objectValue_Json = val.ToObjectValue_Json(); if (objectValue_Json != null) { GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json); IGriffinsBaseValue obj = new T(); obj.PopulateFromBaseValue(griffinsBaseValue); return (T)obj; } } catch { }
            return default;
        }

        public void CopyFrom(DeviceStatusPropertyModelEdit source)
        {
            if (source == null) return;
            base.CopyFrom(source);
            _commonInfo ??= new DeviceStatusCommonInfo();
            if (source.CommonInfo != null) _commonInfo.CopyFrom(source.CommonInfo);
            StatusName = source.StatusName;
            DeviceStatusValue = source.DeviceStatusValue;
            DeviceStatusUnit = source.DeviceStatusUnit;
            CurrentIndex = source.CurrentIndex;
            RaisePropertyChanged(nameof(CommonInfo));
        }
    }

    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("绑定信息", 1)]
    public class DeviceStatusPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        private string _statusName = "StatusName";
        [DisplayName("状态名称")] [Category("绑定信息")] [PropertySortOrder(1)] [BindMPPropertyID]
        public string StatusName { get => _statusName; set => SetProperty(ref _statusName, value); }

        private string _deviceStatusValue = "DeviceStatusValue";
        [DisplayName("设备状态")] [Category("绑定信息")] [PropertySortOrder(2)] [BindMPPropertyID]
        public string DeviceStatusValue { get => _deviceStatusValue; set => SetProperty(ref _deviceStatusValue, value); }

        private string _deviceStatusUnit = "DeviceStatusUnit";
        [DisplayName("单位")] [Category("绑定信息")] [PropertySortOrder(3)] [BindMPPropertyID]
        public string DeviceStatusUnit { get => _deviceStatusUnit; set => SetProperty(ref _deviceStatusUnit, value); }

        private string _currentIndex = "CurrentIndex";
        [DisplayName("当前索引")] [Category("绑定信息")] [PropertySortOrder(4)] [BindMPPropertyID]
        public string CurrentIndex { get => _currentIndex; set => SetProperty(ref _currentIndex, value); }

        public void CopyFrom(DeviceStatusPropertyBindEditModel source)
        {
            base.CopyFrom(source);
            StatusName = source.StatusName;
            DeviceStatusValue = source.DeviceStatusValue;
            DeviceStatusUnit = source.DeviceStatusUnit;
            CurrentIndex = source.CurrentIndex;
        }
    }
}