using Avalonia.Controls;
using Avalonia.VisualTree;
using GKG.UI;
using GKG.UI.General;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 图像预处理参数-视图模型
    /// </summary>
    public class ImagePreProcessParamViewModel : ReactiveObject
    {
        #region 私有字段（数据源）

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;

        private TwoDAdvancedCfgInfo _twoDAdvancedCfgInfo = new TwoDAdvancedCfgInfo();

        #endregion

        #region UI组件模型

        /// <summary>
        /// 低阈值-滑块+数字输入框视图模型
        /// </summary>
        public SliderNumericViewModel LowThresholdViewModel { get; }

        /// <summary>
        /// 高阈值-滑块+数字输入框视图模型
        /// </summary>
        public SliderNumericViewModel HighThresholdViewModel { get; }

        /// <summary>
        /// 矩形度-数字输入框视图模型
        /// </summary>
        public NumericViewModel RectangularityViewModel { get; }

        /// <summary>
        /// 一维测量-开关按钮视图模型
        /// </summary>
        public ToggleSwitchViewModel EnableOneDimensionalMeasurementViewModel { get; }

        /// <summary>
        /// 杂质处理-开关按钮视图模型
        /// </summary>
        public ToggleSwitchViewModel ImpurityProcessingViewModel { get; }

        /// <summary>
        /// 检测框切片个数-数字输入框视图模型
        /// </summary>
        public NumericViewModel DetectionFrameSliceCountViewModel { get; }

        /// <summary>
        /// 允许胶宽超出范围百分比-带标签数字输入框视图模型
        /// </summary>
        public NumericWithLableViewModel GlueWidthExceedPercentViewModel { get; }

        /// <summary>
        /// 杂质过滤-滑块+数字输入框视图模型
        /// </summary>
        public SliderNumericViewModel ImpurityFilterValueViewModel { get; }

        /// <summary>
        /// 目标缝合-滑块+数字输入框视图模型
        /// </summary>
        public SliderNumericViewModel TargetStitchingViewModel { get; }

        /// <summary>
        /// 胶条数-数字输入框视图模型
        /// </summary>
        public NumericViewModel GlueStripCountViewModel { get; }

        /// <summary>
        /// 面积筛选-开关按钮视图模型
        /// </summary>
        public ToggleSwitchViewModel EnableAreaFilterViewModel { get; }

        /// <summary>
        /// 最小面积-滑块+数字输入框视图模型
        /// </summary>
        public SliderNumericViewModel MinimumAreaViewModel { get; }

        /// <summary>
        /// 最大面积-滑块+数字输入框视图模型
        /// </summary>
        public SliderNumericViewModel MaximumAreaViewModel { get; }

        /// <summary>
        /// 填充孔洞-开关按钮视图模型
        /// </summary>
        public ToggleSwitchViewModel FillHolesViewModel { get; }

        /// <summary>
        /// 极性-下拉框视图模型
        /// </summary>
        public ComboxViewModel PolarityViewModel { get; }


        #endregion

        #region 值改变事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        #endregion

        #region 响应式属性

        /// <summary>
        /// 低阈值
        /// </summary>
        public int LowThreshold
        {
            get => (int)LowThresholdViewModel.Value;
            set
            {
                // 校验：不能大于高阈值
                var validValue = Math.Clamp(value, LowThresholdViewModel.Minimum, HighThreshold);
                LowThresholdViewModel.Value = validValue;
                this.RaisePropertyChanged(nameof(LowThreshold));
            }
        }

        /// <summary>
        /// 高阈值
        /// </summary>
        public int HighThreshold
        {
            get => (int)HighThresholdViewModel.Value;
            set
            {
                // 校验：不能小于低阈值
                var validValue = Math.Clamp(value, LowThreshold, HighThresholdViewModel.Maximum);
                HighThresholdViewModel.Value = validValue;
                this.RaisePropertyChanged(nameof(HighThreshold));
            }
        }

        /// <summary>
        /// 矩形度
        /// </summary>
        public decimal Rectangularity
        {
            get => RectangularityViewModel.Value;
            set
            {
                RectangularityViewModel.Value = value;
                this.RaisePropertyChanged(nameof(Rectangularity));
            }
        }

        /// <summary>
        /// 一维测量
        /// </summary>
        public bool EnableOneDimensionalMeasurement
        {
            get => EnableOneDimensionalMeasurementViewModel.IsChecked;
            set
            {
                EnableOneDimensionalMeasurementViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(EnableOneDimensionalMeasurement));
            }
        }

        /// <summary>
        /// 杂质处理
        /// </summary>
        public bool ImpurityProcessing
        {
            get => ImpurityProcessingViewModel.IsChecked;
            set
            {
                ImpurityProcessingViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(ImpurityProcessing));
            }
        }

        /// <summary>
        /// 检测框切片个数
        /// </summary>
        public int DetectionFrameSliceCount
        {
            get => (int)DetectionFrameSliceCountViewModel.Value;
            set
            {
                DetectionFrameSliceCountViewModel.Value = value;
                this.RaisePropertyChanged(nameof(DetectionFrameSliceCount));
            }
        }

        /// <summary>
        /// 允许胶宽超出范围百分比
        /// </summary>
        public decimal GlueWidthExceedPercent
        {
            get => GlueWidthExceedPercentViewModel.Value;
            set
            {
                GlueWidthExceedPercentViewModel.Value = value;
                this.RaisePropertyChanged(nameof(GlueWidthExceedPercent));
            }
        }

        /// <summary>
        /// 杂质过滤值
        /// </summary>
        public int ImpurityFilterValue
        {
            get => (int)ImpurityFilterValueViewModel.Value;
            set
            {
                ImpurityFilterValueViewModel.Value = value;
                this.RaisePropertyChanged(nameof(ImpurityFilterValue));
            }
        }

        /// <summary>
        /// 目标缝合
        /// </summary>
        public int TargetStitching
        {
            get => (int)TargetStitchingViewModel.Value;
            set
            {
                TargetStitchingViewModel.Value = value;
                this.RaisePropertyChanged(nameof(TargetStitching));
            }
        }

        /// <summary>
        /// 胶条数
        /// </summary>
        public int GlueStripCount
        {
            get => (int)GlueStripCountViewModel.Value;
            set
            {
                GlueStripCountViewModel.Value = value;
                this.RaisePropertyChanged(nameof(GlueStripCount));
            }
        }

        /// <summary>
        /// 面积筛选
        /// </summary>
        public bool EnableAreaFilter
        {
            get => EnableAreaFilterViewModel.IsChecked;
            set
            {
                EnableAreaFilterViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(EnableAreaFilter));
            }
        }

        /// <summary>
        /// 最小面积
        /// </summary>
        public int MinimumArea
        {
            get => (int)MinimumAreaViewModel.Value;
            set
            {
                // 校验：不能大于最大面积
                var validValue = Math.Clamp(value, MinimumAreaViewModel.Minimum, MaximumArea);
                MinimumAreaViewModel.Value = validValue;
                this.RaisePropertyChanged(nameof(MinimumArea));
            }
        }

        /// <summary>
        /// 最大面积
        /// </summary>
        public int MaximumArea
        {
            get => (int)MaximumAreaViewModel.Value;
            set
            {
                // 校验：不能小于最小面积
                var validValue = Math.Clamp(value, MinimumArea, MaximumAreaViewModel.Maximum);
                MaximumAreaViewModel.Value = validValue;
                this.RaisePropertyChanged(nameof(MaximumArea));
            }
        }

        /// <summary>
        /// 填充孔洞
        /// </summary>
        public bool FillHoles
        {
            get => FillHolesViewModel.IsChecked;
            set
            {
                FillHolesViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(FillHoles));
            }
        }

        /// <summary>
        /// 极性
        /// </summary>
        public PolarityType Polarity
        {
            get => (PolarityType)((PolarityViewModel.SelectedItem as ComBoxItem)?.Value ?? PolarityType.WhiteBackgroundBlackDot);
            set
            {
                if (PolarityViewModel.ItemsSource != null)
                {
                    var targetItem = PolarityViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (PolarityType)o.Value == value);
                    if (targetItem != null)
                    {
                        PolarityViewModel.SelectedItem = targetItem;
                    }

                    this.RaisePropertyChanged(nameof(Polarity));
                }
            }
        }

        #endregion

        #region 命令

        /// <summary>
        /// 弹出二维高级参数设置窗口命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ShowTwoDAdvancedParamWindowCommand { get; set; }

        #endregion

        /// <summary>
        /// 构造方法（初始化组件、命令、事件订阅）
        /// </summary>
        public ImagePreProcessParamViewModel()
        {
            //命令绑定
            ShowTwoDAdvancedParamWindowCommand = ReactiveCommand.CreateFromTask(ShowTwoDAdvancedParamWindow);

            //初始化开关按钮（默认关闭）
            EnableOneDimensionalMeasurementViewModel = new() { IsChecked = false };//一维测量
            ImpurityProcessingViewModel = new() { IsChecked = false };//杂质处理
            EnableAreaFilterViewModel = new() { IsChecked = false };//面积筛选
            FillHolesViewModel = new() { IsChecked = false };//填充孔洞

            //初始化下拉框
            PolarityViewModel = new(); // 极性
            var polarityDisplayNames = new Dictionary<PolarityType, string>
            {
                { PolarityType.WhiteBackgroundBlackDot, "白底黑点" },
                { PolarityType.BlackBackgroundWhiteDot, "黑底白点" }
            };
            PolarityViewModel.ItemsSource = EnumExtensions.ToEnumItems(polarityDisplayNames);
            PolarityViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            #region 初始化UI组件模型

            // 初始化(滑块)数字输入框（设置合理取值范围和默认值）
            LowThresholdViewModel = new()//低阈值
            {
                Minimum = 0,
                Maximum = 255,
                DecimalPlaces = 0,
                Value = 50,
                Increment = 1
            };

            HighThresholdViewModel = new()//高阈值
            {
                Minimum = 0,
                Maximum = 255,
                DecimalPlaces = 0,
                Value = 150,
                Increment = 1
            };

            ImpurityFilterValueViewModel = new()//杂质过滤
            {
                LableText = " mm",
                Minimum = 0,
                Maximum = 1000,
                DecimalPlaces = 0,
                Value = 100,
                Increment = 1
            };

            MinimumAreaViewModel = new()//最小面积
            {
                LableText = " mm²",
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 0,
                Value = 50,
                Increment = 1
            };

            MaximumAreaViewModel = new()//最大面积
            {
                LableText = " mm²",
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 0,
                Value = 5000,
                Increment = 1
            };

            TargetStitchingViewModel = new()//目标缝合
            {
                LableText = " mm",
                Minimum = 0,
                Maximum = 1000,
                DecimalPlaces = 0,
                Value = 200,
                Increment = 1
            };

            RectangularityViewModel = new()//矩形度
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 2,
                Value = 70,
                Increment = 0.1m
            };

            DetectionFrameSliceCountViewModel = new()//检测框切片个数
            {
                Minimum = 1,
                Maximum = 100,
                DecimalPlaces = 0,
                Value = 10,
                Increment = 1
            };

            GlueStripCountViewModel = new()//胶条数
            {
                Minimum = 1,
                Maximum = 100,
                DecimalPlaces = 0,
                Value = 1,
                Increment = 1
            };

            GlueWidthExceedPercentViewModel = new()//允许胶宽超出范围百分比
            {
                LableText = "%",
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 2,
                Value = 10,
                Increment = 0.1m
            };

            #endregion

            // 订阅值改变事件
            subscribeValueChanges();
        }

        #region 命令定义

        /// <summary>
        /// 弹出二维高级参数设置窗口
        /// </summary>
        public async Task ShowTwoDAdvancedParamWindow()
        {
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", _viewReference);
                return;
            }

            // 备份当前配置，用于取消时恢复
            var backupCfg = new TwoDAdvancedCfgInfo();
            backupCfg.CopyFrom(_twoDAdvancedCfgInfo);

            var editViewModel = new TwoDAdvancedParamWindowViewModel();
            editViewModel.CopyFrom(_twoDAdvancedCfgInfo);

            // 订阅子窗口的修改事件，实现实时预览
            editViewModel.AfterModified += (s, e) =>
            {
                editViewModel.CopyTo(_twoDAdvancedCfgInfo);
                AfterModified?.Invoke(this, e);
            };

            var editWindow = new TwoDAdvancedParamWindow
            {
                DataContext = editViewModel,
                Title = "2D高级参数编辑",
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            editViewModel.SetViewReference(editWindow);

            var result = await editWindow.ShowDialog<bool>(parentWindow);
            if (result)
            {
                // 确认保存
                editViewModel.CopyTo(_twoDAdvancedCfgInfo);
                AfterModified?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // 取消，恢复备份
                _twoDAdvancedCfgInfo.CopyFrom(backupCfg);
                AfterModified?.Invoke(this, EventArgs.Empty);
            }
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
        /// <param name = "model" ></ param >
        public void CopyFrom(ImagePreProcessCfgInfo model)
        {
            LowThreshold = model.LowThreshold;
            HighThreshold = model.HighThreshold;
            Rectangularity = model.Rectangularity;
            EnableOneDimensionalMeasurement = model.EnableOneDimensionalMeasurement;
            ImpurityProcessing = model.ImpurityProcessing;
            DetectionFrameSliceCount = model.DetectionFrameSliceCount;
            GlueWidthExceedPercent = model.GlueWidthExceedPercent;
            ImpurityFilterValue = model.ImpurityFilterValue;
            TargetStitching = model.TargetStitching;
            GlueStripCount = model.GlueStripCount;
            EnableAreaFilter = model.EnableAreaFilter;
            MinimumArea = model.MinimumArea;
            MaximumArea = model.MaximumArea;
            FillHoles = model.FillHoles;
            Polarity = model.Polarity;
            _twoDAdvancedCfgInfo.CopyFrom(model.TwoDAdvancedCfgInfo);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="model"></param>
        public void CopyTo(ImagePreProcessCfgInfo model)
        {
            model.LowThreshold = LowThreshold;
            model.HighThreshold = HighThreshold;
            model.Rectangularity = Rectangularity;
            model.EnableOneDimensionalMeasurement = EnableOneDimensionalMeasurement;
            model.ImpurityProcessing = ImpurityProcessing;
            model.DetectionFrameSliceCount = DetectionFrameSliceCount;
            model.GlueWidthExceedPercent = GlueWidthExceedPercent;
            model.ImpurityFilterValue = ImpurityFilterValue;
            model.TargetStitching = TargetStitching;
            model.GlueStripCount = GlueStripCount;
            model.EnableAreaFilter = EnableAreaFilter;
            model.MinimumArea = MinimumArea;
            model.MaximumArea = MaximumArea;
            model.FillHoles = FillHoles;
            model.Polarity = Polarity;
            model.TwoDAdvancedCfgInfo.CopyFrom(_twoDAdvancedCfgInfo);
        }

        #endregion

        #region 值改变订阅
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            LowThresholdViewModel.ValueChanged += onValueChanged;
            HighThresholdViewModel.ValueChanged += onValueChanged;
            RectangularityViewModel.ValueChanged += onValueChanged;
            DetectionFrameSliceCountViewModel.ValueChanged += onValueChanged;
            GlueWidthExceedPercentViewModel.ValueChanged += onValueChanged;
            ImpurityFilterValueViewModel.ValueChanged += onValueChanged;
            TargetStitchingViewModel.ValueChanged += onValueChanged;
            GlueStripCountViewModel.ValueChanged += onValueChanged;
            MinimumAreaViewModel.ValueChanged += onValueChanged;
            MaximumAreaViewModel.ValueChanged += onValueChanged;
            EnableOneDimensionalMeasurementViewModel.ValueChanged += onValueChanged;
            ImpurityProcessingViewModel.ValueChanged += onValueChanged;
            EnableAreaFilterViewModel.ValueChanged += onValueChanged;
            FillHolesViewModel.ValueChanged += onValueChanged;
            PolarityViewModel.ValueChanged += onValueChanged;
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
