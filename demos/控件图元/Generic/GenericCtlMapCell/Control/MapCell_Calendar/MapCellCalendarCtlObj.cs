using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using Avalonia.Threading;
using GF_Gereric;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar.MapOprtCellParamCfgView;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Objects;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar.ViewModels;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Views;
using Griffins;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar
{
    internal partial class MapCellCalendarCtlObj : ControlCellBase
    {
        private static readonly string[] DefaultEventIds =
        {
            CalendarEventConst.Event_SelectedDatesChanged,
            CalendarEventConst.Event_DisplayModeChanged
        };

        private readonly Dictionary<string, MapOprtCellInstInfoList> _oprtInstsById = new(StringComparer.Ordinal);
        private EventBindEditModel? _eventBindEditModel;
        private CalendarView view;
        private CalendarViewModel viewModel;
        private bool _loadedPropertyEditFromBytes;

        static MapCellCalendarCtlObj()
        {
        }

        public MapCellCalendarCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        public MapCellCalendarCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            SetID(mapCellID);
            SetName(mapCellName);

            view = new CalendarView();

            RegisterProperty(new MapObjPropertyInfo(nameof(CalendarPropertyModelEdit.BrushInfo), "画笔设置", MapCellPropDataType.Object_Json, CalendarBrushInfo.Object_ID, typeof(CalendarBrushInfo), false, true, new MapCellPropValue(CalendarBrushInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(CalendarPropertyModelEdit.AppearanceInfo), "外观设置", MapCellPropDataType.Object_Json, CalendarAppearanceInfo.Object_ID, typeof(CalendarAppearanceInfo), false, true, new MapCellPropValue(CalendarAppearanceInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(CalendarPropertyModelEdit.CommonInfo), "公共设置", MapCellPropDataType.Object_Json, CalendarCommonInfo.Object_ID, typeof(CalendarCommonInfo), false, true, new MapCellPropValue(CalendarCommonInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(CalendarPropertyModelEdit.LayoutInfo), "布局设置", MapCellPropDataType.Object_Json, CalendarLayoutInfo.Object_ID, typeof(CalendarLayoutInfo), false, true, new MapCellPropValue(CalendarLayoutInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(CalendarPropertyModelEdit.TextInfo), "文本设置", MapCellPropDataType.Object_Json, CalendarTextInfo.Object_ID, typeof(CalendarTextInfo), false, true, new MapCellPropValue(CalendarTextInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(CalendarPropertyModelEdit.MiscInfo), "杂项设置", MapCellPropDataType.Object_Json, CalendarMiscInfo.Object_ID, typeof(CalendarMiscInfo), false, true, new MapCellPropValue(CalendarMiscInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(CalendarMiscInfo.SelectedDate), "SelectedDate", MapCellPropDataType.String, Guid.Empty, typeof(string), false, true, new MapCellPropValue(string.Empty)));
            RegisterProperty(new MapObjPropertyInfo(nameof(CalendarMiscInfo.BlackoutDates), "BlackoutDates", MapCellPropDataType.String, Guid.Empty, typeof(string), false, true, new MapCellPropValue("[]")));

            RegisterOprtCellInfo(new MapOprtCellInfo(CalendarMapOprtCellConst.BrushInfo_MapOprtCellID, "画笔设置操作原子", typeof(BrushInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(CalendarMapOprtCellConst.AppearanceInfo_MapOprtCellID, "外观设置操作原子", typeof(AppearanceInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(CalendarMapOprtCellConst.CommonInfo_MapOprtCellID, "公共设置操作原子", typeof(CommonInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(CalendarMapOprtCellConst.LayoutInfo_MapOprtCellID, "布局设置操作原子", typeof(LayoutInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(CalendarMapOprtCellConst.TextInfo_MapOprtCellID, "文本设置操作原子", typeof(TextInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(CalendarMapOprtCellConst.MiscInfo_MapOprtCellID, "杂项设置操作原子", typeof(MiscInfoMapOprtCellParamCfgView)));

            RegisterOprt(nameof(CalendarPropertyModelEdit.BrushInfo), "设置画笔", CalendarMapOprtCellConst.BrushInfo_MapOprtCellID);
            RegisterOprt(nameof(CalendarPropertyModelEdit.AppearanceInfo), "设置外观", CalendarMapOprtCellConst.AppearanceInfo_MapOprtCellID);
            RegisterOprt(nameof(CalendarPropertyModelEdit.CommonInfo), "设置公共", CalendarMapOprtCellConst.CommonInfo_MapOprtCellID);
            RegisterOprt(nameof(CalendarPropertyModelEdit.LayoutInfo), "设置布局", CalendarMapOprtCellConst.LayoutInfo_MapOprtCellID);
            RegisterOprt(nameof(CalendarPropertyModelEdit.TextInfo), "设置文本", CalendarMapOprtCellConst.TextInfo_MapOprtCellID);
            RegisterOprt(nameof(CalendarPropertyModelEdit.MiscInfo), "设置杂项", CalendarMapOprtCellConst.MiscInfo_MapOprtCellID);

            (this as IMapCellTypeBase).Name = "日历";

            viewModel = new CalendarViewModel(designTime, CalendarPropertyModelEdit, SelectedDatesChangedExec, DisplayModeChangedExec);
            view.DataContext = viewModel;

            CalendarPropertyModelEdit.BrushInfo.PropertyChanged += OnBrushInfoPropertyChanged;
            CalendarPropertyModelEdit.AppearanceInfo.PropertyChanged += OnAppearanceInfoPropertyChanged;
            CalendarPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
            CalendarPropertyModelEdit.LayoutInfo.PropertyChanged += OnLayoutInfoPropertyChanged;
            CalendarPropertyModelEdit.TextInfo.PropertyChanged += OnTextInfoPropertyChanged;
            CalendarPropertyModelEdit.MiscInfo.PropertyChanged += OnMiscInfoPropertyChanged;
        }

        private void OnBrushInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(CalendarPropertyModelEdit.BrushInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnAppearanceInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(CalendarPropertyModelEdit.AppearanceInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(CalendarPropertyModelEdit.CommonInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnLayoutInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(CalendarPropertyModelEdit.LayoutInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnTextInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(CalendarPropertyModelEdit.TextInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnMiscInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(nameof(CalendarPropertyModelEdit.MiscInfo), "PropertyChanged", e?.PropertyName);
        }

        public override void OnDispose()
        {
            CalendarPropertyModelEdit.BrushInfo.PropertyChanged -= OnBrushInfoPropertyChanged;
            CalendarPropertyModelEdit.AppearanceInfo.PropertyChanged -= OnAppearanceInfoPropertyChanged;
            CalendarPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;
            CalendarPropertyModelEdit.LayoutInfo.PropertyChanged -= OnLayoutInfoPropertyChanged;
            CalendarPropertyModelEdit.TextInfo.PropertyChanged -= OnTextInfoPropertyChanged;
            CalendarPropertyModelEdit.MiscInfo.PropertyChanged -= OnMiscInfoPropertyChanged;

            view.DataContext = null;
            viewModel?.Dispose();
            viewModel = null;

            base.OnDispose();
        }

        [Browsable(false)]
        public CalendarPropertyModelEdit CalendarPropertyModelEdit => PropertyEditModelBase as CalendarPropertyModelEdit;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new CalendarPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new CalendarPropertyBindEditModel();

        public override EventBindEditModel CreateEventBindEditModel()
        {
            _eventBindEditModel ??= new EventBindEditModel() { EventCmdInfos = CreateDefaultEventCmdInfos() };
            EnsureEventBindEditModel(_eventBindEditModel);
            return _eventBindEditModel;
        }

        private static BindingList<EventCmdInfo> CreateDefaultEventCmdInfos()
        {
            BindingList<EventCmdInfo> eventCmdInfos = new();
            foreach (string eventId in DefaultEventIds)
                eventCmdInfos.Add(new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = eventId });
            return eventCmdInfos;
        }

        private static void EnsureEventBindEditModel(EventBindEditModel? eventBindEditModel)
        {
            if (eventBindEditModel == null)
                return;

            var existingInfos = eventBindEditModel.EventCmdInfos?.Where(info => info != null).ToList() ?? new List<EventCmdInfo>();
            BindingList<EventCmdInfo> normalizedInfos = new();
            foreach (string eventId in DefaultEventIds)
            {
                EventCmdInfo? existingInfo = existingInfos.FirstOrDefault(info => string.Equals(info.EventID, eventId, StringComparison.Ordinal));
                normalizedInfos.Add(existingInfo ?? new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = eventId });
            }
            foreach (EventCmdInfo extraInfo in existingInfos.Where(info => !DefaultEventIds.Contains(info.EventID)))
                normalizedInfos.Add(extraInfo);
            eventBindEditModel.EventCmdInfos = normalizedInfos;
        }

        protected override bool SetPropertyValue(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            if (gFBaseTypePropValues == null)
                return false;

            foreach (GFBaseTypePropValue? gFBaseTypePropValue in gFBaseTypePropValues)
            {
                string propertyId = gFBaseTypePropValue?.PropertyID.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(propertyId))
                    SetPropertyValue(propertyId, gFBaseTypePropValue.Value, true);
            }

            return true;
        }

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal, bool isRuning)
        {
            CalendarPropertyModelEdit.IsRuning = isRuning;
            bool ok = CalendarPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
            if (ok)
                ExecuteOprtByPropertyId(propertyID, "SetPropertyValue", null);
            return ok;
        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            // 这里额外回调 ExecOprt，是为了让图元测试工具右侧能看到“执行操作”日志。
            if (TryGetPrimaryOprtCellId(propertyID, out string normalizedOprtId, out _))
            {
                try { CallBack?.ExecOprt(normalizedOprtId); } catch { }
            }

            if (CalendarPropertyModelEdit.IsRuning)
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

        private void ExecuteOprtByPropertyId(string propertyId, string trigger, string? changedProp)
        {
            if (!TryGetPrimaryOprtCellId(propertyId, out string normalizedOprtId, out MapOprtCellID primaryOprtCellId))
                return;
            TryExecuteOprtInfoById(normalizedOprtId, primaryOprtCellId);
        }

        private static bool TryGetPrimaryOprtCellId(string propertyId, out string normalizedOprtId, out MapOprtCellID oprtCellId)
        {
            if (string.IsNullOrWhiteSpace(propertyId))
            {
                normalizedOprtId = propertyId ?? string.Empty;
                oprtCellId = default;
                return false;
            }

            int dot = propertyId.IndexOf('.');
            if (dot > 0)
                propertyId = propertyId.Substring(0, dot);

            normalizedOprtId = propertyId;
            oprtCellId = default;
            // SelectedDate / BlackoutDates 是叶子绑定字段，但实际仍复用 MiscInfo 这组原子。
            if (string.Equals(propertyId, nameof(CalendarMiscInfo.SelectedDate), StringComparison.Ordinal) || string.Equals(propertyId, nameof(CalendarMiscInfo.BlackoutDates), StringComparison.Ordinal) || string.Equals(propertyId, nameof(CalendarPropertyModelEdit.MiscInfo), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(CalendarPropertyModelEdit.MiscInfo);
                oprtCellId = CalendarMapOprtCellConst.MiscInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(propertyId, nameof(CalendarPropertyModelEdit.BrushInfo), StringComparison.Ordinal))
            {
                oprtCellId = CalendarMapOprtCellConst.BrushInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(propertyId, nameof(CalendarPropertyModelEdit.AppearanceInfo), StringComparison.Ordinal))
            {
                oprtCellId = CalendarMapOprtCellConst.AppearanceInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(propertyId, nameof(CalendarPropertyModelEdit.CommonInfo), StringComparison.Ordinal))
            {
                oprtCellId = CalendarMapOprtCellConst.CommonInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(propertyId, nameof(CalendarPropertyModelEdit.LayoutInfo), StringComparison.Ordinal))
            {
                oprtCellId = CalendarMapOprtCellConst.LayoutInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(propertyId, nameof(CalendarPropertyModelEdit.TextInfo), StringComparison.Ordinal))
            {
                oprtCellId = CalendarMapOprtCellConst.TextInfo_MapOprtCellID;
                return true;
            }
            return false;
        }

        private bool TryExecuteOprtInfoById(string oprtId, MapOprtCellID primaryOprtCellId)
        {
            if (!_oprtInstsById.TryGetValue(oprtId, out MapOprtCellInstInfoList? instList) || instList == null)
                return false;

            foreach (MapOprtCellInstInfo inst in instList.Cast<MapOprtCellInstInfo>())
            {
                if (Dispatcher.UIThread.CheckAccess())
                    ExecOprtCell(inst);
                else
                    Dispatcher.UIThread.Post(() => ExecOprtCell(inst));
            }
            return true;
        }

        private void RegisterOprt(string id, string name, MapOprtCellID oprtCellId)
        {
            MapOprtCellInstInfoList instList = new()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = oprtCellId, CfgInfo = null }
            };
            RegisterOprtInfo(new MapOprtInfo(id, name, OprtExecKind.Normal, "", instList));
            _oprtInstsById[id] = instList;
        }

        private void ExecEvent(string eventId)
        {
            EventCmdInfo? eventCmdInfo = EventBindEditModel?.EventCmdInfos?.FirstOrDefault(info => info.EventID == eventId);
            if (eventCmdInfo != null)
                CallBack?.ExecMapCellEvent(eventCmdInfo.EventCmdKind, eventCmdInfo.CmdID, CommHelper.ToEventParamValueList(eventCmdInfo.CmdParam), out _);
        }

        private void SelectedDatesChangedExec() => ExecEvent(CalendarEventConst.Event_SelectedDatesChanged);

        private void DisplayModeChangedExec(CalendarDisplayModeType oldMode, CalendarDisplayModeType newMode) => ExecEvent(CalendarEventConst.Event_DisplayModeChanged);

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);

            // 先恢复属性模型，再恢复绑定和事件，确保 ViewModel 首次刷新时能拿到完整状态。
            string propertyEditJson = br.ReadString("PropertyEditModelBase");
            if (!string.IsNullOrEmpty(propertyEditJson))
            {
                try
                {
                    var propertyEditModelBase = JsonObjConvert.FromJSon<CalendarPropertyModelEdit>(propertyEditJson);
                    if (propertyEditModelBase != null)
                    {
                        (PropertyEditModelBase as CalendarPropertyModelEdit)?.CopyFrom(propertyEditModelBase);
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
                    var propertyBindEditModelBase = JsonObjConvert.FromJSon<CalendarPropertyBindEditModel>(propertyBindJson);
                    if (propertyBindEditModelBase != null)
                        (PropertyBindEditModelBase as CalendarPropertyBindEditModel)?.CopyFrom(propertyBindEditModelBase);
                }
                catch { }
            }

            string eventBindJson = br.ReadString("EventBindEditModel");
            if (!string.IsNullOrEmpty(eventBindJson))
            {
                try
                {
                    EventBindEditModel = JsonSerializer.Deserialize<EventBindEditModel>(eventBindJson) ?? CreateEventBindEditModel();
                    EnsureEventBindEditModel(EventBindEditModel);
                }
                catch { }
            }
        }

        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", JsonSerializer.Serialize(EventBindEditModel));
        }

        protected override void OnCopyFrom(ControlCellBase source)
        {
            MapCellCalendarCtlObj? sourceObj = source as MapCellCalendarCtlObj;
            base._CopyFrom(sourceObj);
            (PropertyEditModelBase as CalendarPropertyModelEdit)?.CopyFrom(source.PropertyEditModelBase as CalendarPropertyModelEdit);
            if (PropertyBindEditModelBase is CalendarPropertyBindEditModel selfBind && source.PropertyBindEditModelBase is CalendarPropertyBindEditModel srcBind)
                selfBind.CopyFrom(srcBind);
            else
                PropertyBindEditModelBase.CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel.CopyFrom(source.EventBindEditModel);
            _loadedPropertyEditFromBytes = true;
            Dispatcher.UIThread.Post(() => viewModel?.ReloadFromModel());
        }

        protected override void OnInit()
        {
            base.OnInit();
            Dispatcher.UIThread.Post(() =>
            {
                try { viewModel?.ReloadFromModel(); } catch { }
            });
        }

        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => viewModel;

        public override void OnZoomChanged()
        {
        }

        public override string ToString() => "日历";
        public override MapCellPropValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null;
        }
        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == CalendarMapOprtCellConst.BrushInfo_MapOprtCellID)
                return ExecuteOprtCell<BrushInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == CalendarMapOprtCellConst.AppearanceInfo_MapOprtCellID)
                return ExecuteOprtCell<AppearanceInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == CalendarMapOprtCellConst.CommonInfo_MapOprtCellID)
                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == CalendarMapOprtCellConst.LayoutInfo_MapOprtCellID)
                return ExecuteOprtCell<LayoutInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == CalendarMapOprtCellConst.TextInfo_MapOprtCellID)
                return ExecuteOprtCell<TextInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == CalendarMapOprtCellConst.MiscInfo_MapOprtCellID)
                return ExecuteOprtCell<MiscInfoMapOprtCellExector>(mapOprtCellInstInfo);
            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        private bool ExecuteOprtCell<T>(MapOprtCellInstInfo mapOprtCellInstInfo) where T : IMapOprtCellExector, new()
        {
            if (!MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector? mapOprtCellExector))
            {
                mapOprtCellExector = new T();
                mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
            }
            mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
            return true;
        }

        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            ObjectValue_Json objectValueJson = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValueJson);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }

        private static void PostToUI(Action action)
        {
            if (Dispatcher.UIThread.CheckAccess())
                action();
            else
                Dispatcher.UIThread.Post(action);
        }
    }
}
