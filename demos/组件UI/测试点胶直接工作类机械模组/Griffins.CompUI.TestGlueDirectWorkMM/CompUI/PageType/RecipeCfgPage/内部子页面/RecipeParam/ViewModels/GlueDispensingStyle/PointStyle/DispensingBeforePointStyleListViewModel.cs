using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle.PointStyle;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.GlueDispensingStyle
{
    /// <summary>
    /// 点胶前点样式列表-视图模型
    /// </summary>
    public class DispensingBeforePointStyleListViewModel :
        DataGridListBaseViewModel<DispensingBeforePointStyleCfgInfoModel, DispensingBeforePointStyleCfgInfo>
    {

        /// <summary>
        /// 新增点胶前样式
        /// </summary>
        protected override async Task _addItem()
        {
            var prefix = "点-点胶前样式";
            var existingNames = ItemsSource.Select(item => item.StyleName).ToList();
            int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);
            var newStyle = new DispensingBeforePointStyleCfgInfoModel
            {
                StyleID = Guid.NewGuid(),
                StyleName = $"{prefix}{newSerialNumber}",
                RotationAngle = 0,
                TiltAngle = 0,
                DispensingHeight = 1.0m,
                StabilizationTime = 0,
                AdvanceValveOpeningTime = 0
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