using GF_Gereric;
using RailAdjustWidthSubMachineModules.Properties;
using System;

namespace GKG
{
	namespace SubMM
	{
		public class RailAdjustWidthSubMachineModulesFactoryCfg
		{
			/// <summary>
			/// 前轨是否可移动，若可移动则需要绑定轴向输入，若不可移动则需要绑定固定轨道ID和位置
			/// </summary>
			public bool FrontRailIsMovable { get; set; } = true;

            /// <summary>
            /// 后轨是否可移动，若可移动则需要绑定轴向输入，若不可移动则需要绑定固定轨道ID和位置
            /// </summary>
            public bool BackRailIsMovable { get; set; } = true;
            /// <summary>
            /// 定轨ID
            /// </summary>
            public RailAdjustWidthAxis FixRailID { get; set; } = RailAdjustWidthAxis.FrontRail;
			/// <summary>
			/// 定轨位置
			/// </summary>
			public double FixRailPosition { get; set; } = 0;
			/// <summary>
			/// 最大宽度
			/// </summary>
			public double MaxWidth { get; set; } = 0;
			/// <summary>
			/// 最小宽度
			/// </summary>
			public double MinWidth { get; set; } = 0;

            public void CopyFrom(RailAdjustWidthSubMachineModulesFactoryCfg source)
			{
                if (source == null)
                    throw new ArgumentNullException(nameof(source), Resources.RailAdjustWidthFactoryCfgCopySourceNull);

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
