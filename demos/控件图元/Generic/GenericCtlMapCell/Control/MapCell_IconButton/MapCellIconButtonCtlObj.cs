using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

using GF_Gereric;
using Griffins;
using Griffins.PF;
using GKG.Map.MapCell.Generic;
using GKG.Map.MapCell.Generic.IconButton;
using GKG.Map.MapCell.Generic.IconButton.MapOprtCellParamCfgView;
using GKG.Map.MapCell.Generic.Control.MapCell_TextBox;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using Griffins.PF.RichClient;

namespace GKG.Map.MapCell.Generic.IconButton
{
    class MapCellIconButtonCtlObj : ControlCellBase
    {
        private static readonly string[] DefaultEventIds =
        {
            MapObjPropEventConst.Event_MouseDown,
            MapObjPropEventConst.Event_MouseUp,
            MapObjPropEventConst.Event_MouseLeave,
            MapObjPropEventConst.Event_MouseClick,
            MapObjPropEventConst.Event_MouseDoubleClick,
            IconButtonEventConst.Event_MouseRightClick
        };

        #region 私有字段

        private IconButtonView view;
        private IconButtonViewModel iconButtonViewModel;
        private EventBindEditModel _eventBindEditModel;
        private MapObjID _mapCellID;
        private string _mapCellName;
        private bool _loadedPropertyEditFromBytes;
        private readonly bool _designTime;
        private readonly Delegate_MutualInfoProcess _mutualInfoProcessDelegate;
        private bool _mutualInfoRegistered;
        private bool _isRestoringFromSerializedState;
        #endregion

        #region 构造函数

        public MapCellIconButtonCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false) { }

        public MapCellIconButtonCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            _designTime = designTime;
            _mutualInfoProcessDelegate = OnMutualInfoReceived;
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            _mapCellID = mapCellID;
            _mapCellName = mapCellName;
            base.SetID(mapCellID);
            base.SetName(mapCellName);

