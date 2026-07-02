using GF_Gereric;
using RailAdjustWidthSubMachineModules.Properties;
using System;

namespace GKG
{
	namespace SubMM
	{
		public class RailAdjustWidthSubMachineModulesPPCfg
		{
			/// <summary>
			/// 固定轨道的ID，position为固定轨道的位置，单位为mm，若前轨或后轨不可移动则需要绑定一个固定轨道的ID和位置，用于确定调整宽度的基准点和范围，确保调整后的宽度符合要求并且不会发生碰撞
			/// </summary>
			public RailAdjustWidthAxis? FixRailID { get; set; } = RailAdjustWidthAxis.FrontRail;

            /// <summary>
            /// 固定轨道的位置，单位为mm，若前轨或后轨不可移动则需要绑定一个固定轨道的ID和位置，用于确定调整宽度的基准点和范围，确保调整后的宽度符合要求并且不会发生碰撞
            /// </summary>
            public double? FixRailPosition { get; set; } = 0;

            /// <summary>
            /// 板宽，单位为mm，表示当前轨道的宽度，数值越大表示宽度越宽，数值越小表示宽度越窄，根据实际情况进行设置和调整，确保轨道的宽度符合要求并且能够满足使用需求，同时需要注意板宽的合理范围，避免设置过大或过小导致安全隐患
            /// </summary>
			/// 

            public double Width { get; set; } = 0;

            /// <summary>
            /// 调宽速度，单位为mm/s，表示轨道调整宽度的速度，数值越大调整越快，但可能会增加碰撞风险，建议根据实际情况进行设置
            /// </summary>
            public int AdjustWidthSpeed { get; set; } = 20;
            /// <summary>
            /// 加速度
            /// </summary>
            public int AdjustWidthAcceleration { get; set; } = 200;
            public void CopyFrom(RailAdjustWidthSubMachineModulesPPCfg source)
			{
                if (source == null)
                    throw new ArgumentNullException(nameof(source), Resources.RailAdjustWidthPPCfgCopySourceNull);

                JsonObjConvert.PopulateObject(source.ToBytes(), this);
			}

			public byte[] ToBytes()
			{
				return JsonObjConvert.ToJSonBytes(this);
			}

			public void FromBytes(byte[] data)
			{
				if (data != null && data.Length > 0)
					JsonObjConvert.PopulateObject(data, this);
			}
		}
	}
}
