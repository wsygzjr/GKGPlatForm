using Avalonia.Controls;
using Avalonia.VisualTree;
using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using Griffins.UI.General;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    ///区域列表视图模型
    /// </summary>
    public class AreaListViewModel :
        DataGridListBaseViewModel<AreaItemViewModel, AreaInfo>
    {

        /// <summary>
        ///区域项变更事件
        /// </summary>
        public event EventHandler<EventArgs>? AreaItemChanged;
        /// <summary>
        /// 查看示意图命令
        /// </summary>
        public ReactiveCommand<AreaItemViewModel, Unit> ViewSchematicDiagramCommand { get;  }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AreaListViewModel()
        {
            ViewSchematicDiagramCommand = ReactiveCommand.CreateFromTask<AreaItemViewModel>(onViewSchematicDiagram);

            this.ItemsSource.CollectionChanged += (s, e) =>
            {
                AreaItemChanged?.Invoke(this, new EventArgs());
            };
        }
        /// <summary>
        ///查看示意图
        /// </summary>
        private async Task onViewSchematicDiagram(AreaItemViewModel areaItemViewModel)
        {
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", _viewReference);
                return;
            }
            try
            {
                //获取模板实例
                List<DispensingTemplateInstanceExecutionObject> templateInstances = areaItemViewModel.GetCommandTemplateInstance();
                var viewSchematicDiagramWindowViewModel = new ViewSchematicDiagramWindowViewModel(templateInstances);
                var editWindow = new ViewSchematicDiagramWindow
                {
                    DataContext = viewSchematicDiagramWindowViewModel,
                    Title = "示意图",
                    Width = 1100,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                var result = await editWindow.ShowDialog<bool>(parentWindow);
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("错误", ex.Message, parentWindow);
            }

        }
        /// <summary>
        /// 新增Mark点
        /// </summary>
        protected override async Task _addItem()
        {
            var prefix = "区域";
            var existingNames = ItemsSource.Select(item => item.AreaName).ToList();
            int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);

            var areaItemViewModel = new AreaItemViewModel
            {
                AreaID = Guid.NewGuid(),
                SerialNumber = newSerialNumber,
                AreaName = $"{prefix}{newSerialNumber}",
            };
            areaItemViewModel.SetViewReference(_viewReference!);
            areaItemViewModel.AfterModified += onAfterModified;
            ItemsSource.Add(areaItemViewModel);
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