            view = new IconButtonView();

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.BrushInfo), "画笔设置", MapCellPropDataType.Object_Json, IconButtonBrushInfo.Object_ID, typeof(IconButtonBrushInfo), false, true, new MapCellPropValue(IconButtonBrushInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.AppearanceInfo), "外观设置", MapCellPropDataType.Object_Json, IconButtonAppearanceInfo.Object_ID, typeof(IconButtonAppearanceInfo), false, true, new MapCellPropValue(IconButtonAppearanceInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.LayoutInfo), "布局设置", MapCellPropDataType.Object_Json, IconButtonLayoutInfo.Object_ID, typeof(IconButtonLayoutInfo), false, true, new MapCellPropValue(IconButtonLayoutInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.CommonInfo), "公共设置", MapCellPropDataType.Object_Json, IconButtonCommonInfo.Object_ID, typeof(IconButtonCommonInfo), false, true, new MapCellPropValue(IconButtonCommonInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.FontInfo), "字体设置", MapCellPropDataType.Object_Json, IconButtonFontInfo.Object_ID, typeof(IconButtonFontInfo), false, true, new MapCellPropValue(IconButtonFontInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.ParagraphInfo), "段落设置", MapCellPropDataType.Object_Json, IconButtonParagraphInfo.Object_ID, typeof(IconButtonParagraphInfo), false, true, new MapCellPropValue(IconButtonParagraphInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.MiscInfo), "杂项设置", MapCellPropDataType.Object_Json, IconButtonMiscInfo.Object_ID, typeof(IconButtonMiscInfo), false, true, new MapCellPropValue(IconButtonMiscInfo.Default)));
            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseDown, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseDown), MapCellPropDataType.Object_Bytes, GraphMouseEventParam.Object_ID));
            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseUp, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseUp), MapCellPropDataType.Object_Bytes, GraphMouseEventParam.Object_ID));
            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseLeave, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseLeave), MapCellPropDataType.Object_Bytes, GraphMouseEventParam.Object_ID));
            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), MapCellPropDataType.Object_Bytes, GraphMouseEventParam.Object_ID));
            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseDoubleClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseDoubleClick), MapCellPropDataType.Object_Bytes, GraphMouseEventParam.Object_ID));
            RegisterEvent(new MapObjEventInfo(IconButtonEventConst.Event_MouseRightClick, IconButtonEventConst.Event_MouseRightClickName, MapCellPropDataType.Object_Bytes, GraphMouseEventParam.Object_ID));

            RegisterOprtCellInfo(new MapOprtCellInfo(IconButtonMapOprtCellConst.BrushInfo_MapOprtCellID, "画笔设置操作原子", typeof(BrushInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(IconButtonMapOprtCellConst.AppearanceInfo_MapOprtCellID, "外观设置操作原子", typeof(AppearanceInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(IconButtonMapOprtCellConst.LayoutInfo_MapOprtCellID, "布局设置操作原子", typeof(LayoutInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(IconButtonMapOprtCellConst.CommonInfo_MapOprtCellID, "公共设置操作原子", typeof(CommonInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(IconButtonMapOprtCellConst.FontInfo_MapOprtCellID, "字体设置操作原子", typeof(FontInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(IconButtonMapOprtCellConst.ParagraphInfo_MapOprtCellID, "段落设置操作原子", typeof(ParagraphInfoMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(IconButtonMapOprtCellConst.MiscInfo_MapOprtCellID, "杂项设置操作原子", typeof(MiscInfoMapOprtCellParamCfgView)));

            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.BrushInfo), "设置画笔", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = IconButtonMapOprtCellConst.BrushInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.AppearanceInfo), "设置外观", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = IconButtonMapOprtCellConst.AppearanceInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.LayoutInfo), "设置布局", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = IconButtonMapOprtCellConst.LayoutInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.CommonInfo), "设置公共", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = IconButtonMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.FontInfo), "设置字体", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = IconButtonMapOprtCellConst.FontInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.ParagraphInfo), "设置段落", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = IconButtonMapOprtCellConst.ParagraphInfo_MapOprtCellID, CfgInfo = null } }));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.MiscInfo), "设置杂项", OprtExecKind.Normal, "", new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = IconButtonMapOprtCellConst.MiscInfo_MapOprtCellID, CfgInfo = null } }));

            (this as IMapCellTypeBase).Name = "图标按钮";

            iconButtonViewModel = new IconButtonViewModel(
                IconButtonPropertyModelEdit,
                OnButtonClicked,
                _ => ExecEvent(MapObjPropEventConst.Event_MouseDown),
                _ => ExecEvent(MapObjPropEventConst.Event_MouseUp),
                _ => ExecEvent(MapObjPropEventConst.Event_MouseLeave),
                _ => ExecEvent(MapObjPropEventConst.Event_MouseDoubleClick),
                _ => ExecEvent(IconButtonEventConst.Event_MouseRightClick));
            view.DataContext = iconButtonViewModel;

            IconButtonPropertyModelEdit.BrushInfo.PropertyChanged += OnBrushInfoPropertyChanged;
            IconButtonPropertyModelEdit.AppearanceInfo.PropertyChanged += OnAppearanceInfoPropertyChanged;
            IconButtonPropertyModelEdit.LayoutInfo.PropertyChanged += OnLayoutInfoPropertyChanged;
            IconButtonPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
            IconButtonPropertyModelEdit.FontInfo.PropertyChanged += OnFontInfoPropertyChanged;
            IconButtonPropertyModelEdit.ParagraphInfo.PropertyChanged += OnParagraphInfoPropertyChanged;
            IconButtonPropertyModelEdit.MiscInfo.PropertyChanged += OnMiscInfoPropertyChanged;
            EnsureMutualInfoRegistered();
        }

        #endregion

        #region 属性

        public IconButtonPropertyModelEdit IconButtonPropertyModelEdit => PropertyEditModelBase as IconButtonPropertyModelEdit;

        #endregion

        #region 公共方法
        public override PropertyEditModelBase CreatePropertyModelEditBase()
        {
            return new IconButtonPropertyModelEdit();
        }

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
        {
            return new IconButtonPropertyBindEditModel();
        }

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
            EventCmdInfo? eventCmdInfo = EventBindEditModel.EventCmdInfos.FirstOrDefault(info => info.EventID == eventId);
            if (eventCmdInfo != null)
            {
                CallBack?.ExecMapCellEvent(eventCmdInfo.EventCmdKind, eventCmdInfo.CmdID, CommHelper.ToEventParamValueList(eventCmdInfo.CmdParam), out _);
            }
        }

        /// <summary>
        /// 点击按钮时只负责向平台广播“当前组选中项”，不再本地遍历兄弟按钮。
        /// </summary>
        private void OnButtonClicked(Point screenPoint)
        {
            ExecEvent(MapObjPropEventConst.Event_MouseClick);
            BroadcastGroupSelection();
        }

        private void OnBrushInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.BrushInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnAppearanceInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.AppearanceInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnLayoutInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.LayoutInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            if (string.Equals(e?.PropertyName, nameof(IconButtonCommonInfo.GroupId), StringComparison.Ordinal))
            {
                view.ApplyMutualSelectionState(false);
            }

            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.CommonInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnFontInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.FontInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnParagraphInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.ParagraphInfo), "PropertyChanged", e?.PropertyName);
        }

        private void OnMiscInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isRestoringFromSerializedState)
                return;

            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.MiscInfo), "PropertyChanged", e?.PropertyName);
        }

        public override void OnDispose()
        {
            IconButtonPropertyModelEdit.BrushInfo.PropertyChanged -= OnBrushInfoPropertyChanged;
            IconButtonPropertyModelEdit.AppearanceInfo.PropertyChanged -= OnAppearanceInfoPropertyChanged;
            IconButtonPropertyModelEdit.LayoutInfo.PropertyChanged -= OnLayoutInfoPropertyChanged;
            IconButtonPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;
            IconButtonPropertyModelEdit.FontInfo.PropertyChanged -= OnFontInfoPropertyChanged;
            IconButtonPropertyModelEdit.ParagraphInfo.PropertyChanged -= OnParagraphInfoPropertyChanged;
            IconButtonPropertyModelEdit.MiscInfo.PropertyChanged -= OnMiscInfoPropertyChanged;

            view.DataContext = null;

            iconButtonViewModel?.Dispose();
            iconButtonViewModel = null;

            ReleaseMutualInfoRegistration();

            base.OnDispose();
        }

        private void EnsureMutualInfoRegistered()
        {
            if (_mutualInfoRegistered)
            {
                return;
            }

            ClientInfoProcessRegister.RegisterMutualInfoProcessDelegate(MutualInfo_IconButtonGroupSelected.InfoKindID, _mutualInfoProcessDelegate);
            _mutualInfoRegistered = true;
        }

        private void ReleaseMutualInfoRegistration()
        {
            if (!_mutualInfoRegistered)
            {
                return;
            }

           
            ClientInfoProcessRegister.UnRegisterMutualInfoProcessDelegate(MutualInfo_IconButtonGroupSelected.InfoKindID, _mutualInfoProcessDelegate);
            
           

            _mutualInfoRegistered = false;
        }

        /// <summary>
        /// 平台广播的互斥消息只传组ID和图元ID，所有按钮都据此自己判断是否进入组选中态。
        /// </summary>
        private void BroadcastGroupSelection()
        {
            var groupId = IconButtonPropertyModelEdit?.CommonInfo?.GroupId?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(groupId))
            {
                return;
            }

           
           ClientInnerInfoSender.SendMutualInfo(
           MutualInfo_IconButtonGroupSelected.InfoKindID,
           new MutualInfo_IconButtonGroupSelected(groupId, _mapCellID.ToString()));
            
           
        }

        /// <summary>
        /// 收到平台互斥消息后，直接按 GroupId 和 MapObjID 切当前 View 的选中外观，不再经过 ViewModel 中转。
        /// </summary>
        private MutualInfoResponseBase OnMutualInfoReceived(GriffinsInfoKindID infoKind, MutualInfoBase info, System.Threading.CancellationToken token)
        {
            if (token.IsCancellationRequested || info is not MutualInfo_IconButtonGroupSelected mutualInfo)
            {
                return null;
            }

            var selfGroupId = IconButtonPropertyModelEdit?.CommonInfo?.GroupId?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(selfGroupId) || !string.Equals(selfGroupId, mutualInfo.GroupId?.Trim() ?? string.Empty, StringComparison.Ordinal))
            {
                return null;
            }

            var isSelected = string.Equals(mutualInfo.MapObjID ?? string.Empty, _mapCellID.ToString(), StringComparison.Ordinal);
            Dispatcher.UIThread.Post(() => view.ApplyMutualSelectionState(isSelected));
            return null;
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
            if (_loadedPropertyEditFromBytes && !isRuning && IsDefaultOverwriteForLoadedIconButton(propertyID, propertyVal))
            {
                return true;
            }
            IconButtonPropertyModelEdit.IsRuning = isRuning;
            var ok = IconButtonPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
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

            if (IconButtonPropertyModelEdit.IsRuning)
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

        private bool IsDefaultOverwriteForLoadedIconButton(string propertyID, MapCellPropValue propertyVal)
        {
            try
            {
                if (string.Compare(propertyID, nameof(IconButtonPropertyModelEdit.BrushInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<IconButtonBrushInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(IconButtonPropertyModelEdit.BrushInfo);
                }
                if (string.Compare(propertyID, nameof(IconButtonPropertyModelEdit.AppearanceInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<IconButtonAppearanceInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(IconButtonPropertyModelEdit.AppearanceInfo);
                }
                if (string.Compare(propertyID, nameof(IconButtonPropertyModelEdit.LayoutInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<IconButtonLayoutInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(IconButtonPropertyModelEdit.LayoutInfo);
                }
                if (string.Compare(propertyID, nameof(IconButtonPropertyModelEdit.CommonInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<IconButtonCommonInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(IconButtonPropertyModelEdit.CommonInfo);
                }
                if (string.Compare(propertyID, nameof(IconButtonPropertyModelEdit.FontInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<IconButtonFontInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(IconButtonPropertyModelEdit.FontInfo);
                }
                if (string.Compare(propertyID, nameof(IconButtonPropertyModelEdit.ParagraphInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<IconButtonParagraphInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(IconButtonPropertyModelEdit.ParagraphInfo);
                }
                if (string.Compare(propertyID, nameof(IconButtonPropertyModelEdit.MiscInfo)) == 0)
                {
                    var incoming = propertyVal != null ? DeserializeObject<IconButtonMiscInfo>(propertyVal) : null;
                    return IsDefault(incoming) && !IsDefault(IconButtonPropertyModelEdit.MiscInfo);
                }
                return false;
            }
            catch { return false; }
        }

        private static bool IsDefault(IconButtonBrushInfo? info)
        {
            if (info == null) return true;
            return info.BackgroundColor == IconButtonBrushInfo.Default.BackgroundColor
                && info.BorderColor == IconButtonBrushInfo.Default.BorderColor
                && info.ForegroundColor == IconButtonBrushInfo.Default.ForegroundColor
                && info.HoverBackgroundColor == IconButtonBrushInfo.Default.HoverBackgroundColor
                && info.HoverForegroundColor == IconButtonBrushInfo.Default.HoverForegroundColor
                && info.ClickBackgroundColor == IconButtonBrushInfo.Default.ClickBackgroundColor
                && info.ClickForegroundColor == IconButtonBrushInfo.Default.ClickForegroundColor
                && info.ClickBorderColorLeft == IconButtonBrushInfo.Default.ClickBorderColorLeft
                && info.ClickBorderColorTop == IconButtonBrushInfo.Default.ClickBorderColorTop
                && info.ClickBorderColorRight == IconButtonBrushInfo.Default.ClickBorderColorRight
                && info.ClickBorderColorBottom == IconButtonBrushInfo.Default.ClickBorderColorBottom;
        }

        private static bool IsDefault(IconButtonAppearanceInfo? info)
        {
            if (info == null) return true;
            return info.Opacity == IconButtonAppearanceInfo.Default.Opacity
                && info.BorderThicknessLeft == IconButtonAppearanceInfo.Default.BorderThicknessLeft
                && info.BorderThicknessTop == IconButtonAppearanceInfo.Default.BorderThicknessTop
                && info.BorderThicknessRight == IconButtonAppearanceInfo.Default.BorderThicknessRight
                && info.BorderThicknessBottom == IconButtonAppearanceInfo.Default.BorderThicknessBottom
                && info.ClickBorderThicknessLeft == IconButtonAppearanceInfo.Default.ClickBorderThicknessLeft
                && info.ClickBorderThicknessTop == IconButtonAppearanceInfo.Default.ClickBorderThicknessTop
                && info.ClickBorderThicknessRight == IconButtonAppearanceInfo.Default.ClickBorderThicknessRight
                && info.ClickBorderThicknessBottom == IconButtonAppearanceInfo.Default.ClickBorderThicknessBottom;
        }

        private static bool IsDefault(IconButtonLayoutInfo? info)
        {
            if (info == null) return true;
            return info.HorizontalAlignment == IconButtonLayoutInfo.Default.HorizontalAlignment
                && info.VerticalAlignment == IconButtonLayoutInfo.Default.VerticalAlignment
                && info.Margin == IconButtonLayoutInfo.Default.Margin
                && info.MarginLeft == IconButtonLayoutInfo.Default.MarginLeft
                && info.MarginTop == IconButtonLayoutInfo.Default.MarginTop
                && info.MarginRight == IconButtonLayoutInfo.Default.MarginRight
                && info.MarginBottom == IconButtonLayoutInfo.Default.MarginBottom;
        }

        private static bool IsDefault(IconButtonCommonInfo? info)
        {
            if (info == null) return true;
            return string.Equals(info.ButtonText ?? string.Empty, IconButtonCommonInfo.Default.ButtonText ?? string.Empty, StringComparison.Ordinal)
                && info.HoverCursor == IconButtonCommonInfo.Default.HoverCursor
                && info.Enabled == IconButtonCommonInfo.Default.Enabled
                && string.Equals(info.TooltipText ?? string.Empty, IconButtonCommonInfo.Default.TooltipText ?? string.Empty, StringComparison.Ordinal)
                && string.Equals(info.GroupId ?? string.Empty, IconButtonCommonInfo.Default.GroupId ?? string.Empty, StringComparison.Ordinal);
        }

        private static bool IsDefault(IconButtonFontInfo? info)
        {
            if (info == null) return true;
            return info.FontColor == IconButtonFontInfo.Default.FontColor
                && info.FontSize == IconButtonFontInfo.Default.FontSize
                && info.IsBold == IconButtonFontInfo.Default.IsBold
                && info.IsItalic == IconButtonFontInfo.Default.IsItalic
                && info.IsUnderline == IconButtonFontInfo.Default.IsUnderline;
        }

        private static bool IsDefault(IconButtonParagraphInfo? info)
        {
            if (info == null) return true;
            return info.LineHeight == IconButtonParagraphInfo.Default.LineHeight
                && info.ParagraphSpacingBefore == IconButtonParagraphInfo.Default.ParagraphSpacingBefore
                && info.ParagraphSpacingAfter == IconButtonParagraphInfo.Default.ParagraphSpacingAfter
                && info.TextAlignment == IconButtonParagraphInfo.Default.TextAlignment;
        }

        private static bool IsDefault(IconButtonMiscInfo? info)
        {
            if (info == null) return true;
            return string.IsNullOrWhiteSpace(info.IconSourceBase64)
                && info.IconWidth == IconButtonMiscInfo.Default.IconWidth
                && info.IconHeight == IconButtonMiscInfo.Default.IconHeight
                && info.IconMargin == IconButtonMiscInfo.Default.IconMargin
                && info.IconMarginLeft == IconButtonMiscInfo.Default.IconMarginLeft
                && info.IconMarginTop == IconButtonMiscInfo.Default.IconMarginTop
                && info.IconMarginRight == IconButtonMiscInfo.Default.IconMarginRight
                && info.IconMarginBottom == IconButtonMiscInfo.Default.IconMarginBottom
                && info.IconPosition == IconButtonMiscInfo.Default.IconPosition;
        }

        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => iconButtonViewModel;

        protected override void OnCopyFrom(ControlCellBase source)
        {
            var obj = source as MapCellIconButtonCtlObj;
            base._CopyFrom(obj);
            // 复制现有图元时先暂停属性变更触发的逐项回放，避免首帧先出现半成品布局。
            _isRestoringFromSerializedState = true;
            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
            _isRestoringFromSerializedState = false;
            if (PropertyBindEditModelBase is IconButtonPropertyBindEditModel selfBind && source.PropertyBindEditModelBase is IconButtonPropertyBindEditModel srcBind)
                selfBind.CopyFrom(srcBind);
            else
                (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel.CopyFrom(source.EventBindEditModel);
            EnsureEventBindEditModel(EventBindEditModel);
            _loadedPropertyEditFromBytes = true;
            ReloadIconButtonViewSynchronously();
        }

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            string propertyEditJson = br.ReadString("PropertyEditModelBase");
            if (!string.IsNullOrEmpty(propertyEditJson))
            {
                // 读档期间先压住 PropertyChanged -> ExecuteOprtByPropertyId 这条异步回放链，最后再统一同步回放一次。
                _isRestoringFromSerializedState = true;
                try
                {
                    var propertyEditModelBase = JsonObjConvert.FromJSon<IconButtonPropertyModelEdit>(propertyEditJson);
                    if (propertyEditModelBase != null)
                    {
                        (PropertyEditModelBase as IconButtonPropertyModelEdit).CopyFrom(propertyEditModelBase);
                        _loadedPropertyEditFromBytes = true;
                    }
                    else
                    {
                        var fallback = TryParseIconButtonPropertyModelEdit(propertyEditJson);
                        if (fallback != null)
                        {
                            (PropertyEditModelBase as IconButtonPropertyModelEdit).CopyFrom(fallback);
                            _loadedPropertyEditFromBytes = true;
                        }
                    }
                }
                catch
                {
                    var fallback = TryParseIconButtonPropertyModelEdit(propertyEditJson);
                    if (fallback != null)
                    {
                        (PropertyEditModelBase as IconButtonPropertyModelEdit).CopyFrom(fallback);
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
                    var parsed = JsonObjConvert.FromJSon<IconButtonPropertyBindEditModel>(propertyBindJson);
                    if (parsed != null)
                        (PropertyBindEditModelBase as IconButtonPropertyBindEditModel)?.CopyFrom(parsed);
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
                    {
                        EventBindEditModel.CopyFrom(eventBindEditModel);
                        EnsureEventBindEditModel(EventBindEditModel);
                    }
                }
                catch { }
            }

            if (_loadedPropertyEditFromBytes)
                ReloadIconButtonViewSynchronously();
        }

        /// <summary>
        /// 图标按钮读档完成后，统一同步刷新 ViewModel 和首帧布局，避免文字和图标各自分多拍收敛。
        /// </summary>
        private void ReloadIconButtonViewSynchronously()
        {
            ExecuteOnUiThreadSynchronously(() =>
            {
                iconButtonViewModel?.ReloadFromModel();
                view?.ApplyInitialStateFromViewModel();
            });
        }

        /// <summary>
        /// 运行时切页时需要在真正显示前完成首帧状态同步，这里统一用同步方式把 UI 刷新做完。
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

        private static IconButtonPropertyModelEdit? TryParseIconButtonPropertyModelEdit(string json)
        {
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;
                var model = new IconButtonPropertyModelEdit();

                static string? GetString(JsonElement e, string name)
                {
                    if (e.TryGetProperty(name, out var v) && v.ValueKind == System.Text.Json.JsonValueKind.String)
                        return v.GetString();
                    return null;
                }

                static int? GetInt(JsonElement e, string name)
                {
                    if (e.TryGetProperty(name, out var v))
                    {
                        if (v.ValueKind == System.Text.Json.JsonValueKind.Number && v.TryGetInt32(out var i)) return i;
                        if (v.ValueKind == System.Text.Json.JsonValueKind.String && int.TryParse(v.GetString(), out var s)) return s;
                    }
                    return null;
                }

                static bool? GetBool(JsonElement e, string name)
                {
                    if (e.TryGetProperty(name, out var v))
                    {
                        if (v.ValueKind == System.Text.Json.JsonValueKind.True || v.ValueKind == System.Text.Json.JsonValueKind.False) return v.GetBoolean();
                        if (v.ValueKind == System.Text.Json.JsonValueKind.String && bool.TryParse(v.GetString(), out var b)) return b;
                    }
                    return null;
                }

                if (root.TryGetProperty("BrushInfo", out var brush) && brush.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    var s = GetString(brush, "BackgroundColor") ?? GetString(brush, "BackColor") ?? GetString(brush, "BackgroundColorStr");
                    if (!string.IsNullOrWhiteSpace(s)) model.BrushInfo.BackgroundColor = Color.Parse(s);
                    s = GetString(brush, "BorderColor") ?? GetString(brush, "BorderColorStr");
                    if (!string.IsNullOrWhiteSpace(s)) model.BrushInfo.BorderColor = Color.Parse(s);
                    s = GetString(brush, "ForegroundColor") ?? GetString(brush, "ForeColor") ?? GetString(brush, "ForegroundColorStr");
                    if (!string.IsNullOrWhiteSpace(s)) model.BrushInfo.ForegroundColor = Color.Parse(s);
                    s = GetString(brush, "HoverBackgroundColor");
                    if (!string.IsNullOrWhiteSpace(s)) model.BrushInfo.HoverBackgroundColor = Color.Parse(s);
                    s = GetString(brush, "HoverForegroundColor");
                    if (!string.IsNullOrWhiteSpace(s)) model.BrushInfo.HoverForegroundColor = Color.Parse(s);
                    s = GetString(brush, "ClickBackgroundColor");
                    if (!string.IsNullOrWhiteSpace(s)) model.BrushInfo.ClickBackgroundColor = Color.Parse(s);
                    s = GetString(brush, "ClickForegroundColor");
                    if (!string.IsNullOrWhiteSpace(s)) model.BrushInfo.ClickForegroundColor = Color.Parse(s);
                    // 兼容旧数据时，如果新字段不存在，就回退到普通边框颜色，避免历史页面出现突变。
                    s = GetString(brush, "ClickBorderColorLeft");
                    model.BrushInfo.ClickBorderColorLeft = !string.IsNullOrWhiteSpace(s) ? Color.Parse(s) : model.BrushInfo.BorderColor;
                    s = GetString(brush, "ClickBorderColorTop");
                    model.BrushInfo.ClickBorderColorTop = !string.IsNullOrWhiteSpace(s) ? Color.Parse(s) : model.BrushInfo.BorderColor;
                    s = GetString(brush, "ClickBorderColorRight");
                    model.BrushInfo.ClickBorderColorRight = !string.IsNullOrWhiteSpace(s) ? Color.Parse(s) : model.BrushInfo.BorderColor;
                    s = GetString(brush, "ClickBorderColorBottom");
                    model.BrushInfo.ClickBorderColorBottom = !string.IsNullOrWhiteSpace(s) ? Color.Parse(s) : model.BrushInfo.BorderColor;
                }

                if (root.TryGetProperty("AppearanceInfo", out var app) && app.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    var i = GetInt(app, "Opacity");
                    if (i.HasValue) model.AppearanceInfo.Opacity = i.Value;
                    i = GetInt(app, "BorderThickness");
                    if (i.HasValue) model.AppearanceInfo.BorderThickness = i.Value;
                    i = GetInt(app, "BorderThicknessLeft");
                    if (i.HasValue) model.AppearanceInfo.BorderThicknessLeft = i.Value;
                    i = GetInt(app, "BorderThicknessTop");
                    if (i.HasValue) model.AppearanceInfo.BorderThicknessTop = i.Value;
                    i = GetInt(app, "BorderThicknessRight");
                    if (i.HasValue) model.AppearanceInfo.BorderThicknessRight = i.Value;
                    i = GetInt(app, "BorderThicknessBottom");
                    if (i.HasValue) model.AppearanceInfo.BorderThicknessBottom = i.Value;
                    // 点击态边框粗细缺失时，默认跟随普通边框粗细，保证旧页面视觉保持原样。
                    i = GetInt(app, "ClickBorderThicknessLeft");
                    model.AppearanceInfo.ClickBorderThicknessLeft = i ?? model.AppearanceInfo.BorderThicknessLeft;
                    i = GetInt(app, "ClickBorderThicknessTop");
                    model.AppearanceInfo.ClickBorderThicknessTop = i ?? model.AppearanceInfo.BorderThicknessTop;
                    i = GetInt(app, "ClickBorderThicknessRight");
                    model.AppearanceInfo.ClickBorderThicknessRight = i ?? model.AppearanceInfo.BorderThicknessRight;
                    i = GetInt(app, "ClickBorderThicknessBottom");
                    model.AppearanceInfo.ClickBorderThicknessBottom = i ?? model.AppearanceInfo.BorderThicknessBottom;
                }

                if (root.TryGetProperty("LayoutInfo", out var layout) && layout.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    var i = GetInt(layout, "Width");
                    if (i.HasValue)
                    {
                        model.Width = i.Value;
                    }
                    i = GetInt(layout, "Height");
                    if (i.HasValue)
                    {
                        model.Height = i.Value;
                    }

                    if (layout.TryGetProperty("HorizontalAlignment", out var ha))
                    {
                        if (ha.ValueKind == System.Text.Json.JsonValueKind.String && Enum.TryParse<IconButtonLayoutInfo.HorizontalAlignmentEnum>(ha.GetString(), true, out var e))
                            model.LayoutInfo.HorizontalAlignment = e;
                        else if (ha.ValueKind == System.Text.Json.JsonValueKind.Number && ha.TryGetInt32(out var n) && Enum.IsDefined(typeof(IconButtonLayoutInfo.HorizontalAlignmentEnum), n))
                            model.LayoutInfo.HorizontalAlignment = (IconButtonLayoutInfo.HorizontalAlignmentEnum)n;
                    }

                    if (layout.TryGetProperty("VerticalAlignment", out var va))
                    {
                        if (va.ValueKind == System.Text.Json.JsonValueKind.String && Enum.TryParse<IconButtonLayoutInfo.VerticalAlignmentEnum>(va.GetString(), true, out var e))
                            model.LayoutInfo.VerticalAlignment = e;
                        else if (va.ValueKind == System.Text.Json.JsonValueKind.Number && va.TryGetInt32(out var n) && Enum.IsDefined(typeof(IconButtonLayoutInfo.VerticalAlignmentEnum), n))
                            model.LayoutInfo.VerticalAlignment = (IconButtonLayoutInfo.VerticalAlignmentEnum)n;
                    }

                    i = GetInt(layout, "Margin");
                    if (i.HasValue) model.LayoutInfo.Margin = i.Value;
                    i = GetInt(layout, "MarginLeft");
                    if (i.HasValue) model.LayoutInfo.MarginLeft = i.Value;
                    i = GetInt(layout, "MarginTop");
                    if (i.HasValue) model.LayoutInfo.MarginTop = i.Value;
                    i = GetInt(layout, "MarginRight");
                    if (i.HasValue) model.LayoutInfo.MarginRight = i.Value;
                    i = GetInt(layout, "MarginBottom");
                    if (i.HasValue) model.LayoutInfo.MarginBottom = i.Value;
                }

                if (root.TryGetProperty("CommonInfo", out var common) && common.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    var s = GetString(common, "ButtonText");
                    if (s != null) model.CommonInfo.ButtonText = s;
                    s = GetString(common, "TooltipText");
                    if (s != null) model.CommonInfo.TooltipText = s;
                    s = GetString(common, "GroupId");
                    if (s != null) model.CommonInfo.GroupId = s;
                    var b = GetBool(common, "Enabled");
                    if (b.HasValue) model.CommonInfo.Enabled = b.Value;
                    if (common.TryGetProperty("HoverCursor", out var hc))
                    {
                        if (hc.ValueKind == System.Text.Json.JsonValueKind.String && Enum.TryParse<CommonCursorType>(hc.GetString(), true, out var ce))
                            model.CommonInfo.HoverCursor = ce;
                        else if (hc.ValueKind == System.Text.Json.JsonValueKind.Number && hc.TryGetInt32(out var n) && Enum.IsDefined(typeof(CommonCursorType), n))
                            model.CommonInfo.HoverCursor = (CommonCursorType)n;
                    }
                }

                if (root.TryGetProperty("FontInfo", out var font) && font.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    var s = GetString(font, "FontColor");
                    if (!string.IsNullOrWhiteSpace(s)) model.FontInfo.FontColor = Color.Parse(s);
                    var i = GetInt(font, "FontSize");
                    if (i.HasValue) model.FontInfo.FontSize = i.Value;
                    var b = GetBool(font, "IsBold");
                    if (b.HasValue) model.FontInfo.IsBold = b.Value;
                    b = GetBool(font, "IsItalic");
                    if (b.HasValue) model.FontInfo.IsItalic = b.Value;
                    b = GetBool(font, "IsUnderline");
                    if (b.HasValue) model.FontInfo.IsUnderline = b.Value;
                }

                if (root.TryGetProperty("ParagraphInfo", out var para) && para.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    var i = GetInt(para, "LineHeight");
                    if (i.HasValue) model.ParagraphInfo.LineHeight = i.Value;
                    i = GetInt(para, "ParagraphSpacingBefore");
                    if (i.HasValue) model.ParagraphInfo.ParagraphSpacingBefore = i.Value;
                    i = GetInt(para, "ParagraphSpacingAfter");
                    if (i.HasValue) model.ParagraphInfo.ParagraphSpacingAfter = i.Value;
                    if (para.TryGetProperty("TextAlignment", out var ta))
                    {
                        if (ta.ValueKind == System.Text.Json.JsonValueKind.String && Enum.TryParse<IconButtonParagraphInfo.TextAlignmentEnum>(ta.GetString(), true, out var e))
                            model.ParagraphInfo.TextAlignment = e;
                        else if (ta.ValueKind == System.Text.Json.JsonValueKind.Number && ta.TryGetInt32(out var n) && Enum.IsDefined(typeof(IconButtonParagraphInfo.TextAlignmentEnum), n))
                            model.ParagraphInfo.TextAlignment = (IconButtonParagraphInfo.TextAlignmentEnum)n;
                    }
                }

                if (root.TryGetProperty("MiscInfo", out var misc) && misc.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    var i = GetInt(misc, "IconWidth");
                    if (i.HasValue) model.MiscInfo.IconWidth = i.Value;
                    i = GetInt(misc, "IconHeight");
                    if (i.HasValue) model.MiscInfo.IconHeight = i.Value;

                    if (misc.TryGetProperty("IconPosition", out var ip))
                    {
                        if (ip.ValueKind == System.Text.Json.JsonValueKind.String && Enum.TryParse<IconButtonMiscInfo.IconPositionEnum>(ip.GetString(), true, out var e))
                            model.MiscInfo.IconPosition = e;
                        else if (ip.ValueKind == System.Text.Json.JsonValueKind.Number && ip.TryGetInt32(out var n) && Enum.IsDefined(typeof(IconButtonMiscInfo.IconPositionEnum), n))
                            model.MiscInfo.IconPosition = (IconButtonMiscInfo.IconPositionEnum)n;
                    }

                    i = GetInt(misc, "IconMargin");
                    if (i.HasValue) model.MiscInfo.IconMargin = i.Value;
                    i = GetInt(misc, "IconMarginLeft");
                    if (i.HasValue) model.MiscInfo.IconMarginLeft = i.Value;
                    i = GetInt(misc, "IconMarginTop");
                    if (i.HasValue) model.MiscInfo.IconMarginTop = i.Value;
                    i = GetInt(misc, "IconMarginRight");
                    if (i.HasValue) model.MiscInfo.IconMarginRight = i.Value;
                    i = GetInt(misc, "IconMarginBottom");
                    if (i.HasValue) model.MiscInfo.IconMarginBottom = i.Value;

                    var iconSourceBase64 = GetString(misc, "IconSourceBase64");
                    if (!string.IsNullOrWhiteSpace(iconSourceBase64))
                    {
                        try
                        {
                            var bytes = System.Convert.FromBase64String(iconSourceBase64);
                            var bitmapData = new BitmapData();
                            bitmapData.FromBytes(bytes);
                            model.MiscInfo.IconSource = bitmapData;
                        }
                        catch
                        {
                            model.MiscInfo.IconSource = new BitmapData();
                        }
                    }
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
            try
            {
                _ = (PropertyEditModelBase as IconButtonPropertyModelEdit)?.MiscInfo?.IconSourceBase64;
            }
            catch
            {
            }
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
        }

        #endregion

        #region 操作触发(属性变更 => 执行原子)

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
            if (string.Equals(oprtId, nameof(IconButtonCommonInfo.ButtonText), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(IconButtonPropertyModelEdit.CommonInfo);
                oprtCellId = IconButtonMapOprtCellConst.CommonInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(IconButtonCommonInfo.GroupId), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(IconButtonPropertyModelEdit.CommonInfo);
                oprtCellId = IconButtonMapOprtCellConst.CommonInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(IconButtonBrushInfo.BackgroundColor), StringComparison.Ordinal)
                || string.Equals(oprtId, nameof(IconButtonBrushInfo.BorderColor), StringComparison.Ordinal)
                || string.Equals(oprtId, nameof(IconButtonBrushInfo.ForegroundColor), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(IconButtonPropertyModelEdit.BrushInfo);
                oprtCellId = IconButtonMapOprtCellConst.BrushInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(IconButtonAppearanceInfo.Opacity), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(IconButtonPropertyModelEdit.AppearanceInfo);
                oprtCellId = IconButtonMapOprtCellConst.AppearanceInfo_MapOprtCellID;
                return true;
            }
            if (string.Equals(oprtId, nameof(IconButtonPropertyModelEdit.BrushInfo), StringComparison.Ordinal))
            { oprtCellId = IconButtonMapOprtCellConst.BrushInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(IconButtonPropertyModelEdit.AppearanceInfo), StringComparison.Ordinal))
            { oprtCellId = IconButtonMapOprtCellConst.AppearanceInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(IconButtonPropertyModelEdit.LayoutInfo), StringComparison.Ordinal))
            { oprtCellId = IconButtonMapOprtCellConst.LayoutInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(IconButtonPropertyModelEdit.CommonInfo), StringComparison.Ordinal))
            { oprtCellId = IconButtonMapOprtCellConst.CommonInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(IconButtonPropertyModelEdit.FontInfo), StringComparison.Ordinal))
            { oprtCellId = IconButtonMapOprtCellConst.FontInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(IconButtonPropertyModelEdit.ParagraphInfo), StringComparison.Ordinal))
            { oprtCellId = IconButtonMapOprtCellConst.ParagraphInfo_MapOprtCellID; return true; }
            if (string.Equals(oprtId, nameof(IconButtonPropertyModelEdit.MiscInfo), StringComparison.Ordinal))
            { oprtCellId = IconButtonMapOprtCellConst.MiscInfo_MapOprtCellID; return true; }
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

                if (val is MapOprtCellInstInfoList list)
                {
                    var idx = 0;
                    foreach (var item in list)
                    {
                        if (item is not MapOprtCellInstInfo inst) continue;
                        idx++;
                        var key = inst.InstanceID != Guid.Empty ? inst.InstanceID.ToString() : $"{source}:{idx}:{inst.OprtCellID}";
                        if (!seen.Add(key)) continue;
                        yield return (inst, source);
                    }
                }

                if (val is IEnumerable enumerable)
                {
                    var idx = 0;
                    foreach (var item in enumerable)
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

        #region 操作原子执行

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == IconButtonMapOprtCellConst.BrushInfo_MapOprtCellID)
                return ExecuteOprtCell<BrushInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == IconButtonMapOprtCellConst.AppearanceInfo_MapOprtCellID)
                return ExecuteOprtCell<AppearanceInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == IconButtonMapOprtCellConst.LayoutInfo_MapOprtCellID)
                return ExecuteOprtCell<LayoutInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == IconButtonMapOprtCellConst.CommonInfo_MapOprtCellID)
                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == IconButtonMapOprtCellConst.FontInfo_MapOprtCellID)
                return ExecuteOprtCell<FontInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == IconButtonMapOprtCellConst.ParagraphInfo_MapOprtCellID)
                return ExecuteOprtCell<ParagraphInfoMapOprtCellExector>(mapOprtCellInstInfo);
            if (mapOprtCellInstInfo.OprtCellID == IconButtonMapOprtCellConst.MiscInfo_MapOprtCellID)
                return ExecuteOprtCell<MiscInfoMapOprtCellExector>(mapOprtCellInstInfo);
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
                var vmInstance = callBack.GetMapCellVMObjInstance();

                if (vmInstance is IconButtonViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<BrushInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                var back = Color.Parse(param.BackgroundColor);
                                var border = Color.Parse(param.BorderColor);
                                var fore = Color.Parse(param.ForegroundColor);
                                var hoverBack = Color.Parse(param.HoverBackgroundColor);
                                var hoverFore = Color.Parse(param.HoverForegroundColor);
                                var clickBack = Color.Parse(param.ClickBackgroundColor);
                                var clickFore = Color.Parse(param.ClickForegroundColor);
                                var clickBorderLeft = Color.Parse(param.ClickBorderColorLeft);
                                var clickBorderTop = Color.Parse(param.ClickBorderColorTop);
                                var clickBorderRight = Color.Parse(param.ClickBorderColorRight);
                                var clickBorderBottom = Color.Parse(param.ClickBorderColorBottom);
                                PostToUI(() =>
                                {
                                    vm.BrushInfo.BackgroundColor = back;
                                    vm.BrushInfo.BorderColor = border;
                                    vm.BrushInfo.ForegroundColor = fore;
                                    vm.BrushInfo.HoverBackgroundColor = hoverBack;
                                    vm.BrushInfo.HoverForegroundColor = hoverFore;
                                    vm.BrushInfo.ClickBackgroundColor = clickBack;
                                    vm.BrushInfo.ClickForegroundColor = clickFore;
                                    vm.BrushInfo.ClickBorderColorLeft = clickBorderLeft;
                                    vm.BrushInfo.ClickBorderColorTop = clickBorderTop;
                                    vm.BrushInfo.ClickBorderColorRight = clickBorderRight;
                                    vm.BrushInfo.ClickBorderColorBottom = clickBorderBottom;
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(IconButtonPropertyModelEdit.BrushInfo));
                    if (val != null)
                    {
                        try
                        {
                            var brushInfo = DeserializeObject<IconButtonBrushInfo>(val);
                            PostToUI(() =>
                            {
                                vm.BrushInfo.BackgroundColor = brushInfo.BackgroundColor;
                                vm.BrushInfo.BorderColor = brushInfo.BorderColor;
                                vm.BrushInfo.ForegroundColor = brushInfo.ForegroundColor;
                                vm.BrushInfo.HoverBackgroundColor = brushInfo.HoverBackgroundColor;
                                vm.BrushInfo.HoverForegroundColor = brushInfo.HoverForegroundColor;
                                vm.BrushInfo.ClickBackgroundColor = brushInfo.ClickBackgroundColor;
                                vm.BrushInfo.ClickForegroundColor = brushInfo.ClickForegroundColor;
                                vm.BrushInfo.ClickBorderColorLeft = brushInfo.ClickBorderColorLeft;
                                vm.BrushInfo.ClickBorderColorTop = brushInfo.ClickBorderColorTop;
                                vm.BrushInfo.ClickBorderColorRight = brushInfo.ClickBorderColorRight;
                                vm.BrushInfo.ClickBorderColorBottom = brushInfo.ClickBorderColorBottom;
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
                if (callBack.GetMapCellVMObjInstance() is IconButtonViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<AppearanceInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                int.TryParse(param.Opacity, out var opacity);
                                int.TryParse(param.BorderThicknessLeft, out var btl);
                                int.TryParse(param.BorderThicknessTop, out var btt);
                                int.TryParse(param.BorderThicknessRight, out var btr);
                                int.TryParse(param.BorderThicknessBottom, out var btb);
                                int.TryParse(param.ClickBorderThicknessLeft, out var cbtl);
                                int.TryParse(param.ClickBorderThicknessTop, out var cbtt);
                                int.TryParse(param.ClickBorderThicknessRight, out var cbtr);
                                int.TryParse(param.ClickBorderThicknessBottom, out var cbtb);
                                PostToUI(() =>
                                {
                                    vm.AppearanceInfo.Opacity = opacity;
                                    vm.AppearanceInfo.BorderThicknessLeft = btl;
                                    vm.AppearanceInfo.BorderThicknessTop = btt;
                                    vm.AppearanceInfo.BorderThicknessRight = btr;
                                    vm.AppearanceInfo.BorderThicknessBottom = btb;
                                    vm.AppearanceInfo.ClickBorderThicknessLeft = cbtl;
                                    vm.AppearanceInfo.ClickBorderThicknessTop = cbtt;
                                    vm.AppearanceInfo.ClickBorderThicknessRight = cbtr;
                                    vm.AppearanceInfo.ClickBorderThicknessBottom = cbtb;
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(IconButtonPropertyModelEdit.AppearanceInfo));
                    if (val != null)
                    {
                        try
                        {
                            var appearanceInfo = DeserializeObject<IconButtonAppearanceInfo>(val);
                            PostToUI(() =>
                            {
                                vm.AppearanceInfo.Opacity = appearanceInfo.Opacity;
                                vm.AppearanceInfo.BorderThicknessLeft = appearanceInfo.BorderThicknessLeft;
                                vm.AppearanceInfo.BorderThicknessTop = appearanceInfo.BorderThicknessTop;
                                vm.AppearanceInfo.BorderThicknessRight = appearanceInfo.BorderThicknessRight;
                                vm.AppearanceInfo.BorderThicknessBottom = appearanceInfo.BorderThicknessBottom;
                                vm.AppearanceInfo.ClickBorderThicknessLeft = appearanceInfo.ClickBorderThicknessLeft;
                                vm.AppearanceInfo.ClickBorderThicknessTop = appearanceInfo.ClickBorderThicknessTop;
                                vm.AppearanceInfo.ClickBorderThicknessRight = appearanceInfo.ClickBorderThicknessRight;
                                vm.AppearanceInfo.ClickBorderThicknessBottom = appearanceInfo.ClickBorderThicknessBottom;
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
                if (callBack.GetMapCellVMObjInstance() is IconButtonViewModel vm)
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
                                PostToUI(() =>
                                {
                                    // 宽高主数据统一落到父类 Width/Height，旧 LayoutInfo 宽高仅保留兼容镜像。
                                    // 宽高主数据统一落到父类 Width/Height，避免 IconButton 继续维护额外尺寸中间态。
                                    vm.LayoutInfo.HorizontalAlignment = param.HorizontalAlignment;
                                    vm.LayoutInfo.VerticalAlignment = param.VerticalAlignment;
                                    vm.LayoutInfo.MarginTop = mt;
                                    vm.LayoutInfo.MarginLeft = ml;
                                    vm.LayoutInfo.MarginBottom = mb;
                                    vm.LayoutInfo.MarginRight = mr;
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(IconButtonPropertyModelEdit.LayoutInfo));
                    if (val != null)
                    {
                        try
                        {
                            var layoutInfo = DeserializeObject<IconButtonLayoutInfo>(val);
                            PostToUI(() =>
                            {
                                vm.LayoutInfo.HorizontalAlignment = layoutInfo.HorizontalAlignment;
                                vm.LayoutInfo.VerticalAlignment = layoutInfo.VerticalAlignment;
                                vm.LayoutInfo.MarginTop = layoutInfo.MarginTop;
                                vm.LayoutInfo.MarginLeft = layoutInfo.MarginLeft;
                                vm.LayoutInfo.MarginBottom = layoutInfo.MarginBottom;
                                vm.LayoutInfo.MarginRight = layoutInfo.MarginRight;
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
                if (callBack.GetMapCellVMObjInstance() is IconButtonViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<CommonInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                PostToUI(() =>
                                {
                                    vm.CommonInfo.ButtonText = param.ButtonText ?? "";
                                    vm.CommonInfo.HoverCursor = param.HoverCursor;
                                    vm.CommonInfo.Enabled = param.Enabled;
                                    vm.CommonInfo.TooltipText = param.TooltipText ?? "";
                                    vm.CommonInfo.GroupId = param.GroupId ?? "";
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(IconButtonPropertyModelEdit.CommonInfo));
                    if (val != null)
                    {
                        try
                        {
                            var commonInfo = DeserializeObject<IconButtonCommonInfo>(val);
                            PostToUI(() =>
                            {
                                vm.CommonInfo.ButtonText = commonInfo.ButtonText;
                                vm.CommonInfo.HoverCursor = commonInfo.HoverCursor;
                                vm.CommonInfo.Enabled = commonInfo.Enabled;
                                vm.CommonInfo.TooltipText = commonInfo.TooltipText;
                                vm.CommonInfo.GroupId = commonInfo.GroupId;
                            });
                        }
                        catch { }
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
                if (callBack.GetMapCellVMObjInstance() is IconButtonViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<FontInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                int.TryParse(param.FontSize, out var size);
                                var color = Color.Parse(param.FontColor);
                                PostToUI(() =>
                                {
                                    vm.FontInfo.FontColor = color;
                                    vm.FontInfo.FontSize = size;
                                    vm.FontInfo.IsBold = param.IsBold;
                                    vm.FontInfo.IsItalic = param.IsItalic;
                                    vm.FontInfo.IsUnderline = param.IsUnderline;
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(IconButtonPropertyModelEdit.FontInfo));
                    if (val != null)
                    {
                        try
                        {
                            var fontInfo = DeserializeObject<IconButtonFontInfo>(val);
                            PostToUI(() =>
                            {
                                vm.FontInfo.FontColor = fontInfo.FontColor;
                                vm.FontInfo.FontSize = fontInfo.FontSize;
                                vm.FontInfo.IsBold = fontInfo.IsBold;
                                vm.FontInfo.IsItalic = fontInfo.IsItalic;
                                vm.FontInfo.IsUnderline = fontInfo.IsUnderline;
                            });
                        }
                        catch { }
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
                if (callBack.GetMapCellVMObjInstance() is IconButtonViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<ParagraphInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                int.TryParse(param.LineHeight, out var lh);
                                int.TryParse(param.ParagraphSpacingBefore, out var psb);
                                int.TryParse(param.ParagraphSpacingAfter, out var psa);
                                PostToUI(() =>
                                {
                                    vm.ParagraphInfo.LineHeight = lh;
                                    vm.ParagraphInfo.ParagraphSpacingBefore = psb;
                                    vm.ParagraphInfo.ParagraphSpacingAfter = psa;
                                    vm.ParagraphInfo.TextAlignment = param.TextAlignment;
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(IconButtonPropertyModelEdit.ParagraphInfo));
                    if (val != null)
                    {
                        try
                        {
                            var paragraphInfo = DeserializeObject<IconButtonParagraphInfo>(val);
                            PostToUI(() =>
                            {
                                vm.ParagraphInfo.LineHeight = paragraphInfo.LineHeight;
                                vm.ParagraphInfo.ParagraphSpacingBefore = paragraphInfo.ParagraphSpacingBefore;
                                vm.ParagraphInfo.ParagraphSpacingAfter = paragraphInfo.ParagraphSpacingAfter;
                                vm.ParagraphInfo.TextAlignment = paragraphInfo.TextAlignment;
                            });
                        }
                        catch { }
                    }
                }
            }
        }

        private class MiscInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is IconButtonViewModel vm)
                {
                    if (cfg != null && cfg.Length > 0)
                    {
                        try
                        {
                            var param = JsonSerializer.Deserialize<MiscInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                            if (param != null)
                            {
                                int.TryParse(param.IconWidth, out var iw);
                                int.TryParse(param.IconHeight, out var ih);
                                int.TryParse(param.IconMarginTop, out var imt);
                                int.TryParse(param.IconMarginLeft, out var iml);
                                int.TryParse(param.IconMarginBottom, out var imb);
                                int.TryParse(param.IconMarginRight, out var imr);
                                PostToUI(() =>
                                {
                                    vm.MiscInfo.IconWidth = iw;
                                    vm.MiscInfo.IconHeight = ih;
                                    vm.MiscInfo.IconPosition = param.IconPosition;
                                    vm.MiscInfo.IconMarginTop = imt;
                                    vm.MiscInfo.IconMarginLeft = iml;
                                    vm.MiscInfo.IconMarginBottom = imb;
                                    vm.MiscInfo.IconMarginRight = imr;
                                });
                                return;
                            }
                        }
                        catch { }
                    }

                    var val = callBack.GetMapCellPropValue(nameof(IconButtonPropertyModelEdit.MiscInfo));
                    if (val != null)
                    {
                        try
                        {
                            var miscInfo = DeserializeObject<IconButtonMiscInfo>(val);
                            PostToUI(() =>
                            {
                                vm.MiscInfo.IconWidth = miscInfo.IconWidth;
                                vm.MiscInfo.IconHeight = miscInfo.IconHeight;
                                vm.MiscInfo.IconPosition = miscInfo.IconPosition;
                                vm.MiscInfo.IconMarginTop = miscInfo.IconMarginTop;
                                vm.MiscInfo.IconMarginLeft = miscInfo.IconMarginLeft;
                                vm.MiscInfo.IconMarginBottom = miscInfo.IconMarginBottom;
                                vm.MiscInfo.IconMarginRight = miscInfo.IconMarginRight;
                            });
                        }
                        catch { }
                    }
                }
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            if (_loadedPropertyEditFromBytes)
            {
                ReloadIconButtonViewSynchronously();
                return;
            }

            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.BrushInfo), "Init", null);
            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.AppearanceInfo), "Init", null);
            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.LayoutInfo), "Init", null);
            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.CommonInfo), "Init", null);
            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.FontInfo), "Init", null);
            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.ParagraphInfo), "Init", null);
            ExecuteOprtByPropertyId(nameof(IconButtonPropertyModelEdit.MiscInfo), "Init", null);
            ReloadIconButtonViewSynchronously();
        }


        public override void OnZoomChanged() { }

        public override string ToString() => "图标按钮";

        #endregion

        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }
    }

    /// <summary>
    /// 图标按钮属性绑定编辑模型（按Info对象粒度）
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class IconButtonPropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _backgroundColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _borderColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _foregroundColor = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _opacity = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.Integer);
        private PropertyBindInfo _buttonText = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String);

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

        [DisplayName("按钮文本")]
        [Category("绑定信息")]
        [PropertySortOrder(6)]
        [BindMPPropertyID]
        public PropertyBindInfo ButtonText
        {
            get => _buttonText;
            set => SetProperty(ref _buttonText, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String));
        }

        public void CopyFrom(IconButtonPropertyBindEditModel source)
        {
            if (source == null) return;
            base.CopyFrom(source);
            BackgroundColor = source.BackgroundColor;
            BorderColor = source.BorderColor;
            ForegroundColor = source.ForegroundColor;
            Opacity = source.Opacity;
            ButtonText = source.ButtonText;
        }
    }
}



