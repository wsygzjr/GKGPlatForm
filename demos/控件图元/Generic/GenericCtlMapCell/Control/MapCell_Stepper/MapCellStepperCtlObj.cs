using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.GroupPanel;
using GKG.Map.MapCell.Generic.Control.Lable;
using GKG.Map.MapCell.Generic.Stepper.MapOprtCellParamCfgView;
using GKG.Map.MapCell.Generic.Stepper.Objects;
using GKG.Map.MapCell.Generic.Stepper.ViewModels;
using GKG.Map.MapCell.Generic.Stepper.Views;
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

namespace GKG.Map.MapCell.Generic.Stepper
{
    /// <summary>
    /// 步进器图元控件对象
    /// </summary>
    class MapCellStepperCtlObj : ControlCellBase
    {
        #region 私有字段

        private StepperView view;
        private StepperViewModel stepperViewModel;
        private readonly ConcurrentDictionary<Guid, MapOprtCellID> _oprtCellIdByInstanceId = new();
        private MapObjID _mapCellID;
        private string _mapCellName;
        private bool _loadedPropertyEditFromBytes;


        #endregion

        #region 构造函数

        public MapCellStepperCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false) { }

        public MapCellStepperCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            _mapCellID = mapCellID;
            _mapCellName = mapCellName;
            base.SetID(mapCellID);
            base.SetName(mapCellName);
            view = new StepperView();

