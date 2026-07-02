using GKG.UI.General;

namespace Griffins.CompUI.Calibration.Models
{
    /// <summary>
    ///相机Vs胶阀公共参数配置
    /// </summary>
    public class CameraVsGluevalveComInfo
    {
        /// <summary>
        ///校正提醒（H）
        /// </summary>
        public int CorrectionReminder { set; get; }
        /// <summary>
        ///校正提醒倒计时
        ///显示为机器时间
        /// </summary>
        public string CorrectionReminderCountdown { set; get; } = "";
        /// <summary>
        ///停机提醒（H）
        /// </summary>
        public int StopMachineReminder { set; get; }

        public CameraVsGluevalveComInfo()
        {

        }
    }

}
