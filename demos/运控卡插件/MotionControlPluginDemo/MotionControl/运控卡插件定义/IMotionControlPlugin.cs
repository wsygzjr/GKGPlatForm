
namespace MotionControl
{
    /// <summary>
    /// 运控卡插件接口
    /// </summary>
    public interface IMotionControlPlugin
    {
        /// <summary>
        /// 运控卡名称
        /// </summary>
        string MotionControlName { get; }
        /// <summary>
        /// 运控卡类型
        /// </summary>
        MotionCardType MotionCardType { get; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pluginFileName">插件文件路径</param>
        void Init(string pluginFileName);
        /// <summary>
        /// 创建运控卡接口实例
        /// </summary>
        /// <returns></returns>
        IMotionControlBase CreateMotionCardObj();
    }

    /// <summary>
    /// 运控卡类型
    /// </summary>
    public enum MotionCardType
    {
        /// <summary>
        /// 雷赛1000B
        /// </summary>
        DMC1000B,

        /// <summary>
        /// 高川8轴卡
        /// </summary>
        GC800
    }
}
