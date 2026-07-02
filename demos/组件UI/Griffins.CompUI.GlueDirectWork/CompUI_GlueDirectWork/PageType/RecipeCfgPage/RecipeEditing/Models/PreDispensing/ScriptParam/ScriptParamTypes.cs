namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 脚本参数界面类型枚举
    /// </summary>
    public enum ScriptParamType
    {
        /// <summary>
        /// 检测漏点胶
        /// </summary>
        MissingGlueDetection = 0,

        /// <summary>
        /// 最大胶点面积检测
        /// </summary>
        MaxGlueAreaDetection = 1,

        /// <summary>
        /// 胶点直径个数偏移检测
        /// </summary>
        GlueDiameterCountOffset = 2,

        /// <summary>
        /// 禁用
        /// </summary>
        DisableChecks = 3,

        /// <summary>
        /// 总面积检测
        /// </summary>
        TotalAreaDetection = 4,

        /// <summary>
        /// 胶点个数检测
        /// </summary>
        GlueCountDetection = 5
    }

    /// <summary>
    /// 脚本结果枚举
    /// </summary>
    public enum ScriptResultType
    {
        Default = 0,
        Fail = 1,
        Pass = 2
    }

    /// <summary>
    /// 检测逻辑类型
    /// </summary>
    public enum DetectionLogicType
    {
        /// <summary>
        /// 全部脚本通过则OK
        /// </summary>
        AllPass = 0,

        /// <summary>
        /// 有一个脚本通过则Ok
        /// </summary>
        AnyPass = 1
    }

    /// <summary>
    /// 条件类型
    /// </summary>
    public enum ConditionType
    {
        /// <summary>
        /// 区间
        /// </summary>
        Interval = 0,
        /// <summary>
        /// 小于
        /// </summary>
        Less = 1,
        /// <summary>
        /// 大于
        /// </summary>
        Greater = 2,
        /// <summary>
        /// 等于
        /// </summary>
        Equal = 3
    }

    /// <summary>
    /// 数值调节方式类型
    /// </summary>
    public enum ValueAdjustModeType
    {
        /// <summary>
        /// 数值
        /// </summary>
        Value = 0,
        /// <summary>
        /// 百分比
        /// </summary>
        Percentage = 1
    }
}
