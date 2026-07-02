using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace GKG.UI.General
{
    /// <summary>
    /// 单控双限位-视图模型
    /// </summary>
    public class SingleControlDoubleLimitViewModel : ReactiveObject
    {
        private Control? _viewReference;

        /// <summary>
        /// 控制-运控卡IO通道视图模型
        /// </summary>
        public HorizontalControlCardStateInitViewModel ControlViewModel { get; }

        /// <summary>
        /// 限位1-运控卡IO通道框视图模型
        /// </summary>
        public HorizontalControlCardStateInitViewModel FirstLimitViewModel { get; }

        /// <summary>
        /// 限位2-运控卡IO通道框视图模型
        /// </summary>
        public HorizontalControlCardStateInitViewModel SecondLimitViewModel { get; }

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
        public SingleControlDoubleLimitViewModel()
        {
            ControlViewModel = new();
            FirstLimitViewModel = new();
            SecondLimitViewModel = new();
            CylinderDelayViewModel = new();

            subscribeValueChanges();
        }

        /// <summary>
        /// 订阅值变更
        /// </summary>
        private void subscribeValueChanges()
        {
            ControlViewModel.AfterModified += (sender, e) => AfterModified?.Invoke(sender, e);
            FirstLimitViewModel.AfterModified += (sender, e) => AfterModified?.Invoke(sender, e);
            SecondLimitViewModel.AfterModified += (sender, e) => AfterModified?.Invoke(sender, e);
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
            FirstLimitViewModel.SetChannelItems(channelIDs);
            SecondLimitViewModel.SetChannelItems(channelIDs);
        }

        public void SetIOChannelOptions(IEnumerable<GKG.IOStateInformation>? ioStates)
        {
            ControlViewModel.SetChannelItems(ioStates);
            FirstLimitViewModel.SetChannelItems(ioStates);
            SecondLimitViewModel.SetChannelItems(ioStates);
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        public void CopyFrom(SingleControlDoubleLimitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.ControlModel ??= new();
            model.FirstLimitModel ??= new();
            model.SecondLimitModel ??= new();
            model.CylinderDelayModel ??= new();

            ControlViewModel.SelectedControlCardID = model.ControlModel.ControlCardID;
            ControlViewModel.SelectedChannelID = model.ControlModel.channelID;
            FirstLimitViewModel.SelectedControlCardID = model.FirstLimitModel.ControlCardID;
            FirstLimitViewModel.SelectedChannelID = model.FirstLimitModel.channelID;
            SecondLimitViewModel.SelectedControlCardID = model.SecondLimitModel.ControlCardID;
            SecondLimitViewModel.SelectedChannelID = model.SecondLimitModel.channelID;
            CylinderDelayViewModel.CopyFrom(model.CylinderDelayModel);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(SingleControlDoubleLimitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.ControlModel ??= new();
            model.FirstLimitModel ??= new();
            model.SecondLimitModel ??= new();
            model.CylinderDelayModel ??= new();

            model.ControlModel.ControlCardID = ControlViewModel.SelectedControlCardID;
            model.ControlModel.channelID = ControlViewModel.SelectedChannelID;
            model.FirstLimitModel.ControlCardID = FirstLimitViewModel.SelectedControlCardID;
            model.FirstLimitModel.channelID = FirstLimitViewModel.SelectedChannelID;
            model.SecondLimitModel.ControlCardID = SecondLimitViewModel.SelectedControlCardID;
            model.SecondLimitModel.channelID = SecondLimitViewModel.SelectedChannelID;
            CylinderDelayViewModel.CopyTo(model.CylinderDelayModel);
        }
    }
}
