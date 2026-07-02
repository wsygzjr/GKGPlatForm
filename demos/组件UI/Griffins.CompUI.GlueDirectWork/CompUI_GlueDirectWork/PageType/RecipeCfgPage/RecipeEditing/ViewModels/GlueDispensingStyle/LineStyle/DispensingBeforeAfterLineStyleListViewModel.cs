using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Tools;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{

    public class DispensingBeforeAfterLineStyleListViewModel :
        DataGridListBaseViewModel<DispensingBeforeAfterLineStyleCfgInfoModel, DispensingBeforeAfterLineStyleCfgInfo>
    {

        /// <summary>
        /// 新增点胶前样式
        /// </summary>
        protected override async Task _addItem()
        {
            var prefix = "线-点胶前后样式";

            var existingNames = ItemsSource.Select(item => item.StyleName).ToList();
            int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);
            // 创建新样式
            var newStyle = new DispensingBeforeAfterLineStyleCfgInfoModel
            {
                StyleID = Guid.NewGuid(),
                StyleName = $"{prefix}{newSerialNumber}",
            };
            newStyle.AfterModified += onAfterModified;
            ItemsSource.Add(newStyle);
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