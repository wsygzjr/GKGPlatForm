using ReactiveUI;
using System;
using System.Reactive;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel
{
    public enum OneKeyType
    {
        Null, OutGlue, OutWax, Laser_LeftValve, Laser_RightValve,
        MachineCalibration_LeftValve, MachineCalibration_RightValve
    }

    public enum CountDownType
    {
        Null, CleanGlue_LeftValve, CleanGlue_RightValve, Purge_LeftValve,
        Purge_RightValve, Weight_LeftValve, Weight_RightValve,
        AddGlue_LeftValve, AddGlue_RightValve
    }

    public class TipShowWindowViewModel : ReactiveObject
    {
        // 绑定到界面的提示文字
        public string Message { get; }

        // 绑定到取消按钮的可见性
        public bool IsCancelVisible { get; }

        // 关闭/取消命令
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        // 构造函数 1：处理 OneKeyType
        public TipShowWindowViewModel(OneKeyType keyType, Action onCancelAction = null)
        {
            // 1. 决定按钮是否可见
            IsCancelVisible = !(keyType == OneKeyType.Laser_LeftValve ||
                                keyType == OneKeyType.Laser_RightValve ||
                                keyType == OneKeyType.MachineCalibration_LeftValve ||
                                keyType == OneKeyType.MachineCalibration_RightValve);

            // 2. 决定显示的文字
            Message = keyType switch
            {
                OneKeyType.OutGlue => "一键排胶正在进行中...",
                OneKeyType.OutWax => "一键排蜡正在进行中...",
                OneKeyType.Laser_LeftValve => "左阀正在测高中...",
                OneKeyType.Laser_RightValve => "右阀正在测高中...",
                OneKeyType.MachineCalibration_LeftValve => "左阀正在精准校正中...",
                OneKeyType.MachineCalibration_RightValve => "右阀正在精准校正中...",
                _ => "处理中..."
            };

            // 3. 封装取消命令
            CancelCommand = ReactiveCommand.Create(() =>
            {
                // 如果外部传入了专门的硬件取消逻辑，就执行它
                onCancelAction?.Invoke();
            });
        }

        // 构造函数 2：处理 CountDownType
        public TipShowWindowViewModel(CountDownType countDownType, Action onCancelAction = null)
        {
            IsCancelVisible = !(countDownType == CountDownType.AddGlue_LeftValve ||
                                countDownType == CountDownType.AddGlue_RightValve);

            Message = "倒计时进行中...";

            CancelCommand = ReactiveCommand.Create(() =>
            {
                onCancelAction?.Invoke();
            });
        }
    }
}