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
		public class CylinderPushRodSubMachineModulesInitCfg
		{

            public CylinderInitParameters CylinderInitParameters { get; set; } = new CylinderInitParameters();

			/// <summary>
			/// 推料气缸电磁阀升降延时
			/// </summary>
			public double PushCylinderSolenoidValveLiftDelay { get; set; }

            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="source">复制源</param>
            public void CopyFrom(CylinderPushRodSubMachineModulesInitCfg source)
			{
                this.CylinderInitParameters = source.CylinderInitParameters;
				this.PushCylinderSolenoidValveLiftDelay = source.PushCylinderSolenoidValveLiftDelay;
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
