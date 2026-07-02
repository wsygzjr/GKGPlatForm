using Avalonia.Controls;
using GKG;
using GKG.UI;
using GKG.UI.General;
using Griffins.CompUI.Calibration.Models;
//using Griffins.CompUI.GlueDirectWork.InitCfgPage;
using System;
using System.Threading.Tasks;

namespace Griffins.CompUI.Calibration.ViewModels
{

    /// <summary>
    /// 激光位置 ViewModel（对应 LaserPosition）
    /// </summary>
    public class LaserPositionViewModel : BasePositionViewModel
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        private string _functionHeadID = "";
        /// <summary>
        /// 激光高度值 数据模型（对应 decimal LaserHight）
        /// </summary>
        public NumericViewModel LaserHightViewModel { get; }

        /// <summary>
        /// 激光高度值
        /// </summary>
        public decimal LaserHight
        {
            get => (decimal)LaserHightViewModel.Value;
            set => LaserHightViewModel.Value = value;
        }

        public LaserPositionViewModel()
        {
            base.MoveBtText = "激光到";
            LaserHightViewModel = new NumericViewModel
            {
                Minimum = 0,       // 高度不能为负
                Increment = 0.1m,  // 步长0.1mm
                DecimalPlaces = 1,
            };

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
        /// 设置功能头ID
        /// </summary>
        /// <param name="functionHeadID">功能头ID</param>
        public void SetFunctionHeadID(string functionHeadID)
        {
            _functionHeadID = functionHeadID;
        }
        public void CopyFrom(LaserPosition info)
        {
            if (info == null) return;
            base.CopyFrom(info);
            LaserHight = info.LaserHight;
        }

        public void CopyTo(LaserPosition info)
        {
            if (info == null) return;
            base.CopyTo(info);
            info.LaserHight = LaserHight;
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
        /// 移动激光功能头到目标位置
        /// </summary>
        protected override async Task _moveCameraToPosition(decimal x, decimal y, decimal z)
        {
            //await InitCfgPageCommandExecutor.Instance.LaserMoveTo(_functionHeadID, CalibrationType.Offset, new Point3D()
            //{
            //    X = (Double)x,
            //    Y = (Double)y,
            //    Z = (Double)z,
            //});

        }
        #endregion

    }
}
