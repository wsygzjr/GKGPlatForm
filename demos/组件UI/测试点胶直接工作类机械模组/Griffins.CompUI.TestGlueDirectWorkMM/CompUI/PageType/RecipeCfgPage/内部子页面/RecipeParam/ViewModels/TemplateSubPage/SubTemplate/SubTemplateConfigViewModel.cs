using Avalonia.Controls;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.SubTemplate;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.SubTemplate
{
    /// <summary>
    /// 子模板配置参数-视图模型
    /// </summary>
    public class SubTemplateConfigViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 子模板列表视图模型
        /// </summary>
        public SubTemplateListViewModel SubTemplateListViewModel { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SubTemplateConfigViewModel()
        {
            SubTemplateListViewModel=new SubTemplateListViewModel();

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="subTemplateConfigInfo"></param>
        public void CopyFrom(SubTemplateConfigInfo subTemplateConfigInfo)
        {
            SubTemplateListViewModel.CopyFrom(subTemplateConfigInfo.SubTemplatePointInfoes);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="subTemplateConfigInfo"></param>
        public void CopyTo(SubTemplateConfigInfo subTemplateConfigInfo)
        {
            SubTemplateListViewModel.CopyTo(subTemplateConfigInfo.SubTemplatePointInfoes);
        }
        /// <summary>
        /// 设置视图引用（用于弹窗）
        /// </summary>
        public void SetViewReference(Control view)
        {
            SubTemplateListViewModel.SetViewReference(view);
        }

        /// <summary>
        /// 设置所属模板ID
        /// </summary>
        /// <param name="templateID"></param>
        public void SetTemplateID(Guid templateID)
        {
            SubTemplateListViewModel.SetTemplateID(templateID);
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            SubTemplateListViewModel.AfterModified += onAfterModified;
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