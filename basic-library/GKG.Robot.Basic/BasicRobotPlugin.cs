using GF_Gereric;
using GKG.MotionControl;

[assembly: Plugin(GKG.MotionControl.RobotDefAttribute.PLUGINKIND_Str, "{445AA40E-2D5C-4B99-8343-43D8F93CEB75}", "Basic Robot Plugin")]

namespace GKG.Robot.Basic
{
    [RobotDef("{02E6AA8B-7D6E-4807-9DF9-0A83BCF833A4}")]
    internal class BasicRobotPlugin : GriffinsPluginMngClass, IRobotPlugin
    {
        string IRobotPlugin.RobotName => "BasicRobot";

        string IRobotPlugin.DriverName => RobotDriverNames.BasicRobot;

        void IRobotPlugin.Init(string pluginFileName)
        {
        }

        IRobotDriver IRobotPlugin.CreateRobotDriverObj()
        {
            return new BasicRobot();
        }
    }
}
