using Avalonia.Media;

using Avalonia.Threading;

using GF_Gereric;

using Griffins;

using GKG.Map.MapCell.Generic.CheckBox.MapOprtCellParamCfgView;

using GKG.Map.MapCell.Generic.CheckBox.View;

using GKG.Map.MapCell.Generic.CheckBox.ViewModel;

using GKG.Map.MapCell.Generic.GroupPanel;


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
using GKG.Map.MapCell.Generic.Control.Lable;



namespace GKG.Map.MapCell.Generic.CheckBox

{

    class MapCellCheckBoxCtlObj : ControlCellBase

    {

        private CheckBoxView view;

        private CheckBoxViewModel checkBoxViewModel;

        private readonly ConcurrentDictionary<Guid, MapOprtCellID> _oprtCellIdByInstanceId = new();

        private MapObjID _mapCellID;

        private string _mapCellName;

        private bool _loadedPropertyEditFromBytes;





        public MapCellCheckBoxCtlObj(MapObjID mapCellID, string mapCellName)

            : this(mapCellID, mapCellName, false) { }



        public MapCellCheckBoxCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)

        {

            PropertyEditModelBase = CreatePropertyModelEditBase();

            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();

            EventBindEditModel = CreateEventBindEditModel();

            _mapCellID = mapCellID;

            _mapCellName = mapCellName;

            base.SetID(mapCellID);

            base.SetName(mapCellName);

            view = new CheckBoxView();



            RegisterProperty(new MapObjPropertyInfo(nameof(CheckBoxPropertyModelEdit.BrushInfo), "画笔设置", MapCellPropDataType.Object_Json, CheckBoxBrushInfo.Object_ID, typeof(CheckBoxBrushInfo), false, true, new MapCellPropValue(CheckBoxBrushInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(CheckBoxPropertyModelEdit.AppearanceInfo), "外观设置", MapCellPropDataType.Object_Json, CheckBoxAppearanceInfo.Object_ID, typeof(CheckBoxAppearanceInfo), false, true, new MapCellPropValue(CheckBoxAppearanceInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(CheckBoxPropertyModelEdit.LayoutInfo), "布局设置", MapCellPropDataType.Object_Json, CheckBoxLayoutInfo.Object_ID, typeof(CheckBoxLayoutInfo), false, true, new MapCellPropValue(CheckBoxLayoutInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(CheckBoxPropertyModelEdit.CommonInfo), "公共设置", MapCellPropDataType.Object_Json, CheckBoxCommonInfo.Object_ID, typeof(CheckBoxCommonInfo), false, true, new MapCellPropValue(CheckBoxCommonInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(CheckBoxPropertyModelEdit.TextInfo), "文本设置", MapCellPropDataType.Object_Json, CheckBoxTextInfo.Object_ID, typeof(CheckBoxTextInfo), false, true, new MapCellPropValue(CheckBoxTextInfo.Default)));



            RegisterOprtCellInfo(new MapOprtCellInfo(CheckBoxMapOprtCellConst.BrushInfo_MapOprtCellID, "画笔设置操作原子", typeof(BrushInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(CheckBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID, "外观设置操作原子", typeof(AppearanceInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(CheckBoxMapOprtCellConst.LayoutInfo_MapOprtCellID, "布局设置操作原子", typeof(LayoutInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(CheckBoxMapOprtCellConst.CommonInfo_MapOprtCellID, "公共设置操作原子", typeof(CommonInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(CheckBoxMapOprtCellConst.TextInfo_MapOprtCellID, "文本设置操作原子", typeof(TextInfoMapOprtCellParamCfgView)));



            RegisterOprtInfo(new MapOprtInfo(nameof(CheckBoxPropertyModelEdit.BrushInfo), "设置画笔", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = CheckBoxMapOprtCellConst.BrushInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(CheckBoxPropertyModelEdit.AppearanceInfo), "设置外观", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = CheckBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(CheckBoxPropertyModelEdit.LayoutInfo), "设置布局", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = CheckBoxMapOprtCellConst.LayoutInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(CheckBoxPropertyModelEdit.CommonInfo), "设置公共", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = CheckBoxMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(CheckBoxPropertyModelEdit.TextInfo), "设置文本", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = CheckBoxMapOprtCellConst.TextInfo_MapOprtCellID, CfgInfo = null } }));



            (this as IMapCellTypeBase).Name = ResourceA.CheckBox;

            checkBoxViewModel = new CheckBoxViewModel(designTime, CheckBoxPropertyModelEdit, OnEventTriggered);

            view.DataContext = checkBoxViewModel;

            CheckBoxPropertyModelEdit.BrushInfo.PropertyChanged += OnBrushInfoPropertyChanged;
            CheckBoxPropertyModelEdit.AppearanceInfo.PropertyChanged += OnAppearanceInfoPropertyChanged;
            CheckBoxPropertyModelEdit.LayoutInfo.PropertyChanged += OnLayoutInfoPropertyChanged;
            CheckBoxPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
            CheckBoxPropertyModelEdit.TextInfo.PropertyChanged += OnTextInfoPropertyChanged;





        }

        private void OnBrushInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(CheckBoxPropertyModelEdit.BrushInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnAppearanceInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(CheckBoxPropertyModelEdit.AppearanceInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnLayoutInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(CheckBoxPropertyModelEdit.LayoutInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(CheckBoxPropertyModelEdit.CommonInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnTextInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(CheckBoxPropertyModelEdit.TextInfo), "PropertyChanged", e?.PropertyName);
        }

        public override void OnDispose()
        {
            CheckBoxPropertyModelEdit.BrushInfo.PropertyChanged -= OnBrushInfoPropertyChanged;
            CheckBoxPropertyModelEdit.AppearanceInfo.PropertyChanged -= OnAppearanceInfoPropertyChanged;
            CheckBoxPropertyModelEdit.LayoutInfo.PropertyChanged -= OnLayoutInfoPropertyChanged;
            CheckBoxPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;
            CheckBoxPropertyModelEdit.TextInfo.PropertyChanged -= OnTextInfoPropertyChanged;

            view.DataContext = null;
            checkBoxViewModel?.Dispose();
            checkBoxViewModel = null;

            base.OnDispose();
        }



        public CheckBoxPropertyModelEdit CheckBoxPropertyModelEdit => PropertyEditModelBase as CheckBoxPropertyModelEdit;



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

            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoadedCheckBox(propertyID, propertyVal))

            {

                return true;

            }

            CheckBoxPropertyModelEdit.IsRuning = isRuning;

            var ok = CheckBoxPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);

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

                    // ?????????????????????????????? BrushInfo / CommonInfo?

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



        private bool IsDefaultOverwriteForLoadedCheckBox(string propertyID, MapCellPropValue propertyVal)

        {

            try

            {

                if (string.Compare(propertyID, nameof(CheckBoxPropertyModelEdit.BrushInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<CheckBoxBrushInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(CheckBoxPropertyModelEdit.BrushInfo);

                }

                if (string.Compare(propertyID, nameof(CheckBoxPropertyModelEdit.AppearanceInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<CheckBoxAppearanceInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(CheckBoxPropertyModelEdit.AppearanceInfo);

                }

                if (string.Compare(propertyID, nameof(CheckBoxPropertyModelEdit.LayoutInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<CheckBoxLayoutInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(CheckBoxPropertyModelEdit.LayoutInfo);

                }

                if (string.Compare(propertyID, nameof(CheckBoxPropertyModelEdit.CommonInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<CheckBoxCommonInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(CheckBoxPropertyModelEdit.CommonInfo);

                }

                if (string.Compare(propertyID, nameof(CheckBoxPropertyModelEdit.TextInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<CheckBoxTextInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(CheckBoxPropertyModelEdit.TextInfo);

                }

                return false;

            }

            catch { return false; }

        }



        private static bool IsDefault(CheckBoxBrushInfo? info)

        {

            if (info == null) return true;

            return string.Equals(info.BackColorStr ?? "", CheckBoxBrushInfo.Default.BackColorStr, StringComparison.OrdinalIgnoreCase)

                && string.Equals(info.BorderColorStr ?? "", CheckBoxBrushInfo.Default.BorderColorStr, StringComparison.OrdinalIgnoreCase)

                && string.Equals(info.ForeColorStr ?? "", CheckBoxBrushInfo.Default.ForeColorStr, StringComparison.OrdinalIgnoreCase);

        }



        private static bool IsDefault(CheckBoxAppearanceInfo? info)

        {

            if (info == null) return true;

            return Math.Abs(info.Opacity - CheckBoxAppearanceInfo.Default.Opacity) < 0.000001

                && Math.Abs(info.BorderThicknessLeft - CheckBoxAppearanceInfo.Default.BorderThicknessLeft) < 0.000001

                && Math.Abs(info.BorderThicknessTop - CheckBoxAppearanceInfo.Default.BorderThicknessTop) < 0.000001

                && Math.Abs(info.BorderThicknessRight - CheckBoxAppearanceInfo.Default.BorderThicknessRight) < 0.000001

                && Math.Abs(info.BorderThicknessBottom - CheckBoxAppearanceInfo.Default.BorderThicknessBottom) < 0.000001;

        }



        private static bool IsDefault(CheckBoxLayoutInfo? info)

        {

            if (info == null) return true;

            return info.HorizontalAlign == CheckBoxLayoutInfo.Default.HorizontalAlign

                && info.VerticalAlign == CheckBoxLayoutInfo.Default.VerticalAlign

                && Math.Abs(info.MarginTop - CheckBoxLayoutInfo.Default.MarginTop) < 0.000001

                && Math.Abs(info.MarginLeft - CheckBoxLayoutInfo.Default.MarginLeft) < 0.000001

                && Math.Abs(info.MarginBottom - CheckBoxLayoutInfo.Default.MarginBottom) < 0.000001

                && Math.Abs(info.MarginRight - CheckBoxLayoutInfo.Default.MarginRight) < 0.000001

                && Math.Abs(info.MinWidth - CheckBoxLayoutInfo.Default.MinWidth) < 0.000001

                && Math.Abs(info.MaxWidth - CheckBoxLayoutInfo.Default.MaxWidth) < 0.000001

                && Math.Abs(info.MinHeight - CheckBoxLayoutInfo.Default.MinHeight) < 0.000001

                && Math.Abs(info.MaxHeight - CheckBoxLayoutInfo.Default.MaxHeight) < 0.000001;

        }



        private static bool IsDefault(CheckBoxCommonInfo? info)

        {

            if (info == null) return true;

            return string.Equals(info.Text ?? "", CheckBoxCommonInfo.Default.Text ?? "", StringComparison.Ordinal)

                && info.IsChecked == CheckBoxCommonInfo.Default.IsChecked

                && info.IsThreeState == CheckBoxCommonInfo.Default.IsThreeState

                && info.CursorType == CheckBoxCommonInfo.Default.CursorType

                && info.IsEnabled == CheckBoxCommonInfo.Default.IsEnabled

                && string.Equals(info.ToolTip ?? "", CheckBoxCommonInfo.Default.ToolTip ?? "", StringComparison.Ordinal);

        }



        private static bool IsDefault(CheckBoxTextInfo? info)

        {

            if (info == null) return true;

            return info.FontFamilyType == CheckBoxTextInfo.Default.FontFamilyType

                && string.Equals(info.FontColorStr ?? "", CheckBoxTextInfo.Default.FontColorStr, StringComparison.OrdinalIgnoreCase)

                && Math.Abs(info.FontSize - CheckBoxTextInfo.Default.FontSize) < 0.000001

                && info.IsItalic == CheckBoxTextInfo.Default.IsItalic

                && info.IsBold == CheckBoxTextInfo.Default.IsBold;

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



            if (string.Equals(oprtId, nameof(CheckBoxBrushInfo.BackColor), StringComparison.Ordinal)

                || string.Equals(oprtId, nameof(CheckBoxBrushInfo.BorderColor), StringComparison.Ordinal)

                || string.Equals(oprtId, nameof(CheckBoxBrushInfo.ForeColor), StringComparison.Ordinal))

            {

                normalizedOprtId = nameof(CheckBoxPropertyModelEdit.BrushInfo);

                oprtCellId = CheckBoxMapOprtCellConst.BrushInfo_MapOprtCellID;

                return true;

            }

            if (string.Equals(oprtId, nameof(CheckBoxCommonInfo.Text), StringComparison.Ordinal)

                || string.Equals(oprtId, nameof(CheckBoxCommonInfo.IsChecked), StringComparison.Ordinal))

            {

                normalizedOprtId = nameof(CheckBoxPropertyModelEdit.CommonInfo);

                oprtCellId = CheckBoxMapOprtCellConst.CommonInfo_MapOprtCellID;

                return true;

            }

            if (string.Equals(oprtId, nameof(CheckBoxPropertyModelEdit.BrushInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(CheckBoxPropertyModelEdit.BrushInfo); oprtCellId = CheckBoxMapOprtCellConst.BrushInfo_MapOprtCellID; return true; }

            if (string.Equals(oprtId, nameof(CheckBoxPropertyModelEdit.AppearanceInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(CheckBoxPropertyModelEdit.AppearanceInfo); oprtCellId = CheckBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID; return true; }

            if (string.Equals(oprtId, nameof(CheckBoxPropertyModelEdit.LayoutInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(CheckBoxPropertyModelEdit.LayoutInfo); oprtCellId = CheckBoxMapOprtCellConst.LayoutInfo_MapOprtCellID; return true; }

            if (string.Equals(oprtId, nameof(CheckBoxPropertyModelEdit.CommonInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(CheckBoxPropertyModelEdit.CommonInfo); oprtCellId = CheckBoxMapOprtCellConst.CommonInfo_MapOprtCellID; return true; }

            if (string.Equals(oprtId, nameof(CheckBoxPropertyModelEdit.TextInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(CheckBoxPropertyModelEdit.TextInfo); oprtCellId = CheckBoxMapOprtCellConst.TextInfo_MapOprtCellID; return true; }

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

                    var propertyEditModelBase = JsonObjConvert.FromJSon<CheckBoxPropertyModelEdit>(propertyEditJson);

                    if (propertyEditModelBase != null)

                    {

                        (PropertyEditModelBase as CheckBoxPropertyModelEdit).CopyFrom(propertyEditModelBase);

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

                    var propertyBindEditModelBase = JsonObjConvert.FromJSon<CheckBoxPropertyBindEditModel>(propertyBindJson);

                    if (propertyBindEditModelBase != null)

                        (PropertyBindEditModelBase as CheckBoxPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);

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

            MapCellCheckBoxCtlObj obj = (source as MapCellCheckBoxCtlObj);

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

        protected override object OnGetViewModel() => checkBoxViewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new CheckBoxPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new CheckBoxPropertyBindEditModel();

        public override EventBindEditModel CreateEventBindEditModel() => new EventBindEditModel() 

        { 

            EventCmdInfos = new BindingList<EventCmdInfo>() 

            {

                new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = "CheckedChanged" },

                new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = "Checked" },

                new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = "Unchecked" },

            }

        };

        public override void OnZoomChanged() { }

        public override string ToString() => "复选框";





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

            if (mapOprtCellInstInfo.OprtCellID == CheckBoxMapOprtCellConst.BrushInfo_MapOprtCellID)

                return ExecuteOprtCell<BrushInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == CheckBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID)

                return ExecuteOprtCell<AppearanceInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == CheckBoxMapOprtCellConst.LayoutInfo_MapOprtCellID)

                return ExecuteOprtCell<LayoutInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == CheckBoxMapOprtCellConst.CommonInfo_MapOprtCellID)

                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == CheckBoxMapOprtCellConst.TextInfo_MapOprtCellID)

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

                if (callBack.GetMapCellVMObjInstance() is CheckBoxViewModel vm)

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

                    var val = callBack.GetMapCellPropValue(nameof(CheckBoxPropertyModelEdit.BrushInfo));

                    if (val != null)

                    {

                        try

                        {

                            var brushInfo = DeserializeObject<CheckBoxBrushInfo>(val);

                            PostToUI(() => { vm.BackColor = brushInfo.BackColor; vm.BorderColor = brushInfo.BorderColor; vm.ForeColor = brushInfo.ForeColor; });

                        }

                        catch { }

                    }

                }

            }

        }



        private class AppearanceInfoMapOprtCellExector : IMapOprtCellExector

        {

            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)

            {

                if (callBack.GetMapCellVMObjInstance() is CheckBoxViewModel vm)

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

                    var val = callBack.GetMapCellPropValue(nameof(CheckBoxPropertyModelEdit.AppearanceInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<CheckBoxAppearanceInfo>(val);

                            PostToUI(() => { vm.Opacity = info.Opacity; vm.BorderThicknessLeft = info.BorderThicknessLeft; vm.BorderThicknessTop = info.BorderThicknessTop; vm.BorderThicknessRight = info.BorderThicknessRight; vm.BorderThicknessBottom = info.BorderThicknessBottom; });

                        }

                        catch { }

                    }

                }

            }

        }



        private class LayoutInfoMapOprtCellExector : IMapOprtCellExector

        {

            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)

            {

                if (callBack.GetMapCellVMObjInstance() is CheckBoxViewModel vm)

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

                                // 宽高主数据统一落到父类 Width/Height，避免复选框继续维护额外尺寸中间态。
                                PostToUI(() => { vm.HorizontalAlign = param.HorizontalAlign; vm.VerticalAlign = param.VerticalAlign; vm.MarginTop = mt; vm.MarginLeft = ml; vm.MarginBottom = mb; vm.MarginRight = mr; vm.MinWidth = minW; vm.MaxWidth = maxW; vm.MinHeight = minH; vm.MaxHeight = maxH; });

                                return;

                            }

                        }

                        catch { }

                    }

                    var val = callBack.GetMapCellPropValue(nameof(CheckBoxPropertyModelEdit.LayoutInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<CheckBoxLayoutInfo>(val);

                            PostToUI(() => { vm.HorizontalAlign = info.HorizontalAlign; vm.VerticalAlign = info.VerticalAlign; vm.MarginTop = info.MarginTop; vm.MarginLeft = info.MarginLeft; vm.MarginBottom = info.MarginBottom; vm.MarginRight = info.MarginRight; vm.MinWidth = info.MinWidth; vm.MaxWidth = info.MaxWidth; vm.MinHeight = info.MinHeight; vm.MaxHeight = info.MaxHeight; });

                        }

                        catch { }

                    }

                }

            }

        }



        private class CommonInfoMapOprtCellExector : IMapOprtCellExector

        {

            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)

            {

                if (callBack.GetMapCellVMObjInstance() is CheckBoxViewModel vm)

                {

                    if (cfg != null && cfg.Length > 0)

                    {

                        try

                        {

                            var param = JsonSerializer.Deserialize<CommonInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));

                            if (param != null)

                            {

                                PostToUI(() => { vm.Text = param.Text; vm.IsChecked = param.IsChecked; vm.IsThreeState = param.IsThreeState; vm.CursorType = param.CursorType; vm.IsEnabled = param.IsEnabled; vm.ToolTip = param.ToolTip; });

                                return;

                            }

                        }

                        catch { }

                    }

                    var val = callBack.GetMapCellPropValue(nameof(CheckBoxPropertyModelEdit.CommonInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<CheckBoxCommonInfo>(val);

                            PostToUI(() => { vm.Text = info.Text; vm.IsChecked = info.IsChecked; vm.IsThreeState = info.IsThreeState; vm.CursorType = info.CursorType; vm.IsEnabled = info.IsEnabled; vm.ToolTip = info.ToolTip; });

                        }

                        catch { }

                    }

                }

            }

        }



        private class TextInfoMapOprtCellExector : IMapOprtCellExector

        {

            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)

            {

                if (callBack.GetMapCellVMObjInstance() is CheckBoxViewModel vm)

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

                    var val = callBack.GetMapCellPropValue(nameof(CheckBoxPropertyModelEdit.TextInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<CheckBoxTextInfo>(val);

                            PostToUI(() => { vm.FontFamilyType = info.FontFamilyType; vm.FontColor = info.FontColor; vm.FontSize = info.FontSize; vm.IsItalic = info.IsItalic; vm.IsBold = info.IsBold; });

                        }

                        catch { }

                    }

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

    public class CheckBoxPropertyModelEdit : ControlCellPropertyModelEdit

    {

        private CheckBoxBrushInfo _brushInfo = new CheckBoxBrushInfo();

        [DisplayName("画笔设置")]

        [Category("画笔")]

        [PropertySortOrder(1)]

        public CheckBoxBrushInfo BrushInfo { get => _brushInfo; set => SetProperty(ref _brushInfo, value); }



        private CheckBoxAppearanceInfo _appearanceInfo = new CheckBoxAppearanceInfo();

        [DisplayName("外观设置")]

        [Category("外观")]

        [PropertySortOrder(1)]

        public CheckBoxAppearanceInfo AppearanceInfo { get => _appearanceInfo; set => SetProperty(ref _appearanceInfo, value); }



        private CheckBoxLayoutInfo _layoutInfo = new CheckBoxLayoutInfo();

        [DisplayName("布局设置")]

        [Category("布局")]

        [PropertySortOrder(1)]

        public CheckBoxLayoutInfo LayoutInfo { get => _layoutInfo; set => SetProperty(ref _layoutInfo, value); }



        private CheckBoxCommonInfo _commonInfo = new CheckBoxCommonInfo();

        [DisplayName("公共设置")]

        [Category("公共")]

        [PropertySortOrder(1)]

        public CheckBoxCommonInfo CommonInfo { get => _commonInfo; set => SetProperty(ref _commonInfo, value); }



        private CheckBoxTextInfo _textInfo = new CheckBoxTextInfo();

        [DisplayName("文本设置")]

        [Category("文本")]

        [PropertySortOrder(1)]

        public CheckBoxTextInfo TextInfo { get => _textInfo; set => SetProperty(ref _textInfo, value); }



        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)

        {

            if (string.Compare(propertyID, nameof(CheckBoxCommonInfo.IsChecked)) == 0)

            {

                _commonInfo ??= new CheckBoxCommonInfo();

                _commonInfo.IsChecked = propertyVal != null ? (bool?)propertyVal.ToPrimitiveValue<bool>() : CheckBoxCommonInfo.Default.IsChecked;

                RaisePropertyChanged(nameof(CommonInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(CheckBoxCommonInfo.Text)) == 0)

            {

                _commonInfo ??= new CheckBoxCommonInfo();

                _commonInfo.Text = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : CheckBoxCommonInfo.Default.Text;

                RaisePropertyChanged(nameof(CommonInfo));

                return true;

            }


            if (string.Compare(propertyID, nameof(CheckBoxCommonInfo.IsEnabled)) == 0)

            {

                _commonInfo ??= new CheckBoxCommonInfo();

                _commonInfo.IsEnabled = propertyVal != null ? propertyVal.ToPrimitiveValue<bool>() : CheckBoxCommonInfo.Default.IsEnabled;

                RaisePropertyChanged(nameof(CommonInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(CheckBoxBrushInfo.ForeColor)) == 0)

            {

                _brushInfo ??= new CheckBoxBrushInfo();

                if (propertyVal != null)

                {

                    var colorStr = propertyVal.ToPrimitiveValue<string>();

                    _brushInfo.ForeColorStr = Color.Parse(colorStr).ToColorString();

                }

                else

                {

                    _brushInfo.ForeColorStr = CheckBoxBrushInfo.Default.ForeColorStr;

                }

                RaisePropertyChanged(nameof(BrushInfo));

                return true;

            }
            // 
            if (string.Compare(propertyID, nameof(CheckBoxBrushInfo.BackColor)) == 0)
            {
                _brushInfo ??= new CheckBoxBrushInfo();
                if (propertyVal != null)
                {
                    var colorStr = propertyVal.ToPrimitiveValue<string>();
                    _brushInfo.BackColorStr = Color.Parse(colorStr).ToColorString();
                }
                else
                {
                    _brushInfo.BackColorStr = CheckBoxBrushInfo.Default.BackColorStr;
                }
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(CheckBoxBrushInfo.BorderColor)) == 0)
            {
                _brushInfo ??= new CheckBoxBrushInfo();
                if (propertyVal != null)
                {
                    var colorStr = propertyVal.ToPrimitiveValue<string>();
                    _brushInfo.BorderColorStr = Color.Parse(colorStr).ToColorString();
                }
                else
                {
                    _brushInfo.BorderColorStr = CheckBoxBrushInfo.Default.BorderColorStr;
                }
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }

            if (string.Compare(propertyID, nameof(BrushInfo)) == 0)

            {

                var src = propertyVal != null ? DeserializeObject<CheckBoxBrushInfo>(propertyVal) : new CheckBoxBrushInfo();

                _brushInfo ??= new CheckBoxBrushInfo();

                _brushInfo.BackColorStr = src.BackColorStr;

                _brushInfo.BorderColorStr = src.BorderColorStr;

                _brushInfo.ForeColorStr = src.ForeColorStr;

                RaisePropertyChanged(nameof(BrushInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(AppearanceInfo)) == 0)

            {

                var src = propertyVal != null ? DeserializeObject<CheckBoxAppearanceInfo>(propertyVal) : new CheckBoxAppearanceInfo();

                _appearanceInfo ??= new CheckBoxAppearanceInfo();

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

                var src = propertyVal != null ? DeserializeObject<CheckBoxLayoutInfo>(propertyVal) : new CheckBoxLayoutInfo();

                _layoutInfo ??= new CheckBoxLayoutInfo();

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

                var src = propertyVal != null ? DeserializeObject<CheckBoxCommonInfo>(propertyVal) : new CheckBoxCommonInfo();

                _commonInfo ??= new CheckBoxCommonInfo();

                _commonInfo.Text = src.Text;

                _commonInfo.IsChecked = src.IsChecked;

                _commonInfo.IsThreeState = src.IsThreeState;

                _commonInfo.CursorType = src.CursorType;

                _commonInfo.IsEnabled = src.IsEnabled;

                _commonInfo.ToolTip = src.ToolTip;

                RaisePropertyChanged(nameof(CommonInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(TextInfo)) == 0)

            {

                var src = propertyVal != null ? DeserializeObject<CheckBoxTextInfo>(propertyVal) : new CheckBoxTextInfo();

                _textInfo ??= new CheckBoxTextInfo();

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



        public void CopyFrom(CheckBoxPropertyModelEdit source)

        {

            if (source == null) return;



            base.CopyFrom(source);



            _brushInfo ??= new CheckBoxBrushInfo();

            _brushInfo.BackColorStr = source.BrushInfo?.BackColorStr;

            _brushInfo.BorderColorStr = source.BrushInfo?.BorderColorStr;

            _brushInfo.ForeColorStr = source.BrushInfo?.ForeColorStr;

            RaisePropertyChanged(nameof(BrushInfo));



            _appearanceInfo ??= new CheckBoxAppearanceInfo();

            _appearanceInfo.Opacity = source.AppearanceInfo?.Opacity ?? 1.0;

            _appearanceInfo.BorderThicknessLeft = source.AppearanceInfo?.BorderThicknessLeft ?? 1;

            _appearanceInfo.BorderThicknessTop = source.AppearanceInfo?.BorderThicknessTop ?? 1;

            _appearanceInfo.BorderThicknessRight = source.AppearanceInfo?.BorderThicknessRight ?? 1;

            _appearanceInfo.BorderThicknessBottom = source.AppearanceInfo?.BorderThicknessBottom ?? 1;

            RaisePropertyChanged(nameof(AppearanceInfo));



            _layoutInfo ??= new CheckBoxLayoutInfo();

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



            _commonInfo ??= new CheckBoxCommonInfo();

            _commonInfo.Text = source.CommonInfo?.Text ?? "复选框";

            _commonInfo.IsChecked = source.CommonInfo != null ? source.CommonInfo.IsChecked : false;

            _commonInfo.IsThreeState = source.CommonInfo?.IsThreeState ?? false;

            _commonInfo.CursorType = source.CommonInfo?.CursorType ?? CursorType.Arrow;

            _commonInfo.IsEnabled = source.CommonInfo?.IsEnabled ?? true;

            _commonInfo.ToolTip = source.CommonInfo?.ToolTip ?? "";

            RaisePropertyChanged(nameof(CommonInfo));



            _textInfo ??= new CheckBoxTextInfo();

            _textInfo.FontFamilyType = source.TextInfo?.FontFamilyType ?? FontFamilyType.MicrosoftYaHei;

            _textInfo.FontColorStr = source.TextInfo?.FontColorStr;

            _textInfo.FontSize = source.TextInfo?.FontSize ?? 14;

            _textInfo.IsItalic = source.TextInfo?.IsItalic ?? false;

            _textInfo.IsBold = source.TextInfo?.IsBold ?? false;

            RaisePropertyChanged(nameof(TextInfo));

        }

    }



    /// <summary>

    /// 复选框属性绑定编辑模型

    /// </summary>

    [Serializable]

    [MapPropertyOrder]

    [CategoryPriority("绑定信息", 1)]

    public class CheckBoxPropertyBindEditModel : ControlCellPropertyBindEditModel

    {

        private PropertyBindInfo _isChecked = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Bool);

        /// <summary>

        /// 是否选中绑定

        /// </summary>

        [DisplayName("是否选中")]

        [Category("绑定信息")]

        [PropertySortOrder(1)]

        [BindMPPropertyID]

        public PropertyBindInfo IsChecked

        {

            get => _isChecked;

            set => SetProperty(ref _isChecked, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Bool));

        }



        private PropertyBindInfo _text = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        /// <summary>

        /// 文本绑定

        /// </summary>

        [DisplayName("文本")]

        [Category("绑定信息")]

        [PropertySortOrder(2)]

        [BindMPPropertyID]

        public PropertyBindInfo Text

        {

            get => _text;

            set => SetProperty(ref _text, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));

        }


        private PropertyBindInfo _backColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        /// <summary>
        /// ?????
        /// </summary>
        [DisplayName("???")]
        [Category("????")]
        [PropertySortOrder(3)]
        [BindMPPropertyID]
        public PropertyBindInfo BackColor
        {
            get => _backColor;
            set => SetProperty(ref _backColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _borderColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        /// <summary>
        /// ??????
        /// </summary>
        [DisplayName("????")]
        [Category("????")]
        [PropertySortOrder(4)]
        [BindMPPropertyID]
        public PropertyBindInfo BorderColor
        {
            get => _borderColor;
            set => SetProperty(ref _borderColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _foreColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        /// <summary>

        /// 前景色绑定

        /// </summary>

        [DisplayName("前景色")]

        [Category("绑定信息")]

        [PropertySortOrder(5)]

        [BindMPPropertyID]

        public PropertyBindInfo ForeColor

        {

            get => _foreColor;

            set => SetProperty(ref _foreColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));

        }



        public void CopyFrom(CheckBoxPropertyBindEditModel source)

        {

            if (source == null) return;
            base.CopyFrom(source);
            // ??????????????????????? 5 ???????
            IsChecked = source.IsChecked;
            Text = source.Text;
            BackColor = source.BackColor;
            BorderColor = source.BorderColor;
            ForeColor = source.ForeColor;
}

    }

}





