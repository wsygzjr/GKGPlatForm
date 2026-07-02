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
		/// 配方配置
		/// </summary>
		public class MotorPushRodSubMachineModulesPPCfg
		{
			/// <summary>
			/// 推料距离
			/// </summary>
			public double PushDistance { get; set; }

			
			/// <summary>
			/// 推杆轴速度
			/// </summary>
			public double PushAxisSpeed { get; set; }	

			/// <summary>
			/// 推杆轴加速度
			/// </summary>
			public double PushAxisAcceleration { get; set; }	
            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="source">复制源</param>
            public void CopyFrom(MotorPushRodSubMachineModulesPPCfg source)
			{
				this.PushDistance = source.PushDistance;
				this.PushAxisSpeed = source.PushAxisSpeed;
				this.PushAxisAcceleration = source.PushAxisAcceleration;
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
