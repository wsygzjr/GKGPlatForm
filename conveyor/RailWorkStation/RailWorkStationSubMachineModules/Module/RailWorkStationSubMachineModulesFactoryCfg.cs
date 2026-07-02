using GF_Gereric;
using System;

namespace GKG
{
	namespace SubMM
	{
		/// <summary>
		/// 出厂配置
		/// </summary>
		public class RailWorkStationSubMachineModulesFactoryCfg
		{
			/// <summary>
			/// 工位电气配置参数结构（左感应器、右感应器、接近感应器、左挡板、右挡板）
			/// </summary>
			public WorkStationEleConfigParams WorkStationEleConfigParams { get; set; } = new WorkStationEleConfigParams();

			/// <summary>
			/// 工位能力数据结构（左进、左出、右进、右出、轨道工作模式）
			/// </summary>
			public WorkStationCapability WorkStationCapability { get; set; } = new WorkStationCapability();

			/// <summary>
			/// 复制
			/// </summary>
			/// <param name="source">复制源</param>
			public void CopyFrom(RailWorkStationSubMachineModulesFactoryCfg source)
			{
				if (source == null)
					throw new ArgumentNullException(nameof(source));

				JsonObjConvert.PopulateObject(source.ToBytes(), this);
			}

			/// <summary>
			/// 转为字节数组
			/// </summary>
			public byte[] ToBytes()
			{
				return JsonObjConvert.ToJSonBytes(this);
			}

			/// <summary>
			/// 从字节数组转换
			/// </summary>
			public void FromBytes(byte[] data)
			{
				if (data != null && data.Length > 0)
					JsonObjConvert.PopulateObject(data, this);
			}
		}
	}
}
