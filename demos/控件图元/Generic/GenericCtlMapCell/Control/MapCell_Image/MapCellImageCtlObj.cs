using Avalonia.Media;

using Avalonia.Threading;

using GF_Gereric;

using Griffins;

using GKG.Map.MapCell.Generic;

using GKG.Map.MapCell.Generic.Control.MapCell_Image.Objects;

using GKG.Map.MapCell.Generic.Control.MapCell_Image.ViewModels;

using GKG.Map.MapCell.Generic.Control.MapCell_Image.Views;

using GKG.Map.MapCell.Generic.Control.MapCell_Image.MapOprtCellParamCfgView;

using GKG.Map.MapCell.Generic.Control.Lable;

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

using System.Text.Json;

using JsonSerializer = System.Text.Json.JsonSerializer;
using Griffins.Map.Cmd;



namespace GKG.Map.MapCell.Generic.Control.MapCell_Image

{

    /// <summary>

    /// 图片图元控件对象

    /// </summary>

    class MapCellImageCtlObj : ControlCellBase
    {
        private static readonly string[] DefaultEventIds =
        {
            MapObjPropEventConst.Event_MouseClick
        };
        #region 私有字段
        


        private ImageView view;
        private ImageViewModel imageViewModel;
        private EventBindEditModel _eventBindEditModel;
        private bool _loadedPropertyEditFromBytes;
        private readonly ConcurrentDictionary<Guid, MapOprtCellID> _oprtCellIdByInstanceId = new();
       




        #endregion

        

        #region 构造函数



        public MapCellImageCtlObj(MapObjID mapCellID, string mapCellName)

            : this(mapCellID, mapCellName, false) { }



        public MapCellImageCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)

        {

            PropertyEditModelBase = CreatePropertyModelEditBase();

            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();

            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);

            base.SetName(mapCellName);

            view = new ImageView();



            // 注册对象属性

