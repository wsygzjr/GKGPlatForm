using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using GKG.Map.CommunicationFuncCtlMapCell.View;
using GKG.Map.CommunicationFuncCtlMapCell.ViewModel;
using GKG.Map.OneClickCorrectionFuncCtlMapCell;
using Griffins.Map.UI;
using Griffins.UI2;
using Newtonsoft.JsonG;
using Griffins.Map;
using Griffins;

namespace GKG.Map.CommunicationFuncCtlMapCell
{
    class MapCellCommunicationCtlObj : FunctionalCellBase
    {
        #region 私有字段
        public const string Prop_CommunicationValue = "CommunicationValue";
        private CommunicationView view;
        private CommunicationViewModel communicationViewModel;
        #endregion

        #region 构造函数
        static MapCellCommunicationCtlObj()
        {

        }

        public MapCellCommunicationCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        public MapCellCommunicationCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            base.SetID(mapCellID);
            base.SetName(mapCellName);
            view = new();

            #region 注册属性
            RegisterProperty(new MapObjPropertyInfo(nameof(CommunicationPropertyModelEdit.BackColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Color.FromArgb(1, 0, 0, 0).ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(CommunicationPropertyModelEdit.TextColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_TextColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Colors.Black.ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(CommunicationPropertyModelEdit.TextFont), ResourceA.TextFont, GriffinsBaseDataType.Object_Json, FontInfo.Object_ID, typeof(FontInfo), true, true, new GriffinsBaseValue(FontInfo.DefaultFont)));

            RegisterProperty(new MapObjPropertyInfo(nameof(CommunicationPropertyModelEdit.RaiseTime), ResourceA.ResourceManager.GetString("RaiseTime") ?? "抬起时间(ms)", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(CommunicationPropertyModelEdit.DispenseTime), ResourceA.ResourceManager.GetString("DispenseTime") ?? "点胶时间(ms)", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(CommunicationPropertyModelEdit.ImpactTime), ResourceA.ResourceManager.GetString("ImpactTime") ?? "撞击时间(ms)", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(CommunicationPropertyModelEdit.IntermittentTime), ResourceA.ResourceManager.GetString("IntermittentTime") ?? "间歇时间(ms)", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(CommunicationPropertyModelEdit.VoltageRatio), ResourceA.ResourceManager.GetString("VoltageRatio") ?? "电压比(%)", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(CommunicationPropertyModelEdit.DotCount), ResourceA.ResourceManager.GetString("DotCount") ?? "画点次数", GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(CommunicationPropertyModelEdit.DispenseMode), ResourceA.ResourceManager.GetString("DispenseMode") ?? "点胶模式", GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(CommunicationPropertyModelEdit.AfterStop), ResourceA.ResourceManager.GetString("AfterStop") ?? "后停状态", GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(CommunicationPropertyModelEdit.TotalCount), ResourceA.ResourceManager.GetString("TotalCount") ?? "总次数", GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), true, true, null));

            #endregion

            #region 注册事件
            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), GriffinsBaseDataType.Object_Bytes, GraphMouseEventParam.Object_ID));

            #endregion

            #region 注册操作原子
            RegisterOprtCellInfo(new MapOprtCellInfo(CommunicationMapOprtCellConst.TextColor_MapOprtCellID, ResourceA.TextColor_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(CommunicationMapOprtCellConst.BackColor_MapOprtCellID, ResourceA.BackColor_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(CommunicationMapOprtCellConst.TextFont_MapOprtCellID, ResourceA.TextFont_MapOprtCellName));

            RegisterOprtCellInfo(new MapOprtCellInfo(CommunicationMapOprtCellConst.RaiseTime_MapOprtCellID, ResourceA.ResourceManager.GetString("RaiseTime") ?? "抬起时间(ms)"));
            RegisterOprtCellInfo(new MapOprtCellInfo(CommunicationMapOprtCellConst.DispenseTime_MapOprtCellID, ResourceA.ResourceManager.GetString("DispenseTime") ?? "点胶时间(ms)"));
            RegisterOprtCellInfo(new MapOprtCellInfo(CommunicationMapOprtCellConst.ImpactTime_MapOprtCellID, ResourceA.ResourceManager.GetString("ImpactTime") ?? "撞击时间(ms)"));
            RegisterOprtCellInfo(new MapOprtCellInfo(CommunicationMapOprtCellConst.IntermittentTime_MapOprtCellID, ResourceA.ResourceManager.GetString("IntermittentTime") ?? "间歇时间(ms)"));
            RegisterOprtCellInfo(new MapOprtCellInfo(CommunicationMapOprtCellConst.VoltageRatio_MapOprtCellID, ResourceA.ResourceManager.GetString("VoltageRatio") ?? "电压比(%)"));
            RegisterOprtCellInfo(new MapOprtCellInfo(CommunicationMapOprtCellConst.DotCount_MapOprtCellID, ResourceA.ResourceManager.GetString("DotCount") ?? "画点次数"));
            RegisterOprtCellInfo(new MapOprtCellInfo(CommunicationMapOprtCellConst.DispenseMode_MapOprtCellID, ResourceA.ResourceManager.GetString("DispenseMode") ?? "点胶模式"));
            RegisterOprtCellInfo(new MapOprtCellInfo(CommunicationMapOprtCellConst.AfterStop_MapOprtCellID, ResourceA.ResourceManager.GetString("AfterStop") ?? "后停状态"));
            RegisterOprtCellInfo(new MapOprtCellInfo(CommunicationMapOprtCellConst.TotalCount_MapOprtCellID, ResourceA.ResourceManager.GetString("TotalCount") ?? "总次数"));

            #endregion

            #region 注册操作
            RegisterOprtInfo(new MapOprtInfo(nameof(CommunicationPropertyModelEdit.TextColor), ResourceA.TextColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                  InstanceID=Guid.NewGuid(),
                  OprtCellID=CommunicationMapOprtCellConst.TextColor_MapOprtCellID,
                  CfgInfo=null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(CommunicationPropertyModelEdit.BackColor), ResourceA.BackColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                  InstanceID=Guid.NewGuid(),
                  OprtCellID=CommunicationMapOprtCellConst.BackColor_MapOprtCellID,
                  CfgInfo=null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(CommunicationPropertyModelEdit.TextFont), ResourceA.TextFont_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                  InstanceID=Guid.NewGuid(),
                  OprtCellID=CommunicationMapOprtCellConst.TextFont_MapOprtCellID,
                  CfgInfo=null
                }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(CommunicationPropertyModelEdit.RaiseTime), ResourceA.ResourceManager.GetString("RaiseTime") ?? "抬起时间(ms)", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                    InstanceID = Guid.NewGuid(),
                    OprtCellID = CommunicationMapOprtCellConst.RaiseTime_MapOprtCellID,
                    CfgInfo = null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(CommunicationPropertyModelEdit.DispenseTime), ResourceA.ResourceManager.GetString("DispenseTime") ?? "点胶时间(ms)", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                    InstanceID = Guid.NewGuid(),
                    OprtCellID = CommunicationMapOprtCellConst.DispenseTime_MapOprtCellID,
                    CfgInfo = null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(CommunicationPropertyModelEdit.ImpactTime), ResourceA.ResourceManager.GetString("ImpactTime") ?? "撞击时间(ms)", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                    InstanceID = Guid.NewGuid(),
                    OprtCellID = CommunicationMapOprtCellConst.ImpactTime_MapOprtCellID,
                    CfgInfo = null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(CommunicationPropertyModelEdit.IntermittentTime), ResourceA.ResourceManager.GetString("IntermittentTime") ?? "间歇时间(ms)", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                    InstanceID = Guid.NewGuid(),
                    OprtCellID = CommunicationMapOprtCellConst.IntermittentTime_MapOprtCellID,
                    CfgInfo = null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(CommunicationPropertyModelEdit.VoltageRatio), ResourceA.ResourceManager.GetString("VoltageRatio") ?? "电压比(%)", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                    InstanceID = Guid.NewGuid(),
                    OprtCellID = CommunicationMapOprtCellConst.VoltageRatio_MapOprtCellID,
                    CfgInfo = null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(CommunicationPropertyModelEdit.DotCount), ResourceA.ResourceManager.GetString("DotCount") ?? "画点次数", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                    InstanceID = Guid.NewGuid(),
                    OprtCellID = CommunicationMapOprtCellConst.DotCount_MapOprtCellID,
                    CfgInfo = null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(CommunicationPropertyModelEdit.DispenseMode), ResourceA.ResourceManager.GetString("DispenseMode") ?? "点胶模式", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                    InstanceID = Guid.NewGuid(),
                    OprtCellID = CommunicationMapOprtCellConst.DispenseMode_MapOprtCellID,
                    CfgInfo = null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(CommunicationPropertyModelEdit.AfterStop), ResourceA.ResourceManager.GetString("AfterStop") ?? "后停状态", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                    InstanceID = Guid.NewGuid(),
                    OprtCellID = CommunicationMapOprtCellConst.AfterStop_MapOprtCellID,
                    CfgInfo = null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(CommunicationPropertyModelEdit.TotalCount), ResourceA.ResourceManager.GetString("TotalCount") ?? "总次数", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                    InstanceID = Guid.NewGuid(),
                    OprtCellID = CommunicationMapOprtCellConst.TotalCount_MapOprtCellID,
                    CfgInfo = null
                }
            }));

            #endregion

            #region UI
            (this as IMapCellTypeBase).Name = ResourceA.Communication;
            communicationViewModel = new CommunicationViewModel(CommunicationPropertyModelEdit, communicationEXC);
            SyncModelToViewModel();
            view.DataContext = communicationViewModel;

            #endregion
        }
        #endregion

        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null!;
        }

        #region 私有方法

        private void communicationEXC()
        {
            showCommunication();
            //ExecEventByIdWithNotify(CommunicationMapOprtCellConst.Communication_EventIDStr, "通讯");
        }

        /// <summary>
        /// 打开通讯设置界面
        /// </summary>
        private void showCommunication()
        {

        }

        /// <summary>
        /// 同步模型数据到视图模型
        /// </summary>
        private void SyncModelToViewModel()
        {
            if (communicationViewModel == null)
                return;

            communicationViewModel.TextFont = CommunicationPropertyModelEdit.TextFont;
            communicationViewModel.TextColor = CommunicationPropertyModelEdit.TextColor;
            communicationViewModel.BackColor = CommunicationPropertyModelEdit.BackColor;

            communicationViewModel.RaiseTime = CommunicationPropertyModelEdit.RaiseTime;
            communicationViewModel.DispenseTime = CommunicationPropertyModelEdit.DispenseTime;
            communicationViewModel.ImpactTime = CommunicationPropertyModelEdit.ImpactTime;
            communicationViewModel.IntermittentTime = CommunicationPropertyModelEdit.IntermittentTime;
            communicationViewModel.VoltageRatio = CommunicationPropertyModelEdit.VoltageRatio;
            communicationViewModel.DotCount = CommunicationPropertyModelEdit.DotCount;
            communicationViewModel.DispenseMode = CommunicationPropertyModelEdit.DispenseMode;
            communicationViewModel.AfterStop = CommunicationPropertyModelEdit.AfterStop;
            communicationViewModel.TotalCount = CommunicationPropertyModelEdit.TotalCount;
        }
        #endregion

        #region 属性
        [Browsable(false)]
        public CommunicationPropertyModelEdit CommunicationPropertyModelEdit
        {
            get { return PropertyEditModelBase as CommunicationPropertyModelEdit; }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="propertyID"></param>
        /// <param name="propertyVal"></param>
        /// <param name="isRuning"></param>
        /// <returns></returns>
        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            CommunicationPropertyModelEdit.IsRuning = isRuning;
            var ok = CommunicationPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
            if (ok)
                SyncModelToViewModel();
            return ok;
        }
        #endregion

        #region 执行操作原子
        /// <summary>
        /// 执行图元内部操作原子
        /// </summary>
        /// <param name="mapOprtCellInstInfo">图元内部操作原子信息</param>
        /// <returns>True:已经找到该操作原子并设置，false:没有该操作原子</returns>
        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == CommunicationMapOprtCellConst.TextColor_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new TextColorMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }

            if (mapOprtCellInstInfo.OprtCellID == CommunicationMapOprtCellConst.BackColor_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new BackColorMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }

            if (mapOprtCellInstInfo.OprtCellID == CommunicationMapOprtCellConst.TextFont_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new TextFontMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }

            if (mapOprtCellInstInfo.OprtCellID == CommunicationMapOprtCellConst.RaiseTime_MapOprtCellID)
                return ExecNumericProperty(mapOprtCellInstInfo, nameof(CommunicationPropertyModelEdit.RaiseTime), (vm, v) => vm.RaiseTime = v);

            if (mapOprtCellInstInfo.OprtCellID == CommunicationMapOprtCellConst.DispenseTime_MapOprtCellID)
                return ExecNumericProperty(mapOprtCellInstInfo, nameof(CommunicationPropertyModelEdit.DispenseTime), (vm, v) => vm.DispenseTime = v);

            if (mapOprtCellInstInfo.OprtCellID == CommunicationMapOprtCellConst.ImpactTime_MapOprtCellID)
                return ExecNumericProperty(mapOprtCellInstInfo, nameof(CommunicationPropertyModelEdit.ImpactTime), (vm, v) => vm.ImpactTime = v);

            if (mapOprtCellInstInfo.OprtCellID == CommunicationMapOprtCellConst.IntermittentTime_MapOprtCellID)
                return ExecNumericProperty(mapOprtCellInstInfo, nameof(CommunicationPropertyModelEdit.IntermittentTime), (vm, v) => vm.IntermittentTime = v);

            if (mapOprtCellInstInfo.OprtCellID == CommunicationMapOprtCellConst.VoltageRatio_MapOprtCellID)
                return ExecNumericProperty(mapOprtCellInstInfo, nameof(CommunicationPropertyModelEdit.VoltageRatio), (vm, v) => vm.VoltageRatio = v);

            if (mapOprtCellInstInfo.OprtCellID == CommunicationMapOprtCellConst.DotCount_MapOprtCellID)
                return ExecIntProperty(mapOprtCellInstInfo, nameof(CommunicationPropertyModelEdit.DotCount), (vm, v) => vm.DotCount = v);

            if (mapOprtCellInstInfo.OprtCellID == CommunicationMapOprtCellConst.DispenseMode_MapOprtCellID)
                return ExecIntProperty(mapOprtCellInstInfo, nameof(CommunicationPropertyModelEdit.DispenseMode), (vm, v) => vm.DispenseMode = v);

            if (mapOprtCellInstInfo.OprtCellID == CommunicationMapOprtCellConst.AfterStop_MapOprtCellID)
                return ExecBoolProperty(mapOprtCellInstInfo, nameof(CommunicationPropertyModelEdit.AfterStop), (vm, v) => vm.AfterStop = v);

            if (mapOprtCellInstInfo.OprtCellID == CommunicationMapOprtCellConst.TotalCount_MapOprtCellID)
                return ExecIntProperty(mapOprtCellInstInfo, nameof(CommunicationPropertyModelEdit.TotalCount), (vm, v) => vm.TotalCount = v);

            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        protected override bool SetUIDataObjPropValues(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            foreach (GFBaseTypePropValue gFBaseTypePropValue in gFBaseTypePropValues)
            {
                if (gFBaseTypePropValue == null)
                    continue;

                string propId = gFBaseTypePropValue.PropertyID.ToString();
                if (string.IsNullOrWhiteSpace(propId))
                    continue;

                if (string.Compare(propId, nameof(CommunicationPropertyModelEdit.RaiseTime)) == 0)
                {
                    SetPropertyValue(nameof(CommunicationPropertyModelEdit.RaiseTime), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(CommunicationPropertyModelEdit.DispenseTime)) == 0)
                {
                    SetPropertyValue(nameof(CommunicationPropertyModelEdit.DispenseTime), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(CommunicationPropertyModelEdit.ImpactTime)) == 0)
                {
                    SetPropertyValue(nameof(CommunicationPropertyModelEdit.ImpactTime), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(CommunicationPropertyModelEdit.IntermittentTime)) == 0)
                {
                    SetPropertyValue(nameof(CommunicationPropertyModelEdit.IntermittentTime), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(CommunicationPropertyModelEdit.VoltageRatio)) == 0)
                {
                    SetPropertyValue(nameof(CommunicationPropertyModelEdit.VoltageRatio), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(CommunicationPropertyModelEdit.DotCount)) == 0)
                {
                    SetPropertyValue(nameof(CommunicationPropertyModelEdit.DotCount), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(CommunicationPropertyModelEdit.DispenseMode)) == 0)
                {
                    SetPropertyValue(nameof(CommunicationPropertyModelEdit.DispenseMode), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(CommunicationPropertyModelEdit.AfterStop)) == 0)
                {
                    SetPropertyValue(nameof(CommunicationPropertyModelEdit.AfterStop), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(CommunicationPropertyModelEdit.TotalCount)) == 0)
                {
                    SetPropertyValue(nameof(CommunicationPropertyModelEdit.TotalCount), gFBaseTypePropValue.Value, true);
                    continue;
                }
            }
            return true;
        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            if (string.Compare(propertyID, nameof(CommunicationPropertyModelEdit.TextColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(CommunicationPropertyModelEdit.TextColor));
            }
            if (string.Compare(propertyID, nameof(CommunicationPropertyModelEdit.BackColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(CommunicationPropertyModelEdit.BackColor));
            }
            if (string.Compare(propertyID, nameof(CommunicationPropertyModelEdit.TextFont)) == 0)
            {
                CallBack?.ExecOprt(nameof(CommunicationPropertyModelEdit.TextFont));
            }

            if (string.Compare(propertyID, nameof(CommunicationPropertyModelEdit.RaiseTime)) == 0)
            {
                CallBack?.ExecOprt(nameof(CommunicationPropertyModelEdit.RaiseTime));
            }
            if (string.Compare(propertyID, nameof(CommunicationPropertyModelEdit.DispenseTime)) == 0)
            {
                CallBack?.ExecOprt(nameof(CommunicationPropertyModelEdit.DispenseTime));
            }
            if (string.Compare(propertyID, nameof(CommunicationPropertyModelEdit.ImpactTime)) == 0)
            {
                CallBack?.ExecOprt(nameof(CommunicationPropertyModelEdit.ImpactTime));
            }
            if (string.Compare(propertyID, nameof(CommunicationPropertyModelEdit.IntermittentTime)) == 0)
            {
                CallBack?.ExecOprt(nameof(CommunicationPropertyModelEdit.IntermittentTime));
            }
            if (string.Compare(propertyID, nameof(CommunicationPropertyModelEdit.VoltageRatio)) == 0)
            {
                CallBack?.ExecOprt(nameof(CommunicationPropertyModelEdit.VoltageRatio));
            }
            if (string.Compare(propertyID, nameof(CommunicationPropertyModelEdit.DotCount)) == 0)
            {
                CallBack?.ExecOprt(nameof(CommunicationPropertyModelEdit.DotCount));
            }
            if (string.Compare(propertyID, nameof(CommunicationPropertyModelEdit.DispenseMode)) == 0)
            {
                CallBack?.ExecOprt(nameof(CommunicationPropertyModelEdit.DispenseMode));
            }
            if (string.Compare(propertyID, nameof(CommunicationPropertyModelEdit.AfterStop)) == 0)
            {
                CallBack?.ExecOprt(nameof(CommunicationPropertyModelEdit.AfterStop));
            }
            if (string.Compare(propertyID, nameof(CommunicationPropertyModelEdit.TotalCount)) == 0)
            {
                CallBack?.ExecOprt(nameof(CommunicationPropertyModelEdit.TotalCount));
            }

            if (!CommunicationPropertyModelEdit.IsRuning)
            {
                GFBaseTypePropValueList gFBaseTypePropValues = new GFBaseTypePropValueList();

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(CommunicationPropertyModelEdit.RaiseTime)), new GriffinsBaseValue(CommunicationPropertyModelEdit.RaiseTime)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(CommunicationPropertyModelEdit.DispenseTime)), new GriffinsBaseValue(CommunicationPropertyModelEdit.DispenseTime)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(CommunicationPropertyModelEdit.ImpactTime)), new GriffinsBaseValue(CommunicationPropertyModelEdit.ImpactTime)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(CommunicationPropertyModelEdit.IntermittentTime)), new GriffinsBaseValue(CommunicationPropertyModelEdit.IntermittentTime)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(CommunicationPropertyModelEdit.VoltageRatio)), new GriffinsBaseValue(CommunicationPropertyModelEdit.VoltageRatio)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(CommunicationPropertyModelEdit.DotCount)), new GriffinsBaseValue(CommunicationPropertyModelEdit.DotCount)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(CommunicationPropertyModelEdit.DispenseMode)), new GriffinsBaseValue(CommunicationPropertyModelEdit.DispenseMode)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(CommunicationPropertyModelEdit.AfterStop)), new GriffinsBaseValue(CommunicationPropertyModelEdit.AfterStop)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(CommunicationPropertyModelEdit.TotalCount)), new GriffinsBaseValue(CommunicationPropertyModelEdit.TotalCount)));

                CallBack?.UpdateUIDataObjPropValues(gFBaseTypePropValues);
            }
        }

        private bool ExecNumericProperty(MapOprtCellInstInfo info, string propName, Action<CommunicationViewModel, double> set)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new NumericPropertyMapOprtCellExector(propName, set);
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }

        private bool ExecIntProperty(MapOprtCellInstInfo info, string propName, Action<CommunicationViewModel, int> set)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new IntPropertyMapOprtCellExector(propName, set);
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }

        private bool ExecBoolProperty(MapOprtCellInstInfo info, string propName, Action<CommunicationViewModel, bool> set)
        {
            if (!MapOprtCellExectorDict.TryGetValue(info.InstanceID, out var exector))
            {
                exector = new BoolPropertyMapOprtCellExector(propName, set);
                exector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(info.InstanceID, exector);
            }
            exector.Exec(info.CfgInfo);
            return true;
        }
        #endregion

        #region 序列化
        /// <summary>
        /// 从字节流中读画图信息（必须先调用基类的OnReadDrawInfoFromBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="br">字节流读取对象</param>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            var propertyEditModelBase = JsonObjConvert.FromJSon<CommunicationPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            (PropertyEditModelBase as CommunicationPropertyModelEdit).CopyFrom(propertyEditModelBase);
            var propertyBindEditModelBase = JsonObjConvert.FromJSon<CommunicationPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            (PropertyBindEditModelBase as CommunicationPropertyBindEditModel).CopyFrom((FunctionalCellPropertyBindEditModel)propertyBindEditModelBase);
            var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(br.ReadString("EventBindEditModel"));
            EventBindEditModel.CopyFrom(eventBindEditModel);

            SyncModelToViewModel();
        }

        /// <summary>
        /// 当把画图信息写入到字节流中（必须先调用基类的OnWriteDrawInfoToBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="bw">字节流写入对象</param>
        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
        }
        #endregion

        #region 重写方法
        protected override void OnCopyFrom(FunctionalCellBase source)
        {
            MapCellCommunicationCtlObj mapCellTrackCtlObj = (source as MapCellCommunicationCtlObj);
            base._CopyFrom(mapCellTrackCtlObj);
            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
            (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel.CopyFrom(source.EventBindEditModel);
        }

        /// <summary>
        /// 初始化时
        /// </summary>
        protected override void OnInit()
        {

        }

        protected override object OnGetView()
        {
            return view;
        }

        protected override object OnGetViewModel()
        {
            return communicationViewModel;
        }

        /// <summary>
        /// 创建图元属性编辑模型对象
        /// </summary>
        /// <returns>图元属性编辑模型对象</returns>
        public override PropertyEditModelBase CreatePropertyModelEditBase()
        {
            return new CommunicationPropertyModelEdit();
        }

        /// <summary>
        /// 创建图元属性绑定编辑模型对象
        /// </summary>
        /// <returns>图元属性绑定编辑模型对象</returns>
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
        {
            return new CommunicationPropertyBindEditModel();
        }

        /// <summary>
        /// 创建图元事件绑定编辑模型对象
        /// </summary>
        /// <returns>图元事件绑定编辑模型对象</returns>
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

        public override void OnZoomChanged()
        {
            SetButtonTextFont();
        }

        public override string ToString()
        {
            return "通信";
        }
        #endregion

        #region 内部方法
        internal void SetButtonTextFont()
        {
            if (communicationViewModel == null)
                return;

            double size = base.CallBack?.Calc?.CalcZoomVal((decimal)this.CommunicationPropertyModelEdit.TextFont.FontSize) ?? this.CommunicationPropertyModelEdit.TextFont.FontSize;
            if (size < 2)
                size = 2;
            FontInfo font = new FontInfo(this.CommunicationPropertyModelEdit.TextFont.FontFamily, size, this.CommunicationPropertyModelEdit.TextFont.FontWeight, this.CommunicationPropertyModelEdit.TextFont.FontStyle);
            this.communicationViewModel.TextFont = font;
        }
        #endregion

        #region 操作原子执行对象
        /// <summary>
        /// 文本颜色操作原子执行对象
        /// </summary>
        private class TextColorMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellCommunicationCtlObj mapCellCommunicationCtlObj;

            private IMapOprtCellExectorCallBack callBack;

            public TextColorMapOprtCellExector(MapCellCommunicationCtlObj mapCellTrackCtlObj)
            {
                this.mapCellCommunicationCtlObj = mapCellTrackCtlObj;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }
            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is CommunicationViewModel trackViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(CommunicationPropertyModelEdit.TextColor));
                    if (mapCellPropValue != null)
                    {
                        var color = mapCellPropValue.ToPrimitiveValue<string>();
                        trackViewModel.TextColor = Color.Parse(color);
                    }
                }
            }
        }

        /// <summary>
        /// 背景颜色操作原子执行对象
        /// </summary>
        private class BackColorMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellCommunicationCtlObj mapCellTrackCtlObj;

            private IMapOprtCellExectorCallBack callBack;

            public BackColorMapOprtCellExector(MapCellCommunicationCtlObj mapCellTrackCtlObj)
            {
                this.mapCellTrackCtlObj = mapCellTrackCtlObj;
            }

            #region  IMapOprtCellExector 成员

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is CommunicationViewModel trackViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(CommunicationPropertyModelEdit.BackColor));
                    if (mapCellPropValue != null)
                    {
                        var color = mapCellPropValue.ToPrimitiveValue<string>();
                        trackViewModel.BackColor = Color.Parse(color);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// 文本字体操作原子执行对象
        /// </summary>
        private class TextFontMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellCommunicationCtlObj mapCommunicationTrackCtlObj;

            private IMapOprtCellExectorCallBack callBack;

            public TextFontMapOprtCellExector(MapCellCommunicationCtlObj mapCellTrackCtlObj)
            {
                this.mapCommunicationTrackCtlObj = mapCellTrackCtlObj;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is CommunicationViewModel replaceGluePositionViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(CommunicationPropertyModelEdit.TextFont));
                    if (mapCellPropValue != null)
                    {
                        ObjectValue_Json objectValue_Json = mapCellPropValue.ToObjectValue_Json();
                        GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                        IGriffinsBaseValue iMPPropObjectValue = new FontInfo();
                        iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                        replaceGluePositionViewModel.TextFont = (FontInfo)iMPPropObjectValue;
                    }
                }
            }
        }

        private sealed class NumericPropertyMapOprtCellExector : IMapOprtCellExector
        {
            private readonly string _propName;
            private readonly Action<CommunicationViewModel, double> _set;
            private IMapOprtCellExectorCallBack _callBack;

            public NumericPropertyMapOprtCellExector(string propName, Action<CommunicationViewModel, double> set)
            {
                _propName = propName;
                _set = set;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => _callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                var vmObj = _callBack.GetMapCellVMObjInstance();
                if (vmObj is not CommunicationViewModel vm)
                    return;

                var v = _callBack.GetMapCellPropValue(_propName);
                if (v == null)
                    return;

                if (double.TryParse(v.ToPrimitiveValue<string>(), out var d))
                    _set(vm, d);
            }
        }

        private sealed class IntPropertyMapOprtCellExector : IMapOprtCellExector
        {
            private readonly string _propName;
            private readonly Action<CommunicationViewModel, int> _set;
            private IMapOprtCellExectorCallBack _callBack;

            public IntPropertyMapOprtCellExector(string propName, Action<CommunicationViewModel, int> set)
            {
                _propName = propName;
                _set = set;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => _callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                var vmObj = _callBack.GetMapCellVMObjInstance();
                if (vmObj is not CommunicationViewModel vm)
                    return;

                var v = _callBack.GetMapCellPropValue(_propName);
                if (v == null)
                    return;

                if (int.TryParse(v.ToPrimitiveValue<string>(), out var i))
                    _set(vm, i);
            }
        }

        private sealed class BoolPropertyMapOprtCellExector : IMapOprtCellExector
        {
            private readonly string _propName;
            private readonly Action<CommunicationViewModel, bool> _set;
            private IMapOprtCellExectorCallBack _callBack;

            public BoolPropertyMapOprtCellExector(string propName, Action<CommunicationViewModel, bool> set)
            {
                _propName = propName;
                _set = set;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => _callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                var vmObj = _callBack.GetMapCellVMObjInstance();
                if (vmObj is not CommunicationViewModel vm)
                    return;

                var v = _callBack.GetMapCellPropValue(_propName);
                if (v == null)
                    return;

                // 由于一些变量系统里 bool 可能用 0/1 或 true/false 存储，这里做兼容
                var s = v.ToPrimitiveValue<string>();
                if (bool.TryParse(s, out var b))
                {
                    _set(vm, b);
                    return;
                }

                if (int.TryParse(s, out var i))
                    _set(vm, i != 0);
            }
        }
        #endregion
    }

    #region 模型对象类(与属性面板交互)
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    public class CommunicationPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        public CommunicationPropertyModelEdit()
        {
            TextFont.PropertyChanged += textFont_PropertyChanged;
        }

        private void textFont_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(TextFont));
        }

        private FontInfo _textFont = new FontInfo(FontInfo.DefaultFont.FontFamily, 16, FontInfo.DefaultFont.FontWeight, FontInfo.DefaultFont.FontStyle);

        [DisplayName("文字字体")]
        [Category("图元信息")]
        [PropertySortOrder(8)]
        public FontInfo TextFont
        {
            get => _textFont;
            set => SetProperty(ref _textFont, value);
        }

        private Color _textColor = Colors.Black;

        [DisplayName("文本颜色")]
        [Category("图元信息")]
        [PropertySortOrder(9)]
        [JsonConverter(typeof(ColorConvert))]
        public Color TextColor
        {
            get => _textColor;
            set => SetProperty(ref _textColor, value);
        }

        private Color _backColor = Colors.White;

        [DisplayName("背景颜色")]
        [Category("图元信息")]
        [PropertySortOrder(10)]
        [JsonConverter(typeof(ColorConvert))]
        public Color BackColor
        {
            get => _backColor;
            set => SetProperty(ref _backColor, value);
        }

        private double _raiseTime = 33;

        [DisplayName("抬起时间(ms)")]
        [Category("图元信息")]
        [PropertySortOrder(20)]
        public double RaiseTime
        {
            get => _raiseTime;
            set => SetProperty(ref _raiseTime, value);
        }

        private double _dispenseTime = 44;

        [DisplayName("点胶时间(ms)")]
        [Category("图元信息")]
        [PropertySortOrder(21)]
        public double DispenseTime
        {
            get => _dispenseTime;
            set => SetProperty(ref _dispenseTime, value);
        }

        private double _impactTime = 0.2;

        [DisplayName("撞击时间(ms)")]
        [Category("图元信息")]
        [PropertySortOrder(22)]
        public double ImpactTime
        {
            get => _impactTime;
            set => SetProperty(ref _impactTime, value);
        }

        private double _intermittentTime = 1;

        [DisplayName("间歇时间(ms)")]
        [Category("图元信息")]
        [PropertySortOrder(23)]
        public double IntermittentTime
        {
            get => _intermittentTime;
            set => SetProperty(ref _intermittentTime, value);
        }

        private double _voltageRatio = 50;

        [DisplayName("电压比(%)")]
        [Category("图元信息")]
        [PropertySortOrder(24)]
        public double VoltageRatio
        {
            get => _voltageRatio;
            set => SetProperty(ref _voltageRatio, value);
        }

        private int _dotCount = 1;

        [DisplayName("画点次数")]
        [Category("图元信息")]
        [PropertySortOrder(25)]
        public int DotCount
        {
            get => _dotCount;
            set => SetProperty(ref _dotCount, value);
        }

        private int _dispenseMode = 3;

        [DisplayName("点胶模式")]
        [Category("图元信息")]
        [PropertySortOrder(26)]
        public int DispenseMode
        {
            get => _dispenseMode;
            set => SetProperty(ref _dispenseMode, value);
        }

        private bool _afterStop = true;

        [DisplayName("后停状态")]
        [Category("图元信息")]
        [PropertySortOrder(27)]
        public bool AfterStop
        {
            get => _afterStop;
            set => SetProperty(ref _afterStop, value);
        }

        private int _totalCount = 1;

        [DisplayName("总次数")]
        [Category("图元信息")]
        [PropertySortOrder(28)]
        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(BackColor), StringComparison.Ordinal) == 0)
            {
                if (propertyVal == null)
                {
                    BackColor = Color.FromArgb(33, 0, 0, 0);
                }
                else
                {
                    var color1 = propertyVal.ToPrimitiveValue<string>();
                    BackColor = Color.Parse(color1);
                }
                return true;
            }
            if (string.Compare(propertyID, nameof(TextColor), StringComparison.Ordinal) == 0)
            {
                if (propertyVal == null)
                {
                    TextColor = Colors.Black;
                }
                else
                {
                    var color1 = propertyVal.ToPrimitiveValue<string>();
                    TextColor = Color.Parse(color1);
                }
                return true;
            }
            if (string.Compare(propertyID, nameof(TextFont), StringComparison.Ordinal) == 0)
            {
                if (propertyVal == null)
                {
                    TextFont = FontInfo.DefaultFont;
                }
                else
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new FontInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    TextFont = (FontInfo)iMPPropObjectValue;
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(RaiseTime), StringComparison.Ordinal) == 0)
            {
                RaiseTime = propertyVal == null ? 33 : propertyVal.ToPrimitiveValue<double>();
                return true;
            }
            if (string.Compare(propertyID, nameof(DispenseTime), StringComparison.Ordinal) == 0)
            {
                DispenseTime = propertyVal == null ? 44 : propertyVal.ToPrimitiveValue<double>();
                return true;
            }
            if (string.Compare(propertyID, nameof(ImpactTime), StringComparison.Ordinal) == 0)
            {
                ImpactTime = propertyVal == null ? 0.2 : propertyVal.ToPrimitiveValue<double>();
                return true;
            }
            if (string.Compare(propertyID, nameof(IntermittentTime), StringComparison.Ordinal) == 0)
            {
                IntermittentTime = propertyVal == null ? 1 : propertyVal.ToPrimitiveValue<double>();
                return true;
            }
            if (string.Compare(propertyID, nameof(VoltageRatio), StringComparison.Ordinal) == 0)
            {
                VoltageRatio = propertyVal == null ? 50 : propertyVal.ToPrimitiveValue<double>();
                return true;
            }
            if (string.Compare(propertyID, nameof(DotCount), StringComparison.Ordinal) == 0)
            {
                DotCount = propertyVal == null ? 1 : propertyVal.ToPrimitiveValue<int>();
                return true;
            }
            if (string.Compare(propertyID, nameof(DispenseMode), StringComparison.Ordinal) == 0)
            {
                DispenseMode = propertyVal == null ? 3 : propertyVal.ToPrimitiveValue<int>();
                return true;
            }
            if (string.Compare(propertyID, nameof(AfterStop), StringComparison.Ordinal) == 0)
            {
                AfterStop = propertyVal == null || propertyVal.ToPrimitiveValue<bool>();
                return true;
            }
            if (string.Compare(propertyID, nameof(TotalCount), StringComparison.Ordinal) == 0)
            {
                TotalCount = propertyVal == null ? 1 : propertyVal.ToPrimitiveValue<int>();
                return true;
            }

            return base.SetPropertyValue(propertyID, propertyVal);
        }

        public void CopyFrom(CommunicationPropertyModelEdit source)
        {
            base.CopyFrom(source);
            if (source.TextFont != null)
            {
                TextFont = new FontInfo(source.TextFont.FontFamily, source.TextFont.FontSize, source.TextFont.FontWeight, source.TextFont.FontStyle);
            }
            TextColor = source.TextColor;
            BackColor = source.BackColor;
            RaiseTime = source.RaiseTime;
            DispenseTime = source.DispenseTime;
            ImpactTime = source.ImpactTime;
            IntermittentTime = source.IntermittentTime;
            VoltageRatio = source.VoltageRatio;
            DotCount = source.DotCount;
            DispenseMode = source.DispenseMode;
            AfterStop = source.AfterStop;
            TotalCount = source.TotalCount;
        }
    }

    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class CommunicationPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        private string _raiseTime = nameof(CommunicationPropertyModelEdit.RaiseTime);

        [DisplayName("抬起时间(ms)")]
        [Category("绑定信息")]
        [PropertySortOrder(1)]
        [Browsable(false)]
        public string RaiseTime
        {
            get => _raiseTime;
            set => SetProperty(ref _raiseTime, value);
        }

        private string _dispenseTime = nameof(CommunicationPropertyModelEdit.DispenseTime);

        [DisplayName("点胶时间(ms)")]
        [Category("绑定信息")]
        [PropertySortOrder(2)]
        [Browsable(false)]
        public string DispenseTime
        {
            get => _dispenseTime;
            set => SetProperty(ref _dispenseTime, value);
        }

        private string _impactTime = nameof(CommunicationPropertyModelEdit.ImpactTime);

        [DisplayName("撞击时间(ms)")]
        [Category("绑定信息")]
        [PropertySortOrder(3)]
        [Browsable(false)]
        public string ImpactTime
        {
            get => _impactTime;
            set => SetProperty(ref _impactTime, value);
        }

        private string _intermittentTime = nameof(CommunicationPropertyModelEdit.IntermittentTime);

        [DisplayName("间歇时间(ms)")]
        [Category("绑定信息")]
        [PropertySortOrder(4)]
        [Browsable(false)]
        public string IntermittentTime
        {
            get => _intermittentTime;
            set => SetProperty(ref _intermittentTime, value);
        }

        private string _voltageRatio = nameof(CommunicationPropertyModelEdit.VoltageRatio);

        [DisplayName("电压比(%)")]
        [Category("绑定信息")]
        [PropertySortOrder(5)]
        [Browsable(false)]
        public string VoltageRatio
        {
            get => _voltageRatio;
            set => SetProperty(ref _voltageRatio, value);
        }

        private string _dotCount = nameof(CommunicationPropertyModelEdit.DotCount);

        [DisplayName("画点次数")]
        [Category("绑定信息")]
        [PropertySortOrder(6)]
        [Browsable(false)]
        public string DotCount
        {
            get => _dotCount;
            set => SetProperty(ref _dotCount, value);
        }

        private string _dispenseMode = nameof(CommunicationPropertyModelEdit.DispenseMode);

        [DisplayName("点胶模式")]
        [Category("绑定信息")]
        [PropertySortOrder(7)]
        [Browsable(false)]
        public string DispenseMode
        {
            get => _dispenseMode;
            set => SetProperty(ref _dispenseMode, value);
        }

        private string _afterStop = nameof(CommunicationPropertyModelEdit.AfterStop);

        [DisplayName("后停状态")]
        [Category("绑定信息")]
        [PropertySortOrder(8)]
        [Browsable(false)]
        public string AfterStop
        {
            get => _afterStop;
            set => SetProperty(ref _afterStop, value);
        }

        private string _totalCount = nameof(CommunicationPropertyModelEdit.TotalCount);

        [DisplayName("总次数")]
        [Category("绑定信息")]
        [PropertySortOrder(9)]
        [Browsable(false)]
        public string TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        private string _textFont = "TextFont";

        [DisplayName("文本字体")]
        [Category("绑定信息")]
        [PropertySortOrder(7)]
        [Browsable(false)]
        public string TextFont
        {
            get => _textFont;
            set => SetProperty(ref _textFont, value);
        }

        private string _textColor = "TextColor";

        [DisplayName("文本颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(8)]
        [Browsable(false)]
        public string TextColor
        {
            get => _textColor;
            set => SetProperty(ref _textColor, value);
        }

        private string _backColor = "BackColor";

        [DisplayName("背景颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(9)]
        [Browsable(false)]
        public string BackColor
        {
            get => _backColor;
            set => SetProperty(ref _backColor, value);
        }

        public void CopyFrom(CommunicationPropertyBindEditModel source)
        {
            base.CopyFrom(source);
            RaiseTime = source.RaiseTime;
            DispenseTime = source.DispenseTime;
            ImpactTime = source.ImpactTime;
            IntermittentTime = source.IntermittentTime;
            VoltageRatio = source.VoltageRatio;
            DotCount = source.DotCount;
            DispenseMode = source.DispenseMode;
            AfterStop = source.AfterStop;
            TotalCount = source.TotalCount;
            TextFont = source.TextFont;
            TextColor = source.TextColor;
            BackColor = source.BackColor;
        }
    }
    #endregion
}
