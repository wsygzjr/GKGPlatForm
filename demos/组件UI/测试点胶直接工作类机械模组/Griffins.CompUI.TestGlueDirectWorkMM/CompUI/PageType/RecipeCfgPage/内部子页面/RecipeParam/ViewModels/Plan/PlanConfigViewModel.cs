using Avalonia.Controls;
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
    /// 方案配置参数-视图模型
    /// </summary>
    public class PlanConfigViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 点胶方案视图模型
        /// </summary>
        public DispensingPlanConfigViewModel DispensingPlanConfigViewModel { get; }

        /// <summary>
        /// 点胶方案其他参数配置
        /// </summary>
        public DispensingPlanOtherConfigInfoViewModel DispensingPlanOtherConfigInfoViewModel {  get; }

        /// 构造函数
        /// </summary>
        public PlanConfigViewModel()
        {
            DispensingPlanConfigViewModel=new DispensingPlanConfigViewModel();
            DispensingPlanOtherConfigInfoViewModel=new DispensingPlanOtherConfigInfoViewModel();

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="planConfigInfo"></param>
        public void CopyFrom(DispensingPlanConfigInfo planConfigInfo)
        {
            if (planConfigInfo == null) return;
            DispensingPlanConfigViewModel.CopyFrom(planConfigInfo.PlanInfo);
            DispensingPlanOtherConfigInfoViewModel.CopyFrom(planConfigInfo.DispensingPlanOtherConfigInfo);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="planConfigInfo"></param>
        public void CopyTo(DispensingPlanConfigInfo planConfigInfo)
        {
            if (planConfigInfo == null) return;
            DispensingPlanConfigViewModel.CopyTo(planConfigInfo.PlanInfo);
            DispensingPlanOtherConfigInfoViewModel.CopyTo(planConfigInfo.DispensingPlanOtherConfigInfo);

        }
        /// <summary>
        /// 设置视图引用（用于弹窗）
        /// </summary>
        public void SetViewReference(Control view)
        {
            DispensingPlanConfigViewModel.SetViewReference(view);
            DispensingPlanOtherConfigInfoViewModel.SetViewReference(view);
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            DispensingPlanConfigViewModel.AfterModified += onAfterModified;
            DispensingPlanOtherConfigInfoViewModel.AfterModified += onAfterModified;
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