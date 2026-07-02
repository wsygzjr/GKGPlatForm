using GF_Gereric;

using Griffins;

using GKG.Map.MapCell.Generic.Control.MapCell_TextBox.ViewModels;

using GKG.Map.MapCell.Generic.Control.MapCell_TextBox.Views;

using GKG.Map.MapCell.Generic.Control.MapCell_TextBox.MapOprtCellParamCfgView;

using Newtonsoft.JsonG;

using PropertyModels.ComponentModel;

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

using GKG.Map.MapCell.Generic;



using JsonSerializer = System.Text.Json.JsonSerializer;



namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox

{

    internal class MapCellTextBoxCtlObj : ControlCellBase

    {

        #region 私有字段



        private EventBindEditModel _eventBindEditModel;

        private TextBoxView view;

        private TextBoxViewModel viewModel;

        private bool _loadedPropertyEditFromBytes;





        #endregion



        static MapCellTextBoxCtlObj() { }



        public MapCellTextBoxCtlObj(MapObjID mapCellID, string mapCellName)

            : this(mapCellID, mapCellName, false) { }



        public MapCellTextBoxCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)

        {

            // 初始化属性/绑定/事件编辑模型（由基类提供存储与序列化框架）

            PropertyEditModelBase = CreatePropertyModelEditBase();

            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();

            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);

            base.SetName(mapCellName);



            view = new TextBoxView();



            // 注册对象属性（属性面板可编辑的“对象Json”属性组）

            RegisterProperty(new MapObjPropertyInfo(nameof(TextBoxPropertyModelEdit.BrushInfo), "画笔设置", MapCellPropDataType.Object_Json, TextBoxBrushInfo.Object_ID, typeof(TextBoxBrushInfo), false, true, new MapCellPropValue(TextBoxBrushInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(TextBoxPropertyModelEdit.AppearanceInfo), "外观设置", MapCellPropDataType.Object_Json, TextBoxAppearanceInfo.Object_ID, typeof(TextBoxAppearanceInfo), false, true, new MapCellPropValue(TextBoxAppearanceInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(TextBoxPropertyModelEdit.CommonInfo), "公共设置", MapCellPropDataType.Object_Json, TextBoxCommonInfo.Object_ID, typeof(TextBoxCommonInfo), false, true, new MapCellPropValue(TextBoxCommonInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(TextBoxPropertyModelEdit.LayoutInfo), "布局设置", MapCellPropDataType.Object_Json, TextBoxLayoutInfo.Object_ID, typeof(TextBoxLayoutInfo), false, true, new MapCellPropValue(TextBoxLayoutInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(TextBoxPropertyModelEdit.TextInfo), "文本设置", MapCellPropDataType.Object_Json, TextBoxTextInfo.Object_ID, typeof(TextBoxTextInfo), false, true, new MapCellPropValue(TextBoxTextInfo.Default)));



            // 注册操作原子：用于“操作”面板/脚本执行时触发 VM 刷新

            RegisterOprtCellInfo(new MapOprtCellInfo(TextBoxMapOprtCellConst.BrushInfo_MapOprtCellID, "画笔设置操作原子", typeof(BrushInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(TextBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID, "外观设置操作原子", typeof(AppearanceInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(TextBoxMapOprtCellConst.CommonInfo_MapOprtCellID, "公共设置操作原子", typeof(CommonInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(TextBoxMapOprtCellConst.LayoutInfo_MapOprtCellID, "布局设置操作原子", typeof(LayoutInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(TextBoxMapOprtCellConst.TextInfo_MapOprtCellID, "文本设置操作原子", typeof(TextInfoMapOprtCellParamCfgView)));



            // 注册操作：一个操作可包含一个或多个操作原子实例，这里按属性组各注册一个

            RegisterOprtInfo(new MapOprtInfo(nameof(TextBoxPropertyModelEdit.BrushInfo), "设置画笔", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = TextBoxMapOprtCellConst.BrushInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(TextBoxPropertyModelEdit.AppearanceInfo), "设置外观", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = TextBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(TextBoxPropertyModelEdit.CommonInfo), "设置公共", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = TextBoxMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(TextBoxPropertyModelEdit.LayoutInfo), "设置布局", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = TextBoxMapOprtCellConst.LayoutInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(TextBoxPropertyModelEdit.TextInfo), "设置文本", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = TextBoxMapOprtCellConst.TextInfo_MapOprtCellID, CfgInfo = null } }));



            (this as IMapCellTypeBase).Name = "文本输入框";



            // 创建 VM 并作为 View 的 DataContext（View 内通过绑定/订阅刷新）

            viewModel = new TextBoxViewModel(TextBoxPropertyModelEdit);

            view.DataContext = viewModel;

            TextBoxPropertyModelEdit.BrushInfo.PropertyChanged += OnBrushInfoPropertyChanged;
            TextBoxPropertyModelEdit.AppearanceInfo.PropertyChanged += OnAppearanceInfoPropertyChanged;
            TextBoxPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
            TextBoxPropertyModelEdit.LayoutInfo.PropertyChanged += OnLayoutInfoPropertyChanged;
            TextBoxPropertyModelEdit.TextInfo.PropertyChanged += OnTextInfoPropertyChanged;





        }

        private void OnBrushInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(TextBoxPropertyModelEdit.BrushInfo), "PropertyChanged", e?.PropertyName ?? string.Empty);
        }

        private void OnAppearanceInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(TextBoxPropertyModelEdit.AppearanceInfo), "PropertyChanged", e?.PropertyName ?? string.Empty);
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(TextBoxPropertyModelEdit.CommonInfo), "PropertyChanged", e?.PropertyName ?? string.Empty);
        }

        private void OnLayoutInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(TextBoxPropertyModelEdit.LayoutInfo), "PropertyChanged", e?.PropertyName ?? string.Empty);
        }

        private void OnTextInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(TextBoxPropertyModelEdit.TextInfo), "PropertyChanged", e?.PropertyName ?? string.Empty);
        }

        public override void OnDispose()
        {
            TextBoxPropertyModelEdit.BrushInfo.PropertyChanged -= OnBrushInfoPropertyChanged;
            TextBoxPropertyModelEdit.AppearanceInfo.PropertyChanged -= OnAppearanceInfoPropertyChanged;
            TextBoxPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;
            TextBoxPropertyModelEdit.LayoutInfo.PropertyChanged -= OnLayoutInfoPropertyChanged;
            TextBoxPropertyModelEdit.TextInfo.PropertyChanged -= OnTextInfoPropertyChanged;

            view.DataContext = null;
            viewModel?.Dispose();
            viewModel = null;

            base.OnDispose();
        }



        [Browsable(false)]

        public TextBoxPropertyModelEdit TextBoxPropertyModelEdit

        {

            get { return PropertyEditModelBase as TextBoxPropertyModelEdit; }

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

            // 运行态标记：某些属性变更在运行态/编辑态可能有不同处理

            TextBoxPropertyModelEdit.IsRuning = isRuning;

            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoadedTextBox(propertyID, propertyVal))

            {

                return true;

            }

            var ok = TextBoxPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);

            if (ok)

            {

                ExecuteOprtByPropertyId(propertyID, "SetPropertyValue", string.Empty);

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

                    // 文本输入框绑定字段现在会归并回分组原子，用于测试工具记录执行操作日志。

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



        private bool IsDefaultOverwriteForLoadedTextBox(string propertyID, MapCellPropValue propertyVal)

        {

            try

            {

                if (string.Compare(propertyID, nameof(TextBoxPropertyModelEdit.BrushInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<TextBoxBrushInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(TextBoxPropertyModelEdit.BrushInfo);

                }

                if (string.Compare(propertyID, nameof(TextBoxPropertyModelEdit.AppearanceInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<TextBoxAppearanceInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(TextBoxPropertyModelEdit.AppearanceInfo);

                }

                if (string.Compare(propertyID, nameof(TextBoxPropertyModelEdit.CommonInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<TextBoxCommonInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(TextBoxPropertyModelEdit.CommonInfo);

                }

                if (string.Compare(propertyID, nameof(TextBoxPropertyModelEdit.LayoutInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<TextBoxLayoutInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(TextBoxPropertyModelEdit.LayoutInfo);

                }

                if (string.Compare(propertyID, nameof(TextBoxPropertyModelEdit.TextInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<TextBoxTextInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(TextBoxPropertyModelEdit.TextInfo);

                }

                return false;

            }

            catch { return false; }

        }



        private static bool IsDefault(TextBoxBrushInfo? info)

        {

            if (info == null) return true;

            return info.BackgroundColor == TextBoxBrushInfo.Default.BackgroundColor

                && info.BorderColor == TextBoxBrushInfo.Default.BorderColor

                && info.ForegroundColor == TextBoxBrushInfo.Default.ForegroundColor

                && info.SelectedBorderColor == TextBoxBrushInfo.Default.SelectedBorderColor;

        }



        private static bool IsDefault(TextBoxAppearanceInfo? info)

        {

            if (info == null) return true;

            return info.Opacity == TextBoxAppearanceInfo.Default.Opacity

                && info.BorderThicknessLeft == TextBoxAppearanceInfo.Default.BorderThicknessLeft

                && info.BorderThicknessTop == TextBoxAppearanceInfo.Default.BorderThicknessTop

                && info.BorderThicknessRight == TextBoxAppearanceInfo.Default.BorderThicknessRight

                && info.BorderThicknessBottom == TextBoxAppearanceInfo.Default.BorderThicknessBottom;

        }



        private static bool IsDefault(TextBoxCommonInfo? info)

        {

            if (info == null) return true;

            return info.SelectedTextOpacity == TextBoxCommonInfo.Default.SelectedTextOpacity

                && info.EnableSpellCheck == TextBoxCommonInfo.Default.EnableSpellCheck

                && string.Equals(info.Text ?? string.Empty, TextBoxCommonInfo.Default.Text ?? string.Empty, StringComparison.Ordinal)

                && info.HoverCursor == TextBoxCommonInfo.Default.HoverCursor

                && info.Enabled == TextBoxCommonInfo.Default.Enabled

                && info.IsReadOnly == TextBoxCommonInfo.Default.IsReadOnly

                && string.Equals(info.TooltipText ?? string.Empty, TextBoxCommonInfo.Default.TooltipText ?? string.Empty, StringComparison.Ordinal);

        }



        private static bool IsDefault(TextBoxLayoutInfo? info)

        {

            if (info == null) return true;

            return info.HorizontalAlignment == TextBoxLayoutInfo.Default.HorizontalAlignment

                && info.VerticalAlignment == TextBoxLayoutInfo.Default.VerticalAlignment

                && info.Margin == TextBoxLayoutInfo.Default.Margin

                && info.MarginLeft == TextBoxLayoutInfo.Default.MarginLeft

                && info.MarginTop == TextBoxLayoutInfo.Default.MarginTop

                && info.MarginRight == TextBoxLayoutInfo.Default.MarginRight

                && info.MarginBottom == TextBoxLayoutInfo.Default.MarginBottom

                && info.MinWidth == TextBoxLayoutInfo.Default.MinWidth

                && info.MaxWidth == TextBoxLayoutInfo.Default.MaxWidth

                && info.MinHeight == TextBoxLayoutInfo.Default.MinHeight

                && info.MaxHeight == TextBoxLayoutInfo.Default.MaxHeight;

        }



        private static bool IsDefault(TextBoxTextInfo? info)

        {

            if (info == null) return true;

            return info.FontFamily == TextBoxTextInfo.Default.FontFamily

                && info.FontColor == TextBoxTextInfo.Default.FontColor

                && info.FontSize == TextBoxTextInfo.Default.FontSize

                && info.IsItalic == TextBoxTextInfo.Default.IsItalic

                && info.IsBold == TextBoxTextInfo.Default.IsBold

                && info.TextAlignment == TextBoxTextInfo.Default.TextAlignment

                && info.VerticalTextAlignment == TextBoxTextInfo.Default.VerticalTextAlignment;

        }



        #region 操作触发(属性变更 => 执行原子)



        private void ExecuteOprtByPropertyId(string propertyID, string trigger, string changedProp)

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



            if (string.Equals(oprtId, nameof(TextBoxBrushInfo.BackgroundColor), StringComparison.Ordinal)

                || string.Equals(oprtId, nameof(TextBoxBrushInfo.BorderColor), StringComparison.Ordinal)

                || string.Equals(oprtId, nameof(TextBoxBrushInfo.ForegroundColor), StringComparison.Ordinal))

            {

                normalizedOprtId = nameof(TextBoxPropertyModelEdit.BrushInfo);

                oprtCellId = TextBoxMapOprtCellConst.BrushInfo_MapOprtCellID;

                return true;

            }

            if (string.Equals(oprtId, nameof(TextBoxAppearanceInfo.Opacity), StringComparison.Ordinal)

                )

            {

                normalizedOprtId = nameof(TextBoxPropertyModelEdit.AppearanceInfo);

                oprtCellId = TextBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID;

                return true;

            }

            if (string.Equals(oprtId, nameof(TextBoxCommonInfo.Text), StringComparison.Ordinal))

            {

                normalizedOprtId = nameof(TextBoxPropertyModelEdit.CommonInfo);

                oprtCellId = TextBoxMapOprtCellConst.CommonInfo_MapOprtCellID;

                return true;

            }

            if (string.Equals(oprtId, nameof(TextBoxPropertyModelEdit.BrushInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(TextBoxPropertyModelEdit.BrushInfo); oprtCellId = TextBoxMapOprtCellConst.BrushInfo_MapOprtCellID; return true; }

            if (string.Equals(oprtId, nameof(TextBoxPropertyModelEdit.AppearanceInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(TextBoxPropertyModelEdit.AppearanceInfo); oprtCellId = TextBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID; return true; }

            if (string.Equals(oprtId, nameof(TextBoxPropertyModelEdit.CommonInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(TextBoxPropertyModelEdit.CommonInfo); oprtCellId = TextBoxMapOprtCellConst.CommonInfo_MapOprtCellID; return true; }

            if (string.Equals(oprtId, nameof(TextBoxPropertyModelEdit.LayoutInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(TextBoxPropertyModelEdit.LayoutInfo); oprtCellId = TextBoxMapOprtCellConst.LayoutInfo_MapOprtCellID; return true; }

            if (string.Equals(oprtId, nameof(TextBoxPropertyModelEdit.TextInfo), StringComparison.Ordinal))

            { normalizedOprtId = nameof(TextBoxPropertyModelEdit.TextInfo); oprtCellId = TextBoxMapOprtCellConst.TextInfo_MapOprtCellID; return true; }

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

                if (val is MapOprtInfo single)

                {

                    yield return single;

                    continue;

                }

                if (val is IDictionary dict)

                {

                    foreach (DictionaryEntry entry in dict)

                        if (entry.Value is MapOprtInfo info) yield return info;

                    continue;

                }

                if (val is IEnumerable enumerable)

                {

                    foreach (var item in enumerable)

                        if (item is MapOprtInfo info) yield return info;

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

            return string.Empty;

        }



        private static IEnumerable<MapOprtCellInstInfo> GetOprtInfoInstList(object oprtInfo)

        {

            try { return EnumerateOprtInfoInsts(oprtInfo).Select(x => x.inst).ToList(); }

            catch { }

            return Array.Empty<MapOprtCellInstInfo>();

        }



        private static IEnumerable<(MapOprtCellInstInfo inst, string source)> EnumerateOprtInfoInsts(object oprtInfo)

        {

            var t = oprtInfo.GetType();

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var seen = new HashSet<string>(StringComparer.Ordinal);



            IEnumerable<(MapOprtCellInstInfo inst, string source)> EnumerateFromValue(object val, string source)

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

                {

                    foreach (var x in EnumerateFromValue(p.GetValue(oprtInfo), $"prop:{p.Name}"))

                        yield return x;

                }

            }



            foreach (var f in t.GetFields(flags))

            {

                if (f.IsStatic) continue;

                var ft = f.FieldType;

                if (ft == typeof(MapOprtCellInstInfoList) || typeof(IEnumerable).IsAssignableFrom(ft))

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

                foreach (var f in t.GetFields(flags)) { if (!f.IsStatic) yield return f; }

                foreach (var p in t.GetProperties(flags)) { if (p.GetIndexParameters().Length == 0 && p.CanRead) yield return p; }

            }

        }



        private static object GetMemberValue(MemberInfo member, object instance)

        {

            try { return member switch { FieldInfo f => f.GetValue(instance), PropertyInfo p => p.GetValue(instance), _ => null }; }

            catch { return null; }

        }



        #endregion



        #region 工具方法



        private static void PostToUI(Action action)

        {

            if (Dispatcher.UIThread.CheckAccess()) action();

            else Dispatcher.UIThread.Post(action);

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



        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)

        {

            base.OnReadDrawInfoFromBytes(br);



            // 从图元序列化数据恢复属性编辑模型（属性组对象）

            string propertyEditJson = br.ReadString("PropertyEditModelBase");

            if (!string.IsNullOrEmpty(propertyEditJson))

            {

                try

                {

                    var propertyEditModelBase = JsonObjConvert.FromJSon<TextBoxPropertyModelEdit>(propertyEditJson);

                    if (propertyEditModelBase != null)

                    {

                        (PropertyEditModelBase as TextBoxPropertyModelEdit).CopyFrom(propertyEditModelBase);

                        _loadedPropertyEditFromBytes = true;

                    }

                }

                catch { }

            }



            // 从图元序列化数据恢复属性绑定模型

            string propertyBindJson = br.ReadString("PropertyBindEditModelBase");

            if (!string.IsNullOrEmpty(propertyBindJson))

            {

                try

                {

                    var propertyBindEditModelBase = JsonObjConvert.FromJSon<TextBoxPropertyBindEditModel>(propertyBindJson);

                    if (propertyBindEditModelBase != null)

                        (PropertyBindEditModelBase as TextBoxPropertyBindEditModel)?.CopyFrom(propertyBindEditModelBase);

                }

                catch { }

            }



            // 从图元序列化数据恢复事件绑定模型

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

                catch { }

            }



            if (_loadedPropertyEditFromBytes)

                Dispatcher.UIThread.Post(() => viewModel?.ReloadFromModel());

        }



        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)

        {

            base.OnWriteDrawInfoToBytes(bw);

            // 持久化属性编辑模型/绑定模型/事件绑定模型

            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));

            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));

            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));

        }



        protected override void OnCopyFrom(ControlCellBase source)

        {

            MapCellTextBoxCtlObj obj = source as MapCellTextBoxCtlObj;

            base._CopyFrom(obj);

            // 拷贝属性/绑定/事件绑定（用于复制图元、粘贴等场景）

            (PropertyEditModelBase as TextBoxPropertyModelEdit).CopyFrom(source.PropertyEditModelBase as TextBoxPropertyModelEdit);

            if (PropertyBindEditModelBase is TextBoxPropertyBindEditModel selfBind && source.PropertyBindEditModelBase is TextBoxPropertyBindEditModel srcBind)

                selfBind.CopyFrom(srcBind);

            else

                (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);

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

                    ExecuteOprtByPropertyId(nameof(TextBoxPropertyModelEdit.BrushInfo), "Init", string.Empty);

                    ExecuteOprtByPropertyId(nameof(TextBoxPropertyModelEdit.AppearanceInfo), "Init", string.Empty);

                    ExecuteOprtByPropertyId(nameof(TextBoxPropertyModelEdit.CommonInfo), "Init", string.Empty);

                    ExecuteOprtByPropertyId(nameof(TextBoxPropertyModelEdit.LayoutInfo), "Init", string.Empty);

                    ExecuteOprtByPropertyId(nameof(TextBoxPropertyModelEdit.TextInfo), "Init", string.Empty);

                    viewModel?.ReloadFromModel();

                }

                catch { }

            });

        }





        protected override object OnGetView() => view;



        protected override object OnGetViewModel() => viewModel;



        public override PropertyEditModelBase CreatePropertyModelEditBase() => new TextBoxPropertyModelEdit();



        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new TextBoxPropertyBindEditModel();



        public override EventBindEditModel CreateEventBindEditModel()

        {

            _eventBindEditModel ??= new EventBindEditModel() { EventCmdInfos = new BindingList<EventCmdInfo>() };

            return _eventBindEditModel;

        }



        public override void OnZoomChanged() { }



        public override string ToString() => "文本输入框";



        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)

        {

            // 操作原子执行入口：按 OprtCellID 分发到对应 Exector

            if (mapOprtCellInstInfo.OprtCellID == TextBoxMapOprtCellConst.BrushInfo_MapOprtCellID)

                return ExecuteOprtCell<BrushInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == TextBoxMapOprtCellConst.AppearanceInfo_MapOprtCellID)

                return ExecuteOprtCell<AppearanceInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == TextBoxMapOprtCellConst.CommonInfo_MapOprtCellID)

                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == TextBoxMapOprtCellConst.LayoutInfo_MapOprtCellID)

                return ExecuteOprtCell<LayoutInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == TextBoxMapOprtCellConst.TextInfo_MapOprtCellID)

                return ExecuteOprtCell<TextInfoMapOprtCellExector>(mapOprtCellInstInfo);

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

        private class BrushInfoMapOprtCellExector : IMapOprtCellExector

        {

            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)

            {

                if (callBack.GetMapCellVMObjInstance() is TextBoxViewModel vm)

                {

                    if (cfg != null && cfg.Length > 0)

                    {

                        try

                        {

                            var param = JsonSerializer.Deserialize<BrushInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));

                            if (param != null)

                            {

                                var bg = Color.Parse(param.BackgroundColor);

                                var bd = Color.Parse(param.BorderColor);

                                var fg = Color.Parse(param.ForegroundColor);

                                var sbd = Color.Parse(param.SelectedBorderColor);

                                PostToUI(() =>

                                {

                                    vm.BrushInfo.BackgroundColor = bg;

                                    vm.BrushInfo.BorderColor = bd;

                                    vm.BrushInfo.ForegroundColor = fg;

                                    vm.BrushInfo.SelectedBorderColor = sbd;

                                });

                                return;

                            }

                        }

                        catch { }

                    }



                    var val = callBack.GetMapCellPropValue(nameof(TextBoxPropertyModelEdit.BrushInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<TextBoxBrushInfo>(val);

                            PostToUI(() =>

                            {

                                vm.BrushInfo.BackgroundColor = info.BackgroundColor;

                                vm.BrushInfo.BorderColor = info.BorderColor;

                                vm.BrushInfo.ForegroundColor = info.ForegroundColor;

                                vm.BrushInfo.SelectedBorderColor = info.SelectedBorderColor;

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

                if (callBack.GetMapCellVMObjInstance() is TextBoxViewModel vm)

                {

                    if (cfg != null && cfg.Length > 0)

                    {

                        try

                        {

                            var param = JsonSerializer.Deserialize<AppearanceInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));

                            if (param != null)

                            {

                                int.TryParse(param.Opacity, out var opacity);

                                int.TryParse(param.BorderThicknessLeft, out var l);

                                int.TryParse(param.BorderThicknessTop, out var t);

                                int.TryParse(param.BorderThicknessRight, out var r);

                                int.TryParse(param.BorderThicknessBottom, out var b);



                                PostToUI(() =>

                                {

                                    vm.AppearanceInfo.Opacity = opacity;

                                    vm.AppearanceInfo.BorderThicknessLeft = l;

                                    vm.AppearanceInfo.BorderThicknessTop = t;

                                    vm.AppearanceInfo.BorderThicknessRight = r;

                                    vm.AppearanceInfo.BorderThicknessBottom = b;

                                });

                                return;

                            }

                        }

                        catch { }

                    }



                    var val = callBack.GetMapCellPropValue(nameof(TextBoxPropertyModelEdit.AppearanceInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<TextBoxAppearanceInfo>(val);

                            PostToUI(() =>

                            {


                                vm.AppearanceInfo.Opacity = info.Opacity;

                                vm.AppearanceInfo.BorderThicknessLeft = info.BorderThicknessLeft;

                                vm.AppearanceInfo.BorderThicknessTop = info.BorderThicknessTop;

                                vm.AppearanceInfo.BorderThicknessRight = info.BorderThicknessRight;

                                vm.AppearanceInfo.BorderThicknessBottom = info.BorderThicknessBottom;

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

                if (callBack.GetMapCellVMObjInstance() is TextBoxViewModel vm)

                {

                    if (cfg != null && cfg.Length > 0)

                    {

                        try

                        {

                            var param = JsonSerializer.Deserialize<CommonInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));

                            if (param != null)

                            {

                                int.TryParse(param.SelectedTextOpacity, out var sto);

                                PostToUI(() =>

                                {

                                    vm.CommonInfo.Text = param.Text;

                                    vm.CommonInfo.TooltipText = param.TooltipText;

                                    vm.CommonInfo.Enabled = param.Enabled;

                                    vm.CommonInfo.IsReadOnly = param.IsReadOnly;

                                    vm.CommonInfo.HoverCursor = param.HoverCursor;

                                    vm.CommonInfo.SelectedTextOpacity = sto;

                                    vm.CommonInfo.EnableSpellCheck = param.EnableSpellCheck;

                                });

                                return;

                            }

                        }

                        catch { }

                    }



                    var val = callBack.GetMapCellPropValue(nameof(TextBoxPropertyModelEdit.CommonInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<TextBoxCommonInfo>(val);

                            PostToUI(() =>

                            {

                                vm.CommonInfo.Text = info.Text;

                                vm.CommonInfo.TooltipText = info.TooltipText;

                                vm.CommonInfo.Enabled = info.Enabled;

                                vm.CommonInfo.IsReadOnly = info.IsReadOnly;

                                vm.CommonInfo.HoverCursor = info.HoverCursor;

                                vm.CommonInfo.SelectedTextOpacity = info.SelectedTextOpacity;

                                vm.CommonInfo.EnableSpellCheck = info.EnableSpellCheck;

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

                if (callBack.GetMapCellVMObjInstance() is TextBoxViewModel vm)

                {

                    if (cfg != null && cfg.Length > 0)

                    {

                        try

                        {

                            var param = JsonSerializer.Deserialize<LayoutInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));

                            if (param != null)

                            {
                                int.TryParse(param.MarginLeft, out var ml);

                                int.TryParse(param.MarginTop, out var mt);

                                int.TryParse(param.MarginRight, out var mr);

                                int.TryParse(param.MarginBottom, out var mb);

                                int.TryParse(param.MinWidth, out var minW);

                                int.TryParse(param.MaxWidth, out var maxW);

                                int.TryParse(param.MinHeight, out var minH);

                                int.TryParse(param.MaxHeight, out var maxH);



                                PostToUI(() =>

                                {

                                    // 宽高主数据统一落到父类 Width/Height，旧 LayoutInfo 宽高仅保留兼容镜像。
                                    // 宽高主数据统一落到父类 Width/Height，避免 TextBox 继续维护额外尺寸中间态。
                                    vm.LayoutInfo.HorizontalAlignment = param.HorizontalAlignment;

                                    vm.LayoutInfo.VerticalAlignment = param.VerticalAlignment;

                                    vm.LayoutInfo.MarginLeft = ml;

                                    vm.LayoutInfo.MarginTop = mt;

                                    vm.LayoutInfo.MarginRight = mr;

                                    vm.LayoutInfo.MarginBottom = mb;

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



                    var val = callBack.GetMapCellPropValue(nameof(TextBoxPropertyModelEdit.LayoutInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<TextBoxLayoutInfo>(val);

                            PostToUI(() =>

                            {
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

                            });

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

                if (callBack.GetMapCellVMObjInstance() is TextBoxViewModel vm)

                {

                    if (cfg != null && cfg.Length > 0)

                    {

                        try

                        {

                            var param = JsonSerializer.Deserialize<TextInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));

                            if (param != null)

                            {

                                int.TryParse(param.FontSize, out var fs);

                                var fc = Color.Parse(param.FontColor);

                                PostToUI(() =>

                                {

                                    vm.TextInfo.FontFamily = param.FontFamily;

                                    vm.TextInfo.FontColor = fc;

                                    vm.TextInfo.FontSize = fs;

                                    vm.TextInfo.IsItalic = param.IsItalic;

                                    vm.TextInfo.IsBold = param.IsBold;

                                    vm.TextInfo.TextAlignment = param.TextAlignment;

                                    vm.TextInfo.VerticalTextAlignment = param.VerticalTextAlignment;

                                });

                                return;

                            }

                        }

                        catch { }

                    }



                    var val = callBack.GetMapCellPropValue(nameof(TextBoxPropertyModelEdit.TextInfo));

                    if (val != null)

                    {

                        try

                        {

                            var info = DeserializeObject<TextBoxTextInfo>(val);

                            PostToUI(() =>

                            {

                                vm.TextInfo.FontFamily = info.FontFamily;

                                vm.TextInfo.FontColor = info.FontColor;

                                vm.TextInfo.FontSize = info.FontSize;

                                vm.TextInfo.IsItalic = info.IsItalic;

                                vm.TextInfo.IsBold = info.IsBold;

                                vm.TextInfo.TextAlignment = info.TextAlignment;

                                vm.TextInfo.VerticalTextAlignment = info.VerticalTextAlignment;

                            });

                        }

                        catch { }

                    }

                }

            }

        }

    }



    [Serializable]

    [MapPropertyOrder]

    [CategoryPriority("点位信息", 1)]

    [CategoryPriority("绑定信息", 2)]

    public class TextBoxPropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _backgroundColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _borderColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _foregroundColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _opacity = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Integer);
        private PropertyBindInfo _text = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("背景色")]
        [Category("绑定信息")]
        [PropertySortOrder(1)]
        [BindMPPropertyID]
        public PropertyBindInfo BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        [DisplayName("边框颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(2)]
        [BindMPPropertyID]
        public PropertyBindInfo BorderColor
        {
            get => _borderColor;
            set => SetProperty(ref _borderColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        [DisplayName("前景色")]
        [Category("绑定信息")]
        [PropertySortOrder(3)]
        [BindMPPropertyID]
        public PropertyBindInfo ForegroundColor
        {
            get => _foregroundColor;
            set => SetProperty(ref _foregroundColor, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        [DisplayName("透明度")]
        [Category("绑定信息")]
        [PropertySortOrder(4)]
        [BindMPPropertyID]
        public PropertyBindInfo Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Integer));
        }

        [DisplayName("文本")]
        [Category("绑定信息")]
        [PropertySortOrder(6)]
        [BindMPPropertyID]
        public PropertyBindInfo Text
        {
            get => _text;
            set => SetProperty(ref _text, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        public void CopyFrom(TextBoxPropertyBindEditModel source)
        {
            if (source == null) return;
            base.CopyFrom(source);
            // 文本输入框绑定面板现在只保留 6 个叶子绑定项，这里只复制这 6 项和基类继承项。
            BackgroundColor = source.BackgroundColor;
            BorderColor = source.BorderColor;
            ForegroundColor = source.ForegroundColor;
            Opacity = source.Opacity;
            Text = source.Text;
        }
    }

}







