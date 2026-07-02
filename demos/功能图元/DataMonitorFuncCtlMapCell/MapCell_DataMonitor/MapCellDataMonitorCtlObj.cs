using Avalonia.Media;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using GKG.Map.DataMonitorFuncCtlMapCell.ViewModel;
using GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.Objects;
using Griffins.Map.UI;
using Griffins.UI2;
using Newtonsoft.JsonG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Griffins.Map;
using GKG.Map.DataMonitorFuncCtlMapCell.View;

namespace GKG.Map.DataMonitorFuncCtlMapCell
{
    /// <summary>
    /// 数据监控图元控制对象类
    /// 负责管理数据监控图元的所有功能，包括属性注册、事件处理、操作执行等
    /// </summary>
    public class MapCellDataMonitorCtlObj : FunctionalCellBase
    {

        #region 私有字段
        private DataMonitorView view;
        private DataMonitorViewModel viewModel;
        #endregion

        #region 构造函数
        public MapCellDataMonitorCtlObj()
        {
        }

        /// <summary>
        /// 初始化数据监控图元控制对象（运行时）
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        public MapCellDataMonitorCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        /// <summary>
        /// 初始化数据监控图元控制对象
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <param name="designTime">是否为设计时</param>
        public MapCellDataMonitorCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            base.SetID(mapCellID);
            base.SetName(mapCellName);

            view = new DataMonitorView();

