using MotionControl;

namespace GKG.GaoChAuto.MotionControl
{
    /// <summary>
    /// 高川运控卡对象实例
    /// </summary>
	internal class GaoChAutoMotionControl : IMotionControlBase, IMotionControlCategoryA
    {

        #region IMotionControlBase 成员

        /// <summary>
        /// 支持的轴数量
        /// </summary>
        int IMotionControlBase.SupportAxisNum => 1;

        #endregion

        #region IMotionControlCategoryA 成员

        /// <summary>
        /// 在线变速
        /// </summary>
        /// <param name="axisGuid">轴锁定句柄</param>
        /// <param name="targetSpeed">目标速度</param>
        void IMotionControlCategoryA.OnlineSpeedChange(Guid axisGuid, double targetSpeed)
        {

        }

        #endregion
    }
}
