namespace GKG.Station
{
    /// <summary>
    /// 工位数据库常量字典
    /// </summary>
    /// <remarks>
    /// 集中管理物理表名与字段名
    /// </remarks>
    public static class StationDBConst
    {
        #region 数据库表名定义

        /// <summary>
        /// 物理表别名/表名
        /// </summary>
        public const string TableAlias = "R_Station";

        #endregion

        #region 数据库字段定义

        /// <summary>
        /// 工位信息记录ID
        /// </summary>
        public const string FIELD_RecordID = "RecordID";

        /// <summary>
        /// 工位实例别名
        /// </summary>
        public const string FIELD_StationAlias = "StationAlias";

        /// <summary>
        /// 产品批次
        /// </summary>
        public const string FIELD_ProductBatch = "ProductBatch";

        /// <summary>
        /// 产品ID
        /// </summary>
        public const string FIELD_ProductID = "ProductID";

        /// <summary>
        /// 进板时长(秒)
        /// </summary>
        public const string FIELD_InDuration = "InDuration";

        /// <summary>
        /// 进板时间
        /// </summary>
        public const string FIELD_InTime = "InTime";

        /// <summary>
        /// 停板时长(秒)
        /// </summary>
        public const string FIELD_StopDuration = "StopDuration";

        /// <summary>
        /// 停板时间
        /// </summary>
        public const string FIELD_StopTime = "StopTime";

        /// <summary>
        /// 出板时长(秒)
        /// </summary>
        public const string FIELD_OutDuration = "OutDuration";

        /// <summary>
        /// 出板时间
        /// </summary>
        public const string FIELD_OutTime = "OutTime";

        #endregion
    }
}