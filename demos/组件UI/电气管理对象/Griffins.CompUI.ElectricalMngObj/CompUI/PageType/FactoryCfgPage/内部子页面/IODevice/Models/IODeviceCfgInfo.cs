namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models
{
    /// <summary>
    /// IO设备信息配置信息
    /// </summary>
    public class IODeviceCfgInfo
    {
        /// <summary>
        /// IO设备型号
        /// </summary>
        public string IODeviceModel { set; get; }

        /// <summary>
        /// IO设备ID
        /// </summary>
        public string IODeviceID { set; get; }

        /// <summary>
        /// 序号
        /// </summary>
        public string SerialNumber { set; get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public IODeviceCfgInfo()
        {
            IODeviceModel = "";
            IODeviceID = "";
            SerialNumber = "";
        }
     
    }
}
