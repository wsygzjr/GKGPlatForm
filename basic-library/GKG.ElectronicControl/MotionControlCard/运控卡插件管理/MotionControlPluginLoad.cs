using GF_Gereric;
using System.Reflection;

namespace GKG.ElectronicControl
{
    public class MotionControlPluginLoad : PluginManagerBase
    {
        /// <summary>
        /// 创建 MotionControlPluginLoad 新实例
        /// </summary>
        /// <param name="plugInPath">插件路径</param>
        public MotionControlPluginLoad(string pluginPath) : base(pluginPath)
        {
            motionControlInfoDict = new MotionControlPluginInfoDict();
        }

        /// <summary>
        /// 加载
        /// </summary>
        public new void Load()
        {
            base.Load();
        }
        private MotionControlPluginInfoDict motionControlInfoDict;
        /// <summary>
        ///运控卡插件信息字典
        /// </summary>
        public MotionControlPluginInfoDict AllMotionControlInfos
        {
            get { return motionControlInfoDict; }
        }

        #region 重载基类的函数
        /// <summary>
        /// 加载插件前
        /// </summary>
        /// <param name="plugInPath">插件所在目录</param>
        protected sealed override void OnBeforeLoad(string plugInPath)
        {
            motionControlInfoDict.Clear();
        }

        /// <summary>
        /// 取要加载的插件种类
        /// </summary>
        /// <returns>要加载的插件种类</returns>
        protected sealed override Guid GetPluginKind()
        {
            return MotionControlDefAttribute.PLUGINKIND;
        }
        /// <summary>
        /// 检查是否插件管理类
        /// </summary>
        /// <param name="pluginFilename">插件文件路径</param>
        /// <param name="pluginID">插件ID</param>
        /// <param name="pluginDescription">插件描述</param>
        /// <param name="t">检查类的类型信息</param>
        protected sealed override void CheckAddPlugInMngClass(string pluginFilename, Guid pluginID, string pluginDescription, Type t)
        {
            if (checkAddMotionControlObjPlugin(pluginFilename, pluginID, t))
                return;
        }

        private bool checkAddMotionControlObjPlugin(string pluginFilename, Guid pluginID, Type t)
        {
            //检查是不是数据引擎插件对象，是就处理 
            Attribute[] attribs = Attribute.GetCustomAttributes(t, typeof(MotionControlDefAttribute), true);
            if (attribs.Length <= 0)
                return false;
            //判断是不是实现了 IMotionControlPlugin，实现了表示是运控卡插件对象
            if (t.GetInterface(typeof(IMotionControlPlugin).FullName!, true) == null)
                return false;
            var motionControlPluginInfo = new MotionControlPluginInfo(pluginID, pluginFilename, t);
            motionControlInfoDict[((MotionControlDefAttribute)attribs[0]).MotionControlID] = motionControlPluginInfo;

            return true;
        }

        #endregion

        #region 内部类型

        /// <summary>
        /// 运控卡插件信息字典
        /// </summary>
        public class MotionControlPluginInfoDict : Dictionary<Guid, MotionControlPluginInfo>
        {

            /// <summary>
            /// 运控卡ID
            /// </summary>
            public ICollection<Guid> MotionControlIDs
            {
                get
                {
                    return (this.Keys);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public ICollection<MotionControlPluginInfo> SubMachineModulesExecInfos
            {
                get
                {
                    return (this.Values);
                }
            }
        }

        /// <summary>
        /// 运控卡插件信息
        /// </summary>
        public class MotionControlPluginInfo
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pluginID">子机械模组插件ID</param>
            /// <param name="pluginFilename"></param>
            /// <param name="objType"></param>
            public MotionControlPluginInfo(Guid pluginID, string pluginFilename, Type objType)
            {
                this.pluginID = pluginID;
                this.pluginFilename = pluginFilename;
                this.objType = objType;
            }

            private Guid pluginID;
            /// <summary>
            /// 子机械模组插件ID
            /// </summary>
            public Guid PluginID
            {
                get { return pluginID; }
            }
            private Type objType;
            /// <summary>
            /// 子机械模组执行对象类型
            /// </summary>
            public Type ObjType
            {
                get { return objType; }
            }

            private string pluginFilename;
            /// <summary>
            /// 插件文件名和路径
            /// </summary>
            public string PluginFilename
            {
                get { return pluginFilename; }
            }

            /// <summary>
            /// 创建控制子机械模组接口实例
            /// </summary>
            /// <returns>控制子机械模组接口实例</returns>
            public IMotionControlPlugin CreateInstance()
            {
                IMotionControlPlugin instance = (IMotionControlPlugin)objType.InvokeMember(null!,
                    BindingFlags.DeclaredOnly |
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null)!;
                return instance;
            }
        }
        #endregion
    }
}
