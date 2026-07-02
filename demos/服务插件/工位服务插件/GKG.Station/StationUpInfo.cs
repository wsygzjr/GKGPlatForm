using GF_Gereric;
using Newtonsoft.JsonG;
using Newtonsoft.JsonG.Converters;

namespace GKG.Station
{
    /// <summary>
    /// 工位信息上报实体 (DTO)
    /// </summary>
    /// <remarks>
    /// 用于承载底层设备或模块向上层应用服务传输的工位数据
    /// </remarks>
    public class StationUpInfo
    {
        #region 核心属性

        /// <summary>
        /// 工位实例别名
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
        /// 进板时长（秒）
        /// </summary>
        public double InDuration { get; set; } = 0.0;

        /// <summary>
        /// 进板时间
        /// </summary>
        /// <remarks>为空 (null) 代表尚未进板</remarks>
        [JsonConverter(typeof(IsoDateTimeConverter))] 
        public DateTime InTime { get; set; } = DateTime.UtcNow; 

        /// <summary>
        /// 停板时长（秒）
        /// </summary>
        public double StopDuration { get; set; } = 0.0;

        /// <summary>
        /// 停板时间
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime StopTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 出板时长（秒）
        /// </summary>
        public double OutDuration { get; set; } = 0.0;

        /// <summary>
        /// 出板时间
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime OutTime { get; set; } = DateTime.UtcNow;

        #endregion

        #region 序列化与反序列化

        /// <summary>
        /// 将当前实例序列化为 JSON 字节流
        /// </summary>
		public byte[] ToBytes() => JsonObjConvert.ToJSonBytes(this);

        /// <summary>
        /// 将当前实例序列化为标准 JSON 字符串
        /// </summary>
        public string ToJson() => JsonObjConvert.ToJSon(this);

        /// <summary>
        /// 从字节流中反序列化，并直接覆盖/填充到当前实例中
        /// </summary>
		public void FromBytes(byte[] data)
        {
            if (data is { Length: > 0 })
            {
                JsonObjConvert.PopulateObject(data, this);
            }
        }

        /// <summary>
        /// 从 JSON 字符串中反序列化，并直接覆盖/填充到当前实例中
        /// </summary>
        public void FromJson(string json)
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                JsonObjConvert.PopulateObject(json, this);
            }
        }

        #endregion
    }
}