using Avalonia.Controls;
using ReactiveUI;

namespace GKG.UI.General
{
    /// <summary>
    /// 电机型旋转移动机构-视图模型
    /// </summary>
    public class MotorRotateMotionMechanismViewModel : ReactiveObject
    {
        #region 私有字段

        private Control? _viewReference;

        #endregion

        #region UI组件模型

        /// <summary>
        /// 位置编号-数字输入框视图模型
        /// </summary>
        public NumericViewModel PositionNumberViewModel { get; }

        /// <summary>
        /// 坐标值-数字输入框视图模型
        /// </summary>
        public NumericViewModel CoordinateValueViewModel { get; }

        /// <summary>
        /// 移动类型-下拉框视图模型
        /// </summary>
        public ComboxViewModel MotorRotateMoveTypeViewModel { get; }

        #endregion

        #region 事件

        /// <summary>
        /// 事件（通知外部数据变更）
        /// </summary>
        public event EventHandler? AfterModified;

        #endregion

        #region 响应式属性

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

        /// <summary>
        /// 坐标值
        /// </summary>
        public decimal CoordinateValue
        {
            get => CoordinateValueViewModel.Value;
            set
            {
                CoordinateValueViewModel.Value = value;
                this.RaisePropertyChanged(nameof(CoordinateValue));
            }
        }

        /// <summary>
        /// 移动类型
        /// </summary>
        public MotorRotateMoveType MotorRotateMoveType
        {
            get => (MotorRotateMoveType)((MotorRotateMoveTypeViewModel.SelectedItem as ComBoxItem)?.Value ?? MotorRotateMoveType.MoveToSpecifiedPosition);
            set
            {
                if (MotorRotateMoveTypeViewModel.ItemsSource != null)
                {
                    var targetItem = MotorRotateMoveTypeViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (MotorRotateMoveType)o.Value == value);
                    if (targetItem != null)
                    {
                        MotorRotateMoveTypeViewModel.SelectedItem = targetItem;
                    }

                    this.RaisePropertyChanged(nameof(MotorRotateMoveTypeViewModel));
                }
            }
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public MotorRotateMotionMechanismViewModel()
        {
            #region 初始化UI组件模型

            // 位置编号
            PositionNumberViewModel = new()
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 0,
                Value = 1,
                Increment = 1
            };

            // 坐标值
            CoordinateValueViewModel = new()
            {
                Minimum = -3600,
                Maximum = 3600,
                DecimalPlaces = 1,
                Value = 0,
                Increment = 0.1m
            };

            // 移动类型
            MotorRotateMoveTypeViewModel = new();
            var moveTypeDisplayNames = new Dictionary<MotorRotateMoveType, string>
            {
                { MotorRotateMoveType.MoveToSpecifiedPosition, "移动指定位置" },
                { MotorRotateMoveType.MoveToCoordinateValue, "移动到坐标值" }
            };
            MotorRotateMoveTypeViewModel.ItemsSource = EnumExtensions.ToEnumItems(moveTypeDisplayNames);
            MotorRotateMoveTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

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
            PositionNumberViewModel.ValueChanged += onValueChanged;
            CoordinateValueViewModel.ValueChanged += onValueChanged;
            MotorRotateMoveTypeViewModel.ValueChanged += onValueChanged;
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
            _viewReference = view;
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="model">数据模型</param>
        public void CopyFrom(MotorRotateMotionMechanismCfgInfo model)
        {
            PositionNumber = model.PositionNumber;
            CoordinateValue = model.CoordinateValue;
            MotorRotateMoveType = model.MotorRotateMoveType;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="model">数据模型</param>
        public void CopyTo(MotorRotateMotionMechanismCfgInfo model)
        {
            model.PositionNumber = PositionNumber;
            model.CoordinateValue = CoordinateValue;
            model.MotorRotateMoveType = MotorRotateMoveType;
        }

        #endregion
    }
}