            RegisterProperty(new MapObjPropertyInfo(nameof(ImagePropertyModelEdit.AppearanceInfo), ResourceA.Image_AppearanceInfo, MapCellPropDataType.Object_Json, ImageAppearanceInfo.Object_ID, typeof(ImageAppearanceInfo), false, true, new MapCellPropValue(ImageAppearanceInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ImagePropertyModelEdit.CommonInfo), ResourceA.Image_CommonInfo, MapCellPropDataType.Object_Bytes, ImageCommonInfo.Object_ID, typeof(ImageCommonInfo), false, true, new MapCellPropValue(ImageCommonInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ImagePropertyModelEdit.LayoutInfo), ResourceA.Image_LayoutInfo, MapCellPropDataType.Object_Json, ImageLayoutInfo.Object_ID, typeof(ImageLayoutInfo), false, true, new MapCellPropValue(ImageLayoutInfo.Default)));
            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), MapCellPropDataType.Object_Bytes, GraphMouseEventParam.Object_ID));



            // 注册操作原子信息

            RegisterOprtCellInfo(new MapOprtCellInfo(ImageMapOprtCellConst.AppearanceInfo_MapOprtCellID, ResourceA.Image_AppearanceInfo_MapOprtCellName, typeof(AppearanceInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(ImageMapOprtCellConst.CommonInfo_MapOprtCellID, ResourceA.Image_CommonInfo_MapOprtCellName, typeof(CommonInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(ImageMapOprtCellConst.LayoutInfo_MapOprtCellID, ResourceA.Image_LayoutInfo_MapOprtCellName, typeof(LayoutInfoMapOprtCellParamCfgView)));



            // 注册操作信息

            RegisterOprtInfo(new MapOprtInfo(nameof(ImagePropertyModelEdit.AppearanceInfo), ResourceA.Image_AppearanceInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = ImageMapOprtCellConst.AppearanceInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(ImagePropertyModelEdit.CommonInfo), ResourceA.Image_CommonInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = ImageMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(ImagePropertyModelEdit.LayoutInfo), ResourceA.Image_LayoutInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = ImageMapOprtCellConst.LayoutInfo_MapOprtCellID, CfgInfo = null } }));



            (this as IMapCellTypeBase).Name = ResourceA.ImageMapCell;

            imageViewModel = new ImageViewModel(designTime, ImagePropertyModelEdit, () => ExecEvent(MapObjPropEventConst.Event_MouseClick));

            view.DataContext = imageViewModel;



            // 订阅 Info 对象的 PropertyChanged，触发操作原子执行

            ImagePropertyModelEdit.AppearanceInfo.PropertyChanged += OnAppearanceInfoPropertyChanged;

            ImagePropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;

            ImagePropertyModelEdit.LayoutInfo.PropertyChanged += OnLayoutInfoPropertyChanged;





        }

        private void OnAppearanceInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(ImagePropertyModelEdit.AppearanceInfo));
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(ImagePropertyModelEdit.CommonInfo));
        }

        private void OnLayoutInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(ImagePropertyModelEdit.LayoutInfo));
        }

        public override void OnDispose()
        {
            ImagePropertyModelEdit.AppearanceInfo.PropertyChanged -= OnAppearanceInfoPropertyChanged;
            ImagePropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;
            ImagePropertyModelEdit.LayoutInfo.PropertyChanged -= OnLayoutInfoPropertyChanged;

            view.DataContext = null;
            imageViewModel?.Dispose();
            imageViewModel = null;

            base.OnDispose();
        }



        #endregion



        #region 属性



        public ImagePropertyModelEdit ImagePropertyModelEdit => PropertyEditModelBase as ImagePropertyModelEdit;



        #endregion



        #region 公共方法

        private static BindingList<EventCmdInfo> CreateDefaultEventCmdInfos()
        {
            var eventCmdInfos = new BindingList<EventCmdInfo>();
            foreach (var eventId in DefaultEventIds)
            {
                eventCmdInfos.Add(new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = eventId });
            }

            return eventCmdInfos;
        }

        private static void EnsureEventBindEditModel(EventBindEditModel eventBindEditModel)
        {
            if (eventBindEditModel == null)
            {
                return;
            }

            var existingInfos = eventBindEditModel.EventCmdInfos?.Where(info => info != null).ToList() ?? new List<EventCmdInfo>();
            var normalizedInfos = new BindingList<EventCmdInfo>();

            foreach (var eventId in DefaultEventIds)
            {
                var existingInfo = existingInfos.FirstOrDefault(info => string.Equals(info.EventID, eventId, StringComparison.Ordinal));
                normalizedInfos.Add(existingInfo ?? new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = eventId });
            }

            foreach (var extraInfo in existingInfos.Where(info => !DefaultEventIds.Contains(info.EventID)))
            {
                normalizedInfos.Add(extraInfo);
            }

            eventBindEditModel.EventCmdInfos = normalizedInfos;
        }

        private void ExecEvent(string eventId)
        {
            EventCmdInfo eventCmdInfo = EventBindEditModel?.EventCmdInfos?.FirstOrDefault(info => info.EventID == eventId);
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

            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoaded(propertyID, propertyVal))

            {

                return true;

            }

            ImagePropertyModelEdit.IsRuning = isRuning;

            var ok = ImagePropertyModelEdit.SetPropertyValue(propertyID, propertyVal);

