using GF_Gereric;
using GKG.ElectronicControl;

[assembly: Plugin(MotionControlDefAttribute.PLUGINKIND_Str, "{EC072347-C4BE-4207-8D89-BC15EB6F0D1B}", "Zmotion MotionControl")]

namespace GKG.ElectronicControl.Zmotion
{
    /// <summary>
    /// 正运动 ZMC 运控卡插件入口类
    /// 负责注册驱动名称、默认配置并创建 MotionControlZmc 实例
    /// </summary>
    [MotionControlDef("{7B11EC9F-A59F-4CA0-9AA3-B60AD1FB6C6C}")]
    internal class ZmotionMotionControlMain : GriffinsPluginMngClass, IMotionControlPlugin
    {
        /// <summary>
        /// 运控卡显示名称
        /// </summary>
        string IMotionControlPlugin.MotionControlName => "Zmotion";

        /// <summary>
        /// 驱动标识名
        /// </summary>
        string IMotionControlPlugin.DriverName => MotionControlDriverNames.Zmotion;

        /// <summary>
        /// 运控卡硬件类型
        /// </summary>
        MotionCardType IMotionControlPlugin.MotionCardType => MotionCardType.Zmotion;

        /// <summary>
        /// 插件初始化(预留)
        /// </summary>
        void IMotionControlPlugin.Init(string pluginFileName)
        {
        }

        /// <summary>
        /// 获取默认配置: TypeA 类型, 8轴, 32路IO, 0路模拟量
        /// </summary>
        MotionControlDriverDefaultConfig IMotionControlPlugin.GetDefaultConfig()
        {
            return new MotionControlDriverDefaultConfig(MotionControlCardType.TypeA, 8, 32, 0);
        }

        /// <summary>
        /// 创建 ZMC 运控卡驱动实例
        /// </summary>
        IMotionControlBase IMotionControlPlugin.CreateMotionDriverObj()
        {
            return new MotionControlZmc();
        }
    }
}