using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;
using Griffins.UI.General;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Models
{
    /// <summary>
    /// <summary>
    ///0相机位置
    /// </summary>
    public class ZeroCameraPosition : BasePositionInfo
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

        public ZeroCameraPosition()
        {

        }
    }

}
