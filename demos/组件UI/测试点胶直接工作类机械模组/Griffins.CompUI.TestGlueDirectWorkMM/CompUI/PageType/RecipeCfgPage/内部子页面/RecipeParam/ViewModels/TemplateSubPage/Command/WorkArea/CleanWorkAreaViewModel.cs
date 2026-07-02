using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 清胶指令工作区-视图模型
    /// </summary>
    public class CleanWorkAreaViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        #region 响应式属性
        /// <summary>
        /// 排胶点数
        /// </summary>
        public NumericViewModel DispensingPointCountViewModel { get; }

        /// <summary>
        /// 排胶顺序下拉框
        /// </summary>
        public ComboxViewModel DispensingOrderViewModel { get; }

        /// <summary>
        /// 排胶点数
        /// </summary>
        public int DispensingPointCount
        {
            get => (int)DispensingPointCountViewModel.Value;
            set => DispensingPointCountViewModel.Value = value;
        }

        /// <summary>
        /// 排胶顺序
        /// </summary>
        public DispensingOrder DispensingOrder
        {
            get => (DispensingOrder)((DispensingOrderViewModel.SelectedItem as ComBoxItem)?.Value ?? DispensingOrder.FirstDispensing);
            set
            {
                if (DispensingOrderViewModel.ItemsSource != null)
                {
                    var targetItem = DispensingOrderViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (DispensingOrder)o.Value == value);
                    if (targetItem != null)
                        DispensingOrderViewModel.SelectedItem = targetItem;
                }
               
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public CleanWorkAreaViewModel()
        {
            // 初始化排胶点数（最小1）
            DispensingPointCountViewModel = new NumericViewModel
            {
                Increment = 1,
                DecimalPlaces = 0,
                Minimum = 1,
                Maximum = 100,
                Value = 1
            };

            // 初始化排胶顺序下拉框
            DispensingOrderViewModel = new ComboxViewModel();
            var orderItems = new Dictionary<DispensingOrder, string>
            {
                { DispensingOrder.FirstDispensing, "先排胶" }
            };
            DispensingOrderViewModel.ItemsSource = EnumExtensions.ToEnumItems(orderItems);
            DispensingOrderViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            DispensingOrderViewModel.SelectedItem = DispensingOrderViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="command"></param>
        public void CopyFrom(CleanCommandSequence command)
        {
            if (command == null) return;
            DispensingPointCount = command.DispensingPointCount;
            DispensingOrder = command.DispensingOrder;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="command"></param>
        public void CopyTo(CleanCommandSequence command)
        {
            if (command == null) return;
            command.DispensingPointCount = DispensingPointCount;
            command.DispensingOrder = DispensingOrder;
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
            DispensingPointCountViewModel.ValueChanged += onValueChanged;
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
        #endregion
    }
}