            #region 注册基础属性（字体、颜色）
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.BackColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Color.FromArgb(1, 0, 0, 0).ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.TextColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_TextColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Colors.Black.ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.TextFont), ResourceA.TextFont, GriffinsBaseDataType.Object_Json, FontInfo.Object_ID, typeof(FontInfo), true, true, new GriffinsBaseValue(FontInfo.DefaultFont)));
            #endregion

            #region 注册供阀气压设备状态栏属性
            var supplyValvePressureDeviceStatus = new DeviceStatusCommonInfo();
            supplyValvePressureDeviceStatus.StatusName = "供阀气压";
            supplyValvePressureDeviceStatus.ImageSources = new List<BitmapData>
            {
                TryLoadBitmap("avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/Water.png"),
                TryLoadBitmap("avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/Close.png")
            };
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus), ResourceA.SupplyValvePressure, GriffinsBaseDataType.Object_Json, DeviceStatusCommonInfo.Object_ID, typeof(DeviceStatusCommonInfo), true, true, new GriffinsBaseValue(supplyValvePressureDeviceStatus)));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusSwitch), "供阀气压栏状态切换", GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), true, true, new GriffinsBaseValue(true)));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusName), "供阀气压栏状态名称", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("供阀气压")));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusValue), "供阀气压值", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("0.000")));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusUnit), "供阀气压单位", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("kgf/cm²")));
            #endregion

            #region 注册供胶气压设备状态栏属性
            var supplyGluePressureDeviceStatus = new DeviceStatusCommonInfo();
            supplyGluePressureDeviceStatus.StatusName = "供胶气压";
            supplyGluePressureDeviceStatus.ImageSources = new List<BitmapData>
            {
                TryLoadBitmap("avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/Water.png"),
                TryLoadBitmap("avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/Close.png")
            };
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus), ResourceA.SupplyGluePressure, GriffinsBaseDataType.Object_Json, DeviceStatusCommonInfo.Object_ID, typeof(DeviceStatusCommonInfo), true, true, new GriffinsBaseValue(supplyGluePressureDeviceStatus)));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusSwitch), "供胶气压栏状态切换", GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), true, true, new GriffinsBaseValue(true)));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusName), "供胶气压栏状态名称", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("供胶气压")));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusValue), "供胶气压值", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("0.000")));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusUnit), "供胶气压单位", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("kgf/cm²")));
            #endregion

            #region 注册喷嘴加热设备状态栏属性
            var nozzleHeatingDeviceStatus = new DeviceStatusCommonInfo();
            nozzleHeatingDeviceStatus.StatusName = "喷嘴加热";
            nozzleHeatingDeviceStatus.ImageSources = new List<BitmapData>
            {
                TryLoadBitmap("avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/Water.png"),
                TryLoadBitmap("avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/Close.png")
            };
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus), ResourceA.NozzleHeating, GriffinsBaseDataType.Object_Json, DeviceStatusCommonInfo.Object_ID, typeof(DeviceStatusCommonInfo), true, true, new GriffinsBaseValue(nozzleHeatingDeviceStatus)));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusSwitch), "喷嘴加热栏状态切换", GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), true, true, new GriffinsBaseValue(true)));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusName), "喷嘴加热栏状态名称", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("喷嘴加热")));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusValue), "喷嘴加热温度值", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("0.0")));
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusUnit), "喷嘴加热单位", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("°C")));
            #endregion

            #region 注册监控信息栏属性
            // 安全门状态
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SafetyDoorStatus), "安全门状态", GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), true, true, new GriffinsBaseValue(true)));

            // 总气压状态
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.TotalPressureStatus), "总气压状态", GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), true, true, new GriffinsBaseValue(true)));

            // 清洁布状态
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.CleaningClothStatus), "清洁布状态", GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), true, true, new GriffinsBaseValue(true)));

            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.IsDualValve), "双阀模式", GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), true, true, new GriffinsBaseValue(false)));

            // 胶水余量
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.GlueRemaining), "胶水余量", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("50%")));

            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.LeftGlueRemaining), "左阀胶水余量", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("50%")));

            // 校正
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.Calibration), "校正", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("**h|**mg|**pcs")));

            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.LeftCalibration), "左阀校正", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("**h|**mg|**pcs")));

            // 阀体值
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.ValveBodyValue), "阀体值", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("**h")));

            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.LeftValveBodyValue), "左阀阀体值", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("**h")));

            // 阀体图标
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.ValveBodyIcon), "阀体图标", GriffinsBaseDataType.Object_Bytes, BitmapData.Object_ID, typeof(BitmapData), true, true, new GriffinsBaseValue(new BitmapData())));

            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.LeftValveBodyIcon), "左阀阀体图标", GriffinsBaseDataType.Object_Bytes, BitmapData.Object_ID, typeof(BitmapData), true, true, new GriffinsBaseValue(new BitmapData())));

            // 密封圈值
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SealingRingValue), "密封圈值", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("**h")));

            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.LeftSealingRingValue), "左阀密封圈值", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, new GriffinsBaseValue("**h")));

            // 密封圈图标
            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.SealingRingIcon), "密封圈图标", GriffinsBaseDataType.Object_Bytes, BitmapData.Object_ID, typeof(BitmapData), true, true, new GriffinsBaseValue(new BitmapData())));

            RegisterProperty(new MapObjPropertyInfo(nameof(DataMonitorPropertyModelEdit.LeftSealingRingIcon), "左阀密封圈图标", GriffinsBaseDataType.Object_Bytes, BitmapData.Object_ID, typeof(BitmapData), true, true, new GriffinsBaseValue(new BitmapData())));
            #endregion

            #region 注册操作原子信息
            // 注册带参数配置视图的操作原子
            RegisterOprtCellInfo(new MapOprtCellInfo(DataMonitorMapOprtCellConst.TextColor_MapOprtCellID, ResourceA.TextColor, typeof(MapCell_DataMonitor.MapOprtCellParamCfgView.ColorPropertyMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(DataMonitorMapOprtCellConst.BackColor_MapOprtCellID, ResourceA.BackColor, typeof(MapCell_DataMonitor.MapOprtCellParamCfgView.ColorPropertyMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(DataMonitorMapOprtCellConst.TextFont_MapOprtCellID, ResourceA.TextFont, typeof(MapCell_DataMonitor.MapOprtCellParamCfgView.FontPropertyMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(DataMonitorMapOprtCellConst.SupplyValvePressure_MapOprtCellID, "供阀气压", typeof(MapCell_DataMonitor.MapOprtCellParamCfgView.SupplyValvePressureMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(DataMonitorMapOprtCellConst.SupplyGluePressure_MapOprtCellID, "供胶气压", typeof(MapCell_DataMonitor.MapOprtCellParamCfgView.SupplyGluePressureMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(DataMonitorMapOprtCellConst.NozzleHeating_MapOprtCellID, "喷嘴加热", typeof(MapCell_DataMonitor.MapOprtCellParamCfgView.NozzleHeatingMapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(DataMonitorMapOprtCellConst.SystemStatus_MapOprtCellID, "监控信息", typeof(MapCell_DataMonitor.MapOprtCellParamCfgView.SystemStatusMapOprtCellParamCfgView)));

            // 供阀气压设备状态栏操作信息
            RegisterOprtInfo(new MapOprtInfo(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus), ResourceA.SupplyValvePressure, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DataMonitorMapOprtCellConst.SupplyValvePressure_MapOprtCellID, CfgInfo = null }
            }));

            // 供胶气压设备状态栏操作信息
            RegisterOprtInfo(new MapOprtInfo(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus), ResourceA.SupplyGluePressure, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DataMonitorMapOprtCellConst.SupplyGluePressure_MapOprtCellID, CfgInfo = null }
            }));

            // 喷嘴加热设备状态栏操作信息
            RegisterOprtInfo(new MapOprtInfo(nameof(DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus), ResourceA.NozzleHeating, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DataMonitorMapOprtCellConst.NozzleHeating_MapOprtCellID, CfgInfo = null }
            }));

            // 基础属性操作信息
            RegisterOprtInfo(new MapOprtInfo(nameof(DataMonitorPropertyModelEdit.TextColor), ResourceA.TextColor, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DataMonitorMapOprtCellConst.TextColor_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(DataMonitorPropertyModelEdit.BackColor), ResourceA.BackColor, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DataMonitorMapOprtCellConst.BackColor_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(DataMonitorPropertyModelEdit.TextFont), ResourceA.TextFont, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DataMonitorMapOprtCellConst.TextFont_MapOprtCellID, CfgInfo = null }
            }));

            var monitoringDefaultCfg = MapCell_DataMonitor.MapOprtCellParamCfgView.DataMonitorOprtCellCfgSerializer.ToBytes(
                new MapCell_DataMonitor.MapOprtCellParamCfgView.SystemStatusMapOprtCellParamViewModel());
            RegisterOprtInfo(new MapOprtInfo("MonitoringInfo", "监控信息", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DataMonitorMapOprtCellConst.SystemStatus_MapOprtCellID, CfgInfo = monitoringDefaultCfg }
            }));
            RegisterOprtInfo(new MapOprtInfo("MonitoringInfoRefresh", "监控信息", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = DataMonitorMapOprtCellConst.SystemStatus_MapOprtCellID, CfgInfo = null }
            }));
            #endregion

            #region 初始化视图和视图模型
            (this as IMapCellTypeBase).Name = ResourceA.DataMonitor;
            viewModel = new DataMonitorViewModel(DataMonitorPropertyModelEdit, clickExec);
            view.DataContext = viewModel;
            #endregion
        }
        #endregion

        private void clickExec()
        {
            EventCmdInfo? eventCmdInfo = EventBindEditModel.EventCmdInfos.FirstOrDefault
                (info => info.EventID == MapObjPropEventConst.Event_MouseClick);
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

        #region 属性
        /// <summary>
        /// 获取数据监控属性编辑模型
        /// </summary>
        [Browsable(false)]
        public DataMonitorPropertyModelEdit DataMonitorPropertyModelEdit
        {
            get { return PropertyEditModelBase as DataMonitorPropertyModelEdit; }
        }

        /// <summary>
        /// 获取数据监控属性绑定编辑模型
        /// </summary>
        [Browsable(false)]
        public DataMonitorPropertyBindEditModel DataMonitorPropertyBindEditModel
        {
            get { return PropertyBindEditModelBase as DataMonitorPropertyBindEditModel; }
        }
        #endregion

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            DataMonitorPropertyModelEdit.IsRuning = isRuning;
            return DataMonitorPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
        }

        #region 私有方法
        /// <summary>
        /// 同步模型数据到视图模型
        /// </summary>
        private void SyncModelToViewModel()
        {
           viewModel.TextFont = DataMonitorPropertyModelEdit.TextFont;
           viewModel.TextColor = DataMonitorPropertyModelEdit.TextColor;
           viewModel.BackColor = DataMonitorPropertyModelEdit.BackColor;

           viewModel.SupplyValvePressureDeviceStatus = DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus;
           viewModel.SupplyGluePressureDeviceStatus = DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus;
           viewModel.NozzleHeatingDeviceStatus = DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus;

           viewModel.SafetyDoorStatus = DataMonitorPropertyModelEdit.SafetyDoorStatus;
           viewModel.TotalPressureStatus = DataMonitorPropertyModelEdit.TotalPressureStatus;
           viewModel.CleaningClothStatus = DataMonitorPropertyModelEdit.CleaningClothStatus;
           viewModel.IsDualValve = DataMonitorPropertyModelEdit.IsDualValve;
           viewModel.GlueRemaining = DataMonitorPropertyModelEdit.GlueRemaining;
           viewModel.LeftGlueRemaining = DataMonitorPropertyModelEdit.LeftGlueRemaining;
           viewModel.Calibration = DataMonitorPropertyModelEdit.Calibration;
           viewModel.LeftCalibration = DataMonitorPropertyModelEdit.LeftCalibration;
           viewModel.ValveBodyValue = DataMonitorPropertyModelEdit.ValveBodyValue;
           viewModel.ValveBodyIcon = DataMonitorPropertyModelEdit.ValveBodyIcon;
           viewModel.LeftValveBodyValue = DataMonitorPropertyModelEdit.LeftValveBodyValue;
           viewModel.LeftValveBodyIcon = DataMonitorPropertyModelEdit.LeftValveBodyIcon;
           viewModel.SealingRingValue = DataMonitorPropertyModelEdit.SealingRingValue;
           viewModel.SealingRingIcon = DataMonitorPropertyModelEdit.SealingRingIcon;
           viewModel.LeftSealingRingValue = DataMonitorPropertyModelEdit.LeftSealingRingValue;
           viewModel.LeftSealingRingIcon = DataMonitorPropertyModelEdit.LeftSealingRingIcon;
        }

        /// <summary>
        /// 执行操作原子
        /// </summary>
        /// <param name="mapOprtCellInstInfo">操作原子实例信息</param>
        /// <returns>是否执行成功</returns>
        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == DataMonitorMapOprtCellConst.TextColor_MapOprtCellID)
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
            if (mapOprtCellInstInfo.OprtCellID == DataMonitorMapOprtCellConst.BackColor_MapOprtCellID)
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
            if (mapOprtCellInstInfo.OprtCellID == DataMonitorMapOprtCellConst.TextFont_MapOprtCellID)
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
            if (mapOprtCellInstInfo.OprtCellID == DataMonitorMapOprtCellConst.SupplyValvePressure_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new SupplyValvePressureMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == DataMonitorMapOprtCellConst.SupplyGluePressure_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new SupplyGluePressureMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == DataMonitorMapOprtCellConst.NozzleHeating_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new NozzleHeatingMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == DataMonitorMapOprtCellConst.SystemStatus_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new SystemStatusMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }

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

                int dotIndex = propId.LastIndexOf('.');
                if (dotIndex >= 0 && dotIndex < propId.Length - 1)
                    propId = propId.Substring(dotIndex + 1);

                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusSwitch)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusSwitch), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusName)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusName), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusValue)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusValue), gFBaseTypePropValue.Value, true);
                    continue;
                }

                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusUnit)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusUnit), gFBaseTypePropValue.Value, true);
                    continue;
                }

                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusSwitch)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusSwitch), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusName)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusName), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusValue)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusValue), gFBaseTypePropValue.Value, true);
                    continue;
                }

                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusUnit)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusUnit), gFBaseTypePropValue.Value, true);
                    continue;
                }

                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusSwitch)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusSwitch), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusName)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusName), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusValue)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusValue), gFBaseTypePropValue.Value, true);
                    continue;
                }

                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusUnit)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusUnit), gFBaseTypePropValue.Value, true);
                    continue;
                }

                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.SafetyDoorStatus)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.SafetyDoorStatus), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.TotalPressureStatus)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.TotalPressureStatus), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.CleaningClothStatus)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.CleaningClothStatus), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.IsDualValve)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.IsDualValve), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.GlueRemaining)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.GlueRemaining), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.LeftGlueRemaining)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.LeftGlueRemaining), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.Calibration)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.Calibration), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.LeftCalibration)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.LeftCalibration), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.ValveBodyValue)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.ValveBodyValue), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.LeftValveBodyValue)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.LeftValveBodyValue), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.SealingRingValue)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.SealingRingValue), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(DataMonitorPropertyModelEdit.LeftSealingRingValue)) == 0)
                {
                    SetPropertyValue(nameof(DataMonitorPropertyModelEdit.LeftSealingRingValue), gFBaseTypePropValue.Value, true);
                    continue;
                }
            }
            return true;
        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.TextColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.TextColor));
                if (viewModel != null)
                    viewModel.TextColor = DataMonitorPropertyModelEdit.TextColor;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.BackColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.BackColor));
                if (viewModel != null)
                    viewModel.BackColor = DataMonitorPropertyModelEdit.BackColor;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.TextFont)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.TextFont));
                if (viewModel != null)
                    viewModel.TextFont = DataMonitorPropertyModelEdit.TextFont;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusSwitch)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus));
                if (viewModel != null)
                    viewModel.SupplyValvePressureDeviceStatus = DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusName)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus));
                if (viewModel != null)
                    viewModel.SupplyValvePressureDeviceStatus = DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusValue)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus));
                if (viewModel != null)
                    viewModel.SupplyValvePressureDeviceStatus = DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusUnit)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus));
                if (viewModel != null)
                    viewModel.SupplyValvePressureDeviceStatus = DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusSwitch)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus));
                if (viewModel != null)
                    viewModel.SupplyGluePressureDeviceStatus = DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusName)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus));
                if (viewModel != null)
                    viewModel.SupplyGluePressureDeviceStatus = DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusValue)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus));
                if (viewModel != null)
                    viewModel.SupplyGluePressureDeviceStatus = DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusUnit)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus));
                if (viewModel != null)
                    viewModel.SupplyGluePressureDeviceStatus = DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusSwitch)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus));
                if (viewModel != null)
                    viewModel.NozzleHeatingDeviceStatus = DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusName)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus));
                if (viewModel != null)
                    viewModel.NozzleHeatingDeviceStatus = DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusValue)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus));
                if (viewModel != null)
                    viewModel.NozzleHeatingDeviceStatus = DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusUnit)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus));
                if (viewModel != null)
                    viewModel.NozzleHeatingDeviceStatus = DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus;
            }

            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.SafetyDoorStatus)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.SafetyDoorStatus));
                if (viewModel != null) viewModel.SafetyDoorStatus = DataMonitorPropertyModelEdit.SafetyDoorStatus;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.TotalPressureStatus)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.TotalPressureStatus));
                if (viewModel != null) viewModel.TotalPressureStatus = DataMonitorPropertyModelEdit.TotalPressureStatus;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.CleaningClothStatus)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.CleaningClothStatus));
                if (viewModel != null) viewModel.CleaningClothStatus = DataMonitorPropertyModelEdit.CleaningClothStatus;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.IsDualValve)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.IsDualValve));
                if (viewModel != null) viewModel.IsDualValve = DataMonitorPropertyModelEdit.IsDualValve;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.GlueRemaining)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.GlueRemaining));
                if (viewModel != null) viewModel.GlueRemaining = DataMonitorPropertyModelEdit.GlueRemaining;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.LeftGlueRemaining)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.LeftGlueRemaining));
                if (viewModel != null) viewModel.LeftGlueRemaining = DataMonitorPropertyModelEdit.LeftGlueRemaining;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.Calibration)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.Calibration));
                if (viewModel != null) viewModel.Calibration = DataMonitorPropertyModelEdit.Calibration;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.LeftCalibration)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.LeftCalibration));
                if (viewModel != null) viewModel.LeftCalibration = DataMonitorPropertyModelEdit.LeftCalibration;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.ValveBodyValue)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.ValveBodyValue));
                if (viewModel != null) viewModel.ValveBodyValue = DataMonitorPropertyModelEdit.ValveBodyValue;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.LeftValveBodyValue)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.LeftValveBodyValue));
                if (viewModel != null) viewModel.LeftValveBodyValue = DataMonitorPropertyModelEdit.LeftValveBodyValue;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.SealingRingValue)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.SealingRingValue));
                if (viewModel != null) viewModel.SealingRingValue = DataMonitorPropertyModelEdit.SealingRingValue;
            }
            if (string.Compare(propertyID, nameof(DataMonitorPropertyModelEdit.LeftSealingRingValue)) == 0)
            {
                CallBack?.ExecOprt(nameof(DataMonitorPropertyModelEdit.LeftSealingRingValue));
                if (viewModel != null) viewModel.LeftSealingRingValue = DataMonitorPropertyModelEdit.LeftSealingRingValue;
            }
            if (!DataMonitorPropertyModelEdit.IsRuning)
            {
                GFBaseTypePropValueList gFBaseTypePropValues = new GFBaseTypePropValueList();

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusSwitch)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.SupplyValvePressureStatusSwitch)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusName)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.SupplyValvePressureStatusName ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusValue)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.SupplyValvePressureStatusValue ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusUnit)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.SupplyValvePressureStatusUnit ?? string.Empty)));

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusSwitch)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.SupplyGluePressureStatusSwitch)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusName)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.SupplyGluePressureStatusName ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusValue)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.SupplyGluePressureStatusValue ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusUnit)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.SupplyGluePressureStatusUnit ?? string.Empty)));

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusSwitch)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.NozzleHeatingStatusSwitch)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusName)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.NozzleHeatingStatusName ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusValue)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.NozzleHeatingStatusValue ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusUnit)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.NozzleHeatingStatusUnit ?? string.Empty)));

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.SafetyDoorStatus)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.SafetyDoorStatus)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.TotalPressureStatus)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.TotalPressureStatus)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.CleaningClothStatus)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.CleaningClothStatus)));

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.IsDualValve)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.IsDualValve)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.GlueRemaining)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.GlueRemaining ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.LeftGlueRemaining)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.LeftGlueRemaining ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.Calibration)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.Calibration ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.LeftCalibration)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.LeftCalibration ?? string.Empty)));

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.ValveBodyValue)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.ValveBodyValue ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.LeftValveBodyValue)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.LeftValveBodyValue ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.SealingRingValue)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.SealingRingValue ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(DataMonitorPropertyModelEdit.LeftSealingRingValue)), new GriffinsBaseValue(DataMonitorPropertyModelEdit.LeftSealingRingValue ?? string.Empty)));

                CallBack?.UpdateUIDataObjPropValues(gFBaseTypePropValues);
            }
        }

        #endregion

        #region 重写方法
        /// <summary>
        /// 从字节流中读画图信息
        /// </summary>
        /// <param name="br">字节流读取对象</param>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            var propertyEditModelBaseJson = br.ReadString("PropertyEditModelBase");
            if (!string.IsNullOrEmpty(propertyEditModelBaseJson))
            {
                if (TryDeserializePersistModel(propertyEditModelBaseJson, out var persist))
                {
                    ApplyPersistModelToPropertyModel(persist);
                }
                else
                {
                    var propertyEditModelBase = JsonObjConvert.FromJSon<DataMonitorPropertyModelEdit>(propertyEditModelBaseJson);
                    if (propertyEditModelBase != null)
                        (PropertyEditModelBase as DataMonitorPropertyModelEdit).CopyFrom(propertyEditModelBase);
                }
            }
            
            var propertyBindEditModelBaseJson = br.ReadString("PropertyBindEditModelBase");
            if (!string.IsNullOrEmpty(propertyBindEditModelBaseJson))
            {
                var propertyBindEditModelBase = JsonObjConvert.FromJSon<DataMonitorPropertyBindEditModel>(propertyBindEditModelBaseJson);
                if (propertyBindEditModelBase != null)
                    (PropertyBindEditModelBase as DataMonitorPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
            }

            var eventBindEditModelJson = br.ReadString("EventBindEditModel");
            if (!string.IsNullOrEmpty(eventBindEditModelJson))
            {
                var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(eventBindEditModelJson);
                if (eventBindEditModel != null)
                    EventBindEditModel.CopyFrom(eventBindEditModel);
            }

            SyncModelToViewModel();
        }

        /// <summary>
        /// 当把画图信息写入到字节流中
        /// </summary>
        /// <param name="bw">字节流写入对象</param>
        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            
            if (PropertyEditModelBase is DataMonitorPropertyModelEdit m)
            {
                var persist = new DataMonitorPersistModel
                {
                    PersistVersion = 2,
                    TextFontJson = JsonObjConvert.ToJSon(m.TextFont),
                    TextColor = m.TextColor.ToString(),
                    BackColor = m.BackColor.ToString(),

                    SupplyValvePressureStatusName = m.SupplyValvePressureStatusName,
                    SupplyValvePressureStatusValue = m.SupplyValvePressureStatusValue,
                    SupplyValvePressureStatusUnit = m.SupplyValvePressureStatusUnit,
                    SupplyValvePressureStatusSwitch = m.SupplyValvePressureStatusSwitch,

                    SupplyGluePressureStatusName = m.SupplyGluePressureStatusName,
                    SupplyGluePressureStatusValue = m.SupplyGluePressureStatusValue,
                    SupplyGluePressureStatusUnit = m.SupplyGluePressureStatusUnit,
                    SupplyGluePressureStatusSwitch = m.SupplyGluePressureStatusSwitch,

                    NozzleHeatingStatusName = m.NozzleHeatingStatusName,
                    NozzleHeatingStatusValue = m.NozzleHeatingStatusValue,
                    NozzleHeatingStatusUnit = m.NozzleHeatingStatusUnit,
                    NozzleHeatingStatusSwitch = m.NozzleHeatingStatusSwitch,

                    SafetyDoorStatus = m.SafetyDoorStatus,
                    TotalPressureStatus = m.TotalPressureStatus,
                    CleaningClothStatus = m.CleaningClothStatus,
                    IsDualValve = m.IsDualValve,
                    GlueRemaining = m.GlueRemaining,
                    LeftGlueRemaining = m.LeftGlueRemaining,
                    Calibration = m.Calibration,
                    LeftCalibration = m.LeftCalibration,
                    ValveBodyValue = m.ValveBodyValue,
                    SealingRingValue = m.SealingRingValue,
                    LeftValveBodyValue = m.LeftValveBodyValue,
                    LeftSealingRingValue = m.LeftSealingRingValue,
                };
                bw.Write("PropertyEditModelBase", System.Text.Json.JsonSerializer.Serialize(persist));
            }
            else
            {
                bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            }

            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
        }

        /// <summary>
        /// 从来源实例复制字段到本实例
        /// </summary>
        /// <param name="source">来源实例</param>
        protected override void OnCopyFrom(FunctionalCellBase source)
        {
            MapCellDataMonitorCtlObj mapCellHeatingControlCtlObj = (source as MapCellDataMonitorCtlObj);
            base._CopyFrom(mapCellHeatingControlCtlObj);
            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
            (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel.CopyFrom(source.EventBindEditModel);
        }

        /// <summary>
        /// 初始化时
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
        }
        #endregion

        #region 抽象方法实现
        /// <summary>
        /// 获取视图对象
        /// </summary>
        /// <returns>视图对象</returns>
        protected override object OnGetView()
        {
            return view;
        }

        /// <summary>
        /// 获取视图模型
        /// </summary>
        /// <returns>视图模型</returns>
        protected override object OnGetViewModel()
        {
            return viewModel;
        }

        /// <summary>
        /// 创建属性编辑模型
        /// </summary>
        /// <returns>属性编辑模型</returns>
        public override PropertyEditModelBase CreatePropertyModelEditBase()
        {
            return new DataMonitorPropertyModelEdit();
        }

        /// <summary>
        /// 创建属性绑定编辑模型
        /// </summary>
        /// <returns>属性绑定编辑模型</returns>
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
        {
            var m = new DataMonitorPropertyBindEditModel();
            ApplyDefaultTestBindings(m);
            return m;
        }

        /// <summary>
        /// 创建事件绑定编辑模型
        /// </summary>
        /// <returns>事件绑定编辑模型</returns>
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
        #endregion

        #region 辅助方法
        /// <summary>
        /// 应用默认测试绑定
        /// </summary>
        /// <param name="m">属性绑定编辑模型</param>
        private static void ApplyDefaultTestBindings(DataMonitorPropertyBindEditModel m)
        {
        }

        /// <summary>
        /// 从Avalonia资源(avares)加载图片；失败返回null。
        /// </summary>
        /// <param name="uriText">资源URI</param>
        /// <returns>加载的图片</returns>
        private static BitmapData TryLoadBitmap(string uriText)
        {
            try
            {
                var uri = new Uri(uriText);
                using (var stream = AssetLoader.Open(uri))
                {
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        var bitmapData = new BitmapData();
                        bitmapData.FromBytes(memoryStream.ToArray());
                        return bitmapData;
                    }
                }
            }
            catch (Exception)
            {
                return new BitmapData();
            }
        }
        #endregion

        #region 操作原子执行器
        /// <summary>
        /// 文本颜色操作单元执行器
        /// </summary>
        private class TextColorMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellDataMonitorCtlObj owner;
            private IMapOprtCellExectorCallBack callBack;

            public TextColorMapOprtCellExector(MapCellDataMonitorCtlObj owner)
            {
                this.owner = owner;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is ViewModel.DataMonitorViewModel dataMonitorViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.TextColor));
                    if (mapCellPropValue is { })
                    {
                        var color = mapCellPropValue.ToPrimitiveValue<string>();
                        dataMonitorViewModel.TextColor = Color.Parse(color);
                    }
                }
            }
        }

        /// <summary>
        /// 背景颜色操作单元执行器
        /// </summary>
        private class BackColorMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellDataMonitorCtlObj owner;
            private IMapOprtCellExectorCallBack callBack;

            public BackColorMapOprtCellExector(MapCellDataMonitorCtlObj owner)
            {
                this.owner = owner;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is ViewModel.DataMonitorViewModel dataMonitorViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.BackColor));
                    if (mapCellPropValue is { })
                    {
                        var color = mapCellPropValue.ToPrimitiveValue<string>();
                        dataMonitorViewModel.BackColor = Color.Parse(color);
                    }
                }
            }
        }

        /// <summary>
        /// 文本字体操作单元执行器
        /// </summary>
        private class TextFontMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellDataMonitorCtlObj owner;
            private IMapOprtCellExectorCallBack callBack;

            public TextFontMapOprtCellExector(MapCellDataMonitorCtlObj owner)
            {
                this.owner = owner;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is ViewModel.DataMonitorViewModel dataMonitorViewModel)
                {
                    dataMonitorViewModel.SetTextFontFromModel(owner.DataMonitorPropertyModelEdit.TextFont);
                }
            }
        }

        private class SupplyValvePressureMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellDataMonitorCtlObj owner;
            private IMapOprtCellExectorCallBack callBack;

            public SupplyValvePressureMapOprtCellExector(MapCellDataMonitorCtlObj owner)
            {
                this.owner = owner;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (cfg != null && cfg.Length > 0)
                {
                    var config = MapCell_DataMonitor.MapOprtCellParamCfgView.DataMonitorOprtCellCfgSerializer.FromBytes<MapCell_DataMonitor.MapOprtCellParamCfgView.SupplyValvePressureMapOprtCellParamViewModel>(cfg);
                    if (config != null)
                    {
                        owner.DataMonitorPropertyModelEdit.SupplyValvePressureStatusName = config.StatusName;
                        owner.DataMonitorPropertyModelEdit.SupplyValvePressureStatusValue = config.StatusValue;
                        owner.DataMonitorPropertyModelEdit.SupplyValvePressureStatusUnit = config.StatusUnit;
                        owner.DataMonitorPropertyModelEdit.SupplyValvePressureStatusSwitch = config.StatusSwitch;
                    }
                }
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is ViewModel.DataMonitorViewModel dataMonitorViewModel)
                {
                    dataMonitorViewModel.SupplyValvePressureDeviceStatus = owner.DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus;
                }
            }
        }

        private class SupplyGluePressureMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellDataMonitorCtlObj owner;
            private IMapOprtCellExectorCallBack callBack;

            public SupplyGluePressureMapOprtCellExector(MapCellDataMonitorCtlObj owner)
            {
                this.owner = owner;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (cfg != null && cfg.Length > 0)
                {
                    var config = MapCell_DataMonitor.MapOprtCellParamCfgView.DataMonitorOprtCellCfgSerializer.FromBytes<MapCell_DataMonitor.MapOprtCellParamCfgView.SupplyGluePressureMapOprtCellParamViewModel>(cfg);
                    if (config != null)
                    {
                        owner.DataMonitorPropertyModelEdit.SupplyGluePressureStatusName = config.StatusName;
                        owner.DataMonitorPropertyModelEdit.SupplyGluePressureStatusValue = config.StatusValue;
                        owner.DataMonitorPropertyModelEdit.SupplyGluePressureStatusUnit = config.StatusUnit;
                        owner.DataMonitorPropertyModelEdit.SupplyGluePressureStatusSwitch = config.StatusSwitch;
                    }
                }
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is ViewModel.DataMonitorViewModel dataMonitorViewModel)
                {
                    dataMonitorViewModel.SupplyGluePressureDeviceStatus = owner.DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus;
                }
            }
        }

        private class NozzleHeatingMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellDataMonitorCtlObj owner;
            private IMapOprtCellExectorCallBack callBack;

            public NozzleHeatingMapOprtCellExector(MapCellDataMonitorCtlObj owner)
            {
                this.owner = owner;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (cfg != null && cfg.Length > 0)
                {
                    var config = MapCell_DataMonitor.MapOprtCellParamCfgView.DataMonitorOprtCellCfgSerializer.FromBytes<MapCell_DataMonitor.MapOprtCellParamCfgView.NozzleHeatingMapOprtCellParamViewModel>(cfg);
                    if (config != null)
                    {
                        owner.DataMonitorPropertyModelEdit.NozzleHeatingStatusName = config.StatusName;
                        owner.DataMonitorPropertyModelEdit.NozzleHeatingStatusValue = config.StatusValue;
                        owner.DataMonitorPropertyModelEdit.NozzleHeatingStatusUnit = config.StatusUnit;
                        owner.DataMonitorPropertyModelEdit.NozzleHeatingStatusSwitch = config.StatusSwitch;
                    }
                }
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is ViewModel.DataMonitorViewModel dataMonitorViewModel)
                {
                    dataMonitorViewModel.NozzleHeatingDeviceStatus = owner.DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus;
                }
            }
        }

        private class SystemStatusMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellDataMonitorCtlObj owner;
            private IMapOprtCellExectorCallBack callBack;

            public SystemStatusMapOprtCellExector(MapCellDataMonitorCtlObj owner)
            {
                this.owner = owner;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (cfg != null && cfg.Length > 0)
                {
                    var config = MapCell_DataMonitor.MapOprtCellParamCfgView.DataMonitorOprtCellCfgSerializer.FromBytes<MapCell_DataMonitor.MapOprtCellParamCfgView.SystemStatusMapOprtCellParamViewModel>(cfg);
                    if (config != null)
                    {
                        owner.DataMonitorPropertyModelEdit.SafetyDoorStatus = config.SafetyDoorStatus;
                        owner.DataMonitorPropertyModelEdit.TotalPressureStatus = config.TotalPressureStatus;
                        owner.DataMonitorPropertyModelEdit.CleaningClothStatus = config.CleaningClothStatus;
                        owner.DataMonitorPropertyModelEdit.IsDualValve = config.IsDualValve;
                        owner.DataMonitorPropertyModelEdit.GlueRemaining = config.GlueRemaining;
                        owner.DataMonitorPropertyModelEdit.LeftGlueRemaining = config.LeftGlueRemaining;
                        owner.DataMonitorPropertyModelEdit.Calibration = config.Calibration;
                        owner.DataMonitorPropertyModelEdit.LeftCalibration = config.LeftCalibration;
                        owner.DataMonitorPropertyModelEdit.ValveBodyValue = config.ValveBodyValue;
                        owner.DataMonitorPropertyModelEdit.LeftValveBodyValue = config.LeftValveBodyValue;
                        owner.DataMonitorPropertyModelEdit.SealingRingValue = config.SealingRingValue;
                        owner.DataMonitorPropertyModelEdit.LeftSealingRingValue = config.LeftSealingRingValue;
                    }
                }
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is ViewModel.DataMonitorViewModel dataMonitorViewModel)
                {
                    GriffinsBaseValue v;

                    v = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.SafetyDoorStatus));
                    if (v is { }) dataMonitorViewModel.SafetyDoorStatus = v.ToPrimitiveValue<bool>();

                    v = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.TotalPressureStatus));
                    if (v is { }) dataMonitorViewModel.TotalPressureStatus = v.ToPrimitiveValue<bool>();

                    v = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.CleaningClothStatus));
                    if (v is { }) dataMonitorViewModel.CleaningClothStatus = v.ToPrimitiveValue<bool>();

                    v = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.IsDualValve));
                    if (v is { }) dataMonitorViewModel.IsDualValve = v.ToPrimitiveValue<bool>();

                    v = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.GlueRemaining));
                    if (v is { }) dataMonitorViewModel.GlueRemaining = v.ToPrimitiveValue<string>();

                    v = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.LeftGlueRemaining));
                    if (v is { }) dataMonitorViewModel.LeftGlueRemaining = v.ToPrimitiveValue<string>();

                    v = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.Calibration));
                    if (v is { }) dataMonitorViewModel.Calibration = v.ToPrimitiveValue<string>();

                    v = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.LeftCalibration));
                    if (v is { }) dataMonitorViewModel.LeftCalibration = v.ToPrimitiveValue<string>();

                    v = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.ValveBodyValue));
                    if (v is { }) dataMonitorViewModel.ValveBodyValue = v.ToPrimitiveValue<string>();

                    v = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.LeftValveBodyValue));
                    if (v is { }) dataMonitorViewModel.LeftValveBodyValue = v.ToPrimitiveValue<string>();

                    v = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.SealingRingValue));
                    if (v is { }) dataMonitorViewModel.SealingRingValue = v.ToPrimitiveValue<string>();

                    v = callBack.GetMapCellPropValue(nameof(DataMonitorPropertyModelEdit.LeftSealingRingValue));
                    if (v is { }) dataMonitorViewModel.LeftSealingRingValue = v.ToPrimitiveValue<string>();
                }
            }
        }
        #endregion

        /// <summary>
        /// 设置按钮文本字体（供外部/界面在字体变化后统一刷新按钮样式）。
        /// </summary>
        public void SetButtonTextFont()
        {
            double size = base.CallBack?.Calc?.CalcZoomVal((decimal)this.DataMonitorPropertyModelEdit.TextFont.FontSize) ?? this.DataMonitorPropertyModelEdit.TextFont.FontSize;
            if (size < 2)
                size = 2;
            FontInfo font = new FontInfo(this.DataMonitorPropertyModelEdit.TextFont.FontFamily, size, this.DataMonitorPropertyModelEdit.TextFont.FontWeight, this.DataMonitorPropertyModelEdit.TextFont.FontStyle);
            this.viewModel.TextFont = font;
        }

        public override string ToString()
        {
            return "数据监控";
        }

        /// <summary>
        /// DataMonitor 的持久化DTO（仅用于保存/加载）。
        /// 1、只存储稳定、易序列化的字段（bool/string等），避免Color/FontInfo/DeviceStatusCommonInfo/Bitmap等复杂类型的JSON序列化风险。
        /// 2、通过 <see cref="PersistVersion"/> 做版本控制，便于后续扩展并保持向后兼容。
        /// </summary>
        private sealed class DataMonitorPersistModel
        {
            // 持久化版本号
            public int PersistVersion { get; set; }

            // 文本字体
            public string TextFontJson { get; set; }
            // 文本颜色
            public string TextColor { get; set; }
            // 背景颜色
            public string BackColor { get; set; }

            // 供阀气压栏：状态名称
            public string SupplyValvePressureStatusName { get; set; }
            // 供阀气压栏：状态值
            public string SupplyValvePressureStatusValue { get; set; }
            // 供阀气压栏：单位
            public string SupplyValvePressureStatusUnit { get; set; }
            // 供阀气压栏：开关
            public bool SupplyValvePressureStatusSwitch { get; set; }

            // 供胶气压栏：状态名称
            public string SupplyGluePressureStatusName { get; set; }
            // 供胶气压栏：状态值
            public string SupplyGluePressureStatusValue { get; set; }
            // 供胶气压栏：单位
            public string SupplyGluePressureStatusUnit { get; set; }
            // 供胶气压栏：开关
            public bool SupplyGluePressureStatusSwitch { get; set; }

            // 喷嘴加热栏：状态名称
            public string NozzleHeatingStatusName { get; set; }
            // 喷嘴加热栏：状态值
            public string NozzleHeatingStatusValue { get; set; }
            // 喷嘴加热栏：单位
            public string NozzleHeatingStatusUnit { get; set; }
            // 喷嘴加热栏：开关
            public bool NozzleHeatingStatusSwitch { get; set; }

            // 监控信息栏：安全门状态
            public bool SafetyDoorStatus { get; set; }
            // 监控信息栏：总气压状态
            public bool TotalPressureStatus { get; set; }
            // 监控信息栏：清洁布状态
            public bool CleaningClothStatus { get; set; }
            // 是否双阀模式
            public bool IsDualValve { get; set; }
            // 胶水余量文本
            public string GlueRemaining { get; set; }
            // 左阀胶水余量文本
            public string LeftGlueRemaining { get; set; }
            // 校正文本
            public string Calibration { get; set; }
            // 左阀校正文本
            public string LeftCalibration { get; set; }
            // 阀体值文本
            public string ValveBodyValue { get; set; }
            // 密封圈值文本
            public string SealingRingValue { get; set; }
            // 左阀阀体值文本
            public string LeftValveBodyValue { get; set; }
            // 左阀密封圈值文本
            public string LeftSealingRingValue { get; set; }
        }

        private static bool TryDeserializePersistModel(string json, out DataMonitorPersistModel model)
        {
            model = null;
            if (string.IsNullOrWhiteSpace(json))
                return false;
            try
            {
                var m = System.Text.Json.JsonSerializer.Deserialize<DataMonitorPersistModel>(json);
                if (m == null)
                    return false;
                if (m.PersistVersion != 1 && m.PersistVersion != 2)
                    return false;
                model = m;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ApplyPersistModelToPropertyModel(DataMonitorPersistModel persist)
        {
            if (persist == null)
                return;
            if (PropertyEditModelBase is not DataMonitorPropertyModelEdit model)
                return;

            if (model.SupplyValvePressureDeviceStatus == null)
                model.SupplyValvePressureDeviceStatus = new DeviceStatusCommonInfo();
            if (model.SupplyGluePressureDeviceStatus == null)
                model.SupplyGluePressureDeviceStatus = new DeviceStatusCommonInfo();
            if (model.NozzleHeatingDeviceStatus == null)
                model.NozzleHeatingDeviceStatus = new DeviceStatusCommonInfo();

            try
            {
                if (!string.IsNullOrWhiteSpace(persist.TextFontJson))
                {
                    var font = JsonObjConvert.FromJSon<FontInfo>(persist.TextFontJson);
                    if (font != null)
                        model.TextFont = font;
                }
            }
            catch { }

            try
            {
                if (!string.IsNullOrWhiteSpace(persist.TextColor))
                    model.TextColor = Color.Parse(persist.TextColor);
            }
            catch { }

            try
            {
                if (!string.IsNullOrWhiteSpace(persist.BackColor))
                    model.BackColor = Color.Parse(persist.BackColor);
            }
            catch { }

            if (persist.SupplyValvePressureStatusName != null)
                model.SupplyValvePressureStatusName = persist.SupplyValvePressureStatusName;
            if (persist.SupplyValvePressureStatusValue != null)
                model.SupplyValvePressureStatusValue = persist.SupplyValvePressureStatusValue;
            if (persist.PersistVersion >= 2)
            {
                if (persist.SupplyValvePressureStatusUnit != null)
                    model.SupplyValvePressureStatusUnit = persist.SupplyValvePressureStatusUnit;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.SupplyValvePressureStatusUnit))
                    model.SupplyValvePressureStatusUnit = "kgf/cm²";
            }
            model.SupplyValvePressureStatusSwitch = persist.SupplyValvePressureStatusSwitch;

            if (persist.SupplyGluePressureStatusName != null)
                model.SupplyGluePressureStatusName = persist.SupplyGluePressureStatusName;
            if (persist.SupplyGluePressureStatusValue != null)
                model.SupplyGluePressureStatusValue = persist.SupplyGluePressureStatusValue;
            if (persist.PersistVersion >= 2)
            {
                if (persist.SupplyGluePressureStatusUnit != null)
                    model.SupplyGluePressureStatusUnit = persist.SupplyGluePressureStatusUnit;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.SupplyGluePressureStatusUnit))
                    model.SupplyGluePressureStatusUnit = "kgf/cm²";
            }
            model.SupplyGluePressureStatusSwitch = persist.SupplyGluePressureStatusSwitch;

            if (persist.NozzleHeatingStatusName != null)
                model.NozzleHeatingStatusName = persist.NozzleHeatingStatusName;
            if (persist.NozzleHeatingStatusValue != null)
                model.NozzleHeatingStatusValue = persist.NozzleHeatingStatusValue;
            if (persist.PersistVersion >= 2)
            {
                if (persist.NozzleHeatingStatusUnit != null)
                    model.NozzleHeatingStatusUnit = persist.NozzleHeatingStatusUnit;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.NozzleHeatingStatusUnit))
                    model.NozzleHeatingStatusUnit = "°C";
            }
            model.NozzleHeatingStatusSwitch = persist.NozzleHeatingStatusSwitch;

            model.SafetyDoorStatus = persist.SafetyDoorStatus;
            model.TotalPressureStatus = persist.TotalPressureStatus;
            model.CleaningClothStatus = persist.CleaningClothStatus;
            model.IsDualValve = persist.IsDualValve;
            if (persist.GlueRemaining != null)
                model.GlueRemaining = IsLegacyGlueRemainingPlaceholder(persist.GlueRemaining) ? "50%" : persist.GlueRemaining;
            if (persist.LeftGlueRemaining != null)
                model.LeftGlueRemaining = IsLegacyGlueRemainingPlaceholder(persist.LeftGlueRemaining) ? "50%" : persist.LeftGlueRemaining;
            if (persist.Calibration != null)
                model.Calibration = persist.Calibration;
            if (persist.LeftCalibration != null)
                model.LeftCalibration = persist.LeftCalibration;
            if (persist.ValveBodyValue != null)
                model.ValveBodyValue = persist.ValveBodyValue;
            if (persist.SealingRingValue != null)
                model.SealingRingValue = persist.SealingRingValue;
            if (persist.LeftValveBodyValue != null)
                model.LeftValveBodyValue = persist.LeftValveBodyValue;
            if (persist.LeftSealingRingValue != null)
                model.LeftSealingRingValue = persist.LeftSealingRingValue;
        }

        private static bool IsLegacyGlueRemainingPlaceholder(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            var s = value.Trim();
            return s.Contains("**h|") || s.Contains("mg") || s.Contains("pcs") || s.Contains("|");
        }
    }
}

