using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using GKG.Map.MapCell.Generic;
using GKG.Map.MapCell.Generic.IconButton.ViewModels;
using GKG.Map.MapCell.Generic.IconButton.Views;
using Griffins;
using PropertyModels.ComponentModel;
using System;
using System.ComponentModel;
using System.Linq;

namespace GKG.Map.MapCell.Generic.IconButton
{
    /// <summary>
    /// IconButton 图元的控制器对象
    /// 负责 View/ViewModel 与底层 Griffins 平台的属性映射、事件分发及序列化
    /// </summary>
    internal class MapCellIconButtonCtlObj : ControlCellBase
    {
        #region 字段与属性

        private IconButtonView view;
        private IconButtonViewModel viewModel;

        /// <summary>
        /// 防回音屏蔽盾：标记当前是否正在接收平台下发的数据
        /// </summary>
        private bool _isReceivingPlatformData = false;

        [Browsable(false)]
        public IconButtonPropertyModelEdit IconButtonPropertyModelEdit => PropertyEditModelBase as IconButtonPropertyModelEdit;

        #endregion

        #region 构造与初始化

        public MapCellIconButtonCtlObj(MapObjID mapCellID, string mapCellName, bool designTime = false)
        {
            // 1. 初始化基础模型
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);
            base.SetName(mapCellName);

            // 2. 初始化视图与视图模型
            view = new IconButtonView();
            (this as IMapCellTypeBase).Name = Resource_IconButton.IconButton;

            // 3. 完美映射鼠标事件：将 ViewModel 层捕获的动作，路由给平台的事件引擎
            viewModel = new IconButtonViewModel(IconButtonPropertyModelEdit)
            {
                OnClickAction = () => ExecEvent(MapObjPropEventConst.Event_MouseClick),
                OnMouseDoubleClickAction = () => ExecEvent(MapObjPropEventConst.Event_MouseDoubleClick),
                OnMouseEnterAction = () => ExecEvent(MapObjPropEventConst.Event_MouseEnter),
                OnMouseLeaveAction = () => ExecEvent(MapObjPropEventConst.Event_MouseLeave),
                OnMouseDownAction = () => ExecEvent(MapObjPropEventConst.Event_MouseDown),
                OnMouseUpAction = () => ExecEvent(MapObjPropEventConst.Event_MouseUp)
            };
            view.DataContext = viewModel;

            // 4. 注册图元属性与更新原子
            RegisterPropertiesAndAtoms();
        }

