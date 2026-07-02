namespace GKG.Dispense
{
    /// <summary>
    /// 点胶数据库常量字典
    /// </summary>
    /// <remarks>
    /// 集中管理物理表名与字段名
    /// </remarks>
    public static class DispenseDBConst
    {
        #region 数据库表名定义

        /// <summary>
        /// 物理表别名/表名
        /// </summary>
        public const string TableAlias = "R_Dispense";

        #endregion

        #region 数据库字段定义

        /// <summary>
        /// 点胶信息记录ID
        /// </summary>
        public const string FIELD_RecordID = "RecordID";

        /// <summary>
        /// 模组实例别名
        /// </summary>
        public const string FIELD_ModuleAlias = "ModuleAlias";

        /// <summary>
        /// 产品批次
        /// </summary>
        public const string FIELD_ProductBatch = "ProductBatch";

        /// <summary>
        /// 产品ID
        /// </summary>
        public const string FIELD_ProductID = "ProductID";

        /// <summary>
        /// Mark识别时间
        /// </summary>
        public const string FIELD_MarkTime = "MarkTime";

        /// <summary>
        /// Mark持续时间
        /// </summary>
        public const string FIELD_MarkDuration = "MarkDuration";

        /// <summary>
        /// 点胶时间
        /// </summary>
        public const string FIELD_DispenseTime = "DispenseTime";

        /// <summary>
        /// 点胶持续时间
        /// </summary>
        public const string FIELD_DispenseDuration = "DispenseDuration";

        /// <summary>
        /// 处理持续时间 (单板时间)
        /// </summary>
        public const string FIELD_ProcessDuration = "ProcessDuration";
        
        #endregion
    }
}