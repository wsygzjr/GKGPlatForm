using GKG.UI;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels
{
    internal class TrackWideningCompUIViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段
        private bool isDesign;

        private ICompUIRunTimeCallBack callBack;
        #endregion

        public ToggleSwitchViewModel CollisionDetectionViewModel {  get; }

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
        /// 是否启用防撞检测
        /// </summary>
        public bool CollisionDetection
        {
            get => CollisionDetectionViewModel.IsChecked;
            set { this.RaisePropertyChanged(nameof(CollisionDetection)); }
        }

        #endregion

        #region 命令
        public ReactiveCommand<Unit, Unit> ButtonClickCommand { get; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数（初始化）
        /// </summary>
        public TrackWideningCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;
            CollisionDetectionViewModel = new() { IsChecked = false};
            ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);

            this.WhenAnyValue(x => x.CollisionDetectionViewModel.IsChecked)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(CollisionDetection)));
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
            var response = this.callBack.ExecConfigSvrCtlCmd("TrackWidening", null);
        }
        #endregion
    }
}
