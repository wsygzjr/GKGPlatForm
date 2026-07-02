using Avalonia.Controls;
using GKG.UI;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 检测漏点胶界面-视图模型
    /// </summary>
    public class MissingGlueDetectionParamViewModel : ReactiveObject
    {
        #region 私有字段（数据源）

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;

        #endregion

        #region UI组件模型

        /// <summary>
        /// 点胶个数下限-数字输入框视图模型
        /// </summary>
        public NumericViewModel MinDispensingCountViewModel { get; }

        /// <summary>
        /// 点胶个数上限-数字输入框视图模型
        /// </summary>
        public NumericViewModel MaxDispensingCountViewModel { get; }

        #endregion

        #region 值改变事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        #endregion

        #region 响应式属性

        /// <summary>
        /// 点胶个数下限
        /// </summary>
        public int MinDispensingCount
        {
            get => (int)MinDispensingCountViewModel.Value;
            set
            {
                var validValue = Math.Clamp(value, MinDispensingCountViewModel.Minimum, MaxDispensingCount);
                MinDispensingCountViewModel.Value = validValue;
                this.RaisePropertyChanged(nameof(MinDispensingCount));
            }
        }

        /// <summary>
        /// 点胶个数上限
        /// </summary>
        public int MaxDispensingCount
        {
            get => (int)MaxDispensingCountViewModel.Value;
            set
            {
                var validValue = Math.Clamp(value, MinDispensingCount, MaxDispensingCountViewModel.Maximum);
                MaxDispensingCountViewModel.Value = validValue;
                this.RaisePropertyChanged(nameof(MaxDispensingCount));
            }
        }

        #endregion

        /// <summary>
        /// 构造方法（初始化组件、默认值）
        /// </summary>
        public MissingGlueDetectionParamViewModel()
        {
            #region 初始化UI组件模型

            // 初始化点胶个数下限
            MinDispensingCountViewModel = new NumericViewModel
            {
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 0,
                Value = 0,
                Increment = 1
            };

            // 初始化点胶个数上限
            MaxDispensingCountViewModel = new NumericViewModel
            {
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 0,
                Value = 100,
                Increment = 1
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
        public void CopyFrom(MissingGlueDetectionCfgInfo model)
        {
            if (model == null) return;

            MinDispensingCount = model.MinDispensingCount;
            MaxDispensingCount = model.MaxDispensingCount;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(MissingGlueDetectionCfgInfo model)
        {
            if (model == null) return;

            model.MinDispensingCount = MinDispensingCount;
            model.MaxDispensingCount = MaxDispensingCount;
        }

        #endregion

        #region 值改变事件订阅与处理

        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            MinDispensingCountViewModel.ValueChanged += onValueChanged;
            MaxDispensingCountViewModel.ValueChanged += onValueChanged;
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