        /// <summary>
        /// 注册供属性面板和后台绑定的所有属性及操作原子
        /// </summary>
        private void RegisterPropertiesAndAtoms()
        {
            // =========================================================================
            // [组别 1：数据组] - 负责文本、图标内容及启用状态
            // =========================================================================
            RegisterOprtCellInfo(new MapOprtCellInfo(IconButtonMapOprtCellConst.DataGroup_MapOprtCellID, Resource_IconButton.Btn_DataGroup_OprtCellName));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.ShowText), Resource_IconButton.Btn_ShowText_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(true)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.ShowText), Resource_IconButton.Btn_ShowText_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.DataGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.ButtonText), Resource_IconButton.Btn_ButtonText_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("通用按钮")));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.ButtonText), Resource_IconButton.Btn_ButtonText_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.DataGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.ShowIcon), Resource_IconButton.Btn_ShowIcon_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(false)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.ShowIcon), Resource_IconButton.Btn_ShowIcon_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.DataGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.IconBase64), Resource_IconButton.Btn_IconBase64_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue(string.Empty)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.IconBase64), Resource_IconButton.Btn_IconBase64_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.DataGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.IsEnabled), Resource_IconButton.Btn_IsEnabled_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(true)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.IsEnabled), Resource_IconButton.Btn_IsEnabled_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.DataGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.CursorType), Resource_IconButton.Btn_CursorType_Name, GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), false, true, new GriffinsBaseValue(9))); // 9=Hand
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.CursorType), Resource_IconButton.Btn_CursorType_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.DataGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.ToolTip), Resource_IconButton.Btn_ToolTip_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue(string.Empty)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.ToolTip), Resource_IconButton.Btn_ToolTip_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.DataGroup_MapOprtCellID)));

            // =========================================================================
            // [组别 2：样式组] - 负责颜色、边框、透明度及圆角
            // =========================================================================
            RegisterOprtCellInfo(new MapOprtCellInfo(IconButtonMapOprtCellConst.StyleGroup_MapOprtCellID, Resource_IconButton.Btn_StyleGroup_OprtCellName));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.BackgroundColor), Resource_IconButton.Btn_BackgroundColor_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(Color), false, true, new GriffinsBaseValue(Color.Parse("#1890FF").ToString())));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.BackgroundColor), Resource_IconButton.Btn_BackgroundColor_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.ForegroundColor), Resource_IconButton.Btn_ForegroundColor_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(Color), false, true, new GriffinsBaseValue(Colors.White.ToString())));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.ForegroundColor), Resource_IconButton.Btn_ForegroundColor_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.BorderBrush), Resource_IconButton.Btn_BorderBrush_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(Color), false, true, new GriffinsBaseValue(Colors.Transparent.ToString())));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.BorderBrush), Resource_IconButton.Btn_BorderBrush_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.BorderThickness), Resource_IconButton.Btn_BorderThickness_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("0")));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.BorderThickness), Resource_IconButton.Btn_BorderThickness_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.CornerRadius), Resource_IconButton.Btn_CornerRadius_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("4")));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.CornerRadius), Resource_IconButton.Btn_CornerRadius_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.Opacity), Resource_IconButton.Btn_Opacity_Name, GriffinsBaseDataType.Decimal, Guid.Empty, typeof(double), false, true, new GriffinsBaseValue((decimal)1.0)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.Opacity), Resource_IconButton.Btn_Opacity_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.StyleGroup_MapOprtCellID)));

            // =========================================================================
            // [组别 3：字体组] - 负责字号、字体族及字形效果
            // =========================================================================
            RegisterOprtCellInfo(new MapOprtCellInfo(IconButtonMapOprtCellConst.FontGroup_MapOprtCellID, Resource_IconButton.Btn_FontGroup_OprtCellName));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.FontSize), Resource_IconButton.Btn_FontSize_Name, GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), false, true, new GriffinsBaseValue(14)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.FontSize), Resource_IconButton.Btn_FontSize_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.FontFamily), Resource_IconButton.Btn_FontFamily_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("Microsoft YaHei")));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.FontFamily), Resource_IconButton.Btn_FontFamily_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.IsBold), Resource_IconButton.Btn_IsBold_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(false)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.IsBold), Resource_IconButton.Btn_IsBold_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.IsItalic), Resource_IconButton.Btn_IsItalic_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(false)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.IsItalic), Resource_IconButton.Btn_IsItalic_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.IsUnderline), Resource_IconButton.Btn_IsUnderline_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(false)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.IsUnderline), Resource_IconButton.Btn_IsUnderline_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.FontGroup_MapOprtCellID)));

            // =========================================================================
            // [组别 4：布局组] - 负责图标位置、内外边距及对齐方式
            // =========================================================================
            RegisterOprtCellInfo(new MapOprtCellInfo(IconButtonMapOprtCellConst.LayoutGroup_MapOprtCellID, Resource_IconButton.Btn_LayoutGroup_OprtCellName));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.IconPlacement), Resource_IconButton.Btn_IconPlacement_Name, GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), false, true, new GriffinsBaseValue(0)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.IconPlacement), Resource_IconButton.Btn_IconPlacement_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.LayoutGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.IconSize), Resource_IconButton.Btn_IconSize_Name, GriffinsBaseDataType.Decimal, Guid.Empty, typeof(double), false, true, new GriffinsBaseValue((decimal)20.0)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.IconSize), Resource_IconButton.Btn_IconSize_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.LayoutGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.IconSpacing), Resource_IconButton.Btn_IconSpacing_Name, GriffinsBaseDataType.Decimal, Guid.Empty, typeof(double), false, true, new GriffinsBaseValue((decimal)8.0)));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.IconSpacing), Resource_IconButton.Btn_IconSpacing_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.LayoutGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.Margin), Resource_IconButton.Btn_Margin_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("0")));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.Margin), Resource_IconButton.Btn_Margin_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.LayoutGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.Padding), Resource_IconButton.Btn_Padding_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("8,4")));
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.Padding), Resource_IconButton.Btn_Padding_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.LayoutGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(IconButtonPropertyModelEdit.HorizontalAlign), Resource_IconButton.Btn_HorizontalAlign_Name, GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), false, true, new GriffinsBaseValue(2))); // 2=Center
            RegisterOprtInfo(new MapOprtInfo(nameof(IconButtonPropertyModelEdit.HorizontalAlign), Resource_IconButton.Btn_HorizontalAlign_OprtName, OprtExecKind.Normal, "", CreateAtom(IconButtonMapOprtCellConst.LayoutGroup_MapOprtCellID)));
        }

        private MapOprtCellInstInfoList CreateAtom(MapOprtCellID oprtCellId)
            => new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = oprtCellId, CfgInfo = null } };

        #endregion

        #region 事件分发与属性更新控制

        /// <summary>
        /// 触发执行指定的系统事件指令 (宏或下发)
        /// </summary>
        private void ExecEvent(string eventId)
        {
            var eventCmdInfo = EventBindEditModel.EventCmdInfos.FirstOrDefault(info => info.EventID == eventId);
            if (eventCmdInfo != null)
            {
                CallBack?.ExecMapCellEvent(eventCmdInfo.EventCmdKind, eventCmdInfo.CmdID, CommHelper.ToEventParamValueList(eventCmdInfo.CmdParam), out _);
            }
        }

        /// <summary>
        /// 属性变更后的回调通知
        /// </summary>
        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            if (CallBack == null || string.IsNullOrWhiteSpace(propertyID)) return;

            // 按钮作为纯粹的“触发型/下发型”图元，不需要将界面产生的自身状态(如悬停)反推给后端变量。
            // 只需要通知框架执行 UI 更新(ExecOprt)即可。
            CallBack.ExecOprt(propertyID);
        }

        /// <summary>
        /// 批量接收平台下发的值
        /// </summary>
        protected override bool SetPropertyValue(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            if (gFBaseTypePropValues == null) return false;
            bool flag = true;
            foreach (var prop in gFBaseTypePropValues)
            {
                if (prop != null && !string.IsNullOrWhiteSpace(prop.PropertyID.ToString()))
                {
                    flag &= SetPropertyValue(prop.PropertyID.ToString(), prop.Value, true);
                }
            }
            return flag;
        }

        /// <summary>
        /// 接收平台下发的单个属性值（防回音处理核心）
        /// </summary>
        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            IconButtonPropertyModelEdit.IsRuning = isRuning;

            // 开启防回音盾：接收网络数据期间，图元内部的变化绝不能反推回网络
            _isReceivingPlatformData = true;
            try
            {
                if (isRuning && propertyVal != null)
                {
                    if (propertyID == nameof(IconButtonPropertyBindEditModel.ButtonText))
                    {
                        IconButtonPropertyModelEdit.ButtonText = propertyVal.ToPrimitiveValue<string>() ?? string.Empty;
                        return true;
                    }
                    if (propertyID == nameof(IconButtonPropertyBindEditModel.IconBase64))
                    {
                        IconButtonPropertyModelEdit.IconBase64 = propertyVal.ToPrimitiveValue<string>() ?? string.Empty;
                        return true;
                    }
                    if (propertyID == nameof(IconButtonPropertyBindEditModel.IsEnabled))
                    {
                        IconButtonPropertyModelEdit.IsEnabled = propertyVal.ToPrimitiveValue<bool>();
                        return true;
                    }
                    if (propertyID == nameof(IconButtonPropertyBindEditModel.BackgroundColor))
                    {
                        var colorStr = propertyVal.ToPrimitiveValue<string>();
                        if (Color.TryParse(colorStr, out var c))
                        {
                            IconButtonPropertyModelEdit.BackgroundColor = c;
                        }
                        return true;
                    }
                }
                return base.SetPropertyValue(propertyID, propertyVal, isRuning);
            }
            finally
            {
                // 处理完毕，关闭防回音盾
                _isReceivingPlatformData = false;
            }
        }

        #endregion

        #region UI 刷新执行器 (Executors)

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            IMapOprtCellExector exector = null;
            if (mapOprtCellInstInfo.OprtCellID == IconButtonMapOprtCellConst.DataGroup_MapOprtCellID)
                exector = GetOrCreateExector(mapOprtCellInstInfo.InstanceID, () => new DataGroupMapOprtCellExector(this));
            else if (mapOprtCellInstInfo.OprtCellID == IconButtonMapOprtCellConst.StyleGroup_MapOprtCellID)
                exector = GetOrCreateExector(mapOprtCellInstInfo.InstanceID, () => new StyleGroupMapOprtCellExector(this));
            else if (mapOprtCellInstInfo.OprtCellID == IconButtonMapOprtCellConst.FontGroup_MapOprtCellID)
                exector = GetOrCreateExector(mapOprtCellInstInfo.InstanceID, () => new FontGroupMapOprtCellExector(this));
            else if (mapOprtCellInstInfo.OprtCellID == IconButtonMapOprtCellConst.LayoutGroup_MapOprtCellID)
                exector = GetOrCreateExector(mapOprtCellInstInfo.InstanceID, () => new LayoutGroupMapOprtCellExector(this));

            if (exector != null)
            {
                exector.Exec(mapOprtCellInstInfo.CfgInfo);
                return true;
            }
            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        private IMapOprtCellExector GetOrCreateExector(Guid instanceId, Func<IMapOprtCellExector> factory)
        {
            if (!MapOprtCellExectorDict.TryGetValue(instanceId, out IMapOprtCellExector exector))
            {
                exector = factory();
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(instanceId, exector);
            }
            return exector;
        }

        /// <summary>
        /// 确保行为被调度到 UI 主线程执行
        /// </summary>
        private static void PostToUI(Action action)
        {
            if (Dispatcher.UIThread.CheckAccess()) action();
            else Dispatcher.UIThread.Post(action);
        }

        // =======================================================
        // 对应四大操作组的独立执行器
        // =======================================================

        private class DataGroupMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellIconButtonCtlObj _o;
            private IMapOprtCellExectorCallBack _cb = null!;
            public DataGroupMapOprtCellExector(MapCellIconButtonCtlObj o) => _o = o;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack c) => _cb = c;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (_cb.GetMapCellVMObjInstance() is IconButtonViewModel vm)
                    PostToUI(() => vm.RefreshData());
            }
        }

        private class StyleGroupMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellIconButtonCtlObj _o;
            private IMapOprtCellExectorCallBack _cb = null!;
            public StyleGroupMapOprtCellExector(MapCellIconButtonCtlObj o) => _o = o;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack c) => _cb = c;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (_cb.GetMapCellVMObjInstance() is IconButtonViewModel vm)
                    PostToUI(() => vm.RefreshStyle());
            }
        }

        private class FontGroupMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellIconButtonCtlObj _o;
            private IMapOprtCellExectorCallBack _cb = null!;
            public FontGroupMapOprtCellExector(MapCellIconButtonCtlObj o) => _o = o;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack c) => _cb = c;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (_cb.GetMapCellVMObjInstance() is IconButtonViewModel vm)
                    PostToUI(() => vm.RefreshFont());
            }
        }

        private class LayoutGroupMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellIconButtonCtlObj _o;
            private IMapOprtCellExectorCallBack _cb = null!;
            public LayoutGroupMapOprtCellExector(MapCellIconButtonCtlObj o) => _o = o;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack c) => _cb = c;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (_cb.GetMapCellVMObjInstance() is IconButtonViewModel vm)
                    PostToUI(() => vm.RefreshLayout());
            }
        }

        #endregion

        #region 生命周期与序列化 (读档/存档)

        protected override void OnInit()
        {
            base.OnInit();
            Dispatcher.UIThread.Post(() => viewModel?.ReloadFromModel());
        }

        public override void OnDispose()
        {
            view.DataContext = null;
            viewModel?.Dispose();
            viewModel = null;
            base.OnDispose();
        }

        /// <summary>
        /// 从存档字节流中恢复图元配置
        /// </summary>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);

            var propertyEditModelBase = JsonObjConvert.FromJSon<IconButtonPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            if (propertyEditModelBase != null)
            {
                (PropertyEditModelBase as IconButtonPropertyModelEdit)!.CopyFrom(propertyEditModelBase);
            }

            var propertyBindEditModelBase = JsonObjConvert.FromJSon<IconButtonPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            if (propertyBindEditModelBase != null)
            {
                (PropertyBindEditModelBase as IconButtonPropertyBindEditModel)!.CopyFrom(propertyBindEditModelBase);
            }

            var eventBindJson = br.ReadString("EventBindEditModel");
            if (!string.IsNullOrEmpty(eventBindJson))
            {
                var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(eventBindJson);
                if (eventBindEditModel != null)
                {
                    EventBindEditModel.CopyFrom(eventBindEditModel);
                }
            }

            Dispatcher.UIThread.Post(() => viewModel?.ReloadFromModel());
        }

        /// <summary>
        /// 将当前图元配置写入存档字节流
        /// </summary>
        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
        }

        /// <summary>
        /// 支持编辑器内的复制/粘贴图元操作
        /// </summary>
        protected override void OnCopyFrom(ControlCellBase source)
        {
            base._CopyFrom(source);
            var obj = source as MapCellIconButtonCtlObj;
            if (obj == null) return;

            IconButtonPropertyModelEdit.CopyFrom(obj.PropertyEditModelBase as IconButtonPropertyModelEdit);

            if (PropertyBindEditModelBase is IconButtonPropertyBindEditModel selfBind && obj.PropertyBindEditModelBase is IconButtonPropertyBindEditModel srcBind)
            {
                selfBind.CopyFrom(srcBind);
            }

            EventBindEditModel.CopyFrom(obj.EventBindEditModel);
            Dispatcher.UIThread.Post(() => viewModel?.ReloadFromModel());
        }

        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => viewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new IconButtonPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new IconButtonPropertyBindEditModel();

        /// <summary>
        /// 将标准事件集合暴露给平台右侧属性面板，允许用户绑定自定义动作
        /// </summary>
        public override EventBindEditModel CreateEventBindEditModel() => new EventBindEditModel()
        {
            EventCmdInfos = new BindingList<EventCmdInfo>() {
                new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = MapObjPropEventConst.Event_MouseClick },
                new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = MapObjPropEventConst.Event_MouseDoubleClick },
                new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = MapObjPropEventConst.Event_MouseEnter },
                new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = MapObjPropEventConst.Event_MouseLeave },
                new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = MapObjPropEventConst.Event_MouseDown },
                new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = MapObjPropEventConst.Event_MouseUp }
            }
        };

        public override MapCellPropValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null;
        }

        #endregion
    }
}