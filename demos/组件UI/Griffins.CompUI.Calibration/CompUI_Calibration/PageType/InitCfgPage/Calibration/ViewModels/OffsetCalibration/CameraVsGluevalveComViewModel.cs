using Avalonia.Controls;
using GKG.UI;
using Griffins.CompUI.Calibration.Models;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace Griffins.CompUI.Calibration.ViewModels
{

    /// <summary>
    /// 相机Vs胶阀公共参数配置 ViewModel
    /// </summary>
    public class CameraVsGluevalveComViewModel : ReactiveObject
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        /// <summary>
        /// 校正提醒（H）数据模型（int类型，用NumericViewModel承载）
        /// </summary>
        public NumericViewModel CorrectionReminderViewModel { get; }

        /// <summary>
        /// 停机提醒（H）数据模型（int类型）
        /// </summary>
        public NumericViewModel StopMachineReminderViewModel { get; }

        /// <summary>
        /// 校正提醒倒计时 数据模型（字符串类型）
        /// </summary>
        public TextInputViewModel CorrectionReminderCountdownViewModel { get; }

        /// <summary>
        /// 校正提醒（H）
        /// </summary>
        public int CorrectionReminder
        {
            get => (int)CorrectionReminderViewModel.Value;
            set => CorrectionReminderViewModel.Value = value;
        }

        /// <summary>
        /// 校正提醒倒计时
        /// </summary>
        public string CorrectionReminderCountdown
        {
            get => CorrectionReminderCountdownViewModel.Text;
            set => CorrectionReminderCountdownViewModel.Text = value;
        }

        /// <summary>
        /// 停机提醒（H）
        /// </summary>
        public int StopMachineReminder
        {
            get => (int)StopMachineReminderViewModel.Value;
            set => StopMachineReminderViewModel.Value = value;
        }

        /// <summary>
        /// 重置命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ResetCommand { get; }

        public CameraVsGluevalveComViewModel()
        {
            CorrectionReminderViewModel = new NumericViewModel
            {
                Minimum = 0,
                Increment = 1,
                Value = 0
            };
            StopMachineReminderViewModel = new NumericViewModel
            {
                Minimum = 0,
                Increment = 1,
                Value = 0
            };
            CorrectionReminderCountdownViewModel = new TextInputViewModel();

            ResetCommand = ReactiveCommand.CreateFromTask(onReset);
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }
        public void CopyFrom(CameraVsGluevalveComInfo info)
        {
            if (info == null) return;
            CorrectionReminder = info.CorrectionReminder;
            CorrectionReminderCountdown = info.CorrectionReminderCountdown;
            StopMachineReminder = info.StopMachineReminder;
        }

        public void CopyTo(CameraVsGluevalveComInfo info)
        {
            if (info == null) return;
            info.CorrectionReminder = CorrectionReminder;
            info.CorrectionReminderCountdown = CorrectionReminderCountdown;
            info.StopMachineReminder = StopMachineReminder;
        }

        /// <summary>
        ///重置
        /// </summary>
        private async Task onReset()
        {
            CorrectionReminder = 0;
            CorrectionReminderCountdown = "";
            StopMachineReminder = 0;
            await Task.Delay(100);
        }
    }
}
