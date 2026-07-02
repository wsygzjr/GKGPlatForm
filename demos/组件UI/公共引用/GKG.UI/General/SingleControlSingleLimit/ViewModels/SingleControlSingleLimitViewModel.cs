using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace GKG.UI.General
{
    /// <summary>
    /// 单控单限位-视图模型
    /// </summary>
    public class SingleControlSingleLimitViewModel : ReactiveObject
    {
        private Control? _viewReference;

        /// <summary>
        /// 控制-运控卡IO通道视图模型
        /// </summary>
        public HorizontalControlCardStateInitViewModel ControlViewModel { get; }

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
        public SingleControlSingleLimitViewModel()
        {
            ControlViewModel = new();
            LimitViewModel = new();
            CylinderDelayViewModel = new();

            subscribeValueChanges();
        }

        /// <summary>
        /// 订阅值变更
        /// </summary>
        private void subscribeValueChanges()
        {
            ControlViewModel.AfterModified += (sender, e) => AfterModified?.Invoke(sender, e);
            LimitViewModel.AfterModified += (sender, e) => AfterModified?.Invoke(sender, e);
            CylinderDelayViewModel.AfterModified += (sender, e) => AfterModified?.Invoke(sender, e);
        }

        /// <summary>
        /// 值变更处理
        /// </summary>
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
            ControlViewModel.SetChannelItems(channelIDs);
            LimitViewModel.SetChannelItems(channelIDs);
        }

        public void SetIOChannelOptions(IEnumerable<GKG.IOStateInformation>? ioStates)
        {
            ControlViewModel.SetChannelItems(ioStates);
            LimitViewModel.SetChannelItems(ioStates);
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        public void CopyFrom(SingleControlSingleLimitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.ControlModel ??= new();
            model.LimitModel ??= new();
            model.CylinderDelayModel ??= new();

            ControlViewModel.SelectedControlCardID = model.ControlModel.ControlCardID;
            ControlViewModel.SelectedChannelID = model.ControlModel.channelID;
            LimitViewModel.SelectedControlCardID = model.LimitModel.ControlCardID;
            LimitViewModel.SelectedChannelID = model.LimitModel.channelID;
            CylinderDelayViewModel.CopyFrom(model.CylinderDelayModel);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(SingleControlSingleLimitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.ControlModel ??= new();
            model.LimitModel ??= new();
            model.CylinderDelayModel ??= new();

            model.ControlModel.ControlCardID = ControlViewModel.SelectedControlCardID;
            model.ControlModel.channelID = ControlViewModel.SelectedChannelID;
            model.LimitModel.ControlCardID = LimitViewModel.SelectedControlCardID;
            model.LimitModel.channelID = LimitViewModel.SelectedChannelID;
            CylinderDelayViewModel.CopyTo(model.CylinderDelayModel);
        }
    }
}
