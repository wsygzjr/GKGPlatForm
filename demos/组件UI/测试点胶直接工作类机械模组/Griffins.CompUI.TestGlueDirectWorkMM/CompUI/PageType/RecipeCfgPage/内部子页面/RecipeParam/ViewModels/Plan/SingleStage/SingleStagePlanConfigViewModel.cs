using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Mark;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Plan
{
    /// <summary>
    /// 单段流程方案配置参数-视图模型
    /// </summary>
    public class SingleStagePlanConfigViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// Mark配置
        /// </summary>
        public MarkConfigViewModel MarkConfigViewModel { get; }
        /// <summary>
        /// 方案中区域列表
        /// </summary>
        public PlanAreaListViewModel PlanAreaListViewModel { get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SingleStagePlanConfigViewModel()
        {
            MarkConfigViewModel = new MarkConfigViewModel();
            PlanAreaListViewModel=new PlanAreaListViewModel();

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="singleStagePlanInfo"></param>
        public void CopyFrom(SingleStagePlanInfo singleStagePlanInfo)
        {
            MarkConfigViewModel.CopyFrom(singleStagePlanInfo.MarkConfigInfo);
            PlanAreaListViewModel.CopyFrom(singleStagePlanInfo.PlanAreaInfoes);

        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="singleStagePlanInfo"></param>
        public void CopyTo(SingleStagePlanInfo singleStagePlanInfo)
        {
            MarkConfigViewModel.CopyTo(singleStagePlanInfo.MarkConfigInfo);
            PlanAreaListViewModel.CopyTo(singleStagePlanInfo.PlanAreaInfoes);

        }
        /// <summary>
        /// 设置视图引用（用于弹窗）
        /// </summary>
        public void SetViewReference(Control view)
        {
            MarkConfigViewModel.SetViewReference(view);
            PlanAreaListViewModel.SetViewReference(view);

        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            MarkConfigViewModel.AfterModified += onAfterModified;
            PlanAreaListViewModel.AfterModified += onAfterModified;
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