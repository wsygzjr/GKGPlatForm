using GKG.ElectronicControl;

namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 机器人插件管理器。
        /// </summary>
        public static class RobotPluginManager
        {
            private static readonly RobotDriverInfoList allRobotDriverInfos = new RobotDriverInfoList();
            private static bool initialized = false;

            public static void Init()
            {
                loadPlugins();
                initialized = true;
            }

            public static IRobotDriver GetRobotDriver(MotionControlCardType motionControlCardType)
            {
                if (!initialized)
                {
                    Init();
                }

                string resolvedDriverName = ResolveDriverName(motionControlCardType);
                var driverInfo = allRobotDriverInfos.Find(resolvedDriverName);
                if (driverInfo == null)
                    throw new Exception($"不存在对应的机器人驱动插件对象, 驱动名称:{resolvedDriverName}");
                IRobotDriver driver = driverInfo.RobotPlugin.CreateRobotDriverObj();
                return driver;
            }

            public static List<string> GetAllRobotDriverNames()
            {
                if (!initialized)
                {
                    Init();
                }

                List<string> driverNames = new List<string>();
                foreach (var driverInfo in allRobotDriverInfos)
                {
                    driverNames.Add(driverInfo.DriverName);
                }
                return driverNames;
            }

            private static string ResolveDriverName(MotionControlCardType motionControlCardType)
            {
                switch (motionControlCardType)
                {
                    case MotionControlCardType.TypeA:
                        return RobotDriverNames.CategoryARobot;

                    case MotionControlCardType.Normal:
                    default:
                        return RobotDriverNames.BasicRobot;
                }
            }

            private static void loadPlugins()
            {
                string pluginPath = @"F:\Griffins7.1\Bin\Share\Robot\";
                if (!Directory.Exists(pluginPath))
                    Directory.CreateDirectory(pluginPath);

                var pluginLoad = new RobotPluginLoad(pluginPath);
                pluginLoad.Load();

                var tmpRobotInfoDict = pluginLoad.AllRobotInfos;
                allRobotDriverInfos.Clear();

                foreach (var robotID in tmpRobotInfoDict.RobotIDs)
                {
                    var pluginInfo = tmpRobotInfoDict[robotID];
                    try
                    {
                        IRobotPlugin robotPlugin = pluginInfo.CreateInstance();
                        robotPlugin.Init(pluginInfo.PluginFilename);
                        RobotDriverInfo driverInfo = new RobotDriverInfo(pluginInfo.PluginID, pluginInfo.PluginFilename, robotPlugin);
                        allRobotDriverInfos.Add(driverInfo);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"加载机器人插件失败: {pluginInfo.PluginFilename}", ex);
                    }
                }
            }

            private class RobotDriverInfo
            {
                public RobotDriverInfo(Guid pluginID, string pluginFilename, IRobotPlugin robotPlugin)
                {
                    PluginID = pluginID;
                    PluginFilename = pluginFilename;
                    RobotName = robotPlugin.RobotName;
                    DriverName = robotPlugin.DriverName;
                    RobotPlugin = robotPlugin;
                }

                public IRobotPlugin RobotPlugin { get; }
                public string RobotName { get; }
                public string DriverName { get; }
                public string PluginFilename { get; }
                public Guid PluginID { get; }
            }

            private class RobotDriverInfoList : List<RobotDriverInfo>
            {
                public RobotDriverInfo? Find(string driverName)
                {
                    foreach (var driverInfo in this)
                    {
                        if (string.Equals(driverInfo.DriverName, driverName, StringComparison.OrdinalIgnoreCase))
                            return driverInfo;
                    }
                    return null;
                }
            }
        }
    }
}
