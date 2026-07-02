using GF_Gereric;
using System;

namespace GKG
{
	namespace SubMM
	{
		/// <summary>
		/// 配方配置
		/// </summary>
		public class RailWorkStationSubMachineModulesPPCfg
		{
			/// <summary>
			/// 工位运输速度（选择运输速度档位 ID）
			/// </summary>
			public WorkStationTransSpeed WorkStationTransSpeed { get; set; } = new WorkStationTransSpeed();
            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="source">复制源</param>
            public void CopyFrom(RailWorkStationSubMachineModulesPPCfg source)
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
