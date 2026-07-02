using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.CustomEdit;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 模板实例工作区视图模型
    /// </summary>
    public class TemplateInstanceWorkAreaViewModel: ReactiveObject
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        /// <summary>
        /// 执行对象实例ID
        /// </summary>
        private Guid _instanceId { get; set; }
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 是否启用-数据模型
        /// </summary>
        public ToggleSwitchViewModel IsEnableViewModel { get; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable
        {
            get => IsEnableViewModel.IsChecked;
            set
            {
                IsEnableViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(IsEnable));
            }
        }
        /// <summary>
        /// 模板实例工作区视图模型
        /// </summary>
        public DispensingCommandTemplateInstanceViewModel DispensingCommandTemplateInstanceViewModel { get; }
        public TemplateInstanceWorkAreaViewModel()
        {
            IsEnableViewModel = new ToggleSwitchViewModel { IsChecked = true };
            DispensingCommandTemplateInstanceViewModel=new DispensingCommandTemplateInstanceViewModel();
            // 订阅值变更事件
            subscribeValueChanges();
        }
        /// <summary>
        /// 从数据模型复制
        /// </summary>
        public  void CopyFrom(DispensingTemplateInstanceExecutionObject info)
        {
            this._instanceId = info.InstanceId;
            this.IsEnable = info.IsEnabled;
            DispensingCommandTemplateInstanceViewModel.CopyFrom(info.TemplateInstance);
        }

        /// <summary>
        /// 复制到数据模型
        /// </summary>
        public  void CopyTo(DispensingTemplateInstanceExecutionObject targetInfo)
        {
            targetInfo.InstanceId = this._instanceId;
            targetInfo.IsEnabled=this.IsEnable;

            DispensingCommandTemplateInstanceViewModel.CopyTo(targetInfo.TemplateInstance);

        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
            DispensingCommandTemplateInstanceViewModel.SetViewReference(view);

        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            DispensingCommandTemplateInstanceViewModel.AfterModified += onAfterModified;
           
            IsEnableViewModel.ValueChanged += onValueChanged;
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
