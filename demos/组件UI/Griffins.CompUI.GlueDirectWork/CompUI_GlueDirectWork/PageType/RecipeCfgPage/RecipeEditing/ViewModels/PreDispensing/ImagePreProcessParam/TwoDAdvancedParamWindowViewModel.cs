using Avalonia.Controls;
using GKG.UI;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using ReactiveUI;
using System;
using System.Reactive;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 2D高级参数设置弹窗-视图模型
    /// </summary>
    public class TwoDAdvancedParamWindowViewModel : ReactiveObject
    {
        #region 私有字段

        private bool? _dialogResult;
        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;

        #endregion

        #region UI组件模型

        /// <summary>
        /// SelShapeMin - 滑块+数字输入框
        /// </summary>
        public SliderNumericViewModel SelShapeMinViewModel { get; }

        /// <summary>
        /// 焊盘间隔消除大小 - 滑块+数字输入框
        /// </summary>
        public SliderNumericViewModel PadGapEliminateSizeViewModel { get; }

        /// <summary>
        /// 最小阈值 - 滑块+数字输入框
        /// </summary>
        public SliderNumericViewModel MinimumThresholdViewModel { get; }

        /// <summary>
        /// X核 - 滑块+数字输入框
        /// </summary>
        public SliderNumericViewModel XKernelViewModel { get; }

        /// <summary>
        /// 标准圆面积 - 滑块+数字输入框
        /// </summary>
        public SliderNumericViewModel StandardCircleAreaViewModel { get; }

        /// <summary>
        /// 动态阈值偏移 - 滑块+数字输入框
        /// </summary>
        public SliderNumericViewModel DynamicThresholdOffsetViewModel { get; }

        /// <summary>
        /// 膨胀大小2 - 滑块+数字输入框
        /// </summary>
        public SliderNumericViewModel DilationSize2ViewModel { get; }

        /// <summary>
        /// 锡点高阈值 - 滑块+数字输入框
        /// </summary>
        public SliderNumericViewModel SolderHighThresholdViewModel { get; }

        /// <summary>
        /// 使用动态阈值（数值型开关 0/1） - 滑块+数字输入框
        /// </summary>
        public SliderNumericViewModel UseDynamicThresholdViewModel { get; }

        /// <summary>
        /// X方向平滑度 - 滑块+数字输入框
        /// </summary>
        public SliderNumericViewModel SmoothnessXViewModel { get; }

        /// <summary>
        /// EmpMaskWidth - 滑块+数字输入框
        /// </summary>
        public SliderNumericViewModel EmpMaskWidthViewModel { get; }

        /// <summary>
        /// 焊盘面积百分比 - 滑块+数字输入框
        /// </summary>
        public SliderNumericViewModel PadAreaPercentViewModel { get; }

        #endregion

        #region 值改变事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        #endregion

        #region 响应式属性

        /// <summary>
        /// 对话框结果（true:保存，false:取消，null:未操作）
        /// </summary>
        public bool? DialogResult
        {
            get => _dialogResult;
            set => this.RaiseAndSetIfChanged(ref _dialogResult, value);
        }

        /// <summary>
        /// SelShapeMin 值
        /// </summary>
        public int SelShapeMin
        {
            get => (int)SelShapeMinViewModel.Value;
            set
            {
                var valid = (int)Math.Clamp(value, SelShapeMinViewModel.Minimum, SelShapeMinViewModel.Maximum);
                SelShapeMinViewModel.Value = valid;
                this.RaisePropertyChanged(nameof(SelShapeMin));
            }
        }

        /// <summary>
        /// 焊盘间隔消除大小
        /// </summary>
        public int PadGapEliminateSize
        {
            get => (int)PadGapEliminateSizeViewModel.Value;
            set
            {
                var valid = (int)Math.Clamp(value, PadGapEliminateSizeViewModel.Minimum, PadGapEliminateSizeViewModel.Maximum);
                PadGapEliminateSizeViewModel.Value = valid;
                this.RaisePropertyChanged(nameof(PadGapEliminateSize));
            }
        }

        /// <summary>
        /// 最小阈值
        /// </summary>
        public int MinimumThreshold
        {
            get => (int)MinimumThresholdViewModel.Value;
            set
            {
                var valid = (int)Math.Clamp(value, MinimumThresholdViewModel.Minimum, MinimumThresholdViewModel.Maximum);
                MinimumThresholdViewModel.Value = valid;
                this.RaisePropertyChanged(nameof(MinimumThreshold));
            }
        }

        /// <summary>
        /// X核
        /// </summary>
        public int XKernel
        {
            get => (int)XKernelViewModel.Value;
            set
            {
                var valid = (int)Math.Clamp(value, XKernelViewModel.Minimum, XKernelViewModel.Maximum);
                XKernelViewModel.Value = valid;
                this.RaisePropertyChanged(nameof(XKernel));
            }
        }

        /// <summary>
        /// 标准圆面积
        /// </summary>
        public int StandardCircleArea
        {
            get => (int)StandardCircleAreaViewModel.Value;
            set
            {
                var valid = (int)Math.Clamp(value, StandardCircleAreaViewModel.Minimum, StandardCircleAreaViewModel.Maximum);
                StandardCircleAreaViewModel.Value = valid;
                this.RaisePropertyChanged(nameof(StandardCircleArea));
            }
        }

        /// <summary>
        /// 动态阈值偏移
        /// </summary>
        public int DynamicThresholdOffset
        {
            get => (int)DynamicThresholdOffsetViewModel.Value;
            set
            {
                var valid = (int)Math.Clamp(value, DynamicThresholdOffsetViewModel.Minimum, DynamicThresholdOffsetViewModel.Maximum);
                DynamicThresholdOffsetViewModel.Value = valid;
                this.RaisePropertyChanged(nameof(DynamicThresholdOffset));
            }
        }

        /// <summary>
        /// 膨胀大小2
        /// </summary>
        public int DilationSize2
        {
            get => (int)DilationSize2ViewModel.Value;
            set
            {
                var valid = (int)Math.Clamp(value, DilationSize2ViewModel.Minimum, DilationSize2ViewModel.Maximum);
                DilationSize2ViewModel.Value = valid;
                this.RaisePropertyChanged(nameof(DilationSize2));
            }
        }

        /// <summary>
        /// 锡点高阈值
        /// </summary>
        public int SolderHighThreshold
        {
            get => (int)SolderHighThresholdViewModel.Value;
            set
            {
                var valid = (int)Math.Clamp(value, SolderHighThresholdViewModel.Minimum, SolderHighThresholdViewModel.Maximum);
                SolderHighThresholdViewModel.Value = valid;
                this.RaisePropertyChanged(nameof(SolderHighThreshold));
            }
        }

        /// <summary>
        /// 使用动态阈值（0 = 否, 1 = 是）
        /// </summary>
        public int UseDynamicThreshold
        {
            get => (int)UseDynamicThresholdViewModel.Value;
            set
            {
                var valid = (int)Math.Clamp(value, UseDynamicThresholdViewModel.Minimum, UseDynamicThresholdViewModel.Maximum);
                UseDynamicThresholdViewModel.Value = valid;
                this.RaisePropertyChanged(nameof(UseDynamicThreshold));
            }
        }

        /// <summary>
        /// X方向平滑度
        /// </summary>
        public decimal SmoothnessX
        {
            get => SmoothnessXViewModel.Value;
            set
            {
                var valid = Math.Clamp(value, SmoothnessXViewModel.Minimum, SmoothnessXViewModel.Maximum);
                SmoothnessXViewModel.Value = valid;
                this.RaisePropertyChanged(nameof(SmoothnessX));
            }
        }

        /// <summary>
        /// EmpMaskWidth
        /// </summary>
        public int EmpMaskWidth
        {
            get => (int)EmpMaskWidthViewModel.Value;
            set
            {
                var valid = (int)Math.Clamp(value, EmpMaskWidthViewModel.Minimum, EmpMaskWidthViewModel.Maximum);
                EmpMaskWidthViewModel.Value = valid;
                this.RaisePropertyChanged(nameof(EmpMaskWidth));
            }
        }

        /// <summary>
        /// 焊盘面积百分比
        /// </summary>
        public decimal PadAreaPercent
        {
            get => PadAreaPercentViewModel.Value;
            set
            {
                var valid = Math.Clamp(value, PadAreaPercentViewModel.Minimum, PadAreaPercentViewModel.Maximum);
                PadAreaPercentViewModel.Value = valid;
                this.RaisePropertyChanged(nameof(PadAreaPercent));
            }
        }

        #endregion

        #region 命令
        /// <summary>
        /// 保存命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        /// <summary>
        /// 取消命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        #endregion

        /// <summary>
        /// 构造函数（初始化组件、默认值）
        /// </summary>
        public TwoDAdvancedParamWindowViewModel()
        {
            // 初始化命令
            SaveCommand = ReactiveCommand.Create(save);
            CancelCommand = ReactiveCommand.Create(cancel);

            #region 初始化滑块+数字输入框视图模型并设定合理范围与默认值
            // 初始化滑块+数字输入框视图模型并设定合理范围与默认值

            SelShapeMinViewModel = new()// SelShapeMin
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 0,
                Value = 10,
                Increment = 1
            };

            PadGapEliminateSizeViewModel = new()// 焊盘间隔消除大小
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 0,
                Value = 5,
                Increment = 1
            };

            MinimumThresholdViewModel = new()// 最小阈
            {
                Minimum = 0,
                Maximum = 255,
                DecimalPlaces = 0,
                Value = 20,
                Increment = 1
            };

            XKernelViewModel = new()// X核
            {
                Minimum = 1,
                Maximum = 31,
                DecimalPlaces = 0,
                Value = 3,
                Increment = 2
            };

            StandardCircleAreaViewModel = new()// 标准圆面积
            {
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 0,
                Value = 200,
                Increment = 1
            };

            DynamicThresholdOffsetViewModel = new()// 动态阈值偏移
            {
                Minimum = -100,
                Maximum = 100,
                DecimalPlaces = 0,
                Value = 0,
                Increment = 1
            };

            DilationSize2ViewModel = new()// 膨胀大小2
            {
                Minimum = 0,
                Maximum = 50,
                DecimalPlaces = 0,
                Value = 1,
                Increment = 1
            };

            SolderHighThresholdViewModel = new()// 锡点高阈值
            {
                Minimum = 0,
                Maximum = 255,
                DecimalPlaces = 0,
                Value = 200,
                Increment = 1
            };

            UseDynamicThresholdViewModel = new()// 使用动态阈值
            {
                Minimum = 0,
                Maximum = 1,
                DecimalPlaces = 0,
                Value = 1,
                Increment = 1
            };

            SmoothnessXViewModel = new()// X方向平滑度
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 1,
                Value = 10,
                Increment = 0.1m
            };

            EmpMaskWidthViewModel = new()// EmpMaskWidth
            {
                Minimum = 0,
                Maximum = 1000,
                DecimalPlaces = 0,
                Value = 50,
                Increment = 1
            };

            PadAreaPercentViewModel = new()// 焊盘面积百分比
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 2,
                Value = 80,
                Increment = 0.1m
            };

            #endregion

            subscribeValueChanges();
        }

        #region 命令定义

        /// <summary>
        /// /保存逻辑
        /// </summary>
        private void save()
        {
            DialogResult = true;
        }

        /// <summary>
        /// 取消逻辑
        /// </summary>
        private void cancel()
        {
            DialogResult = false;
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
        public void CopyFrom(TwoDAdvancedCfgInfo model)
        {
            // 从数据模型复制数据到ViewModel
            SelShapeMin = model.SelShapeMin;
            PadGapEliminateSize = model.PadGapEliminateSize;
            MinimumThreshold = model.MinimumThreshold;
            XKernel = model.XKernel;
            StandardCircleArea = model.StandardCircleArea;
            DynamicThresholdOffset = model.DynamicThresholdOffset;
            DilationSize2 = model.DilationSize2;
            SolderHighThreshold = model.SolderHighThreshold;
            UseDynamicThreshold = model.UseDynamicThreshold;
            SmoothnessX = model.SmoothnessX;
            EmpMaskWidth = model.EmpMaskWidth;
            PadAreaPercent = model.PadAreaPercent;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(TwoDAdvancedCfgInfo model)
        {
            // 从ViewModel复制数据到数据模型
            model.SelShapeMin = SelShapeMin;
            model.PadGapEliminateSize = PadGapEliminateSize;
            model.MinimumThreshold = MinimumThreshold;
            model.XKernel = XKernel;
            model.StandardCircleArea = StandardCircleArea;
            model.DynamicThresholdOffset = DynamicThresholdOffset;
            model.DilationSize2 = DilationSize2;
            model.SolderHighThreshold = SolderHighThreshold;
            model.UseDynamicThreshold = UseDynamicThreshold;
            model.SmoothnessX = SmoothnessX;
            model.EmpMaskWidth = EmpMaskWidth;
            model.PadAreaPercent = PadAreaPercent;
        }

        #endregion

        #region 值改变事件订阅与处理

        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            SelShapeMinViewModel.ValueChanged += onValueChanged;
            PadGapEliminateSizeViewModel.ValueChanged += onValueChanged;
            MinimumThresholdViewModel.ValueChanged += onValueChanged;
            XKernelViewModel.ValueChanged += onValueChanged;
            StandardCircleAreaViewModel.ValueChanged += onValueChanged;
            DynamicThresholdOffsetViewModel.ValueChanged += onValueChanged;
            DilationSize2ViewModel.ValueChanged += onValueChanged;
            SolderHighThresholdViewModel.ValueChanged += onValueChanged;
            UseDynamicThresholdViewModel.ValueChanged += onValueChanged;
            SmoothnessXViewModel.ValueChanged += onValueChanged;
            EmpMaskWidthViewModel.ValueChanged += onValueChanged;
            PadAreaPercentViewModel.ValueChanged += onValueChanged;
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
