using GF_Gereric;
using GKG.Map.CameraViewerFuncCtlMapCell.View;
using GKG.Map.CameraViewerFuncCtlMapCell.ViewModel;
using Griffins;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;
using System;
using System.ComponentModel;
using System.Linq;

namespace GKG.Map.CameraViewerFuncCtlMapCell
{
    /// <summary>
    /// 相机视图图元控制对象
    /// </summary>
    class MapCellCameraViewerCtlObj : FunctionalCellBase
    {
        private CameraViewerView view;
        private CameraViewerViewModel cameraViewerViewModel;

        static MapCellCameraViewerCtlObj()
        {
        }

        public MapCellCameraViewerCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        public MapCellCameraViewerCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            base.SetID(mapCellID);
            base.SetName(mapCellName);
            view = new CameraViewerView();

            //RegisterProperty(new MapObjPropertyInfo(nameof(ProductionInformationPropertyModelEdit.Datas), ResourceA.Datas, GriffinsBaseDataType.Object_Json, ProductionInfoData.Object_ID, typeof(ProductionInfoData), false, true, new GriffinsBaseValue(ProductionInfoData.DefaultEmpty)));

            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), GriffinsBaseDataType.Object_Bytes, GraphMouseEventParam.Object_ID));

            //RegisterOprtCellInfo(new MapOprtCellInfo(ProductionInformationMapOprtCellConst.Datas_MapOprtCellID, ResourceA.Datas_MapOprtCellName));

            //RegisterOprtInfo(new MapOprtInfo(nameof(ProductionInformationPropertyModelEdit.Datas), ResourceA.Datas_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            //{
            //    new MapOprtCellInstInfo()
            //    {
            //        InstanceID = Guid.NewGuid(),
            //        OprtCellID = ProductionInformationMapOprtCellConst.Datas_MapOprtCellID,
            //        CfgInfo = null
            //    }
            //}));

            (this as IMapCellTypeBase).Name = ResourceA.CameraViewer;

            // 实例化 ViewModel
            cameraViewerViewModel = new CameraViewerViewModel(); //new CameraViewerViewModel(CameraViewerPropertyModelEdit, clickExec, () => new MapCmdExector(base.CallBack.INorthSvrCommandExec), PropertyBindEditModelBase.MpNo);
            view.DataContext = cameraViewerViewModel;
        }

        private void clickExec()
        {
            EventCmdInfo? eventCmdInfo = EventBindEditModel.EventCmdInfos.FirstOrDefault(info => info.EventID == MapObjPropEventConst.Event_MouseClick);
            if (eventCmdInfo != null)
            {
                GFBaseTypeParamValueList cmdParam = null;
                if (!string.IsNullOrWhiteSpace(eventCmdInfo.CmdParam))
                {
                    cmdParam = new GFBaseTypeParamValueList();
                    cmdParam.FromJson(eventCmdInfo.CmdParam);
                }
                CallBack?.ExecMapCellEvent(eventCmdInfo.EventCmdKind, eventCmdInfo.CmdID, cmdParam, out GFBaseTypeParamValueList retVal);
            }
        }

        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null!;
        }

        [Browsable(false)]
        public CameraViewerPropertyModelEdit CameraViewerPropertyModelEdit
        {
            get { return (CameraViewerPropertyModelEdit)PropertyEditModelBase; }
        }

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            CameraViewerPropertyModelEdit.IsRuning = isRuning;
            return CameraViewerPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
        }

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            //if (mapOprtCellInstInfo.OprtCellID == CameraViewerMapOprtCellConst.Datas_MapOprtCellID)
            //{
            //    if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
            //    {
            //        mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
            //    }
            //    else
            //    {
            //        mapOprtCellExector = new DatasMapOprtCellExector(this);
            //        mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
            //        MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
            //        mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
            //    }
            //    return true;
            //}

            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        protected override bool SetUIDataObjPropValues(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            foreach (GFBaseTypePropValue gFBaseTypePropValue in gFBaseTypePropValues)
            {
                string propId = gFBaseTypePropValue.PropertyID.ToString();
                GriffinsBaseValue propValue = gFBaseTypePropValue.Value;

                if (propValue == null) return false;

                switch (propId)
                {
                    default:
                        break;
                }
            }

            return true;
        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            //if (propertyID == nameof(ProductionInformationPropertyModelEdit.Datas))
            //{
            //    CallBack?.ExecOprt(nameof(ProductionInformationPropertyModelEdit.Datas));
            //}

            //if (!ProductionInformationPropertyModelEdit.IsRuning)
            //{
            //    GFBaseTypePropValueList gFBaseTypePropValues = new GFBaseTypePropValueList();
            //    CallBack?.UpdateUIDataObjPropValues(gFBaseTypePropValues);
            //}
        }

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            var propertyEditModelBase = JsonObjConvert.FromJSon<CameraViewerPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            (PropertyEditModelBase as CameraViewerPropertyModelEdit).CopyFrom(propertyEditModelBase);
            var propertyBindEditModelBase = JsonObjConvert.FromJSon<CameraViewerPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            (PropertyBindEditModelBase as CameraViewerPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
            var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(br.ReadString("EventBindEditModel"));
            EventBindEditModel.CopyFrom(eventBindEditModel);
        }

        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
        }

        protected override void OnCopyFrom(FunctionalCellBase source)
        {
            MapCellCameraViewerCtlObj mapCellPressureCtlObj = (source as MapCellCameraViewerCtlObj);
            base._CopyFrom(mapCellPressureCtlObj);
            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
            (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel.CopyFrom(source.EventBindEditModel);
        }

        protected override void OnInit() { }

        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => cameraViewerViewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new CameraViewerPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new CameraViewerPropertyBindEditModel();

        public override EventBindEditModel CreateEventBindEditModel()
        {
            return new EventBindEditModel()
            {
                EventCmdInfos = new BindingList<EventCmdInfo>()
                {
                    new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = MapObjPropEventConst.Event_MouseClick },
                }
            };
        }

        public override void OnZoomChanged() { }

        public override string ToString() => "相机视图";

        #region 操作原子执行对象

        //private class DatasMapOprtCellExector : IMapOprtCellExector
        //{
        //    private MapCellCameraViewerCtlObj mapCellProductionInformationCtlObj;
        //    private IMapOprtCellExectorCallBack callBack = null!;

        //    public DatasMapOprtCellExector(MapCellCameraViewerCtlObj mapCellProductionInformationCtlObj)
        //    {
        //        this.mapCellProductionInformationCtlObj = mapCellProductionInformationCtlObj;
        //    }

        //    void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
        //    {
        //        this.callBack = callBack;
        //    }

        //    void IMapOprtCellExector.Exec(byte[] cfg)
        //    {
        //        object viewModel = callBack.GetMapCellVMObjInstance();
        //        if (viewModel != null && viewModel is ProductionInformationViewModel productionInformationViewModel)
        //        {
        //            GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(ProductionInformationPropertyModelEdit.Datas));
        //            if (mapCellPropValue != null)
        //            {
        //                ProductionInfoData productionInfoData = new ProductionInfoData();
        //                ((IGriffinsBaseValue)productionInfoData).PopulateFromBaseValue(mapCellPropValue);
        //                PushDataToViewModel(productionInformationViewModel, productionInfoData);
        //            }
        //        }
        //    }
        //}

        #endregion
    }

    /// <summary>
    /// 相机视图图元属性编辑模型对象
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    public class CameraViewerPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        public CameraViewerPropertyModelEdit()
        {
        }


        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal)
        {
            return base.SetPropertyValue(propertyID, propertyVal);
        }

        public void CopyFrom(CameraViewerPropertyModelEdit source)
        {
            base.CopyFrom(source);
        }
    }

    /// <summary>
    /// 相机视图图元属性绑定编辑模型对象
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class CameraViewerPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        public void CopyFrom(CameraViewerPropertyBindEditModel source)
        {
            base.CopyFrom(source);
        }
    }
}