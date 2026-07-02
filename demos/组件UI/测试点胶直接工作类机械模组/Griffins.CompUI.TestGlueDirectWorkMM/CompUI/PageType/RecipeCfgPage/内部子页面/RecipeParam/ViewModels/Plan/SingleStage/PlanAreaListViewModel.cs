using Avalonia.Controls;
using Avalonia.VisualTree;
using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using Griffins.UI.General;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan;
using PropertyModels.Extensions;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Plan
{
    /// <summary>
    ///方案中区域列表视图模型
    /// </summary>
    public class PlanAreaListViewModel :
        DataGridListBaseViewModel<PlanAreaItemViewModel, PlanAreaInfo>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PlanAreaListViewModel()
        {
            CacheDataExchange.SubscribAreaListItemChanged(onAreaListItemChanged);
        }

        /// <summary>
        /// 新增Mark点
        /// </summary>
        protected override async Task _addItem()
        {
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", _viewReference);
                return;
            }
            try
            {
                var editViewModel = new SelectAreaWindowViewModel();
                var editWindow = new SelectAreaWindow
                {
                    DataContext = editViewModel,
                    Title = "选择区域",
                    Width = 880,
                    Height = 604,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    CanResize = false
                };
                editViewModel.SetViewReference(editWindow);

                var result = await editWindow.ShowDialog<bool>(parentWindow);
                if ((result))
                {
                    var currentItems = base.ItemsSource as IEnumerable<PlanAreaItemViewModel>;
                    var existingAreaIds = currentItems.Select(o => o.AreaID).ToHashSet();

                    foreach (var item in editViewModel.SelectedItems)
                    {
                        if (existingAreaIds.Contains(item.AreaID))
                            continue;

                        var prefix = "";
                        var existingNames = currentItems.Select(i => i.SerialNumber.ToString()).ToList();
                        int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);

                        var planAreaItemViewModel = new PlanAreaItemViewModel
                        {
                            AreaID = item.AreaID,
                            AreaName = item.AreaName,
                            SerialNumber = newSerialNumber
                        };

                        base.ItemsSource.Add(planAreaItemViewModel);

                        existingAreaIds.Add(item.AreaID);
                    }
                    base.IsSelectAll = false;
                }
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("错误", ex.Message, parentWindow);
            }
        }
      
        private void onAreaListItemChanged(object? sender, EventArgs e)
        {
            //同步更新：已删除的区域则同步删除
            var newAreas = CacheDataExchange.GetAreaListes();

            foreach (var item in base.ItemsSource.ToList())
            {
                //移除已删除的项
                if (!newAreas.Any(o => o.AreaID == item.AreaID))
                    base.ItemsSource.Remove(item);
            }
        }
    }

}