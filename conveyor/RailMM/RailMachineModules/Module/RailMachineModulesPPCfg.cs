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
		public enum EInletPanelWorkStation
		{
			/// <summary>
			/// 进板到待料位
			/// </summary>
			StandBy,
			/// <summary>
			/// 进板到工作位
			/// </summary>
			Working
		}

        public enum EOutletPanelWorkStation
        {
            /// <summary>
            /// 出板位出板
            /// </summary>
            Out,
            /// <summary>
            /// 工作位直接出板
            /// </summary>
            Working
        }

        /// <summary>
        /// 配方配置
        /// </summary>
        public class RailMachineModulesPPCfg
        {
			/// <summary>
			/// 轨道工作模式
			/// </summary>
			public ERailWorkMode TransferDirection { get; set; } = ERailWorkMode.LeftInRightOut;

			/// <summary>
			/// 进板目标位
			/// </summary>
			public EInletPanelWorkStation InletPanelWorkStation { get; set; } = EInletPanelWorkStation.Working;

			/// <summary>
			/// 出板目标位
			/// </summary>
			public EOutletPanelWorkStation OutletPanelWorkStation { get; set; } = EOutletPanelWorkStation.Working;

			/// <summary>
			/// 出板延时
			/// </summary>
			public int OutletPanelDelayTime { get; set; } = 0;
            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="source">复制源</param>
            public void CopyFrom(RailMachineModulesPPCfg source)
			{
				this.TransferDirection = source.TransferDirection;
				this.InletPanelWorkStation = source.InletPanelWorkStation;
				this.OutletPanelWorkStation = source.OutletPanelWorkStation;
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
