using GF_Gereric;
using GKG.MotionControl;

[assembly: Plugin(GKG.MotionControl.RobotDefAttribute.PLUGINKIND_Str, "{36490566-7047-44B9-93D8-12B2C22E750D}", "CategoryA Robot Plugin")]

namespace GKG.Robot.CategoryA
{
    [RobotDef("{D809E61B-2F31-44F0-82D3-4FD29D4FAF32}")]
    internal class CategoryARobotPlugin : GriffinsPluginMngClass, IRobotPlugin
    {
        string IRobotPlugin.RobotName => "CategoryARobot";

        string IRobotPlugin.DriverName => RobotDriverNames.CategoryARobot;

        void IRobotPlugin.Init(string pluginFileName)
        {
        }

        IRobotDriver IRobotPlugin.CreateRobotDriverObj()
        {
            return new CategoryARobot();
        }
    }
}
