using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;

namespace GKG.UI.General
{
    /// <summary>
    /// 气缸型移动机构-视图模型
    /// </summary>
    public class CylinderMotionMechanismViewModel : ReactiveObject
    {
        #region 私有字段

        private Control? _viewReference;

        #endregion

        #region UI组件模型

        /// <summary>
        /// 伸缩状态-开关按钮视图模型
        /// </summary>
        public ToggleSwitchViewModel ExtendStatusViewModel { get; }

        /// <summary>
        /// 位置编号-数字输入框视图模型
        /// </summary>
        public NumericViewModel PositionNumberViewModel { get; }

        #endregion

        #region 事件

        /// <summary>
        /// 事件（通知外部数据变更）
        /// </summary>
        public event EventHandler? AfterModified;

        #endregion

        #region 响应式属性

        /// <summary>
        /// 伸缩状态
        /// </summary>
        public bool ExtendStatus
        {
            get => ExtendStatusViewModel.IsChecked;
            set
            {
                ExtendStatusViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(ExtendStatus));
            }
        }

        /// <summary>
        /// 位置编号
        /// </summary>
        public int PositionNumber
        {
            get => (int)PositionNumberViewModel.Value;
            set
            {
                PositionNumberViewModel.Value = value;
                this.RaisePropertyChanged(nameof(PositionNumber));
            }
        }

        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        public CylinderMotionMechanismViewModel()
        {
            #region 初始化UI组件模型

            // 伸缩状态
            ExtendStatusViewModel = new() { IsChecked = false };

            // 位置编号
            PositionNumberViewModel = new()
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 0,
                Value = 1,
                Increment = 1
            };

            #endregion

            // 订阅值改变事件
            subscribeValueChanges();
        }

        #region 属性变更及事件订阅

        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            ExtendStatusViewModel.ValueChanged += onValueChanged;
            PositionNumberViewModel.ValueChanged += onValueChanged;
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 设置视图引用（用于弹窗等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view ?? throw new ArgumentNullException(nameof(view));
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="model">数据模型</param>
        public void CopyFrom(CylinderMotionMechanismCfgInfo model)
        {
            ExtendStatus = model.ExtendStatus;
            PositionNumber = model.PositionNumber;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="model">数据模型</param>
        public void CopyTo(CylinderMotionMechanismCfgInfo model)
        {
            model.ExtendStatus = ExtendStatus;
            model.PositionNumber = PositionNumber;
        }

        #endregion
    }
}