#region 图元属性编辑模型
[Serializable]
[MapPropertyOrder]
[CategoryPriority("图元信息", 1)]
[CategoryPriority("设备状态栏", 2)]
[CategoryPriority("监控信息栏", 3)]
public class DataMonitorPropertyModelEdit : FunctionalCellPropertyModelEdit
{
    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private DeviceStatusCommonInfo _prevSupplyValvePressureDeviceStatus;

    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private DeviceStatusCommonInfo _prevSupplyGluePressureDeviceStatus;

    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private DeviceStatusCommonInfo _prevNozzleHeatingDeviceStatus;

    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private bool _isSettingBackColor;

    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private bool _isSettingTextColor;

    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private bool _isSettingTextFont;

    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private bool _suppressSupplyValvePressureSwitchChanged;

    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private bool _suppressSupplyGluePressureSwitchChanged;

    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private bool _suppressNozzleHeatingSwitchChanged;

    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private bool _suppressLeftGlueRemainingSwitchChanged;

    #region 构造函数
    /// <summary>
    /// 数据监控属性模型编辑
    /// </summary>
    public DataMonitorPropertyModelEdit()
    {
        TextFont.PropertyChanged += textFont_PropertyChanged;

        // 只有在字段为null时才初始化设备状态栏默认值
        // 这样可以避免覆盖从JSON反序列化恢复的数据
        if (_supplyValvePressureDeviceStatus == null)
            _supplyValvePressureDeviceStatus = CreateDefaultSupplyValvePressureDeviceStatus();
        if (_supplyGluePressureDeviceStatus == null)
            _supplyGluePressureDeviceStatus = CreateDefaultSupplyGluePressureDeviceStatus();
        if (_nozzleHeatingDeviceStatus == null)
            _nozzleHeatingDeviceStatus = CreateDefaultNozzleHeatingDeviceStatus();

        AttachDeviceStatusHandlers();
    }

