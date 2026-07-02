using Avalonia.Controls;
using GKG.UI;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 胶水直径计数偏移参数界面-视图模型
    /// </summary>
    public class GlueDiameterCountOffsetParamViewModel : ReactiveObject
    {
        #region 私有字段（数据源）

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;

        #endregion

        #region UI组件模型

        /// <summary>
        /// 当前胶点直径-带标签数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel CurrentGlueDiameterViewModel { get; }

        /// <summary>
        /// 当前个数-数字输入框视图模型
        /// </summary>
        public NumericViewModel CurrentCountViewModel { get; }

        /// <summary>
        /// 当前X偏移-带标签数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel CurrentXOffsetViewModel { get; }

        /// <summary>
        /// 当前Y偏移-带标签数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel CurrentYOffsetViewModel { get; }

        /// <summary>
        /// 胶点直径上限-带标签数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel MaxGlueDiameterViewModel { get; }

        /// <summary>
        /// 胶点直径下限-带标签数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel MinGlueDiameterViewModel { get; }

        /// <summary>
        /// 胶点X正偏移-带标签数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel MaxXOffsetViewModel { get; }

        /// <summary>
        /// 胶点X负偏移-带标签数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel MinXOffsetViewModel { get; }

        /// <summary>
        /// 胶点Y正偏移-带标签数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel MaxYOffsetViewModel { get; }

        /// <summary>
        /// 胶点Y负偏移-带标签数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel MinYOffsetViewModel { get; }

        #endregion

        #region 值改变事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        #endregion

        #region 响应式属性

        /// <summary>
        /// 当前胶点直径
        /// </summary>
        public decimal CurrentGlueDiameter
        {
            get => CurrentGlueDiameterViewModel.Value;
            set
            {
                CurrentGlueDiameterViewModel.Value = value;
                this.RaisePropertyChanged(nameof(CurrentGlueDiameter));
            }
        }

        /// <summary>
        /// 当前个数
        /// </summary>
        public int CurrentCount
        {
            get => (int)CurrentCountViewModel.Value;
            set
            {
                CurrentCountViewModel.Value = value;
                this.RaisePropertyChanged(nameof(CurrentCount));
            }
        }

        /// <summary>
        /// 当前X偏移
        /// </summary>
        public decimal CurrentXOffset
        {
            get => CurrentXOffsetViewModel.Value;
            set
            {
                CurrentXOffsetViewModel.Value = value;
                this.RaisePropertyChanged(nameof(CurrentXOffset));
            }
        }

        /// <summary>
        /// 当前Y偏移
        /// </summary>
        public decimal CurrentYOffset
        {
            get => CurrentYOffsetViewModel.Value;
            set
            {
                CurrentYOffsetViewModel.Value = value;
                this.RaisePropertyChanged(nameof(CurrentYOffset));
            }
        }

        /// <summary>
        /// 胶点直径上限
        /// </summary>
        public decimal MaxGlueDiameter
        {
            get => MaxGlueDiameterViewModel.Value;
            set
            {
                // 校验：不能小于下限
                var validValue = Math.Clamp(value, MinGlueDiameter, MaxGlueDiameterViewModel.Maximum);
                MaxGlueDiameterViewModel.Value = validValue;
                this.RaisePropertyChanged(nameof(MaxGlueDiameter));
            }
        }

        /// <summary>
        /// 胶点直径下限
        /// </summary>
        public decimal MinGlueDiameter
        {
            get => MinGlueDiameterViewModel.Value;
            set
            {
                // 校验：不能大于上限
                var validValue = Math.Clamp(value, MinGlueDiameterViewModel.Minimum, MaxGlueDiameter);
                MinGlueDiameterViewModel.Value = validValue;
                this.RaisePropertyChanged(nameof(MinGlueDiameter));
            }
        }

        /// <summary>
        /// 胶点X正偏移
        /// </summary>
        public decimal MaxXOffset
        {
            get => MaxXOffsetViewModel.Value;
            set
            {
                MaxXOffsetViewModel.Value = value;
                this.RaisePropertyChanged(nameof(MaxXOffset));
            }
        }

        /// <summary>
        /// 胶点X负偏移
        /// </summary>
        public decimal MinXOffset
        {
            get => MinXOffsetViewModel.Value;
            set
            {
                MinXOffsetViewModel.Value = value;
                this.RaisePropertyChanged(nameof(MinXOffset));
            }
        }

        /// <summary>
        /// 胶点Y正偏移
        /// </summary>
        public decimal MaxYOffset
        {
            get => MaxYOffsetViewModel.Value;
            set
            {
                MaxYOffsetViewModel.Value = value;
                this.RaisePropertyChanged(nameof(MaxYOffset));
            }
        }

        /// <summary>
        /// 胶点Y负偏移
        /// </summary>
        public decimal MinYOffset
        {
            get => MinYOffsetViewModel.Value;
            set
            {
                MinYOffsetViewModel.Value = value;
                this.RaisePropertyChanged(nameof(MinYOffset));
            }
        }

        #endregion

        /// <summary>
        /// 构造方法（初始化组件、默认值）
        /// </summary>
        public GlueDiameterCountOffsetParamViewModel()
        {
            #region 初始化UI组件模型

            // 初始化当前胶点直径
            CurrentGlueDiameterViewModel = new NumericWithLableViewModel
            {
                LableText = "mm",
                Minimum = 0,
                Maximum = 1000,
                DecimalPlaces = 3,
                Value = 0,
                Increment = 0.001m
            };

            // 初始化当前个数
            CurrentCountViewModel = new NumericViewModel
            {
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 0,
                Value = 0,
                Increment = 1
            };

            // 初始化当前X偏移
            CurrentXOffsetViewModel = new NumericWithLableViewModel
            {
                LableText = "um",
                Minimum = -10000,
                Maximum = 10000,
                DecimalPlaces = 1,
                Value = 0,
                Increment = 0.1m
            };

            // 初始化当前Y偏移
            CurrentYOffsetViewModel = new NumericWithLableViewModel
            {
                LableText = "um",
                Minimum = -10000,
                Maximum = 10000,
                DecimalPlaces = 1,
                Value = 0,
                Increment = 0.1m
            };

            // 初始化胶点直径上限
            MaxGlueDiameterViewModel = new NumericWithLableViewModel
            {
                LableText = "mm",
                Minimum = 0,
                Maximum = 1000,
                DecimalPlaces = 3,
                Value = 10,
                Increment = 0.001m
            };

            // 初始化胶点直径下限
            MinGlueDiameterViewModel = new NumericWithLableViewModel
            {
                LableText = "mm",
                Minimum = 0,
                Maximum = 1000,
                DecimalPlaces = 3,
                Value = 0,
                Increment = 0.001m
            };

            // 初始化胶点X正偏移
            MaxXOffsetViewModel = new NumericWithLableViewModel
            {
                LableText = "um",
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 1,
                Value = 100,
                Increment = 0.1m
            };

            // 初始化胶点X负偏移
            MinXOffsetViewModel = new NumericWithLableViewModel
            {
                LableText = "um",
                Minimum = -10000,
                Maximum = 0,
                DecimalPlaces = 1,
                Value = -100,
                Increment = 0.1m
            };

            // 初始化胶点Y正偏移
            MaxYOffsetViewModel = new NumericWithLableViewModel
            {
                LableText = "um",
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 1,
                Value = 100,
                Increment = 0.1m
            };

            // 初始化胶点Y负偏移
            MinYOffsetViewModel = new NumericWithLableViewModel
            {
                LableText = "um",
                Minimum = -10000,
                Maximum = 0,
                DecimalPlaces = 1,
                Value = -100,
                Increment = 0.1m
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
        public void CopyFrom(GlueDiameterCountOffsetCfgInfo model)
        {
            CurrentCount = model.CurrentCount;
            CurrentGlueDiameter = model.CurrentGlueDiameter;
            CurrentXOffset = model.CurrentXOffset;
            CurrentYOffset = model.CurrentYOffset;
            MaxGlueDiameter = model.MaxGlueDiameter;
            MaxXOffset = model.MaxXOffset;
            MaxYOffset = model.MaxYOffset;
            MinGlueDiameter = model.MinGlueDiameter;
            MinXOffset = model.MinXOffset;
            MinYOffset = model.MinYOffset;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(GlueDiameterCountOffsetCfgInfo model)
        {
            model.CurrentCount = CurrentCount;
            model.CurrentGlueDiameter = CurrentGlueDiameter;
            model.CurrentXOffset = CurrentXOffset;
            model.CurrentYOffset = CurrentYOffset;
            model.MaxGlueDiameter = MaxGlueDiameter;
            model.MaxXOffset = MaxXOffset;
            model.MaxYOffset = MaxYOffset;
            model.MinGlueDiameter = MinGlueDiameter;
            model.MinXOffset = MinXOffset;
            model.MinYOffset = MinYOffset;
        }

        #endregion

        #region 值改变事件订阅与处理

        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            CurrentGlueDiameterViewModel.ValueChanged += onValueChanged;
            CurrentCountViewModel.ValueChanged += onValueChanged;
            CurrentXOffsetViewModel.ValueChanged += onValueChanged;
            CurrentYOffsetViewModel.ValueChanged += onValueChanged;
            MaxGlueDiameterViewModel.ValueChanged += onValueChanged;
            MinGlueDiameterViewModel.ValueChanged += onValueChanged;
            MaxXOffsetViewModel.ValueChanged += onValueChanged;
            MinXOffsetViewModel.ValueChanged += onValueChanged;
            MaxYOffsetViewModel.ValueChanged += onValueChanged;
            MinYOffsetViewModel.ValueChanged += onValueChanged;
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
