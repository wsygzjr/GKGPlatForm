using Griffins.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage
{
    /// <summary>
    /// 缓存数据交换
    /// </summary>
    internal class CacheDataExchange
    {
        static CacheDataExchange()
        {
        }

        #region 阀信息（模拟）

        /// <summary>
        /// 获取阀信息
        /// </summary>
        public static List<ValveInfo> GetValveInfoes()
        {
            var valveInfoes = new List<ValveInfo>();
            valveInfoes.Add(new ValveInfo() { ValveNumber = "Valve1", ValveName = "阀1" });
            valveInfoes.Add(new ValveInfo() { ValveNumber = "Valve2", ValveName = "阀2" });
            return valveInfoes;
        }

        #endregion

        #region 相机信息（模拟）

        /// <summary>
        /// 获取相机信息
        /// </summary>
        public static List<ValveInfo> GetCameraes()
        {
            var valveInfoes = new List<ValveInfo>();
            valveInfoes.Add(new ValveInfo() { ValveNumber = "Camera1", ValveName = "相机1" });
            return valveInfoes;
        }

        #endregion

    }


    /// <summary>
    /// 阀信息
    /// </summary>
    public class ValveInfo
    {
        /// <summary>
        /// 阀号
        /// </summary>
        public string ValveNumber { set; get; } = "";
        /// <summary>
        /// 阀名称
        /// </summary>
        public string ValveName { set; get; } = "";
        public ValveInfo()
        {
        }
    }
}
