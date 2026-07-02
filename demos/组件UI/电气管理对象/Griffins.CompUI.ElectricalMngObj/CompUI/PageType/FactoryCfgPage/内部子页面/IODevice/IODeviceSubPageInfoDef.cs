using Griffins.ImeIOT;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage
{
    /// <summary>
    /// IO设备配置子页面信息定义
    /// </summary>
    public class IODeviceSubPageInfoDef
    {
        /// <summary>
        /// 内部子页面类型ID(当子页面种类为内部子页面时有效)
        /// </summary>
        public const string InnerSubPageTypeIDStr = "FactoryCfg-IODeviceCfg";
        public static readonly InnerSubPageTypeID InnerSubPageTypeID = new InnerSubPageTypeID(InnerSubPageTypeIDStr);

        /// <summary>
        /// 内部子页面类型名称
        /// </summary>
        public const string InnerSubPageTypeName = "IO设备配置";
        /// <summary>
        /// 子页面种类
        /// </summary>
        public static readonly SubPageKind SubPageKind = SubPageKind.Inner;

    }
}