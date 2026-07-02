using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Tools;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 点胶中线样式列表-视图模型
    /// 支持样式的新增、单条删除、批量删除
    /// </summary>
    public class DispensingMiddleLineStyleListViewModel :
        DataGridListBaseViewModel<DispensingMiddleLineStyleCfgInfoModel, DispensingMiddleLineStyleCfgInfo>
    {

        /// <summary>
        /// 新增点胶前样式
        /// </summary>
        protected override async Task _addItem()
        {
            var prefix = "线-点胶中样式";

            var existingNames = ItemsSource.Select(item => item.StyleName).ToList();
            int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);
            var newStyle = new DispensingMiddleLineStyleCfgInfoModel
            {
                StyleID = Guid.NewGuid(),
                StyleName = $"{prefix}{newSerialNumber}",
                IsChecked = false,
                DispensingHeight = 1.0m,
                DispensingSpeed = 10.0m
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