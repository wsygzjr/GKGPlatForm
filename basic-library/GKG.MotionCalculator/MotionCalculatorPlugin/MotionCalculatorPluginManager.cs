namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 运动计算插件管理器。
        /// </summary>
        public static class MotionCalculatorPluginManager
        {
            private static readonly MotionCalculatorDriverInfoList allMotionCalculatorDriverInfos = new MotionCalculatorDriverInfoList();
            private static bool initialized = false;

            public static void Init()
            {
                loadPlugins();
                initialized = true;
            }

            public static IMotionCalculatorDriver GetMotionCalculatorDriver(string driverName)
            {
                if (!initialized)
                {
                    Init();
                }

                string normalizedDriverName = string.IsNullOrWhiteSpace(driverName)
                    ? MotionCalculatorDriverNames.XYZ_xyz
                    : driverName.Trim();

                var driverInfo = allMotionCalculatorDriverInfos.Find(normalizedDriverName);
                if (driverInfo == null)
                    throw new Exception($"不存在对应的运动计算驱动插件对象, 驱动名称:{normalizedDriverName}");

                return driverInfo.MotionCalculatorPlugin.CreateMotionCalculatorDriverObj();
            }

            public static List<string> GetAllMotionCalculatorDriverNames()
            {
                if (!initialized)
                {
                    Init();
                }

                List<string> driverNames = new List<string>();
                foreach (var driverInfo in allMotionCalculatorDriverInfos)
                {
                    driverNames.Add(driverInfo.DriverName);
                }
                return driverNames;
            }

            private static void loadPlugins()
            {
                string pluginPath = @"F:\Griffins7.1\Bin\Share\MotionCalculator\";
                if (!Directory.Exists(pluginPath))
                    Directory.CreateDirectory(pluginPath);

                var pluginLoad = new MotionCalculatorPluginLoad(pluginPath);
                pluginLoad.Load();

                var tmpMotionCalculatorInfoDict = pluginLoad.AllMotionCalculatorInfos;
                allMotionCalculatorDriverInfos.Clear();

                foreach (var motionCalculatorID in tmpMotionCalculatorInfoDict.MotionCalculatorIDs)
                {
                    var pluginInfo = tmpMotionCalculatorInfoDict[motionCalculatorID];
                    try
                    {
                        IMotionCalculatorPlugin motionCalculatorPlugin = pluginInfo.CreateInstance();
                        motionCalculatorPlugin.Init(pluginInfo.PluginFilename);
                        MotionCalculatorDriverInfo driverInfo = new MotionCalculatorDriverInfo(pluginInfo.PluginID, pluginInfo.PluginFilename, motionCalculatorPlugin);
                        allMotionCalculatorDriverInfos.Add(driverInfo);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"加载运动计算插件失败: {pluginInfo.PluginFilename}", ex);
                    }
                }
            }

            private class MotionCalculatorDriverInfo
            {
                public MotionCalculatorDriverInfo(Guid pluginID, string pluginFilename, IMotionCalculatorPlugin motionCalculatorPlugin)
                {
                    PluginID = pluginID;
                    PluginFilename = pluginFilename;
                    MotionCalculatorName = motionCalculatorPlugin.MotionCalculatorName;
                    DriverName = motionCalculatorPlugin.DriverName;
                    MotionCalculatorPlugin = motionCalculatorPlugin;
                }

                public IMotionCalculatorPlugin MotionCalculatorPlugin { get; }
                public string MotionCalculatorName { get; }
                public string DriverName { get; }
                public string PluginFilename { get; }
                public Guid PluginID { get; }
            }

            private class MotionCalculatorDriverInfoList : List<MotionCalculatorDriverInfo>
            {
                public MotionCalculatorDriverInfo? Find(string driverName)
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
