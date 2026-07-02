using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.GroupPanel;
using GKG.Map.MapCell.Generic.Control.Lable;
using GKG.Map.MapCell.Generic.ProgressBar.MapOprtCellParamCfgView;
using GKG.Map.MapCell.Generic.ProgressBar.View;
using GKG.Map.MapCell.Generic.ProgressBar.ViewModel;
using GKG.Map.MapCell.Generic;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.ProgressBar
{
    class MapCellProgressBarCtlObj : ControlCellBase
    {
        private ProgressBarView view;
        private ProgressBarViewModel progressBarViewModel;
        private readonly ConcurrentDictionary<Guid, MapOprtCellID> _oprtCellIdByInstanceId = new();
        private MapObjID _mapCellID;
        private string _mapCellName;
        private bool _loadedPropertyEditFromBytes;


        public MapCellProgressBarCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false) { }

        public MapCellProgressBarCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            _mapCellID = mapCellID;
            _mapCellName = mapCellName;
            base.SetID(mapCellID);
            base.SetName(mapCellName);

            view = new ProgressBarView();

            RegisterProperty(new MapObjPropertyInfo(nameof(ProgressBarPropertyModelEdit.BrushInfo), "画笔设置", MapCellPropDataType.Object_Json, ProgressBarBrushInfo.Object_ID, typeof(ProgressBarBrushInfo), false, true, new MapCellPropValue(ProgressBarBrushInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(ProgressBarPropertyModelEdit.AppearanceInfo), "外观设置", MapCellPropDataType.Object_Json, ProgressBarAppearanceInfo.Object_ID, typeof(ProgressBarAppearanceInfo), false, true, new MapCellPropValue(ProgressBarAppearanceInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(ProgressBarPropertyModelEdit.LayoutInfo), "布局设置", MapCellPropDataType.Object_Json, ProgressBarLayoutInfo.Object_ID, typeof(ProgressBarLayoutInfo), false, true, new MapCellPropValue(ProgressBarLayoutInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(ProgressBarPropertyModelEdit.CommonInfo), "公共设置", MapCellPropDataType.Object_Json, ProgressBarCommonInfo.Object_ID, typeof(ProgressBarCommonInfo), false, true, new MapCellPropValue(ProgressBarCommonInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(ProgressBarPropertyModelEdit.TextInfo), "文本设置", MapCellPropDataType.Object_Json, ProgressBarTextInfo.Object_ID, typeof(ProgressBarTextInfo), false, true, new MapCellPropValue(ProgressBarTextInfo.Default)));

            RegisterOprtCellInfo(new MapOprtCellInfo(ProgressBarMapOprtCellConst.BrushInfo_MapOprtCellID, "画笔设置操作原子", typeof(BrushInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(ProgressBarMapOprtCellConst.AppearanceInfo_MapOprtCellID, "外观设置操作原子", typeof(AppearanceInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(ProgressBarMapOprtCellConst.LayoutInfo_MapOprtCellID, "布局设置操作原子", typeof(LayoutInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(ProgressBarMapOprtCellConst.CommonInfo_MapOprtCellID, "公共设置操作原子", typeof(CommonInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(ProgressBarMapOprtCellConst.TextInfo_MapOprtCellID, "文本设置操作原子", typeof(TextInfoMapOprtCellParamCfgView)));

            RegisterOprtInfo(new MapOprtInfo(nameof(ProgressBarPropertyModelEdit.BrushInfo), "设置画笔", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = ProgressBarMapOprtCellConst.BrushInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(ProgressBarPropertyModelEdit.AppearanceInfo), "设置外观", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = ProgressBarMapOprtCellConst.AppearanceInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(ProgressBarPropertyModelEdit.LayoutInfo), "设置布局", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = ProgressBarMapOprtCellConst.LayoutInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(ProgressBarPropertyModelEdit.CommonInfo), "设置公共", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = ProgressBarMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(ProgressBarPropertyModelEdit.TextInfo), "设置文本", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = ProgressBarMapOprtCellConst.TextInfo_MapOprtCellID, CfgInfo = null } }));

            (this as IMapCellTypeBase).Name = ResourceA.ProgressBar;

            progressBarViewModel = new ProgressBarViewModel(designTime, ProgressBarPropertyModelEdit, OnEventTriggered);
            view.DataContext = progressBarViewModel;

            ProgressBarPropertyModelEdit.BrushInfo.PropertyChanged += OnBrushInfoPropertyChanged;
            ProgressBarPropertyModelEdit.AppearanceInfo.PropertyChanged += OnAppearanceInfoPropertyChanged;
            ProgressBarPropertyModelEdit.LayoutInfo.PropertyChanged += OnLayoutInfoPropertyChanged;
            ProgressBarPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
            ProgressBarPropertyModelEdit.TextInfo.PropertyChanged += OnTextInfoPropertyChanged;


        }

        private void OnBrushInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(ProgressBarPropertyModelEdit.BrushInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnAppearanceInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(ProgressBarPropertyModelEdit.AppearanceInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnLayoutInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(ProgressBarPropertyModelEdit.LayoutInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(ProgressBarPropertyModelEdit.CommonInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnTextInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(ProgressBarPropertyModelEdit.TextInfo), "PropertyChanged", e?.PropertyName);
        }

        public override void OnDispose()
        {
            ProgressBarPropertyModelEdit.BrushInfo.PropertyChanged -= OnBrushInfoPropertyChanged;
            ProgressBarPropertyModelEdit.AppearanceInfo.PropertyChanged -= OnAppearanceInfoPropertyChanged;
            ProgressBarPropertyModelEdit.LayoutInfo.PropertyChanged -= OnLayoutInfoPropertyChanged;
            ProgressBarPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;
            ProgressBarPropertyModelEdit.TextInfo.PropertyChanged -= OnTextInfoPropertyChanged;

            view.DataContext = null;
            progressBarViewModel?.Dispose();
            progressBarViewModel = null;

            base.OnDispose();
        }

        public ProgressBarPropertyModelEdit ProgressBarPropertyModelEdit => PropertyEditModelBase as ProgressBarPropertyModelEdit;

        /// <summary>
        /// 事件触发时执行绑定的命令
        /// </summary>
        private void OnEventTriggered(string eventId)
        {
            EventCmdInfo? eventCmdInfo = EventBindEditModel.EventCmdInfos.FirstOrDefault(info => info.EventID == eventId);
            if (eventCmdInfo != null)
            {
                CallBack?.ExecMapCellEvent(eventCmdInfo.EventCmdKind, eventCmdInfo.CmdID, CommHelper.ToEventParamValueList(eventCmdInfo.CmdParam), out _);
            }
        }

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
            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoadedProgressBar(propertyID, propertyVal))
            {
                return true;
            }

            ProgressBarPropertyModelEdit.IsRuning = isRuning;
            var ok = ProgressBarPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
            if (ok)
            {
                ExecuteOprtByPropertyId(propertyID, "SetPropertyValue", null);
            }
return ok;
        }
        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            if (TryGetPrimaryOprtCellId(propertyID, out var normalizedOprtId, out _))
            {
                try
                {
                    CallBack?.ExecOprt(normalizedOprtId);
                }
                catch
                {
                }
            }
            if (CallBack == null || string.IsNullOrWhiteSpace(propertyID) || propertyValue == null)
                return;

            try
            {
                CallBack.UpdatePropertyValue(new GFBaseTypePropValueList()
                {
                    new GFBaseTypePropValue(MPPropertyID.Parse(propertyID), propertyValue)
                });
            }
            catch
            {
            }
        }

        private bool IsDefaultOverwriteForLoadedProgressBar(string propertyID, MapCellPropValue propertyVal)
        {
            try
            {
                if (string.Compare(propertyID, nameof(ProgressBarPropertyModelEdit.BrushInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<ProgressBarBrushInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(ProgressBarPropertyModelEdit.BrushInfo);
                }
                if (string.Compare(propertyID, nameof(ProgressBarPropertyModelEdit.AppearanceInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<ProgressBarAppearanceInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(ProgressBarPropertyModelEdit.AppearanceInfo);
                }
                if (string.Compare(propertyID, nameof(ProgressBarPropertyModelEdit.LayoutInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<ProgressBarLayoutInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(ProgressBarPropertyModelEdit.LayoutInfo);
                }
                if (string.Compare(propertyID, nameof(ProgressBarPropertyModelEdit.CommonInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<ProgressBarCommonInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(ProgressBarPropertyModelEdit.CommonInfo);
                }
                if (string.Compare(propertyID, nameof(ProgressBarPropertyModelEdit.TextInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<ProgressBarTextInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(ProgressBarPropertyModelEdit.TextInfo);
                }
                return false;
            }
            catch { return false; }
        }

        private static bool IsDefault(ProgressBarBrushInfo? info)
        {
            if (info == null) return true;
            return string.Equals(info.BackColorStr ?? "", ProgressBarBrushInfo.Default.BackColorStr, StringComparison.OrdinalIgnoreCase)
                && string.Equals(info.BorderColorStr ?? "", ProgressBarBrushInfo.Default.BorderColorStr, StringComparison.OrdinalIgnoreCase)
                && string.Equals(info.ForeColorStr ?? "", ProgressBarBrushInfo.Default.ForeColorStr, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsDefault(ProgressBarAppearanceInfo? info)
        {
            if (info == null) return true;
            return Math.Abs(info.Opacity - ProgressBarAppearanceInfo.Default.Opacity) < 0.000001
                && Math.Abs(info.BorderThicknessLeft - ProgressBarAppearanceInfo.Default.BorderThicknessLeft) < 0.000001
                && Math.Abs(info.BorderThicknessTop - ProgressBarAppearanceInfo.Default.BorderThicknessTop) < 0.000001
                && Math.Abs(info.BorderThicknessRight - ProgressBarAppearanceInfo.Default.BorderThicknessRight) < 0.000001
                && Math.Abs(info.BorderThicknessBottom - ProgressBarAppearanceInfo.Default.BorderThicknessBottom) < 0.000001;
        }

        private static bool IsDefault(ProgressBarLayoutInfo? info)
        {
            if (info == null) return true;
            return info.HorizontalAlign == ProgressBarLayoutInfo.Default.HorizontalAlign
                && info.VerticalAlign == ProgressBarLayoutInfo.Default.VerticalAlign
                && Math.Abs(info.MarginTop - ProgressBarLayoutInfo.Default.MarginTop) < 0.000001
                && Math.Abs(info.MarginLeft - ProgressBarLayoutInfo.Default.MarginLeft) < 0.000001
                && Math.Abs(info.MarginBottom - ProgressBarLayoutInfo.Default.MarginBottom) < 0.000001
                && Math.Abs(info.MarginRight - ProgressBarLayoutInfo.Default.MarginRight) < 0.000001
                && Math.Abs(info.MinWidth - ProgressBarLayoutInfo.Default.MinWidth) < 0.000001
                && Math.Abs(info.MaxWidth - ProgressBarLayoutInfo.Default.MaxWidth) < 0.000001
                && Math.Abs(info.MinHeight - ProgressBarLayoutInfo.Default.MinHeight) < 0.000001
                && Math.Abs(info.MaxHeight - ProgressBarLayoutInfo.Default.MaxHeight) < 0.000001;
        }

        private static bool IsDefault(ProgressBarCommonInfo? info)
        {
            if (info == null) return true;
            return info.Orientation == ProgressBarCommonInfo.Default.Orientation
                && Math.Abs(info.Value - ProgressBarCommonInfo.Default.Value) < 0.000001
                && Math.Abs(info.Minimum - ProgressBarCommonInfo.Default.Minimum) < 0.000001
                && Math.Abs(info.Maximum - ProgressBarCommonInfo.Default.Maximum) < 0.000001
                && info.IsIndeterminate == ProgressBarCommonInfo.Default.IsIndeterminate
                && info.IsEnabled == ProgressBarCommonInfo.Default.IsEnabled
                && string.Equals(info.ToolTip ?? "", ProgressBarCommonInfo.Default.ToolTip ?? "", StringComparison.Ordinal);
        }

        private static bool IsDefault(ProgressBarTextInfo? info)
        {
            if (info == null) return true;
            return info.FontFamilyType == ProgressBarTextInfo.Default.FontFamilyType
                && string.Equals(info.FontColorStr ?? "", ProgressBarTextInfo.Default.FontColorStr ?? "", StringComparison.OrdinalIgnoreCase)
                && Math.Abs(info.FontSize - ProgressBarTextInfo.Default.FontSize) < 0.000001
                && info.IsItalic == ProgressBarTextInfo.Default.IsItalic
                && info.IsBold == ProgressBarTextInfo.Default.IsBold;
        }

        private void ExecuteOprtByPropertyId(string propertyID, string trigger, string? changedProp)
        {
            if (string.IsNullOrWhiteSpace(propertyID)) return;

            var normalized = propertyID;
            var dot = normalized.IndexOf('.');
            if (dot > 0) normalized = normalized.Substring(0, dot);

            if (!TryGetPrimaryOprtCellId(normalized, out var normalizedOprtId, out var primaryOprtCellId)) return;

            TryExecuteOprtInfoById(normalizedOprtId, primaryOprtCellId);
        }

        private static bool TryGetPrimaryOprtCellId(string oprtId, out string normalizedOprtId, out MapOprtCellID oprtCellId)
        {
            normalizedOprtId = oprtId ?? string.Empty;
            oprtCellId = default;
            if (string.IsNullOrWhiteSpace(oprtId)) return false;

            if (string.Equals(oprtId, nameof(ProgressBarBrushInfo.BackColor), StringComparison.Ordinal)
                || string.Equals(oprtId, nameof(ProgressBarBrushInfo.BorderColor), StringComparison.Ordinal)
                || string.Equals(oprtId, nameof(ProgressBarBrushInfo.ForeColor), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(ProgressBarPropertyModelEdit.BrushInfo);
                oprtCellId = ProgressBarMapOprtCellConst.BrushInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(ProgressBarCommonInfo.Value), StringComparison.Ordinal)
                || string.Equals(oprtId, nameof(ProgressBarCommonInfo.Minimum), StringComparison.Ordinal)
                || string.Equals(oprtId, nameof(ProgressBarCommonInfo.Maximum), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(ProgressBarPropertyModelEdit.CommonInfo);
                oprtCellId = ProgressBarMapOprtCellConst.CommonInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(ProgressBarPropertyModelEdit.BrushInfo), StringComparison.Ordinal))
            { normalizedOprtId = nameof(ProgressBarPropertyModelEdit.BrushInfo); oprtCellId = ProgressBarMapOprtCellConst.BrushInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(ProgressBarPropertyModelEdit.AppearanceInfo), StringComparison.Ordinal))
            { normalizedOprtId = nameof(ProgressBarPropertyModelEdit.AppearanceInfo); oprtCellId = ProgressBarMapOprtCellConst.AppearanceInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(ProgressBarPropertyModelEdit.LayoutInfo), StringComparison.Ordinal))
            { normalizedOprtId = nameof(ProgressBarPropertyModelEdit.LayoutInfo); oprtCellId = ProgressBarMapOprtCellConst.LayoutInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(ProgressBarPropertyModelEdit.CommonInfo), StringComparison.Ordinal))
            { normalizedOprtId = nameof(ProgressBarPropertyModelEdit.CommonInfo); oprtCellId = ProgressBarMapOprtCellConst.CommonInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(ProgressBarPropertyModelEdit.TextInfo), StringComparison.Ordinal))
            { normalizedOprtId = nameof(ProgressBarPropertyModelEdit.TextInfo); oprtCellId = ProgressBarMapOprtCellConst.TextInfo_MapOprtCellID; return true; }
            return false;
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
                        if (Dispatcher.UIThread.CheckAccess())
                            ExecOprtCell(inst);
                        else
                            Dispatcher.UIThread.Post(() => ExecOprtCell(inst));
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
                if (val is IDictionary dict)
                {
                    foreach (DictionaryEntry entry in dict)
                        if (entry.Value is MapOprtInfo) yield return entry.Value;
                    continue;
                }
                if (val is IEnumerable enumerable)
                    foreach (var item in enumerable)
                        if (item is MapOprtInfo) yield return item;
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
                    if (p == null) continue;
                    var v = p.GetValue(oprtInfo);
                    if (v is string s && !string.IsNullOrWhiteSpace(s)) return s;
                }
            }
            catch { }
            return null;
        }

        private static IEnumerable? GetOprtInfoInstList(object oprtInfo)
        {
            try { return EnumerateOprtInfoInsts(oprtInfo).Select(x => x.inst).ToList(); }
            catch { }
            return null;
        }

        private static IEnumerable<(MapOprtCellInstInfo inst, string source)> EnumerateOprtInfoInsts(object oprtInfo)
        {
            var t = oprtInfo.GetType();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var seen = new HashSet<string>(StringComparer.Ordinal);

            IEnumerable<(MapOprtCellInstInfo inst, string source)> EnumerateFromValue(object? val, string source)
            {
                if (val == null) yield break;
                if (val is IEnumerable e)
                {
                    int idx = 0;
                    foreach (var item in e)
                    {
                        if (item is not MapOprtCellInstInfo inst) continue;
                        idx++;
                        var key = inst.InstanceID != Guid.Empty ? inst.InstanceID.ToString() : $"{source}:{idx}:{inst.OprtCellID}";
                        if (!seen.Add(key)) continue;
                        yield return (inst, source);
                    }
                }
            }

            foreach (var p in t.GetProperties(flags))
            {
                if (p.GetIndexParameters().Length != 0 || !p.CanRead) continue;
                var pt = p.PropertyType;
                if (pt == typeof(MapOprtCellInstInfoList) || typeof(IEnumerable).IsAssignableFrom(pt))
                    foreach (var x in EnumerateFromValue(p.GetValue(oprtInfo), $"prop:{p.Name}"))
                        yield return x;
            }

            foreach (var f in t.GetFields(flags))
            {
                if (f.IsStatic) continue;
                var ft = f.FieldType;
                if (ft == typeof(MapOprtCellInstInfoList) || typeof(IEnumerable).IsAssignableFrom(ft))
                    foreach (var x in EnumerateFromValue(f.GetValue(oprtInfo), $"field:{f.Name}"))
                        yield return x;
            }
        }

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);

            string propertyEditJson = br.ReadString("PropertyEditModelBase");
            if (!string.IsNullOrEmpty(propertyEditJson))
            {
                try
                {
                    var propertyEditModelBase = JsonObjConvert.FromJSon<ProgressBarPropertyModelEdit>(propertyEditJson);
                    if (propertyEditModelBase != null)
                    {
                        (PropertyEditModelBase as ProgressBarPropertyModelEdit).CopyFrom(propertyEditModelBase);
                        _loadedPropertyEditFromBytes = true;
                    }
                }
                catch { }
            }

            string propertyBindJson = br.ReadString("PropertyBindEditModelBase");
            if (!string.IsNullOrEmpty(propertyBindJson))
            {
                try
                {
                    var propertyBindEditModelBase = JsonObjConvert.FromJSon<ProgressBarPropertyBindEditModel>(propertyBindJson);
                    if (propertyBindEditModelBase != null)
                        (PropertyBindEditModelBase as ProgressBarPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
                }
                catch { }
            }

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
        }

        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
        }

        protected override void OnCopyFrom(ControlCellBase source)
        {
            MapCellProgressBarCtlObj obj = (source as MapCellProgressBarCtlObj);
            base._CopyFrom(obj);
            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
            (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel.CopyFrom(source.EventBindEditModel);
        }

        protected override void OnInit()
        {
            base.OnInit();
        }
        protected override object OnGetView() => view;
        protected override object OnGetViewModel() => progressBarViewModel;
        public override PropertyEditModelBase CreatePropertyModelEditBase() => new ProgressBarPropertyModelEdit();
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new ProgressBarPropertyBindEditModel();
        public override EventBindEditModel CreateEventBindEditModel() => new EventBindEditModel() 
        { 
            EventCmdInfos = new BindingList<EventCmdInfo>() 
            {
                new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = "ValueChanged" },
                new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = "Completed" },
            }
        };
        public override void OnZoomChanged() { }
        public override string ToString() => "进度条";


        private static IEnumerable<MemberInfo> EnumerateInstanceMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            for (var t = type; t != null; t = t.BaseType)
            {
                foreach (var f in t.GetFields(flags)) { if (!f.IsStatic) yield return f; }
                foreach (var p in t.GetProperties(flags)) { if (p.GetIndexParameters().Length == 0 && p.CanRead) yield return p; }
            }
        }

        private static object? GetMemberValue(MemberInfo member, object instance)
        {
            try { return member switch { FieldInfo f => f.GetValue(instance), PropertyInfo p => p.GetValue(instance), _ => null }; }
            catch { return null; }
        }

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == ProgressBarMapOprtCellConst.BrushInfo_MapOprtCellID)
                return ExecuteOprtCell<BrushInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == ProgressBarMapOprtCellConst.AppearanceInfo_MapOprtCellID)
                return ExecuteOprtCell<AppearanceInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == ProgressBarMapOprtCellConst.LayoutInfo_MapOprtCellID)
                return ExecuteOprtCell<LayoutInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == ProgressBarMapOprtCellConst.CommonInfo_MapOprtCellID)
                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == ProgressBarMapOprtCellConst.TextInfo_MapOprtCellID)
                return ExecuteOprtCell<TextInfoMapOprtCellExector>(mapOprtCellInstInfo);
            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        private bool ExecuteOprtCell<T>(MapOprtCellInstInfo mapOprtCellInstInfo) where T : IMapOprtCellExector, new()
        {
            if (_oprtCellIdByInstanceId.TryGetValue(mapOprtCellInstInfo.InstanceID, out var oldOprtCellId) && oldOprtCellId != mapOprtCellInstInfo.OprtCellID)
                MapOprtCellExectorDict.TryRemove(mapOprtCellInstInfo.InstanceID, out _);

            if (!MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector) || mapOprtCellExector == null || mapOprtCellExector.GetType() != typeof(T))
            {
                mapOprtCellExector = new T();
                mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
            }

            _oprtCellIdByInstanceId.AddOrUpdate(mapOprtCellInstInfo.InstanceID, mapOprtCellInstInfo.OprtCellID, (_, __) => mapOprtCellInstInfo.OprtCellID);
            mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
            return true;
        }

        private static void PostToUI(Action action)
        {
            if (Dispatcher.UIThread.CheckAccess()) action();
            else Dispatcher.UIThread.Post(action);
        }

        private class BrushInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is ProgressBarViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<BrushInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                var back = Color.Parse(param.BackColor);
                                var border = Color.Parse(param.BorderColor);
                                var fore = Color.Parse(param.ForeColor);
                                PostToUI(() => { vm.BackColor = back; vm.BorderColor = border; vm.ForeColor = fore; });
                                return;
                            }
                        }
                        catch { }
                    }
                    // 直接从 Model 读取当前值
                    PostToUI(() => 
                    { 
                        vm.BackColor = vm.Model.BrushInfo.BackColor; 
                        vm.BorderColor = vm.Model.BrushInfo.BorderColor; 
                        vm.ForeColor = vm.Model.BrushInfo.ForeColor; 
                    });
                }
            }
        }

        private class AppearanceInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is ProgressBarViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<AppearanceInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                double.TryParse(param.Opacity, out var opacity);
                                double.TryParse(param.BorderThicknessLeft, out var btl);
                                double.TryParse(param.BorderThicknessTop, out var btt);
                                double.TryParse(param.BorderThicknessRight, out var btr);
                                double.TryParse(param.BorderThicknessBottom, out var btb);
                                PostToUI(() => { vm.Opacity = opacity; vm.BorderThicknessLeft = btl; vm.BorderThicknessTop = btt; vm.BorderThicknessRight = btr; vm.BorderThicknessBottom = btb; });
                                return;
                            }
                        }
                        catch { }
                    }
                    // 直接从 Model 读取当前值
                    PostToUI(() => 
                    { 
                        vm.Opacity = vm.Model.AppearanceInfo.Opacity;
                        vm.BorderThicknessLeft = vm.Model.AppearanceInfo.BorderThicknessLeft;
                        vm.BorderThicknessTop = vm.Model.AppearanceInfo.BorderThicknessTop;
                        vm.BorderThicknessRight = vm.Model.AppearanceInfo.BorderThicknessRight;
                        vm.BorderThicknessBottom = vm.Model.AppearanceInfo.BorderThicknessBottom;
                    });
                }
            }
        }

        private class LayoutInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is ProgressBarViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<LayoutInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                double.TryParse(param.MarginTop, out var mt);
                                double.TryParse(param.MarginLeft, out var ml);
                                double.TryParse(param.MarginBottom, out var mb);
                                double.TryParse(param.MarginRight, out var mr);
                                double.TryParse(param.MinWidth, out var minW);
                                double.TryParse(param.MaxWidth, out var maxW);
                                double.TryParse(param.MinHeight, out var minH);
                                double.TryParse(param.MaxHeight, out var maxH);
                                // 宽高主数据统一落到父类 Width/Height，避免进度条继续维护额外尺寸中间态。
                                PostToUI(() => { vm.HorizontalAlign = param.HorizontalAlign; vm.VerticalAlign = param.VerticalAlign; vm.MarginTop = mt; vm.MarginLeft = ml; vm.MarginBottom = mb; vm.MarginRight = mr; vm.MinWidth = minW; vm.MaxWidth = maxW; vm.MinHeight = minH; vm.MaxHeight = maxH; });
                                return;
                            }
                        }
                        catch { }
                    }
                    // 直接从 Model 读取当前值
                    PostToUI(() => 
                    { 
                        // 宽高主数据已经收口到父类 Width/Height，这里不再额外维护进度条的尺寸中间态。
                        vm.HorizontalAlign = vm.Model.LayoutInfo.HorizontalAlign;
                        vm.VerticalAlign = vm.Model.LayoutInfo.VerticalAlign;
                        vm.MarginTop = vm.Model.LayoutInfo.MarginTop;
                        vm.MarginLeft = vm.Model.LayoutInfo.MarginLeft;
                        vm.MarginBottom = vm.Model.LayoutInfo.MarginBottom;
                        vm.MarginRight = vm.Model.LayoutInfo.MarginRight;
                        vm.MinWidth = vm.Model.LayoutInfo.MinWidth;
                        vm.MaxWidth = vm.Model.LayoutInfo.MaxWidth;
                        vm.MinHeight = vm.Model.LayoutInfo.MinHeight;
                        vm.MaxHeight = vm.Model.LayoutInfo.MaxHeight;
                    });
                }
            }
        }

        private class CommonInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is ProgressBarViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<CommonInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                double.TryParse(param.Minimum, out var min);
                                double.TryParse(param.Maximum, out var max);
                                double.TryParse(param.Value, out var valueNum);
                                PostToUI(() => { vm.Orientation = param.Orientation; vm.Minimum = min; vm.Maximum = max; vm.Value = valueNum; vm.IsIndeterminate = param.IsIndeterminate; vm.IsEnabled = param.IsEnabled; vm.ToolTip = param.ToolTip; });
                                return;
                            }
                        }
                        catch { }
                    }
                    // 直接从 Model 读取当前值
                    PostToUI(() => 
                    { 
                        vm.Orientation = vm.Model.CommonInfo.Orientation;
                        vm.Minimum = vm.Model.CommonInfo.Minimum;
                        vm.Maximum = vm.Model.CommonInfo.Maximum;
                        vm.Value = vm.Model.CommonInfo.Value;
                        vm.IsIndeterminate = vm.Model.CommonInfo.IsIndeterminate;
                        vm.IsEnabled = vm.Model.CommonInfo.IsEnabled;
                        vm.ToolTip = vm.Model.CommonInfo.ToolTip;
                    });
                }
            }
        }

        private class TextInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is ProgressBarViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<TextInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                double.TryParse(param.FontSize, out var fontSize);
                                var fontColor = Color.Parse(param.FontColor);
                                PostToUI(() => { vm.FontFamilyType = param.FontFamilyType; vm.FontColor = fontColor; vm.FontSize = fontSize; vm.IsItalic = param.IsItalic; vm.IsBold = param.IsBold; });
                                return;
                            }
                        }
                        catch { }
                    }
                    // 直接从 Model 读取当前值
                    PostToUI(() => 
                    { 
                        vm.FontFamilyType = vm.Model.TextInfo.FontFamilyType;
                        vm.FontColor = vm.Model.TextInfo.FontColor;
                        vm.FontSize = vm.Model.TextInfo.FontSize;
                        vm.IsItalic = vm.Model.TextInfo.IsItalic;
                        vm.IsBold = vm.Model.TextInfo.IsBold;
                    });
                }
            }
        }

        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }

        public override MapCellPropValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null;
        }
    }

    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("画笔", 1)]
    [CategoryPriority("外观", 2)]
    [CategoryPriority("布局", 3)]
    [CategoryPriority("公共", 4)]
    [CategoryPriority("文本", 5)]
    public class ProgressBarPropertyModelEdit : ControlCellPropertyModelEdit
    {
        private ProgressBarBrushInfo _brushInfo = new ProgressBarBrushInfo();
        [DisplayName("画笔设置")]
        [Category("画笔")]
        [PropertySortOrder(1)]
        public ProgressBarBrushInfo BrushInfo { get => _brushInfo; set => SetProperty(ref _brushInfo, value); }

        private ProgressBarAppearanceInfo _appearanceInfo = new ProgressBarAppearanceInfo();
        [DisplayName("外观设置")]
        [Category("外观")]
        [PropertySortOrder(1)]
        public ProgressBarAppearanceInfo AppearanceInfo { get => _appearanceInfo; set => SetProperty(ref _appearanceInfo, value); }

        private ProgressBarLayoutInfo _layoutInfo = new ProgressBarLayoutInfo();
        [DisplayName("布局设置")]
        [Category("布局")]
        [PropertySortOrder(1)]
        public ProgressBarLayoutInfo LayoutInfo { get => _layoutInfo; set => SetProperty(ref _layoutInfo, value); }

        private ProgressBarCommonInfo _commonInfo = new ProgressBarCommonInfo();
        [DisplayName("公共设置")]
        [Category("公共")]
        [PropertySortOrder(1)]
        public ProgressBarCommonInfo CommonInfo { get => _commonInfo; set => SetProperty(ref _commonInfo, value); }

        private ProgressBarTextInfo _textInfo = new ProgressBarTextInfo();
        [DisplayName("文本设置")]
        [Category("文本")]
        [PropertySortOrder(1)]
        public ProgressBarTextInfo TextInfo { get => _textInfo; set => SetProperty(ref _textInfo, value); }

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(ProgressBarCommonInfo.Value)) == 0)
            {
                _commonInfo ??= new ProgressBarCommonInfo();
                _commonInfo.Value = propertyVal != null ? (double)propertyVal.ToPrimitiveValue<decimal>() : ProgressBarCommonInfo.Default.Value;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(ProgressBarCommonInfo.Minimum)) == 0)
            {
                _commonInfo ??= new ProgressBarCommonInfo();
                _commonInfo.Minimum = propertyVal != null ? (double)propertyVal.ToPrimitiveValue<decimal>() : ProgressBarCommonInfo.Default.Minimum;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(ProgressBarCommonInfo.Maximum)) == 0)
            {
                _commonInfo ??= new ProgressBarCommonInfo();
                _commonInfo.Maximum = propertyVal != null ? (double)propertyVal.ToPrimitiveValue<decimal>() : ProgressBarCommonInfo.Default.Maximum;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(ProgressBarCommonInfo.IsEnabled)) == 0)
            {
                _commonInfo ??= new ProgressBarCommonInfo();
                _commonInfo.IsEnabled = propertyVal != null ? propertyVal.ToPrimitiveValue<bool>() : ProgressBarCommonInfo.Default.IsEnabled;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(ProgressBarBrushInfo.BackColor)) == 0)
            {
                _brushInfo ??= new ProgressBarBrushInfo();
                if (propertyVal != null)
                {
                    var colorStr = propertyVal.ToPrimitiveValue<string>();
                    _brushInfo.BackColorStr = Color.Parse(colorStr).ToColorString();
                }
                else
                {
                    _brushInfo.BackColorStr = ProgressBarBrushInfo.Default.BackColorStr;
                }
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            // 绑定面板现在支持单独下发边框颜色，这里补齐叶子属性到 BrushInfo 的写入链。
            if (string.Compare(propertyID, nameof(ProgressBarBrushInfo.BorderColor)) == 0)
            {
                _brushInfo ??= new ProgressBarBrushInfo();
                if (propertyVal != null)
                {
                    var colorStr = propertyVal.ToPrimitiveValue<string>();
                    _brushInfo.BorderColorStr = Color.Parse(colorStr).ToColorString();
                }
                else
                {
                    _brushInfo.BorderColorStr = ProgressBarBrushInfo.Default.BorderColorStr;
                }
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(ProgressBarBrushInfo.ForeColor)) == 0)
            {
                _brushInfo ??= new ProgressBarBrushInfo();
                if (propertyVal != null)
                {
                    var colorStr = propertyVal.ToPrimitiveValue<string>();
                    _brushInfo.ForeColorStr = Color.Parse(colorStr).ToColorString();
                }
                else
                {
                    _brushInfo.ForeColorStr = ProgressBarBrushInfo.Default.ForeColorStr;
                }
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }

            if (string.Compare(propertyID, nameof(BrushInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<ProgressBarBrushInfo>(propertyVal) : new ProgressBarBrushInfo();
                _brushInfo ??= new ProgressBarBrushInfo();
                _brushInfo.BackColorStr = src.BackColorStr;
                _brushInfo.BorderColorStr = src.BorderColorStr;
                _brushInfo.ForeColorStr = src.ForeColorStr;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(AppearanceInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<ProgressBarAppearanceInfo>(propertyVal) : new ProgressBarAppearanceInfo();
                _appearanceInfo ??= new ProgressBarAppearanceInfo();
                _appearanceInfo.Opacity = src.Opacity;
                _appearanceInfo.BorderThicknessLeft = src.BorderThicknessLeft;
                _appearanceInfo.BorderThicknessTop = src.BorderThicknessTop;
                _appearanceInfo.BorderThicknessRight = src.BorderThicknessRight;
                _appearanceInfo.BorderThicknessBottom = src.BorderThicknessBottom;
                RaisePropertyChanged(nameof(AppearanceInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(LayoutInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<ProgressBarLayoutInfo>(propertyVal) : new ProgressBarLayoutInfo();
                _layoutInfo ??= new ProgressBarLayoutInfo();
                _layoutInfo.HorizontalAlign = src.HorizontalAlign;
                _layoutInfo.VerticalAlign = src.VerticalAlign;
                _layoutInfo.MarginTop = src.MarginTop;
                _layoutInfo.MarginLeft = src.MarginLeft;
                _layoutInfo.MarginBottom = src.MarginBottom;
                _layoutInfo.MarginRight = src.MarginRight;
                _layoutInfo.MinWidth = src.MinWidth;
                _layoutInfo.MaxWidth = src.MaxWidth;
                _layoutInfo.MinHeight = src.MinHeight;
                _layoutInfo.MaxHeight = src.MaxHeight;
                RaisePropertyChanged(nameof(LayoutInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(CommonInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<ProgressBarCommonInfo>(propertyVal) : new ProgressBarCommonInfo();
                _commonInfo ??= new ProgressBarCommonInfo();
                _commonInfo.Orientation = src.Orientation;
                _commonInfo.Value = src.Value;
                _commonInfo.Minimum = src.Minimum;
                _commonInfo.Maximum = src.Maximum;
                _commonInfo.IsIndeterminate = src.IsIndeterminate;
                _commonInfo.IsEnabled = src.IsEnabled;
                _commonInfo.ToolTip = src.ToolTip;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(TextInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<ProgressBarTextInfo>(propertyVal) : new ProgressBarTextInfo();
                _textInfo ??= new ProgressBarTextInfo();
                _textInfo.FontFamilyType = src.FontFamilyType;
                _textInfo.FontColorStr = src.FontColorStr;
                _textInfo.FontSize = src.FontSize;
                _textInfo.IsItalic = src.IsItalic;
                _textInfo.IsBold = src.IsBold;
                RaisePropertyChanged(nameof(TextInfo));
                return true;
            }
            return base.SetPropertyValue(propertyID, propertyVal);
        }

        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }

        public void CopyFrom(ProgressBarPropertyModelEdit source)
        {
            if (source == null) return;

            base.CopyFrom(source);

            _brushInfo ??= new ProgressBarBrushInfo();
            _brushInfo.BackColorStr = source.BrushInfo?.BackColorStr;
            _brushInfo.BorderColorStr = source.BrushInfo?.BorderColorStr;
            _brushInfo.ForeColorStr = source.BrushInfo?.ForeColorStr;
            RaisePropertyChanged(nameof(BrushInfo));

            _appearanceInfo ??= new ProgressBarAppearanceInfo();
            _appearanceInfo.Opacity = source.AppearanceInfo?.Opacity ?? 1.0;
            _appearanceInfo.BorderThicknessLeft = source.AppearanceInfo?.BorderThicknessLeft ?? 1;
            _appearanceInfo.BorderThicknessTop = source.AppearanceInfo?.BorderThicknessTop ?? 1;
            _appearanceInfo.BorderThicknessRight = source.AppearanceInfo?.BorderThicknessRight ?? 1;
            _appearanceInfo.BorderThicknessBottom = source.AppearanceInfo?.BorderThicknessBottom ?? 1;
            RaisePropertyChanged(nameof(AppearanceInfo));

            _layoutInfo ??= new ProgressBarLayoutInfo();
            _layoutInfo.HorizontalAlign = source.LayoutInfo?.HorizontalAlign ?? HorizontalAlignType.Left;
            _layoutInfo.VerticalAlign = source.LayoutInfo?.VerticalAlign ?? VerticalAlignType.Center;
            _layoutInfo.MarginTop = source.LayoutInfo?.MarginTop ?? 0;
            _layoutInfo.MarginLeft = source.LayoutInfo?.MarginLeft ?? 0;
            _layoutInfo.MarginBottom = source.LayoutInfo?.MarginBottom ?? 0;
            _layoutInfo.MarginRight = source.LayoutInfo?.MarginRight ?? 0;
            _layoutInfo.MinWidth = source.LayoutInfo?.MinWidth ?? 0;
            _layoutInfo.MaxWidth = source.LayoutInfo?.MaxWidth ?? 0;
            _layoutInfo.MinHeight = source.LayoutInfo?.MinHeight ?? 0;
            _layoutInfo.MaxHeight = source.LayoutInfo?.MaxHeight ?? 0;
            RaisePropertyChanged(nameof(LayoutInfo));

            _commonInfo ??= new ProgressBarCommonInfo();
            _commonInfo.Orientation = source.CommonInfo?.Orientation ?? Orientation.Horizontal;
            _commonInfo.Minimum = source.CommonInfo?.Minimum ?? 0;
            _commonInfo.Maximum = source.CommonInfo?.Maximum ?? 100;
            _commonInfo.Value = source.CommonInfo?.Value ?? 0;
            _commonInfo.IsIndeterminate = source.CommonInfo?.IsIndeterminate ?? false;
            _commonInfo.IsEnabled = source.CommonInfo?.IsEnabled ?? true;
            _commonInfo.ToolTip = source.CommonInfo?.ToolTip ?? "";
            RaisePropertyChanged(nameof(CommonInfo));

            _textInfo ??= new ProgressBarTextInfo();
            _textInfo.FontFamilyType = source.TextInfo?.FontFamilyType ?? FontFamilyType.MicrosoftYaHei;
            _textInfo.FontColorStr = source.TextInfo?.FontColorStr;
            _textInfo.FontSize = source.TextInfo?.FontSize ?? 12;
            _textInfo.IsItalic = source.TextInfo?.IsItalic ?? false;
            _textInfo.IsBold = source.TextInfo?.IsBold ?? false;
            RaisePropertyChanged(nameof(TextInfo));
        }
    }

    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("绑定信息", 1)]
    public class ProgressBarPropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _value = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Decimal);
        /// <summary>
        /// 值绑定
        /// </summary>
        [DisplayName("值")]
        [Category("绑定信息")]
        [PropertySortOrder(1)]
        [BindMPPropertyID]
        public PropertyBindInfo Value
        {
            get => _value;
            set => SetProperty(ref _value, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Decimal));
        }

        private PropertyBindInfo _minimum = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Decimal);
        /// <summary>
        /// 最小值绑定
        /// </summary>
        [DisplayName("最小值")]
        [Category("绑定信息")]
        [PropertySortOrder(2)]
        [BindMPPropertyID]
        public PropertyBindInfo Minimum
        {
            get => _minimum;
            set => SetProperty(ref _minimum, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Decimal));
        }

        private PropertyBindInfo _maximum = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Decimal);
        /// <summary>
        /// 最大值绑定
        /// </summary>
        [DisplayName("最大值")]
        [Category("绑定信息")]
        [PropertySortOrder(3)]
        [BindMPPropertyID]
        public PropertyBindInfo Maximum
        {
            get => _maximum;
            set => SetProperty(ref _maximum, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Decimal));
        }

        private PropertyBindInfo _foreColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        /// <summary>
        /// 前景色绑定
        /// </summary>
        [DisplayName("前景色")]
        [Category("绑定信息")]
        [PropertySortOrder(4)]
        [BindMPPropertyID]
        public PropertyBindInfo ForeColor
        {
            get => _foreColor;
            set => SetProperty(ref _foreColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _backColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        /// <summary>
        /// 背景色绑定
        /// </summary>
        [DisplayName("背景色")]
        [Category("绑定信息")]
        [PropertySortOrder(5)]
        [BindMPPropertyID]
        public PropertyBindInfo BackColor
        {
            get => _backColor;
            set => SetProperty(ref _backColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _borderColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        /// <summary>
        /// 边框颜色绑定
        /// </summary>
        [DisplayName("边框颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(6)]
        [BindMPPropertyID]
        public PropertyBindInfo BorderColor
        {
            get => _borderColor;
            set => SetProperty(ref _borderColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        public void CopyFrom(ProgressBarPropertyBindEditModel source)
        {
            if (source == null) return;
            base.CopyFrom(source);
            // 这里仅保留进度条当前仍支持的 6 个显式绑定项，避免旧字段继续出现在绑定面板。
            Value = source.Value;
            Minimum = source.Minimum;
            Maximum = source.Maximum;
            ForeColor = source.ForeColor;
            BackColor = source.BackColor;
            BorderColor = source.BorderColor;
        }
    }
}


