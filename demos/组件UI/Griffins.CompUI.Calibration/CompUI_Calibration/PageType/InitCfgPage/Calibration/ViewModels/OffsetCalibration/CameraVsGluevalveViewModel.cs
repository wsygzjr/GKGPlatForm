using Avalonia.Controls;
using GKG;
using GKG.UI;
using GKG.UI.General;
using Griffins.CompUI.Calibration.Models;
//using Griffins.CompUI.GlueDirectWork.InitCfgPage;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Griffins.CompUI.Calibration.ViewModels
{
    /// <summary>
    /// 相机Vs胶阀 ViewModel
    /// </summary>
    public class CameraVsGluevalveViewModel : ReactiveObject
    {
        /// <summary>
        /// 0度标定结果
        /// </summary>
        private OffsetCalibrationResult _zeroOffsetCalibrationResult;
        /// <summary>
        /// 90度标定结果
        /// </summary>
        private OffsetCalibrationResult _ninetyZeroOffsetCalibrationResult;
        /// <summary>
        ///标定结果改变事件
        /// </summary>
        public event EventHandler<CalibrationResultInfoChangedEventArgs>? OnCalibrationValueChanged;
        /// <summary>
        /// 阀-下拉框模型
        /// </summary>
        public ComboxViewModel ValveInfoModel { get; }
        /// <summary>
        /// 0度阀位置 ViewModel
        /// </summary>
        public DegreevalvePositionViewModel ZeroDegreevalvePosition { get; }

        /// <summary>
        /// 0相机位置 ViewModel
        /// </summary>
        public CameraPositionViewModel ZeroCameraPosition { get; }

        /// <summary>
        /// 90度阀位置 ViewModel
        /// </summary>
        public DegreevalvePositionViewModel NinetyDegreevalvePosition { get; }

        /// <summary>
        /// 90相机位置 ViewModel
        /// </summary>
        public CameraPositionViewModel NinetyCameraPosition { get; }
        /// <summary>
        /// 相机Vs胶阀公共参数配置-视图模型
        /// </summary>
        public CameraVsGluevalveComViewModel CameraVsGluevalveComViewModel { get; }

        /// <summary>
        /// 当前选中的阀
        /// </summary>
        public string SelectedValveNumber
        {
            get => (string)((ValveInfoModel.SelectedItem as ComBoxItem)?.Value ?? "1");
            set
            {
                if (ValveInfoModel.ItemsSource != null)
                {
                    var targetItem = ValveInfoModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (string)o.Value == value);
                    if (targetItem != null)
                        ValveInfoModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedValveNumber));
                }
            }
        }

        public CameraVsGluevalveViewModel()
        {
            _zeroOffsetCalibrationResult = new OffsetCalibrationResult();
            _zeroOffsetCalibrationResult.OffsetValue = new Point2D();
            _ninetyZeroOffsetCalibrationResult = new OffsetCalibrationResult();

            ZeroDegreevalvePosition = new DegreevalvePositionViewModel();
            ZeroDegreevalvePosition.IsNinetyDegreevalvePosition = false;
            ZeroCameraPosition = new CameraPositionViewModel();
            ZeroCameraPosition.OnCalculatePosition = executeZeroCameraCalibration;
            ZeroCameraPosition.OnMoveCamera = executeZeroCameraMove;

            NinetyCameraPosition = new CameraPositionViewModel();
            NinetyCameraPosition.OnCalculatePosition = executeNinetyCameraCalibration;
            NinetyCameraPosition.OnMoveCamera = executeNinetyCameraMove;

            NinetyDegreevalvePosition = new DegreevalvePositionViewModel();
            NinetyDegreevalvePosition.IsNinetyDegreevalvePosition = true;


            CameraVsGluevalveComViewModel = new CameraVsGluevalveComViewModel();


            List<ComBoxItem> dataItems = new List<ComBoxItem>();
            foreach (var item in CacheDataExchange.GetValveInfoes())
            {
                dataItems.Add(new ComBoxItem()
                {
                    Value = item.ValveNumber,
                    DisplayName = item.ValveName
                });
            }
            ValveInfoModel = new ComboxViewModel();
            ValveInfoModel.ValueChanged += ValveInfoModel_ValueChanged;
            ValveInfoModel.ItemsSource = dataItems;
            ValveInfoModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ValveInfoModel.SelectedItem = ValveInfoModel.ItemsSource.Cast<ComBoxItem>().First();

        }



        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            ZeroDegreevalvePosition.SetViewReference(view);
            ZeroCameraPosition.SetViewReference(view);
            NinetyDegreevalvePosition.SetViewReference(view);
            NinetyCameraPosition.SetViewReference(view);
            CameraVsGluevalveComViewModel.SetViewReference(view);
        }
        public void CopyFrom(CameraVsGluevalve info)
        {
            if (info == null) return;
            ZeroDegreevalvePosition.CopyFrom(info.ZeroDegreevalvePosition);
            ZeroCameraPosition.CopyFrom(info.ZeroCameraPosition);
            NinetyDegreevalvePosition.CopyFrom(info.NinetyDegreevalvePosition);
            NinetyCameraPosition.CopyFrom(info.NinetyCameraPosition);
            CameraVsGluevalveComViewModel.CopyFrom(info.CameraVsGluevalveComInfo);
        }

        public void CopyTo(CameraVsGluevalve info)
        {
            if (info == null) return;
            ZeroDegreevalvePosition.CopyTo(info.ZeroDegreevalvePosition);
            ZeroCameraPosition.CopyTo(info.ZeroCameraPosition);
            NinetyDegreevalvePosition.CopyTo(info.NinetyDegreevalvePosition);
            NinetyCameraPosition.CopyTo(info.NinetyCameraPosition);
            CameraVsGluevalveComViewModel.CopyTo(info.CameraVsGluevalveComInfo);
        }

        /// <summary>
        /// 将数据回写到缓存
        /// </summary>
        public void UpdateToCache()
        {
            //将当前功能头的标定信息更新到缓存
            handleZeroPositionCache(SelectedValveNumber, isSaveOld: true);
        }
        /// <summary>
        /// 移动相机到目标位置
        /// </summary>
        private async Task executeZeroCameraMove()
        {

            //await InitCfgPageCommandExecutor.Instance.CamreaMoveTo(
            //    ZeroCameraPosition.CameraNumber,
            //    CalibrationType.Offset,
            //    new Point3D
            //    {
            //        X = (double)ZeroCameraPosition.X,
            //        Y = (double)ZeroCameraPosition.Y,
            //        Z = (double)ZeroCameraPosition.Z
                //});
        }

        /// <summary>
        /// 执行0度相机标定
        /// </summary>
        /// <returns></returns>
        private async Task executeZeroCameraCalibration()
        {
            OffsetCalibrationParameters calibrationParams = new OffsetCalibrationParameters();
            calibrationParams.FunctionHeadId = SelectedValveNumber;
            calibrationParams.CameraCoordinates = new Point3D()
            {
                X = (double)ZeroCameraPosition.X,
                Y = (double)ZeroCameraPosition.Y,
                Z = (double)ZeroCameraPosition.Z
            };
            calibrationParams.FunctionHeadCoordinates = new Point3D()
            {
                X = (double)ZeroDegreevalvePosition.X,
                Y = (double)ZeroDegreevalvePosition.Y,
                Z = (double)ZeroDegreevalvePosition.Z
            };
            //CalibrationResultBase calibrationResultBase = await InitCfgPageCommandExecutor.Instance.Calibrate(SelectedValveNumber, CalibrationType.Offset, calibrationParams);
            //_zeroOffsetCalibrationResult = (OffsetCalibrationResult)calibrationResultBase;

            ////更新结果到UI
            //updateResultToUI(_zeroOffsetCalibrationResult.OffsetValue);


        }
        /// <summary>
        /// 90移动相机到目标位置
        /// </summary>
        private async Task executeNinetyCameraMove()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 执行90度相机标定
        /// </summary>
        /// <returns></returns>
        private async Task executeNinetyCameraCalibration()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 选中的阀改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValveInfoModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            ZeroDegreevalvePosition.SetFunctionHeadID(SelectedValveNumber);
            NinetyDegreevalvePosition.SetFunctionHeadID(SelectedValveNumber);

            // 处理旧值
            if (e.OldValue is ComBoxItem oldComboxItem)
            {
                string oldFuncHeadID = oldComboxItem.Value?.ToString() ?? string.Empty;
                handleZeroPositionCache(oldFuncHeadID, isSaveOld: true);
            }

            // 处理新值
            if (e.NewValue is ComBoxItem newComboxItem)
            {
                string newFuncHeadID = newComboxItem.Value?.ToString() ?? string.Empty;
                handleZeroPositionCache(newFuncHeadID);
            }

        }
        /// <summary>
        /// 处理0度位置缓存
        /// </summary>
        /// <param name="funcHeadID"></param>
        /// <param name="isSaveOld"></param>
        private void handleZeroPositionCache(string funcHeadID, bool isSaveOld = false)
        {
            var cameraVsGluevalve = new CameraVsGluevalve();
            if (isSaveOld)
            {
                // 保存界面数据到缓存
                var degreeValvePos = new DegreevalvePosition();
                var cameraPos = new ZeroCameraPosition();
                ZeroDegreevalvePosition.CopyTo(degreeValvePos);
                ZeroCameraPosition.CopyTo(cameraPos);
                cameraVsGluevalve.ZeroDegreevalvePosition = degreeValvePos;
                cameraVsGluevalve.ZeroCameraPosition = cameraPos;

                //缓存指定功能头指定相机的标定结果
                cameraVsGluevalve.ZeroCalibrationResult[ZeroCameraPosition.CameraNumber] = new Point2D()
                {
                    X = _zeroOffsetCalibrationResult.OffsetValue.X,
                    Y = _zeroOffsetCalibrationResult.OffsetValue.Y,
                };
                CalibrationCacheDataMng.SetCameraVsGluevalve(funcHeadID, cameraVsGluevalve);
            }
            else
            {
                // 从缓存加载新值到界面模型
                CalibrationCacheDataMng.GetCameraVsGluevalve(funcHeadID, out cameraVsGluevalve);
                ZeroDegreevalvePosition.CopyFrom(cameraVsGluevalve.ZeroDegreevalvePosition);
                ZeroCameraPosition.CopyFrom(cameraVsGluevalve.ZeroCameraPosition);

                //加载标定结果到界面模型
                if (cameraVsGluevalve.ZeroCalibrationResult.ContainsKey(ZeroCameraPosition.CameraNumber))
                    updateResultToUI(cameraVsGluevalve.ZeroCalibrationResult[ZeroCameraPosition.CameraNumber]);
            }
        }

        /// <summary>
        /// 更新标定结果到界面模型
        /// </summary>
        /// <param name="point2D"></param>
        private void updateResultToUI(Point2D point2D)
        {
            var cameraVsGluevalveOffsetResultInfo = new OffsetResultInfo();
            cameraVsGluevalveOffsetResultInfo.X = (decimal)point2D.X;
            cameraVsGluevalveOffsetResultInfo.Y = (decimal)point2D.Y;
            OnCalibrationValueChanged?.Invoke(this, new CalibrationResultInfoChangedEventArgs(CalibrationResultKind.CameraVsGluevalveR0Offset, cameraVsGluevalveOffsetResultInfo));
        }
    }

}