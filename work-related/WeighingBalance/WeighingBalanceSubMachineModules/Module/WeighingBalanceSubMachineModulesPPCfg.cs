using GF_Gereric;

namespace GKG.SubMM.Dispenser
{
    /// <summary>
    /// 配方配置
    /// </summary>
    public class WeighingBalanceSubMachineModulesPPCfg
    {
        /// <summary>
        /// 称重参数
        /// </summary>
        public WeighingParameterArray WeighingParameters { get; set; }
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">复制源</param>
        public void CopyFrom(WeighingBalanceSubMachineModulesPPCfg source)
        {
            if (source != null)
            {
                WeighingParameters = source.WeighingParameters;
            }
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
        /// 从字节数组转换
        /// </summary>
        /// <param name="data">字节数组</param>
        public void FromBytes(byte[] data)
        {
            if (data != null && data.Length > 0)
                JsonObjConvert.PopulateObject(data, this);
        }
    }
}