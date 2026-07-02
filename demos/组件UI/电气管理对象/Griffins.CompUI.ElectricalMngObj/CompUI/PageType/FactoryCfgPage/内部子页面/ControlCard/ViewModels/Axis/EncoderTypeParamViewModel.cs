using Griffins.UI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using ReactiveUI;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels.Axis
{
    /// <summary>
    /// 编码器类型参数配置-视图模型
    /// </summary>
    public class EncoderTypeParamViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 编码器类型-下拉框数据模型
        /// </summary>
        public ComboxViewModel EncoderTypeViewModel { get; }

        /// <summary>
        /// 选中的编码器类型
        /// </summary>
        public EncoderType SelectedEncoderType
        {
          
            get => (EncoderType)((EncoderTypeViewModel.SelectedItem as ComBoxItem)?.Value ?? EncoderType.NotOutEncoder);
            set
            {
                if (EncoderTypeViewModel.ItemsSource != null)
                {
                    var targetItem = EncoderTypeViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (EncoderType)o.Value == value);
                    if (targetItem != null)
                        EncoderTypeViewModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedEncoderType));
                }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public EncoderTypeParamViewModel()
        {
            var reactionDisplayNames = new Dictionary<EncoderType, string>
            {
                { EncoderType.NotOutEncoder, "无外部编码器" },
                { EncoderType.HasOutEncoder, "有外部编码器" }
            };
            EncoderTypeViewModel = new ComboxViewModel();
            EncoderTypeViewModel.ItemsSource = EnumExtensions.ToEnumItems<EncoderType>(reactionDisplayNames);
            EncoderTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            // 订阅值变更事件
            subscribeValueChanges();
        }
        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="encoderTypeParamInfo"></param>
        public void CopyFrom(EncoderTypeParamInfo encoderTypeParamInfo)
        {
            this.SelectedEncoderType = encoderTypeParamInfo.EncoderType;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="encoderTypeParamInfo"></param>
        public void CopyTo(EncoderTypeParamInfo encoderTypeParamInfo)
        {
            encoderTypeParamInfo.EncoderType = this.SelectedEncoderType;

        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            EncoderTypeViewModel.ValueChanged += onValueChanged;
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