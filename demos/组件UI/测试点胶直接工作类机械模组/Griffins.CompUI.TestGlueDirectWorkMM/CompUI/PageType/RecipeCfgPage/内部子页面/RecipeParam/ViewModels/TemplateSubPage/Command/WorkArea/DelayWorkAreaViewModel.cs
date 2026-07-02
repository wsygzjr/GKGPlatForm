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
    /// 延时指令工作区-视图模型
    /// </summary>
    public class DelayWorkAreaViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 延时时间
        /// </summary>
        public NumericViewModel DelayTimeViewModel { get; }

        /// <summary>
        /// 延时时间
        /// </summary>
        public decimal DelayTime
        {
            get => DelayTimeViewModel.Value;
            set => DelayTimeViewModel.Value = value;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DelayWorkAreaViewModel()
        {
            DelayTimeViewModel = new NumericViewModel() { Increment = 1m, DecimalPlaces = 0, Minimum = 0.000m, Maximum = 500.000m, };

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="delayCcommandSequence"></param>
        public void CopyFrom(DelayCommandSequence delayCcommandSequence)
        {
            if (delayCcommandSequence == null)
                return;
          
            this.DelayTime = delayCcommandSequence.DelayTime;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="delayCcommandSequence"></param>
        public void CopyTo(DelayCommandSequence delayCcommandSequence)
        {
            if (delayCcommandSequence == null)
                return;
          
            delayCcommandSequence.DelayTime = (int)this.DelayTime;
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
            DelayTimeViewModel.ValueChanged += onValueChanged;
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