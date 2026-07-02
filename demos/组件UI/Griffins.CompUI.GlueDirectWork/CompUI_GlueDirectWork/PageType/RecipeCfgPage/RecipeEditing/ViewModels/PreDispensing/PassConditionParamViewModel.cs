using Avalonia.Controls;
using GKG.UI;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 通过条件编辑界面-视图模型
    /// </summary>
    public class PassConditionParamViewModel : ReactiveObject
    {
        #region 私有字段（数据源）

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;

        #endregion

        #region UI组件模型

        /// <summary>
        /// 通过条件-下拉框视图模型
        /// </summary>
        public ComboxViewModel PassConditionCombo { get; }

        /// <summary>
        /// 条件-下拉框视图模型
        /// </summary>
        public ComboxViewModel ConditionCombo { get; }

        /// <summary>
        /// 数值调节方式-下拉框视图模型
        /// </summary>
        public ComboxViewModel ValueAdjustModeCombo { get; }

        /// <summary>
        /// 选中的值-带单位数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel SelectedValueViewModel { get; }

        /// <summary>
        /// 脚本界面 视图模型
        /// </summary>
        public ScriptParamViewModel ScriptParamViewModel { get; }

        #endregion

        #region 值改变事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        #endregion

        #region 响应式属性

        /// <summary>
        /// 通过条件
        /// </summary>
        public ScriptParamType PassCondition
        {
            get => (ScriptParamType)((PassConditionCombo.SelectedItem as ComBoxItem)?.Value ?? ScriptParamType.MissingGlueDetection);
            set
            {
                if (PassConditionCombo.ItemsSource != null)
                {
                    var targetItem = PassConditionCombo.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (ScriptParamType)o.Value == value);
                    if (targetItem != null)
                    {
                        PassConditionCombo.SelectedItem = targetItem;
                    }
                    this.RaisePropertyChanged(nameof(PassCondition));
                }
            }
        }

        /// <summary>
        /// 条件
        /// </summary>
        public ConditionType Condition
        {
            get => (ConditionType)((ConditionCombo.SelectedItem as ComBoxItem)?.Value ?? ConditionType.Equal);
            set
            {
                if (ConditionCombo.ItemsSource != null)
                {
                    var targetItem = ConditionCombo.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => o.Value is ConditionType type && type == value);
                    if (targetItem != null)
                        ConditionCombo.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(Condition));
                }
            }
        }

        /// <summary>
        /// 数值调节方式
        /// </summary>
        public ValueAdjustModeType ValueAdjustMode
        {
            get => (ValueAdjustModeType)((ValueAdjustModeCombo.SelectedItem as ComBoxItem)?.Value ?? ValueAdjustModeType.Value);
            set
            {
                if (ValueAdjustModeCombo.ItemsSource != null)
                {
                    var targetItem = ValueAdjustModeCombo.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => o.Value is ValueAdjustModeType type && type == value);
                    if (targetItem != null)
                        ValueAdjustModeCombo.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(ValueAdjustMode));
                }
            }
        }

        /// <summary>
        /// 选中的数值
        /// </summary>
        public decimal SelectedValue
        {
            get => SelectedValueViewModel.Value;
            set
            {
                SelectedValueViewModel.Value = value;
                this.RaisePropertyChanged(nameof(SelectedValue));
            }
        }

        #endregion

        /// <summary>
        /// 构造方法（初始化组件、默认选项）
        /// </summary>
        public PassConditionParamViewModel()
        {
            // 初始化下拉框
            PassConditionCombo = new ComboxViewModel();
            ConditionCombo = new ComboxViewModel();
            ValueAdjustModeCombo = new ComboxViewModel();
            ScriptParamViewModel = new();

            //事件监听
            this.WhenAnyValue(x => x.PassConditionCombo.SelectedItem)
                .Subscribe(_ =>
                {
                    this.RaisePropertyChanged(nameof(PassCondition));
                });


            // 响应式绑定
            this.WhenAnyValue(x => x.PassCondition)
                 .Subscribe(type => ScriptParamViewModel.CurrentParamType = type);

            var passConditionDisplayNames = new Dictionary<ScriptParamType, string>
            {
                { ScriptParamType.MissingGlueDetection, "检测漏点胶" },
                { ScriptParamType.TotalAreaDetection, "总面积检测" },
                { ScriptParamType.MaxGlueAreaDetection, "最大胶点面积检测" },
                { ScriptParamType.GlueDiameterCountOffset, "胶点直径个数偏移检测" },
                { ScriptParamType.DisableChecks, "禁用" },
                { ScriptParamType.GlueCountDetection, "胶点个数检测" },
            };
            PassConditionCombo.ItemsSource = EnumExtensions.ToEnumItems(passConditionDisplayNames);
            PassConditionCombo.DisplayMemberPath = nameof(ComBoxItem.DisplayName);


            var conditionDisplayNames = new Dictionary<ConditionType, string>
            {
                { ConditionType.Interval, "区间" },
                { ConditionType.Less, "小于" },
                { ConditionType.Greater, "大于" },
                { ConditionType.Equal, "等于" },
            };
            ConditionCombo.ItemsSource = EnumExtensions.ToEnumItems(conditionDisplayNames);
            ConditionCombo.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            var adjustModeDisplayNames = new Dictionary<ValueAdjustModeType, string>
            {
                { ValueAdjustModeType.Value, "数值" },
                { ValueAdjustModeType.Percentage, "百分比" },
            };
            ValueAdjustModeCombo.ItemsSource = EnumExtensions.ToEnumItems(adjustModeDisplayNames);
            ValueAdjustModeCombo.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            // 初始化数值输入框（带单位）
            SelectedValueViewModel = new NumericWithLableViewModel
            {
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 2,
                Value = 0,
                Increment = 0.1m
            };

            // 订阅值改变事件
            subscribeValueChanges();
        }

        #region 值改变事件订阅与处理

        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            PassConditionCombo.ValueChanged += onValueChanged;
            ConditionCombo.ValueChanged += onValueChanged;
            ValueAdjustModeCombo.ValueChanged += onValueChanged;
            SelectedValueViewModel.ValueChanged += onValueChanged;
            ScriptParamViewModel.AfterModified += onAfterModified;
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        private void onAfterModified(object? sender, EventArgs e)
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
        /// 从数据模型复制数据到ViewModel ff:疑问
        /// </summary>
        public void CopyFrom(PassConditionCfgInfo model)
        {
            if (model == null) return;

            PassCondition = model.PassCondition;
            Condition = model.Condition;
            ValueAdjustMode = model.ValueAdjustMode;
            SelectedValue = model.SelectedValue;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(PassConditionCfgInfo model)
        {
            if (model == null) return;

            model.PassCondition = PassCondition;
            model.Condition = Condition;
            model.ValueAdjustMode = ValueAdjustMode;
            model.SelectedValue = SelectedValue;
        }

        #endregion
    }
}
