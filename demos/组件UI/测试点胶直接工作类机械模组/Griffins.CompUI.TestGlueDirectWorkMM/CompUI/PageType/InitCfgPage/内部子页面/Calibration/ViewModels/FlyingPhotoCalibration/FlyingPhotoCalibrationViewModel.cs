using Avalonia.Controls;
using DynamicData;
using Griffins.UI;
using Griffins.UI.General;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModels.FlyingPhotoCalibration
{
    /// <summary>
    /// 飞拍标定信息 ViewModel（对应 FlyingPhotoCalibrationInfo）
    /// </summary>
    public class FlyingPhotoCalibrationViewModel : BasePositionViewModel
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        ///// <summary>
        ///// 标定结果改变事件
        ///// </summary>
        //public event EventHandler<CalibrationResultInfoChangedEventArgs>? OnCalibrationValueChanged;

        /// <summary>
        /// 阀-下拉框模型
        /// </summary>
        public ComboxViewModel ValveInfoModel { get; }
        /// <summary>
        /// 飞拍速度（mm/s）数据模型
        /// </summary>
        public NumericViewModel FlyingSpeedViewModel { get; }

        /// <summary>
        /// 提前加速度距离（mm）数据模型
        /// </summary>
        public NumericViewModel PreAccelerationDistanceViewModel { get; }

        /// <summary>
        /// 飞拍首点整定时间（ms）数据模型
        /// </summary>
        public NumericViewModel FlyingCalibrationTimeViewModel { get; }

        /// <summary>
        /// 已标定速度列表（绑定到UI的集合）
        /// </summary>
        public ObservableCollection<decimal> CalibratedSpeeds { get; }

        /// <summary>
        /// 方向 数据模型（下拉框）
        /// </summary>
        public ComboxViewModel FlyingDirectionViewModel { get; }

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
        /// 飞拍速度（mm/s）
        /// </summary>
        public decimal FlyingSpeed
        {
            get => FlyingSpeedViewModel.Value;
            set => FlyingSpeedViewModel.Value = value;
        }

        /// <summary>
        /// 提前加速度距离（mm）
        /// </summary>
        public decimal PreAccelerationDistance
        {
            get => PreAccelerationDistanceViewModel.Value;
            set => PreAccelerationDistanceViewModel.Value = value;
        }

        /// <summary>
        /// 飞拍首点整定时间（ms）
        /// </summary>
        public decimal FlyingCalibrationTime
        {
            get => FlyingCalibrationTimeViewModel.Value;
            set => FlyingCalibrationTimeViewModel.Value = value;
        }

        ///// <summary>
        ///// 方向
        ///// </summary>
        //public FlyingDirection FlyingDirection
        //{
        //    get
        //    {
        //        if (FlyingDirectionViewModel.SelectedItem is ComBoxItem item)
        //            return (FlyingDirection)item.Value;
        //        return FlyingDirection.X; // 默认X方向
        //    }
        //    set
        //    {
        //        FlyingDirectionViewModel.SelectedItem = FlyingDirectionViewModel.ItemsSource!
        //            .Cast<ComBoxItem>()
        //            .FirstOrDefault(item => (FlyingDirection)item.Value == value);
        //    }
        //}
       
        /// <summary>
        /// 标定命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CalibrationCommand { get; }

        /// <summary>
        /// 删除标定速度命令
        /// </summary>
        public ReactiveCommand<decimal, Unit> DeleteCommand { get; }
        public FlyingPhotoCalibrationViewModel()
        {
            // 初始化数值模型
            FlyingSpeedViewModel = new NumericViewModel
            {
                Minimum = 10, 
                Maximum = 5000, 
                Increment = 10,
                Value = 100
            };
            PreAccelerationDistanceViewModel = new NumericViewModel
            {
                Minimum = 0,
                Increment = 0.5m, 
                DecimalPlaces = 1,
                Value = 10
            };
            FlyingCalibrationTimeViewModel = new NumericViewModel
            {
                Minimum = 0,
                Maximum = 1000, 
                Increment = 10,
                Value = 50
            };

            // 初始化已标定速度列表
            CalibratedSpeeds = new ObservableCollection<decimal>();
            CalibratedSpeeds.Add(0.5m);
            CalibratedSpeeds.Add(1.5m);

            // 初始化方向下拉框（绑定DirectionEnum）
            FlyingDirectionViewModel = new ComboxViewModel
            {
                //ItemsSource = new ObservableCollection<ComBoxItem>
                //{
                //    new ComBoxItem { DisplayName = "X方向", Value =FlyingDirection.X },
                //    new ComBoxItem { DisplayName = "Y方向", Value = FlyingDirection.Y }
                //},
                DisplayMemberPath = nameof(ComBoxItem.DisplayName)
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
            DeleteCommand = ReactiveCommand.Create<decimal>(deleteControlCard);

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
        public void CopyFrom(FlyingPhotoCalibrationInfo info)
        {
            if (info == null) return;
            base.CopyFrom(info); 

            FlyingSpeed = info.FlyingSpeed;
            PreAccelerationDistance = info.AdvanceaccelerationDistance;
            FlyingCalibrationTime = info.FlyingFirsSettingTime;
            //FlyingDirection = info.Direction;

            // 同步已标定速度列表
            //CalibratedSpeeds.Clear();
            foreach (var speed in info.CalibratedSpeeds)
                CalibratedSpeeds.Add(speed);
        }

        /// <summary>
        /// 将数据回写到数据模型
        /// </summary>
        public void CopyTo(FlyingPhotoCalibrationInfo info)
        {
            if (info == null) return;
            base.CopyTo(info); 

            info.FlyingSpeed = FlyingSpeed;
            info.AdvanceaccelerationDistance = PreAccelerationDistance;
            info.FlyingFirsSettingTime = FlyingCalibrationTime;
            //info.Direction = FlyingDirection;

            // 同步已标定速度列表
            info.CalibratedSpeeds.Clear();
            foreach (var speed in CalibratedSpeeds)
                info.CalibratedSpeeds.Add(speed);
        }
        /// <summary>
        /// 将数据回写到缓存
        /// </summary>
        public void UpdateToCache()
        {
            //将当前功能头的标定信息更新到缓存，在保存时从缓存提取标定信息
            handleFuncHeadCache(SelectedValveNumber, isSaveOld: true);
        }

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

            var flyingPhotoCalibrationInfo = new FlyingPhotoCalibrationInfo();
            if (isSaveOld)
            {
                // 保存旧值到缓存
                this.CopyTo(flyingPhotoCalibrationInfo);
                CalibrationCacheDataMng.SetFlyingPhotoCalibrationInfo(funcHeadID, flyingPhotoCalibrationInfo);
            }
            else
            {
                // 从缓存加载新值并更新本地
                CalibrationCacheDataMng.GetFlyingPhotoCalibrationInfo(funcHeadID, out flyingPhotoCalibrationInfo);
                this.CopyFrom(flyingPhotoCalibrationInfo);
            }
        }
        #region 执行命令

        #region 重写基类命令

        /// <summary>
        /// 获取当前设备位置
        /// </summary>
        protected override async Task<(decimal X, decimal Y, decimal Z)> _getCurrentDevicePosition()
        {
            //var axisData = GlobalVisionViewModel.AxisViewModel;
            //return (axisData.X, axisData.Y, axisData.Z);
            return (0,0, 0);
        }

        /// <summary>
        /// 移动相机到目标位置
        /// </summary>
        protected override async Task _moveCameraToPosition(decimal x, decimal y, decimal z)
        {
            //await InitCfgPageCommandExecutor.Instance.CamreaMoveTo(
            //    SelectedValveNumber,
            //    CalibrationType.FlyingCCD,
            //    new Point3D
            //    {
            //        X = (double)this.X,
            //        Y = (double)this.Y,
            //        Z = (double)this.Z
            //    });
        }

        #endregion
        /// <summary>
        ///标定
        /// </summary>
        private async Task onCalibrationCommand()
        {
            IsOping = true;
            //try
            //{
            //    var calibrationParams = new FlyingCalibrationParameters();
            //    calibrationParams.FunctionHeadId = SelectedValveNumber;
            //    calibrationParams.FlyingCoordinates = new Point3D()
            //    {
            //        X = (double)this.X,
            //        Y = (double)this.Y,
            //        Z = (double)this.Z
            //    };
            //    calibrationParams.FlyingSpeed = (double)FlyingSpeed;
            //    calibrationParams.PreAccelerationDistance = (double)PreAccelerationDistance;
            //    calibrationParams.FlyingCalibrationTime = (double)FlyingCalibrationTime;
            //    calibrationParams.FlyingDirection = FlyingDirection;
            //    CalibrationResultBase calibrationResultBase = await InitCfgPageCommandExecutor.Instance.Calibrate(SelectedValveNumber, CalibrationType.FlyingCCD, calibrationParams);
            //    FlyingCalibrationResult flyingCalibrationResult = (FlyingCalibrationResult)calibrationResultBase;
            //    // 更新结果到UI
            //    CalibratedSpeeds.Add((decimal)flyingCalibrationResult.FlyingSpeed);
            //}
            //catch (Exception ex)
            //{
            //    await MessageBox.ShowErrorDialog("失败", $"{ex.Message}", _viewReference);
            //}
            //finally
            //{
            //    IsOping = false;
            //}
        }
        /// <summary>
        /// 删除标定速度
        /// </summary>
        /// <param name="cardModel">待删除的标定速度模型</param>
        private  void deleteControlCard(decimal cardModel)
        {
            CalibratedSpeeds.Remove(cardModel);
        }
        #endregion
        
        //private void onOnCalibrationValueChanged(CalibrationResultInfoChangedEventArgs eventArgs)
        //{
        //    OnCalibrationValueChanged?.Invoke(this, eventArgs);
        //}
    }
}