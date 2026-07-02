using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Mark
{
    /// <summary>
    /// Mark点项配置-视图模型
    /// 每项中是点的位置列表，
    /// </summary>
    public class MarkPointItemConfigViewModel :
        DataGridListBaseViewModel<MarkPointPositionInfoViewModel, MarkPointPositionInfo>
    {
        public MarkPointItemConfigViewModel() : base(1)
        {
        }
        /// <summary>
        /// 新增Mark点
        /// </summary>
        public MarkPointPositionInfoViewModel AddItem()
        {
            var markPointViewModel = new MarkPointPositionInfoViewModel();
            markPointViewModel.SetViewReference(_viewReference!);
            markPointViewModel.AfterModified += onAfterModified;
            ItemsSource.Add(markPointViewModel);
            return markPointViewModel;
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
        /// <summary>
        /// 新增Mark点
        /// </summary>
        protected override async Task _addItem()
        {
            AddItem();
        }

    }

}