using GF_Gereric;
using GKG.ElectronicControl;

[assembly: Plugin(MotionControlDefAttribute.PLUGINKIND_Str, "{9E091F5F-2F32-4140-80FC-8D585D4411DC}", "GMCMini MotionControl")]

namespace GKG.ElectronicControl.GMCMini
{
    [MotionControlDef("{DD10543A-DA2F-4BC2-BD0D-08FE11EC1568}")]
    internal class GMCMiniMotionControlMain : GriffinsPluginMngClass, IMotionControlPlugin
    {
        #region IMotionControlPlugin 成员

        /// <summary>
        /// 运控卡名称
        /// </summary>
        string IMotionControlPlugin.MotionControlName => "GMCMini";

        string IMotionControlPlugin.DriverName => MotionControlDriverNames.GMCMini;

        /// <summary>
        /// 运控卡类型
        /// </summary>
        MotionCardType IMotionControlPlugin.MotionCardType => MotionCardType.GMCMINI;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pluginFileName">插件文件路径</param>
        void IMotionControlPlugin.Init(string pluginFileName)
        {

        }
        MotionControlDriverDefaultConfig IMotionControlPlugin.GetDefaultConfig()
        {
            return new MotionControlDriverDefaultConfig(MotionControlCardType.Normal, 24, 255, 0);
        }

        /// <summary>
        /// 创建运控卡接口实例
        /// </summary>
        /// <returns>运控卡接口实例</returns>
        IMotionControlBase IMotionControlPlugin.CreateMotionDriverObj()
        {
            return new MotionControlGMCMini();
        }
        #endregion
    }
}

