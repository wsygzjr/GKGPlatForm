using GF_Gereric;
using Newtonsoft.JsonG;
using Newtonsoft.JsonG.Converters;

namespace GKG.Log
{
    /// <summary>
    /// 日志上报实体 (DTO)
    /// </summary>
    /// <remarks>
    /// 用于承载底层设备或模块向上层应用服务传输的日志数据
    /// </remarks>
    public class LogUpInfo
    {
        #region 核心属性

        /// <summary>
        /// 日志产生精确时间戳
        /// </summary>
        /// <remarks>作为反序列化缺失时的兜底，强制采用 UTC</remarks>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime UpdateTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 日志级别
        /// </summary>
        /// <remarks>架构防御：默认赋 Info，防止设备端漏传导致重要日志被网关意外过滤</remarks>
        public LogLevel LogLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// 日志来源模组别名
        /// </summary>
        public string ModuleAlias { get; set; } = string.Empty;

        /// <summary>
        /// 日志来源线程ID
        /// </summary>
        public int ThreadID { get; set; } = 0;

        /// <summary>
        /// 日志错误码
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// 日志正文文本
        /// </summary>
        public string LogText { get; set; } = string.Empty;

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