using Avalonia.Controls;
using Griffins.UI;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.AutoGenerate;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.CustomEdit;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 一维矩阵参数-视图模型
    /// </summary>
    public class OneDMatrixParamViewModel : ReactiveObject
    {
        #region 私有字段（数据源）

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;
        #endregion

        #region UI组件模型

        /// <summary>
        /// 启用换行抬针-开关按钮模型
        /// </summary>
        public ToggleSwitchViewModel EnableLiftNeedleOnNewLineViewModel { get; }

        /// <summary>
        /// 启用换行清洁-开关按钮模型
        /// </summary>
        public ToggleSwitchViewModel EnableCleanOnNewLineViewModel { get; }

        /// <summary>
        /// 抬针高度-数字输入框模型
        /// </summary>
        public NumericViewModel LiftNeedleHeightViewModel { get; }

        /// <summary>
        /// 换行首点稳定时间-数字输入框模型
        /// </summary>
        public NumericViewModel NewLineFirstPointStableTimeViewModel { get; }

        /// <summary>
        /// 选中的模板ID-下拉框数据模型
        /// </summary>
        public ComboxViewModel SelectedTemplateModel { get; }
        #endregion

        #region 响应式属性
        /// <summary>
        /// 选中的模板ID
        /// </summary>
        public Guid SelectedTemplateId
        {
            get
            {
                if (SelectedTemplateModel.SelectedItem is ComBoxItem template)
                {
                    return (Guid)template.Value;
                }
                return Guid.Empty;
            }
            set
            {
                if (SelectedTemplateModel.ItemsSource != null)
                {
                    var targetItem = SelectedTemplateModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (Guid)o.Value == value);
                    if (targetItem != null)
                        SelectedTemplateModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedTemplateId));

                }
            }
        }
        /// <summary>
        /// 启用换行抬针
        /// </summary>
        public bool EnableLiftNeedleOnNewLine
        {
            get => EnableLiftNeedleOnNewLineViewModel.IsChecked;
            set
            {
                EnableLiftNeedleOnNewLineViewModel.IsChecked = value;
                updateLiftNeedleRelatedControlEnabled(value);
            }
        }

        /// <summary>
        /// 启用换行清洁
        /// </summary>
        public bool EnableCleanOnNewLine
        {
            get => EnableCleanOnNewLineViewModel.IsChecked;
            set => EnableCleanOnNewLineViewModel.IsChecked = value;
        }

        /// <summary>
        /// 抬针高度
        /// </summary>
        public decimal LiftNeedleHeight
        {
            get => LiftNeedleHeightViewModel.Value;
            set => LiftNeedleHeightViewModel.Value = value;
        }

        /// <summary>
        /// 换行首点稳定时间
        /// </summary>
        public decimal NewLineFirstPointStableTime
        {
            get => NewLineFirstPointStableTimeViewModel.Value;
            set => NewLineFirstPointStableTimeViewModel.Value = value;
        }

        /// <summary>
        /// 一维矩阵参数
        /// </summary>
        public DMatrixParamViewModel OneOfDMatrixParamViewModel { get; }
        #endregion

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 构造函数（初始化组件模型、数据源、事件订阅）
        /// </summary>
        public OneDMatrixParamViewModel()
        {
            OneOfDMatrixParamViewModel = new DMatrixParamViewModel();
            // 1. 初始化下拉框组件
            CacheDataExchange.SubscribTemplateChanged(cacheData_TemplateItemChanged);
            SelectedTemplateModel = new ComboxViewModel();
            SelectedTemplateModel.ItemsSource = getTemplates();
            SelectedTemplateModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            SelectedTemplateModel.SelectedItem = SelectedTemplateModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();
            SelectedTemplateModel.ValueChanged += (s, e) => this.RaisePropertyChanged(nameof(SelectedTemplateId));

            // 2. 初始化开关按钮组件
            EnableLiftNeedleOnNewLineViewModel = new ToggleSwitchViewModel
            {
                IsChecked = false
            };
            EnableLiftNeedleOnNewLineViewModel.ValueChanged += onEnableLiftNeedleOnNewLine_ValueChanged;

            EnableCleanOnNewLineViewModel = new ToggleSwitchViewModel
            {
                IsChecked = false
            };

            // 3. 初始化数字输入框组件
            LiftNeedleHeightViewModel = new NumericViewModel
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 1,
                Value = 10.0m,
                Increment = 0.5m
            };

            NewLineFirstPointStableTimeViewModel = new NumericViewModel
            {
                Minimum = 0,
                Maximum = 5000,
                DecimalPlaces = 0,
                Value = 100,
                Increment = 10
            };
            // 订阅值变更事件
            subscribeValueChanges();
            // 5. 初始化联动状态
            updateLiftNeedleRelatedControlEnabled(EnableLiftNeedleOnNewLine);
        }

        #region 数据同步方法
        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        public void CopyFrom(OneDMatrixParamCfgInfo matrixParamCfgInfo)
        {
            if (matrixParamCfgInfo == null)
            {
                resetToDefault();
                return;
            }
            // 映射所有字段到视图模型
            SelectedTemplateId = matrixParamCfgInfo.TemplateId;
            EnableLiftNeedleOnNewLine = matrixParamCfgInfo.EnableLiftNeedleOnNewLine;
            EnableCleanOnNewLine = matrixParamCfgInfo.EnableCleanOnNewLine;
            LiftNeedleHeight = matrixParamCfgInfo.LiftNeedleHeight;
            NewLineFirstPointStableTime = matrixParamCfgInfo.NewLineFirstPointStableTime;
            OneOfDMatrixParamViewModel.CopyFrom(matrixParamCfgInfo.OneOfDMatrixParamCfgInfo);
            
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(OneDMatrixParamCfgInfo matrixParamCfgInfo)
        {
            if (matrixParamCfgInfo == null) return;

            // 映射所有字段到数据模型
            matrixParamCfgInfo.TemplateId = SelectedTemplateId;
            matrixParamCfgInfo.EnableLiftNeedleOnNewLine = EnableLiftNeedleOnNewLine;
            matrixParamCfgInfo.EnableCleanOnNewLine = EnableCleanOnNewLine;
            matrixParamCfgInfo.LiftNeedleHeight = LiftNeedleHeight;
            matrixParamCfgInfo.NewLineFirstPointStableTime = NewLineFirstPointStableTime;
            OneOfDMatrixParamViewModel.CopyTo(matrixParamCfgInfo.OneOfDMatrixParamCfgInfo);
        }

        /// <summary>
        /// 设置视图引用（用于弹窗等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
            OneOfDMatrixParamViewModel.SetViewReference(view);
        }

        /// <summary>
        /// 获取模板实例列表
        /// </summary>
        /// <returns></returns>
        public List<DispensingTemplateInstanceExecutionObject> GetCommandTemplateInstance()
        {
            int index = 0;
            List<DispensingTemplateInstanceExecutionObject> instances = new List<DispensingTemplateInstanceExecutionObject>();
            for (int row = 0; row < OneOfDMatrixParamViewModel.RowCount; row++)
            {
                for (int column = 0; column < OneOfDMatrixParamViewModel.ColumnCount; column++)
                {
                    instances.Add(new DispensingTemplateInstanceExecutionObject()
                    {
                        SerialNumber= index,
                        InstanceId = Guid.NewGuid(),
                        TemplateInstance = new DispensingCommandTemplateInstance()
                        {
                            TemplateId = SelectedTemplateId
                        }
                    });
                    index++;
                }
            }
            return instances;
        }
        #endregion

        #region 数据源初始化
      
        /// <summary>
        /// 获取模板信息
        /// </summary>
        /// <returns></returns>
        private List<ComBoxItem> getTemplates()
        {
            return CacheDataExchange.GetTemplates();
        }
        /// <summary>
        /// 模板数据源改变通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cacheData_TemplateItemChanged(object? sender, EventArgs e)
        {
            SelectedTemplateModel.ItemsSource = getTemplates();
            SelectedTemplateModel.SelectedItem = SelectedTemplateModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault(o => (Guid)o.Value == SelectedTemplateId);
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 重置为默认值
        /// </summary>
        public void resetToDefault()
        {
            SelectedTemplateId = Guid.NewGuid();
            EnableLiftNeedleOnNewLine = false;
            EnableCleanOnNewLine = false;
            LiftNeedleHeight = 10.0m;
            NewLineFirstPointStableTime = 100;
            OneOfDMatrixParamViewModel.resetToDefault();
        }
        /// <summary>
        /// 联动抬针相关配置的启用状态
        /// </summary>
        private void updateLiftNeedleRelatedControlEnabled(bool isEnabled)
        {
            LiftNeedleHeightViewModel.IsEnabled = isEnabled;
        }

        /// <summary>
        /// 启用换行抬针值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnableLiftNeedleOnNewLine_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                EnableLiftNeedleOnNewLine = (bool)e.NewValue;
            }
        }
        #endregion
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            OneOfDMatrixParamViewModel.AfterModified += onAfterModified;

            // 下拉框事件
            SelectedTemplateModel.ValueChanged += onValueChanged;

            // 开关按钮事件
            EnableLiftNeedleOnNewLineViewModel.ValueChanged += onValueChanged;
            EnableCleanOnNewLineViewModel.ValueChanged += onValueChanged;

            // 数字输入框事件
            LiftNeedleHeightViewModel.ValueChanged += onValueChanged;
            NewLineFirstPointStableTimeViewModel.ValueChanged += onValueChanged;
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        #endregion
    }

}