    /// <summary>
    /// 用于反序列化的构造函数
    /// </summary>
    public DataMonitorPropertyModelEdit(bool forDeserialization)
    {
        TextFont.PropertyChanged += textFont_PropertyChanged;

        // 反序列化时不初始化默认值，等待CopyFrom调用
        if (!forDeserialization)
        {
            if (_supplyValvePressureDeviceStatus == null)
                _supplyValvePressureDeviceStatus = CreateDefaultSupplyValvePressureDeviceStatus();
            if (_supplyGluePressureDeviceStatus == null)
                _supplyGluePressureDeviceStatus = CreateDefaultSupplyGluePressureDeviceStatus();
            if (_nozzleHeatingDeviceStatus == null)
                _nozzleHeatingDeviceStatus = CreateDefaultNozzleHeatingDeviceStatus();
        }

        AttachDeviceStatusHandlers();
    }

    /// <summary>
    /// 重新挂接三个设备状态栏对象的 PropertyChanged 事件，用于将子对象变更冒泡为外层属性变更.
    /// </summary>
    private void AttachDeviceStatusHandlers()
    {
        // DeviceStatusCommonInfo 是可变对象；当属性被整体替换（例如反序列化/CopyFrom）时，
        // 需要重新挂接其 PropertyChanged 事件，否则UI不会刷新。
        DetachDeviceStatusHandler(_prevSupplyValvePressureDeviceStatus, SupplyValvePressureDeviceStatus_PropertyChanged);
        DetachDeviceStatusHandler(_prevSupplyGluePressureDeviceStatus, SupplyGluePressureDeviceStatus_PropertyChanged);
        DetachDeviceStatusHandler(_prevNozzleHeatingDeviceStatus, NozzleHeatingDeviceStatus_PropertyChanged);

        _prevSupplyValvePressureDeviceStatus = _supplyValvePressureDeviceStatus;
        _prevSupplyGluePressureDeviceStatus = _supplyGluePressureDeviceStatus;
        _prevNozzleHeatingDeviceStatus = _nozzleHeatingDeviceStatus;

        AttachDeviceStatusHandler(_supplyValvePressureDeviceStatus, SupplyValvePressureDeviceStatus_PropertyChanged);
        AttachDeviceStatusHandler(_supplyGluePressureDeviceStatus, SupplyGluePressureDeviceStatus_PropertyChanged);
        AttachDeviceStatusHandler(_nozzleHeatingDeviceStatus, NozzleHeatingDeviceStatus_PropertyChanged);
    }

    /// <summary>
    /// 为指定设备状态对象挂接 PropertyChanged 事件.
    /// </summary>
    /// <param name="info">设备状态对象</param>
    /// <param name="handler">事件处理器</param>
    private static void AttachDeviceStatusHandler(DeviceStatusCommonInfo? info, PropertyChangedEventHandler handler)
    {
        if (info == null) return;
        info.PropertyChanged += handler;
    }

    /// <summary>
    /// 为指定设备状态对象解除挂接 PropertyChanged 事件.
    /// </summary>
    /// <param name="info">设备状态对象</param>
    /// <param name="handler">事件处理器</param>
    private static void DetachDeviceStatusHandler(DeviceStatusCommonInfo? info, PropertyChangedEventHandler handler)
    {
        if (info == null) return;
        info.PropertyChanged -= handler;
    }

