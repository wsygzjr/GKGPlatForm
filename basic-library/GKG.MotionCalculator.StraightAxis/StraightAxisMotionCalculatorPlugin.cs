using GF_Gereric;

[assembly: Plugin(GKG.MotionControl.MotionCalculatorDefAttribute.PLUGINKIND_Str, "{7FB95B43-C89C-4363-9F8A-5EA144D67B2A}", "StraightAxis MotionCalculator Plugin")]

namespace GKG
{
    namespace MotionControl
    {
        [MotionCalculatorDef("{8B7E784A-4581-44DB-B733-B5BEFDD0AC17}")]
        internal class StraightAxisMotionCalculatorPlugin : GriffinsPluginMngClass, IMotionCalculatorPlugin
        {
            string IMotionCalculatorPlugin.MotionCalculatorName => "StraightAxisMotionCalculator";

            string IMotionCalculatorPlugin.DriverName => MotionCalculatorDriverNames.StraightAxis;

            void IMotionCalculatorPlugin.Init(string pluginFileName)
            {
            }

            IMotionCalculatorDriver IMotionCalculatorPlugin.CreateMotionCalculatorDriverObj()
            {
                return new StraightAxisMotionCalculatorDriver();
            }
        }
    }
}
