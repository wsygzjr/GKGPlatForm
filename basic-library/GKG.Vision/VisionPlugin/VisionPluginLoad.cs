using GF_Gereric;
using System.Reflection;

namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// 视觉插件加载器。
        /// </summary>
        public class VisionPluginLoad : PluginManagerBase
        {
            public VisionPluginLoad(string pluginPath) : base(pluginPath)
            {
                visionInfoDict = new VisionPluginInfoDict();
            }

            public new void Load()
            {
                base.Load();
            }

            private VisionPluginInfoDict visionInfoDict;

            /// <summary>
            /// 所有视觉插件信息。
            /// </summary>
            public VisionPluginInfoDict AllVisionInfos
            {
                get { return visionInfoDict; }
            }

            protected sealed override void OnBeforeLoad(string plugInPath)
            {
                visionInfoDict.Clear();
            }

            protected sealed override Guid GetPluginKind()
            {
                return VisionDefAttribute.PLUGINKIND;
            }

            protected sealed override void CheckAddPlugInMngClass(string pluginFilename, Guid pluginID, string pluginDescription, Type t)
            {
                if (checkAddVisionPlugin(pluginFilename, pluginID, t))
                    return;
            }

            private bool checkAddVisionPlugin(string pluginFilename, Guid pluginID, Type t)
            {
                Attribute[] attribs = Attribute.GetCustomAttributes(t, typeof(VisionDefAttribute), true);
                if (attribs.Length <= 0)
                    return false;

                if (t.GetInterface(typeof(IVisionPlugin).FullName!, true) == null)
                    return false;

                var visionPluginInfo = new VisionPluginInfo(pluginID, pluginFilename, t);
                visionInfoDict[((VisionDefAttribute)attribs[0]).VisionID] = visionPluginInfo;
                return true;
            }

            public class VisionPluginInfoDict : Dictionary<Guid, VisionPluginInfo>
            {
                public ICollection<Guid> VisionIDs => Keys;

                public ICollection<VisionPluginInfo> VisionPluginInfos => Values;
            }

            public class VisionPluginInfo
            {
                public VisionPluginInfo(Guid pluginID, string pluginFilename, Type objType)
                {
                    this.pluginID = pluginID;
                    this.pluginFilename = pluginFilename;
                    this.objType = objType;
                }

                private Guid pluginID;
                public Guid PluginID => pluginID;

                private Type objType;
                public Type ObjType => objType;

                private string pluginFilename;
                public string PluginFilename => pluginFilename;

                public IVisionPlugin CreateInstance()
                {
                    IVisionPlugin instance = (IVisionPlugin)objType.InvokeMember(null!,
                        BindingFlags.DeclaredOnly |
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null)!;
                    return instance;
                }
            }
        }
    }
}
