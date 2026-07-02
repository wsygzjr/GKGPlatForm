using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModels.OffsetCalibration
{

    /// <summary>
    /// 相机Vs激光 ViewModel（对应 CameraVsLaser）
    /// </summary>
    public class CameraVsLaserViewModel : ReactiveObject
    {
        ///// <summary>
        ///// 0度标定结果
        ///// </summary>
        //private OffsetCalibrationResult _zeroOffsetCalibrationResult;
        ///// <summary>
        ///// 标定结果改变事件
        ///// </summary>
        //public event EventHandler<CalibrationResultInfoChangedEventArgs>? OnCalibrationValueChanged;

        /// <summary>
        /// 激光-下拉框模型
        /// </summary>
        public ComboxViewModel ValveInfoModel { get; }
        /// <summary>
        /// 激光位置 ViewModel
        /// </summary>
        public LaserPositionViewModel LaserPosition { get; }
        /// <summary>
        /// 相机位置 ViewModel
        /// </summary>
        public CameraPositionViewModel CameraPosition { get; }


        /// <summary>
        /// 当前选中的激光
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

        public CameraVsLaserViewModel()
        {
            //_zeroOffsetCalibrationResult = new OffsetCalibrationResult();
            //_zeroOffsetCalibrationResult.OffsetValue = new Point2D();
            LaserPosition = new LaserPositionViewModel();

            CameraPosition = new CameraPositionViewModel();
            CameraPosition.OnCalculatePosition = executeZeroCameraCalibration;
            CameraPosition.OnMoveCamera = executeZeroCameraMove;

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
            LaserPosition.SetViewReference(view);
            CameraPosition.SetViewReference(view);
        }
        public void CopyFrom(CameraVsLaser info)
        {
            if (info == null) return;
            LaserPosition.CopyFrom(info.LaserPosition);
            CameraPosition.CopyFrom(info.CameraPosition);
        }

        public void CopyTo(CameraVsLaser info)
        {
            if (info == null) return;
            LaserPosition.CopyTo(info.LaserPosition);
            CameraPosition.CopyTo(info.CameraPosition);

        }

        /// <summary>
        /// 将数据回写到缓存
        /// </summary>
        public void UpdateToCache()
        {
            //将当前功能头的标定信息更新到缓存
            handleZeroPosition(SelectedValveNumber, isSaveOld: true);
        }
        /// <summary>
        /// 移动相机到目标位置
        /// </summary>
        private async Task executeZeroCameraMove()
        {
            //await InitCfgPageCommandExecutor.Instance.CamreaMoveTo(
            //    CameraPosition.CameraNumber,
            //    CalibrationType.Offset,
            //    new Point3D
            //    {
            //        X = (double)CameraPosition.X,
            //        Y = (double)CameraPosition.Y,
            //        Z = (double)CameraPosition.Z
            //    });
        }

        /// <summary>
        /// 执行相机标定
        /// </summary>
        /// <returns></returns>
        private async Task executeZeroCameraCalibration()
        {
            //OffsetCalibrationParameters calibrationParams = new OffsetCalibrationParameters();
            //calibrationParams.FunctionHeadId = SelectedValveNumber;
            //calibrationParams.CameraCoordinates = new Point3D()
            //{
            //    X = (double)CameraPosition.X,
            //    Y = (double)CameraPosition.Y,
            //    Z = (double)CameraPosition.Z
            //};
            //calibrationParams.FunctionHeadCoordinates = new Point3D()
            //{
            //    X = (double)LaserPosition.X,
            //    Y = (double)LaserPosition.Y,
            //    Z = (double)LaserPosition.Z
            //};
            //CalibrationResultBase calibrationResultBase = await InitCfgPageCommandExecutor.Instance.Calibrate(SelectedValveNumber, CalibrationType.Offset, calibrationParams);
            //_zeroOffsetCalibrationResult = (OffsetCalibrationResult)calibrationResultBase;

            //updateResultToUI(_zeroOffsetCalibrationResult.OffsetValue);

        }

        private void ValveInfoModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            LaserPosition.SetFunctionHeadID(SelectedValveNumber);

            // 处理旧值
            if (e.OldValue is ComBoxItem oldComboxItem)
            {
                string oldFuncHeadID = oldComboxItem.Value?.ToString() ?? string.Empty;
                handleZeroPosition(oldFuncHeadID, isSaveOld: true);
            }

            // 处理新值
            if (e.NewValue is ComBoxItem newComboxItem)
            {
                string newFuncHeadID = newComboxItem.Value?.ToString() ?? string.Empty;
                handleZeroPosition(newFuncHeadID);
            }
        }
        private void handleZeroPosition(string funcHeadID, bool isSaveOld = false)
        {
            var cameraVsLaser = new CameraVsLaser();
            if (isSaveOld)
            {
                // 保存旧值到缓存
                var laserPosition = new LaserPosition();
                var cameraPos = new ZeroCameraPosition();
                LaserPosition.CopyTo(laserPosition);
                CameraPosition.CopyTo(cameraPos);
                cameraVsLaser.LaserPosition = laserPosition;
                cameraVsLaser.CameraPosition = cameraPos;

                ////缓存指定功能头指定相机的标定结果
                //cameraVsLaser.CalibrationResult[CameraPosition.CameraNumber] = new Point2D()
                //{
                //    X = _zeroOffsetCalibrationResult.OffsetValue.X,
                //    Y = _zeroOffsetCalibrationResult.OffsetValue.Y,
                //};

                CalibrationCacheDataMng.SetCameraVsLaser(funcHeadID, cameraVsLaser);
            }
            else
            {
                // 从缓存加载新值并更新本地
                CalibrationCacheDataMng.GetCameraVsLaser(funcHeadID, out cameraVsLaser);
                LaserPosition.CopyFrom(cameraVsLaser.LaserPosition);
                CameraPosition.CopyFrom(cameraVsLaser.CameraPosition);

                ////加载标定结果并显示扫UI
                //if (cameraVsLaser.CalibrationResult.ContainsKey(CameraPosition.CameraNumber))
                //    updateResultToUI(cameraVsLaser.CalibrationResult[CameraPosition.CameraNumber]);
            }
        }
        ///// <summary>
        ///// 更新结果到UI
        ///// </summary>
        ///// <param name="point2D"></param>
        //private void updateResultToUI(Point2D point2D)
        //{
        //    // 更新结果到UI
        //    var cameraVsLaserResultInfo = new OffsetResultInfo();
        //    cameraVsLaserResultInfo.X = (decimal)point2D.X;
        //    cameraVsLaserResultInfo.Y = (decimal)point2D.Y;
        //    OnCalibrationValueChanged?.Invoke(this, new CalibrationResultInfoChangedEventArgs(CalibrationResultKind.CameraVsLaser, cameraVsLaserResultInfo));
        //}
    }

}