using Griffins.ImeIOT;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.ElectricalMngObj.DebuggingPage
{
    /// <summary>
    /// IOOUT调试子页面信息定义
    /// </summary>
    public class IOOutDebuggingSubPageInfoDef
    {
        /// <summary>
        /// 内部子页面类型ID(当子页面种类为内部子页面时有效)
        /// </summary>
        public const string InnerSubPageTypeIDStr = "IOOutDebugging";
        public static readonly InnerSubPageTypeID InnerSubPageTypeID = new InnerSubPageTypeID(InnerSubPageTypeIDStr);

        /// <summary>
        /// 内部子页面类型名称
        /// </summary>
        public const string InnerSubPageTypeName = "IOOUT调试页面";

    }
}