    /// <summary>
    /// 供阀气压状态对象内部字段变化时，触发派生属性的变更通知以刷新UI.
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">属性变更参数</param>
    private void SupplyValvePressureDeviceStatus_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.PropertyName)) return;
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.StatusName)) RaisePropertyChanged(nameof(SupplyValvePressureStatusName));
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.DeviceStatusValue)) RaisePropertyChanged(nameof(SupplyValvePressureStatusValue));
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.DeviceStatusUnit)) RaisePropertyChanged(nameof(SupplyValvePressureStatusUnit));
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.CurrentIndex))
        {
            if (!_suppressSupplyValvePressureSwitchChanged)
                RaisePropertyChanged(nameof(SupplyValvePressureStatusSwitch));
        }
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.ImageSources)) RaisePropertyChanged(nameof(SupplyValvePressureDeviceStatus));
    }

    /// <summary>
    /// 供胶气压状态对象内部字段变化时，触发派生属性的变更通知以刷新UI.
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">属性变更参数</param>
    private void SupplyGluePressureDeviceStatus_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.PropertyName)) return;
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.StatusName)) RaisePropertyChanged(nameof(SupplyGluePressureStatusName));
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.DeviceStatusValue)) RaisePropertyChanged(nameof(SupplyGluePressureStatusValue));
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.DeviceStatusUnit)) RaisePropertyChanged(nameof(SupplyGluePressureStatusUnit));
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.CurrentIndex))
        {
            if (!_suppressSupplyGluePressureSwitchChanged)
                RaisePropertyChanged(nameof(SupplyGluePressureStatusSwitch));
        }
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.ImageSources)) RaisePropertyChanged(nameof(SupplyGluePressureDeviceStatus));
    }

    /// <summary>
    /// 喷嘴加热状态对象内部字段变化时，触发派生属性的变更通知以刷新UI.
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">属性变更参数</param>
    private void NozzleHeatingDeviceStatus_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.PropertyName)) return;
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.StatusName)) RaisePropertyChanged(nameof(NozzleHeatingStatusName));
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.DeviceStatusValue)) RaisePropertyChanged(nameof(NozzleHeatingStatusValue));
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.DeviceStatusUnit)) RaisePropertyChanged(nameof(NozzleHeatingStatusUnit));
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.CurrentIndex))
        {
            if (!_suppressNozzleHeatingSwitchChanged)
                RaisePropertyChanged(nameof(NozzleHeatingStatusSwitch));
        }
        if (e.PropertyName == nameof(DeviceStatusCommonInfo.ImageSources)) RaisePropertyChanged(nameof(NozzleHeatingDeviceStatus));
    }

    /// <summary>
    /// 创建默认的供阀气压设备状态
    /// </summary>
    /// <returns>默认的DeviceStatusCommonInfo实例</returns>
    private DeviceStatusCommonInfo CreateDefaultSupplyValvePressureDeviceStatus()
    {
        var waterImage = LoadBitmapFromAssets("Water.png");
        var closeImage = LoadBitmapFromAssets("Close.png");
        return new DeviceStatusCommonInfo(
            new List<BitmapData> { waterImage, closeImage },
            0, // 默认显示第一张图片
            "供阀气压",
            "0.000",
            "kgf/cm²"
        );
    }

    /// <summary>
    /// 创建默认的供胶气压设备状态
    /// </summary>
    /// <returns>默认的DeviceStatusCommonInfo实例</returns>
    private DeviceStatusCommonInfo CreateDefaultSupplyGluePressureDeviceStatus()
    {
        var waterImage = LoadBitmapFromAssets("Water.png");
        var closeImage = LoadBitmapFromAssets("Close.png");
        return new DeviceStatusCommonInfo(
            new List<BitmapData> { waterImage, closeImage },
            0,
            "供胶气压",
            "0.000",
            "kgf/cm²"
        );
    }

    /// <summary>
    /// 创建默认的喷嘴加热设备状态
    /// </summary>
    /// <returns>默认的DeviceStatusCommonInfo实例</returns>
    private DeviceStatusCommonInfo CreateDefaultNozzleHeatingDeviceStatus()
    {
        var waterImage = LoadBitmapFromAssets("Water.png");
        var closeImage = LoadBitmapFromAssets("Close.png");
        return new DeviceStatusCommonInfo(
            new List<BitmapData> { waterImage, closeImage },
            0,
            "喷嘴加热",
            "0.0",
            "°C"
        );
    }

    /// <summary>
    /// 从Assets加载图片
    /// </summary>
    /// <param name="fileName">图片文件名</param>
    /// <returns>BitmapData对象</returns>
    private BitmapData LoadBitmapFromAssets(string fileName)
    {
        try
        {
            var uri = new Uri($"avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/{fileName}");
            using var assets = Avalonia.Platform.AssetLoader.Open(uri);
            using var stream = new System.IO.MemoryStream();
            assets.CopyTo(stream);
            var bitmapData = new BitmapData();
            bitmapData.FromBytes(stream.ToArray());
            return bitmapData;
        }
        catch
        {
            // 如果加载失败，返回空的BitmapData
            return new BitmapData();
        }
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 将 <see cref="GriffinsBaseValue"/> 尽可能安全地转换为 <see cref="BitmapData"/>.
    /// </summary>
    /// <param name="v">输入值</param>
    /// <returns>转换后的图片数据；失败返回 null</returns>
    private static BitmapData SafeConvertToBitmapData(GriffinsBaseValue v)
    {
        if (v == null)
            return null;

        try
        {
            return v.ToPrimitiveValue<BitmapData>();
        }
        catch
        {
        }

        try
        {
            var objectValueBytes = v.ToObjectValue_Bytes();
            if (objectValueBytes != null)
            {
                var gbv = GriffinsBaseValue.Create(objectValueBytes);
                IGriffinsBaseValue obj = new BitmapData();
                obj.PopulateFromBaseValue(gbv);
                return (BitmapData)obj;
            }
        }
        catch
        {
        }

        try
        {
            var objectValueJson = v.ToObjectValue_Json();
            if (objectValueJson != null)
            {
                var gbv = GriffinsBaseValue.Create(objectValueJson);
                IGriffinsBaseValue obj = new BitmapData();
                obj.PopulateFromBaseValue(gbv);
                return (BitmapData)obj;
            }
        }
        catch
        {
        }

        return null;
    }

    /// <summary>
    /// 从完整值中提取数字部分
    /// </summary>
    /// <param name="fullValue">完整值（如"0.000 MPa"）</param>
    /// <returns>数字部分（如"0.000"）</returns>
    private string ExtractNumericValue(string fullValue)
    {
        if (string.IsNullOrEmpty(fullValue))
            return "";

        // 属性面板希望用户只输入数字，但底层存储/显示需要带单位。
        // 因此getter取值时要把单位剥离，仅返回数字。
        // 移除所有单位，只保留数字和小数点
        var numericValue = fullValue;
        numericValue = numericValue.Replace("MPa", "").Trim();
        numericValue = numericValue.Replace("°C", "").Trim();
        numericValue = numericValue.Replace("kgf/cm²", "").Trim();

        return numericValue;
    }

    /// <summary>
    /// 验证输入是否为有效的数字
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>是否为有效数字</returns>
    private bool IsValidNumericInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return true; // 允许空值

        // 用decimal.TryParse做宽松校验：支持小数、负数、不同区域性小数点（由运行环境决定）。
        // 检查是否为有效的数字格式（支持小数点和负数）
        return decimal.TryParse(input, out _);
    }

    /// <summary>
    /// 字体属性变更事件处理
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void textFont_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        RaisePropertyChanged(nameof(TextFont));
    }
    #endregion

    #region 显示属性
    /// <summary>
    /// 文字字体
    /// </summary>
    private FontInfo _textFont = new FontInfo(FontInfo.DefaultFont.FontFamily, 16, FontInfo.DefaultFont.FontWeight, FontInfo.DefaultFont.FontStyle);
    [DisplayName("文字字体")]
    [Category("图元信息")]
    [PropertySortOrder(8)]
    [Browsable(false)]
    public FontInfo TextFont
    {
        get { return _textFont; }
        set
        {
            if (!ReferenceEquals(_textFont, value))
            {
                try { _textFont.PropertyChanged -= textFont_PropertyChanged; } catch { }
                _textFont = value;
                try { _textFont.PropertyChanged += textFont_PropertyChanged; } catch { }
                RaisePropertyChanged(nameof(TextFont));
            }
        }
    }

    /// <summary>
    /// 文本颜色
    /// </summary>
    private Color _textColor = Colors.Black;
    [DisplayName("文本颜色")]
    [Category("图元信息")]
    [PropertySortOrder(9)]
    [JsonConverter(typeof(ColorConvert))]
    [Browsable(false)]
    public Color TextColor
    {
        get { return _textColor; }
        set
        {
            if (_textColor != value)
            {
                _textColor = value;
                RaisePropertyChanged(nameof(TextColor));
            }
        }
    }

    /// <summary>
    /// 背景颜色
    /// </summary>
    private Color _backColor = Colors.White;
    [DisplayName("背景颜色")]
    [Category("图元信息")]
    [PropertySortOrder(10)]
    [JsonConverter(typeof(ColorConvert))]
    [Browsable(false)]
    public Color BackColor
    {
        get { return _backColor; }
        set
        {
            if (_backColor != value)
            {
                _backColor = value;
                RaisePropertyChanged(nameof(BackColor));
            }
        }
    }
    #endregion

    #region 供阀气压设备状态栏属性
    /// <summary>
    /// 供阀气压设备状态
    /// </summary>
    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private DeviceStatusCommonInfo _supplyValvePressureDeviceStatus;
    [Browsable(false)] // 隐藏原始属性
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public DeviceStatusCommonInfo SupplyValvePressureDeviceStatus
    {
        get { return _supplyValvePressureDeviceStatus; }
        set
        {
            if (_supplyValvePressureDeviceStatus != value)
            {
                if (value == null)
                {
                    _supplyValvePressureDeviceStatus = new DeviceStatusCommonInfo();
                }
                else
                {
                    _supplyValvePressureDeviceStatus = value;

                    // 如果传入的对象没有图片数据，添加默认图片
                    if (_supplyValvePressureDeviceStatus.ImageSources == null || _supplyValvePressureDeviceStatus.ImageSources.Count == 0)
                    {
                        _supplyValvePressureDeviceStatus.ImageSources = new List<BitmapData>
                            {
                                LoadBitmapFromAssets("Water.png"),
                                LoadBitmapFromAssets("Close.png")
                            };
                        // 手动触发ImageSources的属性变更事件
                        _supplyValvePressureDeviceStatus.RaisePropertyChanged(nameof(DeviceStatusCommonInfo.ImageSources));

                        if (string.IsNullOrEmpty(_supplyValvePressureDeviceStatus.StatusName))
                        {
                            _supplyValvePressureDeviceStatus.StatusName = "供阀气压";
                            // 手动触发StatusName的属性变更事件
                            _supplyValvePressureDeviceStatus.RaisePropertyChanged(nameof(DeviceStatusCommonInfo.StatusName));
                        }
                    }
                }

                RaisePropertyChanged(nameof(SupplyValvePressureDeviceStatus));
                RaisePropertyChanged(nameof(SupplyValvePressureStatusName));
                RaisePropertyChanged(nameof(SupplyValvePressureStatusValue));
                RaisePropertyChanged(nameof(SupplyValvePressureStatusUnit));
                RaisePropertyChanged(nameof(SupplyValvePressureStatusSwitch));

                AttachDeviceStatusHandlers();
            }
        }
    }

    /// <summary>
    /// 供阀气压状态名称
    /// </summary>
    [DisplayName("供阀气压状态名称")]
    [Category("供阀气压栏")]
    [PropertySortOrder(21)]
    [Browsable(false)]
    public string SupplyValvePressureStatusName
    {
        get { return _supplyValvePressureDeviceStatus?.StatusName ?? ""; }
        set
        {
            if (_supplyValvePressureDeviceStatus != null && _supplyValvePressureDeviceStatus.StatusName != value)
            {
                _supplyValvePressureDeviceStatus.StatusName = value;
            }
        }
    }

    /// <summary>
    /// 供阀气压设备状态值（仅数字）
    /// </summary>
    [DisplayName("供阀气压值")]
    [Category("供阀气压栏")]
    [PropertySortOrder(22)]
    [Description("供阀气压数值，单位固定为kgf/cm²")]
    [Browsable(false)]
    public string SupplyValvePressureStatusValue
    {
        get
        {
            return _supplyValvePressureDeviceStatus?.DeviceStatusValue ?? "";
        }
        set
        {
            if (_supplyValvePressureDeviceStatus != null)
            {
                if (_supplyValvePressureDeviceStatus.DeviceStatusValue != value)
                    _supplyValvePressureDeviceStatus.DeviceStatusValue = value;
            }
        }
    }

    [DisplayName("供阀气压单位")]
    [Category("供阀气压栏")]
    [PropertySortOrder(23)]
    [Browsable(false)]
    public string SupplyValvePressureStatusUnit
    {
        get { return _supplyValvePressureDeviceStatus?.DeviceStatusUnit ?? ""; }
        set
        {
            if (_supplyValvePressureDeviceStatus != null && _supplyValvePressureDeviceStatus.DeviceStatusUnit != value)
                _supplyValvePressureDeviceStatus.DeviceStatusUnit = value;
        }
    }

    /// <summary>
    /// 供阀气压状态切换
    /// </summary>
    [DisplayName("状态切换")]
    [Category("供阀气压栏")]
    [PropertySortOrder(24)]
    [Browsable(false)]
    public bool SupplyValvePressureStatusSwitch
    {
        get { return (_supplyValvePressureDeviceStatus?.CurrentIndex ?? 0) == 0; }
        set
        {
            if (_supplyValvePressureDeviceStatus != null)
            {
                var newIndex = value ? 0 : 1;
                if (_supplyValvePressureDeviceStatus.CurrentIndex != newIndex)
                {
                    _suppressSupplyValvePressureSwitchChanged = true;
                    try
                    {
                        _supplyValvePressureDeviceStatus.CurrentIndex = newIndex;
                    }
                    finally
                    {
                        _suppressSupplyValvePressureSwitchChanged = false;
                    }
                    RaisePropertyChanged(nameof(SupplyValvePressureStatusSwitch));
                }
            }
        }
    }
    #endregion

    #region 供胶气压设备状态栏属性
    /// <summary>
    /// 供胶气压设备状态
    /// </summary>
    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private DeviceStatusCommonInfo _supplyGluePressureDeviceStatus;
    [Browsable(false)] // 隐藏原始属性
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public DeviceStatusCommonInfo SupplyGluePressureDeviceStatus
    {
        get { return _supplyGluePressureDeviceStatus; }
        set
        {
            if (_supplyGluePressureDeviceStatus != value)
            {
                if (value == null)
                {
                    _supplyGluePressureDeviceStatus = new DeviceStatusCommonInfo();
                }
                else
                {
                    _supplyGluePressureDeviceStatus = value;

                    // 如果传入的对象没有图片数据，添加默认图片
                    if (_supplyGluePressureDeviceStatus.ImageSources == null || _supplyGluePressureDeviceStatus.ImageSources.Count == 0)
                    {
                        _supplyGluePressureDeviceStatus.ImageSources = new List<BitmapData>
                            {
                                LoadBitmapFromAssets("Water.png"),
                                LoadBitmapFromAssets("Close.png")
                            };
                        // 手动触发ImageSources的属性变更事件
                        _supplyGluePressureDeviceStatus.RaisePropertyChanged(nameof(DeviceStatusCommonInfo.ImageSources));

                        if (string.IsNullOrEmpty(_supplyGluePressureDeviceStatus.StatusName))
                        {
                            _supplyGluePressureDeviceStatus.StatusName = "供胶气压";
                            // 手动触发StatusName的属性变更事件
                            _supplyGluePressureDeviceStatus.RaisePropertyChanged(nameof(DeviceStatusCommonInfo.StatusName));
                        }
                    }
                }

                RaisePropertyChanged(nameof(SupplyGluePressureDeviceStatus));
                RaisePropertyChanged(nameof(SupplyGluePressureStatusName));
                RaisePropertyChanged(nameof(SupplyGluePressureStatusValue));
                RaisePropertyChanged(nameof(SupplyGluePressureStatusUnit));
                RaisePropertyChanged(nameof(SupplyGluePressureStatusSwitch));

                AttachDeviceStatusHandlers();
            }
        }
    }

    /// <summary>
    /// 供胶气压状态名称
    /// </summary>
    [DisplayName("供胶气压状态名称")]
    [Category("供胶气压栏")]
    [PropertySortOrder(31)]
    [Browsable(false)]
    public string SupplyGluePressureStatusName
    {
        get { return _supplyGluePressureDeviceStatus?.StatusName ?? ""; }
        set
        {
            if (_supplyGluePressureDeviceStatus != null && _supplyGluePressureDeviceStatus.StatusName != value)
            {
                _supplyGluePressureDeviceStatus.StatusName = value;
            }
        }
    }

    /// <summary>
    /// 供胶气压设备状态值（仅数字）
    /// </summary>
    [DisplayName("供胶气压值")]
    [Category("供胶气压栏")]
    [PropertySortOrder(32)]
    [Description("供胶气压数值，单位固定为kgf/cm²")]
    [Browsable(false)]
    public string SupplyGluePressureStatusValue
    {
        get
        {
            return _supplyGluePressureDeviceStatus?.DeviceStatusValue ?? "";
        }
        set
        {
            if (_supplyGluePressureDeviceStatus != null)
            {
                if (_supplyGluePressureDeviceStatus.DeviceStatusValue != value)
                    _supplyGluePressureDeviceStatus.DeviceStatusValue = value;
            }
        }
    }

    [DisplayName("供胶气压单位")]
    [Category("供胶气压栏")]
    [PropertySortOrder(33)]
    [Browsable(false)]
    public string SupplyGluePressureStatusUnit
    {
        get { return _supplyGluePressureDeviceStatus?.DeviceStatusUnit ?? ""; }
        set
        {
            if (_supplyGluePressureDeviceStatus != null && _supplyGluePressureDeviceStatus.DeviceStatusUnit != value)
                _supplyGluePressureDeviceStatus.DeviceStatusUnit = value;
        }
    }

    /// <summary>
    /// 供胶气压状态切换
    /// </summary>
    [DisplayName("状态切换")]
    [Category("供胶气压栏")]
    [PropertySortOrder(34)]
    [Browsable(false)]
    public bool SupplyGluePressureStatusSwitch
    {
        get { return (_supplyGluePressureDeviceStatus?.CurrentIndex ?? 0) == 0; }
        set
        {
            if (_supplyGluePressureDeviceStatus != null)
            {
                var newIndex = value ? 0 : 1;
                if (_supplyGluePressureDeviceStatus.CurrentIndex != newIndex)
                {
                    _suppressSupplyGluePressureSwitchChanged = true;
                    try
                    {
                        _supplyGluePressureDeviceStatus.CurrentIndex = newIndex;
                    }
                    finally
                    {
                        _suppressSupplyGluePressureSwitchChanged = false;
                    }
                    RaisePropertyChanged(nameof(SupplyGluePressureStatusSwitch));
                }
            }
        }
    }

    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private DeviceStatusCommonInfo _nozzleHeatingDeviceStatus;

    [Browsable(false)]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public DeviceStatusCommonInfo NozzleHeatingDeviceStatus
    {
        get { return _nozzleHeatingDeviceStatus; }
        set
        {
            if (_nozzleHeatingDeviceStatus != value)
            {
                if (value == null)
                {
                    _nozzleHeatingDeviceStatus = new DeviceStatusCommonInfo();
                }
                else
                {
                    _nozzleHeatingDeviceStatus = value;
                    if (_nozzleHeatingDeviceStatus.ImageSources == null || _nozzleHeatingDeviceStatus.ImageSources.Count == 0)
                    {
                        _nozzleHeatingDeviceStatus.ImageSources = new List<BitmapData>
                            {
                                LoadBitmapFromAssets("Water.png"),
                                LoadBitmapFromAssets("Close.png")
                            };
                        _nozzleHeatingDeviceStatus.RaisePropertyChanged(nameof(DeviceStatusCommonInfo.ImageSources));
                        if (string.IsNullOrEmpty(_nozzleHeatingDeviceStatus.StatusName))
                        {
                            _nozzleHeatingDeviceStatus.StatusName = "喷嘴加热";
                            _nozzleHeatingDeviceStatus.RaisePropertyChanged(nameof(DeviceStatusCommonInfo.StatusName));
                        }
                    }
                }

                RaisePropertyChanged(nameof(NozzleHeatingDeviceStatus));
                RaisePropertyChanged(nameof(NozzleHeatingStatusName));
                RaisePropertyChanged(nameof(NozzleHeatingStatusValue));
                RaisePropertyChanged(nameof(NozzleHeatingStatusUnit));
                RaisePropertyChanged(nameof(NozzleHeatingStatusSwitch));

                AttachDeviceStatusHandlers();
            }
        }
    }

    /// <summary>
    /// 喷嘴加热状态名称
    /// </summary>
    [DisplayName("喷嘴加热状态名称")]
    [Category("喷嘴加热栏")]
    [PropertySortOrder(41)]
    [Browsable(false)]
    public string NozzleHeatingStatusName
    {
        get { return _nozzleHeatingDeviceStatus?.StatusName ?? ""; }
        set
        {
            if (_nozzleHeatingDeviceStatus != null && _nozzleHeatingDeviceStatus.StatusName != value)
            {
                _nozzleHeatingDeviceStatus.StatusName = value;
            }
        }
    }

    /// <summary>
    /// 喷嘴加热温度值
    /// </summary>
    [DisplayName("喷嘴加热温度值")]
    [Category("喷嘴加热栏")]
    [PropertySortOrder(42)]
    [Description("喷嘴加热温度数值，单位固定为°C")]
    [Browsable(false)]
    public string NozzleHeatingStatusValue
    {
        get
        {
            return _nozzleHeatingDeviceStatus?.DeviceStatusValue ?? "";
        }
        set
        {
            if (_nozzleHeatingDeviceStatus != null)
            {
                if (_nozzleHeatingDeviceStatus.DeviceStatusValue != value)
                    _nozzleHeatingDeviceStatus.DeviceStatusValue = value;
            }
        }
    }

    [DisplayName("喷嘴加热单位")]
    [Category("喷嘴加热栏")]
    [PropertySortOrder(43)]
    [Browsable(false)]
    public string NozzleHeatingStatusUnit
    {
        get { return _nozzleHeatingDeviceStatus?.DeviceStatusUnit ?? ""; }
        set
        {
            if (_nozzleHeatingDeviceStatus != null && _nozzleHeatingDeviceStatus.DeviceStatusUnit != value)
                _nozzleHeatingDeviceStatus.DeviceStatusUnit = value;
        }
    }

    /// <summary>
    /// 喷嘴加热状态切换
    /// </summary>
    [DisplayName("状态切换")]
    [Category("喷嘴加热栏")]
    [PropertySortOrder(44)]
    [Browsable(false)]
    public bool NozzleHeatingStatusSwitch
    {
        get { return (_nozzleHeatingDeviceStatus?.CurrentIndex ?? 0) == 0; }
        set
        {
            if (_nozzleHeatingDeviceStatus != null)
            {
                var newIndex = value ? 0 : 1;
                if (_nozzleHeatingDeviceStatus.CurrentIndex != newIndex)
                {
                    _suppressNozzleHeatingSwitchChanged = true;
                    try
                    {
                        _nozzleHeatingDeviceStatus.CurrentIndex = newIndex;
                    }
                    finally
                    {
                        _suppressNozzleHeatingSwitchChanged = false;
                    }
                    RaisePropertyChanged(nameof(NozzleHeatingStatusSwitch));
                }
            }
        }
    }

    /// <summary>
    /// 安全门状态
    /// </summary>
    private bool _safetyDoorStatus = true;
    [DisplayName("安全门状态")]
    [Category("监控信息栏")]
    [PropertySortOrder(50)]
    [Browsable(false)]
    public bool SafetyDoorStatus
    {
        get { return _safetyDoorStatus; }
        set
        {
            if (_safetyDoorStatus != value)
            {
                _safetyDoorStatus = value;
                RaisePropertyChanged(nameof(SafetyDoorStatus));
            }
        }
    }

    /// <summary>
    /// 总气压状态
    /// </summary>
    private bool _totalPressureStatus = true;
    [DisplayName("总气压状态")]
    [Category("监控信息栏")]
    [PropertySortOrder(51)]
    [Browsable(false)]
    public bool TotalPressureStatus
    {
        get { return _totalPressureStatus; }
        set
        {
            if (_totalPressureStatus != value)
            {
                _totalPressureStatus = value;
                RaisePropertyChanged(nameof(TotalPressureStatus));
            }
        }
    }

    /// <summary>
    /// 清洁布状态
    /// </summary>
    private bool _cleaningClothStatus = true;
    [DisplayName("清洁布状态")]
    [Category("监控信息栏")]
    [PropertySortOrder(52)]
    [Browsable(false)]
    public bool CleaningClothStatus
    {
        get { return _cleaningClothStatus; }
        set
        {
            if (_cleaningClothStatus != value)
            {
                _cleaningClothStatus = value;
                RaisePropertyChanged(nameof(CleaningClothStatus));
            }
        }
    }

    private bool _isDualValve;
    [DisplayName("双阀模式")]
    [Category("监控信息栏")]
    [PropertySortOrder(53)]
    [Browsable(false)]
    public bool IsDualValve
    {
        get { return _isDualValve; }
        set
        {
            if (_isDualValve != value)
            {
                _isDualValve = value;
                RaisePropertyChanged(nameof(IsDualValve));
            }
        }
    }

    /// <summary>
    /// 胶水余量（mg）
    /// </summary>
    private string _glueRemaining = "50%";
    [DisplayName("胶水余量（mg）")]
    [Category("监控信息栏")]
    [PropertySortOrder(54)]
    [Browsable(false)]
    public string GlueRemaining
    {
        get { return _glueRemaining; }
        set
        {
            if (_glueRemaining != value)
            {
                _glueRemaining = value;
                RaisePropertyChanged(nameof(GlueRemaining));
            }
        }
    }

    /// <summary>
    /// 校正
    /// </summary>
    private string _calibration = "**h|**mg|**pcs";
    [DisplayName("校正")]
    [Category("监控信息栏")]
    [PropertySortOrder(55)]
    [Browsable(false)]
    public string Calibration
    {
        get { return _calibration; }
        set
        {
            if (_calibration != value)
            {
                _calibration = value;
                RaisePropertyChanged(nameof(Calibration));
            }
        }
    }

    /// <summary>
    /// 阀体值
    /// </summary>
    private string _valveBodyValue = "**h";
    [DisplayName("阀体值")]
    [Category("监控信息栏")]
    [PropertySortOrder(56)]
    [Browsable(false)]
    public string ValveBodyValue
    {
        get { return _valveBodyValue; }
        set
        {
            if (_valveBodyValue != value)
            {
                _valveBodyValue = value;
                RaisePropertyChanged(nameof(ValveBodyValue));
            }
        }
    }

    /// <summary>
    /// 阀体自定义图标
    /// </summary>
    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private BitmapData _valveBodyIcon = null;
    [DisplayName("阀体图标")]
    [Category("监控信息栏")]
    [PropertySortOrder(57)]
    [Browsable(false)]
    [Description("选择阀体的自定义图标文件")]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public BitmapData ValveBodyIcon
    {
        get { return _valveBodyIcon; }
        set
        {
            if (_valveBodyIcon != value)
            {
                _valveBodyIcon = value;
                RaisePropertyChanged(nameof(ValveBodyIcon));
            }
        }
    }

    /// <summary>
    /// 密封圈值
    /// </summary>
    private string _sealingRingValue = "**h";
    [DisplayName("密封圈值")]
    [Category("监控信息栏")]
    [PropertySortOrder(58)]
    [Browsable(false)]
    public string SealingRingValue
    {
        get { return _sealingRingValue; }
        set
        {
            if (_sealingRingValue != value)
            {
                _sealingRingValue = value;
                RaisePropertyChanged(nameof(SealingRingValue));
            }
        }
    }

    /// <summary>
    /// 密封圈自定义图标
    /// </summary>
    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private BitmapData _sealingRingIcon = null;
    [DisplayName("密封圈图标")]
    [Category("监控信息栏")]
    [PropertySortOrder(60)]
    [Browsable(false)]
    [Description("选择密封圈的自定义图标文件")]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public BitmapData SealingRingIcon
    {
        get { return _sealingRingIcon; }
        set
        {
            if (_sealingRingIcon != value)
            {
                _sealingRingIcon = value;
                RaisePropertyChanged(nameof(SealingRingIcon));
            }
        }
    }

    private string _leftGlueRemaining = "50%";
    [DisplayName("左阀胶水余量（mg）")]
    [Category("监控信息栏")]
    [PropertySortOrder(61)]
    [Browsable(false)]
    public string LeftGlueRemaining
    {
        get { return _leftGlueRemaining; }
        set
        {
            if (_leftGlueRemaining != value)
            {
                _leftGlueRemaining = value;
                RaisePropertyChanged(nameof(LeftGlueRemaining));
            }
        }
    }

    private string _leftCalibration = "**h|**mg|**pcs";
    [DisplayName("左阀校正")]
    [Category("监控信息栏")]
    [PropertySortOrder(62)]
    [Browsable(false)]
    public string LeftCalibration
    {
        get { return _leftCalibration; }
        set
        {
            if (_leftCalibration != value)
            {
                _leftCalibration = value;
                RaisePropertyChanged(nameof(LeftCalibration));
            }
        }
    }

    private string _leftValveBodyValue = "**h";
    [DisplayName("左阀阀体值")]
    [Category("监控信息栏")]
    [PropertySortOrder(63)]
    [Browsable(false)]
    public string LeftValveBodyValue
    {
        get { return _leftValveBodyValue; }
        set
        {
            if (_leftValveBodyValue != value)
            {
                _leftValveBodyValue = value;
                RaisePropertyChanged(nameof(LeftValveBodyValue));
            }
        }
    }

    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private BitmapData _leftValveBodyIcon = null;
    [DisplayName("左阀阀体图标")]
    [Category("监控信息栏")]
    [PropertySortOrder(64)]
    [Browsable(false)]
    [Description("选择左阀阀体的自定义图标文件")]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public BitmapData LeftValveBodyIcon
    {
        get { return _leftValveBodyIcon; }
        set
        {
            if (_leftValveBodyIcon != value)
            {
                _leftValveBodyIcon = value;
                RaisePropertyChanged(nameof(LeftValveBodyIcon));
            }
        }
    }

    private string _leftSealingRingValue = "**h";
    [DisplayName("左阀密封圈值")]
    [Category("监控信息栏")]
    [PropertySortOrder(65)]
    [Browsable(false)]
    public string LeftSealingRingValue
    {
        get { return _leftSealingRingValue; }
        set
        {
            if (_leftSealingRingValue != value)
            {
                _leftSealingRingValue = value;
                RaisePropertyChanged(nameof(LeftSealingRingValue));
            }
        }
    }

    [NonSerialized]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private BitmapData _leftSealingRingIcon = null;
    [DisplayName("左阀密封圈图标")]
    [Category("监控信息栏")]
    [PropertySortOrder(66)]
    [Browsable(false)]
    [Description("选择左阀密封圈的自定义图标文件")]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public BitmapData LeftSealingRingIcon
    {
        get { return _leftSealingRingIcon; }
        set
        {
            if (_leftSealingRingIcon != value)
            {
                _leftSealingRingIcon = value;
                RaisePropertyChanged(nameof(LeftSealingRingIcon));
            }
        }
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 设置属性值
    /// 根据属性ID设置对应的属性值，支持类型转换
    /// </summary>
    /// <param name="propertyID">属性ID</param>
    /// <param name="propertyVal">属性值</param>
    /// <returns>是否设置成功</returns>
    public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal)
    {
        // 该方法是“宿主属性系统 -> 属性模型”的统一入口：
        // - 运行时管理点订阅写入
        // - 属性面板编辑写入
        // - 保存/加载、复制等流程写入
        // 因此内部需要做大量类型转换/容错，避免因异常导致图元加载失败。
        // 添加空值检查，防止空引用异常
        if (propertyVal == null)
        {
            if (string.Compare(propertyID, nameof(BackColor)) == 0)
            {
                if (_isSettingBackColor)
                    return true;

                _isSettingBackColor = true;
                try
                {
                    var c = Color.FromArgb(33, 0, 0, 0);
                    if (BackColor != c)
                        BackColor = c;
                    return true;
                }
                finally
                {
                    _isSettingBackColor = false;
                }
            }
            if (string.Compare(propertyID, nameof(TextColor)) == 0)
            {
                if (_isSettingTextColor)
                    return true;

                _isSettingTextColor = true;
                try
                {
                    var c = Colors.Black;
                    if (TextColor != c)
                        TextColor = c;
                    return true;
                }
                finally
                {
                    _isSettingTextColor = false;
                }
            }
            if (string.Compare(propertyID, nameof(TextFont)) == 0)
            {
                if (_isSettingTextFont)
                    return true;

                _isSettingTextFont = true;
                try
                {
                    var f = FontInfo.DefaultFont;
                    if (TextFont != f)
                        TextFont = f;
                    return true;
                }
                finally
                {
                    _isSettingTextFont = false;
                }
            }

            return false;
        }

        #region 颜色属性设置
        if (string.Compare(propertyID, nameof(BackColor)) == 0)
        {
            if (_isSettingBackColor)
                return true;

            _isSettingBackColor = true;
            try
            {
                Color newColor;
                try
                {
                    // 尝试直接解析为颜色字符串
                    var colorString = propertyVal.ToPrimitiveValue<string>();
                    if (!string.IsNullOrEmpty(colorString) && colorString.StartsWith("#"))
                    {
                        newColor = Color.Parse(colorString);
                    }
                    else
                    {
                        // 如果不是字符串格式，尝试作为整数处理
                        var intValue = propertyVal.ToPrimitiveValue<uint>();
                        var a = (byte)((intValue >> 24) & 0xFF);
                        var r = (byte)((intValue >> 16) & 0xFF);
                        var g = (byte)((intValue >> 8) & 0xFF);
                        var b = (byte)(intValue & 0xFF);
                        newColor = Color.FromArgb(a, r, g, b);
                    }
                }
                catch
                {
                    // 如果都失败了，使用默认颜色
                    newColor = Color.FromArgb(33, 0, 0, 0);
                }

                if (BackColor != newColor)
                    BackColor = newColor;

                return true;
            }
            finally
            {
                _isSettingBackColor = false;
            }
        }

        if (string.Compare(propertyID, nameof(TextColor)) == 0)
        {
            if (_isSettingTextColor)
                return true;

            _isSettingTextColor = true;
            try
            {
                Color newColor;
                try
                {
                    var colorString = propertyVal.ToPrimitiveValue<string>();
                    if (!string.IsNullOrEmpty(colorString) && colorString.StartsWith("#"))
                    {
                        newColor = Color.Parse(colorString);
                    }
                    else
                    {
                        var intValue = propertyVal.ToPrimitiveValue<uint>();
                        var a = (byte)((intValue >> 24) & 0xFF);
                        var r = (byte)((intValue >> 16) & 0xFF);
                        var g = (byte)((intValue >> 8) & 0xFF);
                        var b = (byte)(intValue & 0xFF);
                        newColor = Color.FromArgb(a, r, g, b);
                    }
                }
                catch
                {
                    newColor = Colors.Black;
                }

                if (TextColor != newColor)
                    TextColor = newColor;

                return true;
            }
            finally
            {
                _isSettingTextColor = false;
            }
        }

        if (string.Compare(propertyID, nameof(TextFont)) == 0)
        {
            if (_isSettingTextFont)
                return true;

            _isSettingTextFont = true;
            try
            {
                ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                IGriffinsBaseValue iMPPropObjectValue = new FontInfo();
                iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                var font = (FontInfo)iMPPropObjectValue;
                if (TextFont != font)
                    TextFont = font;
                return true;
            }
            finally
            {
                _isSettingTextFont = false;
            }
        }
        #endregion

        #region 设备状态栏属性设置
        if (string.Compare(propertyID, nameof(SupplyValvePressureStatusName)) == 0)
        {
            SupplyValvePressureStatusName = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }

        if (string.Compare(propertyID, nameof(SupplyValvePressureStatusValue)) == 0)
        {
            var str = string.Empty;
            try
            {
                str = propertyVal.ToPrimitiveValue<string>();
            }
            catch
            {
                try
                {
                    str = propertyVal.ToPrimitiveValue<decimal>().ToString();
                }
                catch
                {
                    try
                    {
                        str = propertyVal.ToPrimitiveValue<int>().ToString();
                    }
                    catch
                    {
                        str = propertyVal.ToString() ?? string.Empty;
                    }
                }
            }

            SupplyValvePressureStatusValue = str ?? string.Empty;
            return true;
        }

        if (string.Compare(propertyID, nameof(SupplyValvePressureStatusUnit)) == 0)
        {
            SupplyValvePressureStatusUnit = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }

        if (string.Compare(propertyID, nameof(SupplyValvePressureStatusSwitch)) == 0)
        {
            SupplyValvePressureStatusSwitch = ParseBool(propertyVal);
            return true;
        }

        if (string.Compare(propertyID, nameof(SupplyGluePressureStatusName)) == 0)
        {
            SupplyGluePressureStatusName = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }

        if (string.Compare(propertyID, nameof(SupplyGluePressureStatusValue)) == 0)
        {
            var str = string.Empty;
            try
            {
                str = propertyVal.ToPrimitiveValue<string>();
            }
            catch
            {
                try
                {
                    str = propertyVal.ToPrimitiveValue<decimal>().ToString();
                }
                catch
                {
                    try
                    {
                        str = propertyVal.ToPrimitiveValue<int>().ToString();
                    }
                    catch
                    {
                        str = propertyVal.ToString() ?? string.Empty;
                    }
                }
            }

            SupplyGluePressureStatusValue = str ?? string.Empty;
            return true;
        }

        if (string.Compare(propertyID, nameof(SupplyGluePressureStatusUnit)) == 0)
        {
            SupplyGluePressureStatusUnit = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }

        if (string.Compare(propertyID, nameof(NozzleHeatingStatusName)) == 0)
        {
            NozzleHeatingStatusName = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }

        if (string.Compare(propertyID, nameof(NozzleHeatingStatusValue)) == 0)
        {
            var str = string.Empty;
            try
            {
                str = propertyVal.ToPrimitiveValue<string>();
            }
            catch
            {
                try
                {
                    str = propertyVal.ToPrimitiveValue<decimal>().ToString();
                }
                catch
                {
                    try
                    {
                        str = propertyVal.ToPrimitiveValue<int>().ToString();
                    }
                    catch
                    {
                        str = propertyVal.ToString() ?? string.Empty;
                    }
                }
            }

            NozzleHeatingStatusValue = str ?? string.Empty;
            return true;
        }

        if (string.Compare(propertyID, nameof(NozzleHeatingStatusUnit)) == 0)
        {
            NozzleHeatingStatusUnit = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }

        if (string.Compare(propertyID, nameof(SupplyGluePressureStatusSwitch)) == 0)
        {
            SupplyGluePressureStatusSwitch = ParseBool(propertyVal);
            return true;
        }

        if (string.Compare(propertyID, nameof(NozzleHeatingStatusSwitch)) == 0)
        {
            NozzleHeatingStatusSwitch = ParseBool(propertyVal);
            return true;
        }

        if (string.Compare(propertyID, nameof(SupplyValvePressureDeviceStatus)) == 0)
        {
            // 如果propertyVal为null且当前值不为null，保持当前值不变
            if (propertyVal == null && SupplyValvePressureDeviceStatus != null)
            {
                return true;
            }

            DeviceStatusCommonInfo deviceStatus;
            if (propertyVal != null)
            {
                try
                {
                    // 使用正确的转换模式
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new DeviceStatusCommonInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    deviceStatus = (DeviceStatusCommonInfo)iMPPropObjectValue;

                    // 如果转换后的对象没有图片数据，添加默认图片
                    if (deviceStatus.ImageSources == null || deviceStatus.ImageSources.Count == 0)
                    {
                        deviceStatus.ImageSources = new List<BitmapData>
                            {
                                LoadBitmapFromAssets("Water.png"),
                                LoadBitmapFromAssets("Close.png")
                            };
                        // 手动触发ImageSources的属性变更事件
                        deviceStatus.RaisePropertyChanged(nameof(DeviceStatusCommonInfo.ImageSources));

                        if (string.IsNullOrEmpty(deviceStatus.StatusName))
                        {
                            deviceStatus.StatusName = "供阀气压";
                            // 手动触发StatusName的属性变更事件
                            deviceStatus.RaisePropertyChanged(nameof(DeviceStatusCommonInfo.StatusName));
                        }
                    }
                }
                catch
                {
                    // 如果转换失败，使用带有默认图片的默认值
                    deviceStatus = CreateDefaultSupplyValvePressureDeviceStatus();
                }
            }
            else
            {
                // 如果propertyVal为null且当前值也为null，使用默认值
                if (SupplyValvePressureDeviceStatus != null)
                {
                    return true;
                }
                deviceStatus = CreateDefaultSupplyValvePressureDeviceStatus();
            }
            SupplyValvePressureDeviceStatus = deviceStatus;
            return true;
        }
        if (string.Compare(propertyID, nameof(SupplyGluePressureDeviceStatus)) == 0)
        {
            // 如果propertyVal为null且当前值不为null，保持当前值不变
            if (propertyVal == null && SupplyGluePressureDeviceStatus != null)
            {
                return true;
            }

            DeviceStatusCommonInfo deviceStatus;
            if (propertyVal != null)
            {
                try
                {
                    // 使用正确的转换模式
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new DeviceStatusCommonInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    deviceStatus = (DeviceStatusCommonInfo)iMPPropObjectValue;

                    // 如果转换后的对象没有图片数据，添加默认图片
                    if (deviceStatus.ImageSources == null || deviceStatus.ImageSources.Count == 0)
                    {
                        deviceStatus.ImageSources = new List<BitmapData>
                            {
                                LoadBitmapFromAssets("Water.png"),
                                LoadBitmapFromAssets("Close.png")
                            };
                        // 手动触发ImageSources的属性变更事件
                        deviceStatus.RaisePropertyChanged(nameof(DeviceStatusCommonInfo.ImageSources));

                        if (string.IsNullOrEmpty(deviceStatus.StatusName))
                        {
                            deviceStatus.StatusName = "供胶气压";
                            // 手动触发StatusName的属性变更事件
                            deviceStatus.RaisePropertyChanged(nameof(DeviceStatusCommonInfo.StatusName));
                        }
                    }
                }
                catch
                {
                    // 如果转换失败，使用带有默认图片的默认值
                    deviceStatus = CreateDefaultSupplyGluePressureDeviceStatus();
                }
            }
            else
            {
                // 如果propertyVal为null且当前值也为null，使用默认值
                if (SupplyGluePressureDeviceStatus != null)
                {
                    return true;
                }
                deviceStatus = CreateDefaultSupplyGluePressureDeviceStatus();
            }
            SupplyGluePressureDeviceStatus = deviceStatus;
            return true;
        }
        if (string.Compare(propertyID, nameof(NozzleHeatingDeviceStatus)) == 0)
        {
            // 如果propertyVal为null且当前值不为null，保持当前值不变
            if (propertyVal == null && NozzleHeatingDeviceStatus != null)
            {
                return true;
            }

            DeviceStatusCommonInfo deviceStatus;
            if (propertyVal != null)
            {
                try
                {
                    // 使用正确的转换模式
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new DeviceStatusCommonInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    deviceStatus = (DeviceStatusCommonInfo)iMPPropObjectValue;

                    // 如果转换后的对象没有图片数据，添加默认图片
                    if (deviceStatus.ImageSources == null || deviceStatus.ImageSources.Count == 0)
                    {
                        deviceStatus.ImageSources = new List<BitmapData>
                            {
                                LoadBitmapFromAssets("Water.png"),
                                LoadBitmapFromAssets("Close.png")
                            };
                        // 手动触发ImageSources的属性变更事件
                        deviceStatus.RaisePropertyChanged(nameof(DeviceStatusCommonInfo.ImageSources));

                        if (string.IsNullOrEmpty(deviceStatus.StatusName))
                        {
                            deviceStatus.StatusName = "喷嘴加热";
                            // 手动触发StatusName的属性变更事件
                            deviceStatus.RaisePropertyChanged(nameof(DeviceStatusCommonInfo.StatusName));
                        }
                    }
                }
                catch
                {
                    // 如果转换失败，使用带有默认图片的默认值
                    deviceStatus = CreateDefaultNozzleHeatingDeviceStatus();
                }
            }
            else
            {
                // 如果propertyVal为null且当前值也为null，使用默认值
                if (NozzleHeatingDeviceStatus != null)
                {
                    return true;
                }
                deviceStatus = CreateDefaultNozzleHeatingDeviceStatus();
            }
            NozzleHeatingDeviceStatus = deviceStatus;
            return true;
        }
        #endregion

        #region 监控信息栏扩展属性设置
        if (string.Compare(propertyID, nameof(SafetyDoorStatus)) == 0)
        {
            try
            {
                var intValue = propertyVal?.ToPrimitiveValue<int>() ?? 0;
                SafetyDoorStatus = intValue != 0;
            }
            catch
            {
                SafetyDoorStatus = propertyVal?.ToPrimitiveValue<bool>() ?? false;
            }
            return true;
        }
        if (string.Compare(propertyID, nameof(TotalPressureStatus)) == 0)
        {
            try
            {
                var intValue = propertyVal?.ToPrimitiveValue<int>() ?? 0;
                TotalPressureStatus = intValue != 0;
            }
            catch
            {
                TotalPressureStatus = propertyVal?.ToPrimitiveValue<bool>() ?? false;
            }
            return true;
        }
        if (string.Compare(propertyID, nameof(CleaningClothStatus)) == 0)
        {
            try
            {
                var intValue = propertyVal?.ToPrimitiveValue<int>() ?? 0;
                CleaningClothStatus = intValue != 0;
            }
            catch
            {
                CleaningClothStatus = propertyVal?.ToPrimitiveValue<bool>() ?? false;
            }
            return true;
        }
        if (string.Compare(propertyID, nameof(IsDualValve)) == 0)
        {
            // 兼容某些初始化阶段传入null的情况：如果当前已有有效值，则不覆盖。
            if (propertyVal == null && IsDualValve)
            {
                return true;
            }
            IsDualValve = ParseBool(propertyVal);
            return true;
        }
        if (string.Compare(propertyID, nameof(GlueRemaining)) == 0)
        {
            GlueRemaining = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }
        if (string.Compare(propertyID, nameof(LeftGlueRemaining)) == 0)
        {
            if (propertyVal == null && !string.IsNullOrEmpty(LeftGlueRemaining))
            {
                return true;
            }
            LeftGlueRemaining = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }
        if (string.Compare(propertyID, nameof(Calibration)) == 0)
        {
            Calibration = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }
        if (string.Compare(propertyID, nameof(LeftCalibration)) == 0)
        {
            if (propertyVal == null && !string.IsNullOrEmpty(LeftCalibration))
            {
                return true;
            }
            LeftCalibration = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }
        if (string.Compare(propertyID, nameof(ValveBodyValue)) == 0)
        {
            ValveBodyValue = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }
        if (string.Compare(propertyID, nameof(ValveBodyIcon)) == 0)
        {
            try
            {
                ValveBodyIcon = SafeConvertToBitmapData(propertyVal);
            }
            catch
            {
                ValveBodyIcon = null;
            }
            return true;
        }
        if (string.Compare(propertyID, nameof(LeftValveBodyValue)) == 0)
        {
            if (propertyVal == null && !string.IsNullOrEmpty(LeftValveBodyValue))
            {
                return true;
            }
            LeftValveBodyValue = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }
        if (string.Compare(propertyID, nameof(LeftValveBodyIcon)) == 0)
        {
            try
            {
                LeftValveBodyIcon = SafeConvertToBitmapData(propertyVal);
            }
            catch
            {
                LeftValveBodyIcon = null;
            }
            return true;
        }
        if (string.Compare(propertyID, nameof(SealingRingValue)) == 0)
        {
            SealingRingValue = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }
        if (string.Compare(propertyID, nameof(SealingRingIcon)) == 0)
        {
            try
            {
                SealingRingIcon = SafeConvertToBitmapData(propertyVal);
            }
            catch
            {
                SealingRingIcon = null;
            }
            return true;
        }
        if (string.Compare(propertyID, nameof(LeftSealingRingValue)) == 0)
        {
            if (propertyVal == null && !string.IsNullOrEmpty(LeftSealingRingValue))
            {
                return true;
            }
            LeftSealingRingValue = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
            return true;
        }
        if (string.Compare(propertyID, nameof(LeftSealingRingIcon)) == 0)
        {
            try
            {
                LeftSealingRingIcon = SafeConvertToBitmapData(propertyVal);
            }
            catch
            {
                LeftSealingRingIcon = null;
            }
            return true;
        }
        #endregion

        return base.SetPropertyValue(propertyID, propertyVal);
    }

    /// <summary>
    /// 将 <see cref="GriffinsBaseValue"/> 容错解析为布尔值（兼容 bool/int/string 等来源）。
    /// </summary>
    /// <param name="v">输入值</param>
    /// <returns>解析结果</returns>
    private static bool ParseBool(GriffinsBaseValue v)
    {
        // GriffinsBaseValue来源多样，可能是bool/int/string等。
        // 这里按优先级尝试多种方式解析，确保对外部输入有较强容错性。
        if (v == null)
            return false;

        try
        {
            return v.ToPrimitiveValue<bool>();
        }
        catch
        {
        }

        try
        {
            var i = v.ToPrimitiveValue<int>();
            return i != 0;
        }
        catch
        {
        }

        string str;
        try
        {
            str = v.ToPrimitiveValue<string>();
        }
        catch
        {
            str = v.ToString();
        }

        if (string.IsNullOrWhiteSpace(str))
            return false;

        if (bool.TryParse(str, out var b))
            return b;

        if (int.TryParse(str, out var i2))
            return i2 != 0;

        return string.Equals(str.Trim(), "Y", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(str.Trim(), "Yes", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(str.Trim(), "On", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 从另一个模型复制数据（深拷贝设备状态栏对象，并复制监控信息栏字段）。
    public void CopyFrom(DataMonitorPropertyModelEdit source)
    {
        base.CopyFrom(source);

        #region 复制字体和颜色属性
        if (source.TextFont != null)
        {
            this.TextFont = new FontInfo(source.TextFont.FontFamily, source.TextFont.FontSize, source.TextFont.FontWeight, source.TextFont.FontStyle);
        }
        this.TextColor = source.TextColor;
        this.BackColor = source.BackColor;
        #endregion

        #region 复制字符串属性
        // 深度复制DeviceStatusCommonInfo对象，确保图片数据也被正确复制
        if (source.SupplyValvePressureDeviceStatus != null)
        {
            this.SupplyValvePressureDeviceStatus = new DeviceStatusCommonInfo();
            this.SupplyValvePressureDeviceStatus.CopyFrom(source.SupplyValvePressureDeviceStatus);
        }
        else
        {
            this.SupplyValvePressureDeviceStatus = new DeviceStatusCommonInfo();
        }

        if (source.SupplyGluePressureDeviceStatus != null)
        {
            this.SupplyGluePressureDeviceStatus = new DeviceStatusCommonInfo();
            this.SupplyGluePressureDeviceStatus.CopyFrom(source.SupplyGluePressureDeviceStatus);
        }
        else
        {
            this.SupplyGluePressureDeviceStatus = new DeviceStatusCommonInfo();
        }

        if (source.NozzleHeatingDeviceStatus != null)
        {
            this.NozzleHeatingDeviceStatus = new DeviceStatusCommonInfo();
            this.NozzleHeatingDeviceStatus.CopyFrom(source.NozzleHeatingDeviceStatus);
        }
        else
        {
            this.NozzleHeatingDeviceStatus = new DeviceStatusCommonInfo();
        }
        #endregion

        #region 复制监控信息栏属性
        this.SafetyDoorStatus = source.SafetyDoorStatus;
        this.TotalPressureStatus = source.TotalPressureStatus;
        this.CleaningClothStatus = source.CleaningClothStatus;
        this.IsDualValve = source.IsDualValve;
        this.GlueRemaining = source.GlueRemaining;
        this.LeftGlueRemaining = source.LeftGlueRemaining;
        this.Calibration = source.Calibration;
        this.LeftCalibration = source.LeftCalibration;
        this.ValveBodyValue = source.ValveBodyValue;
        this.ValveBodyIcon = source.ValveBodyIcon;
        this.LeftValveBodyValue = source.LeftValveBodyValue;
        this.LeftValveBodyIcon = source.LeftValveBodyIcon;
        this.SealingRingValue = source.SealingRingValue;
        this.SealingRingIcon = source.SealingRingIcon;
        this.LeftSealingRingValue = source.LeftSealingRingValue;
        this.LeftSealingRingIcon = source.LeftSealingRingIcon;
        #endregion
    }
    #endregion
}
#endregion

#region 图元属性绑定模型
[Serializable]
[MapPropertyOrder]
[CategoryPriority("点位信息", 1)]
[CategoryPriority("绑定信息", 2)]
public class DataMonitorPropertyBindEditModel : FunctionalCellPropertyBindEditModel
{
    // 供阀气压设备状态栏绑定属性
    private string _supplyValvePressureDeviceStatus = nameof(DataMonitorPropertyModelEdit.SupplyValvePressureDeviceStatus);
    [DisplayName("供阀气压")]
    [Category("绑定信息")]
    [PropertySortOrder(1)]
    [Browsable(false)]
    public string SupplyValvePressureDeviceStatus { get { return _supplyValvePressureDeviceStatus; } set { SetProperty(ref _supplyValvePressureDeviceStatus, value); } }

    private string _supplyValvePressureStatusName = nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusName);
    [DisplayName("供阀气压状态名称")]
    [Category("绑定信息")]
    [PropertySortOrder(3)]
    [Browsable(false)]
    public string SupplyValvePressureStatusName { get { return _supplyValvePressureStatusName; } set { SetProperty(ref _supplyValvePressureStatusName, value); } }

    private string _supplyValvePressureStatusValue = nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusValue);
    [DisplayName("供阀气压值")]
    [Category("绑定信息")]
    [PropertySortOrder(4)]
    [Browsable(false)]
    public string SupplyValvePressureStatusValue { get { return _supplyValvePressureStatusValue; } set { SetProperty(ref _supplyValvePressureStatusValue, value); } }

    private string _supplyValvePressureStatusUnit = nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusUnit);
    [DisplayName("供阀气压单位")]
    [Category("绑定信息")]
    [PropertySortOrder(5)]
    [Browsable(false)]
    public string SupplyValvePressureStatusUnit { get { return _supplyValvePressureStatusUnit; } set { SetProperty(ref _supplyValvePressureStatusUnit, value); } }

    private string _supplyValvePressureStatusSwitch = nameof(DataMonitorPropertyModelEdit.SupplyValvePressureStatusSwitch);
    [DisplayName("供阀气压状态切换")]
    [Category("绑定信息")]
    [PropertySortOrder(6)]
    [Browsable(false)]
    public string SupplyValvePressureStatusSwitch { get { return _supplyValvePressureStatusSwitch; } set { SetProperty(ref _supplyValvePressureStatusSwitch, value); } }

    // 供胶气压设备状态栏绑定属性
    private string _supplyGluePressureDeviceStatus = nameof(DataMonitorPropertyModelEdit.SupplyGluePressureDeviceStatus);
    [DisplayName("供胶气压")]
    [Category("绑定信息")]
    [PropertySortOrder(2)]
    [Browsable(false)]
    public string SupplyGluePressureDeviceStatus { get { return _supplyGluePressureDeviceStatus; } set { SetProperty(ref _supplyGluePressureDeviceStatus, value); } }

    private string _supplyGluePressureStatusName = nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusName);
    [DisplayName("供胶气压状态名称")]
    [Category("绑定信息")]
    [PropertySortOrder(4)]
    [Browsable(false)]
    public string SupplyGluePressureStatusName { get { return _supplyGluePressureStatusName; } set { SetProperty(ref _supplyGluePressureStatusName, value); } }

    private string _supplyGluePressureStatusValue = nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusValue);
    [DisplayName("供胶气压值")]
    [Category("绑定信息")]
    [PropertySortOrder(7)]
    [Browsable(false)]
    public string SupplyGluePressureStatusValue { get { return _supplyGluePressureStatusValue; } set { SetProperty(ref _supplyGluePressureStatusValue, value); } }

    private string _supplyGluePressureStatusUnit = nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusUnit);
    [DisplayName("供胶气压单位")]
    [Category("绑定信息")]
    [PropertySortOrder(8)]
    [Browsable(false)]
    public string SupplyGluePressureStatusUnit { get { return _supplyGluePressureStatusUnit; } set { SetProperty(ref _supplyGluePressureStatusUnit, value); } }

    private string _supplyGluePressureStatusSwitch = nameof(DataMonitorPropertyModelEdit.SupplyGluePressureStatusSwitch);
    [DisplayName("供胶气压状态切换")]
    [Category("绑定信息")]
    [PropertySortOrder(9)]
    [Browsable(false)]
    public string SupplyGluePressureStatusSwitch { get { return _supplyGluePressureStatusSwitch; } set { SetProperty(ref _supplyGluePressureStatusSwitch, value); } }

    // 喷嘴加热设备状态栏绑定属性
    private string _nozzleHeatingDeviceStatus = nameof(DataMonitorPropertyModelEdit.NozzleHeatingDeviceStatus);
    [DisplayName("喷嘴加热")]
    [Category("绑定信息")]
    [PropertySortOrder(3)]
    [Browsable(false)]
    public string NozzleHeatingDeviceStatus { get { return _nozzleHeatingDeviceStatus; } set { SetProperty(ref _nozzleHeatingDeviceStatus, value); } }

    private string _nozzleHeatingStatusName = nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusName);
    [DisplayName("喷嘴加热状态名称")]
    [Category("绑定信息")]
    [PropertySortOrder(5)]
    [Browsable(false)]
    public string NozzleHeatingStatusName { get { return _nozzleHeatingStatusName; } set { SetProperty(ref _nozzleHeatingStatusName, value); } }

    private string _nozzleHeatingStatusValue = nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusValue);
    [DisplayName("喷嘴加热值")]
    [Category("绑定信息")]
    [PropertySortOrder(10)]
    [Browsable(false)]
    public string NozzleHeatingStatusValue { get { return _nozzleHeatingStatusValue; } set { SetProperty(ref _nozzleHeatingStatusValue, value); } }

    private string _nozzleHeatingStatusUnit = nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusUnit);
    [DisplayName("喷嘴加热单位")]
    [Category("绑定信息")]
    [PropertySortOrder(11)]
    [Browsable(false)]
    public string NozzleHeatingStatusUnit { get { return _nozzleHeatingStatusUnit; } set { SetProperty(ref _nozzleHeatingStatusUnit, value); } }

    private string _nozzleHeatingStatusSwitch = nameof(DataMonitorPropertyModelEdit.NozzleHeatingStatusSwitch);
    [DisplayName("喷嘴加热状态切换")]
    [Category("绑定信息")]
    [PropertySortOrder(12)]
    [Browsable(false)]
    public string NozzleHeatingStatusSwitch { get { return _nozzleHeatingStatusSwitch; } set { SetProperty(ref _nozzleHeatingStatusSwitch, value); } }

    // 监控信息栏绑定属性
    private string _glueRemaining = nameof(DataMonitorPropertyModelEdit.GlueRemaining);
    [DisplayName("胶水余量（mg）")]
    [Category("绑定信息")]
    [PropertySortOrder(13)]
    [Browsable(false)]
    public string GlueRemaining { get { return _glueRemaining; } set { SetProperty(ref _glueRemaining, value); } }

    private string _leftGlueRemaining = nameof(DataMonitorPropertyModelEdit.LeftGlueRemaining);
    [DisplayName("左阀胶水余量")]
    [Category("绑定信息")]
    [PropertySortOrder(26)]
    [Browsable(false)]
    public string LeftGlueRemaining { get { return _leftGlueRemaining; } set { SetProperty(ref _leftGlueRemaining, value); } }

    private string _safetyDoorStatus = nameof(DataMonitorPropertyModelEdit.SafetyDoorStatus);
    [DisplayName("安全门状态")]
    [Category("绑定信息")]
    [PropertySortOrder(22)]
    [Browsable(false)]
    public string SafetyDoorStatus { get { return _safetyDoorStatus; } set { SetProperty(ref _safetyDoorStatus, value); } }

    private string _totalPressureStatus = nameof(DataMonitorPropertyModelEdit.TotalPressureStatus);
    [DisplayName("总气压状态")]
    [Category("绑定信息")]
    [PropertySortOrder(23)]
    [Browsable(false)]
    public string TotalPressureStatus { get { return _totalPressureStatus; } set { SetProperty(ref _totalPressureStatus, value); } }

    private string _cleaningClothStatus = nameof(DataMonitorPropertyModelEdit.CleaningClothStatus);
    [DisplayName("清洁布状态")]
    [Category("绑定信息")]
    [PropertySortOrder(24)]
    [Browsable(false)]
    public string CleaningClothStatus { get { return _cleaningClothStatus; } set { SetProperty(ref _cleaningClothStatus, value); } }

    private string _isDualValve = nameof(DataMonitorPropertyModelEdit.IsDualValve);
    [DisplayName("双阀模式")]
    [Category("绑定信息")]
    [PropertySortOrder(25)]
    [Browsable(false)]
    public string IsDualValve { get { return _isDualValve; } set { SetProperty(ref _isDualValve, value); } }

    private string _calibration = nameof(DataMonitorPropertyModelEdit.Calibration);
    [DisplayName("校正")]
    [Category("绑定信息")]
    [PropertySortOrder(14)]
    [Browsable(false)]
    public string Calibration { get { return _calibration; } set { SetProperty(ref _calibration, value); } }

    private string _leftCalibration = nameof(DataMonitorPropertyModelEdit.LeftCalibration);
    [DisplayName("左阀校正")]
    [Category("绑定信息")]
    [PropertySortOrder(27)]
    [Browsable(false)]
    public string LeftCalibration { get { return _leftCalibration; } set { SetProperty(ref _leftCalibration, value); } }

    private string _valveBodyValue = nameof(DataMonitorPropertyModelEdit.ValveBodyValue);
    [DisplayName("阀体值")]
    [Category("绑定信息")]
    [PropertySortOrder(15)]
    [Browsable(false)]
    public string ValveBodyValue { get { return _valveBodyValue; } set { SetProperty(ref _valveBodyValue, value); } }

    private string _leftValveBodyValue = nameof(DataMonitorPropertyModelEdit.LeftValveBodyValue);
    [DisplayName("左阀阀体值")]
    [Category("绑定信息")]
    [PropertySortOrder(28)]
    [Browsable(false)]
    public string LeftValveBodyValue { get { return _leftValveBodyValue; } set { SetProperty(ref _leftValveBodyValue, value); } }

    private string _sealingRingValue = nameof(DataMonitorPropertyModelEdit.SealingRingValue);
    [DisplayName("密封圈值")]
    [Category("绑定信息")]
    [PropertySortOrder(17)]
    [Browsable(false)]
    public string SealingRingValue { get { return _sealingRingValue; } set { SetProperty(ref _sealingRingValue, value); } }

    private string _leftSealingRingValue = nameof(DataMonitorPropertyModelEdit.LeftSealingRingValue);
    [DisplayName("左阀密封圈值")]
    [Category("绑定信息")]
    [PropertySortOrder(30)]
    [Browsable(false)]
    public string LeftSealingRingValue { get { return _leftSealingRingValue; } set { SetProperty(ref _leftSealingRingValue, value); } }

    // 基础属性绑定
    private string _textFont = nameof(DataMonitorPropertyModelEdit.TextFont);
    [DisplayName("文本字体")]
    [Category("绑定信息")]
    [PropertySortOrder(19)]
    [Browsable(false)]
    public string TextFont { get { return _textFont; } set { SetProperty(ref _textFont, value); } }

    private string _textColor = nameof(DataMonitorPropertyModelEdit.TextColor);
    [DisplayName("文本颜色")]
    [Category("绑定信息")]
    [PropertySortOrder(20)]
    [Browsable(false)]
    public string TextColor { get { return _textColor; } set { SetProperty(ref _textColor, value); } }

    private string _backColor = nameof(DataMonitorPropertyModelEdit.BackColor);
    [DisplayName("背景颜色")]
    [Category("绑定信息")]
    [PropertySortOrder(21)]
    [Browsable(false)]
    public string BackColor { get { return _backColor; } set { SetProperty(ref _backColor, value); } }

    public void CopyFrom(DataMonitorPropertyBindEditModel source)
    {
        base.CopyFrom(source);

        SupplyValvePressureDeviceStatus = source.SupplyValvePressureDeviceStatus;
        SupplyValvePressureStatusName = source.SupplyValvePressureStatusName;
        SupplyValvePressureStatusValue = source.SupplyValvePressureStatusValue;
        SupplyValvePressureStatusUnit = source.SupplyValvePressureStatusUnit;
        SupplyValvePressureStatusSwitch = source.SupplyValvePressureStatusSwitch;
        SupplyGluePressureDeviceStatus = source.SupplyGluePressureDeviceStatus;
        SupplyGluePressureStatusName = source.SupplyGluePressureStatusName;
        NozzleHeatingDeviceStatus = source.NozzleHeatingDeviceStatus;
        NozzleHeatingStatusName = source.NozzleHeatingStatusName;
        SupplyGluePressureStatusValue = source.SupplyGluePressureStatusValue;
        SupplyGluePressureStatusUnit = source.SupplyGluePressureStatusUnit;
        SupplyGluePressureStatusSwitch = source.SupplyGluePressureStatusSwitch;
        NozzleHeatingStatusValue = source.NozzleHeatingStatusValue;
        NozzleHeatingStatusUnit = source.NozzleHeatingStatusUnit;
        NozzleHeatingStatusSwitch = source.NozzleHeatingStatusSwitch;
        GlueRemaining = source.GlueRemaining;
        LeftGlueRemaining = source.LeftGlueRemaining;
        SafetyDoorStatus = source.SafetyDoorStatus;
        TotalPressureStatus = source.TotalPressureStatus;
        CleaningClothStatus = source.CleaningClothStatus;
        IsDualValve = source.IsDualValve;
        Calibration = source.Calibration;
        LeftCalibration = source.LeftCalibration;
        ValveBodyValue = source.ValveBodyValue;
        LeftValveBodyValue = source.LeftValveBodyValue;
        SealingRingValue = source.SealingRingValue;
        LeftSealingRingValue = source.LeftSealingRingValue;
        TextFont = source.TextFont;
        TextColor = source.TextColor;
        BackColor = source.BackColor;
    }
}
#endregion