using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Avalonia.Threading;
using GF_Gereric;
using GKG.Map.MapCell.Generic;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Link
{
    class MapCellLinkCtlObj : ControlCellBase
    {
        private LinkView view;
        private LinkViewModel linkViewModel;
        private EventBindEditModel _eventBindEditModel;
        private readonly bool _designTime;
        private bool _loadedPropertyEditFromBytes;

        public MapCellLinkCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        public MapCellLinkCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            _designTime = designTime;
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            SetID(mapCellID);
            SetName(mapCellName);

            view = new LinkView();

            RegisterProperty(new MapObjPropertyInfo(nameof(LinkPropertyModelEdit.BrushInfo), "画笔设置", MapCellPropDataType.Object_Json, LinkBrushInfo.Object_ID, typeof(LinkBrushInfo), false, true, new MapCellPropValue(LinkBrushInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(LinkPropertyModelEdit.CommonInfo), "公共设置", MapCellPropDataType.Object_Json, LinkCommonInfo.Object_ID, typeof(LinkCommonInfo), false, true, new MapCellPropValue(LinkCommonInfo.Default)));

            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), MapCellPropDataType.Object_Bytes, GraphMouseEventParam.Object_ID));

            RegisterOprtCellInfo(new MapOprtCellInfo(LinkMapOprtCellConst.BrushInfo_MapOprtCellID, "链接画笔设置操作原子", typeof(MapOprtCellParamCfgView.BrushInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(LinkMapOprtCellConst.CommonInfo_MapOprtCellID, "链接公共设置操作原子", typeof(MapOprtCellParamCfgView.CommonInfoMapOprtCellParamCfgView)));

            RegisterOprtInfo(new MapOprtInfo(nameof(LinkPropertyModelEdit.BrushInfo), "设置画笔", OprtExecKind.Normal, "", new MapOprtCellInstInfoList { new MapOprtCellInstInfo { InstanceID = Guid.NewGuid(), OprtCellID = LinkMapOprtCellConst.BrushInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(LinkPropertyModelEdit.CommonInfo), "设置公共", OprtExecKind.Normal, "", new MapOprtCellInstInfoList { new MapOprtCellInstInfo { InstanceID = Guid.NewGuid(), OprtCellID = LinkMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));

            (this as IMapCellTypeBase).Name = GetLinkName();

            linkViewModel = new LinkViewModel(LinkPropertyModelEdit, _ => HandleClick());
            view.DataContext = linkViewModel;

            LinkPropertyModelEdit.BrushInfo.PropertyChanged += OnBrushInfoPropertyChanged;
            LinkPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
        }

        private void OnBrushInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(LinkPropertyModelEdit.BrushInfo));
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(LinkPropertyModelEdit.CommonInfo));
        }

        public LinkPropertyModelEdit LinkPropertyModelEdit => PropertyEditModelBase as LinkPropertyModelEdit;

        public override void OnDispose()
        {
            LinkPropertyModelEdit.BrushInfo.PropertyChanged -= OnBrushInfoPropertyChanged;
            LinkPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;

            view.DataContext = null;
            linkViewModel?.Dispose();
            linkViewModel = null;

            base.OnDispose();
        }

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new LinkPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new LinkPropertyBindEditModel();

        public override EventBindEditModel CreateEventBindEditModel()
        {
            if (_eventBindEditModel == null)
            {
                _eventBindEditModel = new EventBindEditModel
                {
                    EventCmdInfos = new BindingList<EventCmdInfo>
                    {
                        new EventCmdInfo
                        {
                            EventCmdKind = EventCmdKind.MpCmdKind,
                            EventID = MapObjPropEventConst.Event_MouseClick
                        }
                    }
                };
            }

            return _eventBindEditModel;
        }

        protected override bool SetPropertyValue(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            if (gFBaseTypePropValues == null)
                return false;

            foreach (var item in gFBaseTypePropValues)
            {
                var propertyId = item?.PropertyID.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(propertyId))
                    continue;

                SetPropertyValue(propertyId, item.Value, true);
            }

            return true;
        }

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal, bool isRuning)
        {
            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoadedLink(propertyID, propertyVal))
                return true;

            LinkPropertyModelEdit.IsRuning = isRuning;
            var ok = LinkPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
            if (ok)
                ExecuteOprtByPropertyId(propertyID);

            return ok;
        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            if (TryGetPrimaryOprtCellId(propertyID, out var normalizedOprtId, out _))
            {
                try
                {
                    // 让测试工具按归并后的分组操作ID输出“执行操作”日志。
                    CallBack?.ExecOprt(normalizedOprtId);
                }
                catch
                {
                }
            }

            if (LinkPropertyModelEdit.IsRuning || CallBack == null || string.IsNullOrWhiteSpace(propertyID) || propertyValue == null)
                return;

            try
            {
                CallBack.UpdatePropertyValue(new GFBaseTypePropValueList
                {
                    new GFBaseTypePropValue(MPPropertyID.Parse(propertyID), propertyValue)
                });
            }
            catch
            {
            }
        }

        private void HandleClick()
        {
            TryOpenAddress();
            ExecEvent(MapObjPropEventConst.Event_MouseClick);
        }

        private void TryOpenAddress()
        {
            if (_designTime)
                return;

            var address = LinkPropertyModelEdit?.CommonInfo?.Address?.Trim();
            if (string.IsNullOrWhiteSpace(address))
                return;

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = address,
                    UseShellExecute = true
                });
            }
            catch
            {
            }
        }

        private void ExecEvent(string eventId)
        {
            var eventCmdInfo = EventBindEditModel.EventCmdInfos.FirstOrDefault(info => info.EventID == eventId);
            if (eventCmdInfo != null)
            {
                CallBack?.ExecMapCellEvent(eventCmdInfo.EventCmdKind, eventCmdInfo.CmdID, CommHelper.ToEventParamValueList(eventCmdInfo.CmdParam), out _);
            }
        }

        private void ExecuteOprtByPropertyId(string propertyID)
        {
            if (string.IsNullOrWhiteSpace(propertyID))
                return;

            if (!TryGetPrimaryOprtCellId(propertyID, out var normalizedOprtId, out _))
                return;

            TryExecuteOprtInfoById(normalizedOprtId);
        }

        private static bool TryGetPrimaryOprtCellId(string oprtId, out string normalizedOprtId, out MapOprtCellID oprtCellId)
        {
            if (string.IsNullOrWhiteSpace(oprtId))
            {
                normalizedOprtId = oprtId ?? string.Empty;
                oprtCellId = default;
                return false;
            }

            var dot = oprtId.IndexOf('.');
            if (dot > 0)
                oprtId = oprtId.Substring(0, dot);

            normalizedOprtId = oprtId;
            oprtCellId = default;

            // 链接图元当前绑定字段都是叶子字段，这里统一收口到现有 CommonInfo 原子。
            if (string.Equals(oprtId, nameof(LinkCommonInfo.LinkText), StringComparison.Ordinal)
                || string.Equals(oprtId, nameof(LinkCommonInfo.Address), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(LinkPropertyModelEdit.CommonInfo);
                oprtCellId = LinkMapOprtCellConst.CommonInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(LinkPropertyModelEdit.BrushInfo), StringComparison.Ordinal))
            {
                oprtCellId = LinkMapOprtCellConst.BrushInfo_MapOprtCellID;
                return true;
            }

            if (string.Equals(oprtId, nameof(LinkPropertyModelEdit.CommonInfo), StringComparison.Ordinal))
            {
                oprtCellId = LinkMapOprtCellConst.CommonInfo_MapOprtCellID;
                return true;
            }

            return false;
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
                if (val == null)
                    continue;

                if (val is IDictionary dict)
                {
                    foreach (DictionaryEntry entry in dict)
                    {
                        if (entry.Value is MapOprtInfo)
                            yield return entry.Value;
                    }
                    continue;
                }

                if (val is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is MapOprtInfo)
                            yield return item;
                    }
                }
            }
        }

        private static string? GetOprtInfoId(object oprtInfo)
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

        private static IEnumerable? GetOprtInfoInstList(object oprtInfo)
        {
            try
            {
                return EnumerateOprtInfoInsts(oprtInfo).Select(x => x.inst).ToList();
            }
            catch
            {
            }

            return null;
        }

        private static IEnumerable<(MapOprtCellInstInfo inst, string source)> EnumerateOprtInfoInsts(object oprtInfo)
        {
            var t = oprtInfo.GetType();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var seen = new HashSet<string>(StringComparer.Ordinal);

            static bool TryKey(MapOprtCellInstInfo inst, string source, int idx, out string key)
            {
                if (inst.InstanceID != Guid.Empty)
                {
                    key = inst.InstanceID.ToString();
                    return true;
                }

                key = $"{source}:{idx}:{inst.OprtCellID}";
                return true;
            }

            IEnumerable<(MapOprtCellInstInfo inst, string source)> EnumerateFromValue(object? val, string source)
            {
                if (val is not IEnumerable e)
                    yield break;

                int idx = 0;
                foreach (var item in e)
                {
                    if (item is not MapOprtCellInstInfo inst)
                        continue;

                    idx++;
                    if (!TryKey(inst, source, idx, out var key) || !seen.Add(key))
                        continue;

                    yield return (inst, source);
                }
            }

            foreach (var p in t.GetProperties(flags))
            {
                if (p.GetIndexParameters().Length != 0 || !p.CanRead)
                    continue;

                if (typeof(IEnumerable).IsAssignableFrom(p.PropertyType))
                {
                    foreach (var x in EnumerateFromValue(p.GetValue(oprtInfo), $"prop:{p.Name}"))
                        yield return x;
                }
            }

            foreach (var f in t.GetFields(flags))
            {
                if (f.IsStatic)
                    continue;

                if (typeof(IEnumerable).IsAssignableFrom(f.FieldType))
                {
                    foreach (var x in EnumerateFromValue(f.GetValue(oprtInfo), $"field:{f.Name}"))
                        yield return x;
                }
            }
        }

        private static IEnumerable<MemberInfo> EnumerateInstanceMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            for (var t = type; t != null; t = t.BaseType)
            {
                foreach (var f in t.GetFields(flags))
                {
                    if (!f.IsStatic)
                        yield return f;
                }

                foreach (var p in t.GetProperties(flags))
                {
                    if (p.GetIndexParameters().Length == 0 && p.CanRead)
                        yield return p;
                }
            }
        }

        private static object? GetMemberValue(MemberInfo member, object instance)
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

        private bool IsDefaultOverwriteForLoadedLink(string propertyID, MapCellPropValue propertyVal)
        {
            try
            {
                if (string.Equals(propertyID, nameof(LinkPropertyModelEdit.BrushInfo), StringComparison.Ordinal))
                {
                    var incoming = propertyVal != null ? DeserializeObject<LinkBrushInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(LinkPropertyModelEdit.BrushInfo);
                }

                if (string.Equals(propertyID, nameof(LinkPropertyModelEdit.CommonInfo), StringComparison.Ordinal))
                {
                    var incoming = propertyVal != null ? DeserializeObject<LinkCommonInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(LinkPropertyModelEdit.CommonInfo);
                }
            }
            catch
            {
            }

            return false;
        }

        private static bool IsDefault(LinkBrushInfo? info)
        {
            if (info == null)
                return true;

            return info.TextColor == LinkBrushInfo.Default.TextColor
                && info.HoverTextColor == LinkBrushInfo.Default.HoverTextColor;
        }

        private static bool IsDefault(LinkCommonInfo? info)
        {
            if (info == null)
                return true;

            return string.Equals(info.LinkText ?? string.Empty, LinkCommonInfo.Default.LinkText, StringComparison.Ordinal)
                && string.Equals(info.Address ?? string.Empty, LinkCommonInfo.Default.Address, StringComparison.Ordinal);
        }

        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => linkViewModel;

        protected override void OnCopyFrom(ControlCellBase source)
        {
            base._CopyFrom(source as MapCellLinkCtlObj);
            (PropertyEditModelBase as LinkPropertyModelEdit)?.CopyFrom(source.PropertyEditModelBase as LinkPropertyModelEdit);
            if (PropertyBindEditModelBase is LinkPropertyBindEditModel selfBind && source.PropertyBindEditModelBase is LinkPropertyBindEditModel srcBind)
                selfBind.CopyFrom(srcBind);
            else
                PropertyBindEditModelBase.CopyFrom(source.PropertyBindEditModelBase);

            EventBindEditModel.CopyFrom(source.EventBindEditModel);
            _loadedPropertyEditFromBytes = true;
            Dispatcher.UIThread.Post(() => linkViewModel?.ReloadFromModel());
        }

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);

            string propertyEditJson = br.ReadString("PropertyEditModelBase");
            if (!string.IsNullOrEmpty(propertyEditJson))
            {
                try
                {
                    var propertyEditModelBase = JsonObjConvert.FromJSon<LinkPropertyModelEdit>(propertyEditJson);
                    if (propertyEditModelBase != null)
                    {
                        (PropertyEditModelBase as LinkPropertyModelEdit)?.CopyFrom(propertyEditModelBase);
                        _loadedPropertyEditFromBytes = true;
                    }
                }
                catch
                {
                }
            }

            string propertyBindJson = br.ReadString("PropertyBindEditModelBase");
            if (!string.IsNullOrEmpty(propertyBindJson))
            {
                try
                {
                    var propertyBindEditModelBase = JsonObjConvert.FromJSon<LinkPropertyBindEditModel>(propertyBindJson);
                    if (propertyBindEditModelBase != null)
                    {
                        (PropertyBindEditModelBase as LinkPropertyBindEditModel)?.CopyFrom(propertyBindEditModelBase);
                    }
                }
                catch
                {
                }
            }

            string eventBindJson = br.ReadString("EventBindEditModel");
            if (!string.IsNullOrEmpty(eventBindJson))
            {
                try
                {
                    var eventBindEditModel = JsonSerializer.Deserialize<EventBindEditModel>(eventBindJson);
                    if (eventBindEditModel != null)
                    {
                        EventBindEditModel.CopyFrom(eventBindEditModel);
                    }
                }
                catch
                {
                }
            }

            if (_loadedPropertyEditFromBytes)
                Dispatcher.UIThread.Post(() => linkViewModel?.ReloadFromModel());
        }

        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", JsonSerializer.Serialize(EventBindEditModel));
        }

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == LinkMapOprtCellConst.BrushInfo_MapOprtCellID)
                return ExecuteOprtCell<BrushInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == LinkMapOprtCellConst.CommonInfo_MapOprtCellID)
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

        private static void PostToUI(Action action)
        {
            if (Dispatcher.UIThread.CheckAccess())
                action();
            else
                Dispatcher.UIThread.Post(action);
        }

        private class BrushInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is not LinkViewModel vm)
                    return;

                if (cfg != null && cfg.Length > 0)
                {
                    try
                    {
                        var param = JsonSerializer.Deserialize<MapOprtCellParamCfgView.BrushInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                        if (param != null)
                        {
                            PostToUI(() =>
                            {
                                if (!string.IsNullOrWhiteSpace(param.TextColor))
                                    vm.BrushInfo.TextColor = Avalonia.Media.Color.Parse(param.TextColor);
                                if (!string.IsNullOrWhiteSpace(param.HoverTextColor))
                                    vm.BrushInfo.HoverTextColor = Avalonia.Media.Color.Parse(param.HoverTextColor);
                            });
                            return;
                        }
                    }
                    catch
                    {
                    }
                }

                var val = callBack.GetMapCellPropValue(nameof(LinkPropertyModelEdit.BrushInfo));
                if (val == null)
                    return;

                try
                {
                    var brushInfo = DeserializeObject<LinkBrushInfo>(val);
                    PostToUI(() =>
                    {
                        vm.BrushInfo.TextColor = brushInfo.TextColor;
                        vm.BrushInfo.HoverTextColor = brushInfo.HoverTextColor;
                    });
                }
                catch
                {
                }
            }
        }

        private class CommonInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is not LinkViewModel vm)
                    return;

                if (cfg != null && cfg.Length > 0)
                {
                    try
                    {
                        var param = JsonSerializer.Deserialize<MapOprtCellParamCfgView.CommonInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                        if (param != null)
                        {
                            PostToUI(() =>
                            {
                                vm.CommonInfo.LinkText = param.LinkText ?? string.Empty;
                                vm.CommonInfo.Address = param.Address ?? string.Empty;
                            });
                            return;
                        }
                    }
                    catch
                    {
                    }
                }

                var val = callBack.GetMapCellPropValue(nameof(LinkPropertyModelEdit.CommonInfo));
                if (val == null)
                    return;

                try
                {
                    var commonInfo = DeserializeObject<LinkCommonInfo>(val);
                    PostToUI(() =>
                    {
                        vm.CommonInfo.LinkText = commonInfo.LinkText;
                        vm.CommonInfo.Address = commonInfo.Address;
                    });
                }
                catch
                {
                }
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    ExecuteOprtByPropertyId(nameof(LinkPropertyModelEdit.BrushInfo));
                    ExecuteOprtByPropertyId(nameof(LinkPropertyModelEdit.CommonInfo));
                    linkViewModel?.ReloadFromModel();
                }
                catch
                {
                }
            });
        }

        public override void OnZoomChanged()
        {
        }

        public override string ToString() => GetLinkName();

        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }

        private static string GetLinkName()
        {
            return ResourceA.ResourceManager.GetString("Link", ResourceA.Culture) ?? "链接";
        }

        public override MapCellPropValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null;
        }
    }

    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class LinkPropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _linkText = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _address = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("链接文本")]
        [Category("绑定信息")]
        [PropertySortOrder(1)]
        [BindMPPropertyID]
        public PropertyBindInfo LinkText
        {
            get => _linkText;
            set => SetProperty(ref _linkText, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        [DisplayName("地址")]
        [Category("绑定信息")]
        [PropertySortOrder(2)]
        [BindMPPropertyID]
        public PropertyBindInfo Address
        {
            get => _address;
            set => SetProperty(ref _address, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        public void CopyFrom(LinkPropertyBindEditModel source)
        {
            if (source == null)
                return;

            base.CopyFrom(source);
            LinkText = source.LinkText;
            Address = source.Address;
        }
    }
}
