using Newtonsoft.JsonG;
using Newtonsoft.JsonG.Converters;

namespace GKG.Dispense
{
    /// <summary>
    /// 数据库持久化点胶运行信息记录实体 (POCO)
    /// </summary>
    /// <remarks>
    /// 承载 R_Dispense 表的完整行数据
    /// </remarks>
    [Serializable]
    public class DispenseInfo
    {
        #region 核心属性

        private DateTime _markTime;
        private DateTime _dispenseTime;

        /// <summary>
        /// 点胶信息全局唯一ID
        /// </summary>
        public Guid RecordID { get; set; } = Guid.Empty;

        /// <summary>
        /// 模组别名
        /// </summary>
        public string ModuleAlias { get; set; } = string.Empty;

        /// <summary>
        /// 产品批次
        /// </summary>
        public string ProductBatch { get; set; } = string.Empty;

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; } = string.Empty;

        /// <summary>
        /// Mark时间点 (为空代表尚未Mark)
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime MarkTime
        {
            get => _markTime;
            set => _markTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        /// <summary>
        /// Mark持续时间
        /// </summary>
        public double MarkDuration { get; set; } = 0.0;

        /// <summary>
        /// 点胶时间点 (为空代表尚未点胶)
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DispenseTime
        {
            get => _dispenseTime;
            set => _dispenseTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        /// <summary>
        /// 点胶持续时间
        /// </summary>
        public double DispenseDuration { get; set; } = 0.0;

        /// <summary>
        /// 处理持续时间 (单板时间)
        /// </summary>
        public double ProcessDuration { get; set; } = 0.0;

        #endregion

        #region 核心操作

        /// <summary>
        /// 深度值拷贝
        /// </summary>
        /// <param name="source">数据源</param>
        /// <remarks>
        /// 执行防御性拷贝
        /// </remarks>
        public void CopyFrom(DispenseInfo source)
        {
            this.RecordID = source.RecordID;
            this.ModuleAlias = source.ModuleAlias;
            this.ProductBatch = source.ProductBatch;
            this.ProductID = source.ProductID;
            this.MarkTime = source.MarkTime;
            this.MarkDuration = source.MarkDuration;
            this.DispenseTime = source.DispenseTime;
            this.DispenseDuration = source.DispenseDuration;
            this.ProcessDuration = source.ProcessDuration;
        }

        #endregion
    }

    /// <summary>
    /// 点胶信息强类型集合
    /// </summary>
    /// <remarks>
    /// 专为适配 Griffins 底层框架的 UnfixedListObjBindInfo 机制而保留的派生类
    /// </remarks>
    [Serializable]
    public class DispenseInfoList : List<DispenseInfo>
    {
    }
}