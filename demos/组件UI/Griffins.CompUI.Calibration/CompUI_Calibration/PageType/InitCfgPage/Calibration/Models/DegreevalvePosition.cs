using GKG.UI.General;

namespace Griffins.CompUI.Calibration.Models
{
    /// <summary>
    ///胶阀位置信息
    /// </summary>
    public class DegreevalvePosition : BasePositionInfo
    {
        /// <summary>
        ///出胶点数
        /// </summary>
        public int NumberOfDispensingPoints { set; get; }

        public DegreevalvePosition()
        {

        }
    }

}
