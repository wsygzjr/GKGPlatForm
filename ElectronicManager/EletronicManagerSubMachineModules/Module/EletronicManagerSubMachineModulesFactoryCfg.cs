using GF_Gereric;
using Newtonsoft.JsonG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GKG.MotionControlFactoryParameters;


namespace GKG
{
	namespace SubMM
	{
		/// <summary>
		/// 出厂配置
		/// </summary>
		public class EletronicManagerSubMachineModulesFactoryCfg
		{
            /// <summary>
            /// 轴别名/轴配置（来自界面“轴配置”）
            /// </summary>
            public AxisInformation[]? AxisInformations { get; set; }

            /// <summary>
            /// IO别名/IO配置（来自界面“IO配置”）
            /// </summary>
            public IOStateInformation[]? IOStateInformations { get; set; }
            public EletronicFactoryParameters? EletronicFactoryParameters { get; set; }

            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="source">复制源</param>
            public void CopyFrom(EletronicManagerSubMachineModulesFactoryCfg source)
			{
                if (source == null)
                    return;

				this.AxisInformations = source.AxisInformations;
				this.IOStateInformations = source.IOStateInformations;
				this.EletronicFactoryParameters = source.EletronicFactoryParameters;
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
                if (data == null || data.Length == 0)
                    return;

                var cfg = JsonObjConvert.FromJSonBytes<EletronicManagerSubMachineModulesFactoryCfg>(data);
                if (cfg != null)
                    CopyFrom(cfg);
			}
		}
	}
}
