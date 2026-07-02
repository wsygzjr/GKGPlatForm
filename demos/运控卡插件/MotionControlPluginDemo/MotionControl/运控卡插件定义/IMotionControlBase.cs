
namespace MotionControl
{
    /// <summary>
    /// 运控卡基础接口
    /// </summary>
    public interface IMotionControlBase
    {
        /// <summary>
        /// 支持的轴数量
        /// </summary>
        int SupportAxisNum { get; }
    }
   
    /// <summary>
    /// A类运控接口
    /// </summary>
    /// <remarks>
    /// 该接口用于实现A类运动控制的相关功能
    /// </remarks>
    public interface IMotionControlCategoryA : IMotionControlBase
    {
        /// <summary>
        /// 在线变速
        /// </summary>
        /// <param name="axisGuid">轴锁定句柄</param>
        /// <param name="targetSpeed">目标速度</param>
        void OnlineSpeedChange(Guid axisGuid, double targetSpeed);
    }
}
