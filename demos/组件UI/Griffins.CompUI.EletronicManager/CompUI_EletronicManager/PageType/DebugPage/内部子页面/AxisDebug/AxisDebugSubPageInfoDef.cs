using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.DebugPage
{
    public class AxisDebugSubPageInfoDef
    {
        public const string AxisDebugInnerSubPageTypeIDStr = "DebugPage-AxisDebug";
        public static readonly InnerSubPageTypeID AxisDebugInnerSubPageTypeID = new InnerSubPageTypeID(AxisDebugInnerSubPageTypeIDStr);
        public const string AxisDebugInnerSubPageTypeName = "轴调试";

        public const string IOInInnerSubPageTypeIDStr = "DebugPage-IOIn";
        public static readonly InnerSubPageTypeID IOInInnerSubPageTypeID = new InnerSubPageTypeID(IOInInnerSubPageTypeIDStr);
        public const string IOInInnerSubPageTypeName = "状态量调试";

        public const string IOOutInnerSubPageTypeIDStr = "DebugPage-IOOut";
        public static readonly InnerSubPageTypeID IOOutInnerSubPageTypeID = new InnerSubPageTypeID(IOOutInnerSubPageTypeIDStr);
        public const string IOOutInnerSubPageTypeName = "IO输出";
    }
}
