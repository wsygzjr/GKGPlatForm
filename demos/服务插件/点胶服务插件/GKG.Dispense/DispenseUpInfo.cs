using GF_Gereric;
using Newtonsoft.JsonG;
using Newtonsoft.JsonG.Converters;

namespace GKG.Dispense
{
    /// <summary>
    /// 点胶信息上报实体 (DTO)
    /// </summary>
    /// <remarks>
    /// 用于承载底层设备或模块向上层应用服务传输的工位数据
    /// </remarks>
    public class DispenseUpInfo
    {
        #region 核心属性

        /// <summary>
        /// 模组实例别名
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
        /// Mark识别时间
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime MarkTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Mark识别时长
        /// </summary>
        public double MarkDuration { get; set; } = 0.0;

        /// <summary>
        /// 点胶时间
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DispenseTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 点胶时长
        /// </summary>
        public double DispenseDuration { get; set; } = 0.0;

        /// <summary>
        /// 处理时长（单板时间）
        /// </summary>
        public double ProcessDuration { get; set; } = 0.0;

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