using Griffins.ImeIOT;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage
{
    /// <summary>
    ///配方配置子页面信息定义
    /// </summary>
    public class PPCfgSubPageInfoDef
    {
        /// <summary>
        /// 内部子页面类型ID(当子页面种类为内部子页面时有效)
        /// </summary>
        public const string InnerSubPageTypeIDStr = "InnerSubPageTypeID-PPCfg";
        public static readonly InnerSubPageTypeID InnerSubPageTypeID = new InnerSubPageTypeID(InnerSubPageTypeIDStr);

        /// <summary>
        /// 内部子页面类型名称
        /// </summary>
        public const string InnerSubPageTypeName = "工艺参数与计算轨迹子页面";
        /// <summary>
        /// 子页面种类
        /// </summary>
        public static readonly SubPageKind SubPageKind = SubPageKind.Inner;

        /// <summary>
        /// 对应的组件ID（大小写无关的字符串，去前后空字符）
        /// </summary>
        public static readonly CompID CompID = new CompID("DJ");
        /// <summary>
        /// 所属组件类型ID
        /// </summary>

        public static readonly CompTypeID CompTypeID = ImeIOTConst.CompType_MM;

    }
}