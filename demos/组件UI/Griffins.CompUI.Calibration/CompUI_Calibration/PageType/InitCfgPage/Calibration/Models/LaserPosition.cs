using GKG.UI.General;

namespace Griffins.CompUI.Calibration.Models
{

    /// <summary>
    ///激光位置信息
    /// </summary>
    public class LaserPosition : BasePositionInfo
    {
        /// <summary>
        ///激光高度值
        /// </summary>
        public decimal LaserHight { set; get; }

        public LaserPosition()
        {

        }
    }

}
