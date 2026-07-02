using DynamicData;
using Griffins.UI;
using ReactiveUI;
using System.Reactive;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.SubTemplate
{
    /// <summary>
    /// 添加或编辑子模板-视图模型
    /// </summary>
    public class SubTemplateItemEditWindowViewModel : ReactiveObject
    {
        private bool? _dialogResult;
       
        /// <summary>
        /// 选中的模板ID-下拉框数据模型
        /// </summary>
        public ComboxViewModel SelectedTemplateModel { get; }
        /// <summary>
        /// 选中的模板ID
        /// </summary>
        public Guid BelongTemplateID
        {
            get
            {
                if (SelectedTemplateModel.SelectedItem is ComBoxItem template)
                {
                    return (Guid)template.Value;
                }
                return Guid.Empty;
            }
            set
            {
                if (SelectedTemplateModel.ItemsSource != null)
                {
                    var targetItem = SelectedTemplateModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (Guid)o.Value == value);
                    if (targetItem != null)
                        SelectedTemplateModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(BelongTemplateID));

                }
            }
        }
        /// <summary>
        /// 对话框结果（true:保存，false:取消，null:未操作）
        /// </summary>
        public bool? DialogResult
        {
            get => _dialogResult;
            set => this.RaiseAndSetIfChanged(ref _dialogResult, value);
        }
        /// <summary>
        /// 保存命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        /// <summary>
        /// 取消命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SubTemplateItemEditWindowViewModel(Guid templateID)
        {
            SaveCommand = ReactiveCommand.Create(save);
            CancelCommand = ReactiveCommand.Create(cancel);

            // 初始化模板数据源
            var templateItems = CacheDataExchange.TemplateCfgViewModels.ToList();
            //不能将当前模板添加为自己的子模板
            templateItems.RemoveAll(o=>o.TemplateID==templateID);
            //去掉那些把当前模板当做子模板的(父)模板，避免循环嵌套
            //父模板列表（待移除项）
            var fartherTemplateIDs = new List<Guid>();
            getFatherTemplate(templateItems, fartherTemplateIDs, templateID);
            templateItems.RemoveAll(o => fartherTemplateIDs.Contains(o.TemplateID));

            List<ComBoxItem> comBoxItems = new List<ComBoxItem>();
            foreach (var item in templateItems)
            {
                comBoxItems.Add(new ComBoxItem()
                {
                    Value = item.TemplateID,
                    DisplayName = item.TemplateName
                });
            }
            SelectedTemplateModel = new ComboxViewModel();
            SelectedTemplateModel.ItemsSource = comBoxItems;
            SelectedTemplateModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            SelectedTemplateModel.SelectedItem = SelectedTemplateModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();
            SelectedTemplateModel.ValueChanged += (s, e) => this.RaisePropertyChanged(nameof(BelongTemplateID));
        }

        /// <summary>
        /// 递归查找那些把当前模板当做子模板的(父)模板
        /// </summary>
        /// <param name="templateItems">模板集合</param>
        /// <param name="fartherTemplateIDs">(父)模板id列表</param>
        /// <param name="curTemplateID">当前模板</param>
        private void getFatherTemplate(List<TemplateCfgViewModel> templateItems, List<Guid> fartherTemplateIDs,Guid curTemplateID)
        {
            foreach (var item in templateItems)
            {
                if (item.SubTemplateConfigViewModel.SubTemplateListViewModel.ItemsSource.ToList()
                    .Any(o => o.BelongTemplateID == curTemplateID))
                {
                    fartherTemplateIDs.Add(item.TemplateID);
                    getFatherTemplate(templateItems, fartherTemplateIDs, item.TemplateID);
                }
            }
        }
        /// <summary>
        /// /保存逻辑
        /// </summary>
        private void save()
        {
            DialogResult = true;
        }

        /// <summary>
        /// 取消逻辑
        /// </summary>
        private void cancel()
        {
            DialogResult = false;
        }
        
    }
}
