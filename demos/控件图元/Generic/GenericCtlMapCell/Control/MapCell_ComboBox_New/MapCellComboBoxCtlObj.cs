using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using GKG.Map.MapCell.Generic;
using GKG.Map.MapCell.Generic.Control.MapCell_ComboBox.Models;
using GKG.Map.MapCell.Generic.Control.MapCell_ComboBox.ViewModels;
using GKG.Map.MapCell.Generic.Control.MapCell_ComboBox.Views;
using Griffins;
using PropertyModels.ComponentModel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ComboBox
{
    internal class MapCellComboBoxCtlObj : ControlCellBase
    {
        private ComboBoxView view;
        private ComboBoxViewModel viewModel;

        // 新增防回音标志
        private bool _isReceivingPlatformData = false;

        public MapCellComboBoxCtlObj(MapObjID mapCellID, string mapCellName, bool designTime = false)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);
            base.SetName(mapCellName);

            view = new ComboBoxView();
            (this as IMapCellTypeBase).Name = "下拉框";

            viewModel = new ComboBoxViewModel(ComboBoxPropertyModelEdit, SelectionChangedExec, DropDownOpenedExec, DropDownClosedExec);
            view.DataContext = viewModel;

            #region 注册属性、操作原子和操作

            // ---------------- [组别 1：数据组] ----------------
            RegisterOprtCellInfo(new MapOprtCellInfo(ComboBoxMapOprtCellConst.DataGroup_MapOprtCellID, ResourceA.ComboBox_DataGroup_OprtCellName));

            var defaultRunModeList = new RunModeList { "Item1", "Item2" };
            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.Items), ResourceA.ComboBox_Items_Name, GriffinsBaseDataType.Object_Json, RunModeList.Object_ID, typeof(RunModeList), false, true, ((IGriffinsBaseValue)defaultRunModeList).ToBaseValue()));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.Items), ResourceA.ComboBox_Items_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.DataGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.SelectedItem), ResourceA.ComboBox_SelectedItem_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue(string.Empty)));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.SelectedItem), ResourceA.ComboBox_SelectedItem_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.DataGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.PlaceholderText), ResourceA.ComboBox_PlaceholderText_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue(string.Empty)));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.PlaceholderText), ResourceA.ComboBox_PlaceholderText_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.DataGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.IsEditable), ResourceA.ComboBox_IsEditable_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(false)));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.IsEditable), ResourceA.ComboBox_IsEditable_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.DataGroup_MapOprtCellID)));


            // ---------------- [组别 2：样式组] ----------------
            RegisterOprtCellInfo(new MapOprtCellInfo(ComboBoxMapOprtCellConst.StyleGroup_MapOprtCellID, ResourceA.ComboBox_StyleGroup_OprtCellName));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.BackgroundColor), ResourceA.ComboBox_BackgroundColor_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(Color), false, true, new GriffinsBaseValue(Colors.White.ToString())));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.BackgroundColor), ResourceA.ComboBox_BackgroundColor_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.ForegroundColor), ResourceA.ComboBox_ForegroundColor_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(Color), false, true, new GriffinsBaseValue(Colors.Black.ToString())));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.ForegroundColor), ResourceA.ComboBox_ForegroundColor_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.BorderBrush), ResourceA.ComboBox_BorderBrush_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(Color), false, true, new GriffinsBaseValue(Colors.Gray.ToString())));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.BorderBrush), ResourceA.ComboBox_BorderBrush_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.BorderThickness), ResourceA.ComboBox_BorderThickness_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("1")));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.BorderThickness), ResourceA.ComboBox_BorderThickness_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.Opacity), ResourceA.ComboBox_Opacity_Name, GriffinsBaseDataType.Decimal, Guid.Empty, typeof(double), false, true, new GriffinsBaseValue((decimal)1.0)));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.Opacity), ResourceA.ComboBox_Opacity_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.StyleGroup_MapOprtCellID)));


            // ---------------- [组别 3：字体组] ----------------
            RegisterOprtCellInfo(new MapOprtCellInfo(ComboBoxMapOprtCellConst.FontGroup_MapOprtCellID, ResourceA.ComboBox_FontGroup_OprtCellName));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.FontSize), ResourceA.ComboBox_FontSize_Name, GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), false, true, new GriffinsBaseValue(14)));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.FontSize), ResourceA.ComboBox_FontSize_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.FontFamily), ResourceA.ComboBox_FontFamily_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("Microsoft YaHei")));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.FontFamily), ResourceA.ComboBox_FontFamily_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.IsBold), ResourceA.ComboBox_IsBold_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(false)));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.IsBold), ResourceA.ComboBox_IsBold_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.IsItalic), ResourceA.ComboBox_IsItalic_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(false)));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.IsItalic), ResourceA.ComboBox_IsItalic_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.FontGroup_MapOprtCellID)));


            // ---------------- [组别 4：布局组] ----------------
            RegisterOprtCellInfo(new MapOprtCellInfo(ComboBoxMapOprtCellConst.LayoutGroup_MapOprtCellID, ResourceA.ComboBox_LayoutGroup_OprtCellName));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.Margin), ResourceA.ComboBox_Margin_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("0")));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.Margin), ResourceA.ComboBox_Margin_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.LayoutGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.Padding), ResourceA.ComboBox_Padding_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("2")));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.Padding), ResourceA.ComboBox_Padding_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.LayoutGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(ComboBoxPropertyModelEdit.MaxDropDownHeight), ResourceA.ComboBox_MaxDropDownHeight_Name, GriffinsBaseDataType.Decimal, Guid.Empty, typeof(double), false, true, new GriffinsBaseValue((decimal)300.0)));
            RegisterOprtInfo(new MapOprtInfo(nameof(ComboBoxPropertyModelEdit.MaxDropDownHeight), ResourceA.ComboBox_MaxDropDownHeight_OprtName, OprtExecKind.Normal, "", CreateAtom(ComboBoxMapOprtCellConst.LayoutGroup_MapOprtCellID)));

            #endregion
        }

        // 辅助生成操作原子实例列表的简化函数
        private MapOprtCellInstInfoList CreateAtom(MapOprtCellID oprtCellId)
        {
            return new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = oprtCellId, CfgInfo = null } };
        }

        [Browsable(false)]
        public ComboBoxPropertyModelEdit ComboBoxPropertyModelEdit => PropertyEditModelBase as ComboBoxPropertyModelEdit;

        public override void OnDispose()
        {
            view.DataContext = null;
            viewModel?.Dispose();
            viewModel = null;
            base.OnDispose();
        }

        private void SelectionChangedExec() => ExecEvent("SelectionChanged");
        private void DropDownOpenedExec() => ExecEvent("DropDownOpened");
        private void DropDownClosedExec() => ExecEvent("DropDownClosed");

        private void ExecEvent(string eventId)
        {
            var eventCmdInfo = EventBindEditModel.EventCmdInfos.FirstOrDefault(info => info.EventID == eventId);
            if (eventCmdInfo != null)
                CallBack?.ExecMapCellEvent(eventCmdInfo.EventCmdKind, eventCmdInfo.CmdID, CommHelper.ToEventParamValueList(eventCmdInfo.CmdParam), out _);
        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            if (CallBack == null || string.IsNullOrWhiteSpace(propertyID)) return;

            if (ComboBoxPropertyModelEdit.IsRuning &&
                !_isReceivingPlatformData && // 如果护盾开启（正在接收平台数据），坚决不上报
                propertyValue != null &&
                propertyID == nameof(ComboBoxPropertyModelEdit.SelectedItem))
            {
                try
                {
                    CallBack.UpdatePropertyValue(new GFBaseTypePropValueList() { new GFBaseTypePropValue(MPPropertyID.Parse(propertyID), propertyValue) });
                }
                catch { }
            }

            CallBack.ExecOprt(propertyID);
        }

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

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            ComboBoxPropertyModelEdit.IsRuning = isRuning;

            // 开启防回音盾
            _isReceivingPlatformData = true;
            try
            {
                if (isRuning && propertyVal != null)
                {
                    if (propertyID == nameof(ComboBoxPropertyBindEditModel.Items))
                    {
                        if (propertyVal.Val is ObjectValue_Json objJson && !string.IsNullOrEmpty(objJson.JsonVal))
                        {
                            var list = JsonObjConvert.FromJSon<RunModeList>(objJson.JsonVal);
                            if (list != null) ComboBoxPropertyModelEdit.Items = list;
                        }
                        return true;
                    }

                    if (propertyID == nameof(ComboBoxPropertyBindEditModel.SelectedItem))
                    {
                        ComboBoxPropertyModelEdit.SelectedItem = propertyVal.ToPrimitiveValue<string>() ?? string.Empty;
                        return true;
                    }
                }

                return base.SetPropertyValue(propertyID, propertyVal, isRuning);
            }
            finally
            {
                // 数据下发并触发完所有本地事件后，关闭防回音盾
                _isReceivingPlatformData = false;
            }
        }

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == ComboBoxMapOprtCellConst.DataGroup_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new DataGroupMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            else if (mapOprtCellInstInfo.OprtCellID == ComboBoxMapOprtCellConst.StyleGroup_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new StyleGroupMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            else if (mapOprtCellInstInfo.OprtCellID == ComboBoxMapOprtCellConst.FontGroup_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new FontGroupMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            else if (mapOprtCellInstInfo.OprtCellID == ComboBoxMapOprtCellConst.LayoutGroup_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new LayoutGroupMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }

            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        #region 操作原子执行对象

        // 1. 数据执行器 (严格提取复杂 JSON 结构)
        private class DataGroupMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellComboBoxCtlObj mapCellComboBoxCtlObj;
            private IMapOprtCellExectorCallBack callBack = null!;

            public DataGroupMapOprtCellExector(MapCellComboBoxCtlObj mapCellComboBoxCtlObj)
            {
                this.mapCellComboBoxCtlObj = mapCellComboBoxCtlObj;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is ComboBoxViewModel vm)
                {
                    RunModeList extractedList = null;
                    GriffinsBaseValue propValue = callBack.GetMapCellPropValue(nameof(ComboBoxPropertyModelEdit.Items));
                    if (propValue != null && propValue.Val is ObjectValue_Json)
                    {
                        extractedList = new RunModeList();
                        ((IGriffinsBaseValue)extractedList).PopulateFromBaseValue(propValue);
                    }

                    PostToUI(() => vm.RefreshData(extractedList));
                }
            }
        }

        // 2. 样式执行器
        private class StyleGroupMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellComboBoxCtlObj mapCellComboBoxCtlObj;
            private IMapOprtCellExectorCallBack callBack = null!;

            public StyleGroupMapOprtCellExector(MapCellComboBoxCtlObj mapCellComboBoxCtlObj)
            {
                this.mapCellComboBoxCtlObj = mapCellComboBoxCtlObj;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is ComboBoxViewModel vm)
                {
                    PostToUI(() => vm.RefreshStyle());
                }
            }
        }

        // 3. 字体执行器
        private class FontGroupMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellComboBoxCtlObj mapCellComboBoxCtlObj;
            private IMapOprtCellExectorCallBack callBack = null!;

            public FontGroupMapOprtCellExector(MapCellComboBoxCtlObj mapCellComboBoxCtlObj)
            {
                this.mapCellComboBoxCtlObj = mapCellComboBoxCtlObj;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is ComboBoxViewModel vm)
                {
                    PostToUI(() => vm.RefreshFont());
                }
            }
        }

        // 4. 布局执行器
        private class LayoutGroupMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellComboBoxCtlObj mapCellComboBoxCtlObj;
            private IMapOprtCellExectorCallBack callBack = null!;

            public LayoutGroupMapOprtCellExector(MapCellComboBoxCtlObj mapCellComboBoxCtlObj)
            {
                this.mapCellComboBoxCtlObj = mapCellComboBoxCtlObj;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is ComboBoxViewModel vm)
                {
                    PostToUI(() => vm.RefreshLayout());
                }
            }
        }

        #endregion

        
        private static void PostToUI(Action action)
        {
            if (Dispatcher.UIThread.CheckAccess()) action();
            else Dispatcher.UIThread.Post(action);
        }

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            string propertyEditJson = br.ReadString("PropertyEditModelBase");
            if (!string.IsNullOrEmpty(propertyEditJson))
            {
                var model = JsonObjConvert.FromJSon<ComboBoxPropertyModelEdit>(propertyEditJson);
                if (model != null) ComboBoxPropertyModelEdit.CopyFrom(model);
            }

            string propertyBindJson = br.ReadString("PropertyBindEditModelBase");
            if (!string.IsNullOrEmpty(propertyBindJson))
            {
                var bindModel = JsonObjConvert.FromJSon<ComboBoxPropertyBindEditModel>(propertyBindJson);
                if (bindModel != null) (PropertyBindEditModelBase as ComboBoxPropertyBindEditModel)?.CopyFrom(bindModel);
            }

            string eventBindJson = br.ReadString("EventBindEditModel");
            if (!string.IsNullOrEmpty(eventBindJson))
            {
                var eventModel = JsonSerializer.Deserialize<EventBindEditModel>(eventBindJson);
                if (eventModel != null) EventBindEditModel.CopyFrom(eventModel);
            }

            Dispatcher.UIThread.Post(() => viewModel?.ReloadFromModel());
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
            base._CopyFrom(source);
            var obj = source as MapCellComboBoxCtlObj;
            if (obj == null) return;

            ComboBoxPropertyModelEdit.CopyFrom(obj.PropertyEditModelBase as ComboBoxPropertyModelEdit);
            if (PropertyBindEditModelBase is ComboBoxPropertyBindEditModel selfBind && obj.PropertyBindEditModelBase is ComboBoxPropertyBindEditModel srcBind)
                selfBind.CopyFrom(srcBind);

            EventBindEditModel.CopyFrom(obj.EventBindEditModel);
            Dispatcher.UIThread.Post(() => viewModel?.ReloadFromModel());
        }

        protected override void OnInit()
        {
            base.OnInit();
            Dispatcher.UIThread.Post(() => viewModel?.ReloadFromModel());
        }

        protected override object OnGetView() => view;
        protected override object OnGetViewModel() => viewModel;
        public override PropertyEditModelBase CreatePropertyModelEditBase() => new ComboBoxPropertyModelEdit();
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new ComboBoxPropertyBindEditModel();

        public override EventBindEditModel CreateEventBindEditModel()
        {
            return new EventBindEditModel()
            {
                EventCmdInfos = new BindingList<EventCmdInfo>()
                {
                    new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = "SelectionChanged" },
                    new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = "DropDownOpened" },
                    new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = "DropDownClosed" },
                }
            };
        }

        public override MapCellPropValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null;
        }
    }
}