using Avalonia.Controls;
using ReactiveUI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using Griffins.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 抬针工作区-视图模型
    /// </summary>
    public class LiftNeedleWorkAreaViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 抬针高度
        /// </summary>
        public NumericViewModel HeightViewModel { get; }

        /// <summary>
        /// 抬针高度
        /// </summary>
        public decimal Height
        {
            get => HeightViewModel.Value;
            set => HeightViewModel.Value = value;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public LiftNeedleWorkAreaViewModel()
        {
            HeightViewModel = new NumericViewModel() { Increment = 0.001m, DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="liftNeedleCcommandSequence"></param>
        public void CopyFrom(LiftNeedleCommandSequence liftNeedleCcommandSequence)
        {
            if (liftNeedleCcommandSequence == null)
                return;
          
            this.Height = liftNeedleCcommandSequence.Height;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="liftNeedleCcommandSequence"></param>
        public void CopyTo(LiftNeedleCommandSequence liftNeedleCcommandSequence)
        {
            if (liftNeedleCcommandSequence == null)
                return;
          
            liftNeedleCcommandSequence.Height = (int)this.Height;
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
            HeightViewModel.ValueChanged += onValueChanged;
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