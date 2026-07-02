using Avalonia.Controls;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 点胶点样式参数配置-视图模型
    /// </summary>
    public class DispensingPointStyleCfgViewModel : ReactiveObject
    {
        /// <summary>
        ///点胶前点样式项变更事件
        /// </summary>
        public event EventHandler<EventArgs>? StyleItemChanged;
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 点胶前点样式列表-视图模型
        /// </summary>
        public DispensingBeforePointStyleListViewModel DispensingBeforePointStyleListViewModel { get; }
        /// <summary>
        /// 点胶后点样式列表-视图模型
        /// </summary>
        public DispensingAfterPointStyleListViewModel DispensingAfterPointStyleListViewModel { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingPointStyleCfgViewModel()
        {
            DispensingBeforePointStyleListViewModel = new DispensingBeforePointStyleListViewModel();
            DispensingAfterPointStyleListViewModel = new DispensingAfterPointStyleListViewModel();

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="cfgInfo"></param>
        public void CopyFrom(DispensingPointStyleCfgInfo cfgInfo)
        {
            DispensingBeforePointStyleListViewModel.CopyFrom(cfgInfo.DispensingBeforePointStyleCfgInfoes);
            DispensingAfterPointStyleListViewModel.CopyFrom(cfgInfo.DispensingAfterPointStyleCfgInfoes);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="cfgInfo"></param>
        public void CopyTo(DispensingPointStyleCfgInfo cfgInfo)
        {
            DispensingBeforePointStyleListViewModel.CopyTo(cfgInfo.DispensingBeforePointStyleCfgInfoes);
            DispensingAfterPointStyleListViewModel.CopyTo(cfgInfo.DispensingAfterPointStyleCfgInfoes);
        }

        /// <summary>
        /// 设置视图引用（用于弹窗、对话框等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            DispensingBeforePointStyleListViewModel.SetViewReference(view);
            DispensingAfterPointStyleListViewModel.SetViewReference(view);
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            DispensingBeforePointStyleListViewModel.ItemsSource.CollectionChanged += (s, e) =>
            {
                StyleItemChanged?.Invoke(this, new EventArgs());
            };
            DispensingAfterPointStyleListViewModel.ItemsSource.CollectionChanged += (s, e) =>
            {
                StyleItemChanged?.Invoke(this, new EventArgs());
            };

            DispensingBeforePointStyleListViewModel.AfterModified += onAfterModified;
            DispensingAfterPointStyleListViewModel.AfterModified += onAfterModified;

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