using Avalonia.Controls;
using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using DynamicData;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Plan
{
    /// <summary>
    /// 点胶方案配置参数-视图模型
    /// </summary>
    public class DispensingPlanConfigViewModel : ReactiveObject
    {
        private Control? _viewReference;

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 点胶流程类型方式下拉框-数据模型
        /// </summary>
        public ComboxViewModel PlanFlowTypeComboBox { get; }
        /// <summary>
        /// 点胶流程类型方式
        /// </summary>
        public DispensingProcessType PlanFlowType
        {
            get => (DispensingProcessType)((PlanFlowTypeComboBox.SelectedItem as ComBoxItem)?.Value ?? DispensingProcessType.SingleStage);
            set
            {
                if (PlanFlowTypeComboBox.ItemsSource != null)
                {
                    var targetItem = PlanFlowTypeComboBox.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (DispensingProcessType)o.Value == value);
                    if (targetItem != null)
                        PlanFlowTypeComboBox.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(PlanFlowType));
                    onDispensingProcessTypeChanged();
                }
            }
        }

        /// <summary>
        /// 单段流程方案配置参数
        /// </summary>
        private DispensingProcessItemViewModel? _singleStagePlanConfigViewModel;

        /// <summary>
        /// 分段流程视图模型
        /// </summary>
        private DispensingProcessListViewModel? _dispensingProcessListViewModel;
        private DispensingProcessListView? _dispensingProcessListView;
    
        /// <summary>
        /// 工作区的视图实例
        /// </summary>
        private object? _workAreaView;
        public object? WorkAreaView
        {
            get => _workAreaView;
            set
            {
                _workAreaView = value;
                this.RaisePropertyChanged(nameof(WorkAreaView));
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingPlanConfigViewModel()
        {
            // 初始化编辑方式下拉框
            PlanFlowTypeComboBox = new ComboxViewModel();
            PlanFlowTypeComboBox.ValueChanged += onPlanFlowTypeChanged;
            initPlanFlowTypeComboBox();

            // 订阅值变更事件
            subscribeValueChanges();

        }
        /// <summary>
        /// 点胶流程类型方式改变
        /// </summary>
        private void onDispensingProcessTypeChanged()
        {
            switch (PlanFlowType)
            {
                case DispensingProcessType.SingleStage:
                    //为空则创建
                    if(_singleStagePlanConfigViewModel==null)
                    {
                        _singleStagePlanConfigViewModel = new DispensingProcessItemViewModel();
                        _singleStagePlanConfigViewModel.SetViewReference(_viewReference!);
                        _singleStagePlanConfigViewModel.DispensingProcessType = DispensingProcessType.SingleStage;

                        _singleStagePlanConfigViewModel.AfterModified += onAfterModified;
                    }
                    WorkAreaView = _singleStagePlanConfigViewModel.WorkAreaView;
                    break;
                case DispensingProcessType.SegmentationStage:
                    if(_dispensingProcessListViewModel==null)
                    {
                        _dispensingProcessListViewModel = new DispensingProcessListViewModel();
                        _dispensingProcessListViewModel.SetViewReference(_viewReference!);
                        _dispensingProcessListView = new DispensingProcessListView { DataContext = _dispensingProcessListViewModel };
                        _dispensingProcessListViewModel.AfterModified += onAfterModified;

                    }
                    WorkAreaView = _dispensingProcessListView;
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 从源复制
        /// </summary>
        /// <param name="planInfo"></param>
        public void CopyFrom(DispensingPlanInfo planInfo)
        {
            if (planInfo == null) return;
            PlanFlowType = planInfo.DispensingProcessType;

            switch (PlanFlowType)
            {
                case DispensingProcessType.SingleStage:
                    if (_singleStagePlanConfigViewModel == null)
                        return;
                    DispensingProcessItemInfo dispensingProcessItemInfo = planInfo.DispensingProcessInfos.Count!=0?
                        planInfo.DispensingProcessInfos[0] : new DispensingProcessItemInfo()
                        {
                            ID=Guid.NewGuid(),
                            SerialNumber=1,
                            DispensingProcessType= DispensingProcessType.SingleStage,
                            DispensingProcessInfo= new SingleStagePlanInfo()
                        };
                    _singleStagePlanConfigViewModel.CopyFrom(dispensingProcessItemInfo);
                    WorkAreaView = _singleStagePlanConfigViewModel.WorkAreaView;
                    break;
                case DispensingProcessType.SegmentationStage:
                    if (_dispensingProcessListViewModel == null)
                        return;
                    _dispensingProcessListViewModel.CopyFrom(planInfo.DispensingProcessInfos);
                   
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="planInfo"></param>
        /// <exception cref="Exception"></exception>
        public void CopyTo(DispensingPlanInfo planInfo)
        {
            if (planInfo == null) return;
            planInfo.DispensingProcessType= PlanFlowType;
            switch (PlanFlowType)
            {
                case DispensingProcessType.SingleStage:
                    if (_singleStagePlanConfigViewModel == null)
                        return;
                    DispensingProcessItemInfo dispensingProcessItemInfo = new DispensingProcessItemInfo();
                    _singleStagePlanConfigViewModel.CopyTo(dispensingProcessItemInfo);

                    //单流程只生成一条记录
                    planInfo.DispensingProcessInfos.Clear();
                    planInfo.DispensingProcessInfos.Add(dispensingProcessItemInfo);   
                    break;
                case DispensingProcessType.SegmentationStage:
                    if (_dispensingProcessListViewModel == null)
                        return;
                    _dispensingProcessListViewModel.CopyTo(planInfo.DispensingProcessInfos);
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// 设置视图引用（用于弹窗）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }
        /// <summary>
        /// 初始化编辑方式下拉框数据源
        /// </summary>
        private void initPlanFlowTypeComboBox()
        {
            var planFlowTypeItems = new List<ComBoxItem>
            {
                new ComBoxItem { Value = DispensingProcessType.SingleStage, DisplayName = "单段流程" },
                new ComBoxItem { Value = DispensingProcessType.SegmentationStage, DisplayName = "分段流程" }
            };

            PlanFlowTypeComboBox.ItemsSource = planFlowTypeItems;
            PlanFlowTypeComboBox.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
        }

        /// <summary>
        /// 编辑方式下拉框值变更事件处理
        /// </summary>
        private void onPlanFlowTypeChanged(object? sender, EventArgs e)
        {
            if (PlanFlowTypeComboBox.SelectedItem is ComBoxItem selectedItem)
            {
                PlanFlowType = (DispensingProcessType)selectedItem.Value;
            }
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {

            PlanFlowTypeComboBox.ValueChanged += onValueChanged;
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