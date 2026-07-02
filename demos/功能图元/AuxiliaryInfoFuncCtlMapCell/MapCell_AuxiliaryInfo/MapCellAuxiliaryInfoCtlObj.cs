using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using Griffins.Map.UI;
using Griffins.UI2;
using Newtonsoft.JsonG;
using System;
using System.ComponentModel;
using GKG.Map.AuxiliaryInfoFuncCtlMapCell.View;
using GKG.Map.AuxiliaryInfoFuncCtlMapCell.ViewModel;
using Griffins.Map;
using Griffins;

namespace GKG.Map.AuxiliaryInfoFuncCtlMapCell
{
    /// <summary>
    /// 辅助信息图元控制对象类
    /// 负责管理辅助信息图元的所有功能，包括属性注册、事件处理、操作执行等
    /// </summary>
    class MapCellAuxiliaryInfoCtlObj : FunctionalCellBase
    {
        #region 私有字段
        private AuxiliaryInfoView view;
        private AuxiliaryInfoViewModel viewModel;
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化辅助信息图元控制对象（运行时）
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        public MapCellAuxiliaryInfoCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        /// <summary>
        /// 初始化辅助信息图元控制对象
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <param name="designTime">是否为设计时</param>
        public MapCellAuxiliaryInfoCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            base.SetID(mapCellID);
            base.SetName(mapCellName);

            view = new AuxiliaryInfoView();

