using Avalonia.Controls;
using Griffins.UI;
using ReactiveUI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 子点胶指令工作区-视图模型
    /// </summary>
    public class SubDispensingWorkAreaViewModel : ReactiveObject
    {
        /// <summary>
        /// 所属模板ID
        /// </summary>
        private Guid _templateID;

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        #region 响应式属性
        /// <summary>
        /// 子模板下拉框
        /// </summary>
        public ComboxViewModel DispensingOrderViewModel { get; }


        /// <summary>
        /// 子模板ID
        /// </summary>
        public Guid SubTemplateID
        {
            get => (Guid)((DispensingOrderViewModel.SelectedItem as ComBoxItem)?.Value ?? Guid.Empty);
            set
            {
                if (DispensingOrderViewModel.ItemsSource != null)
                {
                    var targetItem = DispensingOrderViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (Guid)o.Value == value);
                    if (targetItem != null)
                        DispensingOrderViewModel.SelectedItem = targetItem;
                }
               
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public SubDispensingWorkAreaViewModel(Guid templateID)
        {
            this._templateID = templateID;
            // 初始化子模板下拉框
            DispensingOrderViewModel = new ComboxViewModel();

            //订阅子模板项改变事件
           CacheDataExchange.SubscribSubTemplateChanged(templateID, subTemplateConfigViewModel_SubTemplateItemChanged);

            DispensingOrderViewModel.ItemsSource = getSubTemplates();
            DispensingOrderViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            DispensingOrderViewModel.SelectedItem = DispensingOrderViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();

            // 订阅值变更事件
            subscribeValueChanges();

        }
        private void subTemplateConfigViewModel_SubTemplateItemChanged(object? sender, EventArgs e)
        {
            DispensingOrderViewModel.ItemsSource = getSubTemplates();
            DispensingOrderViewModel.SelectedItem = DispensingOrderViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault(o => (Guid)o.Value == SubTemplateID);
        }

        private List<ComBoxItem> getSubTemplates()
        {
            var subTemplatetems = CacheDataExchange.GetSubTemplatePointInfoes(_templateID).ToList();
            List<ComBoxItem> comBoxItems = new List<ComBoxItem>();
            foreach (var item in subTemplatetems)
            {
                comBoxItems.Add(new ComBoxItem()
                {
                    Value = item.SubTemplateID,
                    DisplayName = item.SubTemplateName
                });
            }
            return comBoxItems;
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="command"></param>
        public void CopyFrom(SubDispensingCommandSequence command)
        {
            if (command == null) return;
            SubTemplateID = command.SubTemplateID;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="command"></param>
        public void CopyTo(SubDispensingCommandSequence command)
        {
            if (command == null) return;
            command.SubTemplateID = SubTemplateID;
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            DispensingOrderViewModel.ValueChanged += onValueChanged;
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
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