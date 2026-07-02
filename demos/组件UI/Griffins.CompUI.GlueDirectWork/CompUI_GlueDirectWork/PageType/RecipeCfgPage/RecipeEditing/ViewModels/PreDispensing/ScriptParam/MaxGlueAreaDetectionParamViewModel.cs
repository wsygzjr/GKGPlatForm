using Avalonia.Controls;
using GKG.UI;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 最大点胶面积检测界面-视图模型
    /// </summary>
    public class MaxGlueAreaDetectionParamViewModel : ReactiveObject
    {
        #region 私有字段（数据源）

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;

        #endregion

        #region UI组件模型

        /// <summary>
        /// 最大区域下限-数字输入框视图模型
        /// </summary>
        public NumericViewModel MinMaxAreaViewModel { get; }

        /// <summary>
        /// 最大区域上限-数字输入框视图模型
        /// </summary>
        public NumericViewModel MaxMaxAreaViewModel { get; }

        #endregion

        #region 值改变事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        #endregion

        #region 响应式属性

        /// <summary>
        /// 最大区域下限
        /// </summary>
        public int MinMaxArea
        {
            get => (int)MinMaxAreaViewModel.Value;
            set
            {
                var validValue = Math.Clamp(value, MinMaxAreaViewModel.Minimum, MaxMaxArea);
                MinMaxAreaViewModel.Value = validValue;
                this.RaisePropertyChanged(nameof(MinMaxArea));
            }
        }

        /// <summary>
        /// 最大区域上限
        /// </summary>
        public int MaxMaxArea
        {
            get => (int)MaxMaxAreaViewModel.Value;
            set
            {
                var validValue = Math.Clamp(value, MinMaxArea, MaxMaxAreaViewModel.Maximum);
                MaxMaxAreaViewModel.Value = validValue;
                this.RaisePropertyChanged(nameof(MaxMaxArea));
            }
        }

        #endregion

        /// <summary>
        /// 构造方法（初始化组件、默认值）
        /// </summary>
        public MaxGlueAreaDetectionParamViewModel()
        {
            #region 初始化UI组件模型

            // 初始化最大区域下限
            MinMaxAreaViewModel = new NumericViewModel
            {
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 0,
                Value = 0,
                Increment = 1
            };

            // 初始化最大区域上限
            MaxMaxAreaViewModel = new NumericViewModel
            {
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 0,
                Value = 1000,
                Increment = 1
            };

            #endregion

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
        public void CopyFrom(MaxGlueAreaDetectionCfgInfo model)
        {
            if (model == null) return;

            MaxMaxArea = model.MaxMaxArea;
            MinMaxArea = model.MinMaxArea;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(MaxGlueAreaDetectionCfgInfo model)
        {
            if (model == null) return;

            model.MaxMaxArea = MaxMaxArea;
            model.MinMaxArea = MinMaxArea;
        }

        #endregion

        #region 值改变事件订阅与处理

        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            MinMaxAreaViewModel.ValueChanged += onValueChanged;
            MaxMaxAreaViewModel.ValueChanged += onValueChanged;
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
