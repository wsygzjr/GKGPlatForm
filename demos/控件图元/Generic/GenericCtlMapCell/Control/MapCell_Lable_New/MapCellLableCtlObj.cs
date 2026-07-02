using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using GKG.Map.MapCell.Generic;
using GKG.Map.MapCell.Generic.Control.Lable;
using GKG.Map.MapCell.Generic.Control.Lable.View;
using GKG.Map.MapCell.Generic.Control.Lable.ViewModel;
using Griffins;
using PropertyModels.ComponentModel;
using System;
using System.ComponentModel;
using System.Linq;

namespace GKG.Map.MapCell.Generic.Control.Lable
{
    internal class MapCellLableCtlObj : ControlCellBase
    {
        private LableView view;
        private LableViewModel viewModel;

        public MapCellLableCtlObj(MapObjID mapCellID, string mapCellName, bool designTime = false)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);
            base.SetName(mapCellName);

            view = new LableView();
            (this as IMapCellTypeBase).Name = Resource_Lable.Lable;

            viewModel = new LableViewModel(LablePropertyModelEdit);
            view.DataContext = viewModel;

            #region 标签 18 大属性的标准化注册

            // ---------------- [组别 1：数据组] ----------------
            RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.DataGroup_MapOprtCellID, Resource_Lable.Lable_DataGroup_OprtCellName));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.LableText), Resource_Lable.Lable_LableText_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("标签文本")));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.LableText), Resource_Lable.Lable_LableText_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.DataGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.CursorType), Resource_Lable.Lable_CursorType_Name, GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), false, true, new GriffinsBaseValue(0)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.CursorType), Resource_Lable.Lable_CursorType_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.DataGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.IsEnabled), Resource_Lable.Lable_IsEnabled_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(true)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.IsEnabled), Resource_Lable.Lable_IsEnabled_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.DataGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.ToolTip), Resource_Lable.Lable_ToolTip_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue(string.Empty)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.ToolTip), Resource_Lable.Lable_ToolTip_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.DataGroup_MapOprtCellID)));

            // ---------------- [组别 2：样式组] ----------------
            RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.StyleGroup_MapOprtCellID, Resource_Lable.Lable_StyleGroup_OprtCellName));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.BackgroundColor), Resource_Lable.Lable_BackgroundColor_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(Color), false, true, new GriffinsBaseValue(Colors.Transparent.ToString())));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.BackgroundColor), Resource_Lable.Lable_BackgroundColor_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.ForegroundColor), Resource_Lable.Lable_ForegroundColor_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(Color), false, true, new GriffinsBaseValue(Colors.Black.ToString())));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.ForegroundColor), Resource_Lable.Lable_ForegroundColor_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.BorderBrush), Resource_Lable.Lable_BorderBrush_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(Color), false, true, new GriffinsBaseValue(Colors.Transparent.ToString())));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.BorderBrush), Resource_Lable.Lable_BorderBrush_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.BorderThickness), Resource_Lable.Lable_BorderThickness_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("0")));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.BorderThickness), Resource_Lable.Lable_BorderThickness_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.StyleGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.Opacity), Resource_Lable.Lable_Opacity_Name, GriffinsBaseDataType.Decimal, Guid.Empty, typeof(double), false, true, new GriffinsBaseValue((decimal)1.0)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.Opacity), Resource_Lable.Lable_Opacity_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.StyleGroup_MapOprtCellID)));

            // ---------------- [组别 3：字体排版组] ----------------
            RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.FontGroup_MapOprtCellID, Resource_Lable.Lable_FontGroup_OprtCellName));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.FontSize), Resource_Lable.Lable_FontSize_Name, GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), false, true, new GriffinsBaseValue(14)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.FontSize), Resource_Lable.Lable_FontSize_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.FontFamily), Resource_Lable.Lable_FontFamily_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("Microsoft YaHei")));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.FontFamily), Resource_Lable.Lable_FontFamily_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.IsBold), Resource_Lable.Lable_IsBold_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(false)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.IsBold), Resource_Lable.Lable_IsBold_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.IsItalic), Resource_Lable.Lable_IsItalic_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(false)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.IsItalic), Resource_Lable.Lable_IsItalic_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.IsUnderline), Resource_Lable.Lable_IsUnderline_Name, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(false)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.IsUnderline), Resource_Lable.Lable_IsUnderline_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.LineHeight), Resource_Lable.Lable_LineHeight_Name, GriffinsBaseDataType.Decimal, Guid.Empty, typeof(double), false, true, new GriffinsBaseValue((decimal)1.0)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.LineHeight), Resource_Lable.Lable_LineHeight_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.FontGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.TextAlignment), Resource_Lable.Lable_TextAlignment_Name, GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), false, true, new GriffinsBaseValue(0)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.TextAlignment), Resource_Lable.Lable_TextAlignment_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.FontGroup_MapOprtCellID)));

            // ---------------- [组别 4：布局组] ----------------
            RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.LayoutGroup_MapOprtCellID, Resource_Lable.Lable_LayoutGroup_OprtCellName));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.Margin), Resource_Lable.Lable_Margin_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("0")));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.Margin), Resource_Lable.Lable_Margin_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.LayoutGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.Padding), Resource_Lable.Lable_Padding_Name, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, new GriffinsBaseValue("0")));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.Padding), Resource_Lable.Lable_Padding_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.LayoutGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.HorizontalAlign), Resource_Lable.Lable_HorizontalAlign_Name, GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), false, true, new GriffinsBaseValue(0)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.HorizontalAlign), Resource_Lable.Lable_HorizontalAlign_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.LayoutGroup_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.VerticalAlign), Resource_Lable.Lable_VerticalAlign_Name, GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), false, true, new GriffinsBaseValue(0)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.VerticalAlign), Resource_Lable.Lable_VerticalAlign_OprtName, OprtExecKind.Normal, "", CreateAtom(LableMapOprtCellConst.LayoutGroup_MapOprtCellID)));

            #endregion
        }

        private MapOprtCellInstInfoList CreateAtom(MapOprtCellID oprtCellId)
        {
            return new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = oprtCellId, CfgInfo = null } };
        }

        [Browsable(false)]
        public LablePropertyModelEdit LablePropertyModelEdit => PropertyEditModelBase as LablePropertyModelEdit;

        public override void OnDispose()
        {
            view.DataContext = null;
            viewModel?.Dispose();
            viewModel = null;
            base.OnDispose();
        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);
            if (CallBack == null || string.IsNullOrWhiteSpace(propertyID)) return;
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
            LablePropertyModelEdit.IsRuning = isRuning;

            if (isRuning && propertyVal != null)
            {
                if (propertyID == nameof(LablePropertyBindEditModel.LableText))
                {
                    LablePropertyModelEdit.LableText = propertyVal.ToPrimitiveValue<string>() ?? string.Empty;
                    return true;
                }
                if (propertyID == nameof(LablePropertyBindEditModel.ForegroundColor))
                {
                    var colorStr = propertyVal.ToPrimitiveValue<string>();
                    if (Color.TryParse(colorStr, out var c)) LablePropertyModelEdit.ForegroundColor = c;
                    return true;
                }
                if (propertyID == nameof(LablePropertyBindEditModel.BackgroundColor))
                {
                    var colorStr = propertyVal.ToPrimitiveValue<string>();
                    if (Color.TryParse(colorStr, out var c)) LablePropertyModelEdit.BackgroundColor = c;
                    return true;
                }
            }

            return base.SetPropertyValue(propertyID, propertyVal, isRuning);
        }

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.DataGroup_MapOprtCellID)
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
            else if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.StyleGroup_MapOprtCellID)
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
            else if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.FontGroup_MapOprtCellID)
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
            else if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.LayoutGroup_MapOprtCellID)
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

        private class DataGroupMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellLableCtlObj mapCellLableCtlObj;
            private IMapOprtCellExectorCallBack callBack = null!;
            public DataGroupMapOprtCellExector(MapCellLableCtlObj mapCellLableCtlObj) => this.mapCellLableCtlObj = mapCellLableCtlObj;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack cb) => callBack = cb;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is LableViewModel vm) PostToUI(() => vm.RefreshData());
            }
        }

        private class StyleGroupMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellLableCtlObj mapCellLableCtlObj;
            private IMapOprtCellExectorCallBack callBack = null!;
            public StyleGroupMapOprtCellExector(MapCellLableCtlObj mapCellLableCtlObj) => this.mapCellLableCtlObj = mapCellLableCtlObj;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack cb) => callBack = cb;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is LableViewModel vm) PostToUI(() => vm.RefreshStyle());
            }
        }

        private class FontGroupMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellLableCtlObj mapCellLableCtlObj;
            private IMapOprtCellExectorCallBack callBack = null!;
            public FontGroupMapOprtCellExector(MapCellLableCtlObj mapCellLableCtlObj) => this.mapCellLableCtlObj = mapCellLableCtlObj;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack cb) => callBack = cb;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is LableViewModel vm) PostToUI(() => vm.RefreshFont());
            }
        }

        private class LayoutGroupMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellLableCtlObj mapCellLableCtlObj;
            private IMapOprtCellExectorCallBack callBack = null!;
            public LayoutGroupMapOprtCellExector(MapCellLableCtlObj mapCellLableCtlObj) => this.mapCellLableCtlObj = mapCellLableCtlObj;
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack cb) => callBack = cb;
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is LableViewModel vm) PostToUI(() => vm.RefreshLayout());
            }
        }

        private static void PostToUI(Action action)
        {
            if (Dispatcher.UIThread.CheckAccess()) action();
            else Dispatcher.UIThread.Post(action);
        }

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);

            var propertyEditModelBase = JsonObjConvert.FromJSon<LablePropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            (PropertyEditModelBase as LablePropertyModelEdit)!.CopyFrom(propertyEditModelBase);

            var propertyBindEditModelBase = JsonObjConvert.FromJSon<LablePropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            (PropertyBindEditModelBase as LablePropertyBindEditModel)!.CopyFrom(propertyBindEditModelBase);

            var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(br.ReadString("EventBindEditModel"));
            EventBindEditModel.CopyFrom(eventBindEditModel!);

            Dispatcher.UIThread.Post(() => viewModel?.ReloadFromModel());
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
            base._CopyFrom(source);
            var obj = source as MapCellLableCtlObj;
            if (obj == null) return;

            LablePropertyModelEdit.CopyFrom(obj.PropertyEditModelBase as LablePropertyModelEdit);
            if (PropertyBindEditModelBase is LablePropertyBindEditModel selfBind && obj.PropertyBindEditModelBase is LablePropertyBindEditModel srcBind)
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
        public override PropertyEditModelBase CreatePropertyModelEditBase() => new LablePropertyModelEdit();
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new LablePropertyBindEditModel();
        public override EventBindEditModel CreateEventBindEditModel() => new EventBindEditModel() { EventCmdInfos = new BindingList<EventCmdInfo>() };

        public override MapCellPropValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null;
        }
    }
}