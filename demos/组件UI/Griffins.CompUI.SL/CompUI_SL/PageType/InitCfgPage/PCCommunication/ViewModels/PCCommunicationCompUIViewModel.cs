using GKG.UI;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Reactive;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels
{
    internal class PcCommunicationCompUIViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段
        private bool isDesign;

        private ICompUIRunTimeCallBack callBack;
        #endregion

        #region 组件UI模型
        public ToggleSwitchViewModel UpperCommunicationViewModel { get; }
        public ToggleSwitchViewModel SlaveCommunicationViewModel { get; }
        public ToggleSwitchViewModel SupSMEMAViewModel { get; }
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
        /// 是否与上位机通讯
        /// </summary>
        public bool UpperCommunication
        {
            get => UpperCommunicationViewModel.IsChecked;
            set
            {
                UpperCommunicationViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(UpperCommunication));
            }
        }

        /// <summary>
        /// 是否与下位机通讯
        /// </summary>
        public bool SlaveCommunication
        {
            get => SlaveCommunicationViewModel.IsChecked;
            set
            {
                SlaveCommunicationViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(SlaveCommunication));
            }
        }

        /// <summary>
        /// 是否支持SMEMA
        /// </summary>
        public bool SupSMEMA
        {
            get => SupSMEMAViewModel.IsChecked;
            set
            {
                SupSMEMAViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(SupSMEMA));
            }
        }
        #endregion

        #region 命令
        public ReactiveCommand<Unit, Unit> ButtonClickCommand { get; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数（初始化）
        /// </summary>
        public PcCommunicationCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;
            UpperCommunicationViewModel = new() { IsChecked = false };
            SlaveCommunicationViewModel = new() { IsChecked = false };
            SupSMEMAViewModel = new() { IsChecked = false };
            ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);

            this.WhenAnyValue(x => x.UpperCommunicationViewModel.IsChecked)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(UpperCommunication)));
            this.WhenAnyValue(x => x.SlaveCommunicationViewModel.IsChecked)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(SlaveCommunication)));
            this.WhenAnyValue(x => x.SupSMEMAViewModel.IsChecked)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(SupSMEMA)));
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
            var response = this.callBack.ExecConfigSvrCtlCmd("PcCommunication", null);
        }


        #endregion
    }
}
