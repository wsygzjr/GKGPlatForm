using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic;
using GKG.Map.MapCell.Generic.Control.Lable;
using GKG.Map.MapCell.Generic.PasswordBox.View;
using GKG.Map.MapCell.Generic.PasswordBox.ViewModel;
using GKG.Map.MapCell.Generic.PasswordBox.MapOprtCellParamCfgView;
using Newtonsoft.JsonG;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.PasswordBox
{
    class MapCellPasswordBoxCtlObj : ControlCellBase
    {
        private PasswordBoxView view;
        private PasswordBoxViewModel passwordBoxViewModel;
        private bool _loadedPropertyEditFromBytes;
        private readonly ConcurrentDictionary<Guid, MapOprtCellID> _oprtCellIdByInstanceId = new();


        static MapCellPasswordBoxCtlObj() { }

        public MapCellPasswordBoxCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false) { }

        public MapCellPasswordBoxCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            base.SetID(mapCellID);
            base.SetName(mapCellName);
            view = new PasswordBoxView(designTime);

            RegisterProperty(new MapObjPropertyInfo(nameof(PasswordBoxPropertyModelEdit.BrushInfo), ResourceA.PasswordBox_BrushInfo, MapCellPropDataType.Object_Json, PasswordBoxBrushInfo.Object_ID, typeof(PasswordBoxBrushInfo), false, true, new MapCellPropValue(PasswordBoxBrushInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(PasswordBoxPropertyModelEdit.AppearanceInfo), ResourceA.PasswordBox_AppearanceInfo, MapCellPropDataType.Object_Json, PasswordBoxAppearanceInfo.Object_ID, typeof(PasswordBoxAppearanceInfo), false, true, new MapCellPropValue(PasswordBoxAppearanceInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(PasswordBoxPropertyModelEdit.LayoutInfo), ResourceA.PasswordBox_LayoutInfo, MapCellPropDataType.Object_Json, PasswordBoxLayoutInfo.Object_ID, typeof(PasswordBoxLayoutInfo), false, true, new MapCellPropValue(PasswordBoxLayoutInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(PasswordBoxPropertyModelEdit.CommonInfo), ResourceA.PasswordBox_CommonInfo, MapCellPropDataType.Object_Json, PasswordBoxCommonInfo.Object_ID, typeof(PasswordBoxCommonInfo), false, true, new MapCellPropValue(PasswordBoxCommonInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(PasswordBoxPropertyModelEdit.TextInfo), ResourceA.PasswordBox_TextInfo, MapCellPropDataType.Object_Json, PasswordBoxTextInfo.Object_ID, typeof(PasswordBoxTextInfo), false, true, new MapCellPropValue(PasswordBoxTextInfo.Default)));

            RegisterOprtCellInfo(new MapOprtCellInfo(PasswordBoxMapOprtCellConst.BrushInfo_MapOprtCellID, ResourceA.PasswordBox_BrushInfo_MapOprtCellName, typeof(BrushInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(PasswordBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID, ResourceA.PasswordBox_AppearanceInfo_MapOprtCellName, typeof(AppearanceInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(PasswordBoxMapOprtCellConst.LayoutInfo_MapOprtCellID, ResourceA.PasswordBox_LayoutInfo_MapOprtCellName, typeof(LayoutInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(PasswordBoxMapOprtCellConst.CommonInfo_MapOprtCellID, ResourceA.PasswordBox_CommonInfo_MapOprtCellName, typeof(CommonInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(PasswordBoxMapOprtCellConst.TextInfo_MapOprtCellID, ResourceA.PasswordBox_TextInfo_MapOprtCellName, typeof(TextInfoMapOprtCellParamCfgView)));

            RegisterOprtInfo(new MapOprtInfo(nameof(PasswordBoxPropertyModelEdit.BrushInfo), ResourceA.PasswordBox_BrushInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = PasswordBoxMapOprtCellConst.BrushInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(PasswordBoxPropertyModelEdit.AppearanceInfo), ResourceA.PasswordBox_AppearanceInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = PasswordBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(PasswordBoxPropertyModelEdit.LayoutInfo), ResourceA.PasswordBox_LayoutInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = PasswordBoxMapOprtCellConst.LayoutInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(PasswordBoxPropertyModelEdit.CommonInfo), ResourceA.PasswordBox_CommonInfo_MapOprtlName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = PasswordBoxMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(PasswordBoxPropertyModelEdit.TextInfo), ResourceA.PasswordBox_TextInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = PasswordBoxMapOprtCellConst.TextInfo_MapOprtCellID, CfgInfo = null } }));

            (this as IMapCellTypeBase).Name = ResourceA.PasswordBox;
            passwordBoxViewModel = new PasswordBoxViewModel(designTime, PasswordBoxPropertyModelEdit);
            view.DataContext = passwordBoxViewModel;

            // 订阅 Info 对象的 PropertyChanged，触发对应操作原子。
            PasswordBoxPropertyModelEdit.BrushInfo.PropertyChanged += OnBrushInfoPropertyChanged;
            PasswordBoxPropertyModelEdit.AppearanceInfo.PropertyChanged += OnAppearanceInfoPropertyChanged;
            PasswordBoxPropertyModelEdit.LayoutInfo.PropertyChanged += OnLayoutInfoPropertyChanged;
            PasswordBoxPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
            PasswordBoxPropertyModelEdit.TextInfo.PropertyChanged += OnTextInfoPropertyChanged;


        }

        private void OnBrushInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(PasswordBoxPropertyModelEdit.BrushInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnAppearanceInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(PasswordBoxPropertyModelEdit.AppearanceInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnLayoutInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(PasswordBoxPropertyModelEdit.LayoutInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(PasswordBoxPropertyModelEdit.CommonInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnTextInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(PasswordBoxPropertyModelEdit.TextInfo), "PropertyChanged", e?.PropertyName);
        }

        public override void OnDispose()
        {
            PasswordBoxPropertyModelEdit.BrushInfo.PropertyChanged -= OnBrushInfoPropertyChanged;
            PasswordBoxPropertyModelEdit.AppearanceInfo.PropertyChanged -= OnAppearanceInfoPropertyChanged;
            PasswordBoxPropertyModelEdit.LayoutInfo.PropertyChanged -= OnLayoutInfoPropertyChanged;
            PasswordBoxPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;
            PasswordBoxPropertyModelEdit.TextInfo.PropertyChanged -= OnTextInfoPropertyChanged;

            view.DataContext = null;
            passwordBoxViewModel?.Dispose();
            passwordBoxViewModel = null;

            base.OnDispose();
        }

        public PasswordBoxPropertyModelEdit PasswordBoxPropertyModelEdit => PropertyEditModelBase as PasswordBoxPropertyModelEdit;

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
            // 读取旧文件后，设计器可能再次下发默认值，这里避免默认值覆盖已保存配置。
            // 这类保护只在非运行态生效。
            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoadedPasswordBox(propertyID, propertyVal))
            {
                return true;
            }
            PasswordBoxPropertyModelEdit.IsRuning = isRuning;
            var ok = PasswordBoxPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
            if (ok)
            {
                ExecuteOprtByPropertyId(propertyID, "SetPropertyValue", null);
            }
return ok;
        }
        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            if (PasswordBoxPropertyModelEdit.IsRuning)
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

        private bool IsDefaultOverwriteForLoadedPasswordBox(string propertyID, MapCellPropValue propertyVal)
        {
            try
            {
                if (string.Compare(propertyID, nameof(PasswordBoxPropertyModelEdit.BrushInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<PasswordBoxBrushInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(PasswordBoxPropertyModelEdit.BrushInfo);
                }
                if (string.Compare(propertyID, nameof(PasswordBoxPropertyModelEdit.AppearanceInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<PasswordBoxAppearanceInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(PasswordBoxPropertyModelEdit.AppearanceInfo);
                }
                if (string.Compare(propertyID, nameof(PasswordBoxPropertyModelEdit.LayoutInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<PasswordBoxLayoutInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(PasswordBoxPropertyModelEdit.LayoutInfo);
                }
                if (string.Compare(propertyID, nameof(PasswordBoxPropertyModelEdit.CommonInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<PasswordBoxCommonInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(PasswordBoxPropertyModelEdit.CommonInfo);
                }
                if (string.Compare(propertyID, nameof(PasswordBoxPropertyModelEdit.TextInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<PasswordBoxTextInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(PasswordBoxPropertyModelEdit.TextInfo);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsDefault(PasswordBoxBrushInfo info)
        {
            if (info == null) return true;
            return string.Equals(info.BackColorStr ?? "", PasswordBoxBrushInfo.Default.BackColorStr, StringComparison.OrdinalIgnoreCase)
                && string.Equals(info.BorderColorStr ?? "", PasswordBoxBrushInfo.Default.BorderColorStr, StringComparison.OrdinalIgnoreCase)
                && string.Equals(info.ForeColorStr ?? "", PasswordBoxBrushInfo.Default.ForeColorStr, StringComparison.OrdinalIgnoreCase)
                && string.Equals(info.FocusBorderColorStr ?? "", PasswordBoxBrushInfo.Default.FocusBorderColorStr, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsDefault(PasswordBoxAppearanceInfo info)
        {
            if (info == null) return true;
            return Math.Abs(info.Opacity - PasswordBoxAppearanceInfo.Default.Opacity) < 0.000001
                && Math.Abs(info.BorderThicknessLeft - PasswordBoxAppearanceInfo.Default.BorderThicknessLeft) < 0.000001
                && Math.Abs(info.BorderThicknessTop - PasswordBoxAppearanceInfo.Default.BorderThicknessTop) < 0.000001
                && Math.Abs(info.BorderThicknessRight - PasswordBoxAppearanceInfo.Default.BorderThicknessRight) < 0.000001
                && Math.Abs(info.BorderThicknessBottom - PasswordBoxAppearanceInfo.Default.BorderThicknessBottom) < 0.000001;
        }

        private static bool IsDefault(PasswordBoxLayoutInfo info)
        {
            if (info == null) return true;
            return info.HorizontalAlignment == PasswordBoxLayoutInfo.Default.HorizontalAlignment
                && info.VerticalAlignment == PasswordBoxLayoutInfo.Default.VerticalAlignment
                && Math.Abs(info.MarginLeft - PasswordBoxLayoutInfo.Default.MarginLeft) < 0.000001
                && Math.Abs(info.MarginTop - PasswordBoxLayoutInfo.Default.MarginTop) < 0.000001
                && Math.Abs(info.MarginRight - PasswordBoxLayoutInfo.Default.MarginRight) < 0.000001
                && Math.Abs(info.MarginBottom - PasswordBoxLayoutInfo.Default.MarginBottom) < 0.000001
                && Math.Abs(info.MinWidth - PasswordBoxLayoutInfo.Default.MinWidth) < 0.000001
                && Math.Abs(info.MaxWidth - PasswordBoxLayoutInfo.Default.MaxWidth) < 0.000001
                && Math.Abs(info.MinHeight - PasswordBoxLayoutInfo.Default.MinHeight) < 0.000001
                && Math.Abs(info.MaxHeight - PasswordBoxLayoutInfo.Default.MaxHeight) < 0.000001;
        }

        private static bool IsDefault(PasswordBoxCommonInfo info)
        {
            if (info == null) return true;
            return string.Equals(info.PasswordValue ?? "", PasswordBoxCommonInfo.Default.PasswordValue ?? "", StringComparison.Ordinal)
                && info.CursorType == PasswordBoxCommonInfo.Default.CursorType
                && info.Enabled == PasswordBoxCommonInfo.Default.Enabled
                && string.Equals(info.PlaceholderText ?? "", PasswordBoxCommonInfo.Default.PlaceholderText ?? "", StringComparison.Ordinal)
                && info.PasswordVisible == PasswordBoxCommonInfo.Default.PasswordVisible;
        }

        private static bool IsDefault(PasswordBoxTextInfo info)
        {
            if (info == null) return true;
            return info.FontFamily == PasswordBoxTextInfo.Default.FontFamily
                && Math.Abs(info.FontSize - PasswordBoxTextInfo.Default.FontSize) < 0.000001
                && info.IsItalic == PasswordBoxTextInfo.Default.IsItalic
                && info.IsBold == PasswordBoxTextInfo.Default.IsBold;
        }

        #region 操作原子执行辅助

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
            if (string.Equals(oprtId, nameof(PasswordBoxPropertyModelEdit.BrushInfo), StringComparison.Ordinal))
            { oprtCellId = PasswordBoxMapOprtCellConst.BrushInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(PasswordBoxPropertyModelEdit.AppearanceInfo), StringComparison.Ordinal))
            { oprtCellId = PasswordBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(PasswordBoxPropertyModelEdit.LayoutInfo), StringComparison.Ordinal))
            { oprtCellId = PasswordBoxMapOprtCellConst.LayoutInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(PasswordBoxPropertyModelEdit.CommonInfo), StringComparison.Ordinal))
            { oprtCellId = PasswordBoxMapOprtCellConst.CommonInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(PasswordBoxPropertyModelEdit.TextInfo), StringComparison.Ordinal))
            { oprtCellId = PasswordBoxMapOprtCellConst.TextInfo_MapOprtCellID; return true; }
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

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            string propertyEditJson = br.ReadString("PropertyEditModelBase");
            if (!string.IsNullOrEmpty(propertyEditJson))
            {
                try
                {
                    var propertyEditModelBase = JsonObjConvert.FromJSon<PasswordBoxPropertyModelEdit>(propertyEditJson);
                    if (propertyEditModelBase != null)
                    {
                        (PropertyEditModelBase as PasswordBoxPropertyModelEdit).CopyFrom(propertyEditModelBase);
                        _loadedPropertyEditFromBytes = true;
                    }
                    else
                    {
                        var fallback = TryParsePasswordBoxPropertyModelEdit(propertyEditJson);
                        if (fallback != null)
                        {
                            (PropertyEditModelBase as PasswordBoxPropertyModelEdit).CopyFrom(fallback);
                            _loadedPropertyEditFromBytes = true;
                        }
                    }
                }
                catch
                {
                    var fallback = TryParsePasswordBoxPropertyModelEdit(propertyEditJson);
                    if (fallback != null)
                    {
                        (PropertyEditModelBase as PasswordBoxPropertyModelEdit).CopyFrom(fallback);
                        _loadedPropertyEditFromBytes = true;
                    }
                }
            }
            string propertyBindJson = br.ReadString("PropertyBindEditModelBase");
            if (!string.IsNullOrEmpty(propertyBindJson))
            {
                try
                {
                    var propertyBindEditModelBase = JsonObjConvert.FromJSon<PasswordBoxPropertyBindEditModel>(propertyBindJson);
                    if (propertyBindEditModelBase != null)
                        (PropertyBindEditModelBase as PasswordBoxPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
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

            // CopyFrom 会触发 PropertyChanged，这里不再额外刷新 ViewModel。
        }

        private static PasswordBoxPropertyModelEdit TryParsePasswordBoxPropertyModelEdit(string json)
        {
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;
                var model = new PasswordBoxPropertyModelEdit();

                // 读取 BrushInfo
                if (root.TryGetProperty("BrushInfo", out var brushInfo) && brushInfo.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    if (brushInfo.TryGetProperty("BackColorStr", out var backColor))
                        model.BrushInfo.BackColorStr = backColor.GetString() ?? "#FFFFFFFF";
                    else if (brushInfo.TryGetProperty("BackColor", out backColor))
                        model.BrushInfo.BackColorStr = backColor.GetString() ?? "#FFFFFFFF";
                    
                    if (brushInfo.TryGetProperty("BorderColorStr", out var borderColor))
                        model.BrushInfo.BorderColorStr = borderColor.GetString() ?? "#FF808080";
                    else if (brushInfo.TryGetProperty("BorderColor", out borderColor))
                        model.BrushInfo.BorderColorStr = borderColor.GetString() ?? "#FF808080";
                    
                    if (brushInfo.TryGetProperty("ForeColorStr", out var foreColor))
                        model.BrushInfo.ForeColorStr = foreColor.GetString() ?? "#FF000000";
                    else if (brushInfo.TryGetProperty("ForeColor", out foreColor))
                        model.BrushInfo.ForeColorStr = foreColor.GetString() ?? "#FF000000";
                    
                    if (brushInfo.TryGetProperty("FocusBorderColorStr", out var focusBorderColor))
                        model.BrushInfo.FocusBorderColorStr = focusBorderColor.GetString() ?? "#FF0000FF";
                    else if (brushInfo.TryGetProperty("FocusBorderColor", out focusBorderColor))
                        model.BrushInfo.FocusBorderColorStr = focusBorderColor.GetString() ?? "#FF0000FF";
                }

                // 读取 AppearanceInfo
                if (root.TryGetProperty("AppearanceInfo", out var appearanceInfo) && appearanceInfo.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    if (appearanceInfo.TryGetProperty("Opacity", out var opacity))
                        model.AppearanceInfo.Opacity = opacity.GetDouble();
                    if (appearanceInfo.TryGetProperty("BorderThicknessLeft", out var btl))
                        model.AppearanceInfo.BorderThicknessLeft = btl.GetDouble();
                    if (appearanceInfo.TryGetProperty("BorderThicknessTop", out var btt))
                        model.AppearanceInfo.BorderThicknessTop = btt.GetDouble();
                    if (appearanceInfo.TryGetProperty("BorderThicknessRight", out var btr))
                        model.AppearanceInfo.BorderThicknessRight = btr.GetDouble();
                    if (appearanceInfo.TryGetProperty("BorderThicknessBottom", out var btb))
                        model.AppearanceInfo.BorderThicknessBottom = btb.GetDouble();
                }

                // 读取 LayoutInfo
                if (root.TryGetProperty("LayoutInfo", out var layoutInfo) && layoutInfo.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    if (layoutInfo.TryGetProperty("Width", out var width))
                        model.Width = width.GetDouble();
                    if (layoutInfo.TryGetProperty("Height", out var height))
                        model.Height = height.GetDouble();
                    if (layoutInfo.TryGetProperty("HorizontalAlignment", out var ha))
                    {
                        var haStr = ha.GetString();
                        if (Enum.TryParse<HorizontalAlignType>(haStr, out var haVal))
                            model.LayoutInfo.HorizontalAlignment = haVal;
                    }
                    if (layoutInfo.TryGetProperty("VerticalAlignment", out var va))
                    {
                        var vaStr = va.GetString();
                        if (Enum.TryParse<VerticalAlignType>(vaStr, out var vaVal))
                            model.LayoutInfo.VerticalAlignment = vaVal;
                    }
                    if (layoutInfo.TryGetProperty("MarginLeft", out var ml))
                        model.LayoutInfo.MarginLeft = ml.GetDouble();
                    if (layoutInfo.TryGetProperty("MarginTop", out var mt))
                        model.LayoutInfo.MarginTop = mt.GetDouble();
                    if (layoutInfo.TryGetProperty("MarginRight", out var mr))
                        model.LayoutInfo.MarginRight = mr.GetDouble();
                    if (layoutInfo.TryGetProperty("MarginBottom", out var mb))
                        model.LayoutInfo.MarginBottom = mb.GetDouble();
                    if (layoutInfo.TryGetProperty("MinWidth", out var minW))
                        model.LayoutInfo.MinWidth = minW.GetDouble();
                    if (layoutInfo.TryGetProperty("MaxWidth", out var maxW))
                        model.LayoutInfo.MaxWidth = maxW.GetDouble();
                    if (layoutInfo.TryGetProperty("MinHeight", out var minH))
                        model.LayoutInfo.MinHeight = minH.GetDouble();
                    if (layoutInfo.TryGetProperty("MaxHeight", out var maxH))
                        model.LayoutInfo.MaxHeight = maxH.GetDouble();
                }

                // 读取 CommonInfo
                if (root.TryGetProperty("CommonInfo", out var commonInfo) && commonInfo.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    if (commonInfo.TryGetProperty("PasswordValue", out var pv))
                        model.CommonInfo.PasswordValue = pv.GetString() ?? "";
                    if (commonInfo.TryGetProperty("CursorType", out var ct))
                    {
                        var ctStr = ct.GetString();
                        if (Enum.TryParse<CursorType>(ctStr, out var ctVal))
                            model.CommonInfo.CursorType = ctVal;
                    }
                    if (commonInfo.TryGetProperty("Enabled", out var enabled))
                        model.CommonInfo.Enabled = enabled.GetBoolean();
                    if (commonInfo.TryGetProperty("PlaceholderText", out var pt))
                        model.CommonInfo.PlaceholderText = pt.GetString() ?? "";
                    if (commonInfo.TryGetProperty("PasswordVisible", out var pvs))
                        model.CommonInfo.PasswordVisible = pvs.GetBoolean();
                }

                // 读取 TextInfo
                if (root.TryGetProperty("TextInfo", out var textInfo) && textInfo.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    if (textInfo.TryGetProperty("FontFamily", out var ff))
                    {
                        var fontStr = ff.GetString();
                        if (Enum.TryParse<FontFamilyType>(fontStr, out var fontType))
                            model.TextInfo.FontFamily = fontType;
                        else
                            model.TextInfo.FontFamily = FontFamilyType.MicrosoftYaHei;
                    }
                    if (textInfo.TryGetProperty("FontSize", out var fs))
                        model.TextInfo.FontSize = fs.GetDouble();
                    if (textInfo.TryGetProperty("IsItalic", out var ii))
                        model.TextInfo.IsItalic = ii.GetBoolean();
                    if (textInfo.TryGetProperty("IsBold", out var ib))
                        model.TextInfo.IsBold = ib.GetBoolean();
                }

                return model;
            }
            catch
            {
                return null;
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
            MapCellPasswordBoxCtlObj obj = (source as MapCellPasswordBoxCtlObj);
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
        protected override object OnGetViewModel() => passwordBoxViewModel;
        public override PropertyEditModelBase CreatePropertyModelEditBase() => new PasswordBoxPropertyModelEdit();
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new PasswordBoxPropertyBindEditModel();
        public override EventBindEditModel CreateEventBindEditModel() => new EventBindEditModel() { EventCmdInfos = new BindingList<EventCmdInfo>() };

        public override void OnZoomChanged() { }
        public override string ToString() => "密码输入框";

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == PasswordBoxMapOprtCellConst.BrushInfo_MapOprtCellID)
                return ExecuteOprtCell<BrushInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == PasswordBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID)
                return ExecuteOprtCell<AppearanceInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == PasswordBoxMapOprtCellConst.LayoutInfo_MapOprtCellID)
                return ExecuteOprtCell<LayoutInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == PasswordBoxMapOprtCellConst.CommonInfo_MapOprtCellID)
                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == PasswordBoxMapOprtCellConst.TextInfo_MapOprtCellID)
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
                if (callBack.GetMapCellVMObjInstance() is PasswordBoxViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var json = Encoding.UTF8.GetString(cfg);
                            var param = JsonSerializer.Deserialize<BrushInfoMapOprtCellParamViewModel>(json);
                            if (param != null && vm.BrushInfo != null)
                            {
                                PostToUI(() => {
                                    vm.BrushInfo.BackColorStr = param.BackColorStr;
                                    vm.BrushInfo.BorderColorStr = param.BorderColorStr;
                                    vm.BrushInfo.ForeColorStr = param.ForeColorStr;
                                    vm.BrushInfo.FocusBorderColorStr = param.FocusBorderColorStr;
                                    vm.RaisePropertyChanged(nameof(vm.BrushInfo));
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(PasswordBoxPropertyModelEdit.BrushInfo));
                    if (val != null)
                    {
                        var info = DeserializeObject<PasswordBoxBrushInfo>(val);
                        if (vm.BrushInfo != null)
                        {
                            PostToUI(() => {
                                vm.BrushInfo.BackColorStr = info.BackColorStr;
                                vm.BrushInfo.BorderColorStr = info.BorderColorStr;
                                vm.BrushInfo.ForeColorStr = info.ForeColorStr;
                                vm.BrushInfo.FocusBorderColorStr = info.FocusBorderColorStr;
                                vm.RaisePropertyChanged(nameof(vm.BrushInfo));
                            });
                        }
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
                if (callBack.GetMapCellVMObjInstance() is PasswordBoxViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var json = Encoding.UTF8.GetString(cfg);
                            var param = JsonSerializer.Deserialize<AppearanceInfoMapOprtCellParamViewModel>(json);
                            if (param != null && vm.AppearanceInfo != null)
                            {
                                PostToUI(() => {
                                    vm.AppearanceInfo.Opacity = param.Opacity;
                                    vm.AppearanceInfo.BorderThicknessLeft = param.BorderThicknessLeft;
                                    vm.AppearanceInfo.BorderThicknessTop = param.BorderThicknessTop;
                                    vm.AppearanceInfo.BorderThicknessRight = param.BorderThicknessRight;
                                    vm.AppearanceInfo.BorderThicknessBottom = param.BorderThicknessBottom;
                                    vm.RaisePropertyChanged(nameof(vm.AppearanceInfo));
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(PasswordBoxPropertyModelEdit.AppearanceInfo));
                    if (val != null)
                    {
                        var info = DeserializeObject<PasswordBoxAppearanceInfo>(val);
                        if (vm.AppearanceInfo != null)
                        {
                            PostToUI(() => {
                                vm.AppearanceInfo.Opacity = info.Opacity;
                                vm.AppearanceInfo.BorderThicknessLeft = info.BorderThicknessLeft;
                                vm.AppearanceInfo.BorderThicknessTop = info.BorderThicknessTop;
                                vm.AppearanceInfo.BorderThicknessRight = info.BorderThicknessRight;
                                vm.AppearanceInfo.BorderThicknessBottom = info.BorderThicknessBottom;
                                vm.RaisePropertyChanged(nameof(vm.AppearanceInfo));
                            });
                        }
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
                if (callBack.GetMapCellVMObjInstance() is PasswordBoxViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var json = Encoding.UTF8.GetString(cfg);
                            var param = JsonSerializer.Deserialize<LayoutInfoMapOprtCellParamViewModel>(json);
                            if (param != null && vm.LayoutInfo != null)
                            {
                                PostToUI(() => {
                                    vm.LayoutInfo.HorizontalAlignment = param.HorizontalAlignment;
                                    vm.LayoutInfo.VerticalAlignment = param.VerticalAlignment;
                                    vm.LayoutInfo.MarginLeft = param.MarginLeft;
                                    vm.LayoutInfo.MarginTop = param.MarginTop;
                                    vm.LayoutInfo.MarginRight = param.MarginRight;
                                    vm.LayoutInfo.MarginBottom = param.MarginBottom;
                                    vm.LayoutInfo.MinWidth = param.MinWidth;
                                    vm.LayoutInfo.MaxWidth = param.MaxWidth;
                                    vm.LayoutInfo.MinHeight = param.MinHeight;
                                    vm.LayoutInfo.MaxHeight = param.MaxHeight;
                                    vm.RaisePropertyChanged(nameof(vm.LayoutInfo));
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(PasswordBoxPropertyModelEdit.LayoutInfo));
                    if (val != null)
                    {
                        var info = DeserializeObject<PasswordBoxLayoutInfo>(val);
                        if (vm.LayoutInfo != null)
                        {
                            PostToUI(() => {
                                vm.LayoutInfo.HorizontalAlignment = info.HorizontalAlignment;
                                vm.LayoutInfo.VerticalAlignment = info.VerticalAlignment;
                                vm.LayoutInfo.MarginLeft = info.MarginLeft;
                                vm.LayoutInfo.MarginTop = info.MarginTop;
                                vm.LayoutInfo.MarginRight = info.MarginRight;
                                vm.LayoutInfo.MarginBottom = info.MarginBottom;
                                vm.LayoutInfo.MinWidth = info.MinWidth;
                                vm.LayoutInfo.MaxWidth = info.MaxWidth;
                                vm.LayoutInfo.MinHeight = info.MinHeight;
                                vm.LayoutInfo.MaxHeight = info.MaxHeight;
                                vm.RaisePropertyChanged(nameof(vm.LayoutInfo));
                            });
                        }
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
                if (callBack.GetMapCellVMObjInstance() is PasswordBoxViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var json = Encoding.UTF8.GetString(cfg);
                            var param = JsonSerializer.Deserialize<CommonInfoMapOprtCellParamViewModel>(json);
                            if (param != null && vm.CommonInfo != null)
                            {
                                PostToUI(() => {
                                    vm.CommonInfo.PasswordValue = param.PasswordValue;
                                    vm.CommonInfo.CursorType = param.CursorType;
                                    vm.CommonInfo.Enabled = param.Enabled;
                                    vm.CommonInfo.PlaceholderText = param.PlaceholderText;
                                    vm.CommonInfo.PasswordVisible = param.PasswordVisible;
                                    vm.RaisePropertyChanged(nameof(vm.CommonInfo));
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(PasswordBoxPropertyModelEdit.CommonInfo));
                    if (val != null)
                    {
                        var info = DeserializeObject<PasswordBoxCommonInfo>(val);
                        if (vm.CommonInfo != null)
                        {
                            PostToUI(() => {
                                vm.CommonInfo.PasswordValue = info.PasswordValue;
                                vm.CommonInfo.CursorType = info.CursorType;
                                vm.CommonInfo.Enabled = info.Enabled;
                                vm.CommonInfo.PlaceholderText = info.PlaceholderText;
                                vm.CommonInfo.PasswordVisible = info.PasswordVisible;
                                vm.RaisePropertyChanged(nameof(vm.CommonInfo));
                            });
                        }
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
                if (callBack.GetMapCellVMObjInstance() is PasswordBoxViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var json = Encoding.UTF8.GetString(cfg);
                            var param = JsonSerializer.Deserialize<TextInfoMapOprtCellParamViewModel>(json);
                            if (param != null && vm.TextInfo != null)
                            {
                                PostToUI(() => {
                                    vm.TextInfo.FontFamily = param.FontFamily;
                                    vm.TextInfo.FontSize = param.FontSize;
                                    vm.TextInfo.IsItalic = param.IsItalic;
                                    vm.TextInfo.IsBold = param.IsBold;
                                    vm.RaisePropertyChanged(nameof(vm.TextInfo));
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(PasswordBoxPropertyModelEdit.TextInfo));
                    if (val != null)
                    {
                        var info = DeserializeObject<PasswordBoxTextInfo>(val);
                        if (vm.TextInfo != null)
                        {
                            PostToUI(() => {
                                vm.TextInfo.FontFamily = info.FontFamily;
                                vm.TextInfo.FontSize = info.FontSize;
                                vm.TextInfo.IsItalic = info.IsItalic;
                                vm.TextInfo.IsBold = info.IsBold;
                                vm.RaisePropertyChanged(nameof(vm.TextInfo));
                            });
                        }
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
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("画笔", 2)]
    [CategoryPriority("外观", 3)]
    [CategoryPriority("公共", 4)]
    [CategoryPriority("布局", 5)]
    [CategoryPriority("文本", 6)]
    public class PasswordBoxPropertyModelEdit : ControlCellPropertyModelEdit
    {
        private PasswordBoxBrushInfo _brushInfo = new PasswordBoxBrushInfo();
        private PasswordBoxAppearanceInfo _appearanceInfo = new PasswordBoxAppearanceInfo();
        private PasswordBoxCommonInfo _commonInfo = new PasswordBoxCommonInfo();
        private PasswordBoxLayoutInfo _layoutInfo = new PasswordBoxLayoutInfo();
        private PasswordBoxTextInfo _textInfo = new PasswordBoxTextInfo();

        [DisplayName("画笔设置")]
        [Category("画笔")]
        [PropertySortOrder(1)]
        public PasswordBoxBrushInfo BrushInfo
        {
            get => _brushInfo;
            set => SetProperty(ref _brushInfo, value);
        }

        [DisplayName("外观设置")]
        [Category("外观")]
        [PropertySortOrder(1)]
        public PasswordBoxAppearanceInfo AppearanceInfo
        {
            get => _appearanceInfo;
            set => SetProperty(ref _appearanceInfo, value);
        }

        [DisplayName("公共设置")]
        [Category("公共")]
        [PropertySortOrder(1)]
        public PasswordBoxCommonInfo CommonInfo
        {
            get => _commonInfo;
            set => SetProperty(ref _commonInfo, value);
        }

        [DisplayName("布局设置")]
        [Category("布局")]
        [PropertySortOrder(1)]
        public PasswordBoxLayoutInfo LayoutInfo
        {
            get => _layoutInfo;
            set => SetProperty(ref _layoutInfo, value);
        }

        [DisplayName("文本设置")]
        [Category("文本")]
        [PropertySortOrder(1)]
        public PasswordBoxTextInfo TextInfo
        {
            get => _textInfo;
            set => SetProperty(ref _textInfo, value);
        }


        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(PasswordBoxCommonInfo.PasswordValue)) == 0)
            {
                _commonInfo ??= new PasswordBoxCommonInfo();
                _commonInfo.PasswordValue = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : PasswordBoxCommonInfo.Default.PasswordValue;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }

            // 整组属性回放时，宽高主数据统一落到父类 Width/Height，LayoutInfo 仅保留旧页面兼容镜像。
            if (string.Compare(propertyID, nameof(BrushInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<PasswordBoxBrushInfo>(propertyVal) : new PasswordBoxBrushInfo();
                _brushInfo ??= new PasswordBoxBrushInfo();
                _brushInfo.BackColorStr = src.BackColorStr;
                _brushInfo.BorderColorStr = src.BorderColorStr;
                _brushInfo.ForeColorStr = src.ForeColorStr;
                _brushInfo.FocusBorderColorStr = src.FocusBorderColorStr;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(AppearanceInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<PasswordBoxAppearanceInfo>(propertyVal) : new PasswordBoxAppearanceInfo();
                _appearanceInfo ??= new PasswordBoxAppearanceInfo();
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
                var src = propertyVal != null ? DeserializeObject<PasswordBoxLayoutInfo>(propertyVal) : new PasswordBoxLayoutInfo();
                _layoutInfo ??= new PasswordBoxLayoutInfo();
                _layoutInfo.HorizontalAlignment = src.HorizontalAlignment;
                _layoutInfo.VerticalAlignment = src.VerticalAlignment;
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
                var src = propertyVal != null ? DeserializeObject<PasswordBoxCommonInfo>(propertyVal) : new PasswordBoxCommonInfo();
                _commonInfo ??= new PasswordBoxCommonInfo();
                _commonInfo.PasswordValue = src.PasswordValue;
                _commonInfo.CursorType = src.CursorType;
                _commonInfo.Enabled = src.Enabled;
                _commonInfo.PlaceholderText = src.PlaceholderText;
                _commonInfo.PasswordVisible = src.PasswordVisible;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(TextInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<PasswordBoxTextInfo>(propertyVal) : new PasswordBoxTextInfo();
                _textInfo ??= new PasswordBoxTextInfo();
                _textInfo.FontFamily = src.FontFamily;
                _textInfo.FontSize = src.FontSize;
                _textInfo.IsItalic = src.IsItalic;
                _textInfo.IsBold = src.IsBold;
                RaisePropertyChanged(nameof(TextInfo));
                return true;
            }

            if (string.Compare(propertyID, "Width") == 0)
            {
                Width = propertyVal?.ToPrimitiveValue<double>() ?? 120;
                return true;
            }
            if (string.Compare(propertyID, "Height") == 0)
            {
                Height = propertyVal?.ToPrimitiveValue<double>() ?? 50;
                return true;
            }
            if (string.Compare(propertyID, "MinWidth") == 0)
            {
                _layoutInfo.MinWidth = propertyVal?.ToPrimitiveValue<double>() ?? 0;
                return true;
            }
            if (string.Compare(propertyID, "MaxWidth") == 0)
            {
                _layoutInfo.MaxWidth = propertyVal?.ToPrimitiveValue<double>() ?? double.PositiveInfinity;
                return true;
            }
            if (string.Compare(propertyID, "MinHeight") == 0)
            {
                _layoutInfo.MinHeight = propertyVal?.ToPrimitiveValue<double>() ?? 0;
                return true;
            }
            if (string.Compare(propertyID, "MaxHeight") == 0)
            {
                _layoutInfo.MaxHeight = propertyVal?.ToPrimitiveValue<double>() ?? double.PositiveInfinity;
                return true;
            }
            if (string.Compare(propertyID, "MarginLeft") == 0)
            {
                _layoutInfo.MarginLeft = propertyVal?.ToPrimitiveValue<double>() ?? 0;
                return true;
            }
            if (string.Compare(propertyID, "MarginTop") == 0)
            {
                _layoutInfo.MarginTop = propertyVal?.ToPrimitiveValue<double>() ?? 0;
                return true;
            }
            if (string.Compare(propertyID, "MarginRight") == 0)
            {
                _layoutInfo.MarginRight = propertyVal?.ToPrimitiveValue<double>() ?? 0;
                return true;
            }
            if (string.Compare(propertyID, "MarginBottom") == 0)
            {
                _layoutInfo.MarginBottom = propertyVal?.ToPrimitiveValue<double>() ?? 0;
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

        public void CopyFrom(PasswordBoxPropertyModelEdit source)
        {
            if (source == null) return;
            _brushInfo ??= new PasswordBoxBrushInfo();
            _brushInfo.BackColorStr = source.BrushInfo?.BackColorStr ?? "#FFFFFFFF";
            _brushInfo.BorderColorStr = source.BrushInfo?.BorderColorStr ?? "#FF808080";
            _brushInfo.ForeColorStr = source.BrushInfo?.ForeColorStr ?? "#FF000000";
            _brushInfo.FocusBorderColorStr = source.BrushInfo?.FocusBorderColorStr ?? "#FF0000FF";
            RaisePropertyChanged(nameof(BrushInfo));

            _appearanceInfo ??= new PasswordBoxAppearanceInfo();
            _appearanceInfo.Opacity = source.AppearanceInfo?.Opacity ?? 1.0;
            _appearanceInfo.BorderThicknessLeft = source.AppearanceInfo?.BorderThicknessLeft ?? 1;
            _appearanceInfo.BorderThicknessTop = source.AppearanceInfo?.BorderThicknessTop ?? 1;
            _appearanceInfo.BorderThicknessRight = source.AppearanceInfo?.BorderThicknessRight ?? 1;
            _appearanceInfo.BorderThicknessBottom = source.AppearanceInfo?.BorderThicknessBottom ?? 1;
            RaisePropertyChanged(nameof(AppearanceInfo));

            _layoutInfo ??= new PasswordBoxLayoutInfo();
            // 宽高主数据统一走父类 Width/Height，LayoutInfo 不再承载宽高。
            Width = source.Width;
            Height = source.Height;
            _layoutInfo.HorizontalAlignment = source.LayoutInfo?.HorizontalAlignment ?? HorizontalAlignType.Stretch;
            _layoutInfo.VerticalAlignment = source.LayoutInfo?.VerticalAlignment ?? VerticalAlignType.Center;
            _layoutInfo.MarginLeft = source.LayoutInfo?.MarginLeft ?? 0;
            _layoutInfo.MarginTop = source.LayoutInfo?.MarginTop ?? 0;
            _layoutInfo.MarginRight = source.LayoutInfo?.MarginRight ?? 0;
            _layoutInfo.MarginBottom = source.LayoutInfo?.MarginBottom ?? 0;
            _layoutInfo.MinWidth = source.LayoutInfo?.MinWidth ?? 0;
            _layoutInfo.MaxWidth = source.LayoutInfo?.MaxWidth ?? 10000;
            _layoutInfo.MinHeight = source.LayoutInfo?.MinHeight ?? 0;
            _layoutInfo.MaxHeight = source.LayoutInfo?.MaxHeight ?? 10000;
            RaisePropertyChanged(nameof(LayoutInfo));

            _commonInfo ??= new PasswordBoxCommonInfo();
            _commonInfo.PasswordValue = source.CommonInfo?.PasswordValue ?? "";
            _commonInfo.CursorType = source.CommonInfo?.CursorType ?? CursorType.Ibeam;
            _commonInfo.Enabled = source.CommonInfo?.Enabled ?? true;
            _commonInfo.PlaceholderText = source.CommonInfo?.PlaceholderText ?? "";
            _commonInfo.PasswordVisible = source.CommonInfo?.PasswordVisible ?? false;
            RaisePropertyChanged(nameof(CommonInfo));

            _textInfo ??= new PasswordBoxTextInfo();
            _textInfo.FontFamily = source.TextInfo?.FontFamily ?? FontFamilyType.MicrosoftYaHei;
            _textInfo.FontSize = source.TextInfo?.FontSize ?? 14;
            _textInfo.IsItalic = source.TextInfo?.IsItalic ?? false;
            _textInfo.IsBold = source.TextInfo?.IsBold ?? false;
            RaisePropertyChanged(nameof(TextInfo));
        }
    }

    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("定位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class PasswordBoxPropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _passwordValue = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("密码值")]
        [Category("绑定信息")]
        [PropertySortOrder(1)]
        [BindMPPropertyID]
        public PropertyBindInfo PasswordValue
        {
            get => _passwordValue;
            set => SetProperty(ref _passwordValue, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        public void CopyFrom(PasswordBoxPropertyBindEditModel source)
        {
            base.CopyFrom(source);
            PasswordValue = source.PasswordValue;
        }
    }
}


