using GF_Gereric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
	namespace SubMM
	{
		/// <summary>
		/// 初始化配置
		/// </summary>
		public class MotorPushRodSubMachineModulesInitCfg
		{
            /// <summary>推杆轴物理轴号（与 BasicRobot 逻辑轴 0 绑定，可被运行时 PusherPhysicalAxis 覆盖）。</summary>
            public Guid PusherPhysicalAxis { get; set; }

			/// <summary>
			/// 推杆超时时间
			/// </summary>
			public double PushRodTimeout { get; set; }

			/// <summary>
			/// 卡料信号检测时间
			/// </summary>
			public double MaterialJamDetectionTime { get; set; }

            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="source">复制源</param>
            public void CopyFrom(MotorPushRodSubMachineModulesInitCfg source)
			{
                this.PusherPhysicalAxis = source.PusherPhysicalAxis;
				this.PushRodTimeout = source.PushRodTimeout;
				this.MaterialJamDetectionTime = source.MaterialJamDetectionTime;
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
