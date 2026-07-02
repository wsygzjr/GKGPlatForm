using GF_Gereric;

namespace GKG.SubMM
{
    /// <summary>
    /// 配方配置
    /// </summary>
    public class MotionCalculateSubMachineModulesPPCfg
    {
		/// <summary>
		/// 参数1
		/// </summary>
		public string Param1 { get; set; } = "配方参数1";
		/// <summary>
		/// 参数2
		/// </summary>
		public bool Param2 { get; set; } = false;
		/// <summary>
		/// 参数3
		/// </summary>
		public int Param3 { get; set; } = 0;
		/// <summary>
		/// 参数4
		/// </summary>
		public string Param4 { get; set; } = "配方参数4";
		/// <summary>
		/// 参数5
		/// </summary>
		public string Param5 { get; set; } = "配方参数5";
		/// <summary>
		/// 复制
		/// </summary>
		/// <param name="source">复制源</param>
		public void CopyFrom(MotionCalculateSubMachineModulesPPCfg source)
		{
			this.Param1 = source.Param1;
			this.Param2 = source.Param2;
			this.Param3 = source.Param3;
			this.Param4 = source.Param4;
			this.Param5 = source.Param5;
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
