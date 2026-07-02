using GF_Gereric;
using GKG.ElectronicControl;

[assembly: Plugin(MotionControlDefAttribute.PLUGINKIND_Str, "{A9382E32-2729-4C0D-AAD2-67502D62B0C1}", "GaoChAuto MotionControl")]

namespace GKG.ElectronicControl.GaoChAuto
{
    [MotionControlDef("{653C8176-D382-46F0-959E-6871863D7F5E}")]
    internal class GaoChAutoMotionControlMain : GriffinsPluginMngClass, IMotionControlPlugin
    {

        #region IMotionControlPlugin 成员

        /// <summary>
        /// 运控卡名称
        /// </summary>
        string IMotionControlPlugin.MotionControlName => "GaoChAuto";

        string IMotionControlPlugin.DriverName => MotionControlDriverNames.GaoChAuto;

        /// <summary>
        /// 运控卡类型
        /// </summary>
        MotionCardType IMotionControlPlugin.MotionCardType => MotionCardType.GC800;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pluginFileName">插件文件路径</param>
        void IMotionControlPlugin.Init(string pluginFileName)
        {

        }
        MotionControlDriverDefaultConfig IMotionControlPlugin.GetDefaultConfig()
        {
            return new MotionControlDriverDefaultConfig(MotionControlCardType.TypeA, 8, 32, 32);
        }

        /// <summary>
        /// 创建运控卡接口实例
        /// </summary>
        /// <returns>运控卡接口实例</returns>
        IMotionControlBase IMotionControlPlugin.CreateMotionDriverObj()
        {
            return new MotionControlGaoChAuto();
        }
        #endregion
    }
}

