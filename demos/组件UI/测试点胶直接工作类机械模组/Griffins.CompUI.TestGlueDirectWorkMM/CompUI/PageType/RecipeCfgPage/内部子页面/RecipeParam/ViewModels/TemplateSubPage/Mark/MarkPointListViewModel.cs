using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Mark
{
    /// <summary>
    /// Mark点列表视图模型
    /// </summary>
    public class MarkPointListViewModel :
        DataGridListBaseViewModel<MarkPointViewModel, MarkPointInfo>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MarkPointListViewModel():base(1)
        {
           
        }

        /// <summary>
        /// 新增Mark点
        /// </summary>
        protected override async Task _addItem()
        {
            addItem();
        }
        /// <summary>
        /// 新增默认的一个Mark点
        /// </summary>
        public void AddDefaItem()
        {
            if (ItemsSource.Count == 0)
                addItem();
        }
        /// <summary>
        /// 新增Mark点
        /// </summary>
        private void addItem()
        {
            var prefix = "Mark";
            var existingNames = ItemsSource.Select(item => item.PointName).ToList();
            int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);

            var markPointViewModel = new MarkPointViewModel
            {
                PointID = Guid.NewGuid(),
                SerialNumber = newSerialNumber,
                PointName = $"{prefix}{newSerialNumber}",
            };
            markPointViewModel.SetViewReference(_viewReference!);
            //默认生成一项mark点配置信息
            markPointViewModel.MarkPointItemConfigViewModel.AddItem();
            markPointViewModel.AfterModified += onAfterModified;
            ItemsSource.Add(markPointViewModel);
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
    }

}