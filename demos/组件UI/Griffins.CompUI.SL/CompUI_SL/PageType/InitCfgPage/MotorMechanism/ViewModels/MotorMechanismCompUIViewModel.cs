using GKG.UI.General;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels
{
    public class MotorMechanismCompUIViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段
        private bool isDesign;

        private ICompUIRunTimeCallBack callBack;

        private MotorMechanismCompUIModel motorMechanismCompUIModel;
        #endregion

        #region 组件UI模型
        /// <summary>
        /// 电机型直线移动机构-组件UI模型
        /// </summary>
        public MotorLinearMotionMechanismViewModel MotorLinearMotionMechanismViewModel { get; }
        #endregion

        #region 响应式属性
        private object _viewTag;
        /// <summary>
        /// 对应View的Tag属性（支持双向绑定）
        /// </summary>
        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        private bool _readOnly;
        /// <summary>
        /// 只读
        /// </summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set => this.RaiseAndSetIfChanged(ref _readOnly, value);
        }

        /// <summary>
        /// 电机型直线移动机构-响应式属性
        /// </summary>
        public MotorLinearMotionMechanismViewModel MotorLinearMotionMechanism
        {
            get => MotorLinearMotionMechanismViewModel;
            set { this.RaisePropertyChanged(nameof(MotorLinearMotionMechanism)); }
        }

        #endregion

        #region 命令
        public ReactiveCommand<Unit, Unit> ButtonClickCommand { get; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数（初始化）
        /// </summary>
        public MotorMechanismCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;

            MotorLinearMotionMechanismViewModel = new();
            MotorLinearMotionMechanismViewModel.AfterModified += MotorLinearMotionMechanismViewModel_AfterModified;

            ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);

            this.WhenAnyValue(x => x.ReadOnly)
                .Subscribe(_ => ApplyReadOnly());

            ApplyReadOnly();
        }
        #endregion

        #region 公共方法
        public void SetData(MotorMechanismCompUIModel model)
        {
            motorMechanismCompUIModel = model ?? new MotorMechanismCompUIModel();
            MotorLinearMotionMechanismViewModel.PositionNumber = motorMechanismCompUIModel.PositionNumber;
            MotorLinearMotionMechanismViewModel.CoordinateValue = motorMechanismCompUIModel.CoordinateValue;
            MotorLinearMotionMechanismViewModel.MoveType = motorMechanismCompUIModel.MoveType;
        }

        public MotorMechanismCompUIModel GetData()
        {
            var model = new MotorMechanismCompUIModel();
            model.PositionNumber = MotorLinearMotionMechanismViewModel.PositionNumber;
            model.CoordinateValue = MotorLinearMotionMechanismViewModel.CoordinateValue;
            model.MoveType = MotorLinearMotionMechanismViewModel.MoveType;
            return model;
        }

        public void Dispose()
        {
            MotorLinearMotionMechanismViewModel.AfterModified -= MotorLinearMotionMechanismViewModel_AfterModified;
        }
        #endregion

        #region 私有方法
        private void MotorLinearMotionMechanismViewModel_AfterModified(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(nameof(MotorLinearMotionMechanism));
        }

        private void ApplyReadOnly()
        {
            var enabled = !ReadOnly;
            MotorLinearMotionMechanismViewModel.PositionNumberViewModel.IsEnabled = enabled;
            MotorLinearMotionMechanismViewModel.CoordinateValueViewModel.IsEnabled = enabled;
            MotorLinearMotionMechanismViewModel.MoveTypeViewModel.IsEnabled = enabled;
        }

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        private void OnButtonClicked()
        {
            if (isDesign)
            {
                return;
            }
            var response = this.callBack.ExecConfigSvrCtlCmd("MotorMechanism", null);
        }
        #endregion
    }
}
