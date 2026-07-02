using System.Collections.Generic;
using GF_Gereric;

namespace GKG.Log
{
    /// <summary>
    /// 日志配置
    /// </summary>
    public class LogConfig
    {
        /// <summary>
        /// 定义该配置在系统变量库中的唯一身份 Key
        /// </summary>
        public const string VarID = "GKG_Plugin_LogConfig_Var";

        /// <summary>
        /// 最小日志记录阈值
        /// </summary>
        /// <remarks>
        /// 过滤规则：任何严重性【低于】此阈值的日志将被网关直接丢弃
        /// 严重性阶梯：Trace(1) < Debug(2) < Info(3) < Warn(4) < Error(5) < Fatal(6)
        /// 默认保留 Info 及以上级别的核心业务流水，自动过滤 Trace/Debug 等高频底噪
        /// </remarks>
        public LogLevel MinRecordLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// 黑名单实例别名 (在这里面的实例，不记录任何日志)
        /// </summary>
        public List<string> IgnoreInstances { get; set; } = new();

        /// <summary>
        /// 垃圾关键字屏蔽 (包含这些词的日志直接丢弃)
        /// </summary>
        public List<string> IgnoreKeyWords { get; set; } = new();

        /// <summary>
        /// 深拷贝配置数据 (防御性拷贝)
        /// </summary>
        /// <param name="source">复制源</param>
        public void CopyFrom(LogConfig source)
        {
            if (source == null) return;

            this.MinRecordLevel = source.MinRecordLevel;

            this.IgnoreInstances.Clear();
            if (source.IgnoreInstances is { Count: > 0 })
            {
                this.IgnoreInstances.AddRange(source.IgnoreInstances);
            }

            this.IgnoreKeyWords.Clear();
            if (source.IgnoreKeyWords is { Count: > 0 })
            {
                this.IgnoreKeyWords.AddRange(source.IgnoreKeyWords);
            }
        }

        /// <summary>
        /// 转为字节数组
        /// </summary>
        /// <returns>字节数组</returns>
        public byte[] ToBytes() => JsonObjConvert.ToJSonBytes(this);

        /// <summary>
        /// 从字节数组反序列化并填充当前对象
        /// </summary>
        /// <param name="data">字节数组</param>
        public void FromBytes(byte[] data)
        {
            if (data is { Length: > 0 })
            {
                this.IgnoreInstances.Clear();
                this.IgnoreKeyWords.Clear();

                JsonObjConvert.PopulateObject(data, this);
            }
        }
    }
}