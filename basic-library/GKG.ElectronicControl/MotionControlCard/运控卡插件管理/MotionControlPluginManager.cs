using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GKG.ElectronicControl
{
    public static class MotionControlPluginManager
    {
        private static MotionControlCardInfoList allMotionControlCardInfos = new MotionControlCardInfoList();
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            loadPlugins();
        }

        /// <summary>
        /// 加载第三方接出接口插件插件
        /// </summary>
        private static void loadPlugins()
        {
            //string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Share", "MotionControl");
            //考虑通过配置文件读取运控卡插件的目录
            string pluginPath = @"F:\Griffins7.1\Bin\Share\MotionControl\";
            if (!Directory.Exists(pluginPath))
                Directory.CreateDirectory(pluginPath);

            var pluginLoad = new MotionControlPluginLoad(pluginPath);
            pluginLoad.Load();

            var tmpMotionControlInfoDict = pluginLoad.AllMotionControlInfos;
            allMotionControlCardInfos.Clear();
            //调用初始化方法
            foreach (var motionControlID in tmpMotionControlInfoDict.MotionControlIDs)
            {
                var pluginInfo = tmpMotionControlInfoDict[motionControlID];
                try
                {
                    IMotionControlPlugin iMotionControl = pluginInfo.CreateInstance();
                    iMotionControl.Init(pluginInfo.PluginFilename);
                    MotionControlCardInfo mmDriverInfo = new MotionControlCardInfo(pluginInfo.PluginID, pluginInfo.PluginFilename, iMotionControl);
                    allMotionControlCardInfos.Add(mmDriverInfo);
                }
                catch (Exception err)
                {
                    string t = err.Message;
                }
            }
        }

        /// <summary>
        /// 获取运控卡类型对应的运控卡接口实例
        /// </summary>
        /// <param name="motionCardType">运控卡类型</param>
        /// <returns>运控卡类型对应的运控卡接口实例</returns>
        public static IMotionControlBase GetMotionControl(MotionCardType motionCardType)
        {
            var driverInfo = allMotionControlCardInfos.Find(motionCardType);
            if (driverInfo == null)
                throw new Exception($"不存在对应的运控卡插件对象,运控卡类型:{motionCardType.ToString()}");
            return driverInfo.IMotionControl;
        }
        /// <summary>
        /// 获取控制卡默认配置
        /// </summary>
        /// <param name="motionCardType">控制卡类型</param>
        /// <returns></returns>
        public static MotionControlDriverDefaultConfig GetDefaultConfig(MotionCardType motionCardType)
        {
            var driverInfo = allMotionControlCardInfos.Find(motionCardType);
            if (driverInfo == null)
                throw new Exception($"不存在对应的运控卡插件对象,运控卡类型:{motionCardType.ToString()}");
            return driverInfo.MotionControlPlugin.GetDefaultConfig();
        }
        /// <summary>
        /// 获取所有运动卡驱动类型信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<MontionControlDriverTypeInfo> GetAllMontionControlDriverTypeInfo()
        {
            if (allMotionControlCardInfos == null)
                throw new ArgumentNullException(nameof(MotionControlCardInfoList));
            return allMotionControlCardInfos.Select(info =>
            {
                var motionCardInterfaceType = MotionControlCardType.Normal;
                switch (info.MotionCardType)
                {
                    case MotionCardType.GC800:
                        motionCardInterfaceType = MotionControlCardType.TypeA;
                        break;
                    case MotionCardType.DMC1000B:
                    case MotionCardType.GMCMINI:
                        motionCardInterfaceType = MotionControlCardType.Normal;
                        break;
                    case MotionCardType.Zmotion:
                        motionCardInterfaceType = MotionControlCardType.TypeA;
                        break;
                    default:
                        motionCardInterfaceType = MotionControlCardType.Normal;
                        break;
                }
                return new MontionControlDriverTypeInfo(motionCardInterfaceType, info.MotionCardType, info.MotionControlName);
            });
        }

        #region 内部类型

        /// <summary>
        /// 运控卡信息
        /// </summary>
        private class MotionControlCardInfo
        {
            /// <summary>
            /// 创建MotionControlCardInfo新实例
            /// </summary>
            /// <param name="pluginID">插件ID</param>
            /// <param name="pluginFilename">插件文件名和路径</param>
            /// <param name="iMotionControl">运控卡插件接口实例</param>
            public MotionControlCardInfo(Guid pluginID, string pluginFilename, IMotionControlPlugin iMotionControl)
            {
                this.PluginID = pluginID;
                this.PluginFilename = pluginFilename;
                this.MotionCardType = iMotionControl.MotionCardType;
                this.MotionControlName = iMotionControl.MotionControlName;
                this.IMotionControl = iMotionControl.CreateMotionCardObj();
                this.MotionControlPlugin = iMotionControl;
            }

            /// <summary>
            /// 运控卡接口实例
            /// </summary>
            public IMotionControlBase IMotionControl { get; }
            ///<summary>
            /// 运控卡类型
            /// </summary>
            public MotionCardType MotionCardType { get; }
            /// <summary>
            /// 运控卡名称
            /// </summary>
            public string MotionControlName { get; }

            /// <summary>
            /// 插件文件名和路径
            /// </summary>
            public string PluginFilename { get; }
            /// <summary>
            /// 插件ID
            /// </summary>
            public Guid PluginID { get; }

            /// <summary>
            /// 控制卡插件接口
            /// </summary>
            public IMotionControlPlugin MotionControlPlugin { get; }
        }

        /// <summary>
        /// 运控卡信息列表
        /// </summary>
        private class MotionControlCardInfoList : List<MotionControlCardInfo>
        {
            /// <summary>
            /// 查找运控卡类型对应的运控卡信息
            /// </summary>
            /// <param name="motionCardType">查找的运控卡类型</param>
            /// <returns>运控卡类型对应的运控卡信息</returns>
            public MotionControlCardInfo? Find(MotionCardType motionCardType)
            {
                foreach (MotionControlCardInfo mmDriverInfo in this)
                {
                    if (mmDriverInfo.MotionCardType == motionCardType)
                        return mmDriverInfo;
                }
                return null;
            }
        }

        #endregion
    }
}
