using Avalonia.Media;

using Avalonia.Threading;

using GF_Gereric;

using Griffins;

using GKG.Map.MapCell.Generic.GroupPanel;

using GKG.Map.MapCell.Generic.Control.Lable;

using GKG.Map.MapCell.Generic.RadioButton.MapOprtCellParamCfgView;

using GKG.Map.MapCell.Generic.RadioButton.View;

using GKG.Map.MapCell.Generic.RadioButton.ViewModel;

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



namespace GKG.Map.MapCell.Generic.RadioButton

{

    /// <summary>

    /// 单选框图元控件对象

    /// </summary>

    class MapCellRadioButtonCtlObj : ControlCellBase

    {

        #region 私有字段



        private RadioButtonView view;

        private RadioButtonViewModel radioButtonViewModel;

        private readonly ConcurrentDictionary<Guid, MapOprtCellID> _oprtCellIdByInstanceId = new();

        private MapObjID _mapCellID;

        private string _mapCellName;

        private bool _loadedPropertyEditFromBytes;





        #endregion



        #region 构造函数



        public MapCellRadioButtonCtlObj(MapObjID mapCellID, string mapCellName)

            : this(mapCellID, mapCellName, false) { }



        public MapCellRadioButtonCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)

        {

            PropertyEditModelBase = CreatePropertyModelEditBase();

            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();

            EventBindEditModel = CreateEventBindEditModel();

            _mapCellID = mapCellID;

            _mapCellName = mapCellName;

            base.SetID(mapCellID);

            base.SetName(mapCellName);

            view = new RadioButtonView();



            // 注册对象属性

            RegisterProperty(new MapObjPropertyInfo(nameof(RadioButtonPropertyModelEdit.BrushInfo), "画笔设置", MapCellPropDataType.Object_Json, RadioButtonBrushInfo.Object_ID, typeof(RadioButtonBrushInfo), false, true, new MapCellPropValue(RadioButtonBrushInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(RadioButtonPropertyModelEdit.AppearanceInfo), "外观设置", MapCellPropDataType.Object_Json, RadioButtonAppearanceInfo.Object_ID, typeof(RadioButtonAppearanceInfo), false, true, new MapCellPropValue(RadioButtonAppearanceInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(RadioButtonPropertyModelEdit.LayoutInfo), "布局设置", MapCellPropDataType.Object_Json, RadioButtonLayoutInfo.Object_ID, typeof(RadioButtonLayoutInfo), false, true, new MapCellPropValue(RadioButtonLayoutInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(RadioButtonPropertyModelEdit.CommonInfo), "公共设置", MapCellPropDataType.Object_Json, RadioButtonCommonInfo.Object_ID, typeof(RadioButtonCommonInfo), false, true, new MapCellPropValue(RadioButtonCommonInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(RadioButtonPropertyModelEdit.TextInfo), "文本设置", MapCellPropDataType.Object_Json, RadioButtonTextInfo.Object_ID, typeof(RadioButtonTextInfo), false, true, new MapCellPropValue(RadioButtonTextInfo.Default)));



            // 注册操作原子信息

            RegisterOprtCellInfo(new MapOprtCellInfo(RadioButtonMapOprtCellConst.BrushInfo_MapOprtCellID, "画笔设置操作原子", typeof(BrushInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(RadioButtonMapOprtCellConst.AppearanceInfo_MapOprtCellID, "外观设置操作原子", typeof(AppearanceInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(RadioButtonMapOprtCellConst.LayoutInfo_MapOprtCellID, "布局设置操作原子", typeof(LayoutInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(RadioButtonMapOprtCellConst.CommonInfo_MapOprtCellID, "公共设置操作原子", typeof(CommonInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(RadioButtonMapOprtCellConst.TextInfo_MapOprtCellID, "文本设置操作原子", typeof(TextInfoMapOprtCellParamCfgView)));



            // 注册操作信息

            RegisterOprtInfo(new MapOprtInfo(nameof(RadioButtonPropertyModelEdit.BrushInfo), "设置画笔", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = RadioButtonMapOprtCellConst.BrushInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(RadioButtonPropertyModelEdit.AppearanceInfo), "设置外观", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = RadioButtonMapOprtCellConst.AppearanceInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(RadioButtonPropertyModelEdit.LayoutInfo), "设置布局", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = RadioButtonMapOprtCellConst.LayoutInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(RadioButtonPropertyModelEdit.CommonInfo), "设置公共", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = RadioButtonMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(RadioButtonPropertyModelEdit.TextInfo), "设置文本", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = RadioButtonMapOprtCellConst.TextInfo_MapOprtCellID, CfgInfo = null } }));



            (this as IMapCellTypeBase).Name = ResourceA.RadioButton;

            radioButtonViewModel = new RadioButtonViewModel(designTime, RadioButtonPropertyModelEdit, OnEventTriggered);

            view.DataContext = radioButtonViewModel;



            // 订阅Info的PropertyChanged，触发操作原子

            RadioButtonPropertyModelEdit.BrushInfo.PropertyChanged += OnBrushInfoPropertyChanged;
            RadioButtonPropertyModelEdit.AppearanceInfo.PropertyChanged += OnAppearanceInfoPropertyChanged;
            RadioButtonPropertyModelEdit.LayoutInfo.PropertyChanged += OnLayoutInfoPropertyChanged;
            RadioButtonPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
            RadioButtonPropertyModelEdit.TextInfo.PropertyChanged += OnTextInfoPropertyChanged;





        }

        private void OnBrushInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(RadioButtonPropertyModelEdit.BrushInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnAppearanceInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(RadioButtonPropertyModelEdit.AppearanceInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnLayoutInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(RadioButtonPropertyModelEdit.LayoutInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(RadioButtonPropertyModelEdit.CommonInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnTextInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(RadioButtonPropertyModelEdit.TextInfo), "PropertyChanged", e?.PropertyName);
        }

        public override void OnDispose()
        {
            RadioButtonPropertyModelEdit.BrushInfo.PropertyChanged -= OnBrushInfoPropertyChanged;
            RadioButtonPropertyModelEdit.AppearanceInfo.PropertyChanged -= OnAppearanceInfoPropertyChanged;
            RadioButtonPropertyModelEdit.LayoutInfo.PropertyChanged -= OnLayoutInfoPropertyChanged;
            RadioButtonPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;
            RadioButtonPropertyModelEdit.TextInfo.PropertyChanged -= OnTextInfoPropertyChanged;

            view.DataContext = null;
            radioButtonViewModel?.Dispose();
            radioButtonViewModel = null;

            base.OnDispose();
        }



        #endregion



        #region 属性



        public RadioButtonPropertyModelEdit RadioButtonPropertyModelEdit => PropertyEditModelBase as RadioButtonPropertyModelEdit;



        #endregion



        #region 事件处理



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



        #endregion



        #region 公共方法



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

            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoadedRadioButton(propertyID, propertyVal))

            {

                return true;

            }

            RadioButtonPropertyModelEdit.IsRuning = isRuning;

            var ok = RadioButtonPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);

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

                    // ??????????????????????????????? CommonInfo?

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



        private bool IsDefaultOverwriteForLoadedRadioButton(string propertyID, MapCellPropValue propertyVal)

        {

            try

            {

                if (string.Compare(propertyID, nameof(RadioButtonPropertyModelEdit.BrushInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<RadioButtonBrushInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(RadioButtonPropertyModelEdit.BrushInfo);

                }

                if (string.Compare(propertyID, nameof(RadioButtonPropertyModelEdit.AppearanceInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<RadioButtonAppearanceInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(RadioButtonPropertyModelEdit.AppearanceInfo);

                }

                if (string.Compare(propertyID, nameof(RadioButtonPropertyModelEdit.LayoutInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<RadioButtonLayoutInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(RadioButtonPropertyModelEdit.LayoutInfo);

                }

                if (string.Compare(propertyID, nameof(RadioButtonPropertyModelEdit.CommonInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<RadioButtonCommonInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(RadioButtonPropertyModelEdit.CommonInfo);

                }

                if (string.Compare(propertyID, nameof(RadioButtonPropertyModelEdit.TextInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<RadioButtonTextInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(RadioButtonPropertyModelEdit.TextInfo);

                }

                return false;

            }

            catch { return false; }

        }



        private static bool IsDefault(RadioButtonBrushInfo? info)

        {

            if (info == null) return true;

            return string.Equals(info.BackColorStr ?? "", RadioButtonBrushInfo.Default.BackColorStr, StringComparison.OrdinalIgnoreCase)

                && string.Equals(info.BorderColorStr ?? "", RadioButtonBrushInfo.Default.BorderColorStr, StringComparison.OrdinalIgnoreCase)

                && string.Equals(info.ForeColorStr ?? "", RadioButtonBrushInfo.Default.ForeColorStr, StringComparison.OrdinalIgnoreCase);

        }



        private static bool IsDefault(RadioButtonAppearanceInfo? info)

        {

            if (info == null) return true;

            return Math.Abs(info.Opacity - RadioButtonAppearanceInfo.Default.Opacity) < 0.000001

                && Math.Abs(info.BorderThicknessLeft - RadioButtonAppearanceInfo.Default.BorderThicknessLeft) < 0.000001

                && Math.Abs(info.BorderThicknessTop - RadioButtonAppearanceInfo.Default.BorderThicknessTop) < 0.000001

                && Math.Abs(info.BorderThicknessRight - RadioButtonAppearanceInfo.Default.BorderThicknessRight) < 0.000001

                && Math.Abs(info.BorderThicknessBottom - RadioButtonAppearanceInfo.Default.BorderThicknessBottom) < 0.000001;

        }



        private static bool IsDefault(RadioButtonLayoutInfo? info)

        {

            if (info == null) return true;

            return info.HorizontalAlign == RadioButtonLayoutInfo.Default.HorizontalAlign

                && info.VerticalAlign == RadioButtonLayoutInfo.Default.VerticalAlign

                && Math.Abs(info.MarginTop - RadioButtonLayoutInfo.Default.MarginTop) < 0.000001

                && Math.Abs(info.MarginLeft - RadioButtonLayoutInfo.Default.MarginLeft) < 0.000001

                && Math.Abs(info.MarginBottom - RadioButtonLayoutInfo.Default.MarginBottom) < 0.000001

                && Math.Abs(info.MarginRight - RadioButtonLayoutInfo.Default.MarginRight) < 0.000001

                && Math.Abs(info.MinWidth - RadioButtonLayoutInfo.Default.MinWidth) < 0.000001

                && Math.Abs(info.MaxWidth - RadioButtonLayoutInfo.Default.MaxWidth) < 0.000001

                && Math.Abs(info.MinHeight - RadioButtonLayoutInfo.Default.MinHeight) < 0.000001

                && Math.Abs(info.MaxHeight - RadioButtonLayoutInfo.Default.MaxHeight) < 0.000001;

        }



        private static bool IsDefault(RadioButtonCommonInfo? info)

        {

            if (info == null) return true;

            return string.Equals(info.Text ?? "", RadioButtonCommonInfo.Default.Text ?? "", StringComparison.Ordinal)

                && string.Equals(info.GroupName ?? "", RadioButtonCommonInfo.Default.GroupName ?? "", StringComparison.Ordinal)

                && info.IsChecked == RadioButtonCommonInfo.Default.IsChecked

                && info.IsThreeState == RadioButtonCommonInfo.Default.IsThreeState

                && info.CursorType == RadioButtonCommonInfo.Default.CursorType

                && info.IsEnabled == RadioButtonCommonInfo.Default.IsEnabled

                && string.Equals(info.ToolTip ?? "", RadioButtonCommonInfo.Default.ToolTip ?? "", StringComparison.Ordinal);

        }



        private static bool IsDefault(RadioButtonTextInfo? info)

        {

            if (info == null) return true;

            return info.FontFamilyType == RadioButtonTextInfo.Default.FontFamilyType

                && string.Equals(info.FontColorStr ?? "", RadioButtonTextInfo.Default.FontColorStr, StringComparison.OrdinalIgnoreCase)

                && Math.Abs(info.FontSize - RadioButtonTextInfo.Default.FontSize) < 0.000001

                && info.IsItalic == RadioButtonTextInfo.Default.IsItalic

                && info.IsBold == RadioButtonTextInfo.Default.IsBold;

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



            if (string.Equals(oprtId, nameof(RadioButtonCommonInfo.Text), StringComparison.Ordinal)

                || string.Equals(oprtId, nameof(RadioButtonCommonInfo.IsChecked), StringComparison.Ordinal))

            {

                normalizedOprtId = nameof(RadioButtonPropertyModelEdit.CommonInfo);

                oprtCellId = RadioButtonMapOprtCellConst.CommonInfo_MapOprtCellID;

                return true;

            }

            if (string.Equals(oprtId, nameof(RadioButtonPropertyModelEdit.BrushInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(RadioButtonPropertyModelEdit.BrushInfo); oprtCellId = RadioButtonMapOprtCellConst.BrushInfo_MapOprtCellID; return true; }

            if (string.Equals(oprtId, nameof(RadioButtonPropertyModelEdit.AppearanceInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(RadioButtonPropertyModelEdit.AppearanceInfo); oprtCellId = RadioButtonMapOprtCellConst.AppearanceInfo_MapOprtCellID; return true; }

            if (string.Equals(oprtId, nameof(RadioButtonPropertyModelEdit.LayoutInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(RadioButtonPropertyModelEdit.LayoutInfo); oprtCellId = RadioButtonMapOprtCellConst.LayoutInfo_MapOprtCellID; return true; }

            if (string.Equals(oprtId, nameof(RadioButtonPropertyModelEdit.CommonInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(RadioButtonPropertyModelEdit.CommonInfo); oprtCellId = RadioButtonMapOprtCellConst.CommonInfo_MapOprtCellID; return true; }

            if (string.Equals(oprtId, nameof(RadioButtonPropertyModelEdit.TextInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(RadioButtonPropertyModelEdit.TextInfo); oprtCellId = RadioButtonMapOprtCellConst.TextInfo_MapOprtCellID; return true; }

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



        #endregion



        #region 序列化方法



        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)

        {

            base.OnReadDrawInfoFromBytes(br);

            string propertyEditJson = br.ReadString("PropertyEditModelBase");

            if (!string.IsNullOrEmpty(propertyEditJson))

            {

                try

                {

                    var propertyEditModelBase = JsonObjConvert.FromJSon<RadioButtonPropertyModelEdit>(propertyEditJson);

                    if (propertyEditModelBase != null)

                    {

                        (PropertyEditModelBase as RadioButtonPropertyModelEdit).CopyFrom(propertyEditModelBase);

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

                    var propertyBindEditModelBase = JsonObjConvert.FromJSon<RadioButtonPropertyBindEditModel>(propertyBindJson);

                    if (propertyBindEditModelBase != null)

                        (PropertyBindEditModelBase as RadioButtonPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);

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



        #endregion



        #region 重写方法



        protected override void OnCopyFrom(ControlCellBase source)

        {

            MapCellRadioButtonCtlObj mapCellRadioButtonCtlObj = (source as MapCellRadioButtonCtlObj);

            base._CopyFrom(mapCellRadioButtonCtlObj);

            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);

            (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);

            EventBindEditModel.CopyFrom(source.EventBindEditModel);

        }



        protected override void OnInit()

        {

            base.OnInit();

        }

        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => radioButtonViewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new RadioButtonPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new RadioButtonPropertyBindEditModel();

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

        public override string ToString() => "单选框";





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



        #endregion



        #region 操作原子执行



        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)

        {

            if (mapOprtCellInstInfo.OprtCellID == RadioButtonMapOprtCellConst.BrushInfo_MapOprtCellID)

                return ExecuteOprtCell<BrushInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == RadioButtonMapOprtCellConst.AppearanceInfo_MapOprtCellID)

                return ExecuteOprtCell<AppearanceInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == RadioButtonMapOprtCellConst.LayoutInfo_MapOprtCellID)

                return ExecuteOprtCell<LayoutInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == RadioButtonMapOprtCellConst.CommonInfo_MapOprtCellID)

                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == RadioButtonMapOprtCellConst.TextInfo_MapOprtCellID)

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



        #endregion



        #region 操作原子执行对象



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

                if (callBack.GetMapCellVMObjInstance() is RadioButtonViewModel vm)

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

                    var val = callBack.GetMapCellPropValue(nameof(RadioButtonPropertyModelEdit.BrushInfo));

                    if (val != null)

                    {

                        try

                        {

                            var brushInfo = DeserializeObject<RadioButtonBrushInfo>(val);

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

                if (callBack.GetMapCellVMObjInstance() is RadioButtonViewModel vm)

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

                    var val = callBack.GetMapCellPropValue(nameof(RadioButtonPropertyModelEdit.AppearanceInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<RadioButtonAppearanceInfo>(val);

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

                if (callBack.GetMapCellVMObjInstance() is RadioButtonViewModel vm)

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

                                // 宽高主数据统一落到父类 Width/Height，避免单选框继续维护额外尺寸中间态。
                                PostToUI(() => { vm.HorizontalAlign = param.HorizontalAlign; vm.VerticalAlign = param.VerticalAlign; vm.MarginTop = mt; vm.MarginLeft = ml; vm.MarginBottom = mb; vm.MarginRight = mr; vm.MinWidth = minW; vm.MaxWidth = maxW; vm.MinHeight = minH; vm.MaxHeight = maxH; });

                                return;

                            }

                        }

                        catch { }

                    }

                    var val = callBack.GetMapCellPropValue(nameof(RadioButtonPropertyModelEdit.LayoutInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<RadioButtonLayoutInfo>(val);

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

                if (callBack.GetMapCellVMObjInstance() is RadioButtonViewModel vm)

                {

                    if (cfg != null && cfg.Length > 0)

                    {

                        try

                        {

                            var param = JsonSerializer.Deserialize<CommonInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));

                            if (param != null)

                            {

                                PostToUI(() => { vm.Text = param.Text; vm.GroupName = param.GroupName; vm.IsChecked = param.IsChecked; vm.IsThreeState = param.IsThreeState; vm.CursorType = param.CursorType; vm.IsEnabled = param.IsEnabled; vm.ToolTip = param.ToolTip; });

                                return;

                            }

                        }

                        catch { }

                    }

                    var val = callBack.GetMapCellPropValue(nameof(RadioButtonPropertyModelEdit.CommonInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<RadioButtonCommonInfo>(val);

                            PostToUI(() => { vm.Text = info.Text; vm.GroupName = info.GroupName; vm.IsChecked = info.IsChecked; vm.IsThreeState = info.IsThreeState; vm.CursorType = info.CursorType; vm.IsEnabled = info.IsEnabled; vm.ToolTip = info.ToolTip; });

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

                if (callBack.GetMapCellVMObjInstance() is RadioButtonViewModel vm)

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

                    var val = callBack.GetMapCellPropValue(nameof(RadioButtonPropertyModelEdit.TextInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<RadioButtonTextInfo>(val);

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



        #endregion

    }



    /// <summary>

    /// 单选框属性编辑模型对象

    /// </summary>

    [Serializable]

    [MapPropertyOrder]

    [CategoryPriority("画笔", 1)]

    [CategoryPriority("外观", 2)]

    [CategoryPriority("布局", 3)]

    [CategoryPriority("公共", 4)]

    [CategoryPriority("文本", 5)]

    public class RadioButtonPropertyModelEdit : ControlCellPropertyModelEdit

    {

        #region 对象属性



        private RadioButtonBrushInfo _brushInfo = new RadioButtonBrushInfo();

        [DisplayName("画笔设置")]

        [Category("画笔")]

        [PropertySortOrder(1)]

        public RadioButtonBrushInfo BrushInfo

        {

            get => _brushInfo;

            set => SetProperty(ref _brushInfo, value);

        }



        private RadioButtonAppearanceInfo _appearanceInfo = new RadioButtonAppearanceInfo();

        [DisplayName("外观设置")]

        [Category("外观")]

        [PropertySortOrder(1)]

        public RadioButtonAppearanceInfo AppearanceInfo

        {

            get => _appearanceInfo;

            set => SetProperty(ref _appearanceInfo, value);

        }



        private RadioButtonLayoutInfo _layoutInfo = new RadioButtonLayoutInfo();

        [DisplayName("布局设置")]

        [Category("布局")]

        [PropertySortOrder(1)]

        public RadioButtonLayoutInfo LayoutInfo

        {

            get => _layoutInfo;

            set => SetProperty(ref _layoutInfo, value);

        }



        private RadioButtonCommonInfo _commonInfo = new RadioButtonCommonInfo();

        [DisplayName("公共设置")]

        [Category("公共")]

        [PropertySortOrder(1)]

        public RadioButtonCommonInfo CommonInfo

        {

            get => _commonInfo;

            set => SetProperty(ref _commonInfo, value);

        }



        private RadioButtonTextInfo _textInfo = new RadioButtonTextInfo();

        [DisplayName("文本设置")]

        [Category("文本")]

        [PropertySortOrder(1)]

        public RadioButtonTextInfo TextInfo

        {

            get => _textInfo;

            set => SetProperty(ref _textInfo, value);

        }



        #endregion



        #region SetPropertyValue 方法



        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)

        {

            if (string.Compare(propertyID, nameof(RadioButtonCommonInfo.IsChecked)) == 0)

            {

                _commonInfo ??= new RadioButtonCommonInfo();

                _commonInfo.IsChecked = propertyVal != null ? (bool?)propertyVal.ToPrimitiveValue<bool>() : RadioButtonCommonInfo.Default.IsChecked;

                RaisePropertyChanged(nameof(CommonInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(RadioButtonCommonInfo.Text)) == 0)

            {

                _commonInfo ??= new RadioButtonCommonInfo();

                _commonInfo.Text = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : RadioButtonCommonInfo.Default.Text;

                RaisePropertyChanged(nameof(CommonInfo));

                return true;

            }


            if (string.Compare(propertyID, nameof(RadioButtonCommonInfo.IsEnabled)) == 0)

            {

                _commonInfo ??= new RadioButtonCommonInfo();

                _commonInfo.IsEnabled = propertyVal != null ? propertyVal.ToPrimitiveValue<bool>() : RadioButtonCommonInfo.Default.IsEnabled;

                RaisePropertyChanged(nameof(CommonInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(RadioButtonBrushInfo.ForeColor)) == 0)

            {

                _brushInfo ??= new RadioButtonBrushInfo();

                if (propertyVal != null)

                {

                    var colorStr = propertyVal.ToPrimitiveValue<string>();

                    _brushInfo.ForeColorStr = Color.Parse(colorStr).ToColorString();

                }

                else

                {

                    _brushInfo.ForeColorStr = RadioButtonBrushInfo.Default.ForeColorStr;

                }

                RaisePropertyChanged(nameof(BrushInfo));

                return true;

            }



            if (string.Compare(propertyID, nameof(BrushInfo)) == 0)

            {

                var src = propertyVal != null ? DeserializeObject<RadioButtonBrushInfo>(propertyVal) : new RadioButtonBrushInfo();

                _brushInfo ??= new RadioButtonBrushInfo();

                _brushInfo.BackColorStr = src.BackColorStr;

                _brushInfo.BorderColorStr = src.BorderColorStr;

                _brushInfo.ForeColorStr = src.ForeColorStr;

                RaisePropertyChanged(nameof(BrushInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(AppearanceInfo)) == 0)

            {

                var src = propertyVal != null ? DeserializeObject<RadioButtonAppearanceInfo>(propertyVal) : new RadioButtonAppearanceInfo();

                _appearanceInfo ??= new RadioButtonAppearanceInfo();

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

                var src = propertyVal != null ? DeserializeObject<RadioButtonLayoutInfo>(propertyVal) : new RadioButtonLayoutInfo();

                _layoutInfo ??= new RadioButtonLayoutInfo();

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

                var src = propertyVal != null ? DeserializeObject<RadioButtonCommonInfo>(propertyVal) : new RadioButtonCommonInfo();

                _commonInfo ??= new RadioButtonCommonInfo();

                _commonInfo.Text = src.Text;

                _commonInfo.GroupName = src.GroupName;

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

                var src = propertyVal != null ? DeserializeObject<RadioButtonTextInfo>(propertyVal) : new RadioButtonTextInfo();

                _textInfo ??= new RadioButtonTextInfo();

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



        #endregion



        #region CopyFrom 方法



        public void CopyFrom(RadioButtonPropertyModelEdit source)

        {

            if (source == null) return;



            base.CopyFrom(source);



            // 复制画笔信息

            _brushInfo ??= new RadioButtonBrushInfo();

            _brushInfo.BackColorStr = source.BrushInfo?.BackColorStr;

            _brushInfo.BorderColorStr = source.BrushInfo?.BorderColorStr;

            _brushInfo.ForeColorStr = source.BrushInfo?.ForeColorStr;

            RaisePropertyChanged(nameof(BrushInfo));



            // 复制外观信息

            _appearanceInfo ??= new RadioButtonAppearanceInfo();

            _appearanceInfo.Opacity = source.AppearanceInfo?.Opacity ?? 1.0;

            _appearanceInfo.BorderThicknessLeft = source.AppearanceInfo?.BorderThicknessLeft ?? 1;

            _appearanceInfo.BorderThicknessTop = source.AppearanceInfo?.BorderThicknessTop ?? 1;

            _appearanceInfo.BorderThicknessRight = source.AppearanceInfo?.BorderThicknessRight ?? 1;

            _appearanceInfo.BorderThicknessBottom = source.AppearanceInfo?.BorderThicknessBottom ?? 1;

            RaisePropertyChanged(nameof(AppearanceInfo));



            // 复制布局信息

            _layoutInfo ??= new RadioButtonLayoutInfo();

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



            // 复制公共信息

            _commonInfo ??= new RadioButtonCommonInfo();

            _commonInfo.Text = source.CommonInfo?.Text ?? "单选框";

            _commonInfo.GroupName = source.CommonInfo?.GroupName ?? "";

            _commonInfo.IsChecked = source.CommonInfo != null ? source.CommonInfo.IsChecked : false;

            _commonInfo.IsThreeState = source.CommonInfo?.IsThreeState ?? false;

            _commonInfo.CursorType = source.CommonInfo?.CursorType ?? CursorType.Arrow;

            _commonInfo.IsEnabled = source.CommonInfo?.IsEnabled ?? true;

            _commonInfo.ToolTip = source.CommonInfo?.ToolTip ?? "";

            RaisePropertyChanged(nameof(CommonInfo));



            // 复制文本信息

            _textInfo ??= new RadioButtonTextInfo();

            _textInfo.FontFamilyType = source.TextInfo?.FontFamilyType ?? FontFamilyType.MicrosoftYaHei;

            _textInfo.FontColorStr = source.TextInfo?.FontColorStr;

            _textInfo.FontSize = source.TextInfo?.FontSize ?? 14;

            _textInfo.IsItalic = source.TextInfo?.IsItalic ?? false;

            _textInfo.IsBold = source.TextInfo?.IsBold ?? false;

            RaisePropertyChanged(nameof(TextInfo));

        }



        #endregion

    }



    /// <summary>

    /// 单选框属性绑定编辑模型

    /// </summary>

    [Serializable]

    [MapPropertyOrder]

    [CategoryPriority("绑定信息", 1)]

    public class RadioButtonPropertyBindEditModel : ControlCellPropertyBindEditModel

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


        public void CopyFrom(RadioButtonPropertyBindEditModel source)
        {

            if (source == null) return;
            base.CopyFrom(source);
            IsChecked = source.IsChecked;
            Text = source.Text;
}

    }

}





