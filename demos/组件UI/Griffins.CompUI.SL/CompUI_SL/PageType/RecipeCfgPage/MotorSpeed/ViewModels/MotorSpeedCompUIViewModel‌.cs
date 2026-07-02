using Avalonia.Controls.Primitives;
using GKG.UI;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.SL.RecipeCfgPage.ViewModels
{
    internal class MotorSpeedCompUIViewModel‌ : ReactiveObject, IDisposable
    {
        #region 私有字段
        private bool isDesign;

        private ICompUIRunTimeCallBack callBack;
        #endregion

        public NumericWithLableViewModel FrontMotorSpeedViewModel {  get; }
        public NumericWithLableViewModel BehindMotorSpeedViewModel { get; }

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
        /// 前边轨电机速度
        /// </summary>
        public int FrontMotorSpeed
        {
            get => (int)FrontMotorSpeedViewModel.Value;
            set { this.RaisePropertyChanged(nameof(FrontMotorSpeed)); }
        }

        /// <summary>
        /// 后边轨电机速度
        /// </summary>
        public int BehindMotorSpeed
        {
            get => (int)BehindMotorSpeedViewModel.Value;
            set { this.RaisePropertyChanged(nameof(BehindMotorSpeed)); }
        }

        #endregion

        #region 命令
        public ReactiveCommand<Unit, Unit> ButtonClickCommand { get; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数（初始化）
        /// </summary>
        public MotorSpeedCompUIViewModel‌(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;

            FrontMotorSpeedViewModel = new()
            {
                Maximum = 100,
                Minimum = 0,
                Increment = 1,
                DecimalPlaces = 0,
                Value = 0,
                LableText = "mm/s"
            };
            BehindMotorSpeedViewModel = new()
            {
                Maximum = 100,
                Minimum = 0,
                Increment = 1,
                DecimalPlaces = 0,
                Value = 0,
                LableText = "mm/s"
            };
            ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);

            this.WhenAnyValue(x => x.FrontMotorSpeedViewModel.Value)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(FrontMotorSpeed)));
            this.WhenAnyValue(x => x.BehindMotorSpeedViewModel.Value)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(BehindMotorSpeed)));
        }
        #endregion

        #region 公共方法
        public void Dispose()
        {

        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 按钮点击事件
        /// </summary>
        private void OnButtonClicked()
        {
            if (isDesign)
            {
                return;
            }
            var response = this.callBack.ExecConfigSvrCtlCmd("MotorSpeed", null);
        }
        #endregion
    }
}
