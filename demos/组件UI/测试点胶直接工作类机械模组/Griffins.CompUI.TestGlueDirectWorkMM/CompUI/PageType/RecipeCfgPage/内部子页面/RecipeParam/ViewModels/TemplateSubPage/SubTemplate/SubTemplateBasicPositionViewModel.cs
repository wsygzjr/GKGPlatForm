using Avalonia.Controls;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.SubTemplate
{
    /// <summary>
    /// 相子模板基准坐标位置校准-视图模型
    /// </summary>
    public class SubTemplateBasicPositionViewModel : BasePositionViewModel
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
       
        public SubTemplateBasicPositionViewModel()
        {
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
        /// 复制
        /// </summary>
        public void CopyFrom(CoordinateAxisValueList coordinateAxisValues)
        {
            foreach (var axisValue in coordinateAxisValues)
            {
                switch (axisValue.Axis)
                {
                    case CoordinateAxisConstant.X:
                        X = axisValue.AxisValue;
                        break;
                    case CoordinateAxisConstant.Y:
                        Y = axisValue.AxisValue;
                        break;
                    case CoordinateAxisConstant.Z:
                        Z = axisValue.AxisValue;
                        break;
                }
            }
        }

        /// <summary>
        /// 复制到指定对象
        /// </summary>
        public void CopyTo(CoordinateAxisValueList coordinateAxisValues)
        {
            coordinateAxisValues.Clear();
            coordinateAxisValues.Add(new CoordinateAxisValue { Axis = CoordinateAxisConstant.X, AxisValue = X });
            coordinateAxisValues.Add(new CoordinateAxisValue { Axis = CoordinateAxisConstant.Y, AxisValue = Y });
            coordinateAxisValues.Add(new CoordinateAxisValue { Axis = CoordinateAxisConstant.Z, AxisValue = Z });
        }

        #region 执行命令


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
            //string selectedValveNumber = "1";
            //await InitCfgPageCommandExecutor.Instance.CamreaMoveTo(
            //    selectedValveNumber,
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
    }

    
}