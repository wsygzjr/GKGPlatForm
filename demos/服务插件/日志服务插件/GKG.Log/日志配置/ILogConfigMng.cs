namespace GKG.Log
{
    /// <summary>
    /// 日志配置管理接口
    /// </summary>
    public interface ILogConfigMng
    {
        /// <summary>
        /// 读取日志配置信息
        /// </summary>
        /// <returns>日志配置信息</returns>
        LogConfig ReadLogConfig();

        /// <summary>
        /// 写日志配置信息
        /// </summary>
        /// <param name="logConfig">日志配置信息</param>
        void WriteLogConfig(LogConfig logConfig);
    }
}
