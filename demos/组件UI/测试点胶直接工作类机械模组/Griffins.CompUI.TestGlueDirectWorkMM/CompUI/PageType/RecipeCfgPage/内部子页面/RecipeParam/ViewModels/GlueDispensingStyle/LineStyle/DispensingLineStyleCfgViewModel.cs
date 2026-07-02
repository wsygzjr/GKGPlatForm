using Avalonia.Controls;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle.LineStyle;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.GlueDispensingStyle
{
    /// <summary>
    /// 点胶线样式参数配置-视图模型
    /// </summary>
    public class DispensingLineStyleCfgViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        ///点胶线样式项变更事件
        /// </summary>
        public event EventHandler<EventArgs>? StyleItemChanged;
        /// <summary>
        /// 点胶前后线样式列表-视图模型
        /// </summary>
        public DispensingBeforeAfterLineStyleListViewModel DispensingBeforeAfterLineStyleListViewModel { get; }
        /// <summary>
        /// 点胶中线样式列表-视图模型
        /// </summary>
        public DispensingMiddleLineStyleListViewModel DispensingMiddleLineStyleListViewModel { get; }
        

        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingLineStyleCfgViewModel()
        {
            DispensingBeforeAfterLineStyleListViewModel = new DispensingBeforeAfterLineStyleListViewModel();
            DispensingMiddleLineStyleListViewModel = new DispensingMiddleLineStyleListViewModel();

         
            // 订阅值变更事件
            subscribeValueChanges();
        }


        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="cfgInfo"></param>
        public void CopyFrom(DispensingLineStyleCfgInfo cfgInfo)
        {
            DispensingBeforeAfterLineStyleListViewModel.CopyFrom(cfgInfo.DispensingBeforeAfterLineStyleCfgInfoes);
            DispensingMiddleLineStyleListViewModel.CopyFrom(cfgInfo.DispensingMiddleLineStyleCfgInfoes);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="cfgInfo"></param>
        public void CopyTo(DispensingLineStyleCfgInfo cfgInfo)
        {
            DispensingBeforeAfterLineStyleListViewModel.CopyTo(cfgInfo.DispensingBeforeAfterLineStyleCfgInfoes);
            DispensingMiddleLineStyleListViewModel.CopyTo(cfgInfo.DispensingMiddleLineStyleCfgInfoes);
        }

        /// <summary>
        /// 设置视图引用（用于弹窗、对话框等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            DispensingBeforeAfterLineStyleListViewModel.SetViewReference(view);
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            DispensingBeforeAfterLineStyleListViewModel.ItemsSource.CollectionChanged += (s, e) =>
            {
                StyleItemChanged?.Invoke(this, new EventArgs());
            };
            DispensingMiddleLineStyleListViewModel.ItemsSource.CollectionChanged += (s, e) =>
            {
                StyleItemChanged?.Invoke(this, new EventArgs());
            };

            DispensingBeforeAfterLineStyleListViewModel.AfterModified += onAfterModified;
            DispensingMiddleLineStyleListViewModel.AfterModified += onAfterModified;
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