            #region 注册基础属性（字体、颜色）
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.BackColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Color.FromArgb(1, 0, 0, 0).ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.TextColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_TextColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Colors.Black.ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.TextFont), ResourceA.TextFont, GriffinsBaseDataType.Object_Json, FontInfo.Object_ID, typeof(FontInfo), true, true, new GriffinsBaseValue(new FontInfo(FontInfo.DefaultFont.FontFamily, 16, FontInfo.DefaultFont.FontWeight, FontInfo.DefaultFont.FontStyle))));
            #endregion

            #region 注册程序信息属性
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.ProgramName), ResourceA.ProgramName, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.WorkOrderNo), ResourceA.WorkOrderNo, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            #endregion

            #region 注册胶水信息属性
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueInfo), ResourceA.RightValveGlueInfo, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGluePackageId), ResourceA.RightValveGluePackageId, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueBatchNo), ResourceA.RightValveGlueBatchNo, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueMaterialNo), ResourceA.RightValveGlueMaterialNo, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueProdDate), ResourceA.RightValveGlueProdDate, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.IsDualValve), "双阀模式", GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), true, true, new GriffinsBaseValue(false)));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueInfo), "左阀胶水信息", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGluePackageId), "左阀胶水包装ID", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueBatchNo), "左阀胶水生产批次号", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueMaterialNo), "左阀胶水制造料号", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueProdDate), "左阀胶水生产日期", GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            #endregion

            #region 注册设备信息属性
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.DeviceId), ResourceA.DeviceId, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.DeviceName), ResourceA.DeviceName, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.MacAddress), ResourceA.MacAddress, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.IpAddress), ResourceA.IpAddress, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            #endregion

            #region 注册其他信息属性
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.ProductInfo), ResourceA.ProductInfo, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.GlueName), ResourceA.GlueName, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.PiezoValveSerialNo), ResourceA.PiezoValveSerialNo, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryInfoPropertyModelEdit.ValveId), ResourceA.ValveId, GriffinsBaseDataType.String, Guid.Empty, typeof(string), true, true, null));
            #endregion

            #region 注册事件
            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), GriffinsBaseDataType.Object_Bytes, GraphMouseEventParam.Object_ID));
            #endregion

            #region 注册操作原子信息
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.TextColor_MapOprtCellID, ResourceA.TextColor_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.BackColor_MapOprtCellID, ResourceA.BackColor_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.TextFont_MapOprtCellID, ResourceA.TextFont_MapOprtCellName));

            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.ProgramName_MapOprtCellID, ResourceA.ProgramName));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.WorkOrderNo_MapOprtCellID, ResourceA.WorkOrderNo));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.RightValveGlueInfo_MapOprtCellID, ResourceA.RightValveGlueInfo));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.RightValveGluePackageId_MapOprtCellID, ResourceA.RightValveGluePackageId));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.RightValveGlueBatchNo_MapOprtCellID, ResourceA.RightValveGlueBatchNo));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.RightValveGlueMaterialNo_MapOprtCellID, ResourceA.RightValveGlueMaterialNo));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.RightValveGlueProdDate_MapOprtCellID, ResourceA.RightValveGlueProdDate));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.IsDualValve_MapOprtCellID, "双阀模式"));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.LeftValveGlueInfo_MapOprtCellID, "左阀胶水信息"));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.LeftValveGluePackageId_MapOprtCellID, "左阀胶水包装ID"));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.LeftValveGlueBatchNo_MapOprtCellID, "左阀胶水生产批次号"));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.LeftValveGlueMaterialNo_MapOprtCellID, "左阀胶水制造料号"));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.LeftValveGlueProdDate_MapOprtCellID, "左阀胶水生产日期"));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.DeviceId_MapOprtCellID, ResourceA.DeviceId));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.DeviceName_MapOprtCellID, ResourceA.DeviceName));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.MacAddress_MapOprtCellID, ResourceA.MacAddress));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.IpAddress_MapOprtCellID, ResourceA.IpAddress));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.ProductInfo_MapOprtCellID, ResourceA.ProductInfo));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.GlueName_MapOprtCellID, ResourceA.GlueName));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.PiezoValveSerialNo_MapOprtCellID, ResourceA.PiezoValveSerialNo));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryInfoMapOprtCellConst.ValveId_MapOprtCellID, ResourceA.ValveId));
            #endregion

            #region 注册操作信息
            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.TextColor), ResourceA.TextColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.TextColor_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.BackColor), ResourceA.BackColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.BackColor_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.TextFont), ResourceA.TextFont_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.TextFont_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.ProgramName), ResourceA.ProgramName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.ProgramName_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.WorkOrderNo), ResourceA.WorkOrderNo, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.WorkOrderNo_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueInfo), ResourceA.RightValveGlueInfo, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.RightValveGlueInfo_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGluePackageId), ResourceA.RightValveGluePackageId, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.RightValveGluePackageId_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueBatchNo), ResourceA.RightValveGlueBatchNo, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.RightValveGlueBatchNo_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueMaterialNo), ResourceA.RightValveGlueMaterialNo, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.RightValveGlueMaterialNo_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueProdDate), ResourceA.RightValveGlueProdDate, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.RightValveGlueProdDate_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.IsDualValve), "双阀模式", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.IsDualValve_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueInfo), "左阀胶水信息", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.LeftValveGlueInfo_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGluePackageId), "左阀胶水包装ID", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.LeftValveGluePackageId_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueBatchNo), "左阀胶水生产批次号", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.LeftValveGlueBatchNo_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueMaterialNo), "左阀胶水制造料号", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.LeftValveGlueMaterialNo_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueProdDate), "左阀胶水生产日期", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.LeftValveGlueProdDate_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.DeviceId), ResourceA.DeviceId, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.DeviceId_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.DeviceName), ResourceA.DeviceName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.DeviceName_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.MacAddress), ResourceA.MacAddress, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.MacAddress_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.IpAddress), ResourceA.IpAddress, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.IpAddress_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.ProductInfo), ResourceA.ProductInfo, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.ProductInfo_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.GlueName), ResourceA.GlueName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.GlueName_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.PiezoValveSerialNo), ResourceA.PiezoValveSerialNo, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.PiezoValveSerialNo_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryInfoPropertyModelEdit.ValveId), ResourceA.ValveId, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = AuxiliaryInfoMapOprtCellConst.ValveId_MapOprtCellID, CfgInfo = null }
            }));
            #endregion

            #region 初始化视图和视图模型
            (this as IMapCellTypeBase).Name = ResourceA.AuxiliaryInfo;

            viewModel = new AuxiliaryInfoViewModel(
                AuxiliaryInfoPropertyModelEdit);
            SyncModelToViewModel();
            view.DataContext = viewModel;
            #endregion
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 同步模型数据到视图模型
        /// </summary>
        private void SyncModelToViewModel()
        {
            viewModel.TextFont = AuxiliaryInfoPropertyModelEdit.TextFont;
            viewModel.TextColor = AuxiliaryInfoPropertyModelEdit.TextColor;
            viewModel.BackColor = AuxiliaryInfoPropertyModelEdit.BackColor;
            viewModel.ProgramName = AuxiliaryInfoPropertyModelEdit.ProgramName;
            viewModel.WorkOrderNo = AuxiliaryInfoPropertyModelEdit.WorkOrderNo;
            viewModel.RightValveGlueInfo = AuxiliaryInfoPropertyModelEdit.RightValveGlueInfo;
            viewModel.RightValveGluePackageId = AuxiliaryInfoPropertyModelEdit.RightValveGluePackageId;
            viewModel.RightValveGlueBatchNo = AuxiliaryInfoPropertyModelEdit.RightValveGlueBatchNo;
            viewModel.RightValveGlueMaterialNo = AuxiliaryInfoPropertyModelEdit.RightValveGlueMaterialNo;
            viewModel.RightValveGlueProdDate = AuxiliaryInfoPropertyModelEdit.RightValveGlueProdDate;
            viewModel.IsDualValve = AuxiliaryInfoPropertyModelEdit.IsDualValve;
            viewModel.LeftValveGlueInfo = AuxiliaryInfoPropertyModelEdit.LeftValveGlueInfo;
            viewModel.LeftValveGluePackageId = AuxiliaryInfoPropertyModelEdit.LeftValveGluePackageId;
            viewModel.LeftValveGlueBatchNo = AuxiliaryInfoPropertyModelEdit.LeftValveGlueBatchNo;
            viewModel.LeftValveGlueMaterialNo = AuxiliaryInfoPropertyModelEdit.LeftValveGlueMaterialNo;
            viewModel.LeftValveGlueProdDate = AuxiliaryInfoPropertyModelEdit.LeftValveGlueProdDate;
            viewModel.DeviceId = AuxiliaryInfoPropertyModelEdit.DeviceId;
            viewModel.DeviceName = AuxiliaryInfoPropertyModelEdit.DeviceName;
            viewModel.MacAddress = AuxiliaryInfoPropertyModelEdit.MacAddress;
            viewModel.IpAddress = AuxiliaryInfoPropertyModelEdit.IpAddress;
            viewModel.ProductInfo = AuxiliaryInfoPropertyModelEdit.ProductInfo;
            viewModel.GlueName = AuxiliaryInfoPropertyModelEdit.GlueName;
            viewModel.PiezoValveSerialNo = AuxiliaryInfoPropertyModelEdit.PiezoValveSerialNo;
            viewModel.ValveId = AuxiliaryInfoPropertyModelEdit.ValveId;
        }

        /// <summary>
        /// 执行下线操作
        /// </summary>
        private void offlineExec()
        {
            OpenCommandWindow("Offline", ResourceA.Offline);
        }

        //点击按钮弹出窗口展示按钮是否生效
        private void OpenCommandWindow(string methodId, string methodName)
        {
            if (CallBack == null)
            {
                ShowNotify(methodName, "CallBack为空，无法发送命令到底层");
                return;
            }

            var mpNo = PropertyBindEditModelBase?.MpNo;
            if (string.IsNullOrWhiteSpace(mpNo))
            {
                ShowNotify(methodName, "MpNo为空，无法执行界面数据对象命令");
                return;
            }

            var win = new AuxiliaryCommandWindow();
            var vm = new AuxiliaryCommandViewModel(base.CallBack, mpNo, methodId, methodName);
            win.ViewModel = vm;
            win.Show();
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="message">对话框消息</param>
        private void ShowNotify(string title, string message)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var owner = TopLevel.GetTopLevel(view) as Window;

                var dialog = new Window
                {
                    Title = title,
                    Width = 520,
                    Height = 260,
                };

                var closeCmd = ReactiveUI.ReactiveCommand.Create(() => dialog.Close());
                dialog.Content = new StackPanel
                {
                    Margin = new Thickness(12),
                    Spacing = 12,
                    Children =
                    {
                        new TextBox
                        {
                            Text = message,
                            IsReadOnly = true,
                            TextWrapping = TextWrapping.Wrap,
                            AcceptsReturn = true,
                            Height = 170
                        },
                        new Button
                        {
                            Content = "OK",
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                            Width = 90,
                            Command = closeCmd
                        }
                    }
                };

                if (owner != null)
                    _ = dialog.ShowDialog(owner);
                else
                    dialog.Show();
            });
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取辅助信息属性编辑模型
        /// </summary>
        [Browsable(false)]
        public AuxiliaryInfoPropertyModelEdit AuxiliaryInfoPropertyModelEdit
        {
            get { return PropertyEditModelBase as AuxiliaryInfoPropertyModelEdit; }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="propertyID">属性ID</param>
        /// <param name="propertyVal">属性值</param>
        /// <param name="isRuning">是否运行时</param>
        /// <returns>是否设置成功</returns>
        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            AuxiliaryInfoPropertyModelEdit.IsRuning = isRuning;
            var result = AuxiliaryInfoPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
            return result;
        }

        /// <summary>
        /// 执行操作原子
        /// </summary>
        /// <param name="mapOprtCellInstInfo">操作原子实例信息</param>
        /// <returns>是否执行成功</returns>
        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.TextColor_MapOprtCellID)
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

            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.BackColor_MapOprtCellID)
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

            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.TextFont_MapOprtCellID)
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

            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.ProgramName_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new ProgramNameMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.WorkOrderNo_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new WorkOrderNoMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.RightValveGlueInfo_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new RightValveGlueInfoMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.RightValveGluePackageId_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new RightValveGluePackageIdMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.RightValveGlueBatchNo_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new RightValveGlueBatchNoMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.RightValveGlueMaterialNo_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new RightValveGlueMaterialNoMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.RightValveGlueProdDate_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new RightValveGlueProdDateMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.IsDualValve_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new IsDualValveMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.LeftValveGlueInfo_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new LeftValveGlueInfoMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.LeftValveGluePackageId_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new LeftValveGluePackageIdMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.LeftValveGlueBatchNo_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new LeftValveGlueBatchNoMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.LeftValveGlueMaterialNo_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new LeftValveGlueMaterialNoMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.LeftValveGlueProdDate_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new LeftValveGlueProdDateMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.DeviceId_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new DeviceIdMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.DeviceName_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new DeviceNameMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.MacAddress_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new MacAddressMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.IpAddress_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new IpAddressMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.ProductInfo_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new ProductInfoMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.GlueName_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new GlueNameMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.PiezoValveSerialNo_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new PiezoValveSerialNoMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryInfoMapOprtCellConst.ValveId_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new ValveIdMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }

            return base.ExecOprtCell(mapOprtCellInstInfo);
        }
        #endregion

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

                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.ProgramName)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.ProgramName), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.WorkOrderNo)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.WorkOrderNo), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueInfo)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueInfo), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.RightValveGluePackageId)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGluePackageId), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueBatchNo)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueBatchNo), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueMaterialNo)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueMaterialNo), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueProdDate)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueProdDate), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.IsDualValve)) == 0)
                {
                    if (gFBaseTypePropValue.Value == null)
                        continue;
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.IsDualValve), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueInfo)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueInfo), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGluePackageId)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGluePackageId), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueBatchNo)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueBatchNo), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueMaterialNo)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueMaterialNo), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueProdDate)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueProdDate), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.DeviceId)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.DeviceId), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.DeviceName)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.DeviceName), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.MacAddress)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.MacAddress), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.IpAddress)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.IpAddress), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.ProductInfo)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.ProductInfo), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.GlueName)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.GlueName), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.PiezoValveSerialNo)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.PiezoValveSerialNo), gFBaseTypePropValue.Value, true);
                    continue;
                }
                if (string.Compare(propId, nameof(AuxiliaryInfoPropertyModelEdit.ValveId)) == 0)
                {
                    SetPropertyValue(nameof(AuxiliaryInfoPropertyModelEdit.ValveId), gFBaseTypePropValue.Value, true);
                    continue;
                }
            }
            return true;
        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.TextColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.TextColor));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.BackColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.BackColor));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.TextFont)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.TextFont));
            }

            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.ProgramName)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.ProgramName));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.WorkOrderNo)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.WorkOrderNo));
            }

            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueInfo)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueInfo));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.RightValveGluePackageId)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGluePackageId));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueBatchNo)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueBatchNo));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueMaterialNo)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueMaterialNo));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueProdDate)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueProdDate));
            }

            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.IsDualValve)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.IsDualValve));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueInfo)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueInfo));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGluePackageId)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGluePackageId));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueBatchNo)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueBatchNo));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueMaterialNo)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueMaterialNo));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueProdDate)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueProdDate));
            }

            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.DeviceId)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.DeviceId));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.DeviceName)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.DeviceName));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.MacAddress)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.MacAddress));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.IpAddress)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.IpAddress));
            }

            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.ProductInfo)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.ProductInfo));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.GlueName)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.GlueName));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.PiezoValveSerialNo)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.PiezoValveSerialNo));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryInfoPropertyModelEdit.ValveId)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryInfoPropertyModelEdit.ValveId));
            }
            if (!AuxiliaryInfoPropertyModelEdit.IsRuning)
            {
                GFBaseTypePropValueList gFBaseTypePropValues = new GFBaseTypePropValueList();

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.TextColor)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.TextColor.ToColorString())));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.BackColor)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.BackColor.ToColorString())));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.TextFont)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.TextFont)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.ProgramName)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.ProgramName ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.WorkOrderNo)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.WorkOrderNo ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueInfo)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.RightValveGlueInfo ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGluePackageId)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.RightValveGluePackageId ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueBatchNo)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.RightValveGlueBatchNo ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueMaterialNo)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.RightValveGlueMaterialNo ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueProdDate)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.RightValveGlueProdDate ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.IsDualValve)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.IsDualValve)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueInfo)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.LeftValveGlueInfo ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGluePackageId)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.LeftValveGluePackageId ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueBatchNo)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.LeftValveGlueBatchNo ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueMaterialNo)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.LeftValveGlueMaterialNo ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueProdDate)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.LeftValveGlueProdDate ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.DeviceId)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.DeviceId ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.DeviceName)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.DeviceName ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.MacAddress)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.MacAddress ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.IpAddress)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.IpAddress ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.ProductInfo)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.ProductInfo ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.GlueName)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.GlueName ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.PiezoValveSerialNo)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.PiezoValveSerialNo ?? string.Empty)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(AuxiliaryInfoPropertyModelEdit.ValveId)), new GriffinsBaseValue(AuxiliaryInfoPropertyModelEdit.ValveId ?? string.Empty)));

                CallBack?.UpdateUIDataObjPropValues(gFBaseTypePropValues);
            }
        }

        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null!;
        }

        #region 重写方法
        /// <summary>
        /// 从字节流中读画图信息
        /// </summary>
        /// <param name="br">字节流读取对象</param>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            var propertyEditModelBase = JsonObjConvert.FromJSon<AuxiliaryInfoPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            (PropertyEditModelBase as AuxiliaryInfoPropertyModelEdit).CopyFrom(propertyEditModelBase);
            var propertyBindEditModelBase = JsonObjConvert.FromJSon<AuxiliaryInfoPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));

            (PropertyBindEditModelBase as AuxiliaryInfoPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
            var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(br.ReadString("EventBindEditModel"));
            EventBindEditModel.CopyFrom(eventBindEditModel);
            SyncModelToViewModel();
        }

        /// <summary>
        /// 当把画图信息写入到字节流中
        /// </summary>
        /// <param name="bw">字节流写入对象</param>
        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
        }

        /// <summary>
        /// 从来源实例复制字段到本实例
        /// </summary>
        /// <param name="source">来源实例</param>
        protected override void OnCopyFrom(FunctionalCellBase source)
        {
            var src = source as MapCellAuxiliaryInfoCtlObj;
            base._CopyFrom(src);
            PropertyEditModelBase.CopyFrom(source.PropertyEditModelBase);
            PropertyBindEditModelBase.CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel.CopyFrom(source.EventBindEditModel);
        }

        /// <summary>
        /// 初始化时
        /// </summary>
        protected override void OnInit()
        {
            if (CallBack == null)
                view.DataContext = viewModel;
        }

        /// <summary>
        /// 获取视图
        /// </summary>
        /// <returns>视图对象</returns>
        protected override object OnGetView()
        {
            return view;
        }

        /// <summary>
        /// 获取视图模型
        /// </summary>
        /// <returns>视图模型对象</returns>
        protected override object OnGetViewModel()
        {
            return viewModel;
        }

        /// <summary>
        /// 创建图元属性编辑模型对象
        /// </summary>
        /// <returns>图元属性编辑模型对象</returns>
        public override PropertyEditModelBase CreatePropertyModelEditBase()
        {
            return new AuxiliaryInfoPropertyModelEdit();
        }

        /// <summary>
        /// 创建图元属性绑定编辑模型对象
        /// </summary>
        /// <returns>图元属性绑定编辑模型对象</returns>
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
        {
            var m = new AuxiliaryInfoPropertyBindEditModel();
            return m;
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

        /// <summary>
        /// 缩放变化时的处理
        /// </summary>
        public override void OnZoomChanged()
        {
            SetButtonTextFont();
        }

        /// <summary>
        /// 设置按钮文本字体
        /// </summary>
        internal void SetButtonTextFont()
        {
            double size = base.CallBack?.Calc?.CalcZoomVal((decimal)this.AuxiliaryInfoPropertyModelEdit.TextFont.FontSize) ?? this.AuxiliaryInfoPropertyModelEdit.TextFont.FontSize;
            if (size < 2)
                size = 2;
            FontInfo font = new FontInfo(this.AuxiliaryInfoPropertyModelEdit.TextFont.FontFamily, size, this.AuxiliaryInfoPropertyModelEdit.TextFont.FontWeight, this.AuxiliaryInfoPropertyModelEdit.TextFont.FontStyle);
            this.viewModel.TextFont = font;
        }
        #endregion

        #region 操作原子执行对象
        /// <summary>
        /// 文本颜色操作原子执行对象
        /// </summary>
        private class TextColorMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public TextColorMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner)
            {
                _owner = owner;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                {
                    vm.TextColor = _owner.AuxiliaryInfoPropertyModelEdit.TextColor;
                }
            }
        }

        /// <summary>
        /// 背景颜色操作原子执行对象
        /// </summary>
        private class BackColorMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public BackColorMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner)
            {
                _owner = owner;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                {
                    vm.BackColor = _owner.AuxiliaryInfoPropertyModelEdit.BackColor;
                }
            }
        }

        /// <summary>
        /// 文本字体操作原子执行对象
        /// </summary>
        private class TextFontMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public TextFontMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner)
            {
                _owner = owner;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                {
                    _owner.SetButtonTextFont();
                }
            }
        }

        private class ProgramNameMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public ProgramNameMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.ProgramName = _owner.AuxiliaryInfoPropertyModelEdit.ProgramName;
            }
        }

        private class WorkOrderNoMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public WorkOrderNoMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.WorkOrderNo = _owner.AuxiliaryInfoPropertyModelEdit.WorkOrderNo;
            }
        }

        private class RightValveGlueInfoMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public RightValveGlueInfoMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.RightValveGlueInfo = _owner.AuxiliaryInfoPropertyModelEdit.RightValveGlueInfo;
            }
        }

        private class RightValveGluePackageIdMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public RightValveGluePackageIdMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.RightValveGluePackageId = _owner.AuxiliaryInfoPropertyModelEdit.RightValveGluePackageId;
            }
        }

        private class RightValveGlueBatchNoMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public RightValveGlueBatchNoMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.RightValveGlueBatchNo = _owner.AuxiliaryInfoPropertyModelEdit.RightValveGlueBatchNo;
            }
        }

        private class RightValveGlueMaterialNoMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public RightValveGlueMaterialNoMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.RightValveGlueMaterialNo = _owner.AuxiliaryInfoPropertyModelEdit.RightValveGlueMaterialNo;
            }
        }

        private class RightValveGlueProdDateMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public RightValveGlueProdDateMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.RightValveGlueProdDate = _owner.AuxiliaryInfoPropertyModelEdit.RightValveGlueProdDate;
            }
        }

        private class IsDualValveMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public IsDualValveMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.IsDualValve = _owner.AuxiliaryInfoPropertyModelEdit.IsDualValve;
            }
        }

        private class LeftValveGlueInfoMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public LeftValveGlueInfoMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.LeftValveGlueInfo = _owner.AuxiliaryInfoPropertyModelEdit.LeftValveGlueInfo;
            }
        }

        private class LeftValveGluePackageIdMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public LeftValveGluePackageIdMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.LeftValveGluePackageId = _owner.AuxiliaryInfoPropertyModelEdit.LeftValveGluePackageId;
            }
        }

        private class LeftValveGlueBatchNoMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public LeftValveGlueBatchNoMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.LeftValveGlueBatchNo = _owner.AuxiliaryInfoPropertyModelEdit.LeftValveGlueBatchNo;
            }
        }

        private class LeftValveGlueMaterialNoMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public LeftValveGlueMaterialNoMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.LeftValveGlueMaterialNo = _owner.AuxiliaryInfoPropertyModelEdit.LeftValveGlueMaterialNo;
            }
        }

        private class LeftValveGlueProdDateMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public LeftValveGlueProdDateMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.LeftValveGlueProdDate = _owner.AuxiliaryInfoPropertyModelEdit.LeftValveGlueProdDate;
            }
        }

        private class DeviceIdMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public DeviceIdMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.DeviceId = _owner.AuxiliaryInfoPropertyModelEdit.DeviceId;
            }
        }

        private class DeviceNameMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public DeviceNameMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.DeviceName = _owner.AuxiliaryInfoPropertyModelEdit.DeviceName;
            }
        }

        private class MacAddressMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public MacAddressMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.MacAddress = _owner.AuxiliaryInfoPropertyModelEdit.MacAddress;
            }
        }

        private class IpAddressMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public IpAddressMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.IpAddress = _owner.AuxiliaryInfoPropertyModelEdit.IpAddress;
            }
        }

        private class ProductInfoMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public ProductInfoMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.ProductInfo = _owner.AuxiliaryInfoPropertyModelEdit.ProductInfo;
            }
        }

        private class GlueNameMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public GlueNameMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.GlueName = _owner.AuxiliaryInfoPropertyModelEdit.GlueName;
            }
        }

        private class PiezoValveSerialNoMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public PiezoValveSerialNoMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.PiezoValveSerialNo = _owner.AuxiliaryInfoPropertyModelEdit.PiezoValveSerialNo;
            }
        }

        private class ValveIdMapOprtCellExector : IMapOprtCellExector
        {
            private readonly MapCellAuxiliaryInfoCtlObj _owner;
            private IMapOprtCellExectorCallBack callBack;

            public ValveIdMapOprtCellExector(MapCellAuxiliaryInfoCtlObj owner) => _owner = owner;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is AuxiliaryInfoViewModel vm)
                    vm.ValveId = _owner.AuxiliaryInfoPropertyModelEdit.ValveId;
            }
        }
        #endregion
    }

    /// <summary>
    /// 辅助信息功能图元属性模型编辑类
    /// 负责管理辅助信息图元的所有属性，包括字体、颜色、程序信息、设备信息等
    /// 支持属性面板编辑和数据持久化
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    public class AuxiliaryInfoPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        #region 构造函数
        /// <summary>
        /// 初始化辅助信息属性模型
        /// </summary>
        public AuxiliaryInfoPropertyModelEdit()
        {
            TextFont.PropertyChanged += textFont_PropertyChanged;
        }
        #endregion

        #region 私有方法
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
            set { SetProperty(ref _textFont, value); }
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
            set { SetProperty(ref _textColor, value); }
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
            set { SetProperty(ref _backColor, value); }
        }

        /// <summary>
        /// 程序名称
        /// </summary>
        private string _programName = "James underfill";
        [DisplayName("程序名称")]
        [Category("图元信息")]
        [PropertySortOrder(20)]
        [Browsable(false)]
        public string ProgramName
        {
            get { return _programName; }
            set { SetProperty(ref _programName, value); }
        }

        /// <summary>
        /// 工单号
        /// </summary>
        private string _workOrderNo = "2586525000000058";
        [DisplayName("工单号")]
        [Category("图元信息")]
        [PropertySortOrder(21)]
        [Browsable(false)]
        public string WorkOrderNo
        {
            get { return _workOrderNo; }
            set { SetProperty(ref _workOrderNo, value); }
        }

        /// <summary>
        /// 右阀胶水信息
        /// </summary>
        private string _rightValveGlueInfo = "011";
        [DisplayName("右阀胶水信息")]
        [Category("图元信息")]
        [PropertySortOrder(22)]
        [Browsable(false)]
        public string RightValveGlueInfo
        {
            get { return _rightValveGlueInfo; }
            set { SetProperty(ref _rightValveGlueInfo, value); }
        }

        /// <summary>
        /// 右阀胶水包装ID
        /// </summary>
        private string _rightValveGluePackageId = "020025800";
        [DisplayName("右阀胶水包装ID")]
        [Category("图元信息")]
        [PropertySortOrder(23)]
        [Browsable(false)]
        public string RightValveGluePackageId
        {
            get { return _rightValveGluePackageId; }
            set { SetProperty(ref _rightValveGluePackageId, value); }
        }

        /// <summary>
        /// 右阀胶水生产批次号
        /// </summary>
        private string _rightValveGlueBatchNo = "08896586";
        [DisplayName("右阀胶水生产批次号")]
        [Category("图元信息")]
        [PropertySortOrder(24)]
        [Browsable(false)]
        public string RightValveGlueBatchNo
        {
            get { return _rightValveGlueBatchNo; }
            set { SetProperty(ref _rightValveGlueBatchNo, value); }
        }

        /// <summary>
        /// 右阀胶水制造料号
        /// </summary>
        private string _rightValveGlueMaterialNo = "0885858965";
        [DisplayName("右阀胶水制造料号")]
        [Category("图元信息")]
        [PropertySortOrder(25)]
        [Browsable(false)]
        public string RightValveGlueMaterialNo
        {
            get { return _rightValveGlueMaterialNo; }
            set { SetProperty(ref _rightValveGlueMaterialNo, value); }
        }

        /// <summary>
        /// 右阀胶水生产日期
        /// </summary>
        private string _rightValveGlueProdDate = "2023-02-23";
        [DisplayName("右阀胶水生产日期")]
        [Category("图元信息")]
        [PropertySortOrder(26)]
        [Browsable(false)]
        public string RightValveGlueProdDate
        {
            get { return _rightValveGlueProdDate; }
            set { SetProperty(ref _rightValveGlueProdDate, value); }
        }

        /// <summary>
        /// 双阀模式标识
        /// </summary>
        private bool _isDualValve = false;
        [DisplayName("双阀模式")]
        [Category("图元信息")]
        [PropertySortOrder(27)]
        [Browsable(false)]
        public bool IsDualValve
        {
            get { return _isDualValve; }
            set { SetProperty(ref _isDualValve, value); }
        }

        /// <summary>
        /// 左阀胶水信息
        /// </summary>
        private string _leftValveGlueInfo = "022";
        [DisplayName("左阀胶水信息")]
        [Category("图元信息")]
        [PropertySortOrder(28)]
        [Browsable(false)]
        public string LeftValveGlueInfo
        {
            get { return _leftValveGlueInfo; }
            set { SetProperty(ref _leftValveGlueInfo, value); }
        }

        /// <summary>
        /// 左阀胶水包装ID
        /// </summary>
        private string _leftValveGluePackageId = "03242141";
        [DisplayName("左阀胶水包装ID")]
        [Category("图元信息")]
        [PropertySortOrder(29)]
        [Browsable(false)]
        public string LeftValveGluePackageId
        {
            get { return _leftValveGluePackageId; }
            set { SetProperty(ref _leftValveGluePackageId, value); }
        }

        /// <summary>
        /// 左阀胶水生产批次号
        /// </summary>
        private string _leftValveGlueBatchNo = "077141531";
        [DisplayName("左阀胶水生产批次号")]
        [Category("图元信息")]
        [PropertySortOrder(30)]
        [Browsable(false)]
        public string LeftValveGlueBatchNo
        {
            get { return _leftValveGlueBatchNo; }
            set { SetProperty(ref _leftValveGlueBatchNo, value); }
        }

        /// <summary>
        /// 左阀胶水制造料号
        /// </summary>
        private string _leftValveGlueMaterialNo = "1256215";
        [DisplayName("左阀胶水制造料号")]
        [Category("图元信息")]
        [PropertySortOrder(31)]
        [Browsable(false)]
        public string LeftValveGlueMaterialNo
        {
            get { return _leftValveGlueMaterialNo; }
            set { SetProperty(ref _leftValveGlueMaterialNo, value); }
        }

        /// <summary>
        /// 左阀胶水生产日期
        /// </summary>
        private string _leftValveGlueProdDate = "2026-03-11";
        [DisplayName("左阀胶水生产日期")]
        [Category("图元信息")]
        [PropertySortOrder(32)]
        [Browsable(false)]
        public string LeftValveGlueProdDate
        {
            get { return _leftValveGlueProdDate; }
            set { SetProperty(ref _leftValveGlueProdDate, value); }
        }

        /// <summary>
        /// 设备ID
        /// </summary>
        private string _deviceId = "点胶机";
        [DisplayName("设备ID")]
        [Category("图元信息")]
        [PropertySortOrder(33)]
        [Browsable(false)]
        public string DeviceId
        {
            get { return _deviceId; }
            set { SetProperty(ref _deviceId, value); }
        }

        /// <summary>
        /// 设备名称
        /// </summary>
        private string _deviceName = "0.000";
        [DisplayName("设备名称")]
        [Category("图元信息")]
        [PropertySortOrder(34)]
        [Browsable(false)]
        public string DeviceName
        {
            get { return _deviceName; }
            set { SetProperty(ref _deviceName, value); }
        }

        /// <summary>
        /// Mac地址
        /// </summary>
        private string _macAddress = "地址";
        [DisplayName("Mac地址")]
        [Category("图元信息")]
        [PropertySortOrder(35)]
        [Browsable(false)]
        public string MacAddress
        {
            get { return _macAddress; }
            set { SetProperty(ref _macAddress, value); }
        }

        /// <summary>
        /// IP地址
        /// </summary>
        private string _ipAddress = "地址";
        [DisplayName("IP地址")]
        [Category("图元信息")]
        [PropertySortOrder(36)]
        [Browsable(false)]
        public string IpAddress
        {
            get { return _ipAddress; }
            set { SetProperty(ref _ipAddress, value); }
        }

        /// <summary>
        /// 产品信息
        /// </summary>
        private string _productInfo = "信息";
        [DisplayName("产品信息")]
        [Category("图元信息")]
        [PropertySortOrder(37)]
        [Browsable(false)]
        public string ProductInfo
        {
            get { return _productInfo; }
            set { SetProperty(ref _productInfo, value); }
        }

        /// <summary>
        /// 胶水名称
        /// </summary>
        private string _glueName = "胶水";
        [DisplayName("胶水名称")]
        [Category("图元信息")]
        [PropertySortOrder(38)]
        [Browsable(false)]
        public string GlueName
        {
            get { return _glueName; }
            set { SetProperty(ref _glueName, value); }
        }

        /// <summary>
        /// 压电阀序号
        /// </summary>
        private string _piezoValveSerialNo = "03";
        [DisplayName("压电阀序号")]
        [Category("图元信息")]
        [PropertySortOrder(39)]
        [Browsable(false)]
        public string PiezoValveSerialNo
        {
            get { return _piezoValveSerialNo; }
            set { SetProperty(ref _piezoValveSerialNo, value); }
        }

        /// <summary>
        /// 阀ID
        /// </summary>
        private string _valveId = "1";
        [DisplayName("阀ID")]
        [Category("图元信息")]
        [PropertySortOrder(40)]
        [Browsable(false)]
        public string ValveId
        {
            get { return _valveId; }
            set { SetProperty(ref _valveId, value); }
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
            #region 颜色属性设置
            if (string.Compare(propertyID, nameof(BackColor)) == 0)
            {
                if (propertyVal == null) BackColor = Color.FromArgb(33, 0, 0, 0);
                else BackColor = Color.Parse(propertyVal.ToPrimitiveValue<string>());
                return true;
            }

            if (string.Compare(propertyID, nameof(TextColor)) == 0)
            {
                if (propertyVal == null) TextColor = Colors.Black;
                else TextColor = Color.Parse(propertyVal.ToPrimitiveValue<string>());
                return true;
            }

            if (string.Compare(propertyID, nameof(TextFont)) == 0)
            {
                if (propertyVal == null) TextFont = new FontInfo(FontInfo.DefaultFont.FontFamily, 16, FontInfo.DefaultFont.FontWeight, FontInfo.DefaultFont.FontStyle);
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
            #endregion

            #region 字符串属性设置
            if (string.Compare(propertyID, nameof(ProgramName)) == 0) { ProgramName = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(WorkOrderNo)) == 0) { WorkOrderNo = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(RightValveGlueInfo)) == 0) { RightValveGlueInfo = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(RightValveGluePackageId)) == 0) { RightValveGluePackageId = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(RightValveGlueBatchNo)) == 0) { RightValveGlueBatchNo = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(RightValveGlueMaterialNo)) == 0) { RightValveGlueMaterialNo = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(RightValveGlueProdDate)) == 0) { RightValveGlueProdDate = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(IsDualValve)) == 0) { IsDualValve = propertyVal?.ToPrimitiveValue<bool>() ?? false; return true; }
            if (string.Compare(propertyID, nameof(LeftValveGlueInfo)) == 0) { LeftValveGlueInfo = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(LeftValveGluePackageId)) == 0) { LeftValveGluePackageId = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(LeftValveGlueBatchNo)) == 0) { LeftValveGlueBatchNo = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(LeftValveGlueMaterialNo)) == 0) { LeftValveGlueMaterialNo = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(LeftValveGlueProdDate)) == 0) { LeftValveGlueProdDate = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(DeviceId)) == 0) { DeviceId = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(DeviceName)) == 0) { DeviceName = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(MacAddress)) == 0) { MacAddress = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(IpAddress)) == 0) { IpAddress = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(ProductInfo)) == 0) { ProductInfo = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(GlueName)) == 0) { GlueName = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(PiezoValveSerialNo)) == 0) { PiezoValveSerialNo = propertyVal?.ToPrimitiveValue<string>(); return true; }
            if (string.Compare(propertyID, nameof(ValveId)) == 0) { ValveId = propertyVal?.ToPrimitiveValue<string>(); return true; }
            #endregion

            return base.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 从另一个模型复制数据
        /// 将源模型的所有属性值复制到当前模型
        /// <param name="source">源模型</param>
        public void CopyFrom(AuxiliaryInfoPropertyModelEdit source)
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
            this.ProgramName = source.ProgramName;
            this.WorkOrderNo = source.WorkOrderNo;
            this.RightValveGlueInfo = source.RightValveGlueInfo;
            this.RightValveGluePackageId = source.RightValveGluePackageId;
            this.RightValveGlueBatchNo = source.RightValveGlueBatchNo;
            this.RightValveGlueMaterialNo = source.RightValveGlueMaterialNo;
            this.RightValveGlueProdDate = source.RightValveGlueProdDate;
            this.IsDualValve = source.IsDualValve;
            this.LeftValveGlueInfo = source.LeftValveGlueInfo;
            this.LeftValveGluePackageId = source.LeftValveGluePackageId;
            this.LeftValveGlueBatchNo = source.LeftValveGlueBatchNo;
            this.LeftValveGlueMaterialNo = source.LeftValveGlueMaterialNo;
            this.LeftValveGlueProdDate = source.LeftValveGlueProdDate;
            this.DeviceId = source.DeviceId;
            this.DeviceName = source.DeviceName;
            this.MacAddress = source.MacAddress;
            this.IpAddress = source.IpAddress;
            this.ProductInfo = source.ProductInfo;
            this.GlueName = source.GlueName;
            this.PiezoValveSerialNo = source.PiezoValveSerialNo;
            this.ValveId = source.ValveId;
            #endregion
        }
        #endregion
    }

    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class AuxiliaryInfoPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        private string _programName = nameof(AuxiliaryInfoPropertyModelEdit.ProgramName);
        [DisplayName("程序名称")]
        [Category("绑定信息")]
        [PropertySortOrder(1)]
        [Browsable(false)]
        public string ProgramName { get { return _programName; } set { SetProperty(ref _programName, value); } }

        private string _workOrderNo = nameof(AuxiliaryInfoPropertyModelEdit.WorkOrderNo);
        [DisplayName("工单号")]
        [Category("绑定信息")]
        [PropertySortOrder(2)]
        [Browsable(false)]
        public string WorkOrderNo { get { return _workOrderNo; } set { SetProperty(ref _workOrderNo, value); } }

        private string _rightValveGlueInfo = nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueInfo);
        [DisplayName("右阀胶水信息")]
        [Category("绑定信息")]
        [PropertySortOrder(3)]
        [Browsable(false)]
        public string RightValveGlueInfo { get { return _rightValveGlueInfo; } set { SetProperty(ref _rightValveGlueInfo, value); } }

        private string _rightValveGluePackageId = nameof(AuxiliaryInfoPropertyModelEdit.RightValveGluePackageId);
        [DisplayName("右阀胶水包装ID")]
        [Category("绑定信息")]
        [PropertySortOrder(4)]
        [Browsable(false)]
        public string RightValveGluePackageId { get { return _rightValveGluePackageId; } set { SetProperty(ref _rightValveGluePackageId, value); } }

        private string _rightValveGlueBatchNo = nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueBatchNo);
        [DisplayName("右阀胶水生产批次号")]
        [Category("绑定信息")]
        [PropertySortOrder(5)]
        [Browsable(false)]
        public string RightValveGlueBatchNo { get { return _rightValveGlueBatchNo; } set { SetProperty(ref _rightValveGlueBatchNo, value); } }

        private string _rightValveGlueMaterialNo = nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueMaterialNo);
        [DisplayName("右阀胶水制造料号")]
        [Category("绑定信息")]
        [PropertySortOrder(6)]
        [Browsable(false)]
        public string RightValveGlueMaterialNo { get { return _rightValveGlueMaterialNo; } set { SetProperty(ref _rightValveGlueMaterialNo, value); } }

        private string _rightValveGlueProdDate = nameof(AuxiliaryInfoPropertyModelEdit.RightValveGlueProdDate);
        [DisplayName("右阀胶水生产日期")]
        [Category("绑定信息")]
        [PropertySortOrder(7)]
        [Browsable(false)]
        public string RightValveGlueProdDate { get { return _rightValveGlueProdDate; } set { SetProperty(ref _rightValveGlueProdDate, value); } }

        private string _isDualValve = nameof(AuxiliaryInfoPropertyModelEdit.IsDualValve);
        [DisplayName("双阀模式")]
        [Category("绑定信息")]
        [PropertySortOrder(8)]
        [Browsable(false)]
        public string IsDualValve { get { return _isDualValve; } set { SetProperty(ref _isDualValve, value); } }

        private string _leftValveGlueInfo = nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueInfo);
        [DisplayName("左阀胶水信息")]
        [Category("绑定信息")]
        [PropertySortOrder(9)]
        [Browsable(false)]
        public string LeftValveGlueInfo { get { return _leftValveGlueInfo; } set { SetProperty(ref _leftValveGlueInfo, value); } }

        private string _leftValveGluePackageId = nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGluePackageId);
        [DisplayName("左阀胶水包装ID")]
        [Category("绑定信息")]
        [PropertySortOrder(10)]
        [Browsable(false)]
        public string LeftValveGluePackageId { get { return _leftValveGluePackageId; } set { SetProperty(ref _leftValveGluePackageId, value); } }

        private string _leftValveGlueBatchNo = nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueBatchNo);
        [DisplayName("左阀胶水生产批次号")]
        [Category("绑定信息")]
        [PropertySortOrder(11)]
        [Browsable(false)]
        public string LeftValveGlueBatchNo { get { return _leftValveGlueBatchNo; } set { SetProperty(ref _leftValveGlueBatchNo, value); } }

        private string _leftValveGlueMaterialNo = nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueMaterialNo);
        [DisplayName("左阀胶水制造料号")]
        [Category("绑定信息")]
        [PropertySortOrder(12)]
        [Browsable(false)]
        public string LeftValveGlueMaterialNo { get { return _leftValveGlueMaterialNo; } set { SetProperty(ref _leftValveGlueMaterialNo, value); } }

        private string _leftValveGlueProdDate = nameof(AuxiliaryInfoPropertyModelEdit.LeftValveGlueProdDate);
        [DisplayName("左阀胶水生产日期")]
        [Category("绑定信息")]
        [PropertySortOrder(13)]
        [Browsable(false)]
        public string LeftValveGlueProdDate { get { return _leftValveGlueProdDate; } set { SetProperty(ref _leftValveGlueProdDate, value); } }

        private string _deviceId = nameof(AuxiliaryInfoPropertyModelEdit.DeviceId);
        [DisplayName("设备ID")]
        [Category("绑定信息")]
        [PropertySortOrder(14)]
        [Browsable(false)]
        public string DeviceId { get { return _deviceId; } set { SetProperty(ref _deviceId, value); } }

        private string _deviceName = nameof(AuxiliaryInfoPropertyModelEdit.DeviceName);
        [DisplayName("设备名称")]
        [Category("绑定信息")]
        [PropertySortOrder(15)]
        [Browsable(false)]
        public string DeviceName { get { return _deviceName; } set { SetProperty(ref _deviceName, value); } }

        private string _macAddress = nameof(AuxiliaryInfoPropertyModelEdit.MacAddress);
        [DisplayName("Mac地址")]
        [Category("绑定信息")]
        [PropertySortOrder(16)]
        [Browsable(false)]
        public string MacAddress { get { return _macAddress; } set { SetProperty(ref _macAddress, value); } }

        private string _ipAddress = nameof(AuxiliaryInfoPropertyModelEdit.IpAddress);
        [DisplayName("IP地址")]
        [Category("绑定信息")]
        [PropertySortOrder(17)]
        [Browsable(false)]
        public string IpAddress { get { return _ipAddress; } set { SetProperty(ref _ipAddress, value); } }

        private string _productInfo = nameof(AuxiliaryInfoPropertyModelEdit.ProductInfo);
        [DisplayName("产品信息")]
        [Category("绑定信息")]
        [PropertySortOrder(18)]
        [Browsable(false)]
        public string ProductInfo { get { return _productInfo; } set { SetProperty(ref _productInfo, value); } }

        private string _glueName = nameof(AuxiliaryInfoPropertyModelEdit.GlueName);
        [DisplayName("胶水名称")]
        [Category("绑定信息")]
        [PropertySortOrder(19)]
        [Browsable(false)]
        public string GlueName { get { return _glueName; } set { SetProperty(ref _glueName, value); } }

        private string _piezoValveSerialNo = nameof(AuxiliaryInfoPropertyModelEdit.PiezoValveSerialNo);
        [DisplayName("压电阀序号")]
        [Category("绑定信息")]
        [PropertySortOrder(20)]
        [Browsable(false)]
        public string PiezoValveSerialNo { get { return _piezoValveSerialNo; } set { SetProperty(ref _piezoValveSerialNo, value); } }

        private string _valveId = nameof(AuxiliaryInfoPropertyModelEdit.ValveId);
        [DisplayName("阀ID")]
        [Category("绑定信息")]
        [PropertySortOrder(21)]
        [Browsable(false)]
        public string ValveId { get { return _valveId; } set { SetProperty(ref _valveId, value); } }

        private string _textFont = nameof(AuxiliaryInfoPropertyModelEdit.TextFont);
        [DisplayName("文本字体")]
        [Category("绑定信息")]
        [PropertySortOrder(20)]
        [Browsable(false)]
        public string TextFont { get { return _textFont; } set { SetProperty(ref _textFont, value); } }

        private string _textColor = nameof(AuxiliaryInfoPropertyModelEdit.TextColor);
        [DisplayName("文本颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(21)]
        [Browsable(false)]
        public string TextColor { get { return _textColor; } set { SetProperty(ref _textColor, value); } }

        private string _backColor = nameof(AuxiliaryInfoPropertyModelEdit.BackColor);
        [DisplayName("背景颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(22)]
        [Browsable(false)]
        public string BackColor { get { return _backColor; } set { SetProperty(ref _backColor, value); } }

        public void CopyFrom(AuxiliaryInfoPropertyBindEditModel source)
        {
            base.CopyFrom(source);
            ProgramName = source.ProgramName;
            WorkOrderNo = source.WorkOrderNo;
            RightValveGlueInfo = source.RightValveGlueInfo;
            RightValveGluePackageId = source.RightValveGluePackageId;
            RightValveGlueBatchNo = source.RightValveGlueBatchNo;
            RightValveGlueMaterialNo = source.RightValveGlueMaterialNo;
            RightValveGlueProdDate = source.RightValveGlueProdDate;
            IsDualValve = source.IsDualValve;
            LeftValveGlueInfo = source.LeftValveGlueInfo;
            LeftValveGluePackageId = source.LeftValveGluePackageId;
            LeftValveGlueBatchNo = source.LeftValveGlueBatchNo;
            LeftValveGlueMaterialNo = source.LeftValveGlueMaterialNo;
            LeftValveGlueProdDate = source.LeftValveGlueProdDate;
            DeviceId = source.DeviceId;
            DeviceName = source.DeviceName;
            MacAddress = source.MacAddress;
            IpAddress = source.IpAddress;
            ProductInfo = source.ProductInfo;
            GlueName = source.GlueName;
            PiezoValveSerialNo = source.PiezoValveSerialNo;
            ValveId = source.ValveId;
            TextFont = source.TextFont;
            TextColor = source.TextColor;
            BackColor = source.BackColor;
        }
    }
}
