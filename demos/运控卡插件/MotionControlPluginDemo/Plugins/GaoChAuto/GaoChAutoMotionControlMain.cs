using GF_Gereric;
using MotionControl;

[assembly: Plugin(MotionControlDefAttribute.PLUGINKIND_Str, "{A9382E32-2729-4C0D-AAD2-67502D62B0C1}", "GaoChAuto MotionControl")]

namespace GKG.GaoChAuto.MotionControl
{
	[MotionControlDef("{653C8176-D382-46F0-959E-6871863D7F5E}")]
	internal class GaoChAutoMotionControlMain : GriffinsPluginMngClass, IMotionControlPlugin
    {

        #region IMotionControlPlugin 成员

        /// <summary>
        /// 运控卡名称
        /// </summary>
        string IMotionControlPlugin.MotionControlName => ResourceA.MotionControlName;
       
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
        /// <summary>
        /// 创建运控卡接口实例
        /// </summary>
        /// <returns>运控卡接口实例</returns>
        IMotionControlBase IMotionControlPlugin.CreateMotionCardObj()
        {
            return new GaoChAutoMotionControl();
        }
        #endregion
    }
}
