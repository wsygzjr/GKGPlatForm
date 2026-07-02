using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using GKG.Map.MapCell.Generic;
using GKG.Map.MapCell.Generic.Lable.MapOprtCellParamCfgView;
using GKG.Map.MapCell.Generic.Lable.View;
using GKG.Map.MapCell.Generic.Lable.ViewModel;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Lable
{
    #region 枚举定义

    public enum HorizontalAlignType
    {
        [Description("拉伸")] Stretch = 0,
        [Description("左对齐")] Left = 1,
        [Description("居中")] Center = 2,
        [Description("右对齐")] Right = 3
    }

    public enum CursorType
    {
        [Description("默认箭头")] Arrow = 0,
        [Description("文本输入")] Ibeam = 1,
        [Description("等待")] Wait = 2,
        [Description("十字")] Cross = 3,
        [Description("向上箭头")] UpArrow = 4,
        [Description("左右调整")] SizeWestEast = 5,
        [Description("上下调整")] SizeNorthSouth = 6,
        [Description("移动")] SizeAll = 7,
        [Description("禁止")] No = 8,
        [Description("手型")] Hand = 9,
        [Description("启动中")] AppStarting = 10,
        [Description("帮助")] Help = 11
    }

    public enum TextAlignType
    {
        [Description("左对齐")] Left = 0,
        [Description("居中")] Center = 1,
        [Description("右对齐")] Right = 2,
        [Description("两端对齐")] Justify = 3
    }

    public enum VerticalAlignType
    {
        [Description("拉伸")] Stretch = 0,
        [Description("顶部")] Top = 1,
        [Description("居中")] Center = 2,
        [Description("底部")] Bottom = 3
    }

    #endregion


    /// <summary>
    /// 标签图元控件对象
    /// </summary>
    class MapCellLableCtlObj : ControlCellBase
    {
        #region 私有字段

        private LableView view;
        private LableViewModel lableViewModel;

        private MapObjID _mapCellID;
        private string _mapCellName;

        private bool _loadedPropertyEditFromBytes;
        private bool _isRestoringFromSerializedState;

        #endregion

        #region 构造函数

        static MapCellLableCtlObj() { }

        public MapCellLableCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false) { }

        public MapCellLableCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            _mapCellID = mapCellID;
            _mapCellName = mapCellName;
            base.SetID(mapCellID);
            base.SetName(mapCellName);
            view = new LableView();

            // 注册对象属性
            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.BrushInfo), "画笔设置", MapCellPropDataType.Object_Json, LableBrushInfo.Object_ID, typeof(LableBrushInfo), false, true, new MapCellPropValue(LableBrushInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.AppearanceInfo), "外观设置", MapCellPropDataType.Object_Json, LableAppearanceInfo.Object_ID, typeof(LableAppearanceInfo), false, true, new MapCellPropValue(LableAppearanceInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.LayoutInfo), "布局设置", MapCellPropDataType.Object_Json, LableLayoutInfo.Object_ID, typeof(LableLayoutInfo), false, true, new MapCellPropValue(LableLayoutInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.CommonInfo), "公共设置", MapCellPropDataType.Object_Json, LableCommonInfo.Object_ID, typeof(LableCommonInfo), false, true, new MapCellPropValue(LableCommonInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.FontInfo), "字体设置", MapCellPropDataType.Object_Json, LableFontInfo.Object_ID, typeof(LableFontInfo), false, true, new MapCellPropValue(LableFontInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.ParagraphInfo), "段落设置", MapCellPropDataType.Object_Json, LableParagraphInfo.Object_ID, typeof(LableParagraphInfo), false, true, new MapCellPropValue(LableParagraphInfo.Default)));

            // 注册操作原子信息
            RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.BrushInfo_MapOprtCellID, "画笔设置操作原子", typeof(BrushInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.AppearanceInfo_MapOprtCellID, "外观设置操作原子", typeof(AppearanceInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.LayoutInfo_MapOprtCellID, "布局设置操作原子", typeof(LayoutInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.CommonInfo_MapOprtCellID, "公共设置操作原子", typeof(CommonInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.FontInfo_MapOprtCellID, "字体设置操作原子", typeof(FontInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.ParagraphInfo_MapOprtCellID, "段落设置操作原子", typeof(ParagraphInfoMapOprtCellParamCfgView)));

            // 注册操作信息
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.BrushInfo), "设置画笔", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = LableMapOprtCellConst.BrushInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.AppearanceInfo), "设置外观", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = LableMapOprtCellConst.AppearanceInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.LayoutInfo), "设置布局", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = LableMapOprtCellConst.LayoutInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.CommonInfo), "设置公共", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = LableMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.FontInfo), "设置字体", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = LableMapOprtCellConst.FontInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.ParagraphInfo), "设置段落", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = LableMapOprtCellConst.ParagraphInfo_MapOprtCellID, CfgInfo = null } }));

            (this as IMapCellTypeBase).Name = ResourceA.Lable;
            lableViewModel = new LableViewModel(designTime, LablePropertyModelEdit);
            view.DataContext = lableViewModel;

            LablePropertyModelEdit.BrushInfo.PropertyChanged += OnBrushInfoPropertyChanged;
            LablePropertyModelEdit.AppearanceInfo.PropertyChanged += OnAppearanceInfoPropertyChanged;
            LablePropertyModelEdit.LayoutInfo.PropertyChanged += OnLayoutInfoPropertyChanged;
            LablePropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
            LablePropertyModelEdit.FontInfo.PropertyChanged += OnFontInfoPropertyChanged;
            LablePropertyModelEdit.ParagraphInfo.PropertyChanged += OnParagraphInfoPropertyChanged;


        }

        private void OnBrushInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(LablePropertyModelEdit.BrushInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnAppearanceInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(LablePropertyModelEdit.AppearanceInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnLayoutInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(LablePropertyModelEdit.LayoutInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(LablePropertyModelEdit.CommonInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnFontInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(LablePropertyModelEdit.FontInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnParagraphInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(LablePropertyModelEdit.ParagraphInfo), "PropertyChanged", e?.PropertyName);
        }

        public override void OnDispose()
        {
            LablePropertyModelEdit.BrushInfo.PropertyChanged -= OnBrushInfoPropertyChanged;
            LablePropertyModelEdit.AppearanceInfo.PropertyChanged -= OnAppearanceInfoPropertyChanged;
            LablePropertyModelEdit.LayoutInfo.PropertyChanged -= OnLayoutInfoPropertyChanged;
            LablePropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;
            LablePropertyModelEdit.FontInfo.PropertyChanged -= OnFontInfoPropertyChanged;
            LablePropertyModelEdit.ParagraphInfo.PropertyChanged -= OnParagraphInfoPropertyChanged;

            view.DataContext = null;
            lableViewModel?.Dispose();
            lableViewModel = null;

            base.OnDispose();
        }

        #endregion

        #region 属性

        public LablePropertyModelEdit LablePropertyModelEdit => PropertyEditModelBase as LablePropertyModelEdit;

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
            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoadedLabel(propertyID, propertyVal))
            {
                return true;
            }
            LablePropertyModelEdit.IsRuning = isRuning;
            var ok = LablePropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
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

            if (LablePropertyModelEdit.IsRuning)
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

        private void ExecuteOprtByPropertyId(string propertyID, string trigger, string? changedProp)
        {
            if (string.IsNullOrWhiteSpace(propertyID))
                return;

            var normalized = propertyID;
            var dot = normalized.IndexOf('.');
            if (dot > 0)
                normalized = normalized.Substring(0, dot);

            if (!TryGetPrimaryOprtCellId(normalized, out var normalizedOprtId, out var primaryOprtCellId))
                return;

            TryExecuteOprtInfoById(normalizedOprtId, primaryOprtCellId);
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
            if (string.Equals(oprtId, nameof(LableCommonInfo.LableValue), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(LablePropertyModelEdit.CommonInfo);
                oprtCellId = LableMapOprtCellConst.CommonInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(LableBrushInfo.ForeColor), StringComparison.Ordinal)
                || string.Equals(oprtId, nameof(LableBrushInfo.BackColor), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(LablePropertyModelEdit.BrushInfo);
                oprtCellId = LableMapOprtCellConst.BrushInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(LableAppearanceInfo.Opacity), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(LablePropertyModelEdit.AppearanceInfo);
                oprtCellId = LableMapOprtCellConst.AppearanceInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(LablePropertyModelEdit.BrushInfo), StringComparison.Ordinal))
            {
                oprtCellId = LableMapOprtCellConst.BrushInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(LablePropertyModelEdit.AppearanceInfo), StringComparison.Ordinal))
            {
                oprtCellId = LableMapOprtCellConst.AppearanceInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(LablePropertyModelEdit.LayoutInfo), StringComparison.Ordinal))
            {
                oprtCellId = LableMapOprtCellConst.LayoutInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(LablePropertyModelEdit.CommonInfo), StringComparison.Ordinal))
            {
                oprtCellId = LableMapOprtCellConst.CommonInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(LablePropertyModelEdit.FontInfo), StringComparison.Ordinal))
            {
                oprtCellId = LableMapOprtCellConst.FontInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(LablePropertyModelEdit.ParagraphInfo), StringComparison.Ordinal))
            {
                oprtCellId = LableMapOprtCellConst.ParagraphInfo_MapOprtCellID;
                return true;
            }
            return false;
        }

        private bool TryExecuteOprtInfoById(string oprtId, MapOprtCellID primaryOprtCellId)
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
                    if (p == null) continue;
                    var v = p.GetValue(oprtInfo);
                    if (v is string s && !string.IsNullOrWhiteSpace(s))
                        return s;
                }
            }
            catch { }
            return null;
        }

        private static IEnumerable? GetOprtInfoInstList(object oprtInfo)
        {
            try
            {
                return EnumerateOprtInfoInsts(oprtInfo).Select(x => x.inst).ToList();
            }
            catch { }
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
                if (val == null)
                    yield break;

                if (val is IEnumerable e)
                {
                    int idx = 0;
                    foreach (var item in e)
                    {
                        if (item is not MapOprtCellInstInfo inst)
                            continue;
                        idx++;
                        if (!TryKey(inst, source, idx, out var key))
                            continue;
                        if (!seen.Add(key))
                            continue;
                        yield return (inst, source);
                    }
                }
            }

            foreach (var p in t.GetProperties(flags))
            {
                if (p.GetIndexParameters().Length != 0)
                    continue;
                if (!p.CanRead)
                    continue;

                var pt = p.PropertyType;
                if (pt == typeof(MapOprtCellInstInfoList))
                {
                    foreach (var x in EnumerateFromValue(p.GetValue(oprtInfo), $"prop:{p.Name}"))
                        yield return x;
                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(pt))
                {
                    foreach (var x in EnumerateFromValue(p.GetValue(oprtInfo), $"prop:{p.Name}"))
                        yield return x;
                }
            }

            foreach (var f in t.GetFields(flags))
            {
                if (f.IsStatic)
                    continue;

                var ft = f.FieldType;
                if (ft == typeof(MapOprtCellInstInfoList))
                {
                    foreach (var x in EnumerateFromValue(f.GetValue(oprtInfo), $"field:{f.Name}"))
                        yield return x;
                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(ft))
                {
                    foreach (var x in EnumerateFromValue(f.GetValue(oprtInfo), $"field:{f.Name}"))
                        yield return x;
                }
            }
        }

        private bool IsDefaultOverwriteForLoadedLabel(string propertyID, MapCellPropValue propertyVal)
        {
            try
            {
                // only guard label's own complex properties
                if (string.Compare(propertyID, nameof(LablePropertyModelEdit.BrushInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<LableBrushInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(LablePropertyModelEdit.BrushInfo);
                }
                if (string.Compare(propertyID, nameof(LablePropertyModelEdit.AppearanceInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<LableAppearanceInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(LablePropertyModelEdit.AppearanceInfo);
                }
                if (string.Compare(propertyID, nameof(LablePropertyModelEdit.LayoutInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<LableLayoutInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(LablePropertyModelEdit.LayoutInfo);
                }
                if (string.Compare(propertyID, nameof(LablePropertyModelEdit.CommonInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<LableCommonInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(LablePropertyModelEdit.CommonInfo);
                }
                if (string.Compare(propertyID, nameof(LablePropertyModelEdit.FontInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<LableFontInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(LablePropertyModelEdit.FontInfo);
                }
                if (string.Compare(propertyID, nameof(LablePropertyModelEdit.ParagraphInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<LableParagraphInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(LablePropertyModelEdit.ParagraphInfo);
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static bool IsDefault(LableBrushInfo? info)
        {
            if (info == null) return true;
            return string.Equals(info.BackColorStr ?? "", LableBrushInfo.Default.BackColorStr, StringComparison.OrdinalIgnoreCase)
                && string.Equals(info.ForeColorStr ?? "", LableBrushInfo.Default.ForeColorStr, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsDefault(LableAppearanceInfo? info)
        {
            if (info == null) return true;
            return Math.Abs(info.Opacity - LableAppearanceInfo.Default.Opacity) < 0.000001
                && Math.Abs(info.BorderThicknessLeft - LableAppearanceInfo.Default.BorderThicknessLeft) < 0.000001
                && Math.Abs(info.BorderThicknessTop - LableAppearanceInfo.Default.BorderThicknessTop) < 0.000001
                && Math.Abs(info.BorderThicknessRight - LableAppearanceInfo.Default.BorderThicknessRight) < 0.000001
                && Math.Abs(info.BorderThicknessBottom - LableAppearanceInfo.Default.BorderThicknessBottom) < 0.000001;
        }

        private static bool IsDefault(LableLayoutInfo? info)
        {
            if (info == null) return true;
            return info.HorizontalAlign == LableLayoutInfo.Default.HorizontalAlign
                && info.VerticalAlign == LableLayoutInfo.Default.VerticalAlign;
        }

        private static bool IsDefault(LableCommonInfo? info)
        {
            if (info == null) return true;
            return string.Equals(info.LableValue ?? "", LableCommonInfo.Default.LableValue ?? "", StringComparison.Ordinal)
                && info.CursorType == LableCommonInfo.Default.CursorType
                && info.IsEnabled == LableCommonInfo.Default.IsEnabled
                && string.Equals(info.ToolTip ?? "", LableCommonInfo.Default.ToolTip ?? "", StringComparison.Ordinal);
        }

        private static bool IsDefault(LableFontInfo? info)
        {
            if (info == null) return true;
            return string.Equals(info.FontColorStr ?? "", LableFontInfo.Default.FontColorStr, StringComparison.OrdinalIgnoreCase)
                && Math.Abs(info.FontSize - LableFontInfo.Default.FontSize) < 0.000001
                && info.IsBold == LableFontInfo.Default.IsBold
                && info.IsItalic == LableFontInfo.Default.IsItalic
                && info.IsUnderline == LableFontInfo.Default.IsUnderline;
        }

        private static bool IsDefault(LableParagraphInfo? info)
        {
            if (info == null) return true;
            return Math.Abs(info.LineHeight - LableParagraphInfo.Default.LineHeight) < 0.000001
                && Math.Abs(info.ParagraphSpacingBefore - LableParagraphInfo.Default.ParagraphSpacingBefore) < 0.000001
                && Math.Abs(info.ParagraphSpacingAfter - LableParagraphInfo.Default.ParagraphSpacingAfter) < 0.000001
                && info.TextAlignment == LableParagraphInfo.Default.TextAlignment
                && info.VerticalTextAlignment == LableParagraphInfo.Default.VerticalTextAlignment;
        }

        #endregion

        #region 序列化方法

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);

            string propertyEditJson = br.ReadString("PropertyEditModelBase");
            if (!string.IsNullOrEmpty(propertyEditJson))
            {
                // 读档恢复属性模型时先暂停自动回放，等所有值就位后再同步刷新一次界面。
                _isRestoringFromSerializedState = true;
                try
                {
                    var propertyEditModelBase = JsonObjConvert.FromJSon<LablePropertyModelEdit>(propertyEditJson);
                    if (propertyEditModelBase != null)
                    {
                        (PropertyEditModelBase as LablePropertyModelEdit).CopyFrom(propertyEditModelBase);
                        _loadedPropertyEditFromBytes = true;
                    }
                    else
                    {
                        var fallback = TryParseLablePropertyModelEdit(propertyEditJson);
                        if (fallback != null)
                        {
                            (PropertyEditModelBase as LablePropertyModelEdit).CopyFrom(fallback);
                            _loadedPropertyEditFromBytes = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var fallback = TryParseLablePropertyModelEdit(propertyEditJson);
                    if (fallback != null)
                    {
                        (PropertyEditModelBase as LablePropertyModelEdit).CopyFrom(fallback);
                        _loadedPropertyEditFromBytes = true;
                    }
                }
                _isRestoringFromSerializedState = false;
            }

            string propertyBindJson = br.ReadString("PropertyBindEditModelBase");
            if (!string.IsNullOrEmpty(propertyBindJson))
            {
                try
                {
                    var propertyBindEditModelBase = JsonObjConvert.FromJSon<LablePropertyBindEditModel>(propertyBindJson);
                    if (propertyBindEditModelBase != null)
                    {
                        (PropertyBindEditModelBase as LablePropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            string eventBindJson = br.ReadString("EventBindEditModel");
            if (!string.IsNullOrEmpty(eventBindJson))
            {
                try
                {
                    var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(eventBindJson);
                    if (eventBindEditModel != null)
                    {
                        EventBindEditModel.CopyFrom(eventBindEditModel);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (_loadedPropertyEditFromBytes)
            {
                ReloadLableViewSynchronously();
            }
        }

        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));

        }

        private static LablePropertyModelEdit? TryParseLablePropertyModelEdit(string json)
        {
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;
                var model = new LablePropertyModelEdit();

                static bool TryGetObject(System.Text.Json.JsonElement rootEl, string name, out System.Text.Json.JsonElement obj)
                {
                    if (rootEl.TryGetProperty(name, out obj) && obj.ValueKind == System.Text.Json.JsonValueKind.Object)
                        return true;
                    obj = default;
                    return false;
                }

                static string? GetString(System.Text.Json.JsonElement obj, params string[] names)
                {
                    foreach (var n in names)
                    {
                        if (obj.TryGetProperty(n, out var v))
                        {
                            if (v.ValueKind == System.Text.Json.JsonValueKind.String)
                                return v.GetString();
                            if (v.ValueKind == System.Text.Json.JsonValueKind.Number)
                                return v.GetRawText();
                        }
                    }
                    return null;
                }

                static double? GetDouble(System.Text.Json.JsonElement obj, params string[] names)
                {
                    foreach (var n in names)
                    {
                        if (obj.TryGetProperty(n, out var v))
                        {
                            if (v.ValueKind == System.Text.Json.JsonValueKind.Number)
                                return v.GetDouble();
                            if (v.ValueKind == System.Text.Json.JsonValueKind.String && double.TryParse(v.GetString(), out var d))
                                return d;
                        }
                    }
                    return null;
                }

                static bool? GetBool(System.Text.Json.JsonElement obj, params string[] names)
                {
                    foreach (var n in names)
                    {
                        if (obj.TryGetProperty(n, out var v))
                        {
                            if (v.ValueKind == System.Text.Json.JsonValueKind.True) return true;
                            if (v.ValueKind == System.Text.Json.JsonValueKind.False) return false;
                            if (v.ValueKind == System.Text.Json.JsonValueKind.String && bool.TryParse(v.GetString(), out var b))
                                return b;
                        }
                    }
                    return null;
                }

                static TEnum? GetEnum<TEnum>(System.Text.Json.JsonElement obj, params string[] names) where TEnum : struct
                {
                    foreach (var n in names)
                    {
                        if (!obj.TryGetProperty(n, out var v))
                            continue;

                        if (v.ValueKind == System.Text.Json.JsonValueKind.Number)
                        {
                            if (v.TryGetInt32(out var i) && Enum.IsDefined(typeof(TEnum), i))
                                return (TEnum)Enum.ToObject(typeof(TEnum), i);
                        }
                        else if (v.ValueKind == System.Text.Json.JsonValueKind.String)
                        {
                            var s = v.GetString();
                            if (!string.IsNullOrWhiteSpace(s) && Enum.TryParse<TEnum>(s, out var e))
                                return e;
                        }
                    }
                    return null;
                }

                if (TryGetObject(root, "BrushInfo", out var brush) || TryGetObject(root, "_brushInfo", out brush))
                {
                    model.BrushInfo.BackColorStr = GetString(brush, "BackColorStr", "BackColor") ?? model.BrushInfo.BackColorStr;
                    model.BrushInfo.ForeColorStr = GetString(brush, "ForeColorStr", "ForeColor") ?? model.BrushInfo.ForeColorStr;
                }

                if (TryGetObject(root, "AppearanceInfo", out var appearance) || TryGetObject(root, "_appearanceInfo", out appearance))
                {
                    model.AppearanceInfo.Opacity = GetDouble(appearance, "Opacity") ?? model.AppearanceInfo.Opacity;
                    model.AppearanceInfo.BorderThicknessLeft = GetDouble(appearance, "BorderThicknessLeft") ?? model.AppearanceInfo.BorderThicknessLeft;
                    model.AppearanceInfo.BorderThicknessTop = GetDouble(appearance, "BorderThicknessTop") ?? model.AppearanceInfo.BorderThicknessTop;
                    model.AppearanceInfo.BorderThicknessRight = GetDouble(appearance, "BorderThicknessRight") ?? model.AppearanceInfo.BorderThicknessRight;
                    model.AppearanceInfo.BorderThicknessBottom = GetDouble(appearance, "BorderThicknessBottom") ?? model.AppearanceInfo.BorderThicknessBottom;
                }

                if (TryGetObject(root, "LayoutInfo", out var layout) || TryGetObject(root, "_layoutInfo", out layout))
                {
                    var legacyWidth = GetDouble(layout, "Width");
                    var legacyHeight = GetDouble(layout, "Height");
                    // 旧页面仍可能把宽高写在 LayoutInfo 里，这里读取后统一迁入父类 Width/Height。
                    model.Width = legacyWidth ?? model.Width;
                    model.Height = legacyHeight ?? model.Height;
                    model.LayoutInfo.HorizontalAlign = GetEnum<HorizontalAlignType>(layout, "HorizontalAlign") ?? model.LayoutInfo.HorizontalAlign;
                    model.LayoutInfo.VerticalAlign = GetEnum<VerticalAlignType>(layout, "VerticalAlign") ?? model.LayoutInfo.VerticalAlign;
                }

                if (TryGetObject(root, "CommonInfo", out var common) || TryGetObject(root, "_commonInfo", out common))
                {
                    model.CommonInfo.LableValue = GetString(common, "LableValue") ?? model.CommonInfo.LableValue;
                    model.CommonInfo.CursorType = GetEnum<CursorType>(common, "CursorType") ?? model.CommonInfo.CursorType;
                    model.CommonInfo.IsEnabled = GetBool(common, "IsEnabled") ?? model.CommonInfo.IsEnabled;
                    model.CommonInfo.ToolTip = GetString(common, "ToolTip") ?? model.CommonInfo.ToolTip;
                }

                if (TryGetObject(root, "FontInfo", out var font) || TryGetObject(root, "_fontInfo", out font))
                {
                    model.FontInfo.FontColorStr = GetString(font, "FontColorStr", "FontColor") ?? model.FontInfo.FontColorStr;
                    model.FontInfo.FontSize = GetDouble(font, "FontSize") ?? model.FontInfo.FontSize;
                    model.FontInfo.IsBold = GetBool(font, "IsBold") ?? model.FontInfo.IsBold;
                    model.FontInfo.IsItalic = GetBool(font, "IsItalic") ?? model.FontInfo.IsItalic;
                    model.FontInfo.IsUnderline = GetBool(font, "IsUnderline") ?? model.FontInfo.IsUnderline;
                }

                if (TryGetObject(root, "ParagraphInfo", out var paragraph) || TryGetObject(root, "_paragraphInfo", out paragraph))
                {
                    model.ParagraphInfo.LineHeight = GetDouble(paragraph, "LineHeight") ?? model.ParagraphInfo.LineHeight;
                    model.ParagraphInfo.ParagraphSpacingBefore = GetDouble(paragraph, "ParagraphSpacingBefore") ?? model.ParagraphInfo.ParagraphSpacingBefore;
                    model.ParagraphInfo.ParagraphSpacingAfter = GetDouble(paragraph, "ParagraphSpacingAfter") ?? model.ParagraphInfo.ParagraphSpacingAfter;
                    model.ParagraphInfo.TextAlignment = GetEnum<TextAlignType>(paragraph, "TextAlignment") ?? model.ParagraphInfo.TextAlignment;
                    model.ParagraphInfo.VerticalTextAlignment = GetEnum<TextVerticalAlignType>(paragraph, "VerticalTextAlignment") ?? model.ParagraphInfo.VerticalTextAlignment;
                }

                return model;
            }
            catch
            {
                return null;
            }
        }

        #endregion


        #region 重写方法

        protected override void OnCopyFrom(ControlCellBase source)
        {
            MapCellLableCtlObj mapCellLableCtlObj = (source as MapCellLableCtlObj);
            base._CopyFrom(mapCellLableCtlObj);
            // 复制整组属性时先抑制逐项回放，避免运行时第一页先闪默认态再恢复。
            _isRestoringFromSerializedState = true;
            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
            _isRestoringFromSerializedState = false;
            (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel.CopyFrom(source.EventBindEditModel);

            if (mapCellLableCtlObj != null)
            {
                TryCopyOperationData(mapCellLableCtlObj, this);
            }

            ReloadLableViewSynchronously();
        }

        private static void TryCopyOperationData(object source, object target)
        {
            try
            {
                var sourceType = source.GetType();
                var targetType = target.GetType();

                foreach (var member in EnumerateInstanceMembers(sourceType))
                {
                    var memberType = GetMemberType(member);
                    if (memberType == null)
                        continue;

                    if (!IsOperationDataContainerType(memberType))
                        continue;

                    var srcVal = GetMemberValue(member, source);
                    if (srcVal == null)
                        continue;

                    var dstMember = FindMember(targetType, member);
                    if (dstMember == null)
                        continue;

                    var dstVal = GetMemberValue(dstMember, target);
                    if (dstVal != null)
                    {
                        if (dstVal is IDictionary dstDict && srcVal is IDictionary srcDict)
                        {
                            dstDict.Clear();
                            foreach (DictionaryEntry entry in srcDict)
                                dstDict[entry.Key] = entry.Value;
                            continue;
                        }

                        if (dstVal is IList dstList && srcVal is IEnumerable srcEnum)
                        {
                            dstList.Clear();
                            foreach (var item in srcEnum)
                                dstList.Add(item);
                            continue;
                        }
                    }

                    SetMemberValue(dstMember, target, srcVal);
                }
            }
            catch
            {
            }
        }

        private static IEnumerable<MemberInfo> EnumerateInstanceMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            for (var t = type; t != null; t = t.BaseType)
            {
                foreach (var f in t.GetFields(flags))
                {
                    if (f.IsStatic)
                        continue;
                    yield return f;
                }

                foreach (var p in t.GetProperties(flags))
                {
                    if (p.GetIndexParameters().Length != 0)
                        continue;
                    if (!p.CanRead)
                        continue;
                    yield return p;
                }
            }
        }

        private static MemberInfo? FindMember(Type targetType, MemberInfo sourceMember)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            for (var t = targetType; t != null; t = t.BaseType)
            {
                if (sourceMember is FieldInfo sf)
                {
                    var tf = t.GetField(sf.Name, flags);
                    if (tf != null && tf.FieldType == sf.FieldType)
                        return tf;
                }
                else if (sourceMember is PropertyInfo sp)
                {
                    var tp = t.GetProperty(sp.Name, flags);
                    if (tp != null && tp.PropertyType == sp.PropertyType && tp.CanWrite)
                        return tp;
                }
            }
            return null;
        }

        private static Type? GetMemberType(MemberInfo member)
        {
            return member switch
            {
                FieldInfo f => f.FieldType,
                PropertyInfo p => p.PropertyType,
                _ => null,
            };
        }

        private static object? GetMemberValue(MemberInfo member, object instance)
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

        private static void SetMemberValue(MemberInfo member, object instance, object value)
        {
            try
            {
                if (member is FieldInfo f)
                {
                    f.SetValue(instance, value);
                    return;
                }
                if (member is PropertyInfo p)
                {
                    p.SetValue(instance, value);
                }
            }
            catch
            {
            }
        }

        private static bool IsOperationDataContainerType(Type t)
        {
            try
            {
                if (t == typeof(MapOprtCellInstInfoList))
                    return true;

                if (t.IsGenericType)
                {
                    foreach (var arg in t.GetGenericArguments())
                    {
                        if (arg == typeof(MapOprtInfo)
                            || arg == typeof(MapOprtCellInstInfo)
                            || arg == typeof(MapOprtCellInfo)
                            || arg == typeof(MapOprtCellInstInfoList))
                        {
                            return true;
                        }
                    }
                }

                return typeof(IEnumerable).IsAssignableFrom(t)
                    && (t.FullName?.IndexOf("MapOprt", StringComparison.OrdinalIgnoreCase) >= 0
                        || t.FullName?.IndexOf("OprtCell", StringComparison.OrdinalIgnoreCase) >= 0);
            }
            catch
            {
                return false;
            }
        }

        protected override void OnInit()
        {
            base.OnInit();

            if (_loadedPropertyEditFromBytes)
            {
                ReloadLableViewSynchronously();
                return;
            }

            ReloadLableViewSynchronously();
        }

        /// <summary>
        /// 让标签图元在页面切换首帧前一次性收敛到真实状态，避免文本晚一拍才出现。
        /// </summary>
        private void ReloadLableViewSynchronously()
        {
            ExecuteOnUiThreadSynchronously(() =>
            {
                lableViewModel?.ReloadFromModel();
                view?.ApplyInitialStateFromViewModel();
            });
        }

        /// <summary>
        /// 读档完成后直接同步刷新 UI，避免再排队到下一拍导致首帧缺字。
        /// </summary>
        private static void ExecuteOnUiThreadSynchronously(Action action)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                action();
                return;
            }

            Dispatcher.UIThread.InvokeAsync(action).GetAwaiter().GetResult();
        }


        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => lableViewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new LablePropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new LablePropertyBindEditModel();

        public override EventBindEditModel CreateEventBindEditModel() => new EventBindEditModel() { EventCmdInfos = new BindingList<EventCmdInfo>() };

        public override void OnZoomChanged() { }

        public override string ToString() => "标签";

        #endregion

        #region 操作原子执行

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.BrushInfo_MapOprtCellID)
                return ExecuteOprtCell<BrushInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.AppearanceInfo_MapOprtCellID)
                return ExecuteOprtCell<AppearanceInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.LayoutInfo_MapOprtCellID)
                return ExecuteOprtCell<LayoutInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.CommonInfo_MapOprtCellID)
                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.FontInfo_MapOprtCellID)
                return ExecuteOprtCell<FontInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.ParagraphInfo_MapOprtCellID)
                return ExecuteOprtCell<ParagraphInfoMapOprtCellExector>(mapOprtCellInstInfo);
            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        private bool ExecuteOprtCell<T>(MapOprtCellInstInfo mapOprtCellInstInfo) where T : IMapOprtCellExector, new()
        {
            if (!MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector)
                || mapOprtCellExector == null
                || mapOprtCellExector.GetType() != typeof(T))
            {
                mapOprtCellExector = new T();
                mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
            }

            mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
            return true;
        }

        #endregion

        #region 操作原子执行对象

        private static void PostToUI(Action action)
        {
            if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                action();
            }
            else
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(action);
            }
        }

        private class BrushInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is LableViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<BrushInfoMapOprtCellParamViewModel>(System.Text.Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                var back = Color.Parse(param.BackColor);
                                var fore = Color.Parse(param.ForeColor);
                                PostToUI(() =>
                                {
                                    vm.BackColor = back;
                                    vm.LableColor = fore;
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(LablePropertyModelEdit.BrushInfo));
                    if (val != null)
                    {
                        try
                        {
                            var brushInfo = DeserializeObject<LableBrushInfo>(val);
                            var back = brushInfo.BackColor;
                            var fore = brushInfo.ForeColor;
                            PostToUI(() =>
                            {
                                vm.BackColor = back;
                                vm.LableColor = fore;
                            });
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
                if (callBack.GetMapCellVMObjInstance() is LableViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<AppearanceInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                double opacity = 1.0;
                                double borderLeft = 0;
                                double borderTop = 0;
                                double borderRight = 0;
                                double borderBottom = 0;
                                if (!double.TryParse(param.Opacity, out opacity))
                                    opacity = 1.0;
                                double.TryParse(param.BorderThicknessLeft, out borderLeft);
                                double.TryParse(param.BorderThicknessTop, out borderTop);
                                double.TryParse(param.BorderThicknessRight, out borderRight);
                                double.TryParse(param.BorderThicknessBottom, out borderBottom);
                                PostToUI(() =>
                                {
                                    vm.Opacity = opacity;
                                    vm.BorderThicknessLeft = borderLeft;
                                    vm.BorderThicknessTop = borderTop;
                                    vm.BorderThicknessRight = borderRight;
                                    vm.BorderThicknessBottom = borderBottom;
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(LablePropertyModelEdit.AppearanceInfo));
                    if (val != null)
                    {
                        try
                        {
                            var appearanceInfo = DeserializeObject<LableAppearanceInfo>(val);
                            var opacity = appearanceInfo.Opacity;
                            PostToUI(() =>
                            {
                                vm.Opacity = opacity;
                                vm.BorderThicknessLeft = appearanceInfo.BorderThicknessLeft;
                                vm.BorderThicknessTop = appearanceInfo.BorderThicknessTop;
                                vm.BorderThicknessRight = appearanceInfo.BorderThicknessRight;
                                vm.BorderThicknessBottom = appearanceInfo.BorderThicknessBottom;
                            });
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
                if (callBack.GetMapCellVMObjInstance() is LableViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<LayoutInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                var ha = param.HorizontalAlign;
                                var va = param.VerticalAlign;
                                PostToUI(() =>
                                {
                                    vm.HorizontalAlign = ha;
                                    vm.VerticalAlign = va;
                                });
                                return;
                            }
                        }
                        catch
                        {
                        }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(LablePropertyModelEdit.LayoutInfo));
                    if (val != null)
                    {
                        try
                        {
                            var layoutInfo = DeserializeObject<LableLayoutInfo>(val);
                            var ha = layoutInfo.HorizontalAlign;
                            var va = layoutInfo.VerticalAlign;
                            PostToUI(() =>
                            {
                                vm.HorizontalAlign = ha;
                                vm.VerticalAlign = va;
                            });
                        }
                        catch
                        {
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
                // 如果没有配置信息，跳过执行，避免覆盖其他有配置的同类型原子的设置
                if (callBack.GetMapCellVMObjInstance() is LableViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<CommonInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                string lableValue = param.LableValue;
                                var cursorType = param.CursorType;
                                bool isEnabled = param.IsEnabled;
                                string toolTip = param.ToolTip;
                                PostToUI(() =>
                                {
                                    vm.LableText = lableValue;
                                    vm.CursorType = cursorType;
                                    vm.IsEnabled = isEnabled;
                                    vm.ToolTip = toolTip;
                                });
                                return;
                            }
                        }
                        catch
                        {
                        }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(LablePropertyModelEdit.CommonInfo));
                    if (val != null)
                    {
                        try
                        {
                            var commonInfo = DeserializeObject<LableCommonInfo>(val);
                            var text = commonInfo.LableValue;
                            var cursor = commonInfo.CursorType;
                            var isEnabled = commonInfo.IsEnabled;
                            var tip = commonInfo.ToolTip;
                            PostToUI(() =>
                            {
                                vm.LableText = text;
                                vm.CursorType = cursor;
                                vm.IsEnabled = isEnabled;
                                vm.ToolTip = tip;
                            });
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private class FontInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                // 如果没有配置信息，跳过执行，避免覆盖其他有配置的同类型原子的设置
                if (callBack.GetMapCellVMObjInstance() is LableViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<FontInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                double fontSize = 14;
                                if (!double.TryParse(param.FontSize, out fontSize))
                                    fontSize = 14;
                                var color = Color.Parse(param.FontColor);
                                var isBold = param.IsBold;
                                var isItalic = param.IsItalic;
                                var isUnderline = param.IsUnderline;
                                PostToUI(() =>
                                {
                                    vm.LableColor = color;
                                    vm.FontSize = fontSize;
                                    vm.IsBold = isBold;
                                    vm.IsItalic = isItalic;
                                    vm.IsUnderline = isUnderline;
                                });
                                return;
                            }
                        }
                        catch
                        {
                        }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(LablePropertyModelEdit.FontInfo));
                    if (val != null)
                    {
                        try
                        {
                            var fontInfo = DeserializeObject<LableFontInfo>(val);
                            var color = fontInfo.FontColor;
                            var size = fontInfo.FontSize;
                            var bold = fontInfo.IsBold;
                            var italic = fontInfo.IsItalic;
                            var underline = fontInfo.IsUnderline;
                            PostToUI(() =>
                            {
                                vm.LableColor = color;
                                vm.FontSize = size;
                                vm.IsBold = bold;
                                vm.IsItalic = italic;
                                vm.IsUnderline = underline;
                            });
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private class ParagraphInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                // 如果没有配置信息，跳过执行，避免覆盖其他有配置的同类型原子的设置
                if (callBack.GetMapCellVMObjInstance() is LableViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<ParagraphInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                double lineHeight = 1;
                                double spacingBefore = 0;
                                double spacingAfter = 0;
                                var textAlignment = param.TextAlignment;
                                var verticalTextAlignment = param.VerticalTextAlignment;
                                if (double.TryParse(param.LineHeight, out lineHeight))
                                {
                                }
                                if (double.TryParse(param.ParagraphSpacingBefore, out spacingBefore))
                                {
                                }
                                if (double.TryParse(param.ParagraphSpacingAfter, out spacingAfter))
                                {
                                }
                                PostToUI(() =>
                                {
                                    vm.LineHeight = lineHeight;
                                    vm.ParagraphSpacingBefore = spacingBefore;
                                    vm.ParagraphSpacingAfter = spacingAfter;
                                    vm.TextAlignment = textAlignment;
                                    vm.VerticalTextAlignment = verticalTextAlignment;
                                });
                                return;
                            }
                        }
                        catch
                        {
                        }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(LablePropertyModelEdit.ParagraphInfo));
                    if (val != null)
                    {
                        try
                        {
                            var paragraphInfo = DeserializeObject<LableParagraphInfo>(val);
                            var lh = paragraphInfo.LineHeight;
                            var sb = paragraphInfo.ParagraphSpacingBefore;
                            var sa = paragraphInfo.ParagraphSpacingAfter;
                            var ta = paragraphInfo.TextAlignment;
                            var vta = paragraphInfo.VerticalTextAlignment;
                            PostToUI(() =>
                            {
                                vm.LineHeight = lh;
                                vm.ParagraphSpacingBefore = sb;
                                vm.ParagraphSpacingAfter = sa;
                                vm.TextAlignment = ta;
                                vm.VerticalTextAlignment = vta;
                            });
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 反序列化对象属性值
        /// </summary>
        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }

        private static double ParseDoubleOrDefault(string? val, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(val))
                return defaultValue;
            if (double.TryParse(val, out var d))
                return d;
            return defaultValue;
        }

        #endregion
    }


    /// <summary>
    /// 标签属性编辑模型对象
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("画笔", 1)]
    [CategoryPriority("外观", 2)]
    [CategoryPriority("布局", 3)]
    [CategoryPriority("公共", 4)]
    [CategoryPriority("文本.字体", 5)]
    [CategoryPriority("文本.段落", 6)]
    public class LablePropertyModelEdit : ControlCellPropertyModelEdit
    {
        #region 构造函数

        public LablePropertyModelEdit()
        {
            // 注意：不要订阅嵌套对象的 PropertyChanged 事件
            // ExpandableObjectConverter 已经能够正确处理嵌套对象的属性变化
            // 订阅会导致无限循环，造成界面卡死
        }

        #endregion

        #region 对象属性

        private LableBrushInfo _brushInfo = new LableBrushInfo();
        [DisplayName("画笔设置")]
        [Category("画笔")]
        [PropertySortOrder(1)]
        public LableBrushInfo BrushInfo
        {
            get => _brushInfo;
            set => SetProperty(ref _brushInfo, value);
        }

        private LableAppearanceInfo _appearanceInfo = new LableAppearanceInfo();
        [DisplayName("外观设置")]
        [Category("外观")]
        [PropertySortOrder(1)]
        public LableAppearanceInfo AppearanceInfo
        {
            get => _appearanceInfo;
            set => SetProperty(ref _appearanceInfo, value);
        }

        private LableLayoutInfo _layoutInfo = new LableLayoutInfo();
        [DisplayName("布局设置")]
        [Category("布局")]
        [PropertySortOrder(1)]
        public LableLayoutInfo LayoutInfo
        {
            get => _layoutInfo;
            set => SetProperty(ref _layoutInfo, value);
        }

        private LableCommonInfo _commonInfo = new LableCommonInfo();
        [DisplayName("公共设置")]
        [Category("公共")]
        [PropertySortOrder(1)]
        public LableCommonInfo CommonInfo
        {
            get => _commonInfo;
            set => SetProperty(ref _commonInfo, value);
        }

        private LableFontInfo _fontInfo = new LableFontInfo();
        [DisplayName("字体设置")]
        [Category("文本.字体")]
        [PropertySortOrder(1)]
        public LableFontInfo FontInfo
        {
            get => _fontInfo;
            set => SetProperty(ref _fontInfo, value);
        }

        private LableParagraphInfo _paragraphInfo = new LableParagraphInfo();
        [DisplayName("段落设置")]
        [Category("文本.段落")]
        [PropertySortOrder(1)]
        public LableParagraphInfo ParagraphInfo
        {
            get => _paragraphInfo;
            set => SetProperty(ref _paragraphInfo, value);
        }

        #endregion


        #region SetPropertyValue 方法

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(LableCommonInfo.LableValue)) == 0)
            {
                _commonInfo ??= new LableCommonInfo();
                _commonInfo.LableValue = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : LableCommonInfo.Default.LableValue;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(LableBrushInfo.ForeColor)) == 0)
            {
                _brushInfo ??= new LableBrushInfo();
                if (propertyVal != null)
                {
                    var colorStr = propertyVal.ToPrimitiveValue<string>();
                    _brushInfo.ForeColorStr = Color.Parse(colorStr).ToColorString();
                }
                else
                {
                    _brushInfo.ForeColorStr = LableBrushInfo.Default.ForeColorStr;
                }
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(LableBrushInfo.BackColor)) == 0)
            {
                _brushInfo ??= new LableBrushInfo();
                if (propertyVal != null)
                {
                    var colorStr = propertyVal.ToPrimitiveValue<string>();
                    _brushInfo.BackColorStr = Color.Parse(colorStr).ToColorString();
                }
                else
                {
                    _brushInfo.BackColorStr = LableBrushInfo.Default.BackColorStr;
                }
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(LableAppearanceInfo.BorderThicknessLeft)) == 0)
            {
                _appearanceInfo ??= new LableAppearanceInfo();
                _appearanceInfo.BorderThicknessLeft = propertyVal != null ? propertyVal.ToPrimitiveValue<double>() : LableAppearanceInfo.Default.BorderThicknessLeft;
                RaisePropertyChanged(nameof(AppearanceInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(LableAppearanceInfo.BorderThicknessTop)) == 0)
            {
                _appearanceInfo ??= new LableAppearanceInfo();
                _appearanceInfo.BorderThicknessTop = propertyVal != null ? propertyVal.ToPrimitiveValue<double>() : LableAppearanceInfo.Default.BorderThicknessTop;
                RaisePropertyChanged(nameof(AppearanceInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(LableAppearanceInfo.BorderThicknessRight)) == 0)
            {
                _appearanceInfo ??= new LableAppearanceInfo();
                _appearanceInfo.BorderThicknessRight = propertyVal != null ? propertyVal.ToPrimitiveValue<double>() : LableAppearanceInfo.Default.BorderThicknessRight;
                RaisePropertyChanged(nameof(AppearanceInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(LableAppearanceInfo.BorderThicknessBottom)) == 0)
            {
                _appearanceInfo ??= new LableAppearanceInfo();
                _appearanceInfo.BorderThicknessBottom = propertyVal != null ? propertyVal.ToPrimitiveValue<double>() : LableAppearanceInfo.Default.BorderThicknessBottom;
                RaisePropertyChanged(nameof(AppearanceInfo));
                return true;
            }

            if (string.Compare(propertyID, nameof(BrushInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<LableBrushInfo>(propertyVal) : new LableBrushInfo();
                _brushInfo ??= new LableBrushInfo();
                _brushInfo.BackColorStr = src.BackColorStr;
                _brushInfo.ForeColorStr = src.ForeColorStr;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(AppearanceInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<LableAppearanceInfo>(propertyVal) : new LableAppearanceInfo();
                _appearanceInfo ??= new LableAppearanceInfo();
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
                var src = propertyVal != null ? DeserializeObject<LableLayoutInfo>(propertyVal) : new LableLayoutInfo();
                _layoutInfo ??= new LableLayoutInfo();
                _layoutInfo.HorizontalAlign = src.HorizontalAlign;
                _layoutInfo.VerticalAlign = src.VerticalAlign;
                RaisePropertyChanged(nameof(LayoutInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(CommonInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<LableCommonInfo>(propertyVal) : new LableCommonInfo();
                _commonInfo ??= new LableCommonInfo();
                _commonInfo.LableValue = src.LableValue;
                _commonInfo.CursorType = src.CursorType;
                _commonInfo.IsEnabled = src.IsEnabled;
                _commonInfo.ToolTip = src.ToolTip;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(FontInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<LableFontInfo>(propertyVal) : new LableFontInfo();
                _fontInfo ??= new LableFontInfo();
                _fontInfo.FontColorStr = src.FontColorStr;
                _fontInfo.FontSize = src.FontSize;
                _fontInfo.IsBold = src.IsBold;
                _fontInfo.IsItalic = src.IsItalic;
                _fontInfo.IsUnderline = src.IsUnderline;
                RaisePropertyChanged(nameof(FontInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(ParagraphInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<LableParagraphInfo>(propertyVal) : new LableParagraphInfo();
                _paragraphInfo ??= new LableParagraphInfo();
                _paragraphInfo.LineHeight = src.LineHeight;
                _paragraphInfo.ParagraphSpacingBefore = src.ParagraphSpacingBefore;
                _paragraphInfo.ParagraphSpacingAfter = src.ParagraphSpacingAfter;
                _paragraphInfo.TextAlignment = src.TextAlignment;
                _paragraphInfo.VerticalTextAlignment = src.VerticalTextAlignment;
                RaisePropertyChanged(nameof(ParagraphInfo));
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

        public void CopyFrom(LablePropertyModelEdit source)
        {
            if (source == null) return;

            base.CopyFrom(source);

            // 复制画笔信息 - 使用字符串属性
            _brushInfo ??= new LableBrushInfo();
            _brushInfo.BackColorStr = source.BrushInfo?.BackColorStr;
            _brushInfo.ForeColorStr = source.BrushInfo?.ForeColorStr;
            RaisePropertyChanged(nameof(BrushInfo));

            // 复制外观信息
            _appearanceInfo ??= new LableAppearanceInfo();
            _appearanceInfo.Opacity = source.AppearanceInfo?.Opacity ?? 1.0;
            _appearanceInfo.BorderThicknessLeft = source.AppearanceInfo?.BorderThicknessLeft ?? 0;
            _appearanceInfo.BorderThicknessTop = source.AppearanceInfo?.BorderThicknessTop ?? 0;
            _appearanceInfo.BorderThicknessRight = source.AppearanceInfo?.BorderThicknessRight ?? 0;
            _appearanceInfo.BorderThicknessBottom = source.AppearanceInfo?.BorderThicknessBottom ?? 0;
            RaisePropertyChanged(nameof(AppearanceInfo));

            // 复制布局信息
            _layoutInfo ??= new LableLayoutInfo();
            _layoutInfo.HorizontalAlign = source.LayoutInfo?.HorizontalAlign ?? HorizontalAlignType.Center;
            _layoutInfo.VerticalAlign = source.LayoutInfo?.VerticalAlign ?? VerticalAlignType.Center;
            RaisePropertyChanged(nameof(LayoutInfo));

            // 复制公共信息
            _commonInfo ??= new LableCommonInfo();
            _commonInfo.LableValue = source.CommonInfo?.LableValue ?? "";
            _commonInfo.CursorType = source.CommonInfo?.CursorType ?? CursorType.Arrow;
            _commonInfo.IsEnabled = source.CommonInfo?.IsEnabled ?? true;
            _commonInfo.ToolTip = source.CommonInfo?.ToolTip ?? "";
            RaisePropertyChanged(nameof(CommonInfo));

            // 复制字体信息 - 使用字符串属性
            _fontInfo ??= new LableFontInfo();
            _fontInfo.FontColorStr = source.FontInfo?.FontColorStr;
            _fontInfo.FontSize = source.FontInfo?.FontSize ?? 14;
            _fontInfo.IsBold = source.FontInfo?.IsBold ?? false;
            _fontInfo.IsItalic = source.FontInfo?.IsItalic ?? false;
            _fontInfo.IsUnderline = source.FontInfo?.IsUnderline ?? false;
            RaisePropertyChanged(nameof(FontInfo));

            // 复制段落信息
            _paragraphInfo ??= new LableParagraphInfo();
            _paragraphInfo.LineHeight = source.ParagraphInfo?.LineHeight ?? 1.0;
            _paragraphInfo.ParagraphSpacingBefore = source.ParagraphInfo?.ParagraphSpacingBefore ?? 0;
            _paragraphInfo.ParagraphSpacingAfter = source.ParagraphInfo?.ParagraphSpacingAfter ?? 0;
            _paragraphInfo.TextAlignment = source.ParagraphInfo?.TextAlignment ?? TextAlignType.Left;
            _paragraphInfo.VerticalTextAlignment = source.ParagraphInfo?.VerticalTextAlignment ?? TextVerticalAlignType.Center;
            RaisePropertyChanged(nameof(ParagraphInfo));
        }

        #endregion
    }

    /// <summary>
    /// 标签图元属性绑定编辑模型对象
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class LablePropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _lableValue = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _foreColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _backColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("标签值")]
        [Category("绑定信息")]
        [PropertySortOrder(1)]
        [BindMPPropertyID]
        public PropertyBindInfo LableValue
        {
            get => _lableValue;
            set => SetProperty(ref _lableValue, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        [DisplayName("前景色")]
        [Category("绑定信息")]
        [PropertySortOrder(2)]
        [BindMPPropertyID]
        public PropertyBindInfo ForeColor
        {
            get => _foreColor;
            set => SetProperty(ref _foreColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        [DisplayName("背景色")]
        [Category("绑定信息")]
        [PropertySortOrder(3)]
        [BindMPPropertyID]
        public PropertyBindInfo BackColor
        {
            get => _backColor;
            set => SetProperty(ref _backColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        public void CopyFrom(LablePropertyBindEditModel source)
        {
            base.CopyFrom(source);
            LableValue = source.LableValue;
            ForeColor = source.ForeColor;
            BackColor = source.BackColor;
        }
    }
}


