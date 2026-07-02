using Avalonia.Controls;
using ReactiveUI;

namespace GKG.UI.General
{
    /// <summary>
    /// 电机型直线移动机构-视图模型
    /// </summary>
    public class MotorLinearMotionMechanismViewModel : ReactiveObject
    {
        #region 私有字段

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
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
        public ComboxViewModel MoveTypeViewModel { get; }

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
        public MotorMoveType MoveType
        {
            get => (MotorMoveType)((MoveTypeViewModel.SelectedItem as ComBoxItem)?.Value ?? MotorMoveType.MoveToSpecifiedPosition);
            set
            {
                if (MoveTypeViewModel.ItemsSource != null)
                {
                    var targetItem = MoveTypeViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (MotorMoveType)o.Value == value);
                    if (targetItem != null)
                    {
                        MoveTypeViewModel.SelectedItem = targetItem;
                    }

                    this.RaisePropertyChanged(nameof(MoveType));
                }
            }
        }

        #endregion

        /// <summary>
        /// 构造方法（初始化组件、命令、事件订阅）
        /// </summary>
        public MotorLinearMotionMechanismViewModel()
        {
            #region 初始化UI组件模型

            // 数字输入框
            PositionNumberViewModel = new()// 位置编号
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 0,
                Value = 1,
                Increment = 1
            };

            CoordinateValueViewModel = new()// 坐标值
            {
                Minimum = -10000,
                Maximum = 10000,
                DecimalPlaces = 3,
                Value = 0,
                Increment = 0.001m
            };

            // 下拉框
            MoveTypeViewModel = new();// 移动类型
            var moveTypeDisplayNames = new Dictionary<MotorMoveType, string>
            {
                { MotorMoveType.MoveToSpecifiedPosition, "移动指定位置" },
                { MotorMoveType.MoveToCoordinateValue, "移动到坐标值" }
            };
            MoveTypeViewModel.ItemsSource = EnumExtensions.ToEnumItems(moveTypeDisplayNames);
            MoveTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            #endregion

            // 订阅值改变事件
            subscribeValueChanges();
        }

        #region 值改变事件及订阅

        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            PositionNumberViewModel.ValueChanged += onValueChanged;
            CoordinateValueViewModel.ValueChanged += onValueChanged;
            MoveTypeViewModel.ValueChanged += onValueChanged;
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
        public void CopyFrom(MotorLinearMotionMechanismCfgInfo model)
        {
            PositionNumber = model.PositionNumber;
            CoordinateValue = model.CoordinateValue;
            MoveType = model.MoveType;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="model">数据模型</param>
        public void CopyTo(MotorLinearMotionMechanismCfgInfo model)
        {
            model.PositionNumber = PositionNumber;
            model.CoordinateValue = CoordinateValue;
            model.MoveType = MoveType;
        }

        #endregion
    }
}