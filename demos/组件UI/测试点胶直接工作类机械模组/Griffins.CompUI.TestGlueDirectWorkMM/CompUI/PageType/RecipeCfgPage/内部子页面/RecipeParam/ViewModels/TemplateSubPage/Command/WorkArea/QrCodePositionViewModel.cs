using Avalonia.Controls;
using Griffins.UI;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 二维码位置信息 ViewModel
    /// </summary>
    public class QrCodePositionViewModel : BasePositionViewModel
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
      
        /// <summary>
        /// 阀-下拉框模型
        /// </summary>
        public ComboxViewModel CamreaInfoModel { get; }
        
        /// <summary>
        /// 当前选中的相机
        /// </summary>
        public string SelectedCamreaNumber
        {
            get => (string)((CamreaInfoModel.SelectedItem as ComBoxItem)?.Value ?? "1");
            set
            {
                if (CamreaInfoModel.ItemsSource != null)
                {
                    var targetItem = CamreaInfoModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (string)o.Value == value);
                    if (targetItem != null)
                        CamreaInfoModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedCamreaNumber));
                }
            }
        }
        public QrCodePositionViewModel()
        {
          
            List<ComBoxItem> dataItems = new List<ComBoxItem>();
            foreach (var item in CacheDataExchange.GetCameraes())
            {
                dataItems.Add(new ComBoxItem()
                {
                    Value = item.ValveNumber,
                    DisplayName = item.ValveName
                });
            }
            CamreaInfoModel = new ComboxViewModel();
            CamreaInfoModel.ValueChanged += CamreaInfoModel_ValueChanged;
            CamreaInfoModel.ItemsSource = dataItems;
            CamreaInfoModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            CamreaInfoModel.SelectedItem = CamreaInfoModel.ItemsSource.Cast<ComBoxItem>().First();

        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public new  void SetViewReference(Control view)
        {
            base.SetViewReference(view);
            _viewReference = view;
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
            //    SelectedCamreaNumber,
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