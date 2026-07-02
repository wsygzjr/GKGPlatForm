using Avalonia.Controls;
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
    /// 相机位置 ViewModel
    /// </summary>
    public class CameraPositionViewModel : BasePositionViewModel
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;

        /// <summary>
        /// 功能头ID
        /// </summary>
        private string _functionHeadID = string.Empty;
        /// <summary>
        /// 相机源-下拉框模型
        /// </summary>
        public ComboxViewModel CameraInfoModel { get; }

        /// <summary>
        /// 计算位置命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CalculatePositionCommand { get; }
        /// <summary>
        ///指定的计算位置委托
        /// </summary>
        public Func<Task>? OnCalculatePosition { get; set; }
        /// <summary>
        ///移动相机委托
        /// </summary>
        public Func<Task>? OnMoveCamera { get; set; }
        /// <summary>
        /// 当前选中的相机
        /// </summary>
        public string CameraNumber
        {
            get => (string)((CameraInfoModel.SelectedItem as ComBoxItem)?.Value ?? "1");
            set
            {
                if (CameraInfoModel.ItemsSource != null)
                {
                    var targetItem = CameraInfoModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (string)o.Value == value);
                    if (targetItem != null)
                        CameraInfoModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(CameraNumber));
                }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public CameraPositionViewModel()
        {
            CalculatePositionCommand = ReactiveCommand.CreateFromTask(executeCalculatePosition);

            List<ComBoxItem> dataItems = new List<ComBoxItem>();
            foreach (var item in CacheDataExchange.GetCameraes())
            {
                dataItems.Add(new ComBoxItem()
                {
                    Value = item.ValveNumber,
                    DisplayName = item.ValveName
                });
            }
            CameraInfoModel = new ComboxViewModel();
            CameraInfoModel.ItemsSource = dataItems;
            CameraInfoModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            CameraInfoModel.SelectedItem = CameraInfoModel.ItemsSource.Cast<ComBoxItem>().First();
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
        /// 从模型复制数据
        /// </summary>
        public void CopyFrom(ZeroCameraPosition info)
        {
            if (info == null) return;
            base.CopyFrom(info);
        }

        /// <summary>
        /// 复制数据到模型
        /// </summary>
        public void CopyTo(ZeroCameraPosition info)
        {
            if (info == null) return;
            base.CopyTo(info);
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
            if (OnMoveCamera == null)
                throw new Exception("未指定移动相机的执行逻辑");
            await OnMoveCamera.Invoke();
        }

        #endregion

        #region 子类命令

        /// <summary>
        /// 标定
        /// </summary>
        private async Task executeCalculatePosition()
        {
            IsOping = true;
            try
            {
                if (OnCalculatePosition == null)
                    throw new Exception("未指定标定的执行逻辑");
                await OnCalculatePosition.Invoke();
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
    }

}