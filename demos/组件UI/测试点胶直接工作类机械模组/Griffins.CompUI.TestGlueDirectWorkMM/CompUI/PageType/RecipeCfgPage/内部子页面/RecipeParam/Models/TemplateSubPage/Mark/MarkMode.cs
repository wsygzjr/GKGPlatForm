namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark
{

    /// <summary>
    /// Mark方式枚举
    /// </summary>
    public enum MarkMode
    {
        /// <summary>
        /// 定位补偿
        /// </summary>
        PositionCompensation = 0,
        /// <summary>
        /// 模板基准
        /// </summary>
        TemplateBenchmark = 1,
    }

    /// <summary>
    /// Mark操作类型枚举
    /// </summary>
    public enum MarkOpKind
    {
        /// <summary>
        /// 标准
        /// </summary>
        Standard = 0,
        /// <summary>
        /// 摄像头模组
        /// </summary>
        Camera ,
        /// <summary>
        /// 边角中心
        /// </summary>
        CornerCenter,
        /// <summary>
        /// 自定义
        /// </summary>
        Custom ,
    }
}