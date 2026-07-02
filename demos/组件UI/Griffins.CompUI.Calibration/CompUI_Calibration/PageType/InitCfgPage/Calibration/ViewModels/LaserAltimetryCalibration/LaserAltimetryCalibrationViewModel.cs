using Avalonia.Controls;
using Avalonia.Media;
using GKG;
using GKG.UI;
using GKG.UI.General;
using Griffins.CompUI.Calibration.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Griffins.CompUI.Calibration.ViewModels
{
    /// <summary>
    /// 激光测高标定信息 ViewModel
    /// </summary>
    public class LaserAltimetryCalibrationViewModel : BasePositionViewModel
    {
        //private bool  _hightState =  false;
        private bool _hightState = true;
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        /// <summary>
        /// 激光和功能头标定结果
        /// </summary>
        private LaserAndFunctionHeadCalibrationResult _calibrationResult;
        /// <summary>
        /// 标定结果改变事件
        /// </summary>
        public event EventHandler<CalibrationResultInfoChangedEventArgs>? OnCalibrationValueChanged;

        /// <summary>
        /// 阀-下拉框模型
        /// </summary>
        public ComboxViewModel ValveInfoModel { get; }
        /// <summary>
        /// 蘑菇头限制高度(mm) 数据模型
        /// </summary>
        public NumericViewModel MushroomHeadLimitHightViewModel { get; }

        /// <summary>
        /// 阀到平面距离偏差报警（mm）数据模型
        /// </summary>
        public NumericViewModel ValveToPlaneDistanceDeviationAlarmViewModel { get; }

        /// <summary>
        /// 蘑菇头标定补偿值(mm) 数据模型
        /// </summary>
        public NumericViewModel MushroomHeadCalibrationCompensationValueViewModel { get; }

        /// <summary>
        /// 激光测高值(mm)显示 数据模型
        /// </summary>
        public TextBlockViewModel LaserHightViewModel { get; }

        /// <summary>
        /// 蘑菇头限制高度(mm)
        /// </summary>
        public decimal MushroomHeadLimitHight
        {
            get => MushroomHeadLimitHightViewModel.Value;
            set => MushroomHeadLimitHightViewModel.Value = value;
        }

        /// <summary>
        /// 阀到平面距离偏差报警（mm）
        /// </summary>
        public decimal ValveToPlaneDistanceDeviationAlarm
        {
            get => ValveToPlaneDistanceDeviationAlarmViewModel.Value;
            set => ValveToPlaneDistanceDeviationAlarmViewModel.Value = value;
        }

        /// <summary>
        /// 蘑菇头标定补偿值(mm)
        /// </summary>
        public decimal MushroomHeadCalibrationCompensationValue
        {
            get => MushroomHeadCalibrationCompensationValueViewModel.Value;
            set => MushroomHeadCalibrationCompensationValueViewModel.Value = value;
        }

        /// <summary>
        /// 激光测高值(mm)显示
        /// </summary>  
        public decimal LaserHight
        {
            get => Convert.ToDecimal(LaserHightViewModel.Text);
            set => LaserHightViewModel.Text = value.ToString();
        }

        /// <summary>
        /// 测高状态
        /// true:颜色1
        /// false:颜色2
        /// </summary>
        public bool HightState
        {
            get => _hightState;
            set => this.RaiseAndSetIfChanged(ref _hightState, value);
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
        /// 阀测试命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ValveTestingCommand { get; }

        // 接收资源中的 IconPath_Circle 字符串
        private Geometry _iconPathCircle = StreamGeometry.Parse("");
        /// <summary>
        /// Path的图形
        /// </summary>
        public Geometry IconPathCircle
        {
            get => _iconPathCircle;
            set
            {
                //_iconPathCircle = null;
                _iconPathCircle = value;
                this.RaiseAndSetIfChanged(ref _iconPathCircle, value);
            }
        }


        public LaserAltimetryCalibrationViewModel()
        {
            _calibrationResult = new LaserAndFunctionHeadCalibrationResult();

            this.MoveBtText = "激光到";
            // 初始化数值模型（设置合理范围和步长）
            MushroomHeadLimitHightViewModel = new NumericViewModel
            {
                Minimum = 0,
                Increment = 0.1m,
                DecimalPlaces = 1,
                Value = 0
            };
            ValveToPlaneDistanceDeviationAlarmViewModel = new NumericViewModel
            {
                Minimum = 0,
                Increment = 0.01m,
                DecimalPlaces = 2,
                Value = 0
            };
            MushroomHeadCalibrationCompensationValueViewModel = new NumericViewModel
            {
                Increment = 0.01m, // 支持正负补偿
                DecimalPlaces = 2,
                Value = 0
            };
            LaserHightViewModel = new TextBlockViewModel
            {
            };

            ValveTestingCommand = ReactiveCommand.CreateFromTask(onValveTestingCommand);

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
        public new void SetViewReference(Control view)
        {
            base.SetViewReference(view);
            _viewReference = view;
        }
        /// <summary>
        /// 从数据模型加载数据
        /// </summary>
        public void CopyFrom(LaserAltimetryCalibrationInfo info)
        {
            if (info == null) return;
            base.CopyFrom(info);
            MushroomHeadLimitHight = info.MushroomHeadLimitHight;
            ValveToPlaneDistanceDeviationAlarm = info.ValveToPlaneDistanceDeviationAlarm;
            MushroomHeadCalibrationCompensationValue = info.MushroomHeadCalibrationCompensationValue;
            LaserHight = info.LaserHight;
            HightState = info.HightState;
        }

        /// <summary>
        /// 将数据回写到数据模型
        /// </summary>
        public void CopyTo(LaserAltimetryCalibrationInfo info)
        {
            if (info == null) return;
            base.CopyTo(info);
            info.MushroomHeadLimitHight = MushroomHeadLimitHight;
            info.ValveToPlaneDistanceDeviationAlarm = ValveToPlaneDistanceDeviationAlarm;
            info.MushroomHeadCalibrationCompensationValue = MushroomHeadCalibrationCompensationValue;
            info.LaserHight = LaserHight;
            info.HightState = HightState;
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
        /// 移动激光到目标位置
        /// </summary>
        protected override async Task _moveCameraToPosition(decimal x, decimal y, decimal z)
        {
            //await InitCfgPageCommandExecutor.Instance.LaserMoveTo(
            //    SelectedValveNumber,
            //    CalibrationType.LaserHeight,
            //    new Point3D
            //    {
            //        X = (double)this.X,
            //        Y = (double)this.Y,
            //        Z = (double)this.Z
            //    });
        }

        #endregion

        /// <summary>
        ///阀测试
        /// </summary>
        private async Task onValveTestingCommand()
        {
            IsOping = true;
            try
            {
                var calibrationParams = new LaserAndFunctionHeadCalibrationParameters();
                calibrationParams.FunctionHeadId = SelectedValveNumber;
                calibrationParams.LaserCoordinates = new Point3D()
                {
                    X = (double)this.X,
                    Y = (double)this.Y,
                    Z = (double)this.Z
                };
                calibrationParams.MushroomHeadLimitHeight = (double)MushroomHeadLimitHight;
                //疑问：值怎么来？
                calibrationParams.ZAxisDescentSpeed = 0;
                //CalibrationResultBase calibrationResultBase = await InitCfgPageCommandExecutor.Instance.Calibrate(SelectedValveNumber, CalibrationType.LaserHeight, calibrationParams);
                //_calibrationResult = (LaserAndFunctionHeadCalibrationResult)calibrationResultBase;
                ////更新结果到UI
                //updateResultToUI(_calibrationResult);

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
        #endregion

        /// <summary>
        /// 阀选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            var laserAltimetryCalibrationInfo = new LaserAltimetryCalibrationInfo();
            if (isSaveOld)
            {
                // 保存旧值到缓存
                this.CopyTo(laserAltimetryCalibrationInfo);

                //缓存标定结果
                laserAltimetryCalibrationInfo.CalibrationResult.FunctionHeadId = _calibrationResult.FunctionHeadId;
                laserAltimetryCalibrationInfo.CalibrationResult.ValveZAxisPosition = _calibrationResult.ValveZAxisPosition;
                laserAltimetryCalibrationInfo.CalibrationResult.LaserToPlaneValue = _calibrationResult.LaserToPlaneValue;
                laserAltimetryCalibrationInfo.CalibrationResult.MeasureHeightPositiveDir = _calibrationResult.MeasureHeightPositiveDir;

                CalibrationCacheDataMng.SetLaserAltimetryCalibrationInfo(funcHeadID, laserAltimetryCalibrationInfo);
            }
            else
            {
                // 从缓存加载新值并更新本地
                CalibrationCacheDataMng.GetLaserAltimetryCalibrationInfo(funcHeadID, out laserAltimetryCalibrationInfo);
                this.CopyFrom(laserAltimetryCalibrationInfo);
                //加载标定结果并显示扫UI
                updateResultToUI(laserAltimetryCalibrationInfo.CalibrationResult);
            }
        }

        /// <summary>
        /// 更新结果到UI
        /// </summary>
        /// <param name="calibrationResult"></param>
        private void updateResultToUI(LaserAndFunctionHeadCalibrationResult calibrationResult)
        {
            var laserVsGluevalveResultInfo = new LaserVsGluevalveResultInfo();
            laserVsGluevalveResultInfo.LaserToPlaneValue = (decimal)calibrationResult.LaserToPlaneValue;
            laserVsGluevalveResultInfo.ValveZAxisPosition = (decimal)calibrationResult.ValveZAxisPosition;
            OnCalibrationValueChanged?.Invoke(this, new CalibrationResultInfoChangedEventArgs(CalibrationResultKind.LaserVsGluevalve, laserVsGluevalveResultInfo));
        }
    }
}