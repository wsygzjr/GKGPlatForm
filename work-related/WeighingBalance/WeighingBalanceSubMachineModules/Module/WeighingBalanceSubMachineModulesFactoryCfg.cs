using GF_Gereric;

namespace GKG.SubMM.Dispenser
{

    /// <summary>
    /// 出厂配置
    /// </summary>
    public class WeighingBalanceSubMachineModulesFactoryCfg
	{
        /// <summary>
        /// 称重天平品牌型号
        /// </summary>
        public WeighingBalanceType WeighingBalanceType { get; set; } = WeighingBalanceType.APW;
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">复制源</param>
        public void CopyFrom(WeighingBalanceSubMachineModulesFactoryCfg source)
		{
			if(source == null)
				return;
            this.WeighingBalanceType = source.WeighingBalanceType;
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
