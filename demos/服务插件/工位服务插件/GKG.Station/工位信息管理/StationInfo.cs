using Newtonsoft.JsonG;
using Newtonsoft.JsonG.Converters;

namespace GKG.Station
{
    /// <summary>
    /// 数据库持久化工位运行信息记录实体 (POCO)
    /// </summary>
    /// <remarks>
    /// 承载 R_Station 表的完整行数据
    /// </remarks>
    [Serializable]
    public class StationInfo
    {
        #region 核心属性

        private DateTime _inTime;
        private DateTime _stopTime;
        private DateTime _outTime;

        /// <summary>
        /// 工位信息全局唯一ID
        /// </summary>
        public Guid RecordID { get; set; } = Guid.Empty;

        /// <summary>
        /// 工位别名
        /// </summary>
        public string StationAlias { get; set; } = string.Empty;

        /// <summary>
        /// 产品批次
        /// </summary>
        public string ProductBatch { get; set; } = string.Empty;

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; } = string.Empty;

        /// <summary>
        /// 进板持续时间
        /// </summary>
        public double InDuration { get; set; } = 0.0;

        /// <summary>
        /// 进板时间点 (为空代表尚未进板)
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime InTime
        {
            get => _inTime;
            set => _inTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        /// <summary>
        /// 停留持续时间
        /// </summary>
        public double StopDuration { get; set; } = 0.0;

        /// <summary>
        /// 停留时间点 (为空代表尚未到达停留位)
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime StopTime
        {
            get => _stopTime;
            set => _stopTime  = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        /// <summary>
        /// 出板持续时间
        /// </summary>
        public double OutDuration { get; set; } = 0.0;

        /// <summary>
        /// 出板时间点 (为空代表尚未出板)
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime OutTime
        {
            get => _outTime;
            set => _outTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        #endregion

        #region 核心操作

        /// <summary>
        /// 深度值拷贝
        /// </summary>
        /// <param name="source">数据源</param>
        /// <remarks>
        /// 执行防御性拷贝
        /// </remarks>
        public void CopyFrom(StationInfo source)
        {
            this.RecordID = source.RecordID;
            this.StationAlias = source.StationAlias;
            this.ProductBatch = source.ProductBatch;
            this.ProductID = source.ProductID;
            this.InDuration = source.InDuration;
            this.InTime = source.InTime;
            this.StopDuration = source.StopDuration;
            this.StopTime = source.StopTime;
            this.OutDuration = source.OutDuration;
            this.OutTime = source.OutTime;
        }

        #endregion
    }

    /// <summary>
    /// 工位信息强类型集合
    /// </summary>
    /// <remarks>
    /// 专为适配 Griffins 底层框架的 UnfixedListObjBindInfo 机制而保留的派生类
    /// </remarks>
    [Serializable]
    public class StationInfoList : List<StationInfo>
    {
    }
}