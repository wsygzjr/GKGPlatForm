using Avalonia.Controls;
using Griffins.UI.General;
using Griffins.UI;
using ReactiveUI;
using System.Reactive;
using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.ViewModels.OffsetCalibration
{

    /// <summary>
    /// 胶阀位置 ViewModel
    /// </summary>
    public class DegreevalvePositionViewModel : BasePositionViewModel
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        private string _functionHeadID = "";
        /// <summary>
        /// 出胶点数 数据模型（对应 decimal NumberOfDispensingPoints）
        /// </summary>
        public NumericViewModel NumberOfDispensingPointsViewModel { get; }

        /// <summary>
        /// 出胶点数（包装器，绑定UI）
        /// </summary>
        public int NumberOfDispensingPoints
        {
            get => (int)NumberOfDispensingPointsViewModel.Value;
            set => NumberOfDispensingPointsViewModel.Value = value;
        }

        /// <summary>
        /// 是否为90度位置
        /// </summary>
        private bool _isNinetyDegreevalvePosition;
        public bool IsNinetyDegreevalvePosition
        {
            get => _isNinetyDegreevalvePosition;
            set => this.RaiseAndSetIfChanged(ref _isNinetyDegreevalvePosition, value);
        }
     
        /// <summary>
        /// 出胶命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> DischargeRubberCommand { get; }
        /// <summary>
        /// 一键出胶命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> OneClickDischargeRubberCommand { get; }
        /// <summary>
        /// 旋转90度命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> RotateNinetyDegreesCommand { get; }

        public DegreevalvePositionViewModel()
        {
            base.MoveBtText = "针头到";

            NumberOfDispensingPointsViewModel = new NumericViewModel
            {
                Minimum = 0,
                Increment = 1
            };

            DischargeRubberCommand = ReactiveCommand.CreateFromTask(onDischargeRubberCommand);

            var canOneClick = this.WhenAnyValue<DegreevalvePositionViewModel, bool, bool>(
                x => x.IsNinetyDegreevalvePosition,
                isNinety => !isNinety
                );
            OneClickDischargeRubberCommand = ReactiveCommand.CreateFromTask(onOneClickDischargeRubberCommand, canOneClick);

            var canRotate = this.WhenAnyValue<DegreevalvePositionViewModel, bool, bool>(
                x => x.IsNinetyDegreevalvePosition,
                isNinety => isNinety 
                );
            RotateNinetyDegreesCommand = ReactiveCommand.CreateFromTask(onRotateNinetyDegreesCommand, canRotate);

        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public new void SetViewReference(Control view)
        {
            base.SetViewReference(view);
            _viewReference = view;
        }
        public void CopyFrom(DegreevalvePosition info)
        {
            if (info == null) return;
            base.CopyFrom(info);
            NumberOfDispensingPoints =info.NumberOfDispensingPoints;
        }

        public void CopyTo(DegreevalvePosition info)
        {
            if (info == null) return;
            base.CopyTo(info);
            info.NumberOfDispensingPoints = NumberOfDispensingPoints;
        }

        /// <summary>
        /// 设置功能头ID
        /// </summary>
        /// <param name="functionHeadID">功能头ID</param>
        public void SetFunctionHeadID(string functionHeadID)
        {
            _functionHeadID = functionHeadID;
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
        /// 移动功能头到目标位置
        /// </summary>
        protected override async Task _moveCameraToPosition(decimal x, decimal y, decimal z)
        {
            //await InitCfgPageCommandExecutor.Instance.NeedleMoveTo(_functionHeadID, CalibrationType.Offset,new Point3D()
            //{
            //    X=(Double)x,
            //    Y=(Double)y,
            //    Z=(Double)z,
            //});
           
        }
        #endregion



        /// <summary>
        ///出胶
        /// </summary>
        private async Task onDischargeRubberCommand()
        {
            //try
            //{
            //    await InitCfgPageCommandExecutor.Instance.OutGlue(_functionHeadID, CalibrationType.Offset, NumberOfDispensingPoints);
            //}
            //catch (Exception ex)
            //{
            //    await MessageBox.ShowErrorDialog("失败", $"{ex.Message}", _viewReference);
            //}
        }

        /// <summary>
        /// 一键出胶
        /// </summary>
        /// <returns></returns>
        private async Task onOneClickDischargeRubberCommand()
        {
            //try
            //{
            //    await InitCfgPageCommandExecutor.Instance.OutGlue(_functionHeadID, CalibrationType.Offset, NumberOfDispensingPoints);
            //}
            //catch (Exception ex)
            //{
            //    await MessageBox.ShowErrorDialog("失败", $"{ex.Message}", _viewReference);
            //}
        }

        /// <summary>
        /// 旋转90度
        /// </summary>
        /// <returns></returns>
        private async Task onRotateNinetyDegreesCommand()
        {
            try
            {
                await InitCfgPageCommandExecutor.Instance.RotateNinetyDegreesCommand();
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("失败", $"{ex.Message}", _viewReference);
            }
        }
    }
}
