using Avalonia.Media;

using Avalonia.Threading;

using GF_Gereric;

using Griffins;

using GKG.Map.MapCell.Generic;

using GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.Objects;

using GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.ViewModels;

using GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.Views;

using GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.MapOprtCellParamCfgView;

using GKG.Map.MapCell.Generic.Control.Lable;

using Newtonsoft.JsonG;

using PropertyModels.ComponentModel;

using System;

using System.Collections;

using System.Collections.Concurrent;

using System.Collections.Generic;

using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Text.Json;

using JsonSerializer = System.Text.Json.JsonSerializer;



namespace GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup

{

    /// <summary>

    /// 图片组图元控件对象

    /// </summary>

    class MapCellImageGroupCtlObj : ControlCellBase
    {
        private static readonly string[] DefaultEventIds =
        {
            MapObjPropEventConst.Event_MouseClick
        };
        private const string HasBoundMouseClickCommandKey = "HasBoundMouseClickCommand";
        #region 私有字段



        private ImageGroupView view;
        private ImageGroupViewModel imageGroupViewModel;
        private EventBindEditModel _eventBindEditModel;
        private bool _hasBoundMouseClickCommand;
        private bool _loadedPropertyEditFromBytes;
        private bool _isRestoringFromSerializedState;

        private static bool IsRecoverablePersistenceException(Exception ex)
        {
            return ex is System.Text.Json.JsonException
                || ex is FormatException
                || ex is InvalidOperationException
                || ex is NotSupportedException
                || ex is ArgumentException;
        }

        private static void TracePersistenceIssue(string stage, Exception ex)
        {
            string message = $"[ImageGroup] {stage} failed: {ex.GetType().Name}: {ex.Message}";
            Trace.WriteLine(message);
            Debug.WriteLine(message);
        }
        private readonly ConcurrentDictionary<Guid, MapOprtCellID> _oprtCellIdByInstanceId = new();





        #endregion



        #region 构造函数



        public MapCellImageGroupCtlObj(MapObjID mapCellID, string mapCellName)

            : this(mapCellID, mapCellName, false) { }



        public MapCellImageGroupCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)

        {

            PropertyEditModelBase = CreatePropertyModelEditBase();

            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();

            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);

            base.SetName(mapCellName);

            view = new ImageGroupView();



            // 注册对象属性

