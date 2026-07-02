using Avalonia.Controls;
using Griffins.UI.General;
using Griffins.UI;
using ReactiveUI;
using System.Reactive;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 测高位置信息 ViewModel
    /// </summary>
    public class MeasurementPositionViewModel : BasePositionViewModel
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        
        /// <summary>
        /// 高度显示名称
        /// </summary>
        private string _hightShowName="";
        public string HightShowName
        {
            get => _hightShowName;
            set
            {
                _hightShowName = value;
                this.RaisePropertyChanged(nameof(HightShowName));
            }
        }

        /// <summary>
        /// 激光到命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> LaserMoveToCommand { get;  }
        public MeasurementPositionViewModel()
        {
            LaserMoveToCommand = ReactiveCommand.CreateFromTask(onLaserMoveTo);
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public new  void SetViewReference(Control view)
        {
            base.SetViewReference(view);
            _viewReference = view;
        }
        /// <summary>
        /// 移动激光
        /// </summary>
        /// <returns></returns>
        private async Task onLaserMoveTo()
        {
            IsOping = true;
            try
            {
               // await InitCfgPageCommandExecutor.Instance.LaserMoveTo(
               //"",
               //CalibrationType.LaserHeight,
               //new Point3D
               //{
               //    X = (double)this.X,
               //    Y = (double)this.Y,
               //    Z = (double)this.Z
               //});
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

        ///// <summary>
        ///// 获取当前设备位置
        ///// </summary>
        //protected override async Task<(decimal X, decimal Y, decimal Z)> _getCurrentDevicePosition()
        //{
        //    var axisData = GlobalVisionViewModel.AxisViewModel;
        //    return (axisData.X, axisData.Y, axisData.Z);
        //}

        /// <summary>
        /// 移动相机到目标位置
        /// </summary>
        protected override async Task _moveCameraToPosition(decimal x, decimal y, decimal z)
        {
            //await InitCfgPageCommandExecutor.Instance.CamreaMoveTo(
            //    "1",
            //    CalibrationType.CameraScale,
            //    new Point3D
            //    {
            //        X = (double)this.X,
            //        Y = (double)this.Y,
            //        Z = (double)this.Z
            //    });
        }

        #endregion


        private void CamreaInfoModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
           
        }
       
    }


}