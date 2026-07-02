using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace GKG.UI.General
{
    /// <summary>
    /// 双控双限位-视图模型
    /// </summary>
    public class DoubleControlDoubleLimitViewModel : ReactiveObject
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
        public DoubleControlDoubleLimitViewModel()
        {
            FirstControlViewModel = new();
            SecondControlViewModel = new();
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
            FirstControlViewModel.AfterModified += (sender, e) => AfterModified?.Invoke(sender, e);
            SecondControlViewModel.AfterModified += (sender, e) => AfterModified?.Invoke(sender, e);
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
            FirstControlViewModel.SetChannelItems(channelIDs);
            SecondControlViewModel.SetChannelItems(channelIDs);
            FirstLimitViewModel.SetChannelItems(channelIDs);
            SecondLimitViewModel.SetChannelItems(channelIDs);
        }

        public void SetIOChannelOptions(IEnumerable<GKG.IOStateInformation>? ioStates)
        {
            FirstControlViewModel.SetChannelItems(ioStates);
            SecondControlViewModel.SetChannelItems(ioStates);
            FirstLimitViewModel.SetChannelItems(ioStates);
            SecondLimitViewModel.SetChannelItems(ioStates);
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        public void CopyFrom(DoubleControlDoubleLimitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.FirstControlModel ??= new();
            model.SecondControlModel ??= new();
            model.FirstLimitModel ??= new();
            model.SecondLimitModel ??= new();
            model.CylinderDelayModel ??= new();

            FirstControlViewModel.SelectedControlCardID = model.FirstControlModel.ControlCardID;
            FirstControlViewModel.SelectedChannelID = model.FirstControlModel.channelID;
            SecondControlViewModel.SelectedControlCardID = model.SecondControlModel.ControlCardID;
            SecondControlViewModel.SelectedChannelID = model.SecondControlModel.channelID;
            FirstLimitViewModel.SelectedControlCardID = model.FirstLimitModel.ControlCardID;
            FirstLimitViewModel.SelectedChannelID = model.FirstLimitModel.channelID;
            SecondLimitViewModel.SelectedControlCardID = model.SecondLimitModel.ControlCardID;
            SecondLimitViewModel.SelectedChannelID = model.SecondLimitModel.channelID;
            CylinderDelayViewModel.CopyFrom(model.CylinderDelayModel);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(DoubleControlDoubleLimitCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.FirstControlModel ??= new();
            model.SecondControlModel ??= new();
            model.FirstLimitModel ??= new();
            model.SecondLimitModel ??= new();
            model.CylinderDelayModel ??= new();

            model.FirstControlModel.ControlCardID = FirstControlViewModel.SelectedControlCardID;
            model.FirstControlModel.channelID = FirstControlViewModel.SelectedChannelID;
            model.SecondControlModel.ControlCardID = SecondControlViewModel.SelectedControlCardID;
            model.SecondControlModel.channelID = SecondControlViewModel.SelectedChannelID;
            model.FirstLimitModel.ControlCardID = FirstLimitViewModel.SelectedControlCardID;
            model.FirstLimitModel.channelID = FirstLimitViewModel.SelectedChannelID;
            model.SecondLimitModel.ControlCardID = SecondLimitViewModel.SelectedControlCardID;
            model.SecondLimitModel.channelID = SecondLimitViewModel.SelectedChannelID;
            CylinderDelayViewModel.CopyTo(model.CylinderDelayModel);
        }
    }
}
