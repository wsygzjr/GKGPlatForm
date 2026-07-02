using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle.PointStyle;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.GlueDispensingStyle
{
    /// <summary>
    /// 点胶后点样式列表-视图模型
    /// </summary>
    public class DispensingAfterPointStyleListViewModel :
        DataGridListBaseViewModel<DispensingAfterPointStyleCfgInfoModel, DispensingAfterPointStyleCfgInfo>
    {

        #region 实现基类抽象方法
        /// <summary>
        /// 新增点胶后点样式
        /// </summary>
        protected override async Task _addItem()
        {
            var prefix = "点-点胶后样式";
            var existingNames = ItemsSource.Select(item => item.StyleName).ToList();
            int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);
            var newStyle = new DispensingAfterPointStyleCfgInfoModel
            {
                StyleID = Guid.NewGuid(),
                StyleName = $"{prefix}{newSerialNumber}",
                StabilizationTime = 0,
                LiftHeight = 5.0m,
                LiftSpeed = 50.0m
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
        #endregion
    }

   
}