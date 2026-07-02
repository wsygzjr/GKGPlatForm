using GF_Gereric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
	namespace MM
	{
		/// <summary>
		/// 初始化配置
		/// </summary>
		public class FixingStructureMachineModulesInitCfg
		{
            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="source">复制源</param>
            public void CopyFrom(FixingStructureMachineModulesInitCfg source)
			{

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
