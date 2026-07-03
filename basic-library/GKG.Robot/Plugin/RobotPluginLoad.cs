using GF_Gereric;
using System.Reflection;

namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 机器人插件加载器。
        /// </summary>
        public class RobotPluginLoad : PluginManagerBase
        {
            public RobotPluginLoad(string pluginPath) : base(pluginPath)
            {
                robotInfoDict = new RobotPluginInfoDict();
            }

            public new void Load()
            {
                base.Load();
            }

            private RobotPluginInfoDict robotInfoDict;

            public RobotPluginInfoDict AllRobotInfos => robotInfoDict;

            protected sealed override void OnBeforeLoad(string plugInPath)
            {
                robotInfoDict.Clear();
            }

            protected sealed override Guid GetPluginKind()
            {
                return RobotDefAttribute.PLUGINKIND;
            }

            protected sealed override void CheckAddPlugInMngClass(string pluginFilename, Guid pluginID, string pluginDescription, Type t)
            {
                if (checkAddRobotPlugin(pluginFilename, pluginID, t))
                    return;
            }

            private bool checkAddRobotPlugin(string pluginFilename, Guid pluginID, Type t)
            {
                Attribute[] attribs = Attribute.GetCustomAttributes(t, typeof(RobotDefAttribute), true);
                if (attribs.Length <= 0)
                    return false;

                if (t.GetInterface(typeof(IRobotPlugin).FullName!, true) == null)
                    return false;

                var robotPluginInfo = new RobotPluginInfo(pluginID, pluginFilename, t);
                robotInfoDict[((RobotDefAttribute)attribs[0]).RobotID] = robotPluginInfo;
                return true;
            }

            public class RobotPluginInfoDict : Dictionary<Guid, RobotPluginInfo>
            {
                public ICollection<Guid> RobotIDs => Keys;

                public ICollection<RobotPluginInfo> RobotPluginInfos => Values;
            }

            public class RobotPluginInfo
            {
                public RobotPluginInfo(Guid pluginID, string pluginFilename, Type objType)
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

                public IRobotPlugin CreateInstance()
                {
                    IRobotPlugin instance = (IRobotPlugin)objType.InvokeMember(null!,
                        BindingFlags.DeclaredOnly |
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null)!;
                    return instance;
                }
            }
        }
    }
}
