using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace GKG.UI.General
{
    /// <summary>
    /// 双控单限位-视图模型
    /// </summary>
    public class DoubleControlSingleLimitViewModel : ReactiveObject
    {
        private Control? _viewReference;

        /// <summary>
        /// 控制1-运控卡IO通道视图模型
        /// </summary>
        public HorizontalControlCardStateInitViewModel FirstControlViewModel { get; }

        /// <summary>
        /// 控制2-运控卡IO通道视图模型
        /// </summary>
        public HorizontalControlCardStateInitViewModel SecondControlViewModel { get; }

        /// <summary>
        /// 限位-运控卡IO通道框视图模型
        /// </summary>
        public HorizontalControlCardStateInitViewModel LimitViewModel { get; }

        /// <summary>
        /// 气缸超时时间视图模型
        /// </summary>
        public CylinderDelayViewModel CylinderDelayViewModel { get; }

        /// <summary>
        /// 值改变事件
        /// </summary>
        public event EventHandler? AfterModified;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DoubleControlSingleLimitViewModel()
        {
            FirstControlViewModel = new();
            SecondControlViewModel = new();
            LimitViewModel = new();
            CylinderDelayViewModel = new();

            subscribeValueChanges();
        }

        private void subscribeValueChanges()
        {
            FirstControlViewModel.AfterModified += (sender, e) => AfterModified?.Invoke(sender, e);
            SecondControlViewModel.AfterModified += (sender, e) => AfterModified?.Invoke(sender, e);
            LimitViewModel.AfterModified += (sender, e) => AfterModified?.Invoke(sender, e);
            CylinderDelayViewModel.AfterModified += (sender, e) => AfterModified?.Invoke(sender, e);
        }

        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        /// <summary>
        /// 设置视图引用（用于弹窗等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        public void SetIOChannelOptions(IEnumerable<string>? channelIDs)
        {
            FirstControlViewModel.SetChannelItems(channelIDs);
            SecondControlViewModel.SetChannelItems(channelIDs);
            LimitViewModel.SetChannelItems(channelIDs);
        }

        public void SetIOChannelOptions(IEnumerable<GKG.IOStateInformation>? ioStates)
        {
            FirstControlViewModel.SetChannelItems(ioStates);
            SecondControlViewModel.SetChannelItems(ioStates);
            LimitViewModel.SetChannelItems(ioStates);
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        public void CopyFrom(DoubleControlSingleLimitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.FirstControlModel ??= new();
            model.SecondControlModel ??= new();
            model.LimitModel ??= new();
            model.CylinderDelayModel ??= new();

            FirstControlViewModel.CopyFrom(model.FirstControlModel);
            SecondControlViewModel.CopyFrom(model.SecondControlModel);
            LimitViewModel.CopyFrom(model.LimitModel);
            CylinderDelayViewModel.CopyFrom(model.CylinderDelayModel);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(DoubleControlSingleLimitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.FirstControlModel ??= new();
            model.SecondControlModel ??= new();
            model.LimitModel ??= new();
            model.CylinderDelayModel ??= new();

            FirstControlViewModel.CopyTo(model.FirstControlModel);
            SecondControlViewModel.CopyTo(model.SecondControlModel);
            LimitViewModel.CopyTo(model.LimitModel);
            CylinderDelayViewModel.CopyTo(model.CylinderDelayModel);
        }
    }
}
