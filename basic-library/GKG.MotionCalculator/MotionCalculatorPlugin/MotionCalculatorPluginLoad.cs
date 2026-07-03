using GF_Gereric;
using System.Reflection;

namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 运动计算插件加载器。
        /// </summary>
        public class MotionCalculatorPluginLoad : PluginManagerBase
        {
            public MotionCalculatorPluginLoad(string pluginPath) : base(pluginPath)
            {
                motionCalculatorInfoDict = new MotionCalculatorPluginInfoDict();
            }

            public new void Load()
            {
                base.Load();
            }

            private MotionCalculatorPluginInfoDict motionCalculatorInfoDict;

            /// <summary>
            /// 所有运动计算插件信息。
            /// </summary>
            public MotionCalculatorPluginInfoDict AllMotionCalculatorInfos
            {
                get { return motionCalculatorInfoDict; }
            }

            protected sealed override void OnBeforeLoad(string plugInPath)
            {
                motionCalculatorInfoDict.Clear();
            }

            protected sealed override Guid GetPluginKind()
            {
                return MotionCalculatorDefAttribute.PLUGINKIND;
            }

            protected sealed override void CheckAddPlugInMngClass(string pluginFilename, Guid pluginID, string pluginDescription, Type t)
            {
                if (checkAddMotionCalculatorPlugin(pluginFilename, pluginID, t))
                    return;
            }

            private bool checkAddMotionCalculatorPlugin(string pluginFilename, Guid pluginID, Type t)
            {
                Attribute[] attribs = Attribute.GetCustomAttributes(t, typeof(MotionCalculatorDefAttribute), true);
                if (attribs.Length <= 0)
                    return false;

                if (t.GetInterface(typeof(IMotionCalculatorPlugin).FullName!, true) == null)
                    return false;

                var motionCalculatorPluginInfo = new MotionCalculatorPluginInfo(pluginID, pluginFilename, t);
                motionCalculatorInfoDict[((MotionCalculatorDefAttribute)attribs[0]).MotionCalculatorID] = motionCalculatorPluginInfo;
                return true;
            }

            public class MotionCalculatorPluginInfoDict : Dictionary<Guid, MotionCalculatorPluginInfo>
            {
                public ICollection<Guid> MotionCalculatorIDs => Keys;

                public ICollection<MotionCalculatorPluginInfo> MotionCalculatorPluginInfos => Values;
            }

            public class MotionCalculatorPluginInfo
            {
                public MotionCalculatorPluginInfo(Guid pluginID, string pluginFilename, Type objType)
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

                public IMotionCalculatorPlugin CreateInstance()
                {
                    IMotionCalculatorPlugin instance = (IMotionCalculatorPlugin)objType.InvokeMember(null!,
                        BindingFlags.DeclaredOnly |
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null)!;
                    return instance;
                }
            }
        }
    }
}
