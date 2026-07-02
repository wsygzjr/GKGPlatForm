using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using Griffins;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using GKG.Map.MapCell.Generic.Control.MapCell_Slider.Views;
using GKG.Map.MapCell.Generic.Control.MapCell_Slider.ViewModels;
using GKG.Map.MapCell.Generic.Control.MapCell_Slider.MapOprtCellParamCfgView;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider
{
    internal class MapCellSliderCtlObj : ControlCellBase
    {
        #region 私有字段

        private SliderPropertyModelEdit _sliderPropertyModelEdit = new SliderPropertyModelEdit();
        private EventBindEditModel _eventBindEditModel;
        private SliderView view;
        private SliderViewModel sliderViewModel;
        private MapObjID _mapCellID;
        private string _mapCellName;
        private bool _loadedPropertyEditFromBytes;

        #endregion

        #region 属性

        public SliderPropertyModelEdit SliderPropertyModelEdit
        {
            get => _sliderPropertyModelEdit;
            set => SetProperty(ref _sliderPropertyModelEdit, value);
        }

        #endregion

        #region 构造函数

        static MapCellSliderCtlObj() { }

        public MapCellSliderCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false) { }

        public MapCellSliderCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            _mapCellID = mapCellID;
            _mapCellName = mapCellName;
            base.SetID(mapCellID);
            base.SetName(mapCellName);

            view = new SliderView();

            // 注册对象属性
            RegisterProperty(new MapObjPropertyInfo(nameof(SliderPropertyModelEdit.BrushInfo), "画笔设置", MapCellPropDataType.Object_Json, SliderBrushInfo.Object_ID, typeof(SliderBrushInfo), false, true, new MapCellPropValue(SliderBrushInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(SliderPropertyModelEdit.AppearanceInfo), "外观设置", MapCellPropDataType.Object_Json, SliderAppearanceInfo.Object_ID, typeof(SliderAppearanceInfo), false, true, new MapCellPropValue(SliderAppearanceInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(SliderPropertyModelEdit.CommonInfo), "公共设置", MapCellPropDataType.Object_Json, SliderCommonInfo.Object_ID, typeof(SliderCommonInfo), false, true, new MapCellPropValue(SliderCommonInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(SliderPropertyModelEdit.LayoutInfo), "布局设置", MapCellPropDataType.Object_Json, SliderLayoutInfo.Object_ID, typeof(SliderLayoutInfo), false, true, new MapCellPropValue(SliderLayoutInfo.Default)));

            // 注册操作原子信息
            RegisterOprtCellInfo(new MapOprtCellInfo(SliderMapOprtCellConst.BrushInfo_MapOprtCellID, "画笔设置操作原子", typeof(BrushInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(SliderMapOprtCellConst.AppearanceInfo_MapOprtCellID, "外观设置操作原子", typeof(AppearanceInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(SliderMapOprtCellConst.CommonInfo_MapOprtCellID, "公共设置操作原子", typeof(CommonInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(SliderMapOprtCellConst.LayoutInfo_MapOprtCellID, "布局设置操作原子", typeof(LayoutInfoMapOprtCellParamCfgView)));

            // 注册操作信息
            RegisterOprtInfo(new MapOprtInfo(nameof(SliderPropertyModelEdit.BrushInfo), "设置画笔", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = SliderMapOprtCellConst.BrushInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(SliderPropertyModelEdit.AppearanceInfo), "设置外观", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = SliderMapOprtCellConst.AppearanceInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(SliderPropertyModelEdit.CommonInfo), "设置公共", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = SliderMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(SliderPropertyModelEdit.LayoutInfo), "设置布局", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = SliderMapOprtCellConst.LayoutInfo_MapOprtCellID, CfgInfo = null } }));

            (this as IMapCellTypeBase).Name = "滑块";

            sliderViewModel = new SliderViewModel(SliderPropertyModelEdit);

            view.DataContext = sliderViewModel;

            SliderPropertyModelEdit.BrushInfo.PropertyChanged += OnBrushInfoPropertyChanged;
            SliderPropertyModelEdit.AppearanceInfo.PropertyChanged += OnAppearanceInfoPropertyChanged;
            SliderPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
            SliderPropertyModelEdit.LayoutInfo.PropertyChanged += OnLayoutInfoPropertyChanged;
        }

        private void OnBrushInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(SliderPropertyModelEdit.BrushInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnAppearanceInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(SliderPropertyModelEdit.AppearanceInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(SliderPropertyModelEdit.CommonInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnLayoutInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(SliderPropertyModelEdit.LayoutInfo), "PropertyChanged", e?.PropertyName);
        }

        public override void OnDispose()
        {
            SliderPropertyModelEdit.BrushInfo.PropertyChanged -= OnBrushInfoPropertyChanged;
            SliderPropertyModelEdit.AppearanceInfo.PropertyChanged -= OnAppearanceInfoPropertyChanged;
            SliderPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;
            SliderPropertyModelEdit.LayoutInfo.PropertyChanged -= OnLayoutInfoPropertyChanged;

            view.DataContext = null;
            sliderViewModel?.Dispose();
            sliderViewModel = null;

            base.OnDispose();
        }

        #endregion

        public override EventBindEditModel CreateEventBindEditModel()
        {
            if (_eventBindEditModel == null)
            {
                _eventBindEditModel = new EventBindEditModel()
                {
                    EventCmdInfos = new BindingList<EventCmdInfo>()
                    {
                        new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = "ValueChanged" },
                    }
                };
            }
            return _eventBindEditModel;
        }

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
        {
            return new SliderPropertyBindEditModel();
        }

        public override PropertyEditModelBase CreatePropertyModelEditBase()
        {
            return SliderPropertyModelEdit;
        }

        protected override void OnCopyFrom(ControlCellBase source)
        {
            if (source is MapCellSliderCtlObj sliderSource)
            {
                base._CopyFrom(sliderSource);
                (PropertyEditModelBase as SliderPropertyModelEdit).CopyFrom(source.PropertyEditModelBase as SliderPropertyModelEdit);
                if (PropertyBindEditModelBase is SliderPropertyBindEditModel selfBind && source.PropertyBindEditModelBase is SliderPropertyBindEditModel srcBind)
                    selfBind.CopyFrom(srcBind);
                else
                    (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);
                EventBindEditModel.CopyFrom(source.EventBindEditModel);
                _loadedPropertyEditFromBytes = true;
                Dispatcher.UIThread.Post(() => sliderViewModel?.ReloadFromModel());
            }
        }

        protected override object OnGetView()
        {
            return view;
        }

        protected override object OnGetViewModel()
        {
            return sliderViewModel;
        }

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
            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoadedSlider(propertyID, propertyVal))
            {
                return true;
            }
            SliderPropertyModelEdit.IsRuning = isRuning;
            var ok = SliderPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
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
                    // 测试工具右侧日志依赖 ExecOprt，这里把叶子字段归并后的分组操作ID回调出去。
                    CallBack?.ExecOprt(normalizedOprtId);
                }
                catch
                {
                }
            }

            if (SliderPropertyModelEdit.IsRuning)
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

        private bool IsDefaultOverwriteForLoadedSlider(string propertyID, MapCellPropValue propertyVal)
        {
            try
            {
                if (string.Compare(propertyID, nameof(SliderPropertyModelEdit.BrushInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<SliderBrushInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(SliderPropertyModelEdit.BrushInfo);
                }
                if (string.Compare(propertyID, nameof(SliderPropertyModelEdit.AppearanceInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<SliderAppearanceInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(SliderPropertyModelEdit.AppearanceInfo);
                }
                if (string.Compare(propertyID, nameof(SliderPropertyModelEdit.CommonInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<SliderCommonInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(SliderPropertyModelEdit.CommonInfo);
                }
                if (string.Compare(propertyID, nameof(SliderPropertyModelEdit.LayoutInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<SliderLayoutInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(SliderPropertyModelEdit.LayoutInfo);
                }
                return false;
            }
            catch { return false; }
        }

        private static bool IsDefault(SliderBrushInfo? info)
        {
            if (info == null) return true;
            return info.BackgroundColor == SliderBrushInfo.Default.BackgroundColor;
        }

        private static bool IsDefault(SliderAppearanceInfo? info)
        {
            if (info == null) return true;
            return info.Opacity == SliderAppearanceInfo.Default.Opacity;
        }

        private static bool IsDefault(SliderCommonInfo? info)
        {
            if (info == null) return true;
            return info.Maximum == SliderCommonInfo.Default.Maximum
                && info.Minimum == SliderCommonInfo.Default.Minimum
                && info.Direction == SliderCommonInfo.Default.Direction
                && info.SmallChange == SliderCommonInfo.Default.SmallChange
                && info.Value == SliderCommonInfo.Default.Value
                && info.TickFrequency == SliderCommonInfo.Default.TickFrequency
                && info.TickPlacement == SliderCommonInfo.Default.TickPlacement
                && info.HoverCursor == SliderCommonInfo.Default.HoverCursor
                && info.Enabled == SliderCommonInfo.Default.Enabled
                && string.Equals(info.TooltipText ?? string.Empty, SliderCommonInfo.Default.TooltipText ?? string.Empty, StringComparison.Ordinal);
        }

        private static bool IsDefault(SliderLayoutInfo? info)
        {
            if (info == null) return true;
            return info.HorizontalAlignment == SliderLayoutInfo.Default.HorizontalAlignment
                && info.VerticalAlignment == SliderLayoutInfo.Default.VerticalAlignment
                && info.Margin == SliderLayoutInfo.Default.Margin
                && info.MarginLeft == SliderLayoutInfo.Default.MarginLeft
                && info.MarginTop == SliderLayoutInfo.Default.MarginTop
                && info.MarginRight == SliderLayoutInfo.Default.MarginRight
                && info.MarginBottom == SliderLayoutInfo.Default.MarginBottom
                && info.MinWidth == SliderLayoutInfo.Default.MinWidth
                && info.MaxWidth == SliderLayoutInfo.Default.MaxWidth
                && info.MinHeight == SliderLayoutInfo.Default.MinHeight
                && info.MaxHeight == SliderLayoutInfo.Default.MaxHeight;
        }

        #region 操作触发(属性变更 => 执行原子)

        private void ExecuteOprtByPropertyId(string propertyID, string trigger, string? changedProp)
        {
            if (string.IsNullOrWhiteSpace(propertyID)) return;

            if (!TryGetPrimaryOprtCellId(propertyID, out var normalizedOprtId, out var primaryOprtCellId)) return;

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

            // 绑定面板下发的是叶子字段，这里统一映射到现有分组原子，避免再新增字段级操作体系。
            if (string.Equals(oprtId, nameof(SliderBrushInfo.BackgroundColor), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(SliderPropertyModelEdit.BrushInfo);
                oprtCellId = SliderMapOprtCellConst.BrushInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(SliderAppearanceInfo.Opacity), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(SliderPropertyModelEdit.AppearanceInfo);
                oprtCellId = SliderMapOprtCellConst.AppearanceInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(SliderCommonInfo.Value), StringComparison.Ordinal)
                || string.Equals(oprtId, nameof(SliderCommonInfo.Minimum), StringComparison.Ordinal)
                || string.Equals(oprtId, nameof(SliderCommonInfo.Maximum), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(SliderPropertyModelEdit.CommonInfo);
                oprtCellId = SliderMapOprtCellConst.CommonInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(SliderPropertyModelEdit.BrushInfo), StringComparison.Ordinal))
            { oprtCellId = SliderMapOprtCellConst.BrushInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(SliderPropertyModelEdit.AppearanceInfo), StringComparison.Ordinal))
            { oprtCellId = SliderMapOprtCellConst.AppearanceInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(SliderPropertyModelEdit.CommonInfo), StringComparison.Ordinal))
            { oprtCellId = SliderMapOprtCellConst.CommonInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(SliderPropertyModelEdit.LayoutInfo), StringComparison.Ordinal))
            { oprtCellId = SliderMapOprtCellConst.LayoutInfo_MapOprtCellID; return true; }
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

        /// <summary>
        /// 反序列化对象
        /// </summary>
        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }

        #endregion

        #region 操作原子执行

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == SliderMapOprtCellConst.BrushInfo_MapOprtCellID)
                return ExecuteOprtCell<BrushInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == SliderMapOprtCellConst.AppearanceInfo_MapOprtCellID)
                return ExecuteOprtCell<AppearanceInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == SliderMapOprtCellConst.CommonInfo_MapOprtCellID)
                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == SliderMapOprtCellConst.LayoutInfo_MapOprtCellID)
                return ExecuteOprtCell<LayoutInfoMapOprtCellExector>(mapOprtCellInstInfo);
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
                if (callBack.GetMapCellVMObjInstance() is SliderViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<BrushInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                var back = Color.Parse(param.BackgroundColor);
                                PostToUI(() => vm.BrushInfo.BackgroundColor = back);
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(SliderPropertyModelEdit.BrushInfo));
                    if (val != null)
                    {
                        try
                        {
                            var brushInfo = DeserializeObject<SliderBrushInfo>(val);
                            PostToUI(() => vm.BrushInfo.BackgroundColor = brushInfo.BackgroundColor);
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
                if (callBack.GetMapCellVMObjInstance() is SliderViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<AppearanceInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                int.TryParse(param.Opacity, out var opacity);
                                PostToUI(() =>
                                {
                                    vm.AppearanceInfo.Opacity = opacity;
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(SliderPropertyModelEdit.AppearanceInfo));
                    if (val != null)
                    {
                        try
                        {
                            var appearanceInfo = DeserializeObject<SliderAppearanceInfo>(val);
                            PostToUI(() =>
                            {
                                vm.AppearanceInfo.Opacity = appearanceInfo.Opacity;
                            });
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
                if (callBack.GetMapCellVMObjInstance() is SliderViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<CommonInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                int.TryParse(param.Maximum, out var max);
                                int.TryParse(param.Minimum, out var min);
                                int.TryParse(param.SmallChange, out var sc);
                                int.TryParse(param.Value, out var valInt);
                                int.TryParse(param.TickFrequency, out var tickFrequency);
                                PostToUI(() =>
                                {
                                    // 公共设置原子统一负责滑块步进、刻度和交互类配置。
                                    vm.CommonInfo.Maximum = max;
                                    vm.CommonInfo.Minimum = min;
                                    vm.CommonInfo.Direction = param.Direction;
                                    vm.CommonInfo.SmallChange = sc;
                                    vm.CommonInfo.Value = valInt;
                                    vm.CommonInfo.TickFrequency = tickFrequency;
                                    vm.CommonInfo.TickPlacement = param.TickPlacement;
                                    vm.CommonInfo.HoverCursor = param.HoverCursor;
                                    vm.CommonInfo.Enabled = param.Enabled;
                                    vm.CommonInfo.TooltipText = param.TooltipText ?? "";
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(SliderPropertyModelEdit.CommonInfo));
                    if (val != null)
                    {
                        try
                        {
                            var commonInfo = DeserializeObject<SliderCommonInfo>(val);
                            PostToUI(() =>
                            {
                                vm.CommonInfo.Maximum = commonInfo.Maximum;
                                vm.CommonInfo.Minimum = commonInfo.Minimum;
                                vm.CommonInfo.Direction = commonInfo.Direction;
                                vm.CommonInfo.SmallChange = commonInfo.SmallChange;
                                vm.CommonInfo.Value = commonInfo.Value;
                                vm.CommonInfo.TickFrequency = commonInfo.TickFrequency;
                                vm.CommonInfo.TickPlacement = commonInfo.TickPlacement;
                                vm.CommonInfo.HoverCursor = commonInfo.HoverCursor;
                                vm.CommonInfo.Enabled = commonInfo.Enabled;
                                vm.CommonInfo.TooltipText = commonInfo.TooltipText;
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
                if (callBack.GetMapCellVMObjInstance() is SliderViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<LayoutInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                int.TryParse(param.MarginTop, out var mt);
                                int.TryParse(param.MarginLeft, out var ml);
                                int.TryParse(param.MarginBottom, out var mb);
                                int.TryParse(param.MarginRight, out var mr);
                                int.TryParse(param.MinWidth, out var minW);
                                int.TryParse(param.MaxWidth, out var maxW);
                                int.TryParse(param.MinHeight, out var minH);
                                int.TryParse(param.MaxHeight, out var maxH);
                                PostToUI(() =>
                                {
                                    // 宽高主数据统一落到父类 Width/Height，旧 LayoutInfo 宽高仅保留兼容镜像。
                                    vm.LayoutInfo.HorizontalAlignment = param.HorizontalAlignment;
                                    vm.LayoutInfo.VerticalAlignment = param.VerticalAlignment;
                                    vm.LayoutInfo.MarginTop = mt;
                                    vm.LayoutInfo.MarginLeft = ml;
                                    vm.LayoutInfo.MarginBottom = mb;
                                    vm.LayoutInfo.MarginRight = mr;
                                    vm.LayoutInfo.MinWidth = minW;
                                    vm.LayoutInfo.MaxWidth = maxW;
                                    vm.LayoutInfo.MinHeight = minH;
                                    vm.LayoutInfo.MaxHeight = maxH;
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(SliderPropertyModelEdit.LayoutInfo));
                    if (val != null)
                    {
                        try
                        {
                            var layoutInfo = DeserializeObject<SliderLayoutInfo>(val);
                            PostToUI(() =>
                            {
                                vm.LayoutInfo.HorizontalAlignment = layoutInfo.HorizontalAlignment;
                                vm.LayoutInfo.VerticalAlignment = layoutInfo.VerticalAlignment;
                                vm.LayoutInfo.MarginTop = layoutInfo.MarginTop;
                                vm.LayoutInfo.MarginLeft = layoutInfo.MarginLeft;
                                vm.LayoutInfo.MarginBottom = layoutInfo.MarginBottom;
                                vm.LayoutInfo.MarginRight = layoutInfo.MarginRight;
                                vm.LayoutInfo.MinWidth = layoutInfo.MinWidth;
                                vm.LayoutInfo.MaxWidth = layoutInfo.MaxWidth;
                                vm.LayoutInfo.MinHeight = layoutInfo.MinHeight;
                                vm.LayoutInfo.MaxHeight = layoutInfo.MaxHeight;
                            });
                        }
                        catch { }
                    }
                }
            }
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
                    var propertyEditModelBase = JsonObjConvert.FromJSon<SliderPropertyModelEdit>(propertyEditJson);
                    if (propertyEditModelBase != null)
                    {
                        (PropertyEditModelBase as SliderPropertyModelEdit).CopyFrom(propertyEditModelBase);
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
                    var propertyBindEditModelBase = JsonObjConvert.FromJSon<SliderPropertyBindEditModel>(propertyBindJson);
                    if (propertyBindEditModelBase != null)
                        (PropertyBindEditModelBase as SliderPropertyBindEditModel)?.CopyFrom(propertyBindEditModelBase);
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

            if (_loadedPropertyEditFromBytes)
                Dispatcher.UIThread.Post(() => sliderViewModel?.ReloadFromModel());
        }

        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
        }

        #region 重写方法

        protected override void OnInit()
        {
            base.OnInit();
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    ExecuteOprtByPropertyId(nameof(SliderPropertyModelEdit.BrushInfo), "Init", null);
                    ExecuteOprtByPropertyId(nameof(SliderPropertyModelEdit.AppearanceInfo), "Init", null);
                    ExecuteOprtByPropertyId(nameof(SliderPropertyModelEdit.CommonInfo), "Init", null);
                    ExecuteOprtByPropertyId(nameof(SliderPropertyModelEdit.LayoutInfo), "Init", null);
                    sliderViewModel?.ReloadFromModel();
                }
                catch { }
            });
        }

        public override void OnZoomChanged() { }

        public override string ToString() => "滑块";

        public override MapCellPropValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null;
        }

        #endregion
    }

    /// <summary>
    /// 滑块属性绑定编辑模型
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class SliderPropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _value = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Decimal);
        [DisplayName("当前值")]
        [Category("绑定信息")]
        [PropertySortOrder(1)]
        [BindMPPropertyID]
        public PropertyBindInfo Value
        {
            get => _value;
            set => SetProperty(ref _value, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Decimal));
        }

        private PropertyBindInfo _minimum = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Decimal);
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
        [DisplayName("最大值")]
        [Category("绑定信息")]
        [PropertySortOrder(3)]
        [BindMPPropertyID]
        public PropertyBindInfo Maximum
        {
            get => _maximum;
            set => SetProperty(ref _maximum, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Decimal));
        }

        private PropertyBindInfo _backgroundColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        [DisplayName("背景色")]
        [Category("绑定信息")]
        [PropertySortOrder(4)]
        [BindMPPropertyID]
        public PropertyBindInfo BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        private PropertyBindInfo _opacity = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Decimal);
        [DisplayName("透明度")]
        [Category("绑定信息")]
        [PropertySortOrder(6)]
        [BindMPPropertyID]
        public PropertyBindInfo Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Decimal));
        }

        public void CopyFrom(SliderPropertyBindEditModel source)
        {
            if (source == null) return;
            base.CopyFrom(source);
            // 滑块绑定面板只保留当前值、范围、背景色、可见性和透明度这 6 项。
            Value = source.Value;
            Minimum = source.Minimum;
            Maximum = source.Maximum;
            BackgroundColor = source.BackgroundColor;
            Opacity = source.Opacity;
        }
    }
}
