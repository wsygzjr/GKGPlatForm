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
    internal class TrackWideningCompUIViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段
        private bool isDesign;

        private ICompUIRunTimeCallBack callBack;
        #endregion

        #region 组件UI模型
        public NumericViewModel TrackIDViewModel { get; }
        public NumericWithLableViewModel TrackLocationViewModel { get; }
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
        /// 定轨编号
        /// </summary>
        public int TrackID
        {
            get => (int)TrackIDViewModel.Value;
            set { this.RaisePropertyChanged(nameof(TrackID)); }
        }

        /// <summary>
        /// 定轨固定位置
        /// </summary>
        public int TrackLocation
        {
            get => (int)TrackLocationViewModel.Value;
            set { this.RaisePropertyChanged(nameof(TrackLocation)); }
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

            TrackIDViewModel = new()
            {
                Maximum = 100,
                Minimum = 0,
                Increment = 1,
                DecimalPlaces = 0,
                Value = 0,
            };
            TrackLocationViewModel = new()
            {
                Maximum = 100,
                Minimum = 0,
                Increment = 1,
                DecimalPlaces = 0,
                Value = 0,
                LableText = "mm"
            };
            ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);
            this.WhenAnyValue(x => x.TrackIDViewModel.Value)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(TrackID)));
            this.WhenAnyValue(x => x.TrackLocationViewModel.Value)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(TrackLocation)));
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
