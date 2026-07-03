using GF_Gereric;

[assembly: Plugin(GKG.MotionControl.MotionCalculatorDefAttribute.PLUGINKIND_Str, "{B47F298C-18A3-4E16-BEF1-69E14C54A638}", "Plane MotionCalculator Plugin")]

namespace GKG
{
    namespace MotionControl
    {
        [MotionCalculatorDef("{0E5A1C8B-21B0-4302-A22E-6B1C57A0E6F4}")]
        internal class PlaneMotionCalculatorPlugin : GriffinsPluginMngClass, IMotionCalculatorPlugin
        {
            string IMotionCalculatorPlugin.MotionCalculatorName => "PlaneMotionCalculator";

            string IMotionCalculatorPlugin.DriverName => MotionCalculatorDriverNames.Plane;

            void IMotionCalculatorPlugin.Init(string pluginFileName)
            {
            }

            IMotionCalculatorDriver IMotionCalculatorPlugin.CreateMotionCalculatorDriverObj()
            {
                return new PlaneMotionCalculatorDriver();
            }
        }
    }
}
