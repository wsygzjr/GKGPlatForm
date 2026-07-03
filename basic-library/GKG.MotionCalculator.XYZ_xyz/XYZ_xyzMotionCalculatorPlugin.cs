using GF_Gereric;

[assembly: Plugin(GKG.MotionControl.MotionCalculatorDefAttribute.PLUGINKIND_Str, "{FD181601-1789-4D8B-B038-770F9204A7C0}", "XYZ_xyz MotionCalculator Plugin")]

namespace GKG
{
    namespace MotionControl
    {
        [MotionCalculatorDef("{5E842375-C6B4-47A0-A591-5687D7045A6D}")]
        internal class XYZ_xyzMotionCalculatorPlugin : GriffinsPluginMngClass, IMotionCalculatorPlugin
        {
            string IMotionCalculatorPlugin.MotionCalculatorName => "XYZ_xyzMotionCalculator";

            string IMotionCalculatorPlugin.DriverName => MotionCalculatorDriverNames.XYZ_xyz;

            void IMotionCalculatorPlugin.Init(string pluginFileName)
            {
            }

            IMotionCalculatorDriver IMotionCalculatorPlugin.CreateMotionCalculatorDriverObj()
            {
                return new XYZ_xyzMotionCalculatorDriver();
            }
        }
    }
}