            // 注册对象属性
            RegisterProperty(new MapObjPropertyInfo(nameof(StepperPropertyModelEdit.BrushInfo), ResourceA.Stepper_BrushInfo, MapCellPropDataType.Object_Json, StepperBrushInfo.Object_ID, typeof(StepperBrushInfo), false, true, new MapCellPropValue(StepperBrushInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(StepperPropertyModelEdit.AppearanceInfo), ResourceA.Stepper_AppearanceInfo, MapCellPropDataType.Object_Json, StepperAppearanceInfo.Object_ID, typeof(StepperAppearanceInfo), false, true, new MapCellPropValue(StepperAppearanceInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(StepperPropertyModelEdit.LayoutInfo), ResourceA.Stepper_LayoutInfo, MapCellPropDataType.Object_Json, StepperLayoutInfo.Object_ID, typeof(StepperLayoutInfo), false, true, new MapCellPropValue(StepperLayoutInfo.Default)));
                RegisterProperty(new MapObjPropertyInfo(nameof(StepperPropertyModelEdit.CommonInfo), ResourceA.Stepper_CommonInfo, MapCellPropDataType.Object_Json, StepperCommonInfo.Object_ID, typeof(StepperCommonInfo), false, true, new MapCellPropValue(StepperCommonInfo.Default)));
                RegisterProperty(new MapObjPropertyInfo(nameof(StepperPropertyModelEdit.TextInfo), ResourceA.Stepper_TextInfo, MapCellPropDataType.Object_Json, StepperTextInfo.Object_ID, typeof(StepperTextInfo), false, true, new MapCellPropValue(StepperTextInfo.Default)));

            // 注册操作原子信息
            RegisterOprtCellInfo(new MapOprtCellInfo(StepperMapOprtCellConst.BrushInfo_MapOprtCellID, ResourceA.Stepper_BrushInfo_MapOprtCellName, typeof(BrushInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(StepperMapOprtCellConst.AppearanceInfo_MapOprtCellID, ResourceA.Stepper_AppearanceInfo_MapOprtCellName, typeof(AppearanceInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(StepperMapOprtCellConst.LayoutInfo_MapOprtCellID, ResourceA.Stepper_LayoutInfo_MapOprtCellName, typeof(LayoutInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(StepperMapOprtCellConst.CommonInfo_MapOprtCellID, ResourceA.Stepper_CommonInfo_MapOprtCellName, typeof(CommonInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(StepperMapOprtCellConst.TextInfo_MapOprtCellID, ResourceA.Stepper_TextInfo_MapOprtCellName, typeof(TextInfoMapOprtCellParamCfgView)));

            // 注册操作信息
            RegisterOprtInfo(new MapOprtInfo(nameof(StepperPropertyModelEdit.BrushInfo), ResourceA.Stepper_BrushInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = StepperMapOprtCellConst.BrushInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(StepperPropertyModelEdit.AppearanceInfo), ResourceA.Stepper_AppearanceInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = StepperMapOprtCellConst.AppearanceInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(StepperPropertyModelEdit.LayoutInfo), ResourceA.Stepper_LayoutInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = StepperMapOprtCellConst.LayoutInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(StepperPropertyModelEdit.CommonInfo), ResourceA.Stepper_CommonInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = StepperMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(StepperPropertyModelEdit.TextInfo), ResourceA.Stepper_TextInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = StepperMapOprtCellConst.TextInfo_MapOprtCellID, CfgInfo = null } }));

            (this as IMapCellTypeBase).Name = ResourceA.Stepper; // 使用资源名称，保持属性面板显示一致。
            stepperViewModel = new StepperViewModel(designTime, StepperPropertyModelEdit);
            view.DataContext = stepperViewModel;

            // 订阅 Info 对象的 PropertyChanged，触发对应操作原子。
            StepperPropertyModelEdit.BrushInfo.PropertyChanged += OnBrushInfoPropertyChanged;
            StepperPropertyModelEdit.AppearanceInfo.PropertyChanged += OnAppearanceInfoPropertyChanged;
            StepperPropertyModelEdit.LayoutInfo.PropertyChanged += OnLayoutInfoPropertyChanged;
            StepperPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
            StepperPropertyModelEdit.TextInfo.PropertyChanged += OnTextInfoPropertyChanged;


        }

        private void OnBrushInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(StepperPropertyModelEdit.BrushInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnAppearanceInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(StepperPropertyModelEdit.AppearanceInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnLayoutInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(StepperPropertyModelEdit.LayoutInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(StepperPropertyModelEdit.CommonInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnTextInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(StepperPropertyModelEdit.TextInfo), "PropertyChanged", e?.PropertyName);
        }

        public override void OnDispose()
        {
            StepperPropertyModelEdit.BrushInfo.PropertyChanged -= OnBrushInfoPropertyChanged;
            StepperPropertyModelEdit.AppearanceInfo.PropertyChanged -= OnAppearanceInfoPropertyChanged;
            StepperPropertyModelEdit.LayoutInfo.PropertyChanged -= OnLayoutInfoPropertyChanged;
            StepperPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;
            StepperPropertyModelEdit.TextInfo.PropertyChanged -= OnTextInfoPropertyChanged;

            view.DataContext = null;
            stepperViewModel?.Dispose();
            stepperViewModel = null;

            base.OnDispose();
        }

        #endregion

        #region 属性

        public StepperPropertyModelEdit StepperPropertyModelEdit => PropertyEditModelBase as StepperPropertyModelEdit;

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
            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoadedStepper(propertyID, propertyVal))
            {
                return true;
            }
            StepperPropertyModelEdit.IsRuning = isRuning;
            var ok = StepperPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
            if (ok)
            {
                ExecuteOprtByPropertyId(propertyID, "SetPropertyValue", null);
            }
return ok;
        }
        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            if (StepperPropertyModelEdit.IsRuning)
                return;
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

        private bool IsDefaultOverwriteForLoadedStepper(string propertyID, MapCellPropValue propertyVal)
        {
            try
            {
                if (string.Compare(propertyID, nameof(StepperPropertyModelEdit.BrushInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<StepperBrushInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(StepperPropertyModelEdit.BrushInfo);
                }
                if (string.Compare(propertyID, nameof(StepperPropertyModelEdit.AppearanceInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<StepperAppearanceInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(StepperPropertyModelEdit.AppearanceInfo);
                }
                if (string.Compare(propertyID, nameof(StepperPropertyModelEdit.LayoutInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<StepperLayoutInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(StepperPropertyModelEdit.LayoutInfo);
                }
                if (string.Compare(propertyID, nameof(StepperPropertyModelEdit.CommonInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<StepperCommonInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(StepperPropertyModelEdit.CommonInfo);
                }
                if (string.Compare(propertyID, nameof(StepperPropertyModelEdit.TextInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<StepperTextInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(StepperPropertyModelEdit.TextInfo);
                }
                return false;
            }
            catch { return false; }
        }

        private static bool IsDefault(StepperBrushInfo? info)
        {
            if (info == null) return true;
            return string.Equals(info.BackColorStr ?? "", StepperBrushInfo.Default.BackColorStr, StringComparison.OrdinalIgnoreCase)
                && string.Equals(info.BorderColorStr ?? "", StepperBrushInfo.Default.BorderColorStr, StringComparison.OrdinalIgnoreCase)
                && string.Equals(info.ForeColorStr ?? "", StepperBrushInfo.Default.ForeColorStr, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsDefault(StepperAppearanceInfo? info)
        {
            if (info == null) return true;
            return Math.Abs(info.Opacity - StepperAppearanceInfo.Default.Opacity) < 0.000001
                && Math.Abs(info.BorderThicknessLeft - StepperAppearanceInfo.Default.BorderThicknessLeft) < 0.000001
                && Math.Abs(info.BorderThicknessTop - StepperAppearanceInfo.Default.BorderThicknessTop) < 0.000001
                && Math.Abs(info.BorderThicknessRight - StepperAppearanceInfo.Default.BorderThicknessRight) < 0.000001
                && Math.Abs(info.BorderThicknessBottom - StepperAppearanceInfo.Default.BorderThicknessBottom) < 0.000001;
        }

        private static bool IsDefault(StepperLayoutInfo? info)
        {
            if (info == null) return true;
            return info.HorizontalAlign == StepperLayoutInfo.Default.HorizontalAlign
                && info.VerticalAlign == StepperLayoutInfo.Default.VerticalAlign
                && Math.Abs(info.MarginTop - StepperLayoutInfo.Default.MarginTop) < 0.000001
                && Math.Abs(info.MarginLeft - StepperLayoutInfo.Default.MarginLeft) < 0.000001
                && Math.Abs(info.MarginBottom - StepperLayoutInfo.Default.MarginBottom) < 0.000001
                && Math.Abs(info.MarginRight - StepperLayoutInfo.Default.MarginRight) < 0.000001
                && Math.Abs(info.MinWidth - StepperLayoutInfo.Default.MinWidth) < 0.000001
                && Math.Abs(info.MaxWidth - StepperLayoutInfo.Default.MaxWidth) < 0.000001
                && Math.Abs(info.MinHeight - StepperLayoutInfo.Default.MinHeight) < 0.000001
                && Math.Abs(info.MaxHeight - StepperLayoutInfo.Default.MaxHeight) < 0.000001;
        }

        private static bool IsDefault(StepperCommonInfo? info)
        {
            if (info == null) return true;
            return string.Equals(info.LabelName ?? "", StepperCommonInfo.Default.LabelName ?? "", StringComparison.Ordinal)
                && info.Value == StepperCommonInfo.Default.Value
                && info.Minimum == StepperCommonInfo.Default.Minimum
                && info.Maximum == StepperCommonInfo.Default.Maximum
                && info.Increment == StepperCommonInfo.Default.Increment
                && info.DecimalPlaces == StepperCommonInfo.Default.DecimalPlaces
                && info.IsEnabled == StepperCommonInfo.Default.IsEnabled
                && string.Equals(info.ToolTip ?? "", StepperCommonInfo.Default.ToolTip ?? "", StringComparison.Ordinal);
        }

        private static bool IsDefault(StepperTextInfo? info)
        {
            if (info == null) return true;
            return info.FontFamilyType == StepperTextInfo.Default.FontFamilyType
                && string.Equals(info.FontColorStr ?? "", StepperTextInfo.Default.FontColorStr, StringComparison.OrdinalIgnoreCase)
                && Math.Abs(info.FontSize - StepperTextInfo.Default.FontSize) < 0.000001
                && info.IsItalic == StepperTextInfo.Default.IsItalic
                && info.IsBold == StepperTextInfo.Default.IsBold;
        }

        private void ExecuteOprtByPropertyId(string propertyID, string trigger, string? changedProp)
        {
            if (string.IsNullOrWhiteSpace(propertyID)) return;

            var normalized = propertyID;
            var dot = normalized.IndexOf('.');
            if (dot > 0) normalized = normalized.Substring(0, dot);

            if (!TryGetPrimaryOprtCellId(normalized, out var primaryOprtCellId)) return;

            TryExecuteOprtInfoById(normalized, primaryOprtCellId);
        }

        private static bool TryGetPrimaryOprtCellId(string oprtId, out MapOprtCellID oprtCellId)
        {
            oprtCellId = default;
            if (string.Equals(oprtId, nameof(StepperPropertyModelEdit.BrushInfo), StringComparison.Ordinal))
            { oprtCellId = StepperMapOprtCellConst.BrushInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(StepperPropertyModelEdit.AppearanceInfo), StringComparison.Ordinal))
            { oprtCellId = StepperMapOprtCellConst.AppearanceInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(StepperPropertyModelEdit.LayoutInfo), StringComparison.Ordinal))
            { oprtCellId = StepperMapOprtCellConst.LayoutInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(StepperPropertyModelEdit.CommonInfo), StringComparison.Ordinal))
            { oprtCellId = StepperMapOprtCellConst.CommonInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(StepperPropertyModelEdit.TextInfo), StringComparison.Ordinal))
            { oprtCellId = StepperMapOprtCellConst.TextInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(StepperPropertyModelEdit.Width), StringComparison.Ordinal))
            { oprtCellId = StepperMapOprtCellConst.Width_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(StepperPropertyModelEdit.Height), StringComparison.Ordinal))
            { oprtCellId = StepperMapOprtCellConst.Height_MapOprtCellID; return true; }
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
                    var propertyEditModelBase = JsonObjConvert.FromJSon<StepperPropertyModelEdit>(propertyEditJson);
                    if (propertyEditModelBase != null)
                    {
                        (PropertyEditModelBase as StepperPropertyModelEdit).CopyFrom(propertyEditModelBase);
                        _loadedPropertyEditFromBytes = true;

                        try
                        {
                            var self = PropertyEditModelBase as StepperPropertyModelEdit;
                            if (self?.LayoutInfo != null && IsLegacyDefaultLayout(self.LayoutInfo))
                            {
                                // 旧页面若仍命中历史默认布局，则把父类宽度迁回步进器旧默认宽度。
                                self.Width = 135;
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }

            string propertyBindJson = br.ReadString("PropertyBindEditModelBase");
            if (!string.IsNullOrEmpty(propertyBindJson))
            {
                try
                {
                    var propertyBindEditModelBase = JsonObjConvert.FromJSon<StepperPropertyBindEditModel>(propertyBindJson);
                    if (propertyBindEditModelBase != null)
                        (PropertyBindEditModelBase as StepperPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
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

        private static bool IsLegacyDefaultLayout(StepperLayoutInfo info)
        {
            if (info == null)
                return false;

            const double eps = 0.000001;
            return info.HorizontalAlign == HorizontalAlignType.Center
                && info.VerticalAlign == VerticalAlignType.Center
                && Math.Abs(info.MarginTop) < eps
                && Math.Abs(info.MarginLeft) < eps
                && Math.Abs(info.MarginBottom) < eps
                && Math.Abs(info.MarginRight) < eps
                && Math.Abs(info.MinWidth) < eps
                && Math.Abs(info.MaxWidth - 10000) < eps
                && Math.Abs(info.MinHeight) < eps
                && Math.Abs(info.MaxHeight - 10000) < eps;
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
        public override MapCellPropValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null;
        }
        protected override void OnCopyFrom(ControlCellBase source)
        {
            MapCellStepperCtlObj mapCellStepperCtlObj = (source as MapCellStepperCtlObj);
            base._CopyFrom(mapCellStepperCtlObj);
            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
            if (PropertyBindEditModelBase is StepperPropertyBindEditModel selfBind && source.PropertyBindEditModelBase is StepperPropertyBindEditModel srcBind)
                selfBind.CopyFrom(srcBind);
            else
                (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel.CopyFrom(source.EventBindEditModel);
        }

        protected override void OnInit()
        {
            base.OnInit();
        }
        protected override object OnGetView() => view;
        protected override object OnGetViewModel() => stepperViewModel;
        public override PropertyEditModelBase CreatePropertyModelEditBase() => new StepperPropertyModelEdit();
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new StepperPropertyBindEditModel();
        public override EventBindEditModel CreateEventBindEditModel() => new EventBindEditModel() { EventCmdInfos = new BindingList<EventCmdInfo>() };
        public override void OnZoomChanged() { }
        public override string ToString() => "步进器";


        private static Dictionary<string, List<string>> GetBindMapCellProperty(PropertyBindEditModelBase propertyBindEditModelBase)
        {
            var valueToPropertyIdsDict = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            if (propertyBindEditModelBase == null)
                return valueToPropertyIdsDict;

            var modelType = propertyBindEditModelBase.GetType();
            var modelProperties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in modelProperties)
            {
                if (prop.GetIndexParameters().Length > 0)
                    continue;
                if (prop.PropertyType != typeof(string))
                    continue;

                try
                {
                    var hasAttr = prop.GetCustomAttributes(typeof(BindMPPropertyIDAttribute), true)?.Length > 0;
                    if (!hasAttr)
                        continue;
                }
                catch
                {
                    continue;
                }

                var propertyId = prop.Name;
                var propertyValue = prop.GetValue(propertyBindEditModelBase) as string;
                if (string.IsNullOrWhiteSpace(propertyValue))
                    continue;

                if (!valueToPropertyIdsDict.TryGetValue(propertyValue, out var list))
                {
                    list = new List<string>();
                    valueToPropertyIdsDict.Add(propertyValue, list);
                }
                list.Add(propertyId);
            }
            return valueToPropertyIdsDict;
        }

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
            if (mapOprtCellInstInfo.OprtCellID == StepperMapOprtCellConst.BrushInfo_MapOprtCellID)
                return ExecuteOprtCell<BrushInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == StepperMapOprtCellConst.AppearanceInfo_MapOprtCellID)
            {
                var ok = ExecuteOprtCell<AppearanceInfoMapOprtCellExector>(mapOprtCellInstInfo);
                if (ok && mapOprtCellInstInfo.CfgInfo != null && mapOprtCellInstInfo.CfgInfo.Length > 0)
                {
                     try
                     {
                         var param = JsonSerializer.Deserialize<AppearanceInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(mapOprtCellInstInfo.CfgInfo));
                         if (param != null)
                         {
            // 同步属性编辑模型，便于保存。
                             if(double.TryParse(param.Opacity, out var opacity)) StepperPropertyModelEdit.AppearanceInfo.Opacity = opacity;
                             if(double.TryParse(param.BorderThicknessLeft, out var l)) StepperPropertyModelEdit.AppearanceInfo.BorderThicknessLeft = l;
                             if(double.TryParse(param.BorderThicknessTop, out var t)) StepperPropertyModelEdit.AppearanceInfo.BorderThicknessTop = t;
                             if(double.TryParse(param.BorderThicknessRight, out var r)) StepperPropertyModelEdit.AppearanceInfo.BorderThicknessRight = r;
                             if(double.TryParse(param.BorderThicknessBottom, out var b)) StepperPropertyModelEdit.AppearanceInfo.BorderThicknessBottom = b;
                         }
                     }
                     catch { }
                }
                return ok;
            }
            if (mapOprtCellInstInfo.OprtCellID == StepperMapOprtCellConst.LayoutInfo_MapOprtCellID)
                return ExecuteOprtCell<LayoutInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == StepperMapOprtCellConst.CommonInfo_MapOprtCellID)
                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == StepperMapOprtCellConst.TextInfo_MapOprtCellID)
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
                if (callBack.GetMapCellVMObjInstance() is StepperViewModel vm)
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
                    var val = callBack.GetMapCellPropValue(nameof(StepperPropertyModelEdit.BrushInfo));
                    if (val != null)
                    {
                        try
                        {
                            var brushInfo = DeserializeObject<StepperBrushInfo>(val);
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
                if (callBack.GetMapCellVMObjInstance() is StepperViewModel vm)
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
                    var val = callBack.GetMapCellPropValue(nameof(StepperPropertyModelEdit.AppearanceInfo));
                    if (val != null)
                    {
                        try
                        {
                            var info = DeserializeObject<StepperAppearanceInfo>(val);
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
                if (callBack.GetMapCellVMObjInstance() is StepperViewModel vm)
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
                                // 宽高主数据统一落到父类 Width/Height，避免步进器继续维护额外尺寸中间态。
                                PostToUI(() => { vm.HorizontalAlign = param.HorizontalAlign; vm.VerticalAlign = param.VerticalAlign; vm.MarginTop = mt; vm.MarginLeft = ml; vm.MarginBottom = mb; vm.MarginRight = mr; vm.MinWidth = minW; vm.MaxWidth = maxW; vm.MinHeight = minH; vm.MaxHeight = maxH; });
                                return;
                            }
                        }
                        catch { }
                    }
                    var val = callBack.GetMapCellPropValue(nameof(StepperPropertyModelEdit.LayoutInfo));
                    if (val != null)
                    {
                        try
                        {
                            var info = DeserializeObject<StepperLayoutInfo>(val);
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
                if (callBack.GetMapCellVMObjInstance() is StepperViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<CommonInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                decimal.TryParse(param.Value, out var val);
                                decimal.TryParse(param.Minimum, out var min);
                                decimal.TryParse(param.Maximum, out var max);
                                decimal.TryParse(param.Increment, out var inc);
                                int.TryParse(param.DecimalPlaces, out var dp);

                                PostToUI(() => { vm.LabelName = param.LabelName; vm.Value = val; vm.Minimum = min; vm.Maximum = max; vm.Increment = inc; vm.DecimalPlaces = dp; vm.IsEnabled = param.IsEnabled; vm.ToolTip = param.ToolTip; });
                                return;
                            }
                        }
                        catch { }
                    }
                    var valObj = callBack.GetMapCellPropValue(nameof(StepperPropertyModelEdit.CommonInfo));
                    if (valObj != null)
                    {
                        try
                        {
                            var info = DeserializeObject<StepperCommonInfo>(valObj);
                            PostToUI(() => { vm.LabelName = info.LabelName; vm.Value = info.Value; vm.Minimum = info.Minimum; vm.Maximum = info.Maximum; vm.Increment = info.Increment; vm.DecimalPlaces = info.DecimalPlaces; vm.IsEnabled = info.IsEnabled; vm.ToolTip = info.ToolTip; });
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
                if (callBack.GetMapCellVMObjInstance() is StepperViewModel vm)
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
                    var val = callBack.GetMapCellPropValue(nameof(StepperPropertyModelEdit.TextInfo));
                    if (val != null)
                    {
                        try
                        {
                            var info = DeserializeObject<StepperTextInfo>(val);
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

       

        #endregion
    }

    /// <summary>
    /// 步进器属性编辑模型对象
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("画笔", 1)]
    [CategoryPriority("外观", 2)]
    [CategoryPriority("布局", 3)]
    [CategoryPriority("公共", 4)]
    [CategoryPriority("文本", 5)]
    public class StepperPropertyModelEdit : ControlCellPropertyModelEdit
    {
        #region 基础属性

        private StepperBrushInfo _brushInfo = new StepperBrushInfo();
        [DisplayName("画笔设置")]
        [Category("画笔")]
        [PropertySortOrder(1)]
        public StepperBrushInfo BrushInfo
        {
            get => _brushInfo;
            set => SetProperty(ref _brushInfo, value);
        }

        private StepperAppearanceInfo _appearanceInfo = new StepperAppearanceInfo();
        [DisplayName("外观设置")]
        [Category("外观")]
        [PropertySortOrder(1)]
        public StepperAppearanceInfo AppearanceInfo
        {
            get => _appearanceInfo;
            set => SetProperty(ref _appearanceInfo, value);
        }

        private StepperLayoutInfo _layoutInfo = new StepperLayoutInfo();
        [DisplayName("布局设置")]
        [Category("布局")]
        [PropertySortOrder(1)]
        public StepperLayoutInfo LayoutInfo
        {
            get => _layoutInfo;
            set => SetProperty(ref _layoutInfo, value);
        }

        private StepperCommonInfo _commonInfo = new StepperCommonInfo();
        [DisplayName("公共设置")]
        [Category("公共")]
        [PropertySortOrder(1)]
        public StepperCommonInfo CommonInfo
        {
            get => _commonInfo;
            set => SetProperty(ref _commonInfo, value);
        }

        private StepperTextInfo _textInfo = new StepperTextInfo();
        [DisplayName("文本设置")]
        [Category("文本")]
        [PropertySortOrder(1)]
        public StepperTextInfo TextInfo
        {
            get => _textInfo;
            set => SetProperty(ref _textInfo, value);
        }

        #endregion

        #region SetPropertyValue 方法

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(StepperCommonInfo.LabelName)) == 0)
            {
                _commonInfo ??= new StepperCommonInfo();
                _commonInfo.LabelName = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : StepperCommonInfo.Default.LabelName;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperCommonInfo.Value)) == 0)
            {
                _commonInfo ??= new StepperCommonInfo();
                _commonInfo.Value = propertyVal != null ? propertyVal.ToPrimitiveValue<decimal>() : StepperCommonInfo.Default.Value;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperCommonInfo.Minimum)) == 0)
            {
                _commonInfo ??= new StepperCommonInfo();
                _commonInfo.Minimum = propertyVal != null ? propertyVal.ToPrimitiveValue<decimal>() : StepperCommonInfo.Default.Minimum;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperCommonInfo.Maximum)) == 0)
            {
                _commonInfo ??= new StepperCommonInfo();
                _commonInfo.Maximum = propertyVal != null ? propertyVal.ToPrimitiveValue<decimal>() : StepperCommonInfo.Default.Maximum;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperCommonInfo.Increment)) == 0)
            {
                _commonInfo ??= new StepperCommonInfo();
                _commonInfo.Increment = propertyVal != null ? propertyVal.ToPrimitiveValue<decimal>() : StepperCommonInfo.Default.Increment;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperCommonInfo.DecimalPlaces)) == 0)
            {
                _commonInfo ??= new StepperCommonInfo();
                _commonInfo.DecimalPlaces = propertyVal != null ? (int)propertyVal.ToPrimitiveValue<decimal>() : StepperCommonInfo.Default.DecimalPlaces;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperCommonInfo.IsEnabled)) == 0)
            {
                _commonInfo ??= new StepperCommonInfo();
                _commonInfo.IsEnabled = propertyVal != null ? propertyVal.ToPrimitiveValue<bool>() : StepperCommonInfo.Default.IsEnabled;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperCommonInfo.ToolTip)) == 0)
            {
                _commonInfo ??= new StepperCommonInfo();
                _commonInfo.ToolTip = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : StepperCommonInfo.Default.ToolTip;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }

            if (string.Compare(propertyID, nameof(StepperBrushInfo.BackColor)) == 0 || string.Compare(propertyID, "BackColor") == 0)
            {
                _brushInfo ??= new StepperBrushInfo();
                _brushInfo.BackColorStr = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : StepperBrushInfo.Default.BackColorStr;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperBrushInfo.BorderColor)) == 0 || string.Compare(propertyID, "BorderColor") == 0)
            {
                _brushInfo ??= new StepperBrushInfo();
                _brushInfo.BorderColorStr = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : StepperBrushInfo.Default.BorderColorStr;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperBrushInfo.ForeColor)) == 0 || string.Compare(propertyID, "ForeColor") == 0)
            {
                _brushInfo ??= new StepperBrushInfo();
                _brushInfo.ForeColorStr = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : StepperBrushInfo.Default.ForeColorStr;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }

            if (string.Compare(propertyID, nameof(StepperAppearanceInfo.Opacity)) == 0)
            {
                _appearanceInfo ??= new StepperAppearanceInfo();
                _appearanceInfo.Opacity = propertyVal != null ? (double)propertyVal.ToPrimitiveValue<decimal>() : StepperAppearanceInfo.Default.Opacity;
                RaisePropertyChanged(nameof(AppearanceInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperTextInfo.FontFamilyType)) == 0)
            {
                _textInfo ??= new StepperTextInfo();
                if (propertyVal != null)
                {
                    var fontStr = propertyVal.ToPrimitiveValue<string>();
                    _textInfo.FontFamilyType = Enum.TryParse<FontFamilyType>(fontStr, out var f) ? f : StepperTextInfo.Default.FontFamilyType;
                }
                else
                {
                    _textInfo.FontFamilyType = StepperTextInfo.Default.FontFamilyType;
                }
                RaisePropertyChanged(nameof(TextInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperTextInfo.FontColor)) == 0 || string.Compare(propertyID, "FontColor") == 0)
            {
                _textInfo ??= new StepperTextInfo();
                _textInfo.FontColorStr = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : StepperTextInfo.Default.FontColorStr;
                RaisePropertyChanged(nameof(TextInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperTextInfo.FontSize)) == 0)
            {
                _textInfo ??= new StepperTextInfo();
                _textInfo.FontSize = propertyVal != null ? (double)propertyVal.ToPrimitiveValue<decimal>() : StepperTextInfo.Default.FontSize;
                RaisePropertyChanged(nameof(TextInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperTextInfo.IsItalic)) == 0)
            {
                _textInfo ??= new StepperTextInfo();
                _textInfo.IsItalic = propertyVal != null ? propertyVal.ToPrimitiveValue<bool>() : StepperTextInfo.Default.IsItalic;
                RaisePropertyChanged(nameof(TextInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(StepperTextInfo.IsBold)) == 0)
            {
                _textInfo ??= new StepperTextInfo();
                _textInfo.IsBold = propertyVal != null ? propertyVal.ToPrimitiveValue<bool>() : StepperTextInfo.Default.IsBold;
                RaisePropertyChanged(nameof(TextInfo));
                return true;
            }

            if (string.Compare(propertyID, nameof(BrushInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<StepperBrushInfo>(propertyVal) : new StepperBrushInfo();
                _brushInfo ??= new StepperBrushInfo();
                _brushInfo.BackColorStr = src.BackColorStr;
                _brushInfo.BorderColorStr = src.BorderColorStr;
                _brushInfo.ForeColorStr = src.ForeColorStr;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(AppearanceInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<StepperAppearanceInfo>(propertyVal) : new StepperAppearanceInfo();
                _appearanceInfo ??= new StepperAppearanceInfo();
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
                var src = propertyVal != null ? DeserializeObject<StepperLayoutInfo>(propertyVal) : new StepperLayoutInfo();
                _layoutInfo ??= new StepperLayoutInfo();
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
                var src = propertyVal != null ? DeserializeObject<StepperCommonInfo>(propertyVal) : new StepperCommonInfo();
                _commonInfo ??= new StepperCommonInfo();
                _commonInfo.LabelName = src.LabelName;
                _commonInfo.Value = src.Value;
                _commonInfo.Minimum = src.Minimum;
                _commonInfo.Maximum = src.Maximum;
                _commonInfo.Increment = src.Increment;
                _commonInfo.DecimalPlaces = src.DecimalPlaces;
                _commonInfo.IsEnabled = src.IsEnabled;
                _commonInfo.ToolTip = src.ToolTip;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(TextInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<StepperTextInfo>(propertyVal) : new StepperTextInfo();
                _textInfo ??= new StepperTextInfo();
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

        public void CopyFrom(StepperPropertyModelEdit source)
        {
            if (source == null) return;

            base.CopyFrom(source);

            // 复制画笔信息
            _brushInfo ??= new StepperBrushInfo();
            _brushInfo.BackColorStr = source.BrushInfo?.BackColorStr;
            _brushInfo.BorderColorStr = source.BrushInfo?.BorderColorStr;
            _brushInfo.ForeColorStr = source.BrushInfo?.ForeColorStr;
            RaisePropertyChanged(nameof(BrushInfo));

            // 复制外观信息
            _appearanceInfo ??= new StepperAppearanceInfo();
            _appearanceInfo.Opacity = source.AppearanceInfo?.Opacity ?? 1.0;
            _appearanceInfo.BorderThicknessLeft = source.AppearanceInfo?.BorderThicknessLeft ?? 1;
            _appearanceInfo.BorderThicknessTop = source.AppearanceInfo?.BorderThicknessTop ?? 1;
            _appearanceInfo.BorderThicknessRight = source.AppearanceInfo?.BorderThicknessRight ?? 1;
            _appearanceInfo.BorderThicknessBottom = source.AppearanceInfo?.BorderThicknessBottom ?? 1;
            RaisePropertyChanged(nameof(AppearanceInfo));

            // 复制布局信息
            _layoutInfo ??= new StepperLayoutInfo();
            // 宽高主数据统一走父类 Width/Height，LayoutInfo 仅保留旧页面兼容镜像。
            Width = source.Width;
            Height = source.Height;
            _layoutInfo.HorizontalAlign = source.LayoutInfo?.HorizontalAlign ?? StepperLayoutInfo.Default.HorizontalAlign;
            _layoutInfo.VerticalAlign = source.LayoutInfo?.VerticalAlign ?? StepperLayoutInfo.Default.VerticalAlign;
            _layoutInfo.MarginTop = source.LayoutInfo?.MarginTop ?? StepperLayoutInfo.Default.MarginTop;
            _layoutInfo.MarginLeft = source.LayoutInfo?.MarginLeft ?? StepperLayoutInfo.Default.MarginLeft;
            _layoutInfo.MarginBottom = source.LayoutInfo?.MarginBottom ?? StepperLayoutInfo.Default.MarginBottom;
            _layoutInfo.MarginRight = source.LayoutInfo?.MarginRight ?? StepperLayoutInfo.Default.MarginRight;
            _layoutInfo.MinWidth = source.LayoutInfo?.MinWidth ?? StepperLayoutInfo.Default.MinWidth;
            _layoutInfo.MaxWidth = source.LayoutInfo?.MaxWidth ?? StepperLayoutInfo.Default.MaxWidth;
            _layoutInfo.MinHeight = source.LayoutInfo?.MinHeight ?? StepperLayoutInfo.Default.MinHeight;
            _layoutInfo.MaxHeight = source.LayoutInfo?.MaxHeight ?? StepperLayoutInfo.Default.MaxHeight;
            RaisePropertyChanged(nameof(LayoutInfo));

            // 复制公共信息
            _commonInfo ??= new StepperCommonInfo();
            _commonInfo.LabelName = source.CommonInfo?.LabelName ?? "标签";
            _commonInfo.Value = source.CommonInfo?.Value ?? 0;
            _commonInfo.Minimum = source.CommonInfo?.Minimum ?? 0;
            _commonInfo.Maximum = source.CommonInfo?.Maximum ?? 100;
            _commonInfo.Increment = source.CommonInfo?.Increment ?? 1;
            _commonInfo.DecimalPlaces = source.CommonInfo?.DecimalPlaces ?? 0;
            _commonInfo.IsEnabled = source.CommonInfo?.IsEnabled ?? true;
            _commonInfo.ToolTip = source.CommonInfo?.ToolTip ?? "";
            RaisePropertyChanged(nameof(CommonInfo));

            // 复制文本信息
            _textInfo ??= new StepperTextInfo();
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
    /// 步进器属性绑定编辑模型
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("绑定信息", 1)]
    public class StepperPropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _labelName = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        [DisplayName("标签名称")]
        [Category("绑定信息")]
        [PropertySortOrder(1)]
        [BindMPPropertyID]
        public PropertyBindInfo LabelName
        {
            get => _labelName;
            set => SetProperty(ref _labelName, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _value = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Decimal);
        [DisplayName("当前值")]
        [Category("绑定信息")]
        [PropertySortOrder(2)]
        [BindMPPropertyID]
        public PropertyBindInfo Value
        {
            get => _value;
            set => SetProperty(ref _value, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Decimal));
        }

        private PropertyBindInfo _minimum = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Decimal);
        [DisplayName("最小值")]
        [Category("绑定信息")]
        [PropertySortOrder(3)]
        [BindMPPropertyID]
        public PropertyBindInfo Minimum
        {
            get => _minimum;
            set => SetProperty(ref _minimum, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Decimal));
        }

        private PropertyBindInfo _maximum = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Decimal);
        [DisplayName("最大值")]
        [Category("绑定信息")]
        [PropertySortOrder(4)]
        [BindMPPropertyID]
        public PropertyBindInfo Maximum
        {
            get => _maximum;
            set => SetProperty(ref _maximum, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Decimal));
        }

        private PropertyBindInfo _increment = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Decimal);
        [DisplayName("增量")]
        [Category("绑定信息")]
        [PropertySortOrder(5)]
        [BindMPPropertyID]
        public PropertyBindInfo Increment
        {
            get => _increment;
            set => SetProperty(ref _increment, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Decimal));
        }

        private PropertyBindInfo _decimalPlaces = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Integer);
        [DisplayName("小数位数")]
        [Category("绑定信息")]
        [PropertySortOrder(6)]
        [BindMPPropertyID]
        public PropertyBindInfo DecimalPlaces
        {
            get => _decimalPlaces;
            set => SetProperty(ref _decimalPlaces, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Integer));
        }

        private PropertyBindInfo _isEnabled = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Bool);
        [DisplayName("是否启用")]
        [Category("绑定信息")]
        [PropertySortOrder(7)]
        [BindMPPropertyID]
        public PropertyBindInfo IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Bool));
        }

        private PropertyBindInfo _toolTip = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        [DisplayName("提示文本")]
        [Category("绑定信息")]
        [PropertySortOrder(8)]
        [BindMPPropertyID]
        public PropertyBindInfo ToolTip
        {
            get => _toolTip;
            set => SetProperty(ref _toolTip, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _backColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        [DisplayName("背景色")]
        [Category("绑定信息")]
        [PropertySortOrder(9)]
        [BindMPPropertyID]
        public PropertyBindInfo BackColor
        {
            get => _backColor;
            set => SetProperty(ref _backColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _borderColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        [DisplayName("边框色")]
        [Category("绑定信息")]
        [PropertySortOrder(10)]
        [BindMPPropertyID]
        public PropertyBindInfo BorderColor
        {
            get => _borderColor;
            set => SetProperty(ref _borderColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _foreColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        [DisplayName("前景色")]
        [Category("绑定信息")]
        [PropertySortOrder(11)]
        [BindMPPropertyID]
        public PropertyBindInfo ForeColor
        {
            get => _foreColor;
            set => SetProperty(ref _foreColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _opacity = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Decimal);
        [DisplayName("透明度")]
        [Category("绑定信息")]
        [PropertySortOrder(12)]
        [BindMPPropertyID]
        public PropertyBindInfo Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Decimal));
        }

        private PropertyBindInfo _fontFamilyType = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        [DisplayName("字体")]
        [Category("绑定信息")]
        [PropertySortOrder(14)]
        [BindMPPropertyID]
        public PropertyBindInfo FontFamilyType
        {
            get => _fontFamilyType;
            set => SetProperty(ref _fontFamilyType, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _fontColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        [DisplayName("字体颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(15)]
        [BindMPPropertyID]
        public PropertyBindInfo FontColor
        {
            get => _fontColor;
            set => SetProperty(ref _fontColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _fontSize = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Decimal);
        [DisplayName("字号")]
        [Category("绑定信息")]
        [PropertySortOrder(16)]
        [BindMPPropertyID]
        public PropertyBindInfo FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Decimal));
        }

        private PropertyBindInfo _isItalic = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Bool);
        [DisplayName("斜体")]
        [Category("绑定信息")]
        [PropertySortOrder(17)]
        [BindMPPropertyID]
        public PropertyBindInfo IsItalic
        {
            get => _isItalic;
            set => SetProperty(ref _isItalic, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Bool));
        }

        private PropertyBindInfo _isBold = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Bool);
        [DisplayName("粗体")]
        [Category("绑定信息")]
        [PropertySortOrder(18)]
        [BindMPPropertyID]
        public PropertyBindInfo IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Bool));
        }

        public void CopyFrom(StepperPropertyBindEditModel source)
        {
            if (source == null) return;
            base.CopyFrom(source);
            LabelName = source.LabelName;
            Value = source.Value;
            Minimum = source.Minimum;
            Maximum = source.Maximum;
            Increment = source.Increment;
            DecimalPlaces = source.DecimalPlaces;
            IsEnabled = source.IsEnabled;
            ToolTip = source.ToolTip;
            BackColor = source.BackColor;
            BorderColor = source.BorderColor;
            ForeColor = source.ForeColor;
            Opacity = source.Opacity;
            FontFamilyType = source.FontFamilyType;
            FontColor = source.FontColor;
            FontSize = source.FontSize;
            IsItalic = source.IsItalic;
            IsBold = source.IsBold;
        }
    }
}

