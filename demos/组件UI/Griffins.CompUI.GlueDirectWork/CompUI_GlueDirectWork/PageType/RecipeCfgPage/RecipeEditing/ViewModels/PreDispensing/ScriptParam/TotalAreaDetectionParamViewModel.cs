using Avalonia.Controls;
using GKG.UI;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 总面积检测界面-视图模型
    /// </summary>
    public class TotalAreaDetectionParamViewModel : ReactiveObject
    {
        #region 私有字段（数据源）

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;

        #endregion

        #region UI组件模型

        /// <summary>
        /// 总面积下限-带标签数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel MinTotalAreaViewModel { get; }

        /// <summary>
        /// 总面积上限-带标签数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel MaxTotalAreaViewModel { get; }

        #endregion

        #region 值改变事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        #endregion

        #region 响应式属性

        /// <summary>
        /// 总面积下限
        /// </summary>
        public decimal MinTotalArea
        {
            get => MinTotalAreaViewModel.Value;
            set
            {
                // 校验：不能大于上限
                var validValue = Math.Clamp(value, MinTotalAreaViewModel.Minimum, MaxTotalArea);
                MinTotalAreaViewModel.Value = validValue;
                this.RaisePropertyChanged(nameof(MinTotalArea));
            }
        }

        /// <summary>
        /// 总面积上限
        /// </summary>
        public decimal MaxTotalArea
        {
            get => MaxTotalAreaViewModel.Value;
            set
            {
                // 校验：不能小于下限
                var validValue = Math.Clamp(value, MinTotalArea, MaxTotalAreaViewModel.Maximum);
                MaxTotalAreaViewModel.Value = validValue;
                this.RaisePropertyChanged(nameof(MaxTotalArea));
            }
        }

        #endregion

        /// <summary>
        /// 构造方法（初始化组件、默认值）
        /// </summary>
        public TotalAreaDetectionParamViewModel()
        {
            #region 初始化UI组件模型

            // 初始化总面积下限
            MinTotalAreaViewModel = new NumericWithLableViewModel
            {
                LableText = " mm²",
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 2,
                Value = 0,
                Increment = 0.1m
            };

            // 初始化总面积上限
            MaxTotalAreaViewModel = new NumericWithLableViewModel
            {
                LableText = " mm²",
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 2,
                Value = 1000,
                Increment = 0.1m
            };

            #endregion

            // 订阅值改变事件
            subscribeValueChanges();
        }

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
        public void CopyFrom(TotalAreaDetectionCfgInfo model)
        {
            MinTotalArea = model.MinTotalArea;
            MaxTotalArea = model.MaxTotalArea;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(TotalAreaDetectionCfgInfo model)
        {
            model.MinTotalArea = MinTotalArea;
            model.MaxTotalArea = MaxTotalArea;
        }

        #endregion

        #region 值改变事件订阅与处理

        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            MinTotalAreaViewModel.ValueChanged += onValueChanged;
            MaxTotalAreaViewModel.ValueChanged += onValueChanged;
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        #endregion
    }
}
