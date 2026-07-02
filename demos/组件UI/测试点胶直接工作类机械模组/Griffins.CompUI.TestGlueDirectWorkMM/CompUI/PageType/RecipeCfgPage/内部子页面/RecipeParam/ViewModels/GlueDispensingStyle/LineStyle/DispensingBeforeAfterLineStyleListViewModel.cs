using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle.LineStyle;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.GlueDispensingStyle
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