            RegisterProperty(new MapObjPropertyInfo(nameof(ImageGroupPropertyModelEdit.AppearanceInfo), ResourceA.ImageGroup_AppearanceInfo, MapCellPropDataType.Object_Json, ImageGroupAppearanceInfo.Object_ID, typeof(ImageGroupAppearanceInfo), false, true, new MapCellPropValue(ImageGroupAppearanceInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ImageGroupPropertyModelEdit.CommonInfo), ResourceA.ImageGroup_CommonInfo, MapCellPropDataType.Object_Bytes, ImageGroupCommonInfo.Object_ID, typeof(ImageGroupCommonInfo), false, true, new MapCellPropValue(ImageGroupCommonInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ImageGroupPropertyModelEdit.LayoutInfo), ResourceA.ImageGroup_LayoutInfo, MapCellPropDataType.Object_Json, ImageGroupLayoutInfo.Object_ID, typeof(ImageGroupLayoutInfo), false, true, new MapCellPropValue(ImageGroupLayoutInfo.Default)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ImageGroupPropertyModelEdit.CurrentIndex), ResourceA.ImageGroup_CurrentIndex, MapCellPropDataType.Integer, Guid.Empty, typeof(int), false, true, new MapCellPropValue(0)));
            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), MapCellPropDataType.Object_Bytes, GraphMouseEventParam.Object_ID));



            // 注册操作原子信息

            RegisterOprtCellInfo(new MapOprtCellInfo(ImageGroupMapOprtCellConst.AppearanceInfo_MapOprtCellID, ResourceA.ImageGroup_AppearanceInfo_MapOprtCellName, typeof(AppearanceInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(ImageGroupMapOprtCellConst.CommonInfo_MapOprtCellID, ResourceA.ImageGroup_CommonInfo_MapOprtCellName, typeof(CommonInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(ImageGroupMapOprtCellConst.LayoutInfo_MapOprtCellID, ResourceA.ImageGroup_LayoutInfo_MapOprtCellName, typeof(LayoutInfoMapOprtCellParamCfgView)));

            RegisterOprtCellInfo(new MapOprtCellInfo(ImageGroupMapOprtCellConst.CurrentIndex_MapOprtCellID, ResourceA.ImageGroup_CurrentIndex_MapOprtCellName, typeof(CurrentIndexMapOprtCellParamCfgView)));



            // 注册操作信息

            RegisterOprtInfo(new MapOprtInfo(nameof(ImageGroupPropertyModelEdit.AppearanceInfo), ResourceA.ImageGroup_AppearanceInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = ImageGroupMapOprtCellConst.AppearanceInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(ImageGroupPropertyModelEdit.CommonInfo), ResourceA.ImageGroup_CommonInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = ImageGroupMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(ImageGroupPropertyModelEdit.LayoutInfo), ResourceA.ImageGroup_LayoutInfo_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = ImageGroupMapOprtCellConst.LayoutInfo_MapOprtCellID, CfgInfo = null } }));

            RegisterOprtInfo(new MapOprtInfo(nameof(ImageGroupPropertyModelEdit.CurrentIndex), ResourceA.ImageGroup_CurrentIndex_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = ImageGroupMapOprtCellConst.CurrentIndex_MapOprtCellID, CfgInfo = null } }));



            (this as IMapCellTypeBase).Name = ResourceA.ImageGroupMapCell;

            imageGroupViewModel = new ImageGroupViewModel(designTime, ImageGroupPropertyModelEdit, HandleMouseClick);

            view.DataContext = imageGroupViewModel;



            // 订阅 Info 对象的 PropertyChanged，触发操作原子执行（参考 Label 模式）

            ImageGroupPropertyModelEdit.AppearanceInfo.PropertyChanged += OnAppearanceInfoPropertyChanged;
            ImageGroupPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
            ImageGroupPropertyModelEdit.LayoutInfo.PropertyChanged += OnLayoutInfoPropertyChanged;
            ImageGroupPropertyModelEdit.PropertyChanged += OnPropertyModelPropertyChanged;





        }

        private void OnAppearanceInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(ImageGroupPropertyModelEdit.AppearanceInfo));
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(ImageGroupPropertyModelEdit.CommonInfo));
        }

        private void OnLayoutInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(ImageGroupPropertyModelEdit.LayoutInfo));
        }

        private void OnPropertyModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            if (e?.PropertyName == nameof(ImageGroupPropertyModelEdit.CurrentIndex))
                ExecuteOprtByPropertyId(nameof(ImageGroupPropertyModelEdit.CurrentIndex));
        }

        public override void OnDispose()
        {
            ImageGroupPropertyModelEdit.AppearanceInfo.PropertyChanged -= OnAppearanceInfoPropertyChanged;
            ImageGroupPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;
            ImageGroupPropertyModelEdit.LayoutInfo.PropertyChanged -= OnLayoutInfoPropertyChanged;
            ImageGroupPropertyModelEdit.PropertyChanged -= OnPropertyModelPropertyChanged;

            view.DataContext = null;
            imageGroupViewModel?.Dispose();
            imageGroupViewModel = null;

            base.OnDispose();
        }



        #endregion



        #region 属性



        public ImageGroupPropertyModelEdit ImageGroupPropertyModelEdit => PropertyEditModelBase as ImageGroupPropertyModelEdit;



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

        private bool HasCurrentMouseClickCommandBinding()
        {
            EventCmdInfo eventCmdInfo = EventBindEditModel?.EventCmdInfos?.FirstOrDefault(info => info.EventID == MapObjPropEventConst.Event_MouseClick);
            return eventCmdInfo != null && !string.IsNullOrWhiteSpace(eventCmdInfo.CmdID);
        }

        private bool HasEffectiveMouseClickCommandBinding()
        {
            return HasCurrentMouseClickCommandBinding() || _hasBoundMouseClickCommand;
        }

        private void SyncBoundMouseClickCommandFromCurrent()
        {
            _hasBoundMouseClickCommand = HasCurrentMouseClickCommandBinding();
        }

        private void RestoreBoundMouseClickCommand(string persistedValue)
        {
            _hasBoundMouseClickCommand = false;
            if (!string.IsNullOrWhiteSpace(persistedValue))
            {
                bool.TryParse(persistedValue, out _hasBoundMouseClickCommand);
            }

            if (HasCurrentMouseClickCommandBinding())
            {
                _hasBoundMouseClickCommand = true;
            }
        }

        private void HandleMouseClick()
        {
            EventCmdInfo eventCmdInfo = EventBindEditModel?.EventCmdInfos?.FirstOrDefault(info => info.EventID == MapObjPropEventConst.Event_MouseClick);
            if (eventCmdInfo != null)
            {
                
                ExecEvent(MapObjPropEventConst.Event_MouseClick);
                if (HasEffectiveMouseClickCommandBinding())
                {
                    AdvanceCurrentIndexOnClick();
                }
                
            }
        }

        private void AdvanceCurrentIndexOnClick()
        {
            var imageCount = ImageGroupPropertyModelEdit?.CommonInfo?.ImageSources?.Count ?? 0;
            if (imageCount <= 1)
            {
                return;
            }

            var currentIndex = ImageGroupPropertyModelEdit.CurrentIndex;
            if (currentIndex < 0 || currentIndex >= imageCount)
            {
                currentIndex = 0;
            }

            var nextIndex = (currentIndex + 1) % imageCount;
            SetPropertyValue(nameof(ImageGroupPropertyModelEdit.CurrentIndex), new MapCellPropValue(nextIndex), false);
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

                return true;

            ImageGroupPropertyModelEdit.IsRuning = isRuning;

            var ok = ImageGroupPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);

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



            if (ImageGroupPropertyModelEdit.IsRuning)

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

                if (string.Compare(propertyID, nameof(ImageGroupPropertyModelEdit.AppearanceInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<ImageGroupAppearanceInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(ImageGroupPropertyModelEdit.AppearanceInfo);

                }

                if (string.Compare(propertyID, nameof(ImageGroupPropertyModelEdit.CommonInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<ImageGroupCommonInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(ImageGroupPropertyModelEdit.CommonInfo);

                }

                if (string.Compare(propertyID, nameof(ImageGroupPropertyModelEdit.LayoutInfo)) == 0)

                {

                    var incoming = propertyVal != null ? DeserializeObject<ImageGroupLayoutInfo>(propertyVal) : null;

                    return IsDefault(incoming) && !IsDefault(ImageGroupPropertyModelEdit.LayoutInfo);

                }

                return false;

            }

            catch { return false; }

        }



        private static bool IsDefault(ImageGroupAppearanceInfo info)

        {

            if (info == null) return true;

            return Math.Abs(info.Opacity - ImageGroupAppearanceInfo.Default.Opacity) < 0.000001;

        }



        private static bool IsDefault(ImageGroupCommonInfo info)

        {

            if (info == null) return true;

            return (info.ImageSources == null || info.ImageSources.Count == 0)

                && info.CurrentIndex == ImageGroupCommonInfo.Default.CurrentIndex

                && info.StretchMode == ImageGroupCommonInfo.Default.StretchMode

                && info.IsEnabled == ImageGroupCommonInfo.Default.IsEnabled

                && string.Equals(info.ToolTip ?? "", ImageGroupCommonInfo.Default.ToolTip ?? "", StringComparison.Ordinal);

        }



        private static bool IsDefault(ImageGroupLayoutInfo info)

        {

            if (info == null) return true;

            return info.HorizontalAlign == ImageGroupLayoutInfo.Default.HorizontalAlign

                && info.VerticalAlign == ImageGroupLayoutInfo.Default.VerticalAlign;

        }



        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()

        {

            if (val == null) return default;

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
            _isRestoringFromSerializedState = true;

            string propertyEditJson = br.ReadString("PropertyEditModelBase");

            if (!string.IsNullOrEmpty(propertyEditJson))

            {

                try

                {

                    var propertyEditModelBase = JsonObjConvert.FromJSon<ImageGroupPropertyModelEdit>(propertyEditJson);

                    if (propertyEditModelBase != null)

                    {

                        (PropertyEditModelBase as ImageGroupPropertyModelEdit).CopyFrom(propertyEditModelBase);

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

                    var propertyBindEditModelBase = JsonObjConvert.FromJSon<ImageGroupPropertyBindEditModel>(propertyBindJson);

                    if (propertyBindEditModelBase != null)

                    {

                        (PropertyBindEditModelBase as ImageGroupPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);

                    }

                }

                catch (Exception ex) when (IsRecoverablePersistenceException(ex))
                {
                    TracePersistenceIssue("Read PropertyEditModelBase", ex);
                }

            }

            string commonInfoJson = br.ReadString("ImageGroupCommonInfo");

            if (!string.IsNullOrEmpty(commonInfoJson))

            {

                try

                {

                    var commonInfo = new ImageGroupCommonInfo();

                    ((IJsonValueConvert)commonInfo).FromJsonDataObject(commonInfoJson);

                    ImageGroupPropertyModelEdit.CommonInfo.CopyFrom(commonInfo);

                    _loadedPropertyEditFromBytes = true;

                }

                catch (Exception ex) when (IsRecoverablePersistenceException(ex))
                {
                    TracePersistenceIssue("Read PropertyBindEditModelBase", ex);
                }

            }

            RestoreBoundMouseClickCommand(br.ReadString(HasBoundMouseClickCommandKey));

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

                catch (Exception ex) when (IsRecoverablePersistenceException(ex))
                {
                    TracePersistenceIssue("Read ImageGroupCommonInfo", ex);
                }

            }

            EnsureEventBindEditModel(EventBindEditModel);
            _isRestoringFromSerializedState = false;

            if (_loadedPropertyEditFromBytes)
            {
                ReloadImageGroupViewSynchronously();
            }

        }



        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)

        {

            base.OnWriteDrawInfoToBytes(bw);

            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));

            try

            {
                var commonInfoToSave = new ImageGroupCommonInfo();
                commonInfoToSave.CopyFrom(ImageGroupPropertyModelEdit.CommonInfo);
                commonInfoToSave.CurrentIndex = ImageGroupPropertyModelEdit.CurrentIndex;
                bw.Write("ImageGroupCommonInfo", ((IJsonValueConvert)commonInfoToSave).ToJsonDataObject());

            }

            catch (Exception ex) when (IsRecoverablePersistenceException(ex))
            {
                TracePersistenceIssue("Refresh CommonInfo after read", ex);
            }

            SyncBoundMouseClickCommandFromCurrent();
            bw.Write(HasBoundMouseClickCommandKey, _hasBoundMouseClickCommand.ToString());
            bw.Write("EventBindEditModel", JsonSerializer.Serialize(EventBindEditModel));

        }



        #endregion



        #region 重写方法



        protected override void OnCopyFrom(ControlCellBase source)

        {

            MapCellImageGroupCtlObj sourceObj = source as MapCellImageGroupCtlObj;
            _isRestoringFromSerializedState = true;

            base._CopyFrom(sourceObj);

            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
            (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);

            EventBindEditModel.CopyFrom(source.EventBindEditModel);
            _hasBoundMouseClickCommand = sourceObj?.HasEffectiveMouseClickCommandBinding() ?? false;

            EnsureEventBindEditModel(EventBindEditModel);
            _loadedPropertyEditFromBytes = true;
            _isRestoringFromSerializedState = false;
            ReloadImageGroupViewSynchronously();

        }



        protected override void OnInit()

        {

            base.OnInit();

            if (_loadedPropertyEditFromBytes)
            {
                ReloadImageGroupViewSynchronously();
            }

        }

        /// <summary>
        /// 图片组读档完成后统一同步刷新 ViewModel 和首帧布局，避免先空白一帧再补图。
        /// </summary>
        private void ReloadImageGroupViewSynchronously()
        {
            ExecuteOnUiThreadSynchronously(() =>
            {
                imageGroupViewModel?.ReloadFromModel(true);
                view?.ApplyInitialStateFromViewModel();
            });
        }

        /// <summary>
        /// 切页首帧需要在真正显示前完成图片组状态同步，这里统一同步切到 UI 线程。
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

        protected override object OnGetViewModel() => imageGroupViewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new ImageGroupPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new ImageGroupPropertyBindEditModel();

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

        public override string ToString() => ResourceA.ImageGroupMapCell;



        #endregion





        #region 操作原子执行



        private void ExecuteOprtByPropertyId(string propertyId)

        {

            if (string.IsNullOrWhiteSpace(propertyId)) return;

            if (!TryGetPrimaryOprtCellId(propertyId, out var normalizedOprtId, out var primaryOprtCellId)) return;

            TryExecuteOprtInfoById(normalizedOprtId, primaryOprtCellId);

        }



        private bool TryGetPrimaryOprtCellId(string propertyId, out string normalizedOprtId, out MapOprtCellID oprtCellId)

        {

            if (string.IsNullOrWhiteSpace(propertyId))

            {

                normalizedOprtId = propertyId ?? string.Empty;

                oprtCellId = default;

                return false;

            }

            var dot = propertyId.IndexOf('.');

            if (dot > 0)

                propertyId = propertyId.Substring(0, dot);

            normalizedOprtId = propertyId;

            oprtCellId = default;

            switch (propertyId)

            {

                case nameof(ImageGroupPropertyModelEdit.AppearanceInfo):

                    oprtCellId = ImageGroupMapOprtCellConst.AppearanceInfo_MapOprtCellID;

                    return true;

                case nameof(ImageGroupPropertyModelEdit.CommonInfo):

                    oprtCellId = ImageGroupMapOprtCellConst.CommonInfo_MapOprtCellID;

                    return true;

                case nameof(ImageGroupPropertyModelEdit.LayoutInfo):

                    oprtCellId = ImageGroupMapOprtCellConst.LayoutInfo_MapOprtCellID;

                    return true;

                case nameof(ImageGroupPropertyModelEdit.CurrentIndex):

                    oprtCellId = ImageGroupMapOprtCellConst.CurrentIndex_MapOprtCellID;

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

            catch (Exception ex) when (IsRecoverablePersistenceException(ex))
            {
                TracePersistenceIssue("Write ImageGroupCommonInfo", ex);
            }

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

            if (mapOprtCellInstInfo.OprtCellID == ImageGroupMapOprtCellConst.AppearanceInfo_MapOprtCellID)

                return ExecuteOprtCell<AppearanceInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == ImageGroupMapOprtCellConst.CommonInfo_MapOprtCellID)

                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == ImageGroupMapOprtCellConst.LayoutInfo_MapOprtCellID)

                return ExecuteOprtCell<LayoutInfoMapOprtCellExector>(mapOprtCellInstInfo);

            if (mapOprtCellInstInfo.OprtCellID == ImageGroupMapOprtCellConst.CurrentIndex_MapOprtCellID)

                return ExecuteOprtCell<CurrentIndexMapOprtCellExector>(mapOprtCellInstInfo);

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

                if (callBack.GetMapCellVMObjInstance() is ImageGroupViewModel vm)

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

                    var val = callBack.GetMapCellPropValue(nameof(ImageGroupPropertyModelEdit.AppearanceInfo));

                    if (val != null)

                    {

                        var info = DeserializeObject<ImageGroupAppearanceInfo>(val);

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

                if (callBack.GetMapCellVMObjInstance() is ImageGroupViewModel vm)

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

                                    if (param.CurrentIndex.HasValue) vm.CurrentIndex = param.CurrentIndex.Value;

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

                    var val = callBack.GetMapCellPropValue(nameof(ImageGroupPropertyModelEdit.CommonInfo));

                    if (val != null)

                    {

                        var info = DeserializeObject<ImageGroupCommonInfo>(val);

                        PostToUI(() =>

                        {

                            vm.ImageSources = info.ImageSources ?? new List<BitmapData>();

                            vm.CurrentIndex = info.CurrentIndex;

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

                if (callBack.GetMapCellVMObjInstance() is ImageGroupViewModel vm)

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

                                    // 宽高主数据统一落到父类 Width/Height，避免图片组继续维护额外尺寸中间态。
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

                    var val = callBack.GetMapCellPropValue(nameof(ImageGroupPropertyModelEdit.LayoutInfo));

                    if (val != null)

                    {

                        var info = DeserializeObject<ImageGroupLayoutInfo>(val);

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



        private class CurrentIndexMapOprtCellExector : IMapOprtCellExector

        {

            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)

            {

                if (callBack.GetMapCellVMObjInstance() is ImageGroupViewModel vm)

                {

                    // 浼樺厛浣跨敤cfg閰嶇疆

                    if (cfg != null && cfg.Length > 0)

                    {

                        try

                        {

                            var param = JsonSerializer.Deserialize<CurrentIndexCfg>(Encoding.UTF8.GetString(cfg));

                            if (param != null)

                            {

                                PostToUI(() =>

                                {

                                    if (param.CurrentIndex.HasValue) vm.CurrentIndex = param.CurrentIndex.Value;

                                });

                                return;

                            }

                        }

                        catch { }

                    }

                    // 浠嶮odel璇诲彇

                    var val = callBack.GetMapCellPropValue(nameof(ImageGroupPropertyModelEdit.CurrentIndex));

                    if (val != null)

                    {

                        var index = (int)val.ToInteger();

                        PostToUI(() => vm.CurrentIndex = index);

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

            public int? CurrentIndex { get; set; }

            public ImageGroupStretchMode? StretchMode { get; set; }

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



        private class CurrentIndexCfg

        {

            public int? CurrentIndex { get; set; }

        }



        #endregion

    }





    /// <summary>

    /// 图片组图元属性编辑模型对象

    /// </summary>

    [Serializable]

    [MapPropertyOrder]

    [CategoryPriority("外观", 1)]

    [CategoryPriority("公共", 2)]

    [CategoryPriority("布局", 3)]

    [CategoryPriority("索引", 4)]

    public class ImageGroupPropertyModelEdit : ControlCellPropertyModelEdit

    {
        private readonly PropertyChangedEventHandler _appearanceInfoChangedHandler;

        private readonly PropertyChangedEventHandler _commonInfoChangedHandler;

        private readonly PropertyChangedEventHandler _layoutInfoChangedHandler;

        public ImageGroupPropertyModelEdit()

        {
            _appearanceInfoChangedHandler = (_, __) => RaisePropertyChanged(nameof(AppearanceInfo));

            _commonInfoChangedHandler = (_, __) => RaisePropertyChanged(nameof(CommonInfo));

            _layoutInfoChangedHandler = (_, __) => RaisePropertyChanged(nameof(LayoutInfo));

            AppearanceInfo = new ImageGroupAppearanceInfo();

            CommonInfo = new ImageGroupCommonInfo();

            LayoutInfo = new ImageGroupLayoutInfo();

        }



        #region 外观属性



        private ImageGroupAppearanceInfo _appearanceInfo;

        [DisplayName("外观")]

        [Category("外观")]

        [PropertySortOrder(1)]

        public ImageGroupAppearanceInfo AppearanceInfo

        {

            get => _appearanceInfo;

            set

            {

                if (value == null) value = new ImageGroupAppearanceInfo();

                if (ReferenceEquals(_appearanceInfo, value)) return;

                if (_appearanceInfo != null)

                    _appearanceInfo.PropertyChanged -= _appearanceInfoChangedHandler;

                _appearanceInfo = value;

                _appearanceInfo.PropertyChanged += _appearanceInfoChangedHandler;

                RaisePropertyChanged(nameof(AppearanceInfo));

            }

        }



        #endregion



        #region 公共属性



        private ImageGroupCommonInfo _commonInfo;

        [DisplayName("公共")]

        [Category("公共")]

        [PropertySortOrder(1)]

        public ImageGroupCommonInfo CommonInfo

        {

            get => _commonInfo;

            set

            {

                if (value == null) value = new ImageGroupCommonInfo();

                if (ReferenceEquals(_commonInfo, value)) return;

                if (_commonInfo != null)

                    _commonInfo.PropertyChanged -= _commonInfoChangedHandler;

                _commonInfo = value;

                _commonInfo.PropertyChanged += _commonInfoChangedHandler;

                RaisePropertyChanged(nameof(CommonInfo));

            }

        }



        #endregion



        #region 布局属性



        private ImageGroupLayoutInfo _layoutInfo;

        [DisplayName("布局")]

        [Category("布局")]

        [PropertySortOrder(1)]

        public ImageGroupLayoutInfo LayoutInfo

        {

            get => _layoutInfo;

            set

            {

                if (value == null) value = new ImageGroupLayoutInfo();

                if (ReferenceEquals(_layoutInfo, value)) return;

                if (_layoutInfo != null)

                    _layoutInfo.PropertyChanged -= _layoutInfoChangedHandler;

                _layoutInfo = value;

                _layoutInfo.PropertyChanged += _layoutInfoChangedHandler;

                RaisePropertyChanged(nameof(LayoutInfo));

            }

        }



        #endregion



        #region 索引属性



        private int _currentIndex;

        [DisplayName("当前索引")]

        [Category("索引")]

        [PropertySortOrder(1)]

        public int CurrentIndex

        {

            get => _currentIndex;

            set

            {

                if (_currentIndex == value) return;

                _currentIndex = value;

                RaisePropertyChanged(nameof(CurrentIndex));

            }

        }



        #endregion



        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)

        {

            if (string.Compare(propertyID, nameof(ImageGroupAppearanceInfo.Opacity)) == 0)

            {

                _appearanceInfo ??= new ImageGroupAppearanceInfo();

                _appearanceInfo.Opacity = propertyVal != null ? (double)propertyVal.ToPrimitiveValue<decimal>() : ImageGroupAppearanceInfo.Default.Opacity;

                RaisePropertyChanged(nameof(AppearanceInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(ImageGroupPropertyModelEdit.CurrentIndex)) == 0)

            {

                _currentIndex = (int)(propertyVal?.ToInteger() ?? 0);

                RaisePropertyChanged(nameof(CurrentIndex));

                return true;

            }



            if (string.Compare(propertyID, nameof(AppearanceInfo)) == 0)

            {

                var src = propertyVal != null ? DeserializeObject<ImageGroupAppearanceInfo>(propertyVal) : new ImageGroupAppearanceInfo();

                _appearanceInfo ??= new ImageGroupAppearanceInfo();

                _appearanceInfo.CopyFrom(src);

                RaisePropertyChanged(nameof(AppearanceInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(CommonInfo)) == 0)

            {

                var src = propertyVal != null ? DeserializeObject<ImageGroupCommonInfo>(propertyVal) : new ImageGroupCommonInfo();

                _commonInfo ??= new ImageGroupCommonInfo();

                _commonInfo.CopyFrom(src);

                RaisePropertyChanged(nameof(CommonInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(LayoutInfo)) == 0)

            {

                var src = propertyVal != null ? DeserializeObject<ImageGroupLayoutInfo>(propertyVal) : new ImageGroupLayoutInfo();

                _layoutInfo ??= new ImageGroupLayoutInfo();

                _layoutInfo.CopyFrom(src);

                RaisePropertyChanged(nameof(LayoutInfo));

                return true;

            }

            if (string.Compare(propertyID, nameof(CurrentIndex)) == 0)

            {

                _currentIndex = (int)(propertyVal?.ToInteger() ?? 0);

                RaisePropertyChanged(nameof(CurrentIndex));

                return true;

            }

            return base.SetPropertyValue(propertyID, propertyVal);

        }



        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()

        {

            if (val == null) return default;

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



        public void CopyFrom(ImageGroupPropertyModelEdit source)

        {

            if (source == null) return;

            base.CopyFrom(source);

            _appearanceInfo ??= new ImageGroupAppearanceInfo();

            _commonInfo ??= new ImageGroupCommonInfo();

            _layoutInfo ??= new ImageGroupLayoutInfo();

            if (source.AppearanceInfo != null) _appearanceInfo.CopyFrom(source.AppearanceInfo);

            if (source.CommonInfo != null) _commonInfo.CopyFrom(source.CommonInfo);

            if (source.LayoutInfo != null) _layoutInfo.CopyFrom(source.LayoutInfo);

            _currentIndex = source.CurrentIndex;

            RaisePropertyChanged(nameof(AppearanceInfo));

            RaisePropertyChanged(nameof(CommonInfo));

            RaisePropertyChanged(nameof(LayoutInfo));

            RaisePropertyChanged(nameof(CurrentIndex));

        }

    }



    /// <summary>

    /// 图片组图元属性绑定编辑模型

    /// </summary>

    [Serializable]

    [MapPropertyOrder]

    [CategoryPriority("绑定信息", 1)]

    public class ImageGroupPropertyBindEditModel : ControlCellPropertyBindEditModel

    {

        private PropertyBindInfo _currentIndex = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Integer);

        /// <summary>

        /// 当前索引绑定

        /// </summary>

        [DisplayName("当前索引")]

        [Category("绑定信息")]

        [PropertySortOrder(1)]

        [BindMPPropertyID]

        public PropertyBindInfo CurrentIndex

        {

            get => _currentIndex;

            set => SetProperty(ref _currentIndex, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.Integer));

        }



        public void CopyFrom(ImageGroupPropertyBindEditModel source)

        {

            base.CopyFrom(source);

            CurrentIndex = source.CurrentIndex;

        }

    }

}







