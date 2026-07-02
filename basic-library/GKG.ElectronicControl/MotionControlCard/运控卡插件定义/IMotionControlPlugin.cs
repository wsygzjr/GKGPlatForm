using System;

namespace GKG.ElectronicControl
{
    /// <summary>
    /// 运动控制插件接口
    /// </summary>
    public interface IMotionControlPlugin
    {
        /// <summary>
        /// 运动控制插件名称
        /// </summary>
        string MotionControlName { get; }

        /// <summary>
        /// 驱动名称
        /// </summary>
        string DriverName { get; }

        /// <summary>
        /// 运动控制卡类型。
        /// 兼容旧调用链保留该字段，新的调用建议优先使用 DriverName。
        /// </summary>
        MotionCardType MotionCardType { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pluginFileName">插件文件路径</param>
        void Init(string pluginFileName);

        /// <summary>
        /// 获取驱动默认配置。
        /// 用于在不初始化硬件的情况下，获取轴数量、状态量数量、模拟量数量等基础能力。
        /// </summary>
        MotionControlDriverDefaultConfig GetDefaultConfig();

        /// <summary>
        /// 创建运动控制驱动对象
        /// </summary>
        IMotionControlBase CreateMotionDriverObj();

        /// <summary>
        /// 创建运动控制卡对象。
        /// 兼容旧接口，默认转发到 CreateMotionDriverObj。
        /// </summary>
        IMotionControlBase CreateMotionCardObj()
        {
            return CreateMotionDriverObj();
        }
    }
}
