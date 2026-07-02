namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// 视觉插件管理器。
        /// 参考 MotionControlPluginManager 的方式统一管理视觉驱动插件。
        /// </summary>
        public static class VisionPluginManager
        {
            private static readonly VisionDriverInfoList allVisionDriverInfos = new VisionDriverInfoList();
            private static bool initialized = false;

            public static void Init()
            {
                loadPlugins();
                initialized = true;
            }

            public static IVisionDriver GetVisionDriver(string driverName)
            {
                if (!initialized)
                {
                    Init();
                }

                string normalizedDriverName = string.IsNullOrWhiteSpace(driverName) ? "GVision" : driverName.Trim();
                var driverInfo = allVisionDriverInfos.Find(normalizedDriverName);
                if (driverInfo == null)
                    throw new Exception($"不存在对应的视觉驱动插件对象, 驱动名称:{normalizedDriverName}");

                return driverInfo.VisionPlugin.CreateVisionDriverObj();
            }

            public static IVisionDriverParameterEditor GetVisionDriverParameterEditor(string driverName)
            {
                if (!initialized)
                {
                    Init();
                }

                string normalizedDriverName = string.IsNullOrWhiteSpace(driverName) ? "GVision" : driverName.Trim();
                var driverInfo = allVisionDriverInfos.Find(normalizedDriverName);
                if (driverInfo == null)
                    throw new Exception($"不存在对应的视觉驱动参数编辑器对象, 驱动名称:{normalizedDriverName}");

                return driverInfo.VisionPlugin.CreateVisionDriverParameterEditorObj();
            }

            public static IFrontendVisionService? GetFrontendVisionService(string driverName)
            {
                if (!initialized)
                {
                    Init();
                }

                string normalizedDriverName = string.IsNullOrWhiteSpace(driverName) ? "GVision" : driverName.Trim();
                var driverInfo = allVisionDriverInfos.Find(normalizedDriverName);
                if (driverInfo == null)
                    throw new Exception($"不存在对应的前端视觉服务对象, 驱动名称:{normalizedDriverName}");

                return driverInfo.VisionPlugin.CreateFrontendVisionServiceObj();
            }

            public static List<string> GetAllVisionDriverNames()
            {
                if (!initialized)
                {
                    Init();
                }
                List<string> driverNames = new List<string>();
                foreach (var driverInfo in allVisionDriverInfos)
                {
                    driverNames.Add(driverInfo.DriverName);
                }
                return driverNames;
            }

            private static void loadPlugins()
            {
                string pluginPath = @"F:\Griffins7.1\Bin\Share\Vision\";
                if (!Directory.Exists(pluginPath))
                    Directory.CreateDirectory(pluginPath);

                var pluginLoad = new VisionPluginLoad(pluginPath);
                pluginLoad.Load();

                var tmpVisionInfoDict = pluginLoad.AllVisionInfos;
                allVisionDriverInfos.Clear();

                foreach (var visionID in tmpVisionInfoDict.VisionIDs)
                {
                    var pluginInfo = tmpVisionInfoDict[visionID];
                    try
                    {
                        IVisionPlugin visionPlugin = pluginInfo.CreateInstance();
                        visionPlugin.Init(pluginInfo.PluginFilename);
                        VisionDriverInfo driverInfo = new VisionDriverInfo(pluginInfo.PluginID, pluginInfo.PluginFilename, visionPlugin);
                        allVisionDriverInfos.Add(driverInfo);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"加载视觉插件失败: {pluginInfo.PluginFilename}", ex);
                    }
                }
            }

            private class VisionDriverInfo
            {
                public VisionDriverInfo(Guid pluginID, string pluginFilename, IVisionPlugin visionPlugin)
                {
                    PluginID = pluginID;
                    PluginFilename = pluginFilename;
                    VisionName = visionPlugin.VisionName;
                    DriverName = visionPlugin.DriverName;
                    VisionPlugin = visionPlugin;
                }

                public IVisionPlugin VisionPlugin { get; }
                public string VisionName { get; }
                public string DriverName { get; }
                public string PluginFilename { get; }
                public Guid PluginID { get; }
            }

            private class VisionDriverInfoList : List<VisionDriverInfo>
            {
                public VisionDriverInfo? Find(string driverName)
                {
                    foreach (VisionDriverInfo driverInfo in this)
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
