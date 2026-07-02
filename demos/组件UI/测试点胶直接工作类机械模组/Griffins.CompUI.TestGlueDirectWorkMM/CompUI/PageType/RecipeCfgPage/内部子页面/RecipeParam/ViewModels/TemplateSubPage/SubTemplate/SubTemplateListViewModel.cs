using Avalonia.Controls;
using Avalonia.VisualTree;
using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using Griffins.UI.General;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.SubTemplate;
using ReactiveUI;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.SubTemplate
{
    /// <summary>
    ///子模板列表视图模型
    /// </summary>
    public class SubTemplateListViewModel :
        DataGridListBaseViewModel<SubTemplateItemViewModel, SubTemplateItemInfo>
    {
        /// <summary>
        /// 所属模板ID
        /// </summary>
        private Guid _templateID;
        /// <summary>
        ///子模板项变更事件
        /// </summary>
        public event EventHandler<EventArgs>? SubTemplateItemChanged;
        /// <summary>
        /// 设置所属模板ID
        /// </summary>
        /// <param name="templateID"></param>
        public void SetTemplateID(Guid templateID)
        {
            this._templateID = templateID;
        }

        /// <summary>
        ///  点位设置栏指定为显示的属性（绑定界面）
        /// </summary>
        private bool _isVisibleBySelectedItem;
        public bool IsVisibleBySelectedItem
        {
            get => _isVisibleBySelectedItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _isVisibleBySelectedItem, value);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SubTemplateListViewModel()
        {  
            //  子模板列表选中行时，将点位设置栏指定为显示
            this.WhenAnyValue(vm => vm.SelectedItem)
            .Subscribe(selectedItem =>
            {
                if (selectedItem != null)
                    IsVisibleBySelectedItem = true;
                else 
                    IsVisibleBySelectedItem = false;
            });

            this.ItemsSource.CollectionChanged += (s, e) =>
            {
                SubTemplateItemChanged?.Invoke(this, new EventArgs());
            };
        }

        /// <summary>
        /// 新增子模板
        /// </summary>
        protected override async Task _addItem()
        {
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", _viewReference);
                return;
            }

            var editViewModel = new SubTemplateItemEditWindowViewModel(_templateID);
            var editWindow = new SubTemplateItemEditWindow
            {
                DataContext = editViewModel,
                Title = "选择模板",
                Width = 470,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                CanResize = false
            };

            var result = await editWindow.ShowDialog<bool>(parentWindow);
            if ((result))
            {
                var prefix = "子模板";
                var existingNames = ItemsSource.Select(item => item.SubTemplateName).ToList();
                int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);
                var subTemplateItemViewModel = new SubTemplateItemViewModel
                {
                    SubTemplateID = Guid.NewGuid(),
                    SubTemplateName = $"{prefix}{newSerialNumber}",
                    SerialNumber = newSerialNumber,
                    BelongTemplateID = editViewModel.BelongTemplateID
                };
                subTemplateItemViewModel.SetViewReference(_viewReference!);
                ItemsSource.Add(subTemplateItemViewModel);
            }
        }
      
    }

}