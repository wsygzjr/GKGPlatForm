using GF_Gereric;
using RailMotorSubMachineModules.Properties;
using System;

namespace GKG
{
	namespace SubMM
	{
		/// <summary>
		/// 配方配置
		/// </summary>
		public class RailMotorSubMachineModulesPPCfg
		{

			/// <summary>
			/// 复制
			/// </summary>
			/// <param name="source">复制源</param>
			public void CopyFrom(RailMotorSubMachineModulesPPCfg source)
			{
				if (source == null)
					throw new ArgumentNullException(nameof(source), Resources.RailMotorPPCfgCopySourceNull);

				JsonObjConvert.PopulateObject(source.ToBytes(), this);
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
}
