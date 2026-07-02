using System;
using System.Collections.ObjectModel;
using System.Reactive;
using GKG.UI;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels
{
    public class TrackBasicParamCompUIViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段
        private bool isDesign;

        private ICompUIRunTimeCallBack callBack;
        #endregion

        #region 组件UI模型
        public ToggleSwitchViewModel RefluxNoBoardViewModel { get; }
        public ToggleSwitchViewModel SlavePriorityViewModel { get; }
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
        /// 回流不回板
        /// </summary>
        public bool RefluxNoBoard
        {
            get { return RefluxNoBoardViewModel.IsChecked; }
            set { this.RaisePropertyChanged(nameof(RefluxNoBoard)); }
        }

        /// <summary>
        /// 是否下机位要板优先
        /// </summary>
        public bool SlavePriority
        {
            get { return SlavePriorityViewModel.IsChecked; }
            set { this.RaisePropertyChanged(nameof(SlavePriority)); }
        }

        #endregion

        #region 命令
        public ReactiveCommand<Unit, Unit> ButtonClickCommand { get; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数（初始化）
        /// </summary>
        public TrackBasicParamCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;

            RefluxNoBoardViewModel = new() { IsChecked = false };
            SlavePriorityViewModel = new() { IsChecked = false };
            ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);

            //监听响应式属性变化
            this.WhenAnyValue(x => x.RefluxNoBoardViewModel.IsChecked)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(RefluxNoBoard)));
            this.WhenAnyValue(x => x.SlavePriorityViewModel.IsChecked)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(SlavePriority)));
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
            var response = this.callBack.ExecConfigSvrCtlCmd("TrackBasicParam", null);
        }
        #endregion
    }
}