using GF_Gereric;
using RailMotorSubMachineModules.Properties;
using System;

namespace GKG
{
	namespace SubMM
	{
		/// <summary>
		/// 初始化配置
		/// </summary>
		public class RailMotorSubMachineModulesInitCfg
		{
            /// <summary>
            /// 电机运动模式
            /// </summary>
            public ERailMotionMode RailMotionMode { get; set; } = ERailMotionMode.ExclusiveMode;

            /// <summary>
            /// 运输电机绑定对象ID
            /// </summary>
            public Guid AxisBindingObjID { get; set; } = Guid.Empty;

			/// <summary>
			/// 复制
			/// </summary>
			/// <param name="source">复制源</param>
			public void CopyFrom(RailMotorSubMachineModulesInitCfg source)
			{
				if (source == null)
					throw new ArgumentNullException(nameof(source), Resources.RailMotorInitCfgCopySourceNull);

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