return ok;

        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)

        {

            base.AfterPropertyChanged(propertyID, propertyValue);



            if (ImagePropertyModelEdit.IsRuning)

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



        private bool IsDefaultOverwriteForLoaded(string propertyID, MapCellPropValue propertyVal)

        {

            try

            {

                if (string.Compare(propertyID, nameof(ImagePropertyModelEdit.AppearanceInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<ImageAppearanceInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(ImagePropertyModelEdit.AppearanceInfo);

                }

                if (string.Compare(propertyID, nameof(ImagePropertyModelEdit.CommonInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<ImageCommonInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(ImagePropertyModelEdit.CommonInfo);

                }

                if (string.Compare(propertyID, nameof(ImagePropertyModelEdit.LayoutInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<ImageLayoutInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(ImagePropertyModelEdit.LayoutInfo);

                }

                return false;

            }

            catch { return false; }

        }



        private static bool IsDefault(ImageAppearanceInfo info)

        {

            if (info == null) return true;

            return Math.Abs(info.Opacity - ImageAppearanceInfo.Default.Opacity) < 0.000001;

        }



        private static bool IsDefault(ImageCommonInfo info)

        {

            if (info == null) return true;

            return (info.ImageSource == null || info.ImageSource.Bitmap == null)

                && info.StretchMode == ImageCommonInfo.Default.StretchMode

                && info.IsEnabled == ImageCommonInfo.Default.IsEnabled

                && string.Equals(info.ToolTip ?? "", ImageCommonInfo.Default.ToolTip ?? "", StringComparison.Ordinal);

        }



        private static bool IsDefault(ImageLayoutInfo info)

        {

            if (info == null) return true;

            return info.HorizontalAlign == ImageLayoutInfo.Default.HorizontalAlign

                && info.VerticalAlign == ImageLayoutInfo.Default.VerticalAlign;

        }



        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()

        {

            if (val == null)

                return default;



            try

            {

                ObjectValue_Bytes objectValue_Bytes = val.ToObjectValue_Bytes();

                if (objectValue_Bytes != null)

                {

                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Bytes);

                    IMPPropObjectValue obj = new T();

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

                    IMPPropObjectValue obj = new T();

                    obj.PopulateFromBaseValue(griffinsBaseValue);

                    return (T)obj;

                }

            }

            catch { }



            return default;

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

                    var propertyEditModelBase = JsonObjConvert.FromJSon<ImagePropertyModelEdit>(propertyEditJson);

                    if (propertyEditModelBase != null)

                    {

                        (PropertyEditModelBase as ImagePropertyModelEdit).CopyFrom(propertyEditModelBase);

                        _loadedPropertyEditFromBytes = true;

                    }

                }

                catch { }

            }

            string commonInfoJson = br.ReadString("ImageCommonInfo");

            if (!string.IsNullOrEmpty(commonInfoJson))

            {

                try

                {

                    // 图片公共信息单独持久化，避免图片字节只依赖 PropertyEditModelBase 的常规 JSON 链丢失。
                    var commonInfo = new ImageCommonInfo();

                    ((IJsonValueConvert)commonInfo).FromJsonDataObject(commonInfoJson);

                    ImagePropertyModelEdit.CommonInfo.CopyFrom(commonInfo);

                    _loadedPropertyEditFromBytes = true;

                }

                catch { }

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

                catch { }

            }

            EnsureEventBindEditModel(EventBindEditModel);

            try

            {

                // 读取完成后主动刷新一次公共信息执行链，确保设计时和运行时都能立即看到图片。
                ExecuteOprtByPropertyId(nameof(ImagePropertyModelEdit.CommonInfo));

            }

            catch { }

        }



        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)

        {

            base.OnWriteDrawInfoToBytes(bw);

            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));

            try

            {

                // 图片公共信息额外单独落盘，保证图片字节能稳定进入设计持久化链。
                var commonInfoToSave = new ImageCommonInfo();

                commonInfoToSave.CopyFrom(ImagePropertyModelEdit.CommonInfo);

                bw.Write("ImageCommonInfo", ((IJsonValueConvert)commonInfoToSave).ToJsonDataObject());

            }

            catch { }

            bw.Write("EventBindEditModel", JsonSerializer.Serialize(EventBindEditModel));

        }



        #endregion



        #region 重写方法



        protected override void OnCopyFrom(ControlCellBase source)

        {

            MapCellImageCtlObj sourceObj = source as MapCellImageCtlObj;

            base._CopyFrom(sourceObj);

            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);

            EventBindEditModel.CopyFrom(source.EventBindEditModel);

            EnsureEventBindEditModel(EventBindEditModel);

        }



        protected override void OnInit()

        {

            base.OnInit();

        }





        protected override object OnGetView() => view;



        protected override object OnGetViewModel() => imageViewModel;



        public override PropertyEditModelBase CreatePropertyModelEditBase() => new ImagePropertyModelEdit();



        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new ImagePropertyBindEditModel();



        public override EventBindEditModel CreateEventBindEditModel()
        {
            if (_eventBindEditModel == null)
            {
                _eventBindEditModel = new EventBindEditModel()
                {
                    EventCmdInfos = CreateDefaultEventCmdInfos()
                };
            }

            EnsureEventBindEditModel(_eventBindEditModel);
            return _eventBindEditModel;
        }



        public override void OnZoomChanged() { }



        public override string ToString() => ResourceA.ImageMapCell;



        #endregion



        #region 操作原子执行



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

                case nameof(ImagePropertyModelEdit.AppearanceInfo):

                    oprtCellId = ImageMapOprtCellConst.AppearanceInfo_MapOprtCellID;

                    return true;

                case nameof(ImagePropertyModelEdit.CommonInfo):

                    oprtCellId = ImageMapOprtCellConst.CommonInfo_MapOprtCellID;

                    return true;

                case nameof(ImagePropertyModelEdit.LayoutInfo):

                    oprtCellId = ImageMapOprtCellConst.LayoutInfo_MapOprtCellID;

                    return true;

                default:

                    return false;

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

                if (val is MapOprtInfo oprtInfo) yield return oprtInfo;

                if (val is IEnumerable enumerable && val is not string)

                {

                    foreach (var item in enumerable)

                        if (item is MapOprtInfo info) yield return info;

                }

            }

        }



        private static string GetOprtInfoId(object oprtInfo)

        {

            if (oprtInfo is MapOprtInfo info) return info.PropertyID;

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

                foreach (var p in t.GetProperties(flags))

                    if (p.GetIndexParameters().Length == 0 && p.CanRead) yield return p;

            }

        }



        private static object GetMemberValue(MemberInfo member, object instance)

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

            catch { return null; }

        }



        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)

        {

            if (mapOprtCellInstInfo.OprtCellID == ImageMapOprtCellConst.AppearanceInfo_MapOprtCellID)

                return ExecuteOprtCell<AppearanceInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == ImageMapOprtCellConst.CommonInfo_MapOprtCellID)

                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == ImageMapOprtCellConst.LayoutInfo_MapOprtCellID)

                return ExecuteOprtCell<LayoutInfoMapOprtCellExector>(mapOprtCellInstInfo);

            return base.ExecOprtCell(mapOprtCellInstInfo);

        }



        private bool ExecuteOprtCell<T>(MapOprtCellInstInfo mapOprtCellInstInfo) where T : IMapOprtCellExector, new()

        {

            if (_oprtCellIdByInstanceId.TryGetValue(mapOprtCellInstInfo.InstanceID, out var oldOprtCellId)

                && oldOprtCellId != mapOprtCellInstInfo.OprtCellID)

            {

                MapOprtCellExectorDict.TryRemove(mapOprtCellInstInfo.InstanceID, out _);

            }



            if (!MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector)

                || mapOprtCellExector == null || mapOprtCellExector.GetType() != typeof(T))

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



        #endregion



        #region 操作原子执行对象



        private static void PostToUI(Action action)

        {

            if (Dispatcher.UIThread.CheckAccess()) action();

            else Dispatcher.UIThread.Post(action);

        }

        public override MapCellPropValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null;
        }

        private class AppearanceInfoMapOprtCellExector : IMapOprtCellExector

        {

            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)

            {

                if (callBack.GetMapCellVMObjInstance() is ImageViewModel vm)

                {

                    // 浼樺厛浣跨敤cfg閰嶇疆

                    if (cfg != null && cfg.Length > 0)

                    {

                        try

                        {

                            var param = JsonSerializer.Deserialize<AppearanceInfoCfg>(Encoding.UTF8.GetString(cfg));

                            if (param != null)

                            {

                                PostToUI(() =>

                                {

                                    if (double.TryParse(param.Opacity, out var opacity)) vm.Opacity = opacity;

                                });

                                return;

                            }

                        }

                        catch { }

                    }

                    // 浠嶮odel璇诲彇

                    var val = callBack.GetMapCellPropValue(nameof(ImagePropertyModelEdit.AppearanceInfo));

                    if (val != null)

                    {

                        var info = DeserializeObject<ImageAppearanceInfo>(val);

                        PostToUI(() =>

                        {

                            vm.Opacity = info.Opacity;


                        });

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

                if (callBack.GetMapCellVMObjInstance() is ImageViewModel vm)

                {

                    // 浼樺厛浣跨敤cfg閰嶇疆

                    if (cfg != null && cfg.Length > 0)

                    {

                        try

                        {

                            var param = JsonSerializer.Deserialize<CommonInfoCfg>(Encoding.UTF8.GetString(cfg));

                            if (param != null)

                            {

                                PostToUI(() =>

                                {

                                    if (param.StretchMode.HasValue) vm.StretchMode = param.StretchMode.Value;

                                    if (param.IsEnabled.HasValue) vm.IsEnabled = param.IsEnabled.Value;

                                    if (param.ToolTip != null) vm.ToolTip = param.ToolTip;

                                });

                                return;

                            }

                        }

                        catch { }

                    }

                    // 浠嶮odel璇诲彇

                    var val = callBack.GetMapCellPropValue(nameof(ImagePropertyModelEdit.CommonInfo));

                    if (val != null)

                    {

                        var info = DeserializeObject<ImageCommonInfo>(val);

                        PostToUI(() =>

                        {

                            vm.ImageSource = info.ImageSource;

                            vm.StretchMode = info.StretchMode;

                            vm.IsEnabled = info.IsEnabled;

                            vm.ToolTip = info.ToolTip;

                        });

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

                if (callBack.GetMapCellVMObjInstance() is ImageViewModel vm)

                {

                    // 浼樺厛浣跨敤cfg閰嶇疆

                    if (cfg != null && cfg.Length > 0)

                    {

                        try

                        {

                            var param = JsonSerializer.Deserialize<LayoutInfoCfg>(Encoding.UTF8.GetString(cfg));

                            if (param != null)

                            {

                                PostToUI(() =>

                                {

                                    // 宽高主数据统一落到父类 Width/Height，图片视图不再维护额外的尺寸中间态。
                                    if (param.HorizontalAlign.HasValue) vm.HorizontalAlign = param.HorizontalAlign.Value;

                                    if (param.VerticalAlign.HasValue) vm.VerticalAlign = param.VerticalAlign.Value;

                                    if (double.TryParse(param.MarginTop, out var mt)) vm.MarginTop = mt;

                                    if (double.TryParse(param.MarginLeft, out var ml)) vm.MarginLeft = ml;

                                    if (double.TryParse(param.MarginBottom, out var mb)) vm.MarginBottom = mb;

                                    if (double.TryParse(param.MarginRight, out var mr)) vm.MarginRight = mr;

                                    if (double.TryParse(param.MinWidth, out var minW)) vm.MinWidth = minW;

                                    if (double.TryParse(param.MaxWidth, out var maxW)) vm.MaxWidth = maxW;

                                    if (double.TryParse(param.MinHeight, out var minH)) vm.MinHeight = minH;

                                    if (double.TryParse(param.MaxHeight, out var maxH)) vm.MaxHeight = maxH;

                                });

                                return;

                            }

                        }

                        catch { }

                    }

                    // 浠嶮odel璇诲彇

                    var val = callBack.GetMapCellPropValue(nameof(ImagePropertyModelEdit.LayoutInfo));

                    if (val != null)

                    {

                        var info = DeserializeObject<ImageLayoutInfo>(val);

                        PostToUI(() =>

                        {

                            vm.HorizontalAlign = info.HorizontalAlign;

                            vm.VerticalAlign = info.VerticalAlign;

                            vm.MarginTop = info.MarginTop;

                            vm.MarginLeft = info.MarginLeft;

                            vm.MarginBottom = info.MarginBottom;

                            vm.MarginRight = info.MarginRight;

                            vm.MinWidth = info.MinWidth;

                            vm.MaxWidth = info.MaxWidth;

                            vm.MinHeight = info.MinHeight;

                            vm.MaxHeight = info.MaxHeight;

                        });

                    }

                }

            }

        }



        // cfg閰嶇疆绫?

        private class AppearanceInfoCfg

        {

            public string Opacity { get; set; } = string.Empty;

        }



        private class CommonInfoCfg

        {

            public ImageStretchMode? StretchMode { get; set; }

            public bool? IsEnabled { get; set; }

            public string ToolTip { get; set; }

        }



        private class LayoutInfoCfg

        {

            public string Width { get; set; }

            public string Height { get; set; }

            public HorizontalAlignType? HorizontalAlign { get; set; }

            public VerticalAlignType? VerticalAlign { get; set; }

            public string MarginTop { get; set; }

            public string MarginLeft { get; set; }

            public string MarginBottom { get; set; }

            public string MarginRight { get; set; }

            public string MinWidth { get; set; }

            public string MaxWidth { get; set; }

            public string MinHeight { get; set; }

            public string MaxHeight { get; set; }

        }



        #endregion

    }



    /// <summary>

    /// 图片图元属性编辑模型对象

    /// </summary>

    [Serializable]

    [MapPropertyOrder]

    [CategoryPriority("外观", 1)]

    [CategoryPriority("公共", 2)]

    [CategoryPriority("布局", 3)]

    public class ImagePropertyModelEdit : ControlCellPropertyModelEdit

    {

        public ImagePropertyModelEdit()

        {

            AppearanceInfo = new ImageAppearanceInfo();

            CommonInfo = new ImageCommonInfo();

            LayoutInfo = new ImageLayoutInfo();

        }



        private bool _suppressChildNotify;



        private bool _childNotifyPending;



        private int _changeStamp;



        [Browsable(false)]

        public int ChangeStamp

        {

            get => _changeStamp;

            private set => SetProperty(ref _changeStamp, value);

        }



        private void OnChildInfoPropertyChanged(object sender, PropertyChangedEventArgs e)

        {

            if (_suppressChildNotify)

                return;



            if (_childNotifyPending)

                return;



            _childNotifyPending = true;

            Dispatcher.UIThread.Post(() =>

            {

                _childNotifyPending = false;



                ChangeStamp++;

            }, DispatcherPriority.Background);

        }



        private void AttachChild(System.ComponentModel.INotifyPropertyChanged child)

        {

            if (child == null) return;

            child.PropertyChanged += OnChildInfoPropertyChanged;

        }



        private void DetachChild(System.ComponentModel.INotifyPropertyChanged child)

        {

            if (child == null) return;

            child.PropertyChanged -= OnChildInfoPropertyChanged;

        }



        #region 外观属性



        private ImageAppearanceInfo _appearanceInfo;

        [DisplayName("外观")]

        [Category("外观")]

        [PropertySortOrder(1)]

        public ImageAppearanceInfo AppearanceInfo

        {

            get => _appearanceInfo;

            set

            {

                if (value == null)

                    value = new ImageAppearanceInfo();



                if (ReferenceEquals(_appearanceInfo, value))

                    return;



                if (_appearanceInfo is System.ComponentModel.INotifyPropertyChanged old)

                    DetachChild(old);



                _appearanceInfo = value;



                if (_appearanceInfo is System.ComponentModel.INotifyPropertyChanged cur)

                    AttachChild(cur);



                RaisePropertyChanged(nameof(AppearanceInfo));

            }

        }



        #endregion



        #region 公共属性



        private ImageCommonInfo _commonInfo;

        [DisplayName("公共")]

        [Category("公共")]

        [PropertySortOrder(1)]

        public ImageCommonInfo CommonInfo

        {

            get => _commonInfo;

            set

            {

                if (value == null)

                    value = new ImageCommonInfo();



                if (ReferenceEquals(_commonInfo, value))

                    return;



                if (_commonInfo is System.ComponentModel.INotifyPropertyChanged old)

                    DetachChild(old);



                _commonInfo = value;



                if (_commonInfo is System.ComponentModel.INotifyPropertyChanged cur)

                    AttachChild(cur);



                RaisePropertyChanged(nameof(CommonInfo));

            }

        }



        #endregion



        #region 布局属性



        private ImageLayoutInfo _layoutInfo;

        [DisplayName("布局")]

        [Category("布局")]

        [PropertySortOrder(1)]

        public ImageLayoutInfo LayoutInfo

        {

            get => _layoutInfo;

            set

            {

                if (value == null)

                    value = new ImageLayoutInfo();



                if (ReferenceEquals(_layoutInfo, value))

                    return;



                if (_layoutInfo is System.ComponentModel.INotifyPropertyChanged old)

                    DetachChild(old);



                _layoutInfo = value;



                if (_layoutInfo is System.ComponentModel.INotifyPropertyChanged cur)

                    AttachChild(cur);



                RaisePropertyChanged(nameof(LayoutInfo));

            }

        }



        #endregion



        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)

        {

            if (string.Compare(propertyID, nameof(ImageCommonInfo.ImageSource)) == 0)

            {

                _commonInfo ??= new ImageCommonInfo();

                if (propertyVal != null)

                {

                    try

                    {

                        var base64 = propertyVal.ToPrimitiveValue<string>();

                        var bd = new BitmapData();

                        ((IJsonValueConvert)bd).FromJsonDataObject(base64);

                        _commonInfo.ImageSource = bd;

                    }

                    catch

                    {

                        _commonInfo.ImageSource = new BitmapData();

                    }

                }

                else

                {

                    _commonInfo.ImageSource = ImageCommonInfo.Default.ImageSource ?? new BitmapData();

                }

                RaisePropertyChanged(nameof(CommonInfo));

                ChangeStamp++;

                return true;

            }

            if (string.Compare(propertyID, nameof(ImageAppearanceInfo.Opacity)) == 0)

            {

                _appearanceInfo ??= new ImageAppearanceInfo();

                _appearanceInfo.Opacity = propertyVal != null ? (double)propertyVal.ToPrimitiveValue<decimal>() : ImageAppearanceInfo.Default.Opacity;

                RaisePropertyChanged(nameof(AppearanceInfo));

                ChangeStamp++;

                return true;

            }



            if (string.Compare(propertyID, nameof(AppearanceInfo)) == 0)

            {

                var src = propertyVal != null ? DeserializeObject<ImageAppearanceInfo>(propertyVal) : new ImageAppearanceInfo();

                _appearanceInfo ??= new ImageAppearanceInfo();

                try

                {

                    _suppressChildNotify = true;

                    _appearanceInfo.CopyFrom(src);

                }

                finally

                {

                    _suppressChildNotify = false;

                }

                RaisePropertyChanged(nameof(AppearanceInfo));

                ChangeStamp++;

                return true;

            }

            if (string.Compare(propertyID, nameof(CommonInfo)) == 0)

            {

                var src = propertyVal != null ? DeserializeObject<ImageCommonInfo>(propertyVal) : new ImageCommonInfo();

                _commonInfo ??= new ImageCommonInfo();

                try

                {

                    _suppressChildNotify = true;

                    _commonInfo.CopyFrom(src);

                }

                finally

                {

                    _suppressChildNotify = false;

                }

                RaisePropertyChanged(nameof(CommonInfo));

                ChangeStamp++;

                return true;

            }

            if (string.Compare(propertyID, nameof(LayoutInfo)) == 0)

            {

                var src = propertyVal != null ? DeserializeObject<ImageLayoutInfo>(propertyVal) : new ImageLayoutInfo();

                _layoutInfo ??= new ImageLayoutInfo();

                try

                {

                    _suppressChildNotify = true;

                    _layoutInfo.CopyFrom(src);

                }

                finally

                {

                    _suppressChildNotify = false;

                }

                RaisePropertyChanged(nameof(LayoutInfo));

                ChangeStamp++;

                return true;

            }

            return base.SetPropertyValue(propertyID, propertyVal);

        }



        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()

        {

            if (val == null)

                return default;



            try

            {

                ObjectValue_Bytes objectValue_Bytes = val.ToObjectValue_Bytes();

                if (objectValue_Bytes != null)

                {

                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Bytes);

                    IMPPropObjectValue obj = new T();

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

                    IMPPropObjectValue obj = new T();

                    obj.PopulateFromBaseValue(griffinsBaseValue);

                    return (T)obj;

                }

            }

            catch { }



            return default;

        }



        public void CopyFrom(ImagePropertyModelEdit source)

        {

            if (source == null) return;

            base.CopyFrom(source);



            _appearanceInfo ??= new ImageAppearanceInfo();

            _commonInfo ??= new ImageCommonInfo();

            _layoutInfo ??= new ImageLayoutInfo();



            AttachChild(_appearanceInfo);

            AttachChild(_commonInfo);

            AttachChild(_layoutInfo);



            try

            {

                _suppressChildNotify = true;

                if (source.AppearanceInfo != null)

                    _appearanceInfo.CopyFrom(source.AppearanceInfo);

                if (source.CommonInfo != null)

                    _commonInfo.CopyFrom(source.CommonInfo);

                if (source.LayoutInfo != null)

                    _layoutInfo.CopyFrom(source.LayoutInfo);

            }

            finally

            {

                _suppressChildNotify = false;

            }



            RaisePropertyChanged(nameof(AppearanceInfo));

            RaisePropertyChanged(nameof(CommonInfo));

            RaisePropertyChanged(nameof(LayoutInfo));



            ChangeStamp++;

        }

    }



    /// <summary>

    /// 图片图元属性绑定编辑模型

    /// </summary>

    [Serializable]

    [MapPropertyOrder]

    [CategoryPriority("绑定信息", 1)]

    public class ImagePropertyBindEditModel : ControlCellPropertyBindEditModel

    {
        public void CopyFrom(ImagePropertyBindEditModel source)

        {

            base.CopyFrom(source);

        }

    }

}





