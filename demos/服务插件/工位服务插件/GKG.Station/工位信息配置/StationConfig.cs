using GF_Gereric;

namespace GKG.Station
{
    /// <summary>
    /// 工位配置
    /// </summary>
    public class StationConfig
    {
        /// <summary>
        /// 定义该配置在系统变量库中的唯一身份 Key
        /// </summary>
        public const string VarID = "GKG_Plugin_StationConfig_Var";

        /// <summary>
        /// 是否启用工位数据统计
        /// </summary>
        public bool EnableStatistics { get; set; } = true;

        /// <summary>
        /// 深拷贝配置数据 (防御性拷贝)
        /// </summary>
        /// <param name="source">复制源</param>
        public void CopyFrom(StationConfig source)
        {
            if (source == null) return;

            this.EnableStatistics = source.EnableStatistics;
        }

        /// <summary>
		/// 转为字节数组
		/// </summary>
		/// <returns>字节数组</returns>
        public byte[] ToBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }

        /// <summary>
		/// 从字节数组反序列化并填充当前对象
		/// </summary>
		/// <param name="data">字节数组</param>
        public void FromBytes(byte[] data)
        {
            if (data is { Length: > 0 })
                JsonObjConvert.PopulateObject(data, this);
        }
    }
}