using GF_Gereric;
using System;

namespace GKG
{
	namespace SubMM
	{
		/// <summary>
		/// 初始化配置
		/// </summary>
		public class RailWorkStationSubMachineModulesInitCfg
		{
			/// <summary>
			/// 工位电气初始化参数结构（左右挡板气缸初始化参数、左/右/接近感应器 ID）
			/// </summary>
			public WorkStationEleInitParams WorkStationEleInitParams { get; set; } = new WorkStationEleInitParams();

			/// <summary>
			/// 工位运输速度档位列表（档位 ID 与运输速度映射）
			/// </summary>
			public WorkStationTransSpeedGearList WorkStationTransSpeedGearList { get; set; } = new WorkStationTransSpeedGearList();

			/// <summary>
			/// 复制
			/// </summary>
			/// <param name="source">复制源</param>
			public void CopyFrom(RailWorkStationSubMachineModulesInitCfg source)
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
