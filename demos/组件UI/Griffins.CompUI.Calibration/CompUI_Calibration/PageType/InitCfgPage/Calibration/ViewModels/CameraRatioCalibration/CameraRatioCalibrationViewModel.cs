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
using System.Reactive;
using System.Threading.Tasks;

namespace Griffins.CompUI.Calibration.ViewModels
{
    /// <summary>
    /// 相机比例标定信息 ViewModel
    /// </summary>
    public class CameraRatioCalibrationViewModel : BasePositionViewModel
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        /// <summary>
        ///标定结果改变事件
        /// </summary>
        public event EventHandler<CalibrationResultInfoChangedEventArgs>? OnCalibrationValueChanged;
        /// <summary>
        /// 激光和功能头标定结果
        /// </summary>
        private CameraScaleCalibrationResult _calibrationResult;
        /// <summary>
        /// 运动步距(mm) 数据模型
        /// </summary>
        public NumericViewModel SportsStrideViewModel { get; }

        /// <summary>
        /// 阀-下拉框模型
        /// </summary>
        public ComboxViewModel ValveInfoModel { get; }
        /// <summary>
        /// 运动步距(mm)
        /// </summary>
        public decimal SportsStride
        {
            get => SportsStrideViewModel.Value;
            set => SportsStrideViewModel.Value = value;
        }
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
        /// <summary>
        /// 标定命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CalibrationCommand { get; }
        public CameraRatioCalibrationViewModel()
        {
            _calibrationResult = new CameraScaleCalibrationResult();

            SportsStrideViewModel = new NumericViewModel
            {
                Minimum = 0.1m, // 步距不能为0或负数
                Increment = 0.1m,
                DecimalPlaces = 1,
                Value = 1.0m // 默认步距1mm
            };

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
            CalibrationCommand = ReactiveCommand.CreateFromTask(onCalibrationCommand);

        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public new void SetViewReference(Control view)
        {
            base.SetViewReference(view);
            _viewReference = view;
        }
        /// <summary>
        /// 从数据模型加载数据
        /// </summary>
        public void CopyFrom(CameraRatioCalibrationInfo info)
        {
            if (info == null) return;
            base.CopyFrom(info);
            SportsStride = info.SportsStride;
        }

        /// <summary>
        /// 将数据回写到数据模型
        /// </summary>
        public void CopyTo(CameraRatioCalibrationInfo info)
        {
            if (info == null) return;
            base.CopyTo(info);
            info.SportsStride = SportsStride;
        }
        /// <summary>
        /// 将数据回写到缓存
        /// </summary>
        public void UpdateToCache()
        {
            //将当前功能头的标定信息更新到缓存
            handleFuncHeadCache(SelectedValveNumber, isSaveOld: true);
        }

        #region 执行命令

        /// <summary>
        ///标定
        /// </summary>
        private async Task onCalibrationCommand()
        {
            IsOping = true;
            try
            {
                var calibrationParams = new CameraScaleCalibrationParameters();
                calibrationParams.FunctionHeadId = SelectedValveNumber;
                calibrationParams.CameraCoordinates = new Point3D()
                {
                    X = (double)this.X,
                    Y = (double)this.Y,
                    Z = (double)this.Z
                };
                calibrationParams.MotionStep = (double)SportsStride;
                calibrationParams.VisionTemplateData = null;
                //CalibrationResultBase calibrationResultBase = await InitCfgPageCommandExecutor.Instance.Calibrate(SelectedValveNumber, CalibrationType.CameraScale, calibrationParams);
                //_calibrationResult = (CameraScaleCalibrationResult)calibrationResultBase;

                //更新结果到UI
                updateResultToUI(_calibrationResult);


            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("失败", $"{ex.Message}", _viewReference);
            }
            finally
            {
                IsOping = false;
            }
        }

        #region 重写基类命令

        /// <summary>
        /// 获取当前设备位置
        /// </summary>
        protected override async Task<(decimal X, decimal Y, decimal Z)> _getCurrentDevicePosition()
        {
            var axisData = GlobalVisionViewModel.AxisViewModel;
            return (axisData.X, axisData.Y, axisData.Z);
        }

        /// <summary>
        /// 移动相机到目标位置
        /// </summary>
        protected override async Task _moveCameraToPosition(decimal x, decimal y, decimal z)
        {
            //await InitCfgPageCommandExecutor.Instance.CamreaMoveTo(
            //    SelectedValveNumber,
            //    CalibrationType.CameraScale,
            //    new Point3D
            //    {
            //        X = (double)this.X,
            //        Y = (double)this.Y,
            //        Z = (double)this.Z
            //    });
        }

        #endregion


        #endregion

        private void ValveInfoModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            // 处理旧值
            if (e.OldValue is ComBoxItem oldComboxItem)
            {
                string oldFuncHeadID = oldComboxItem.Value?.ToString() ?? string.Empty;
                handleFuncHeadCache(oldFuncHeadID, isSaveOld: true);
            }

            // 处理新值
            if (e.NewValue is ComBoxItem newComboxItem)
            {
                string newFuncHeadID = newComboxItem.Value?.ToString() ?? string.Empty;
                handleFuncHeadCache(newFuncHeadID);
            }
        }
        /// <summary>
        /// 处理功能头缓存
        /// </summary>
        /// <param name="funcHeadID"></param>
        /// <param name="isSaveOld"></param>
        private void handleFuncHeadCache(string funcHeadID, bool isSaveOld = false)
        {
            var cameraRatioCalibrationInfo = new CameraRatioCalibrationInfo();
            if (isSaveOld)
            {
                // 保存旧值到缓存
                this.CopyTo(cameraRatioCalibrationInfo);

                //缓存标定结果
                cameraRatioCalibrationInfo.CalibrationResult.FunctionHeadId = _calibrationResult.FunctionHeadId;
                cameraRatioCalibrationInfo.CalibrationResult.XPixelScale = _calibrationResult.XPixelScale;
                cameraRatioCalibrationInfo.CalibrationResult.YPixelScale = _calibrationResult.YPixelScale;
                cameraRatioCalibrationInfo.CalibrationResult.hvHomMat2D = _calibrationResult.hvHomMat2D;
                cameraRatioCalibrationInfo.CalibrationResult.worldHomMat2D = _calibrationResult.worldHomMat2D;
                CalibrationCacheDataMng.SetCameraRatioCalibrationInfo(funcHeadID, cameraRatioCalibrationInfo);
            }
            else
            {
                // 从缓存加载新值并更新本地
                CalibrationCacheDataMng.GetCameraRatioCalibrationInfo(funcHeadID, out cameraRatioCalibrationInfo);
                this.CopyFrom(cameraRatioCalibrationInfo);
                //加载标定结果并显示扫UI
                updateResultToUI(cameraRatioCalibrationInfo.CalibrationResult);
            }
        }
        /// <summary>
        /// 更新结果到UI
        /// </summary>
        /// <param name="calibrationResult"></param>
        private void updateResultToUI(CameraScaleCalibrationResult calibrationResult)
        {
            //更新结果到UI
            var cameraRatioResultInfo = new CameraRatioResultInfo();
            cameraRatioResultInfo.XOffset = (decimal)calibrationResult.XPixelScale;
            cameraRatioResultInfo.YOffset = (decimal)calibrationResult.YPixelScale;
            OnCalibrationValueChanged?.Invoke(this, new CalibrationResultInfoChangedEventArgs(CalibrationResultKind.CameraRatio, cameraRatioResultInfo));
        }
    }


}